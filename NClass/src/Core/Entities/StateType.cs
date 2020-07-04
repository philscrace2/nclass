using System;
using System.Collections.Generic;
using System.Xml;
using NClass.Translations; 
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NClass.Core
{
    public abstract class StateType : StateBase

    {
		ClassModifier modifier = ClassModifier.None;
		ClassType baseClass = null;
		int derivedClassCount = 0;

		/// <exception cref="BadSyntaxException">
		/// The <paramref name="name"/> does not fit to the syntax.
		/// </exception>
		protected StateType(String name) : base(name)
		{
		}

		public sealed override EntityType EntityType
		{
			get { return EntityType.Class; }
		}

		/// <exception cref="BadSyntaxException">
		/// The <paramref name="value"/> does not fit to the syntax.
		/// </exception>
		public virtual ClassModifier Modifier
		{
			get
			{
				return modifier;
			}
			set
			{
				if (modifier != value)
				{
					if (value == ClassModifier.Static && (IsSuperClass)) //TODO || HasExplicitBase))
						throw new BadSyntaxException(Strings.ErrorInvalidModifier);
					if (value == ClassModifier.Sealed && IsSuperClass)
						throw new BadSyntaxException(Strings.ErrorInvalidModifier);

					modifier = value;
					Changed();
				}
			}
		}		

		public bool IsSuperClass
		{
			get { return (derivedClassCount > 0); }
		}

		public sealed override string Signature
		{
			get
			{
				string accessString = Language.GetAccessString(Access, false);
				string modifierString = Language.GetClassModifierString(Modifier, false);

				if (Modifier == ClassModifier.None)
					return string.Format("{0} Class", accessString);
				else
					return string.Format("{0} {1} Class", accessString, modifierString);
			}
		}

		private string stereotype = null;

		/// <summary>
		/// Gets or sets the stereotype for this class.
		/// </summary>
		public override string Stereotype
		{
			get
			{
				return stereotype;
			}
			set
			{
				if (value != stereotype)
				{
					stereotype = value;
					// Make sure the element knows that the stereotype has changed.
					Changed();
				}
			}
		}		

		/// <exception cref="RelationshipException">
		/// The language of <paramref name="value"/> does not equal.-or-
		/// <paramref name="value"/> is static or sealed class.-or-
		/// The <paramref name="value"/> is descendant of the class.
		/// </exception>
		public virtual ClassType BaseClass
		{
			get
			{
				return baseClass;
			}
			set
			{
				//TODO
				//if (value == baseClass)
				//	return;

				//if (value == null)
				//{
				//	baseClass.derivedClassCount--;
				//	baseClass = null;
				//	Changed();
				//	return;
				//}

				//if (value == this)
				//	throw new RelationshipException(Strings.ErrorInvalidBaseType);

				//if (value.Modifier == ClassModifier.Sealed ||
				//	value.Modifier == ClassModifier.Static)
				//{
				//	throw new RelationshipException(Strings.ErrorCannotInherit);
				//}
				//if (value.IsAncestor(this))
				//{
				//	throw new RelationshipException(string.Format(Strings.ErrorCyclicBase,
				//		Strings.Class));
				//}
				//if (value.Language != this.Language)
				//	throw new RelationshipException(Strings.ErrorLanguagesDoNotEqual);

				baseClass = value;
				//baseClass.derivedClassCount++;
				//Changed();
			}
		}		

		private bool IsAncestor(ClassType classType)
		{
			//TODO
			//if (BaseClass != null && BaseClass.IsAncestor(classType))
			//	return true;
			//else
			//	return (classType == this);

			return true;
		}

		protected override void CopyFrom(StateBase type)
		{
			//TODO
			//base.CopyFrom(type);
			//ClassType classType = (ClassType)type;
			//modifier = classType.modifier;
			//Stereotype = classType.Stereotype;
			//NHMTableName = classType.NHMTableName;
			//IdGenerator = classType.IdGenerator;
		}

		public abstract StateType Clone();

		/// <exception cref="ArgumentNullException">
		/// <paramref name="node"/> is null.
		/// </exception>
		protected internal override void Serialize(XmlElement node)
		{
			base.Serialize(node);

			XmlElement child = node.OwnerDocument.CreateElement("Modifier");
			child.InnerText = Modifier.ToString();
			node.AppendChild(child);

			// Save the stereotype.
			XmlElement childStereotype = node.OwnerDocument.CreateElement("Stereotype");
			childStereotype.InnerText = Stereotype;
			node.AppendChild(childStereotype);
		}

		/// <exception cref="BadSyntaxException">
		/// An error occured while deserializing.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// The XML document is corrupt.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="node"/> is null.
		/// </exception>
		protected internal override void Deserialize(XmlElement node)
		{
			RaiseChangedEvent = false;

			XmlElement child = node["Modifier"];
			if (child != null)
				Modifier = Language.TryParseClassModifier(child.InnerText);

			// Load the stereotype.
			XmlElement childStereotype = node["Stereotype"];
			if (childStereotype != null)
			{
				if (string.IsNullOrEmpty(childStereotype.InnerText))
				{
					Stereotype = null;
				}
				else
				{
					Stereotype = childStereotype.InnerText;
				}
			}

			base.Deserialize(node);
			RaiseChangedEvent = true;
		}

		string nhmTableName;

		/// <summary>
		/// Gets or sets the nhmTableName for this class.
		/// </summary>
		public override string NHMTableName
		{
			get
			{
				return nhmTableName;
			}
			set
			{
				if (value != nhmTableName)
				{
					nhmTableName = value;
					Changed();
				}
			}
		}

		string idGenerator;

		/// <summary>
		/// Gets or sets the idGenerator for this class.
		/// </summary>
		public override string IdGenerator
		{
			get
			{
				return idGenerator;
			}
			set
			{
				if (value != idGenerator)
				{
					idGenerator = value;
					Changed();
				}
			}
		}

		string generatorParameters;

		/// <summary>
		/// Gets or sets the GeneratorParameters for this class.
		/// </summary>
		public override string GeneratorParameters
		{
			get
			{
				return generatorParameters;
			}
			set
			{
				if (value != generatorParameters)
				{
					generatorParameters = value;
					Changed();
				}
			}
		}

		public sealed override bool MoveUpItem(object item)
		{
			//TODO
			//if (item is Field)
			//{
			//	if (MoveUp(FieldList, item))
			//	{
			//		Changed();
			//		return true;
			//	}
			//}
			//else if (item is Operation)
			//{
			//	if (MoveUp(OperationList, item))
			//	{
			//		Changed();
			//		return true;
			//	}
			//}
			return false;
		}

		public sealed override bool MoveDownItem(object item)
		{
			//TODO
			//if (item is Field)
			//{
			//	if (MoveDown(FieldList, item))
			//	{
			//		Changed();
			//		return true;
			//	}
			//}
			//else if (item is Operation)
			//{
			//	if (MoveDown(OperationList, item))
			//	{
			//		Changed();
			//		return true;
			//	}
			//}
			return false;
		}

		public override AccessModifier DefaultAccess
		{
			get { return AccessModifier.Public; }
		}
	}
       
}
