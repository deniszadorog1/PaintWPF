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
using System.Runtime.Remoting;

namespace PaintWPF.CustomControls
{
    /// <summary>
    /// Логика взаимодействия для LineSizing.xaml
    /// </summary>
    public partial class LineSizing : UserControl
    {
        public LineSizingRectType _moveRect;
        public bool _isDraggingSelection;
        public bool IfSelectionIsClicked;
        private Point _startPointSelection;
        private Point _anchorPointSelection;

        public Polyline _sizingLine;
        private Label _lineSize;
        public LineSizing(Polyline line, Label lineSize)
        {
            _lineSize = lineSize;

            InitializeComponent();

            _sizingLine = line;
            InitLines();

            InitEvents();
        }
        public void InitLines()
        {
            Line.X1 = _sizingLine.Points.First().X;
            Line.X2 = _sizingLine.Points.Last().X;
            Line.Y1 = _sizingLine.Points.First().Y;
            Line.Y2 = _sizingLine.Points.Last().Y;

            Line.Stroke = _sizingLine.Stroke;
        }
        private void InitEvents()
        {
            Line.MouseLeftButtonDown += Line_MouseDown;
            Line.MouseMove += Line_MouseMove;
            Line.MouseLeftButtonUp += Line_MouseButtonUp;

            TransLine.MouseLeftButtonDown += Line_MouseDown;
            TransLine.MouseMove += Line_MouseMove;
            TransLine.PreviewMouseLeftButtonUp += Line_MouseButtonUp;

            Start.MouseLeftButtonDown += Start_MouseDown;
            End.MouseLeftButtonDown += End_MouseDown;

            Start.MouseUp += Rect_MouseUp;
            End.MouseUp += Rect_MouseUp;
        }
        private void Rect_MouseUp(object sender, MouseEventArgs e)
        {
            (sender as Rectangle).ReleaseMouseCapture();
        }
        private void Start_MouseDown(object sender, MouseEventArgs e)
        {
            IfSelectionIsClicked = true;
            _moveRect = LineSizingRectType.Start;
            (sender as Rectangle).CaptureMouse();
        }
        private void End_MouseDown(object sender, MouseEventArgs e)
        {
            IfSelectionIsClicked = true;
            _moveRect = LineSizingRectType.End;
            (sender as Rectangle).CaptureMouse();
        }
        private void Line_MouseDown(object sender, MouseEventArgs e)
        {
            _isDraggingSelection = true;
            _anchorPointSelection = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
            IfSelectionIsClicked = true;
            _startPointSelection = e.GetPosition(this.Parent as IInputElement);
            (sender as Line).CaptureMouse(); 
        }
        public void Line_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDraggingSelection)
            {
                ClipOutOfBoundariesGeo();

                if (ChangeSizeForSelection(e)) return;
                Point currentPoint = e.GetPosition(this.Parent as IInputElement);
                double offsetX = currentPoint.X - _startPointSelection.X;
                double offsetY = currentPoint.Y - _startPointSelection.Y;

                Canvas.SetLeft(this, _anchorPointSelection.X + offsetX);
                Canvas.SetTop(this, _anchorPointSelection.Y + offsetY);
            }
        }
        private void Line_MouseButtonUp(object sender, MouseEventArgs e)
        {
            //_isDraggingSelection = false;
            //(sender as Line).ReleaseMouseCapture();
        }
        public bool ChangeSizeForSelection(MouseEventArgs e)
        {
            if (_moveRect == LineSizingRectType.Nothing)
            {
                return false;
            }
            else if (_moveRect == LineSizingRectType.Start)
            {
                MoveStart(e);
            }
            else if (_moveRect == LineSizingRectType.End)
            {
                MoveEnd(e);
            }
            _lineSize.Content = $"{Math.Abs(Line.X2 - Line.X1)} x {Math.Abs(Line.Y2 - Line.Y1)} пкс";

            return true;
        }
        public void MoveStart(MouseEventArgs e)
        {
            Point temp = e.GetPosition(LineCanvas);

            Line.X1 = temp.X;
            Line.Y1 = temp.Y;

        }
        public void MoveEnd(MouseEventArgs e)
        {
            Point temp = e.GetPosition(LineCanvas);

            Line.X2 = temp.X;
            Line.Y2 = temp.Y;
        }
        public Line GetLineObject()
        {
            for(int i = 0; i < CheckLine.Children.Count; i++)
            {
                if (CheckLine.Children[i] is Line)
                {
                    return (Line)CheckLine.Children[i];
                }
            }
            return null;
        }
        public void RemoveLineFromCanvas()
        {
            CheckLine.Children.Remove(Line);
        }
        public void RemoveEveryThingExceptLine()
        {
            CheckLine.Children.Remove(TransLine);
            LineCanvas.Children.Remove(Start);
            LineCanvas.Children.Remove(End);
        }
        public void ClipOutOfBoundariesGeo()
        {
            Canvas parent = this.Parent as Canvas;
            if (parent is null) return;
            RectangleGeometry geo = new RectangleGeometry(new Rect(0, 0, parent.Width, parent.Height));
            CheckLine.Clip = geo;

            Point check = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
            SetRectGeometry(check, parent);
        }
        public void SetRectGeometry(Point point, Canvas parent)
        {
            if (CheckLine.Clip is null) return;


            Rect rect = CheckLine.Clip.Bounds;

            if (point.X < 0)
            {
                //rect.X = Math.Abs(point.X);
            }
            else if (point.X + this.Width > parent.Width)
            {
                double biggerWidth = this.Width - (point.X + this.Width - parent.Width);
                rect.Width = biggerWidth < 0 ? 0 : biggerWidth;
            }
            if (point.Y < 0)
            {
                //rect.Y = Math.Abs(point.Y);
            }
            else if (point.Y + this.Height > parent.Height)
            {
                double biggerHeight = this.Height - (point.Y + this.Height - parent.Height);
                rect.Height = biggerHeight < 0 ? 0 : biggerHeight;
            }
            RectangleGeometry geo = new RectangleGeometry(new Rect(rect.X - point.X, rect.Y - point.Y, rect.Width , rect.Height));
            CheckLine.Clip = geo;
        }
    }
}
