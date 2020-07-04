// NClass - Free class diagram editor
// Copyright (C) 2006-2009 Balazs Tihanyi
// 
// This program is free software; you can redistribute it and/or modify it under 
// the terms of the GNU General Public License as published by the Free Software 
// Foundation; either version 3 of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT 
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS 
// FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with 
// this program; if not, write to the Free Software Foundation, Inc., 
// 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Xml;
using System.Collections;
using NClass.Translations;

namespace NClass.Core
{
    public abstract class StateBase : LanguageElement, IEntity
	{
		string name;
		AccessModifier access = AccessModifier.Public;
		CompositeType nestingParent = null;

		public event SerializeEventHandler Serializing;
		public event SerializeEventHandler Deserializing;

		/// <exception cref="BadSyntaxException">
		/// The <paramref name="name"/> does not fit to the syntax.
		/// </exception>
		protected StateBase(string name)
		{
			Initializing = true;
			Name = name;
			Initializing = false;
		}

		/// <exception cref="BadSyntaxException">
		/// The <paramref name="value"/> does not fit to the syntax.
		/// </exception>
		public virtual string Name
		{
			get
			{
				return name;
			}
			set
			{
				string newName = Language.GetValidName(value, true);

				if (newName != name) {
					name = newName;
					Changed();
				}
			}
		}

		public abstract EntityType EntityType
		{
			get;
		}

		/// <exception cref="BadSyntaxException">
		/// The type visibility is not valid in the current context.
		/// </exception>
		public virtual AccessModifier AccessModifier
		{
			get
			{
				return access;
			}
			set
			{
				if (!Language.IsValidModifier(value))
					throw new BadSyntaxException(Strings.ErrorInvalidModifier);

				if (access != value) {
					access = value;
					Changed();
				}
			}
		}

		public abstract AccessModifier DefaultAccess
		{
			get;
		}

		public AccessModifier Access
		{
			get
			{
				if (AccessModifier == AccessModifier.Default)
					return DefaultAccess;
				else
					return AccessModifier;
			}
		}		

		public abstract Language Language
		{
			get;
		}

		public abstract string Stereotype
		{
			get;
			set;
		}

		public abstract string Signature
		{
			get;
		}		

		public abstract bool MoveUpItem(object item);

		public abstract bool MoveDownItem(object item);

		protected static bool MoveUp(IList list, object item)
		{
			if (item == null)
				return false;

			int index = list.IndexOf(item);
			if (index > 0) {
				object temp = list[index - 1];
				list[index - 1] = list[index];
				list[index] = temp;
				return true;
			}
			else {
				return false;
			}
		}

		protected static bool MoveDown(IList list, object item)
		{
			if (item == null)
				return false;

			int index = list.IndexOf(item);
			if (index >= 0 && index < list.Count - 1) {
				object temp = list[index + 1];
				list[index + 1] = list[index];
				list[index] = temp;
				return true;
			}
			else {
				return false;
			}
		}

		protected virtual void CopyFrom(StateBase type)
		{
			name = type.name;
			access = type.access;
		}

		void ISerializableElement.Serialize(XmlElement node)
		{
			Serialize(node);
		}

		void ISerializableElement.Deserialize(XmlElement node)
		{
			Deserialize(node);
		}

		/// <exception cref="ArgumentNullException">
		/// <paramref name="node"/> is null.
		/// </exception>
		protected internal virtual void Serialize(XmlElement node)
		{
			if (node == null)
				throw new ArgumentNullException("node");

			XmlElement child;

            child = node.OwnerDocument.CreateElement("GeneratorParameters");
            child.InnerXml = GeneratorParameters;
            node.AppendChild(child);

			child = node.OwnerDocument.CreateElement("Name");
			child.InnerText = Name;
			node.AppendChild(child);

			child = node.OwnerDocument.CreateElement("Access");
			child.InnerText = AccessModifier.ToString();
			node.AppendChild(child);

			OnSerializing(new SerializeEventArgs(node));
		}

		/// <exception cref="BadSyntaxException">
		/// An error occured whiledeserializing.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// The XML document is corrupt.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="node"/> is null.
		/// </exception>
		protected internal virtual void Deserialize(XmlElement node)
		{
			if (node == null)
				throw new ArgumentNullException("node");

			RaiseChangedEvent = false;
            XmlElement nameChild = node["Name"];
            if (nameChild != null)
                Name = nameChild.InnerText;

            XmlElement GeneratorParametersChild = node["GeneratorParameters"];
            if (GeneratorParametersChild != null)
                GeneratorParameters = GeneratorParametersChild.InnerXml;

			XmlElement accessChild = node["Access"];
			if (accessChild != null)
				AccessModifier = Language.TryParseAccessModifier(accessChild.InnerText);

			RaiseChangedEvent = true;
			OnDeserializing(new SerializeEventArgs(node));
		}

		private void OnSerializing(SerializeEventArgs e)
		{
			if (Serializing != null)
				Serializing(this, e);
		}

		private void OnDeserializing(SerializeEventArgs e)
		{
			if (Deserializing != null)
				Deserializing(this, e);
		}

		public override string ToString()
		{
			return Name + ": " + Signature;
		}

        public virtual string NHMTableName
        { 
            get;
            set;
        }

        public virtual string IdGenerator
        {
            get;
            set;
        }

        public virtual string GeneratorParameters
        {
            get;
            set;
        }
	}
}