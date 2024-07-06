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
using Xceed.Wpf.Toolkit.PropertyGrid.Converters;

using PaintWPF.Models.Enums;

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
        public LineSizing(Polyline line)
        {
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
        }
        private void Start_MouseDown(object sender, MouseEventArgs e)
        {
            IfSelectionIsClicked = true;
            _moveRect = LineSizingRectType.Start;
        }
        private void End_MouseDown(object sender, MouseEventArgs e)
        {
            IfSelectionIsClicked = true;
            _moveRect = LineSizingRectType.End;
        }
        private void Line_MouseDown(object sender, MouseEventArgs e)
        {
            _isDraggingSelection = true;
            _anchorPointSelection = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
            IfSelectionIsClicked = true;
            _startPointSelection = e.GetPosition(this.Parent as IInputElement);
        }
        private void Line_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDraggingSelection)
            {
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
            _isDraggingSelection = false;
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
        public Line GetPolyLineObject()
        {
            for(int i = 0; i < LineCanvas.Children.Count; i++)
            {
                if (LineCanvas.Children[i] is Line)
                {
                    return (Line)LineCanvas.Children[i];
                }
            }
            return null;
        }
        public void RemoveLineFromCanvas()
        {
            LineCanvas.Children.Remove(Line);
        }
    }
}
