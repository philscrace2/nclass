﻿using System;
using System.Collections.Generic;
using NClass.Core;

using System.Text.RegularExpressions;
using System.Linq;

using System.IO;
using System.Text;
using DatabaseSchemaReader.DataSchema;

using DatabaseSchemaReader.SqlGen;

namespace NClass.CodeGenerator
{
    internal sealed class CSharpSQLSourceFileGenerator 
        : SourceFileGenerator
    {
        bool useLowercaseUnderscored;

        DatabaseSchema schema;

        public CSharpSQLSourceFileGenerator
            (string rootNamespace, Model model)
            : base(rootNamespace, model)
        {}

        protected override string Extension
        {
            get { return ".sql"; }
        }

        protected override void WriteFileContent()
        {
            useLowercaseUnderscored = Settings.Default.UseUnderscoreAndLowercaseInDB;
            var sqlToServerType = Settings.Default.SQLToServerType;

            schema = new DatabaseSchema(null, sqlToServerType);

            foreach (IEntity entity in Model.Entities.Where(e => e is ClassType).ToList<IEntity>())
            {
                if(((ClassType)entity).Operations.Where(x => x is Property).Count() > 0)
                    CreateTable((ClassType)entity);
            }

            var ddlTables = new DdlGeneratorFactory(sqlToServerType).AllTablesGenerator(schema);
            ddlTables.IncludeSchema = false;

            Write(ddlTables.Write());
        }

        private DatabaseTable CreateTable(ClassType _class)
        {
            var name = PrefixedText(
                useLowercaseUnderscored
                ? LowercaseAndUnderscoredWord(_class.Name)
                : _class.Name
                );

            DatabaseTable table = schema.FindTableByName(name);

            if (table != null)
                return table;

            List<Operation> keys = _class.Operations.Where(o => o is Property && o.IsIdentity).ToList<Operation>();

            table = schema.AddTable(name);

            foreach(var key in keys)
            {
                if (!string.IsNullOrEmpty(key.ManyToOne))
                {
                    table.AddColumn(
                        useLowercaseUnderscored
                        ? LowercaseAndUnderscoredWord(key.Name)
                        : key.Name,
                        GetTypeFromString(key.Type))
                        .AddForeignKey(CreateTable(GetClassByName(key.Type)).Name)
                        .AddLength(255)
                        .AddPrimaryKey();
                }
                else
                {
                    table.AddColumn(
                        useLowercaseUnderscored
                        ? LowercaseAndUnderscoredWord(key.Name)
                        : key.Name,
                        GetTypeFromString(key.Type))
                        .AddLength(255)
                        .AddPrimaryKey();
                }
            }

            foreach (var property in _class.Operations.Where(o => o is Property && !o.IsIdentity).ToList<Operation>())
            {
                try
                {
                    CreateColumn(ref table, (Property)property);
                }
                catch (ArgumentException e)
                {
                    System.Windows.Forms.MessageBox.Show(
                        string.Format("{0}, {1} - {2}", _class.Name, property.Name, e.Message),
                        "Error",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Error);
                    throw;
                }
            }

            return table;
        }

        private void CreateColumn(ref DatabaseTable table, Property property)
        {
            if (!string.IsNullOrEmpty(property.ManyToOne))
            {
                var column = table.AddColumn(
                                useLowercaseUnderscored
                                ? LowercaseAndUnderscoredWord(property.Name)
                                : property.Name,
                                GetTypeFromString(property.Type))
                                .AddLength(255)
                                .AddForeignKey(CreateTable(GetClassByName(property.Type)).Name);

                if (property.IsUnique)
                    column.AddUniqueKey();
            }
            else
            {
                var column = table.AddColumn(
                                useLowercaseUnderscored
                                ? LowercaseAndUnderscoredWord(property.Name)
                                : property.Name,
                                GetTypeFromString(property.Type))
                                .AddPrecisionScale(18, 5)
                                .AddLength(255);

                if (property.IsUnique)
                    column.AddUniqueKey();
            }
        }

        private ClassType GetClassByName(string className)
        {
            return Model.Entities.SingleOrDefault(x => x is ClassType && x.Name == className) as ClassType;
        }

        public override string Generate(string directory)
        {
            try
            {
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                string fileName = "script" + Extension;
                string path = Path.Combine(directory, fileName);

                using (StreamWriter writer = new StreamWriter(path, false, Encoding.Default))
                {
                    WriteFileContent(writer);
                }

                return fileName;
            }
            catch (Exception ex)
            {
                throw new FileGenerationException(directory, ex);
            }
        }

        private Type GetTypeFromString(string _type)
        {
            Type t = null;

            Dictionary<string, Type> types = new Dictionary<string, Type>
            {
                { "bool", typeof(bool) },
                { "byte", typeof(byte) },
                { "sbyte", typeof(short) },
                { "SByte", typeof(short) },
                { "System.SByte", typeof(short) },
                { "byte[]", typeof(byte[]) },
                { "char", typeof(char) },
                { "decimal", typeof(decimal) },
                { "double", typeof(double) },
                { "float", typeof(float) },
                { "int", typeof(int) },
                { "uint", typeof(long) },
                { "UInt32", typeof(long) },
                { "System.UInt32", typeof(long) },
                { "long", typeof(long) },
                { "ulong", typeof(decimal) },
                { "UInt64", typeof(decimal) },
                { "System.UInt64", typeof(decimal) },
                { "short", typeof(short) },
                { "ushort", typeof(int) },
                { "UInt16", typeof(int) },
                { "System.UInt16", typeof(int) },
                { "string", typeof(string) },
                { "object", typeof(object) }
            };

            if (Model.Entities.Where(e => e.Name == _type).Count() > 0)
            {
                var _class = Model.Entities.SingleOrDefault(x => x is ClassType && x.Name == _type) as ClassType;
                var properties = _class.Operations.Where(x => x is Property).ToList();
                _type = properties[0].Type;
            }
            
            if (types.ContainsKey(_type))
                t = types[_type];
            else if ((new Regex("^System", RegexOptions.IgnoreCase).IsMatch(_type)))
                t = System.Type.GetType(_type);
            else
                t = System.Type.GetType("System." + _type);

            if (t == null) 
                throw new ArgumentException(string.Format("Cannot map type \"{0}\" to a database type.", _type));

            return t;
        }
    }
}
