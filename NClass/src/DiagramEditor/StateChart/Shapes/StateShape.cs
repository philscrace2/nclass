using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NClass.Core;
using NClass.DiagramEditor.ClassDiagram;
using NClass.DiagramEditor.ClassDiagram.Editors;
using NClass.DiagramEditor.ClassDiagram.Dialogs;
using NClass.DiagramEditor.GenericUml.Shapes;
using NClass.DiagramEditor.GenericUml;
using NClass.DiagramEditor.GenericUml.Editors;

namespace NClass.DiagramEditor.StateChart.Shapes
{
	public abstract class StateShape : NodeShape
    {
		const int AccessSpacing = 12;

		static CompositeTypeEditor typeEditor = new CompositeTypeEditor();
		static MemberEditor memberEditor = new MemberEditor();
		static MembersDialog membersDialog = new MembersDialog();
		static SolidBrush memberBrush = new SolidBrush(Color.Black);
		static StringFormat accessFormat = new StringFormat(StringFormat.GenericTypographic);
		static Pen selectionPen = new Pen(Color.Black);

		static StateShape()
		{
			accessFormat.Alignment = StringAlignment.Center;
			accessFormat.LineAlignment = StringAlignment.Center;
			selectionPen.DashPattern = new float[] { 2, 4 };
		}

		/// <exception cref="ArgumentNullException">
		/// <paramref name="stateType"/> is null.
		/// </exception>
		protected StateShape(StateType stateType)
			: base(stateType)
		{
		}

		public abstract StateType NodeType { get; }

		public sealed override StateBase StateBase
		{
			get { return NodeType; }
		}

		protected override TypeEditor HeaderEditor
		{
			get { return typeEditor; }
		}

		protected override EditorWindow ContentEditor
		{
			get { return memberEditor; }
		}

		protected internal Member ActiveMember
		{
			get
			{
				//TODO
				//if (ActiveMemberIndex >= 0 && ActiveMemberIndex < NodeType.FieldCount)
				//{
				//	return NodeType.GetField(ActiveMemberIndex);
				//}
				//else if (ActiveMemberIndex >= NodeType.FieldCount)
				//{
				//	return NodeType.GetOperation(ActiveMemberIndex - NodeType.FieldCount);
				//}
				//else
				//{
				//	return null;
				//}

				return null;
			}
		}


		protected internal override int ActiveMemberIndex
		{
			get
			{
				return base.ActiveMemberIndex;
			}
			set
			{
				Member oldMember = ActiveMember;

				//if (value < NodeType.MemberCount)
				//	base.ActiveMemberIndex = value;
				//else
				//	base.ActiveMemberIndex = NodeType.MemberCount - 1;

				if (oldMember != ActiveMember)
					OnActiveMemberChanged(EventArgs.Empty);
			}
		}

		public override void MoveUp()
		{
			if (ActiveMember != null && NodeType.MoveUpItem(ActiveMember))
			{
				ActiveMemberIndex--;
			}
		}

		public override void MoveDown()
		{
			if (ActiveMember != null && NodeType.MoveDownItem(ActiveMember))
			{
				ActiveMemberIndex++;
			}
		}

		protected internal override void EditMembers()
		{
			//TODO
			//membersDialog.ShowDialog(NodeType, this.Diagram.Entities);
		}

		protected override EditorWindow GetEditorWindow()
		{
			if (IsActive)
			{
				if (ActiveMember == null)
					return HeaderEditor;
				else
					return memberEditor;
			}
			else
			{
				return null;
			}
		}

		protected internal sealed override bool DeleteSelectedMember(bool showConfirmation)
		{
			if (IsActive && ActiveMember != null)
			{
				if (!showConfirmation || ConfirmMemberDelete())
					DeleteActiveMember();
				return true;
			}
			else
			{
				return false;
			}
		}

		protected override void OnMouseDown(AbsoluteMouseEventArgs e)
		{
			base.OnMouseDown(e);
			SelectMember(e.Location);
		}

		internal Rectangle GetMemberRectangle(int memberIndex)
		{
			Rectangle record = new Rectangle(
				Left + MarginSize, Top + HeaderHeight + MarginSize,
				Width - MarginSize * 2, MemberHeight);

			record.Y += memberIndex * MemberHeight;
			//TODO if (NodeType.SupportsFields && memberIndex >= NodeType.FieldCount)
			//{
			//	record.Y += MarginSize * 2;
			//}
			return record;
		}

