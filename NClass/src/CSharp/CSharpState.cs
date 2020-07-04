using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using NClass.Core;

namespace NClass.CSharp
{
    public class CSharpState : StateType
    {
        internal CSharpState() : this("NewState")
        {
        }

        internal CSharpState(string name) : base(name)
        {
        }        

        public override string Name { get => base.Name; set => base.Name = value; }
        public override AccessModifier AccessModifier { get => base.AccessModifier; set => base.AccessModifier = value; }        
        public override ClassModifier Modifier { get => base.Modifier; set => base.Modifier = value; }        

        public override string Stereotype { get => base.Stereotype; set => base.Stereotype = value; }
        
        public override ClassType BaseClass { get => base.BaseClass; set => base.BaseClass = value; }       

        public override string NHMTableName { get => base.NHMTableName; set => base.NHMTableName = value; }
        public override string IdGenerator { get => base.IdGenerator; set => base.IdGenerator = value; }
        public override string GeneratorParameters { get => base.GeneratorParameters; set => base.GeneratorParameters = value; }        

        public override Language Language
        {
            get { return CSharpLanguage.Instance; }
        }        

        public override void Clean()
        {
            base.Clean();
        }

        public override StateType Clone()
        {
            CSharpState newState = new CSharpState();
            newState.CopyFrom(this);
            return newState;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }       

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        protected override void CopyFrom(StateBase type)
        {
            base.CopyFrom(type);
        }

        protected override void Deserialize(XmlElement node)
        {
            base.Deserialize(node);
        }

        protected override void Serialize(XmlElement node)
        {
            base.Serialize(node);
        }

        public override string GetDeclaration()
        {
            StringBuilder builder = new StringBuilder();
            //TODO
            //if (AccessModifier != AccessModifier.Default)
            //{
            //    builder.Append(Language.GetAccessString(AccessModifier, true));
            //    builder.Append(" ");
            //}
            //if (Modifier != ClassModifier.None)
            //{
            //    builder.Append(Language.GetClassModifierString(Modifier, true));
            //    builder.Append(" ");
            //}
            //builder.AppendFormat("class {0}", Name);

            //if (HasExplicitBase || InterfaceList.Count > 0)
            //{
            //    builder.Append(" : ");
            //    if (HasExplicitBase)
            //    {
            //        builder.Append(BaseClass.Name);
            //        if (InterfaceList.Count > 0)
            //            builder.Append(", ");
            //    }
            //    for (int i = 0; i < InterfaceList.Count; i++)
            //    {
            //        builder.Append(InterfaceList[i].Name);
            //        if (i < InterfaceList.Count - 1)
            //            builder.Append(", ");
            //    }
            //}

            return builder.ToString();
        }
    }
}
