using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using PaintWPF.Models.Enums;
using PaintWPF.CustomControls;
using System.Globalization;
using PaintWPF.Models.Tools;
using System.Configuration;



namespace PaintWPF.CustomControls
{
    /// <summary>
    /// Логика взаимодействия для Selection.xaml
    /// </summary>
    public partial class Selection : UserControl
    {
        public bool _isDraggingSelection;
        public bool IfSelectionIsClicked = true;
        public bool _ifInvertedSelection;
        private Point _startPointSelection;
        private Point _anchorPointSelection;
        public SelectionSide _selectionSizeToChangeSize = SelectionSide.Nothing;
        private Image _figuresImg = null;
        private Label _selSize;

        private readonly Cursor _horizontalCurs = new Cursor(
            Application.GetResourceStream(new Uri(
                "Models/Cursors/Horizontal.cur", UriKind.Relative)).Stream);
        private Cursor _verticalCurs = new Cursor(
            Application.GetResourceStream(new Uri(
                "Models/Cursors/Vertical.cur", UriKind.Relative)).Stream);
        private Cursor _upLeftlCurs = new Cursor(
            Application.GetResourceStream(new Uri(
                "Models/Cursors/Diagonal1.cur", UriKind.Relative)).Stream);
        private Cursor _upRightlCurs = new Cursor(
            Application.GetResourceStream(new Uri(
                "Models/Cursors/Diagonal2.cur", UriKind.Relative)).Stream);
        public Cursor _moveCurs = new Cursor(
            Application.GetResourceStream(new Uri(
                "Models/Cursors/Move.cur", UriKind.Relative)).Stream);

        public Cursor _tempCursor;

        private (FigureTypes type, string data)? _figureParams = null;

        public Selection(Label selSize)
        {
            _selSize = selSize;

            InitializeComponent();
            InitEventsForSelection();
        }
        public Shape _shape;
        public Selection(Label selSize, Shape shape)
        {
            _selSize = selSize;
            _shape = shape;
            _shape.Stretch = Stretch.Fill;
            InitializeComponent();
            InitEventsForSelection();

            InitSizes();

            // ClipOutOfBoundariesGeo();
            //InitCursors();
        }
        public void InitCursors()
        {
            SelectCan.Cursor = _moveCurs;

            RightCenter.Cursor = _horizontalCurs;
            LeftCenter.Cursor = _horizontalCurs;

            RightBottom.Cursor = _upLeftlCurs;
            LeftTop.Cursor = _upLeftlCurs;

            RightTop.Cursor = _upRightlCurs;
            LeftBottom.Cursor = _upRightlCurs;

            CenterTop.Cursor = _verticalCurs;
            CenterBottom.Cursor = _verticalCurs;
        }
        private void InitSizes()
        {
            SelectCan.Height = _shape.Height;
            SelectCan.Width = _shape.Width;

            Height = _shape.Height;
            Width = _shape.Width;

            SelectionBorder.Height = _shape.Height;
            SelectionBorder.Width = _shape.Width;
        }
        public void SetMoveCursor()
        {
            Cursor = _moveCurs;
        }
        public Selection(Label selSize, (FigureTypes type, string data)? figParams)
        {
            _selSize = selSize;
            _figureParams = figParams;

            InitializeComponent();
            InitEventsForSelection();
        }
        private void InitEventsForSelection()
        {
            CheckCan.PreviewMouseDown += SelectionBorder_MouseLeftButtonDown;
            CheckCan.MouseMove += SelectionBorder_MouseMove;
            CheckCan.MouseUp += SelectionBorder_MouseLeftButtonUp;
            MouseLeave += SelectionBorder_MouseLeave;
            CheckCan.MouseDown += SelectCan_MouseLeftButtonDown;


            this.KeyDown += ReleseCapture_KeyDown;
            SelectCan.KeyDown += ReleseCapture_KeyDown;
            CheckCan.KeyDown += ReleseCapture_KeyDown;
            SelectionBorder.KeyDown += ReleseCapture_KeyDown;
            SizingGrid.KeyDown += ReleseCapture_KeyDown;

            LeftTop.MouseLeftButtonDown += SelectionLeftTop_MouseDown;
            CenterTop.MouseLeftButtonDown += SelectionCenterTop_MouseDown;
            RightTop.MouseLeftButtonDown += SelectionRightTop_MouseDown;
            RightCenter.MouseLeftButtonDown += SelectionRightCenter_MouseDown;
            RightBottom.MouseLeftButtonDown += SelectionRightBottom_MouseDown;
            CenterBottom.MouseLeftButtonDown += SelectionCenterBottom_MouseDown;
            LeftBottom.MouseLeftButtonDown += SelectionLeftBottom_MouseDown;
            LeftCenter.MouseLeftButtonDown += SelectionLeftCenter_MouseDown;
        }