		private void SelectMember(PointF location)
		{
			if (Contains(location))
			{
				int index;
				int y = (int)location.Y;
				int top = Top + HeaderHeight + MarginSize;

				//TODO if (top <= y)
				//{
				//	if (NodeType.SupportsFields)
				//	{
				//		index = (y - top) / MemberHeight;
				//		if (index < NodeType.FieldCount)
				//		{
				//			ActiveMemberIndex = index;
				//			return;
				//		}
				//		top += MarginSize * 2;
				//	}

				//	int operationTop = top + NodeType.FieldCount * MemberHeight;
				//	if (operationTop <= y)
				//	{
				//		index = (y - top) / MemberHeight;
				//		if (index < NodeType.MemberCount)
				//		{
				//			ActiveMemberIndex = index;
				//			return;
				//		}
				//	}
				//}
				ActiveMemberIndex = -1;
			}
		}

		internal void DeleteActiveMember()
		{
			if (ActiveMemberIndex >= 0)
			{
				int newIndex;
				int fieldCount = 2; //TODO NodeType.FieldCount;
				int memberCount = 2; //TODO NodeType.MemberCount;

				if (ActiveMemberIndex == fieldCount - 1 && fieldCount >= 2) // Last field
				{
					newIndex = fieldCount - 2;
				}
				else if (ActiveMemberIndex == memberCount - 1) // Last member
				{
					newIndex = ActiveMemberIndex - 1;
				}
				else
				{
					newIndex = ActiveMemberIndex;
				}

				//NodeType.RemoveMember(ActiveMember);
				ActiveMemberIndex = newIndex;
				OnActiveMemberChanged(EventArgs.Empty);
			}
		}

		internal void InsertNewMember(MemberType type)
		{
			//int fieldCount = NodeType.FieldCount;
			switch (type)
			{
				//case MemberType.Field:
				//	if (NodeType.SupportsFields)
				//	{
				//		int index = Math.Min(ActiveMemberIndex + 1, fieldCount);
				//		bool changing = (index == fieldCount &&
				//			ActiveMember.MemberType != MemberType.Field);

				//		NodeType.InsertMember(MemberType.Field, index);
				//		ActiveMemberIndex = index;
				//	}
				//	break;

				//case MemberType.Method:
				//	if (NodeType.SupportsMethods)
				//	{
				//		int index = Math.Max(ActiveMemberIndex + 1, fieldCount);
				//		NodeType.InsertMember(MemberType.Method, index);
				//		ActiveMemberIndex = index;
				//	}
				//	break;

				//case MemberType.Constructor:
				//	if (NodeType.SupportsConstuctors)
				//	{
				//		int index = Math.Max(ActiveMemberIndex + 1, fieldCount);
				//		NodeType.InsertMember(MemberType.Constructor, index);
				//		ActiveMemberIndex = index;
				//	}
				//	break;

				//case MemberType.Destructor:
				//	if (NodeType.SupportsDestructors)
				//	{
				//		int index = Math.Max(ActiveMemberIndex + 1, fieldCount);
				//		NodeType.InsertMember(MemberType.Destructor, index);
				//		ActiveMemberIndex = index;
				//	}
				//	break;

				//case MemberType.Property:
				//	if (NodeType.SupportsProperties)
				//	{
				//		int index = Math.Max(ActiveMemberIndex + 1, fieldCount);
				//		NodeType.InsertMember(MemberType.Property, index);
				//		ActiveMemberIndex = index;
				//	}
				//	break;

				//case MemberType.Event:
				//	if (NodeType.SupportsEvents)
				//	{
				//		int index = Math.Max(ActiveMemberIndex + 1, fieldCount);
				//		NodeType.InsertMember(MemberType.Event, index);
				//		ActiveMemberIndex = index;
				//	}
				//	break;
			}
		}

		private static string GetAccessString(Member member)
		{
			switch (member.Access)
			{
				case AccessModifier.Public:
					return "+";

				case AccessModifier.Internal:
					return "~";

				case AccessModifier.ProtectedInternal:
				case AccessModifier.Protected:
					return "#";

				case AccessModifier.Private:
				default:
					return "-";
			}
		}

		private static string GetMemberString(Member member)
		{
			return member.GetUmlDescription(
				Settings.Default.ShowType,
				Settings.Default.ShowParameters,
				Settings.Default.ShowParameterNames,
				Settings.Default.ShowInitialValue);
		}

		private Font GetMemberFont(Member member, Style style)
		{
			Font memberFont;
			if (member.IsStatic)
			{
				memberFont = style.StaticMemberFont;
			}
			else if (member is Operation &&
				(((Operation)member).IsAbstract || member.Parent is InterfaceType))
			{
				memberFont = style.AbstractMemberFont;
			}
			else
			{
				memberFont = GetFont(style);
			}

			return memberFont;
		}

