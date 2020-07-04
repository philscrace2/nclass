using System;
using System.Drawing;
using NClass.Core;
using NClass.DiagramEditor.StateChart.Editors;
using NClass.DiagramEditor.GenericUml;
using NClass.DiagramEditor.GenericUml.Shapes;
using NClass.DiagramEditor.GenericUml.Editors;

namespace NClass.DiagramEditor.StateChart.Shapes
{
    public sealed class StateChartShape : StateShape
    {
		StateType _state;

		static StateTypeEditor typeEditor = new StateTypeEditor();

		/// <exception cref="ArgumentNullException">
		/// <paramref name="stateType"/> is null.
		/// </exception>
		internal StateChartShape(StateType stateType)
			: base(stateType)
		{
			_state = stateType;
			UpdateMinSize();
		}

		public override StateType NodeType
		{
			get { return _state; }
		}

		protected override TypeEditor HeaderEditor
		{
			get { return typeEditor; }
		}

		public StateType StateType
		{
			get { return _state; }
		}

		protected override bool CloneEntity(Diagram diagram)
		{
			return diagram.InsertState(StateType.Clone());
		}

		protected override Color GetBackgroundColor(Style style)
		{
			return style.ClassBackgroundColor;
		}

		protected override Color GetBorderColor(Style style)
		{
			return style.ClassBorderColor;
		}

		protected override int GetBorderWidth(Style style)
		{
			switch (_state.Modifier)
			{
				case ClassModifier.Abstract:
					return style.AbstractClassBorderWidth;

				case ClassModifier.Sealed:
					return style.SealedClassBorderWidth;

				case ClassModifier.Static:
					return style.StaticClassBorderWidth;

				case ClassModifier.None:
				default:
					return style.ClassBorderWidth;
			}
		}

		protected override bool IsBorderDashed(Style style)
		{
			switch (_state.Modifier)
			{
				case ClassModifier.Abstract:
					return style.IsAbstractClassBorderDashed;

				case ClassModifier.Sealed:
					return style.IsSealedClassBorderDashed;

				case ClassModifier.Static:
					return style.IsStaticClassBorderDashed;

				case ClassModifier.None:
				default:
					return style.IsClassBorderDashed;
			}
		}

		protected override Color GetHeaderColor(Style style)
		{
			return style.ClassHeaderColor;
		}

		protected override Font GetNameFont(Style style)
		{
			if (_state.Modifier == ClassModifier.Abstract)
				return style.AbstractNameFont;
			else
				return base.GetNameFont(style);
		}

		protected override int GetRoundingSize(Style style)
		{
			return style.ClassRoundingSize;
		}

		protected override GradientStyle GetGradientHeaderStyle(Style style)
		{
			return style.ClassGradientHeaderStyle;
		}
	}
}

