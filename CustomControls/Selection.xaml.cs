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
        public SelectionSide _selectionSizeToChangeSize;
        private Image _figuresImg = null;

        public Selection()
        {
            InitializeComponent();
            InitEventsForSelection();
        }
        public Selection(Image image)
        {
            _figuresImg = image;

            InitializeComponent();
            InitEventsForSelection();
            InitShapeFigure();
        }
        private void InitEventsForSelection()
        {
            SelectCan.MouseLeftButtonDown += SelectionBorder_MouseLeftButtonDown;
            SelectCan.MouseMove += SelectionBorder_MouseMove;
            SelectCan.MouseLeftButtonUp += SelectionBorder_MouseLeftButtonUp;
            MouseLeave += SelectionBorder_MouseLeave;

            LeftTop.MouseLeftButtonDown += SelectionLeftTop_MouseDown;
            CenterTop.MouseLeftButtonDown += SelectionCenterTop_MouseDown;
            RightTop.MouseLeftButtonDown += SelectionRightTop_MouseDown;
            RightCenter.MouseLeftButtonDown += SelectionRightCenter_MouseDown;
            RightBottom.MouseLeftButtonDown += SelectionRightBottom_MouseDown;
            CenterBottom.MouseLeftButtonDown += SelectionCenterBottom_MouseDown;
            LeftBottom.MouseLeftButtonDown += SelectionLeftBottom_MouseDown;
            LeftCenter.MouseLeftButtonDown += SelectionLeftCenter_MouseDown;
        }
        private void SelectionBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDraggingSelection = false;
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
        private void SelectionBorder_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDraggingSelection && SelectCan.Children.Contains(SizingGrid))
            {
                if (ChangeSizeForSelection(e)) return;
                Point currentPoint = e.GetPosition(this.Parent as IInputElement);
                double offsetX = currentPoint.X - _startPointSelection.X;
                double offsetY = currentPoint.Y - _startPointSelection.Y;

                Canvas.SetLeft(this, _anchorPointSelection.X + offsetX);
                Canvas.SetTop(this, _anchorPointSelection.Y + offsetY);
            }
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
        private void ChangeLeftBottom(MouseEventArgs e)
        {
            Point tempPoint = e.GetPosition(this.Parent as IInputElement);
            if (tempPoint.X > _startPointSelection.X || tempPoint.X < _startPointSelection.X)
            {
                ChangeLeftCenter(e);
                return;
            }
            ChangeCenterBottom(e);
        }
        private void ChangeRightBottom(MouseEventArgs e)
        {
            Point tempPoint = e.GetPosition(this.Parent as IInputElement);
            if (tempPoint.X > _startPointSelection.X || tempPoint.X < _startPointSelection.X)
            {
                ChangeRightCenter(e);
                return;
            }
            ChangeCenterBottom(e);
        }
        private void ChangeRightTop(MouseEventArgs e)
        {
            Point tempPoint = e.GetPosition(this.Parent as IInputElement);
            if (tempPoint.X > _startPointSelection.X || tempPoint.X < _startPointSelection.X)
            {
                ChangeRightCenter(e);
                return;
            }
            ChangeCenterTop(e);
        }
        private void ChangeLeftTop(MouseEventArgs e)
        {
            Point tempPoint = e.GetPosition(this.Parent as IInputElement);
            if (tempPoint.X > _startPointSelection.X || tempPoint.X < _startPointSelection.X)
            {
                ChangeLeftCenter(e);
                return;
            }
            ChangeCenterTop(e);
        }
        public void ChangeLeftCenter(MouseEventArgs e)
        {
            if (!CheckForMousePressing(e)) return;

            Point point = e.GetPosition(this.Parent as IInputElement);
            double newWidth = this.Width;
            double offsetX = point.X - _startPointSelection.X;

            if (point.X > _startPointSelection.X)
            {
                newWidth = this.Width - offsetX;
            }
            else if (point.X < _startPointSelection.X)
            {
                newWidth = this.Width - offsetX;
            }
            if (newWidth < 25)
            {
                return;
            }
            if (newWidth > 0)
            {
                Canvas.SetLeft(this, Canvas.GetLeft(this) + offsetX);
                this.Width = newWidth;
                this.SelectionBorder.Width = newWidth;
            }
            _startPointSelection = point;
            CheckForAddedObjects();
        }
        public void ChangeRightCenter(MouseEventArgs e)
        {
            if (!CheckForMousePressing(e)) return;

            Point point = e.GetPosition(this.Parent as IInputElement);
            double widthPoint = Canvas.GetLeft(this) + this.Width;

            double newWidth = this.Width;
            double offsetX = point.X - _startPointSelection.X;

            if (widthPoint > _startPointSelection.X)
            {
                newWidth = this.Width + offsetX;
            }
            else if (widthPoint < _startPointSelection.X)
            {
                newWidth = this.Width - offsetX;
            }
            if (newWidth < 25)
            {
                return;
            }
            if (newWidth > 0)
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

            Point point = e.GetPosition(this.Parent as IInputElement);

            double heightPoint = Canvas.GetTop(this) + this.Height;

            double newHeight = this.Height;
            double offsetY = point.Y - _startPointSelection.Y;

            if (heightPoint > _startPointSelection.Y)
            {
                newHeight = this.Height + offsetY;
            }
            else if (heightPoint < _startPointSelection.Y)
            {
                newHeight = this.Height + offsetY;
            }
            if (newHeight < 25)
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

            Point point = e.GetPosition(this.Parent as IInputElement);
            double newHeight = this.Height;
            double offsetY = point.Y - _startPointSelection.Y;

            if (point.Y > _startPointSelection.Y)
            {
                newHeight = this.Height - offsetY;
            }
            else if (point.Y < _startPointSelection.Y)
            {
                newHeight = this.Height - offsetY;
            }
            if (newHeight < 25)
            {
                return;
            }
            if (newHeight > 0)
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
        }
        public void CheckForTextBox()
        {
            int richTextBoxIndex = GetRichTextBoxIndex();
            if (richTextBoxIndex == -1) return;
            ((RichTextBox)SelectCan.Children[richTextBoxIndex]).Height = SelectionBorder.Height - 15;
            ((RichTextBox)SelectCan.Children[richTextBoxIndex]).Width = SelectionBorder.Width - 15;
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
        }
        private void InitShapeFigure()
        {
            SelectCan.Background = new ImageBrush()
            {
                ImageSource = _figuresImg.Source
            };

            SelectionBorder.Height = _figuresImg.Height + 15;
            SelectionBorder.Width = _figuresImg.Width + 15;
        }
        private void ReinitShapeSize()
        {
            Image shape = GetShapeImageObject();
            if (shape is null) return;
            shape.Height = SelectionBorder.Height - 15;
            shape.Width = SelectionBorder.Width - 15;
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
            const int _sizeDiffer = 5;
            if (SelectionBorder.Width - _sizeDiffer <= 0 || SelectionBorder.Height - _sizeDiffer <= 0) return;
            SelectCan.Width = SelectionBorder.Width - _sizeDiffer;
            SelectCan.Height = SelectionBorder.Height - _sizeDiffer;
        }
        public void RemoveSizingGrid()
        {
            SelectCan.Children.Remove(SizingGrid);
            //SizingGrid.Children.Clear();
        }
        public void AddSizingGrid()
        {
            SelectCan.Children.Add(SizingGrid);
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize.Width == 0 || e.PreviousSize.Height == 0) return;

            double widthRatio = e.NewSize.Width / e.PreviousSize.Width;
            double heightRatio = e.NewSize.Height / e.PreviousSize.Height;

            foreach (UIElement child in SelectCan.Children)
            {           
                if (child.GetType() != typeof(Grid) &&  child is FrameworkElement element)
                {
                    double newWidth = element.Width * widthRatio;
                    double newHeight = element.Height * heightRatio;

                    // Обновляем размеры элемента
                    element.Width = newWidth;
                    element.Height = newHeight;

                    if(child is Selection)
                    {
                        ((Selection)child).SelectionBorder.Width = newWidth;
                        ((Selection)child).SelectionBorder.Height = newHeight;
                    }


                    // Обновляем позицию элемента, если это Canvas
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
    }
}