		protected virtual Font GetNameFont(Style style)
		{
			return style.NameFont;
		}


		private void DrawMember(IGraphics g, Member member, Rectangle record, Style style)
		{
			Font memberFont = GetMemberFont(member, style);

			if (member is Field)
				memberBrush.Color = style.AttributeColor;
			else
				memberBrush.Color = style.OperationColor;

			if (style.UseIcons)
			{
				Image icon = Icons.GetImage(member);
				g.DrawImage(icon, record.X, record.Y);

				Rectangle textBounds = new Rectangle(
					record.X + IconSpacing, record.Y,
					record.Width - IconSpacing, record.Height);

				string memberString = GetMemberString(member);
				g.DrawString(memberString, memberFont, memberBrush, textBounds, memberFormat);
			}
			else
			{
				Rectangle accessBounds = new Rectangle(
					record.X, record.Y, AccessSpacing, record.Height);
				Rectangle textBounds = new Rectangle(
					record.X + AccessSpacing, record.Y,
					record.Width - AccessSpacing, record.Height);

				g.DrawString(GetAccessString(member), GetFont(style),
					memberBrush, accessBounds, accessFormat);
				g.DrawString(GetMemberString(member), memberFont,
					memberBrush, textBounds, memberFormat);
			}
		}

		protected internal override void DrawSelectionLines(Graphics g, float zoom, Point offset)
		{
			base.DrawSelectionLines(g, zoom, offset);

			// Draw selected member rectangle
			if (IsActive && ActiveMember != null)
			{
				Rectangle record = GetMemberRectangle(ActiveMemberIndex);
				record = TransformRelativeToAbsolute(record, zoom, offset);
				record.Inflate(2, 0);
				g.DrawRectangle(Diagram.SelectionPen, record);
			}
		}

		protected override void DrawContent(IGraphics g, Style style)
		{
			Rectangle record = new Rectangle(
				Left + MarginSize, Top + HeaderHeight + MarginSize,
				Width - MarginSize * 2, MemberHeight);

			// Draw fields
			//foreach (Field field in NodeType.Fields)
			//{
			//	DrawMember(g, field, record, style);
			//	record.Y += MemberHeight;
			//}

			//Draw separator line 
			//if (NodeType.SupportsFields)
			//{
			//	DrawSeparatorLine(g, record.Top + MarginSize);
			//	record.Y += MarginSize * 2;
			//}

			// Draw operations
			//foreach (Operation operation in NodeType.Operations)
			//{
			//	DrawMember(g, operation, record, style);
			//	record.Y += MemberHeight;
			//}
		}

		protected override float GetRequiredWidth(Graphics g, Style style)
		{
			float requiredWidth = 0;

			//foreach (Field field in NodeType.Fields)
			//{
			//	float fieldWidth = g.MeasureString(GetMemberString(field),
			//		GetMemberFont(field, style), PointF.Empty, memberFormat).Width;
			//	requiredWidth = Math.Max(requiredWidth, fieldWidth);
			//}
			//foreach (Operation operation in NodeType.Operations)
			//{
			//	float operationWidth = g.MeasureString(GetMemberString(operation),
			//		GetMemberFont(operation, style), PointF.Empty, memberFormat).Width;
			//	requiredWidth = Math.Max(requiredWidth, operationWidth);
			//}
			requiredWidth += (style.UseIcons) ? IconSpacing : AccessSpacing;
			requiredWidth += MarginSize * 2;

			return Math.Max(requiredWidth, base.GetRequiredWidth(g, style));
		}

		protected override int GetRequiredHeight()
		{
			int memberCount = 0;
			int spacingHeight = 0;

			//TODO
			//if (NodeType.SupportsFields)
			//{
			//	memberCount += NodeType.FieldCount;
			//	spacingHeight += MarginSize * 2;
			//}
			//if (NodeType.SupportsOperations)
			//{
			//	memberCount += NodeType.OperationCount;
			//	spacingHeight += MarginSize * 2;
			//}

			return (HeaderHeight + spacingHeight + (memberCount * MemberHeight));
		}

		private int GetRowIndex(int height)
		{
			//TODO
			//height -= HeaderHeight + MarginSize;

			//if (NodeType.SupportsFields && (height > NodeType.FieldCount * MemberHeight))
			//	height -= MarginSize * 2;

			return (height / MemberHeight);			
		}
	}
}
