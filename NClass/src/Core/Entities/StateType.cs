using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NClass.Core.Entities
{
    public class StateType : ClassType
    {
        /// <exception cref="BadSyntaxException">
		/// The <paramref name="name"/> does not fit to the syntax.
		/// </exception>
		protected StateType(String name) : base(name)
        {
        }

        public override bool SupportsDestructors => throw new NotImplementedException();

        public override bool SupportsProperties => throw new NotImplementedException();

        public override bool SupportsEvents => throw new NotImplementedException();

        public override AccessModifier DefaultMemberAccess => throw new NotImplementedException();

        public override AccessModifier DefaultAccess => throw new NotImplementedException();

        public override Language Language => throw new NotImplementedException();

        public override Constructor AddConstructor()
        {
            throw new NotImplementedException();
        }

        public override Destructor AddDestructor()
        {
            throw new NotImplementedException();
        }

        public override Event AddEvent()
        {
            throw new NotImplementedException();
        }

        public override Field AddField()
        {
            throw new NotImplementedException();
        }

        public override Method AddMethod()
        {
            throw new NotImplementedException();
        }

        public override Property AddProperty()
        {
            throw new NotImplementedException();
        }

        public override ClassType Clone()
        {
            throw new NotImplementedException();
        }

        public override string GetDeclaration()
        {
            throw new NotImplementedException();
        }
    }
}
