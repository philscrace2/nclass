using NClass.Core;
using NClass.DiagramEditor.ClassDiagram;
using NClass.DiagramEditor.ClassDiagram.Shapes;
using NClass.DiagramEditor.GenericUml.Editors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;


namespace NClass.DiagramEditor.GenericUml.Shapes
{
    public abstract class NodeShape : Shape
    {        
        public const int DefaultWidth = 162;
        public const int DefaultHeight = 216;
        protected const int MarginSize = 8;
        protected const int IconSpacing = 21;
        protected const int HeaderHeight = 45;
        protected const int MemberHeight = 17;
        protected static readonly StringFormat memberFormat;

        static Pen borderPen = new Pen(Color.Black);
        static SolidBrush backgroundBrush = new SolidBrush(Color.White);
        static SolidBrush solidHeaderBrush = new SolidBrush(Color.White);
        static SolidBrush nameBrush = new SolidBrush(Color.Black);
        static SolidBrush identifierBrush = new SolidBrush(Color.Black);
        static StringFormat headerFormat = new StringFormat(StringFormat.GenericTypographic);
        static readonly Size chevronSize = new Size(13, 13);

        public event EventHandler ActiveMemberChanged;

        int activeMemberIndex = -1;
        bool collapsed = false;
        bool showChevron = false;
        EditorWindow showedEditor = null;

        static NodeShape()
        {
            memberFormat = new StringFormat(StringFormat.GenericTypographic);
            memberFormat.FormatFlags = StringFormatFlags.NoWrap;
            memberFormat.LineAlignment = StringAlignment.Center;
            memberFormat.Trimming = StringTrimming.EllipsisCharacter;

            headerFormat.FormatFlags = StringFormatFlags.NoWrap;
            headerFormat.Trimming = StringTrimming.EllipsisCharacter;
        }

        /// <exception cref="ArgumentNullException">
		/// <paramref name="stateBase"/> is null.
		/// </exception>
		protected NodeShape(StateBase stateBase) : base(stateBase)
        {
            MinimumSize = new Size(DefaultWidth, MinimumSize.Height);
            stateBase.Modified += delegate { UpdateMinSize(); };
        }

        protected void UpdateMinSize()
        {
            MinimumSize = new Size(MinimumSize.Width, GetRequiredHeight());
        }

        public sealed override IEntity Entity
        {
            get { return StateBase; }
        }

