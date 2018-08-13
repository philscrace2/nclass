﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using NClass.Core;
using NClass.CSharp;
using System.Xml;

using System.Linq;

namespace NClass.CodeGenerator
{
    internal sealed class CSharpNHibernateXmlSourceFileGenerator 
        : SourceFileGenerator
    {
        bool useLazyLoading;
        bool useLowercaseUnderscored;
        string idGeneratorType;

		/// <exception cref="NullReferenceException">
		/// <paramref name="type"/> is null.
		/// </exception>
        public CSharpNHibernateXmlSourceFileGenerator
            (TypeBase type, string rootNamespace, Model model)
			: base(type, rootNamespace, model)
		{}

        protected override string Extension
        {
            get { return ".hbm.xml"; }
        }

        protected override void WriteFileContent()
        {
            useLazyLoading = Settings.Default.DefaultLazyFetching;
            useLowercaseUnderscored = Settings.Default.UseUnderscoreAndLowercaseInDB;

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            settings.Encoding = System.Text.Encoding.Unicode;

            ClassType _class = (ClassType)Type;
            
            if(_class.IdGenerator == null)
                idGeneratorType = EnumExtensions.GetDescription(Settings.Default.DefaultIdGenerator);
            else
                idGeneratorType = EnumExtensions.GetDescription((IdGeneratorType)Enum.Parse(typeof(IdGeneratorType), _class.IdGenerator));
            
            using (XmlWriter xml = XmlWriter.Create(CodeBuilder, settings))
            {
                xml.WriteStartDocument();
                xml.WriteComment(
                    string.Format(
                    " This code was generated by {0} ", 
                    GetVersionString()
                    ));
                xml.WriteStartElement("hibernate-mapping", "urn:nhibernate-mapping-2.2");
                xml.WriteAttributeString("assembly", ProjectName);
                xml.WriteAttributeString("namespace", RootNamespace);
                xml.WriteStartElement("class");
                xml.WriteAttributeString("name", _class.Name);
                xml.WriteAttributeString("table", 
                    string.Format("`{0}`",
                    PrefixedText(
                        useLowercaseUnderscored 
                        ? LowercaseAndUnderscoredWord(_class.Name) 
                        : string.IsNullOrEmpty(_class.NHMTableName)
                        ? _class.Name
                        : _class.NHMTableName
                    )));
                xml.WriteAttributeString("lazy", useLazyLoading.ToString().ToLower());

                List<Operation> ids = _class.Operations.Where(o => o is Property && o.IsPrimaryKey).ToList<Operation>();

                if(ids.Count > 1)
                {
                    xml.WriteStartElement("composite-id");
                    foreach (var id in ids)
                    {
                        if(Model.Entities.Where(e => e.Name == id.Type).Count() > 0)
                        {
                            xml.WriteStartElement("key-many-to-one");
                            xml.WriteAttributeString("name", id.Name);
                            xml.WriteAttributeString("column",
                                string.Format("`{0}`",
                                    useLowercaseUnderscored
                                    ? LowercaseAndUnderscoredWord(id.Name)
                                    : string.IsNullOrEmpty(id.NHMColumnName)
                                    ? id.Name
                                    : id.NHMColumnName
                                ));
                            xml.WriteAttributeString("class", id.Type);
                            xml.WriteEndElement();
                        }
                        else
                        {
                            xml.WriteStartElement("key-property");
                            xml.WriteAttributeString("name", id.Name);
                            xml.WriteAttributeString("column",
                                string.Format("`{0}`",
                                    useLowercaseUnderscored
                                    ? LowercaseAndUnderscoredWord(id.Name)
                                    : string.IsNullOrEmpty(id.NHMColumnName)
                                    ? id.Name
                                    : id.NHMColumnName
                                ));
                            xml.WriteAttributeString("type", id.Type);
                            xml.WriteEndElement();
                        }
                    }
                    xml.WriteEndElement();
                }
                else if (ids.Count == 1)
                {
                    xml.WriteStartElement("id");
                    xml.WriteAttributeString("name", ids[0].Name);
                    xml.WriteAttributeString("column",
                        string.Format("`{0}`",
                            useLowercaseUnderscored
                            ? LowercaseAndUnderscoredWord(ids[0].Name)
                            : string.IsNullOrEmpty(ids[0].NHMColumnName)
                            ? ids[0].Name
                            : ids[0].NHMColumnName
                        ));
                    xml.WriteAttributeString("type", ids[0].Type);
                    xml.WriteAttributeString("generator", idGeneratorType);
                    xml.WriteEndElement();
                }

                foreach (var property in _class.Operations.Where(o => o is Property && !o.IsPrimaryKey).ToList<Operation>())
                {
                    if (Model.Entities.Where(e => e.Name == property.Type).Count() > 0)
                    {
                        xml.WriteStartElement("many-to-one");
                        xml.WriteAttributeString("name", property.Name);
                        xml.WriteAttributeString("class", property.Type);
                        xml.WriteAttributeString("column",
                            string.Format("`{0}`",
                                useLowercaseUnderscored
                                ? LowercaseAndUnderscoredWord(property.Name)
                                : string.IsNullOrEmpty(property.NHMColumnName)
                                ? property.Name
                                : property.NHMColumnName
                            ));
                        xml.WriteAttributeString("unique", property.IsUnique.ToString().ToLower());
                        xml.WriteAttributeString("not-null", property.IsNotNull.ToString().ToLower());
                        xml.WriteEndElement();
                    }
                    else
                    {
                        xml.WriteStartElement("property");
                        xml.WriteAttributeString("name", property.Name);
                        xml.WriteAttributeString("column",
                            string.Format("`{0}`",
                                useLowercaseUnderscored
                                ? LowercaseAndUnderscoredWord(property.Name)
                                : string.IsNullOrEmpty(property.NHMColumnName)
                                ? property.Name
                                : property.NHMColumnName
                            ));
                        xml.WriteAttributeString("type", property.Type);
                        xml.WriteAttributeString("unique", property.IsUnique.ToString().ToLower());
                        xml.WriteAttributeString("not-null", property.IsNotNull.ToString().ToLower());
                        xml.WriteEndElement();
                    }
                }

                xml.WriteEndElement();
                xml.WriteEndElement();
                xml.WriteEndDocument();
            }
        }
    }
}
