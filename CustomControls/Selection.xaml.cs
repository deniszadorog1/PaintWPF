using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

using PaintWPF.Models.Enums;


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


        private ScrollViewer _viewer;
        public Selection(Label selSize, ScrollViewer viewer)
        {
            _selSize = selSize;
            _viewer = viewer;

            InitializeComponent();
            InitEventsForSelection();
        }
        public Shape _shape;
        public Selection(Label selSize, Shape shape)
        {
            _selSize = selSize;
            _shape = shape;

            InitializeComponent();
            InitEventsForSelection();

            InitSizes();
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
            Size shapeSize = new Size(_shape.Width, _shape.Height);

            SelectCan.Height = shapeSize.Height;
            SelectCan.Width = shapeSize.Width;

            Height = shapeSize.Height;
            Width = shapeSize.Width;

            SelectionBorder.Height = shapeSize.Height;
            SelectionBorder.Width = shapeSize.Width;
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
                ClipOutOfBoundariesGeoInDeep(new Point(0, 0), (Canvas)this.Parent, this);
                Point currentPoint = e.GetPosition(this.Parent as IInputElement);
                if (ChangeSizeForSelection(e)) return;

                double offsetX = currentPoint.X - _startPointSelection.X;
                double offsetY = currentPoint.Y - _startPointSelection.Y;

                Canvas.SetLeft(this, _anchorPointSelection.X + offsetX);
                Canvas.SetTop(this, _anchorPointSelection.Y + offsetY);
            }
        }
        private void ClipOutOfBoundariesGeoInDeep(Point tempLoc, Canvas drawingCanvas, Selection sel)
        {
            if (sel is null) return;
            Canvas tempSelParent = (Canvas)sel.Parent;
            if (tempSelParent is null) return;
            RectangleGeometry geo = new RectangleGeometry(new Rect(0, 0, drawingCanvas.Width, drawingCanvas.Height));
            sel.BgCanvas.Clip = geo;

            Point check = new Point(Canvas.GetLeft(sel) + tempLoc.X, Canvas.GetTop(sel) + tempLoc.Y);
            SetRectGeometry(check, drawingCanvas, sel);
            sel.BgCanvas.Width = sel.Width;
            sel.BgCanvas.Height = sel.Height;

            ClipOutOfBoundariesGeoInDeep(check, drawingCanvas, sel.SelectCan.Children.OfType<Selection>().FirstOrDefault());
        }
        public void ClipOutOfBoundariesGeo()
        {
            this.SelectCan.Background = null;

            Canvas parent = this.Parent as Canvas;
            if (parent is null) return;
            RectangleGeometry geo = new RectangleGeometry(new Rect(0, 0, parent.Width, parent.Height));
            BgCanvas.Clip = geo;

            Point check = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
            SetRectGeometry(check, parent, this);
        }
        private RichTextBox GetRichTextBox()
        {
            return SelectCan.Children.OfType<RichTextBox>().FirstOrDefault();
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
        public void SetRectGeometry(Point point, Canvas parent, Selection sel)
        {
            if (sel.BgCanvas.Clip is null || parent is null) return;
            Rect rect = sel.BgCanvas.Clip.Bounds;

            if (point.X < 0)
            {
                rect.X = Math.Abs(point.X) - sel.DashedBorder.StrokeThickness;
            }
            else if (point.X + sel.Width > parent.Width)
            {
                double biggerWidth = sel.Width - (point.X + sel.Width - parent.Width);
                rect.Width = biggerWidth < 0 ? 0 : biggerWidth;
            }
            if (point.Y < 0)
            {
                rect.Y = Math.Abs(point.Y) - sel.DashedBorder.StrokeThickness;
            }
            else if (point.Y + sel.Height > parent.Height)
            {
                double biggerHeight = sel.Height - (point.Y + sel.Height - parent.Height);
                rect.Height = biggerHeight < 0 ? 0 : biggerHeight;
            }
            if (sel.CheckCan.Children.OfType<Polyline>().Any())
            {
                double xOffsetValue = Math.Abs(_viewer.HorizontalOffset);
                rect.Width += xOffsetValue;
            }
            RectangleGeometry geo = new RectangleGeometry(new Rect(rect.X, rect.Y, rect.Width, rect.Height));
            sel.BgCanvas.Clip = geo;
        }
        public bool ChangeSizeForSelection(MouseEventArgs e)
        {
            switch (_selectionSizeToChangeSize)
            {
                case SelectionSide.LeftTop:
                    ChangeLeftTop(e);
                    break;
                case SelectionSide.CenterTop:
                    ChangeCenterTop(e);
                    break;
                case SelectionSide.RightTop:
                    ChangeRightTop(e);
                    break;
                case SelectionSide.RightCenter:
                    ChangeRightCenter(e);
                    break;
                case SelectionSide.RightBottom:
                    ChangeRightBottom(e);
                    break;
                case SelectionSide.CenterBottom:
                    ChangeCenterBottom(e);
                    break;
                case SelectionSide.LeftBottom:
                    ChangeLeftBottom(e);
                    break;
                case SelectionSide.LeftCenter:
                    ChangeLeftCenter(e);
                    break;
                default: return false;
            }

            ChangeSelectionLineSettings();

            this.BgCanvas.Width = this.Width;
            this.BgCanvas.Height = this.Height;

            if (!(_shape is null))
            {
                _shape.Stretch = Stretch.Fill;
                _shape.Width = this.Width;
                _shape.Height = this.Height;
            }
            return true;
        }

        public void ChangePointCordesAfterChenageLineSize()
        {
            Polyline line = CheckCan.Children.OfType<Polyline>().FirstOrDefault();
            if (line is null) return;

            double minX = line.Points.Min(p => p.X);
            double minY = line.Points.Min(p => p.Y);
            double maxX = line.Points.Max(p => p.X);
            double maxY = line.Points.Max(p => p.Y);

            const double sizeCorrelParam = 0;

            double originalWidth = maxX - minX + sizeCorrelParam;
            double originalHeight = maxY - minY + sizeCorrelParam;
            double newWidth = line.Width;
            double newHeight = line.Height;

            double scaleX;
            double scaleY;

            scaleX = newWidth / originalWidth;
            scaleY = newHeight / originalHeight;

            PointCollection asd = line.Points;

            for (int i = 0; i < line.Points.Count; i++)
            {
                Point oldPoint = line.Points[i];
                double newX = (oldPoint.X - minX) * scaleX + minX;
                double newY = (oldPoint.Y - minY) * scaleY + minY;
                line.Points[i] = new Point((int)(newX), (int)(newY));
            }
            line.InvalidateVisual();
        }
        public void ChangeSelectionLineSettings()
        {
            Polyline line = CheckCan.Children.OfType<Polyline>().FirstOrDefault();
            if (line is null) return;
            //Polyline line = CheckCan.Children.OfType<Polyline>().First();
            line.Stretch = Stretch.Fill;

            Canvas.SetLeft(line, 0);
            Canvas.SetTop(line, 0);
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
                if (newWidth > _sizeBlock)
                {
                    Canvas.SetLeft(this, Canvas.GetLeft(this) + offsetX);
                    this.Width = newWidth;
                    this.SelectionBorder.Width = newWidth;
                }
            }
            if (!OutOfBoundariesResize(e, ResizeSelectionType.Height))
            {
                double offsetY = point.Y - _startPointSelection.Y;
                double newHeight = this.Height + offsetY;
                if (newHeight > _sizeBlock)
                {
                    this.Height = newHeight;
                    this.SelectionBorder.Height = newHeight;
                }
            }
            _startPointSelection = point;
            CheckForAddedObjects();
        }

        private void ResizeIfOutOfBoundariesWidthRightHorizontal(MouseEventArgs e, Point point)
        {
            if (!OutOfBoundariesResize(e, ResizeSelectionType.Width))
            {
                double offsetX = point.X - _startPointSelection.X;
                double newWidth = this.Width + offsetX;
                if (newWidth > _sizeBlock)
                {
                    this.Width = newWidth;
                    this.SelectionBorder.Width = newWidth;
                }
            }
        }
        private void ResizeIfOutOfBoundariesHeightRightHorizontal(double newHeight)
        {
            if (newHeight > _sizeBlock)
            {
                this.Height = newHeight;
                this.SelectionBorder.Height = newHeight;
            }
        }


        private void ChangeRightBottom(MouseEventArgs e)
        {
            if (!CheckForMousePressing(e)) return;
            Point point = e.GetPosition(this.Parent as IInputElement);

            ResizeIfOutOfBoundariesWidthRightHorizontal(e, point);
            /*             if (!OutOfBoundariesResize(e, ResizeSelectionType.Width))
                        {
                            double offsetX = point.X - _startPointSelection.X;
                            double newWidth = this.Width + offsetX;
                            if (newWidth > _widthBlock)
                            {
                                this.Width = newWidth;
                                this.SelectionBorder.Width = newWidth;
                            }
                        }*/
            if (!OutOfBoundariesResize(e, ResizeSelectionType.Height))
            {
                double newHeight = this.Height;
                double offsetY = point.Y - _startPointSelection.Y;
                newHeight = this.Height + offsetY;
                //ResizeIfOutOfBoundariesHeightRightHorizontal(newHeight);
                if (newHeight > _sizeBlock)
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

            ResizeIfOutOfBoundariesWidthRightHorizontal(e, point);
            /*  if (!OutOfBoundariesResize(e, ResizeSelectionType.Width))
        {
            double newWidth = this.Width;
            double offsetX = point.X - _startPointSelection.X;
            newWidth = this.Width + offsetX;
            if (newWidth > _widthBlock)
            {
                this.Width = newWidth;
                this.SelectionBorder.Width = newWidth;
            }
        }*/
            if (!OutOfBoundariesResize(e, ResizeSelectionType.ReverseHeight))
            {
                double newHeight = this.Height;
                double offsetY = point.Y - _startPointSelection.Y;
                newHeight = this.Height - offsetY;
                if (newHeight > _sizeBlock)
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
                OutOfBorederRight(point);
            }
            if (!OutOfBoundariesResize(e, ResizeSelectionType.ReverseHeight))
            {
                double offsetY = point.Y - _startPointSelection.Y;
                double newHeight = this.Height - offsetY;
                if (newHeight > _sizeBlock)
                {
                    Canvas.SetTop(this, Canvas.GetTop(this) + offsetY);
                    this.Height = newHeight;
                    this.SelectionBorder.Height = newHeight;
                }
            }
            _startPointSelection = point;
            CheckForAddedObjects();
        }
        private void OutOfBorederRight(Point point)
        {
            double newWidth = this.Width;
            double offsetX = point.X - _startPointSelection.X;
            newWidth = this.Width - offsetX;

            if (newWidth > _sizeBlock)
            {
                Canvas.SetLeft(this, Canvas.GetLeft(this) + offsetX);
                this.Width = newWidth;
                this.SelectionBorder.Width = newWidth;
            }
        }
        private const int _sizeBlock = 5;
        public void ChangeLeftCenter(MouseEventArgs e)
        {
            if (!CheckForMousePressing(e)) return;
            if (OutOfBoundariesResize(e, ResizeSelectionType.ReverseWidth)) return;

            Point point = e.GetPosition(this.Parent as IInputElement);
            OutOfBorederRight(point);

            _startPointSelection = point;
            CheckForAddedObjects();
        }
        public void ChangeRightCenter(MouseEventArgs e)
        {
            if (!CheckForMousePressing(e)) return;
            if (OutOfBoundariesResize(e, ResizeSelectionType.Width)) return;

            Point point = e.GetPosition(this.Parent as IInputElement);

            double newWidth = this.Width;
            double offsetX = point.X - _startPointSelection.X;

            newWidth = this.Width + offsetX;

            if (newWidth > _sizeBlock)
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

            double offsetY = point.Y - _startPointSelection.Y;
            double newHeight = this.Height + offsetY;
            if (newHeight < _sizeBlock)
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
            double offsetY = point.Y - _startPointSelection.Y;
            double newHeight = this.Height - offsetY;
            if (newHeight > _sizeBlock)
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
            SetRectGeometry(check, parent, this);
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

            if (((RichTextBox)SelectCan.Children[richTextBoxIndex]).Width <= locParam)
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
            return SelectCan.Children.OfType<Image>().FirstOrDefault();
            /*
                        for (int i = 0; i < SelectCan.Children.Count; i++)
                        {
                            if (SelectCan.Children[i] is Image)
                            {
                                return (Image)SelectCan.Children[i];
                            }
                        }
                        return null;*/
        }
        private bool CheckForMousePressing(MouseEventArgs e)
        {
            return e.LeftButton == MouseButtonState.Pressed;
        }
        public Point GetImageLocation()
        {
            Point startLoc = new Point(-1, -1);
            for (int i = 0; i < SelectCan.Children.Count; i++)
            {
                if (SelectCan.Children[i] is Image)
                {
                    return new Point(Canvas.GetLeft(SelectCan.Children[i]),
                        Canvas.GetTop(SelectCan.Children[i]));
                }
            }
            return startLoc;
        }
        public void RemoveImagesFromCanvas()
        {
            List<Image> imgs = new List<Image>();
            for (int i = 0; i < SelectCan.Children.Count; i++)
            {
                if (SelectCan.Children[i] is Image img)
                {
                    imgs.Add(img);
                }
            }
            foreach (Image tempImg in imgs)
            {
                SelectCan.Children.Remove(tempImg);
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
            CheckCan.Children.Add(SizingGrid);
        }
        public void RemoveShape()
        {
            BgCanvas.Children.Remove(_shape);
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
            ((Path)_figToPaint).Data =
            Geometry.Parse(_figureParams.Value.data);

            _figToPaint.Width = SelectCan.Width;
            _figToPaint.Height = SelectCan.Height;

            SelectCan.Children.Add(_figToPaint);
        }
        public UIElement GetShapeElement()
        {
            for (int i = 0; i < BgCanvas.Children.Count; i++)
            {
                if (BgCanvas.Children[i].GetType() != typeof(Grid))
                {
                    return BgCanvas.Children[i];
                }
            }
            return null;
        }
        public void RemoveShapea()
        {
            for (int i = 0; i < BgCanvas.Children.Count; i++)
            {
                if (BgCanvas.Children[i].GetType() != typeof(Grid))
                {
                    BgCanvas.Children.RemoveAt(i);
                }
            }
        }

        private bool _ifMoveCursMouseEnter;
        private void LeftTopCursor_MouseEnter(object sender, MouseEventArgs e)
        {
            ChangeCursor(_upLeftlCurs);
            /*            if (!(_tempCursor is null)) return;
                        _ifMoveCursMouseEnter = false;
                        Cursor = _upLeftlCurs;*/
        }
        private void RightTopCursor_MouseEnter(object sender, MouseEventArgs e)
        {
            ChangeCursor(_upRightlCurs);
            /*            if (!(_tempCursor is null)) return;
                        Cursor = _upRightlCurs;
                        _ifMoveCursMouseEnter = false;*/
        }
        private void VerticalCursor_MouseEnter(object sender, MouseEventArgs e)
        {
            ChangeCursor(_verticalCurs);
            /*            if (!(_tempCursor is null)) return;
                        Cursor = _verticalCurs;
                        _ifMoveCursMouseEnter = false;*/
        }
        private void HorizontalCursor_MouseEnter(object sender, MouseEventArgs e)
        {
            ChangeCursor(_horizontalCurs);
            /*            if (!(_tempCursor is null)) return;
                        Cursor = _horizontalCurs;
                        _ifMoveCursMouseEnter = false;*/
        }

        public void ChangeCursor(Cursor cursor)
        {
            if (!(_tempCursor is null)) return;
            Cursor = cursor;
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
            CursorMouseDown(_upLeftlCurs);
            /*            ReassignCursor(_upLeftlCurs);
                        _ifMoveCursFlagouseDown = false;*/
        }
        private void RightTopCursor_MouseDown(object sender, MouseEventArgs e)
        {
            CursorMouseDown(_upRightlCurs);
            /*            ReassignCursor(_upRightlCurs);
                        _ifMoveCursFlagouseDown = false;*/
        }
        private void VerticalCursor_MouseDown(object sender, MouseEventArgs e)
        {
            CursorMouseDown(_verticalCurs);
            /*            ReassignCursor(_verticalCurs);
                        _ifMoveCursFlagouseDown = false;*/
        }
        private void HorizontalCursor_MouseDown(object sender, MouseEventArgs e)
        {
            CursorMouseDown(_horizontalCurs);
            /*            ReassignCursor(_horizontalCurs);
                        _ifMoveCursFlagouseDown = false;*/
        }

        public void CursorMouseDown(Cursor cursor)
        {
            ReassignCursor(cursor);
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
        public SolidColorBrush GetShapeColor()
        {
            return (SolidColorBrush)_shape.Stroke;
        }
        public void SetStrokeForFigure(SolidColorBrush brush)
        {
            _shape.Stroke = brush;
        }
        public void ClearDragging()
        {
            _isDraggingSelection = false;
            IfSelectionIsClicked = false;
            _selectionSizeToChangeSize = SelectionSide.Nothing;
        }
    }
}