        public override Size Size { get => base.Size; set => base.Size = value; }
        public override int Width { get => base.Width; set => base.Width = value; }
        public override int Height { get => base.Height; set => base.Height = value; }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(DefaultWidth, DefaultHeight);
            }
        }

        public abstract StateBase StateBase
        {
            get;
        }

        protected abstract TypeEditor HeaderEditor
        {
            get;
        }

        protected abstract EditorWindow ContentEditor
        {
            get;
        }

        public override void Clean()
        {
            base.Clean();
        }

        protected bool Collapsed
        {
            get
            {
                return collapsed;
            }
            set
            {
                if (collapsed != value)
                {
                    Size oldSize = Size;
                    collapsed = value;
                    OnResize(new ResizeEventArgs(Size - oldSize));
                    OnModified(EventArgs.Empty);
                }
            }
        }

        public override void Draw(IGraphics g, bool onScreen, Style style)
        {
            DrawSurface(g, onScreen, style);
            DrawHeaderText(g, style);
            if (onScreen && CanDrawChevron)
                DrawChevron(g);
            if (!Collapsed)
                DrawContent(g, style);
        }

        private void DrawSurface(IGraphics g, bool onScreen, Style style)
        {
            // Update styles
            backgroundBrush.Color = GetBackgroundColor(style);
            borderPen.Color = GetBorderColor(style);
            borderPen.Width = GetBorderWidth(style);
            if (IsBorderDashed(style))
                borderPen.DashPattern = borderDashPattern;
            else
                borderPen.DashStyle = DashStyle.Solid;

            if (GetRoundingSize(style) == 0)
                DrawRectangleSurface(g, onScreen, style);
            else
                DrawRoundedSurface(g, onScreen, style);
        }

        private void DrawChevron(IGraphics g)
        {
            Bitmap chevron = Collapsed ? Properties.Resources.Expand : Properties.Resources.Collapse;
            Point location = new Point(Right - MarginSize - chevronSize.Width, Top + MarginSize);

            g.DrawImage(chevron, location);
        }

        private RectangleF CaptionRegion
        {
            get
            {
                return new RectangleF(
                    Left + MarginSize, Top + MarginSize,
                    Width - MarginSize * 2, HeaderHeight - MarginSize * 2
                );
            }
        }

        private void DrawHeaderText(IGraphics g, Style style)
        {
            string name = StateBase.Name;
            RectangleF textRegion = CaptionRegion;

            // Update styles
            nameBrush.Color = style.NameColor;
            identifierBrush.Color = style.IdentifierColor;
            headerFormat.Alignment = GetHorizontalAlignment(style.HeaderAlignment);

            if (HasIdentifier(style))
            {
                float nameHeight = GetNameFont(style).GetHeight();
                float identifierHeight = style.IdentifierFont.GetHeight();
                float textHeight = nameHeight + identifierHeight;

                textRegion.Y = GetHeaderTextTop(textRegion, textHeight, style.HeaderAlignment);
                textRegion.Height = textHeight;

                // Draw name and signature
                if (style.ShowSignature)
                {
                    // Draw name to the top
                    headerFormat.LineAlignment = StringAlignment.Near;
                    g.DrawString(name, GetNameFont(style), nameBrush, textRegion, headerFormat);

                    // Draw signature to the bottom
                    headerFormat.LineAlignment = StringAlignment.Far;
                    g.DrawString(StateBase.Signature, style.IdentifierFont, identifierBrush,
                        textRegion, headerFormat);
                }
                // Draw name and stereotype
                else
                {
                    // Draw stereotype to the top
                    headerFormat.LineAlignment = StringAlignment.Near;
                    g.DrawString(StateBase.Stereotype, style.IdentifierFont,
                        identifierBrush, textRegion, headerFormat);

                    // Draw name to the bottom
                    headerFormat.LineAlignment = StringAlignment.Far;
                    g.DrawString(name, GetNameFont(style), nameBrush, textRegion, headerFormat);
                }
            }
            else
            {
                // Draw name only
                headerFormat.LineAlignment = GetVerticalAlignment(style.HeaderAlignment);
                g.DrawString(name, GetNameFont(style), nameBrush, textRegion, headerFormat);
            }

            if (!Collapsed)
                DrawSeparatorLine(g, Top + HeaderHeight);
        }

        private bool CanDrawChevron
        {
            get
            {
                return
                    Settings.Default.ShowChevron == ChevronMode.Always ||
                    Settings.Default.ShowChevron == ChevronMode.AsNeeded && showChevron
                ;
            }
        }

        protected void DrawSeparatorLine(IGraphics g, int height)
        {
            g.DrawLine(borderPen, Left, height, Right, height);
        }

        private void DrawRectangleSurface(IGraphics g, bool onScreen, Style style)
        {
            // Draw shadow
            if ((!onScreen || !IsSelected) && !style.ShadowOffset.IsEmpty)
            {
                shadowBrush.Color = style.ShadowColor;
                g.TranslateTransform(style.ShadowOffset.Width, style.ShadowOffset.Height);
                g.FillRectangle(shadowBrush, BorderRectangle);
                g.TranslateTransform(-style.ShadowOffset.Width, -style.ShadowOffset.Height);
            }

            // Draw background
            backgroundBrush.Color = GetBackgroundColor(style);
            g.FillRectangle(backgroundBrush, BorderRectangle);

            // Draw header background
            DrawHeaderBackground(g, style);

            // Draw border
            g.DrawRectangle(borderPen, BorderRectangle);
        }

        private void DrawHeaderBackground(IGraphics g, Style style)
        {
            Color backColor = GetBackgroundColor(style);
            Color headerColor = GetHeaderColor(style);

            Rectangle headerRectangle = new Rectangle(Left, Top, Width, HeaderHeight);

            if (GetGradientHeaderStyle(style) != GradientStyle.None)
            {
                LinearGradientMode gradientMode;

                switch (GetGradientHeaderStyle(style))
                {
                    case GradientStyle.Vertical:
                        gradientMode = LinearGradientMode.Vertical; break;

                    case GradientStyle.Diagonal:
                        gradientMode = LinearGradientMode.ForwardDiagonal; break;

                    case GradientStyle.Horizontal:
                    default:
                        gradientMode = LinearGradientMode.Horizontal; break;
                }

                Brush headerBrush = new LinearGradientBrush(headerRectangle,
                    headerColor, backColor, gradientMode);
                g.FillRectangle(headerBrush, headerRectangle);
                headerBrush.Dispose();
            }
            else
            {
                if (headerColor != backColor || headerColor.A < 255)
                {
                    solidHeaderBrush.Color = GetHeaderColor(style);
                    g.FillRectangle(solidHeaderBrush, headerRectangle);
                }
            }
        }

        private void DrawRoundedSurface(IGraphics g, bool onScreen, Style style)
        {
            int diameter = GetRoundingSize(style) * 2;

            GraphicsPath borderPath = new GraphicsPath();
            borderPath.AddArc(Left, Top, diameter, diameter, 180, 90);
            borderPath.AddArc(Right - diameter, Top, diameter, diameter, 270, 90);
            borderPath.AddArc(Right - diameter, Bottom - diameter, diameter, diameter, 0, 90);
            borderPath.AddArc(Left, Bottom - diameter, diameter, diameter, 90, 90);
            borderPath.CloseFigure();

            // Draw shadow
            if ((!onScreen || !IsSelected) && !style.ShadowOffset.IsEmpty)
            {
                shadowBrush.Color = style.ShadowColor;
                g.TranslateTransform(style.ShadowOffset.Width, style.ShadowOffset.Height);
                g.FillPath(shadowBrush, borderPath);
                g.TranslateTransform(-style.ShadowOffset.Width, -style.ShadowOffset.Height);
            }

            // Draw background
            g.FillPath(backgroundBrush, borderPath);

            // Draw header background
            Region oldClip = g.Clip;
            g.SetClip(borderPath, CombineMode.Intersect);
            DrawHeaderBackground(g, style);
            g.Clip.Dispose();
            g.Clip = oldClip;

            // Draw border
            g.DrawPath(borderPen, borderPath);

            borderPath.Dispose();
        }

        protected virtual Font GetNameFont(Style style)
        {
            return style.NameFont;
        }

        private static StringAlignment GetHorizontalAlignment(ContentAlignment alignment)
        {
            switch (alignment)
            {
                case ContentAlignment.BottomLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.TopLeft:
                    return StringAlignment.Near;

                case ContentAlignment.BottomCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.TopCenter:
                default:
                    return StringAlignment.Center;

                case ContentAlignment.BottomRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.TopRight:
                    return StringAlignment.Far;
            }
        }

        private static StringAlignment GetVerticalAlignment(ContentAlignment alignment)
        {
            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.TopCenter:
                case ContentAlignment.TopRight:
                    return StringAlignment.Near;

                case ContentAlignment.MiddleLeft:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.MiddleRight:
                default:
                    return StringAlignment.Center;

                case ContentAlignment.BottomLeft:
                case ContentAlignment.BottomCenter:
                case ContentAlignment.BottomRight:
                    return StringAlignment.Far;
            }
        }

        private static float GetHeaderTextTop(RectangleF textRegion, float textHeight,
            ContentAlignment alignment)
        {
            float top = textRegion.Top;

            switch (alignment)
            {
                case ContentAlignment.BottomLeft:
                case ContentAlignment.BottomCenter:
                case ContentAlignment.BottomRight:
                    top += textRegion.Height - textHeight;
                    break;

                case ContentAlignment.MiddleLeft:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.MiddleRight:
                    top += (textRegion.Height - textHeight) / 2;
                    break;
            }

            return top;
        }

        private bool HasIdentifier(Style style)
        {
            return
                style.ShowSignature ||
                style.ShowStereotype && StateBase.Stereotype != null
            ;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override void Expand()
        {
            base.Expand();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override void MoveDown()
        {
            base.MoveDown();
        }

        public override void MoveUp()
        {
            base.MoveUp();
        }

        public override void SelectNext()
        {
            base.SelectNext();
        }

        public override void SelectPrevious()
        {
            base.SelectPrevious();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        protected abstract Color GetBackgroundColor(Style style);

        protected abstract Color GetHeaderColor(Style style);

        protected abstract GradientStyle GetGradientHeaderStyle(Style style);

        protected abstract Color GetBorderColor(Style style);

        protected abstract bool IsBorderDashed(Style style);

        protected abstract int GetRoundingSize(Style style);

        protected virtual void OnActiveMemberChanged(EventArgs e)
        {
            if (ActiveMemberChanged != null)
                ActiveMemberChanged(this, e);

            if (showedEditor != null)
            {
                EditorWindow editor = GetEditorWindow();

                if (editor != showedEditor)
                {
                    HideWindow(showedEditor);
                }
                ShowEditor(editor);
            }
            NeedsRedraw = true;
        }

        protected abstract EditorWindow GetEditorWindow();

        private void ShowEditor(EditorWindow editor)
        {
            editor.Relocate(this);
            editor.Init(this);
            ShowWindow(editor);
            editor.Focus();
            showedEditor = editor;
        }

        protected virtual Font GetFont(Style style)
        {
            return style.MemberFont;
        }

        protected internal virtual int ActiveMemberIndex
        {
            get
            {
                return activeMemberIndex;
            }
            set
            {
                if (value >= -1)
                    activeMemberIndex = value;
            }
        }

        protected abstract void DrawContent(IGraphics g, Style style);

        protected internal abstract void EditMembers();

        protected override RectangleF CalculateDrawingArea(Style style, bool printing, float zoom)
        {
            return base.CalculateDrawingArea(style, printing, zoom);
        }

        protected override bool CloneEntity(Diagram diagram)
        {
            throw new NotImplementedException();
        }

        protected override void CopyFrom(Shape shape)
        {
            base.CopyFrom(shape);
        }

        protected override int GetBorderWidth(Style style)
        {
            throw new NotImplementedException();
        }

        protected override int GetRequiredHeight()
        {
            return base.GetRequiredHeight();
        }

        protected override float GetRequiredWidth(Graphics g, Style style)
        {
            return base.GetRequiredWidth(g, style);
        }

        protected override ResizeMode GetResizeMode(AbsoluteMouseEventArgs e)
        {
            return base.GetResizeMode(e);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
        }

        protected override void OnActivating(EventArgs e)
        {
            base.OnActivating(e);
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
        }

        protected override void OnDeactivating(EventArgs e)
        {
            base.OnDeactivating(e);
        }

        protected override void OnDeserializing(SerializeEventArgs e)
        {
            base.OnDeserializing(e);
        }

        protected override void OnDoubleClick(AbsoluteMouseEventArgs e)
        {
            base.OnDoubleClick(e);
        }

        protected override void OnDragging(MoveEventArgs e)
        {
            base.OnDragging(e);
        }

        protected override void OnModified(EventArgs e)
        {
            base.OnModified(e);
        }

        protected override void OnMouseDown(AbsoluteMouseEventArgs e)
        {
            base.OnMouseDown(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
        }

        protected override void OnMouseMove(AbsoluteMouseEventArgs e)
        {
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(AbsoluteMouseEventArgs e)
        {
            base.OnMouseUp(e);
        }

        protected override void OnMove(MoveEventArgs e)
        {
            base.OnMove(e);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
        }

        protected override void OnResizing(ResizeEventArgs e)
        {
            base.OnResizing(e);
        }

        protected override void OnSelectionChanged(EventArgs e)
        {
            base.OnSelectionChanged(e);
        }

        protected override void OnSerializing(SerializeEventArgs e)
        {
            base.OnSerializing(e);
        }

        protected internal override bool DeleteSelectedMember(bool showConfirmation)
        {
            return base.DeleteSelectedMember(showConfirmation);
        }

        protected internal override void DrawSelectionLines(Graphics g, float zoom, Point offset)
        {
            base.DrawSelectionLines(g, zoom, offset);
        }

        protected internal override IEnumerable<ToolStripItem> GetContextMenuItems(Diagram diagram)
        {
            return base.GetContextMenuItems(diagram);
        }

        protected internal override Size GetMaximalOffset(Size offset, int padding)
        {
            return base.GetMaximalOffset(offset, padding);
        }

        protected internal override void HideEditor()
        {
            base.HideEditor();
        }

        protected internal override void MoveWindow()
        {
            base.MoveWindow();
        }

        protected internal override void ShowEditor()
        {
            base.ShowEditor();
        }
    }
}