        private void ReleseCapture_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                (sender as UIElement).ReleaseMouseCapture();
                SelectCan.ReleaseMouseCapture();
            }
        }
        private void SelectionBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDraggingSelection = false;
            (sender as UIElement).ReleaseMouseCapture();
            SelectCan.ReleaseMouseCapture();
            //_selectionType = SelectionType.Nothing;
        }
        private void SelectionBorder_MouseLeave(object sender, EventArgs e)
        {
            //_selectionSizeToChangeSize = SelectionSide.Nothing;
            //_isDraggingSelection = false;
        }
        private void SelectionLeftTop_MouseDown(object sender, MouseEventArgs e)
        {
            _selectionSizeToChangeSize = SelectionSide.LeftTop;
        }
        private void SelectionCenterTop_MouseDown(object sender, MouseEventArgs e)
        {
            _selectionSizeToChangeSize = SelectionSide.CenterTop;
        }
        private void SelectionRightTop_MouseDown(object sender, MouseEventArgs e)
        {
            _selectionSizeToChangeSize = SelectionSide.RightTop;
        }
        private void SelectionRightCenter_MouseDown(object sender, MouseEventArgs e)
        {
            _selectionSizeToChangeSize = SelectionSide.RightCenter;
        }
        private void SelectionRightBottom_MouseDown(object sender, MouseEventArgs e)
        {
            _selectionSizeToChangeSize = SelectionSide.RightBottom;
        }
        private void SelectionCenterBottom_MouseDown(object sender, MouseEventArgs e)
        {
            _selectionSizeToChangeSize = SelectionSide.CenterBottom;
        }
        private void SelectionLeftBottom_MouseDown(object sender, MouseEventArgs e)
        {
            _selectionSizeToChangeSize = SelectionSide.LeftBottom;
        }
        private void SelectionLeftCenter_MouseDown(object sender, MouseEventArgs e)
        {
            _selectionSizeToChangeSize = SelectionSide.LeftCenter;
        }
        private void SelectionBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_ifInvertedSelection) return;
            _isDraggingSelection = true;
            _anchorPointSelection = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
            IfSelectionIsClicked = true;
            _startPointSelection = e.GetPosition(this.Parent as IInputElement);
        }
        public void SelectionBorder_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed &&
                e.RightButton != MouseButtonState.Pressed && !this.IsMouseCaptured)
            {
                return;
            }

            Canvas parent = this.Parent as Canvas;

            CheckForRightClickMenu(parent);
            if (_isDraggingSelection &&
                CheckCan.Children.Contains(SizingGrid))
            {
                RichTextBox textBox = GetRichTextBox();
                if (textBox is null) (sender as UIElement).CaptureMouse();
                if (!(textBox is null) && textBox.IsFocused)
                {
                    (sender as UIElement).ReleaseMouseCapture();
                }

                Cursor = _moveCurs;
                /*                RectangleGeometry geo = new RectangleGeometry(new Rect(0, 0, parent.Width, parent.Height));
                                SelectCan.Clip = geo;*/
                ClipOutOfBoundariesGeo();

                Point currentPoint = e.GetPosition(this.Parent as IInputElement);

                /*                Point check = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
                                SetRectGeometry(check, parent);*/

                if (ChangeSizeForSelection(e)) return;

                double offsetX = currentPoint.X - _startPointSelection.X;
                double offsetY = currentPoint.Y - _startPointSelection.Y;

                Canvas.SetLeft(this, _anchorPointSelection.X + offsetX);
                Canvas.SetTop(this, _anchorPointSelection.Y + offsetY);
            }
        }
        public void ClipOutOfBoundariesGeo()
        {
            Canvas parent = this.Parent as Canvas;
            if (parent is null) return;
            RectangleGeometry geo = new RectangleGeometry(new Rect(0, 0, parent.Width, parent.Height));
            SelectCan.Clip = geo;

            Point check = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
            SetRectGeometry(check, parent);
        }
        private RichTextBox GetRichTextBox()
        {
            foreach (UIElement el in SelectCan.Children)
            {
                if (el is RichTextBox) return (RichTextBox)el;
            }
            return null;
        }
        private void CheckForRightClickMenu(Canvas parent)
        {
            if (parent is null) return;
            for (int i = 0; i < parent.Children.Count; i++)
            {
                if (parent.Children[i] is LeftClickSelectionMenu)
                {
                    _isDraggingSelection = false;
                    (CheckCan as UIElement).ReleaseMouseCapture();
                    return;
                }
            }
        }
        public void SetRectGeometry(Point point, Canvas parent)
        {
            if (SelectCan.Clip is null) return;
            if (this.Parent is null) return;
            Rect rect = SelectCan.Clip.Bounds;

            if (point.X < 0)
            {
                rect.X = Math.Abs(point.X) - DashedBorder.StrokeThickness;
            }
            else if (point.X + this.Width > parent.Width)
            {
                double biggerWidth = this.Width - (point.X + this.Width - parent.Width);
                rect.Width = biggerWidth < 0 ? 0 : biggerWidth;
            }
            if (point.Y < 0)
            {
                rect.Y = Math.Abs(point.Y) - DashedBorder.StrokeThickness;
            }
            else if (point.Y + this.Height > parent.Height)
            {
                double biggerHeight = this.Height - (point.Y + this.Height - parent.Height);
                rect.Height = biggerHeight < 0 ? 0 : biggerHeight;
            }
            RectangleGeometry geo = new RectangleGeometry(new Rect(rect.X, rect.Y, rect.Width, rect.Height));
            SelectCan.Clip = geo;
        }
        public bool ChangeSizeForSelection(MouseEventArgs e)
        {
            if (_selectionSizeToChangeSize == SelectionSide.Nothing)
            {
                return false;
            }
            if (_selectionSizeToChangeSize == SelectionSide.CenterTop)
            {
                ChangeCenterTop(e);
            }
            else if (_selectionSizeToChangeSize == SelectionSide.CenterBottom)
            {
                ChangeCenterBottom(e);
            }
            else if (_selectionSizeToChangeSize == SelectionSide.RightCenter)
            {
                ChangeRightCenter(e);
            }
            else if (_selectionSizeToChangeSize == SelectionSide.LeftCenter)
            {
                ChangeLeftCenter(e);
            }
            else if (_selectionSizeToChangeSize == SelectionSide.LeftTop)
            {
                ChangeLeftTop(e);
            }
            else if (_selectionSizeToChangeSize == SelectionSide.RightTop)
            {
                ChangeRightTop(e);
            }
            else if (_selectionSizeToChangeSize == SelectionSide.RightBottom)
            {
                ChangeRightBottom(e);
            }
            else if (_selectionSizeToChangeSize == SelectionSide.LeftBottom)
            {
                ChangeLeftBottom(e);
            }
            return true;
        }
        private const int _minSelSize = 5;
        private bool OutOfBoundariesResize(MouseEventArgs e, ResizeSelectionType resizeType)
        {
            Point point = e.GetPosition(this.Parent as IInputElement);
            Point selLoc = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));

            if ((resizeType == ResizeSelectionType.Width && selLoc.X > point.X) ||
               (resizeType == ResizeSelectionType.ReverseWidth && selLoc.X + this.Width < point.X))
            {
                if (resizeType == ResizeSelectionType.ReverseWidth)
                {
                    if (this.Width > _minSelSize)
                    {
                        Canvas.SetLeft(this, Canvas.GetLeft(this) + this.Width - _minSelSize);
                        this.Width = _minSelSize;
                        this.SelectionBorder.Width = _minSelSize;
                    }
                }
                else
                {
                    this.Width = _minSelSize;
                    this.SelectionBorder.Width = _minSelSize;
                }
                return true;
            }
            else if ((resizeType == ResizeSelectionType.Height && selLoc.Y > point.Y) ||
                    (resizeType == ResizeSelectionType.ReverseHeight && selLoc.Y + this.Height < point.Y))
            {
                if (resizeType == ResizeSelectionType.ReverseHeight)
                {
                    if (this.Height > _minSelSize)
                    {
                        Canvas.SetTop(this, Canvas.GetTop(this) + this.Height - _minSelSize);
                        this.Height = _minSelSize;
                        this.SelectionBorder.Height = _minSelSize;
                    }
                }
                else
                {
                    this.Height = _minSelSize;
                    this.SelectionBorder.Height = _minSelSize;
                }
                return true;
            }
            return false;
        }
        private void ChangeLeftBottom(MouseEventArgs e)
        {
            if (!CheckForMousePressing(e)) return;
            Point point = e.GetPosition(this.Parent as IInputElement);
            double newWidth = this.Width;
            double offsetX = point.X - _startPointSelection.X;
            if (!OutOfBoundariesResize(e, ResizeSelectionType.ReverseWidth))
            {
                if (point.X > _startPointSelection.X ||
                point.X < _startPointSelection.X)
                {
                    newWidth = this.Width - offsetX;
                }
                if (newWidth > _widthBlock)
                {
                    Canvas.SetLeft(this, Canvas.GetLeft(this) + offsetX);
                    this.Width = newWidth;
                    this.SelectionBorder.Width = newWidth;
                }
            }
            if (!OutOfBoundariesResize(e, ResizeSelectionType.Height))
            {
                double newHeight = this.Height;
                double offsetY = point.Y - _startPointSelection.Y;
                newHeight = this.Height + offsetY;
                if (newHeight > _widthBlock)
                {
                    this.Height = newHeight;
                    this.SelectionBorder.Height = newHeight;
                }
            }
            _startPointSelection = point;
            CheckForAddedObjects();

        }
        private void ChangeRightBottom(MouseEventArgs e)
        {
            if (!CheckForMousePressing(e)) return;
            Point point = e.GetPosition(this.Parent as IInputElement);

            if (!OutOfBoundariesResize(e, ResizeSelectionType.Width))
            {
                double newWidth = this.Width;
                double offsetX = point.X - _startPointSelection.X;
                newWidth = this.Width + offsetX;
                if (newWidth > _widthBlock)
                {
                    this.Width = newWidth;
                    this.SelectionBorder.Width = newWidth;
                }
            }

            if (!OutOfBoundariesResize(e, ResizeSelectionType.Height))
            {
                double newHeight = this.Height;
                double offsetY = point.Y - _startPointSelection.Y;
                newHeight = this.Height + offsetY;
                if (newHeight > _widthBlock)
                {
                    this.Height = newHeight;
                    this.SelectionBorder.Height = newHeight;
                }
            }
            _startPointSelection = point;
            CheckForAddedObjects();
        }
        private void ChangeRightTop(MouseEventArgs e)
        {
            if (!CheckForMousePressing(e)) return;

            Point point = e.GetPosition(this.Parent as IInputElement);

            if (!OutOfBoundariesResize(e, ResizeSelectionType.Width))
            {
                double newWidth = this.Width;
                double offsetX = point.X - _startPointSelection.X;
                newWidth = this.Width + offsetX;
                if (newWidth > _widthBlock)
                {
                    this.Width = newWidth;
                    this.SelectionBorder.Width = newWidth;
                }
            }
            if (!OutOfBoundariesResize(e, ResizeSelectionType.ReverseHeight))
            {
                double newHeight = this.Height;
                double offsetY = point.Y - _startPointSelection.Y;
                newHeight = this.Height - offsetY;
                if (newHeight > _widthBlock)
                {
                    Canvas.SetTop(this, Canvas.GetTop(this) + offsetY);
                    this.Height = newHeight;
                    this.SelectionBorder.Height = newHeight;
                }
            }
            _startPointSelection = point;
            CheckForAddedObjects();
        }
        private void ChangeLeftTop(MouseEventArgs e)
        {
            if (!CheckForMousePressing(e)) return;
            Point point = e.GetPosition(this.Parent as IInputElement);

            if (!OutOfBoundariesResize(e, ResizeSelectionType.ReverseWidth))
            {
                double newWidth = this.Width;
                double offsetX = point.X - _startPointSelection.X;
                newWidth = this.Width - offsetX;
                if (newWidth > _widthBlock)
                {
                    Canvas.SetLeft(this, Canvas.GetLeft(this) + offsetX);
                    this.Width = newWidth;
                    this.SelectionBorder.Width = newWidth;
                }
            }
            if (!OutOfBoundariesResize(e, ResizeSelectionType.ReverseHeight))
            {
                double newHeight = this.Height;
                double offsetY = point.Y - _startPointSelection.Y;
                newHeight = this.Height - offsetY;
                if (newHeight > _widthBlock)
                {
                    Canvas.SetTop(this, Canvas.GetTop(this) + offsetY);
                    this.Height = newHeight;
                    this.SelectionBorder.Height = newHeight;
                }
            }
            _startPointSelection = point;
            CheckForAddedObjects();
        }

        private const int _widthBlock = 5;
        public void ChangeLeftCenter(MouseEventArgs e)
        {
            if (!CheckForMousePressing(e)) return;
            if (OutOfBoundariesResize(e, ResizeSelectionType.ReverseWidth)) return;

            Point point = e.GetPosition(this.Parent as IInputElement);
            double newWidth = this.Width;
            double offsetX = point.X - _startPointSelection.X;
            newWidth = this.Width - offsetX;

            if (newWidth > _widthBlock)
            {
                Canvas.SetLeft(this, Canvas.GetLeft(this) + offsetX);
                this.Width = newWidth;
                this.SelectionBorder.Width = newWidth;
            }
            _startPointSelection = point;
            CheckForAddedObjects();
        }
        private (RotateTransform, ScaleTransform) GetTransforms()
        {
            UIElement elem = GetElemExceptGrid();

            if (elem is null) return (default, default);

            TransformGroup transforms = elem.RenderTransform as TransformGroup;

            RotateTransform rotateRes = transforms.Children.OfType<RotateTransform>().FirstOrDefault();
            ScaleTransform scaleRes = transforms.Children.OfType<ScaleTransform>().FirstOrDefault();

            return (rotateRes, scaleRes);

        }
        private UIElement GetElemExceptGrid()
        {
            for (int i = 0; i < SelectCan.Children.Count; i++)
            {
                if (SelectCan.Children[i].GetType() != typeof(Grid))
                {
                    return SelectCan.Children[i];
                }
            }
            return null;
        }
        public void ChangeRightCenter(MouseEventArgs e)
        {
            if (!CheckForMousePressing(e)) return;
            if (OutOfBoundariesResize(e, ResizeSelectionType.Width)) return;

            Point point = e.GetPosition(this.Parent as IInputElement);

            double newWidth = this.Width;
            double offsetX = point.X - _startPointSelection.X;

            newWidth = this.Width + offsetX;

            if (newWidth > _widthBlock)
            {
                this.Width = newWidth;
                this.SelectionBorder.Width = newWidth;
            }
            _startPointSelection = point;
            CheckForAddedObjects();
        }
        public void ChangeCenterBottom(MouseEventArgs e)
        {
            if (!CheckForMousePressing(e)) return;
            if (OutOfBoundariesResize(e, ResizeSelectionType.Height)) return;

            Point point = e.GetPosition(this.Parent as IInputElement);

            double heightPoint = Canvas.GetTop(this) + this.Height;

            double newHeight = this.Height;
            double offsetY = point.Y - _startPointSelection.Y;
            newHeight = this.Height + offsetY;
            if (newHeight < _widthBlock)
            {
                return;
            }
            if (newHeight > 0)
            {
                this.Height = newHeight;
                this.SelectionBorder.Height = newHeight;
            }
            _startPointSelection = point;
            CheckForAddedObjects();
        }
        public void ChangeCenterTop(MouseEventArgs e)
        {
            if (!CheckForMousePressing(e)) return;
            if (OutOfBoundariesResize(e, ResizeSelectionType.ReverseHeight)) return;

            Point point = e.GetPosition(this.Parent as IInputElement);
            double newHeight = this.Height;
            double offsetY = point.Y - _startPointSelection.Y;
            newHeight = this.Height - offsetY;
            if (newHeight > _widthBlock)
            {
                Canvas.SetTop(this, Canvas.GetTop(this) + offsetY);
                this.Height = newHeight;
                this.SelectionBorder.Height = newHeight;
            }
            _startPointSelection = point;
            CheckForAddedObjects();
        }
        private void CheckForAddedObjects()
        {
            ReinitShapeSize();
            CheckForTextBox();
            UpdateSizeLabel();
            CheckOutOfBorders();
        }
        private void CheckOutOfBorders()
        {
            Canvas parent = this.Parent as Canvas;
            Point check = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
            SetRectGeometry(check, parent);
        }
        private void UpdateSizeLabel()
        {
            _selSize.Content = $"{this.Width} x {this.Height} пкс";
        }
        public void CheckForTextBox()
        {
            const int locParam = 5;
            const int sizeDifParam = 15;
            int richTextBoxIndex = GetRichTextBoxIndex();
            if (richTextBoxIndex == -1) return;

            if (SelectionBorder.Height - sizeDifParam >= 0)
            {
                ((RichTextBox)SelectCan.Children[richTextBoxIndex]).Height = SelectionBorder.Height - sizeDifParam;
            }
            if (SelectionBorder.Width - sizeDifParam >= 0)
            {
                ((RichTextBox)SelectCan.Children[richTextBoxIndex]).Width = SelectionBorder.Width - sizeDifParam;
            }

            if (((RichTextBox)SelectCan.Children[richTextBoxIndex]).Width <= 5)
            {
                ((RichTextBox)SelectCan.Children[richTextBoxIndex]).Visibility = Visibility.Hidden;
            }
            else
            {
                ((RichTextBox)SelectCan.Children[richTextBoxIndex]).Visibility = Visibility.Visible;
            }

            Canvas.SetLeft(((RichTextBox)SelectCan.Children[richTextBoxIndex]), locParam);
            Canvas.SetTop(((RichTextBox)SelectCan.Children[richTextBoxIndex]), locParam);
        }
        public int GetRichTextBoxIndex()
        {
            for (int i = 0; i < SelectCan.Children.Count; i++)
            {
                if (SelectCan.Children[i] is RichTextBox)
                {
                    return i;
                }
            }
            return -1;
        }
        public RichTextBox GetRichTextBoxObject()
        {
            for (int i = 0; i < SelectCan.Children.Count; i++)
            {
                if (SelectCan.Children[i] is RichTextBox)
                {
                    return (RichTextBox)SelectCan.Children[i];
                }
            }
            return null;
        }

        public bool IfSelectiongClicked = true;
        private void SelectCan_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IfSelectiongClicked = true;

            HitTestResult result = VisualTreeHelper.HitTest(CheckCan, e.GetPosition(CheckCan));

            if (result != null && result.VisualHit is UIElement clickedElement)
            {

                if (clickedElement == CheckCan)
                {
                    SelectCan.CaptureMouse();
                }
                else if (clickedElement == SelectCan)
                {
                    SelectCan.CaptureMouse();

                }
            }
        }
        private void InitShapeFigure()
        {
            const int sizeDifPram = 15;
            SelectCan.Background = new ImageBrush()
            {
                ImageSource = _figuresImg.Source
            };

            SelectionBorder.Height = _figuresImg.Height + sizeDifPram;
            SelectionBorder.Width = _figuresImg.Width + sizeDifPram;
        }
        private void ReinitShapeSize()
        {
            const int sizeDifParam = 15;
            Image shape = GetShapeImageObject();
            if (shape is null) return;
            shape.Height = SelectionBorder.Height - sizeDifParam;
            shape.Width = SelectionBorder.Width - sizeDifParam;
        }
        public Image GetShapeImageObject()
        {
            for (int i = 0; i < SelectCan.Children.Count; i++)
            {
                if (SelectCan.Children[i] is Image)
                {
                    return (Image)SelectCan.Children[i];
                }
            }
            return null;
        }
        private bool CheckForMousePressing(MouseEventArgs e)
        {
            return e.LeftButton == MouseButtonState.Pressed;
        }
        public Point GetImageLocation()
        {
            for (int i = 0; i < SelectCan.Children.Count; i++)
            {
                if (SelectCan.Children[i] is Image)
                {
                    return new Point(Canvas.GetLeft(SelectCan.Children[i]),
                        Canvas.GetTop(SelectCan.Children[i]));
                }
            }
            return new Point(-1, -1);
        }
        public void RemoveImagesFromCanvas()
        {
            for (int i = 0; i < SelectCan.Children.Count; i++)
            {
                if (SelectCan.Children[i] is Image)
                {
                    SelectCan.Children.RemoveAt(i);
                }
            }
        }
        private void SelectionBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            const int _sizeDiffer = 0;
            if (SelectionBorder.Width - _sizeDiffer <= 0 ||
                SelectionBorder.Height - _sizeDiffer <= 0) return;

            SelectCan.Width = SelectionBorder.Width;
            SelectCan.Height = SelectionBorder.Height;

            CheckCan.Width = SelectionBorder.Width;
            CheckCan.Height = SelectionBorder.Height;

            SetSizeForSelectionLine();
        }
        private void SetSizeForSelectionLine()
        {
            for (int i = 0; i < CheckCan.Children.Count; i++)
            {
                if (CheckCan.Children[i] is Polyline)
                {
                    ((Polyline)CheckCan.Children[i]).Width = CheckCan.Width;
                    ((Polyline)CheckCan.Children[i]).Height = CheckCan.Height;
                }
            }
        }

        public void RemoveSizingGrid()
        {
            CheckCan.Children.Remove(SizingGrid);
        }
        public void AddSizingGrid()
        {
            //SelectCan.Children.Add(SizingGrid);
            CheckCan.Children.Add(SizingGrid);
        }
        public void RemoveShape()
        {
            SelectCan.Children.Remove(_shape);
        }
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize.Width == 0 || e.PreviousSize.Height == 0) return;

            double widthRatio = e.NewSize.Width / e.PreviousSize.Width;
            double heightRatio = e.NewSize.Height / e.PreviousSize.Height;

            foreach (UIElement child in SelectCan.Children)
            {
                if (child.GetType() != typeof(Grid) && child is FrameworkElement element)
                {
                    double newWidth = element.Width * widthRatio;
                    double newHeight = element.Height * heightRatio;

                    element.Width = newWidth;
                    element.Height = newHeight;

                    if (child is Selection)
                    {
                        ((Selection)child).SelectionBorder.Width = newWidth;
                        ((Selection)child).SelectionBorder.Height = newHeight;
                    }

                    if (Canvas.GetLeft(element) != double.NaN)
                    {
                        Canvas.SetLeft(element, Canvas.GetLeft(element) * widthRatio);
                    }
                    if (Canvas.GetTop(element) != double.NaN)
                    {
                        Canvas.SetTop(element, Canvas.GetTop(element) * heightRatio);
                    }
                }
            }
        }
        public void ChangeInvertionStatus()
        {
            _ifInvertedSelection = !_ifInvertedSelection;
        }
        private Shape _figToPaint;
        private void GetPathToPaint()
        {
            if (_figureParams is null) return;
            SelectCan.Background = null;
            SelectCan.Children.Remove(_figToPaint);

            const int pathThickness = 3;
            const int transformParam = 1;
            _figToPaint = new Path()
            {
                Stroke = Brushes.Black,
                StrokeThickness = pathThickness,
                RenderTransform = new ScaleTransform(transformParam, transformParam),
                Stretch = Stretch.Fill
            };
            ((System.Windows.Shapes.Path)_figToPaint).Data =
            Geometry.Parse(_figureParams.Value.data);

            _figToPaint.Width = SelectCan.Width;
            _figToPaint.Height = SelectCan.Height;

            SelectCan.Children.Add(_figToPaint);
        }
        public UIElement GetShapeElement()
        {
            for (int i = 0; i < SelectCan.Children.Count; i++)
            {
                if (SelectCan.Children[i].GetType() != typeof(Grid))
                {
                    if (SelectCan.Children[i] == _figToPaint)
                    {
                        Console.WriteLine();
                    }
                    return SelectCan.Children[i];
                }
            }
            return null;
        }
        public void RemoveObj()
        {
            for (int i = 0; i < SelectCan.Children.Count; i++)
            {
                if (SelectCan.Children[i].GetType() != typeof(Grid))
                {
                    SelectCan.Children.RemoveAt(i);
                }
            }
        }
        private bool _ifMoveCursMouseEnter;
        private void LeftTopCursor_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!(_tempCursor is null)) return;
            _ifMoveCursMouseEnter = false;
            Cursor = _upLeftlCurs;
        }
        private void RightTopCursor_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!(_tempCursor is null)) return;
            Cursor = _upRightlCurs;
            _ifMoveCursMouseEnter = false;
        }
        private void VerticalCursor_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!(_tempCursor is null)) return;
            Cursor = _verticalCurs;
            _ifMoveCursMouseEnter = false;
        }
        private void HorizontalCursor_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!(_tempCursor is null)) return;
            Cursor = _horizontalCurs;
            _ifMoveCursMouseEnter = false;
        }
        private void MoveCursor_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!(_tempCursor is null)) return;
            if (!_ifMoveCursMouseEnter)
            {
                _ifMoveCursMouseEnter = true;
                return;
            }
            Cursor = _moveCurs;
        }
        public bool _ifMoveCursFlagouseDown;
        private void LeftTopCurosor_MouseDown(object sender, MouseEventArgs e)
        {
            ReassignCursor(_upLeftlCurs);
            _ifMoveCursFlagouseDown = false;
        }
        private void RightTopCursor_MouseDown(object sender, MouseEventArgs e)
        {
            ReassignCursor(_upRightlCurs);
            _ifMoveCursFlagouseDown = false;
        }
        private void VerticalCursor_MouseDown(object sender, MouseEventArgs e)
        {
            ReassignCursor(_verticalCurs);
            _ifMoveCursFlagouseDown = false;
        }
        private void HorizontalCursor_MouseDown(object sender, MouseEventArgs e)
        {
            ReassignCursor(_horizontalCurs);
            _ifMoveCursFlagouseDown = false;
        }
        private void MoveCursor_MouseDown(object sender, MouseEventArgs e)
        {
            if (!_ifMoveCursFlagouseDown) return;
            ReassignCursor(_moveCurs);
        }
        private void ReassignCursor(Cursor curs)
        {
            _tempCursor = curs;
            Cursor = curs;
        }
    }
}
