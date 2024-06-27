using System;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Ink;
using System.Collections.ObjectModel;
using System.Linq;
using PaintWPF.Models;
using PaintWPF.Models.Enums;
using System.IO;
using System.Windows.Threading;
using Microsoft.Win32;

using PaintWPF.Models.Tools;


using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;


namespace PaintWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainPaint _main = new MainPaint();

        private List<Button> _tools = new List<Button>();
        private List<MenuItem> _brushTypes = new List<MenuItem>();
        private Button _chosenTool = null;

        private ActionType _type = ActionType.Nothing;
        private SelectionType _selectionType = SelectionType.Nothing;

        private Rectangle _selectionRect = new Rectangle();

        private bool IfDrawing = false;
        private bool IfFilling = false;
        private bool IfErazing = false;
        private bool IfFiguring = false;
        private bool IfSelection = false;


        private FigureTypes? _figType = null;
        private Shape _figToPaint;
        private Polyline poligonFigure = null;

        private Point previousPoint;
        private int brushThickness = 2;

        private bool IfShowBrushSize = false;


        private UIElement valueDragElem = null;
        private Point valueOffset;

        private int sprayDensity = 30;
        private Random random = new Random();
        private DispatcherTimer sprayTimer;
        private Point currentPoint;

        private List<Polyline> polylines = new List<Polyline>();
        private List<Polygon> polygons = new List<Polygon>();

        private readonly SolidColorBrush _clickedBorderColor = 
            new SolidColorBrush(Color.FromRgb(0, 103, 192));

        private const double CalligraphyBrushAngle = 135 * Math.PI / 180;
        private const double FountainBrushAngle = 45 * Math.PI / 180;


        private readonly DrawingAttributes paeAttributes = new DrawingAttributes()
        {
            Color = Colors.Black,
            Height = 2,
            Width = 2,

        };

        public MainWindow()
        {
            InitializeComponent();

            InitToolButsInList();
            InitBrushTypesInList();

            InitializeSprayTimer();
            CanvasSize.Content = $"{DrawingCanvas.Width} x {DrawingCanvas.Height}";
        }
        public void InitBrushTypesInList()
        {
            _brushTypes.Clear();
            _brushTypes.Add(ZeroBrushType);
            _brushTypes.Add(OneBrushType);
            _brushTypes.Add(TwoBrushType);
            _brushTypes.Add(ThreeBrushType);
            _brushTypes.Add(FourBrushType);
            _brushTypes.Add(FiveBrushType);
            _brushTypes.Add(SixBrushType);
            _brushTypes.Add(SevenBrushType);
            _brushTypes.Add(EightBrushType);

            InitTagsForBrushesTypes();
        }
        public void InitTagsForBrushesTypes()
        {
            for (int i = 0; i < _brushTypes.Count; i++)
            {
                _brushTypes[i].Tag = ((BrushType)i).ToString();
            }
        }

        public void InitToolButsInList()
        {
            _tools.Clear();
            _tools.Add(Pen);
            _tools.Add(Bucket);
            _tools.Add(Text);
            _tools.Add(Erazer);
            _tools.Add(ColorDrop);
            _tools.Add(Glass);
        }
        private void PaintColor_Click(object sender, EventArgs e)
        {
            ClearMainColorBorders();
            Button button = sender as Button;
            Ellipse greyCircle = new Ellipse();
            greyCircle.Width = button.Width + 1;
            greyCircle.Height = button.Height + 1;

            greyCircle.Stroke = Brushes.Blue;
            greyCircle.StrokeThickness = 1;

            // Задаем отступы для greyCircle
            greyCircle.Margin = new Thickness(-10); // Отступы по умолчанию

            button.Content = greyCircle;
        }
        public void ClearMainColorBorders()
        {
            ClearMainColorBorder(FirstColor);
            ClearMainColorBorder(SecondColor);
        }
        public void ClearMainColorBorder(Button button)
        {

            Ellipse greyCircle = new Ellipse();
            greyCircle.Width = button.Width + 1;
            greyCircle.Height = button.Height + 1;
            greyCircle.Fill = Brushes.Transparent;

            greyCircle.Stroke = MainPanel.Background;
            greyCircle.StrokeThickness = 1;

            greyCircle.Margin = new Thickness(-10);

            button.Content = greyCircle;
        }

        private void MyButton_MouseEnter(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;

            Ellipse greyCircle = new Ellipse();
            greyCircle.Width = button.Width + 1;
            greyCircle.Height = button.Height + 1;

            greyCircle.Stroke = Brushes.DarkGray;
            greyCircle.StrokeThickness = 1;

            greyCircle.Margin = new Thickness(-10);

            button.Content = greyCircle;
        }
        private void MyButton_MouseLeave(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;

            Ellipse greyCircle = new Ellipse();
            greyCircle.Width = button.Width + 1;
            greyCircle.Height = button.Height + 1;
            greyCircle.Fill = Brushes.Transparent;

            greyCircle.Stroke = MainPanel.Background;
            greyCircle.StrokeThickness = 1;

            greyCircle.Margin = new Thickness(-10);

            button.Content = greyCircle;
        }
        private void Pallete_Click(object sender, EventArgs e)
        {
            Pallete pallete = new Pallete(_main.PalleteMod);
            pallete.ShowDialog();
        }
        private void MainPanelBoxes_MouseEnter(object sender, MouseEventArgs e)
        {
            SolidColorBrush brush = new SolidColorBrush(Color.FromRgb(240, 240, 240));
            SolidColorBrush borderBrush = new SolidColorBrush(Color.FromRgb(209, 209, 209));

            if (sender is Button but)
            {
                if (!(_chosenTool is null) && but.Name == _chosenTool.Name) return;

                but.Background = brush;
                but.BorderBrush = borderBrush;
                return;
            }
            if (sender is Border bord)
            {
                bord.Background = brush;
                bord.BorderBrush = borderBrush;
                return;
            }
            if (sender is Grid grid)
            {
                grid.Background = brush;
            }
        }
        private void MainPanelTop_MouseEnter(object sender, EventArgs e)
        {
            SolidColorBrush brush = new SolidColorBrush(Color.FromRgb(230, 235, 240));
            if (sender is Button but)
            {
                but.Background = brush;
                return;
            }
            if (sender is Grid grid)
            {
                grid.Background = brush;
                return;
            }
        }
        private void MainPabelBoxes_MouseLeave(object sender, MouseEventArgs e)
        {
            SolidColorBrush transparentBrush = new SolidColorBrush(Color.FromArgb(0, 248, 249, 252));
            if (sender is Button but)
            {
                if (!(_chosenTool is null) && but.Name == _chosenTool.Name) return;
                but.Background = transparentBrush;
                but.BorderBrush = but.BorderBrush != _clickedBorderColor ? transparentBrush : _clickedBorderColor;
                return;
            }
            if (sender is Border bord)
            {
                bord.Background = transparentBrush;
                bord.BorderBrush = bord.BorderBrush != _clickedBorderColor ? transparentBrush : _clickedBorderColor;
                return;
            }
            if (sender is Grid grid)
            {
                grid.Background = transparentBrush;
                return;
            }
        }
        private void Tool_MouseClick(object sender, EventArgs e)
        {
            if (sender is Button but)
            {
                if (!(_chosenTool is null) &&
                    but.Name == _chosenTool.Name) return;
                ClearBGs();
                _chosenTool = but;

                SolidColorBrush border = new SolidColorBrush(Color.FromRgb(0, 103, 192));
                but.BorderBrush = border;

                SolidColorBrush brush = new SolidColorBrush(Color.FromRgb(240, 240, 240));
                but.Background = brush;

                if (but.Name == "Pen")
                {
                    _type = ActionType.Drawing;
                }
                else if (but.Name == "Erazer")
                {
                    _type = ActionType.Erazing;
                }
                else if (but.Name == "Bucket")
                {
                    _type = ActionType.Filling;
                }
            }
        }
        public void ClearBGs()
        {
            ClearBGForTools();
            _chosenTool = null;
            CleaBgsForFigures();
        }
        public void ClearBGForTools()
        {
            SolidColorBrush trancparenBrush = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));

            for (int i = 0; i < _tools.Count; i++)
            {
                _tools[i].Background = trancparenBrush;
                _tools[i].BorderBrush = trancparenBrush;
            }
        }
        public void CleaBgsForFigures()
        {
            SolidColorBrush trancparenBrush = new SolidColorBrush(
                Color.FromArgb(0, 255, 255, 255));
            for (int i = 0; i < FigurePanel.Children.Count; i++)
            {
                if (FigurePanel.Children[i] is Button but)
                {
                    but.Background = trancparenBrush;
                    but.BorderBrush = trancparenBrush;
                }
            }
        }
        public void PaintField_MouseLeave(object sender, MouseEventArgs e)
        {
            CursCords.Content = "";
        }
        private RenderTargetBitmap _renderBitmap;

        private void Field_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _main.ColorToPaint = _main.FirstColor;
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                _main.ColorToPaint = _main.SecondColor;
            }

            previousPoint = e.GetPosition(DrawingCanvas);
            InitDeed();
            if (IfSelection)
            {
                MakeSelection(e);
            }
            else if (IfFilling)
            {
                RenderCanvasToBitmap();
                PerformFloodFill((int)previousPoint.X, (int)previousPoint.Y);
            }
            else if (IfFiguring)
            {
                InitShapesToPaint(e);
            }
            else
            {
                previousPoint = e.GetPosition(DrawingCanvas);
                SetPaintingMarker(e);
            }
        }
        private void MakeSelection(MouseEventArgs e)
        {
            if (_selectionType == SelectionType.Rectangle)
            {
                MakeRecangleSelection(e);
            }
            else if (_selectionType == SelectionType.Custom)
            {

            }
            else if (_selectionType == SelectionType.All)
            {

            }
        }
        private void MakeRecangleSelection(MouseEventArgs e)
        {
            _selectionRect = new Rectangle
            {
                Stroke = Brushes.LightBlue,
                StrokeThickness = 3,
                Fill = Brushes.Transparent
            };
            Canvas.SetLeft(_selectionRect, e.GetPosition(DrawingCanvas).X);
            Canvas.SetTop(_selectionRect, e.GetPosition(DrawingCanvas).Y);
            DrawingCanvas.Children.Add(_selectionRect);
        }
        private void RenderCanvasToBitmap()
        {
            double dpi = 96;
            _renderBitmap = new RenderTargetBitmap((int)DrawingCanvas.ActualWidth,
                (int)DrawingCanvas.ActualHeight, dpi, dpi, PixelFormats.Pbgra32);
            _renderBitmap.Render(DrawingCanvas);
        }
        private void InitDeed()
        {
            if (_type == ActionType.Drawing)
            {
                IfDrawing = true;
                IfErazing = false;
                IfFiguring = false;
                IfFilling = false;
                IfSelection = false;
            }
            else if (_type == ActionType.Figuring)
            {
                IfDrawing = false;
                IfErazing = false;
                IfFiguring = true;
                IfFilling = false;
                IfSelection = false;
            }
            else if (_type == ActionType.Erazing)
            {
                IfDrawing = false;
                IfErazing = true;
                IfFiguring = false;
                IfFilling = false;
                IfSelection = false;
            }
            else if (_type == ActionType.Filling)
            {
                IfDrawing = false;
                IfErazing = false;
                IfFiguring = false;
                IfFilling = true;
                IfSelection = false;
            }
            else if (_type == ActionType.Selection)
            {
                IfSelection = true;
                IfDrawing = false;
                IfErazing = false;
                IfFiguring = false;
                IfFilling = false;
            }
            else
            {
                IfDrawing = true;
                IfErazing = false;
                IfFiguring = false;
                IfFilling = false;
                IfSelection = false;
            }
        }
        private void InitShapesToPaint(MouseEventArgs e)
        {
            switch (_figType)
            {
                case FigureTypes.Line:
                    {
                        _figToPaint = new Polyline();
                        break;
                    }
                case FigureTypes.Curve:
                    break;
                case FigureTypes.Oval:
                    {
                        _figToPaint = new Ellipse();
                        break;
                    }
                case FigureTypes.Rectangle:
                    {
                        _figToPaint = new Rectangle();
                        break;
                    }
                case FigureTypes.RoundedRectangle:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse("M 20 10 L 160 10 A 10 10 0 0 1 170 20 L 170 140 A 10 10 0 0 1 160 150 " +
                        "L 20 150 A 10 10 0 0 1 10 140 L 10, 20 A 10 10 0 0 1 20 10");
                        break;
                    }
                case FigureTypes.Polygon:
                    {
                        if (poligonFigure is null)
                        {
                            poligonFigure = new Polyline();
                        }
                        _figToPaint = new Polyline();

                        break;
                    }
                case FigureTypes.Triangle:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse("M 10 150 L 170 150 L 90 10 Z");
                        break;
                    }
                case FigureTypes.RightTriangle:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse("M 10 150 L 170 150 L 10 10 Z");
                        break;
                    }
                case FigureTypes.Rhombus:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse("M 90 10 L 170 80 L 90 150 L 10 80 Z");
                        break;
                    }
                case FigureTypes.Pentagon:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse("M 100 10 L 170 60 L 140 150 L 60 150 L 30 60 Z");
                        break;
                    }
                case FigureTypes.Hexagon:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse("M 100 10 L 150 40 L 150 100 L 100 130 L 50 100 L 50 40 Z");
                        break;
                    }
                case FigureTypes.RightArrow:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse("M 10 50 L 100 50 L 100 30 L 150 70 L 100 110 L 100 90 L 10 90 Z");
                        break;
                    }
                case FigureTypes.LeftArrow:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse("M 150 50 L 50 50 L 50 30 L 0 70 L 50 110 L 50 90 L 150 90 Z");
                        break;
                    }
                case FigureTypes.UpArrow:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse("M 50 150 L 50 50 L 30 50 L 70 0 L 110 50 L 90 50 L 90 150 Z");
                        break;
                    }
                case FigureTypes.DownArrow:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse("M 50 50 L 50 150 L 30 150 L 70 200 L 110 150 L 90 150 L 90 50 Z");
                        break;
                    }
                case FigureTypes.FourPointedStar:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse("M 95 20 L 110 70 L 180 80 L 110 100 L 95 160 L 75 100 L 20 80 L 80 70 L 95 20");
                        break;
                    }
                case FigureTypes.FivePointedStar:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse("M 100 10  L 130 90 L 200 90 L 145 130 L 170 200 L 100 160 L 30 200  L 55 130 L 0 90  L 70 90 Z");
                        break;
                    }
                case FigureTypes.SixPointedStar:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse("M 100 10 L 130 60 L 180 60 L 150 100 L 180 140 L 130 140 L 100 180 L 70 140 L 20 140 L 50 100 L 20 60 L 70 60 Z");
                        break;
                    }
                case FigureTypes.RoundedRectangularLeader:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse("M 120 10 L 150 10 Q 170, 10, 170 40 L 170 100 Q 170 120, 150 120 L 75 120 L 65 140 L 55 120 Q 25, 120, 25 100 L 25 40 Q 25 10, 40 10 Z");
                        break;
                    }
                case FigureTypes.OvalCallout:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse("M 100,100 A 40, 30 0 1 1 200, 100 A 60, 60 1 0 1 140, 130 L 125 145 L 120 130 Q 100 120 100 100 ");
                        break;
                    }

                case FigureTypes.CalloutCloud:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse("M 80,50 A 20 10 1 1 1 150 45 A 20 10 1 1 1 210 40 " +
                        "A 10 5 1 1 1 270 70 A 10 10 1 1 1 270 120 A 10 10 1 1 1 240 155 " +
                        "A 15 12 1 1 1 180 165 A 18 10 1 1 1 90 170 A 45 35 1 0 1 30 130 " +
                        "A 15 20 1 0 1 25 90 A 20 20 0 0 1 80 50 " +
                        "M 30 180 A 20 10 0 0 1 70 180 A 20 10 0 0 1 30 180 " +
                        "M 20 200 A 20 10 0 0 1 60 200 A 20 10 0 0 1 20 200 " +
                        "M 15 215 A 20 20 0 0 1 45 215 A 20 20 0 0 1 15 215");
                        break;
                    }
                case FigureTypes.Heart:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse("M 50 100 C 50 0 150 0 150 100 C 150 200 50 250 50 350 C 50, 250 -50 200 -50 100 C -50 0 50 0 50 100 Z");
                        break;
                    }
                case FigureTypes.Lightning:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse("M 80,50 L 120 40 L 150 80 L 140 85 L 175 120 L 165 125 L 200 170 L 140 140 L 150 135 L 105 90 L 120 85 Z");
                        break;
                    }
                default:
                    {
                        MessageBox.Show("How did you get here?");
                        break;
                    }
            }
            _figToPaint.Stroke = ConvertColorIntoBrushes();
            _figToPaint.StrokeThickness = 3;
            _figToPaint.Fill = Brushes.Transparent;

            Canvas.SetTop(_figToPaint, e.GetPosition(DrawingCanvas).Y);
            Canvas.SetLeft(_figToPaint, e.GetPosition(DrawingCanvas).X);
            DrawingCanvas.Children.Add(_figToPaint);
        }
        private void GetPathToPaint()
        {
            _figToPaint = new System.Windows.Shapes.Path()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 3,
                RenderTransform = new ScaleTransform(1, 1),
                Stretch = Stretch.Fill

            };

        }
        private void Field_MouseMove(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition(DrawingCanvas);
            string cursPosInPaintField = $"{position.X}, {position.Y}";
            CursCords.Content = cursPosInPaintField;

            if (IfSelection)
            {
                ChangeSelectionSize(e);
            }
            else if (IfDrawing || IfErazing)
            {
                BrushPaint(e);
            }
            else if (IfFiguring && !(_figToPaint is null))
            {
                PaintFigures(e);
            }
        }
        private void ChangeSelectionSize(MouseEventArgs e)
        {
            if(_selectionType == SelectionType.Rectangle)
            {
                FigureRotation(e, _selectionRect);

                return;
            }            
            
        }

        private void PaintFigures(MouseEventArgs e)
        {
            if (_figType == FigureTypes.Line)
            {
                (_figToPaint as Polyline).Points = new PointCollection()
                {
                    new Point(0, 0),
                    new Point(e.GetPosition(DrawingCanvas).X - Canvas.GetLeft(_figToPaint),
                    e.GetPosition(DrawingCanvas).Y - Canvas.GetTop(_figToPaint))
                };
            }
            else if (_figType == FigureTypes.Polygon)
            {
                Point startPoint = poligonFigure.Points.Count == 0 ? new Point(0, 0) : new Point(-1, -1);

                if (startPoint.X == -1 && startPoint.Y == -1)
                {
                    startPoint = poligonFigure.Points[poligonFigure.Points.Count - 1];
                }
                (_figToPaint as Polyline).Points = new PointCollection()
                {
                    new Point(startPoint.X, startPoint.Y),

                    new Point(e.GetPosition(DrawingCanvas).X - Canvas.GetLeft(_figToPaint),
                    e.GetPosition(DrawingCanvas).Y - Canvas.GetTop(_figToPaint))
                };
            }
            else if (_figType == FigureTypes.Curve)
            {

            }
            else
            {
                FigureRotation(e, _figToPaint);
            }
        }
        private void FigureRotation(MouseEventArgs e, Shape shape)
        {
            double x = Math.Min(previousPoint.X, e.GetPosition(DrawingCanvas).X);
            double y = Math.Min(previousPoint.Y, e.GetPosition(DrawingCanvas).Y);

            double width = Math.Abs(e.GetPosition(DrawingCanvas).X - previousPoint.X);
            double height = Math.Abs(e.GetPosition(DrawingCanvas).Y - previousPoint.Y);

            shape.Width = width;
            shape.Height = height;

            Canvas.SetLeft(shape, x);
            Canvas.SetTop(shape, y);
        }
        private void BrushPaint(MouseEventArgs e)
        {
            currentPoint = e.GetPosition(DrawingCanvas);
            if (_main.TempBrushType == BrushType.UsualBrush || IfErazing)
            {
                GetLineToPaint(currentPoint);
            }
            else if (_main.TempBrushType == BrushType.CalligraphyBrush)
            {
                CalligraphyBrushPaint(CalligraphyBrushAngle);
            }
            else if (_main.TempBrushType == BrushType.FountainPen)
            {
                CalligraphyBrushPaint(FountainBrushAngle);
            }
            else if (_main.TempBrushType == BrushType.Spray)
            {
                sprayTimer.Start();
                SprayPaint(currentPoint);
            }
            else if (_main.TempBrushType == BrushType.OilPaintBrush)
            {
                OilBrushPaint();
            }
            else if (_main.TempBrushType == BrushType.ColorPencil)
            {
                ColorPencilBrushPaint(); // not working
            }
            else if (_main.TempBrushType == BrushType.Marker)
            {
                MarkerBrushPaint(e);
            }
            else if (_main.TempBrushType == BrushType.TexturePencil)
            {
                TextureBrushPaint(e);
            }
            else if (_main.TempBrushType == BrushType.WatercolorBrush)
            {
                //not working 
            }
            previousPoint = currentPoint;
        }
        private void TextureBrushPaint(MouseEventArgs e)
        {
            var point = e.GetPosition(DrawingCanvas);
            Random random = new Random();
            int pointsCount = 10;

            for (int i = 0; i < pointsCount; i++)
            {
                double angle = random.NextDouble() * Math.PI * 2;
                double radius = random.NextDouble() * (brushThickness / 2);
                double offsetX = Math.Cos(angle) * radius;
                double offsetY = Math.Sin(angle) * radius;

                Ellipse ellipse = new Ellipse
                {
                    Opacity = 0.4,
                    Width = random.NextDouble() * (brushThickness),
                    Height = random.NextDouble() * (brushThickness),
                    Fill = _main.FirstColor
                };
                double x = point.X + offsetX;
                double y = point.Y + offsetY;
                Canvas.SetLeft(ellipse, x);
                Canvas.SetTop(ellipse, y);

                DrawingCanvas.Children.Add(ellipse);
            }
            if (polylines.Last().Points.Last() ==
                            e.GetPosition(DrawingCanvas))
                return;

            if (polylines.Last().Points.Count > 0 &&
                polylines.Last().Points.Contains(point))
            {
                var polygon = new Polygon();
                polygon.Points = polylines.Last().Points;
                polygon.Fill = Brushes.Transparent;

                polygons.Add(polygon);
                DrawingCanvas.Children.Add(polygon);

                SetPaintingMarker(e);
            }
            polylines.Last().Points.Add(point);
        }
        private void MarkerBrushPaint(MouseEventArgs e)
        {
            if (polylines.Last().Points.Last() ==
                e.GetPosition(DrawingCanvas))
                return;

            var point = e.GetPosition(DrawingCanvas);

            if (polylines.Last().Points.Count > 0 &&
                polylines.Last().Points.Contains(point))
            {
                var polygon = new Polygon();
                polygon.Points = polylines.Last().Points;
                polygon.Fill = Brushes.Transparent;

                polygons.Add(polygon);
                DrawingCanvas.Children.Add(polygon);

                SetPaintingMarker(e);
            }
            polylines.Last().Points.Add(point);
        }
        public void SetPaintingMarker(MouseEventArgs e)
        {
            var polyline = new Polyline();
            polyline.StrokeThickness = 1;
            polyline.Stroke = ConvertColorIntoBrushes();
            polylines.Add(polyline);
            polyline.Points.Add(e.GetPosition(DrawingCanvas));
            DrawingCanvas.Children.Add(polyline);

            polyline.StrokeThickness = brushThickness;
            polyline.Stroke = ConvertColorIntoBrushes();

            polyline.Opacity = 0.5;
        }
        private void ColorPencilBrushPaint()
        {
            Random random = new Random();

            int pointsCount = (int)(brushThickness * 2);

            List<Ellipse> ellipses = new List<Ellipse>();

            for (double t = 0; t <= 1; t += 1.0 / pointsCount)
            {
                double x = previousPoint.X + (currentPoint.X - previousPoint.X) * t;
                double y = previousPoint.Y + (currentPoint.Y - previousPoint.Y) * t;

                double angle = random.NextDouble() * Math.PI * 2;
                double radius = random.NextDouble() * (brushThickness / 2);

                double offsetX = Math.Cos(angle) * radius;
                double offsetY = Math.Sin(angle) * radius;

                Ellipse ellipse = new Ellipse
                {
                    Width = 3,
                    Height = 3,
                    Fill = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255))
                };

                Canvas.SetLeft(ellipse, x + offsetX);
                Canvas.SetTop(ellipse, y + offsetY);
                ellipses.Add(ellipse);
            }
            Line line = new Line
            {
                X1 = previousPoint.X,
                Y1 = currentPoint.Y,
                X2 = currentPoint.X,
                Y2 = currentPoint.Y,
                Stroke = _main.FirstColor,
                StrokeThickness = brushThickness,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeLineJoin = PenLineJoin.Round
            };
            foreach (Ellipse ellipse in ellipses)
            {
                DrawingCanvas.Children.Add(ellipse);
            }
            DrawingCanvas.Children.Add(line);

            previousPoint = currentPoint;
        }
        private void OilBrushPaint()
        {
            Random random = new Random();
            SolidColorBrush brush = _main.FirstColor;

            int pointsCount = (int)(brushThickness * 2);
            for (int i = 0; i < pointsCount; i++)
            {
                double offsetX = random.NextDouble() * brushThickness - brushThickness / 2;
                double offsetY = random.NextDouble() * brushThickness - brushThickness / 2;

                Ellipse ellipse = new Ellipse
                {
                    Opacity = 0.5,
                    Width = random.NextDouble() * (brushThickness / 2),
                    Height = random.NextDouble() * (brushThickness / 2),
                    Fill = _main.FirstColor
                };
                Canvas.SetLeft(ellipse, currentPoint.X + offsetX);
                Canvas.SetTop(ellipse, currentPoint.Y + offsetY);

                DrawingCanvas.Children.Add(ellipse);
            }
        }
        private void CalligraphyBrushPaint(double angle)
        {
            Vector offset = new Vector(Math.Cos(angle) * brushThickness / 2,
                                Math.Sin(angle) * brushThickness / 2);

            Point[] points = new Point[4];
            points[0] = new Point(previousPoint.X + offset.X, previousPoint.Y + offset.Y);
            points[1] = new Point(previousPoint.X - offset.X, previousPoint.Y - offset.Y);
            points[2] = new Point(currentPoint.X - offset.X, currentPoint.Y - offset.Y);
            points[3] = new Point(currentPoint.X + offset.X, currentPoint.Y + offset.Y);

            Polygon polygon = new Polygon
            {
                Points = new PointCollection(points),
                Fill = ConvertColorIntoBrushes(),
                Stroke = ConvertColorIntoBrushes(),
                StrokeThickness = 0.5

            };

            DrawingCanvas.Children.Add(polygon);
        }
        private void InitializeSprayTimer()
        {
            sprayTimer = new DispatcherTimer();
            sprayTimer.Interval = TimeSpan.FromMilliseconds(50);
            sprayTimer.Tick += SprayTimer_Tick;
        }
        private void SprayTimer_Tick(object sender, EventArgs e)
        {
            SprayPaint(currentPoint);
        }
        private void SprayPaint(Point point)
        {
            for (int i = 0; i < sprayDensity; i++)
            {
                double angle = random.NextDouble() * 2 * Math.PI;
                double radius = Math.Sqrt(random.NextDouble()) * brushThickness / 2;

                double offsetX = radius * Math.Cos(angle);
                double offsetY = radius * Math.Sin(angle);

                Ellipse ellipse = new Ellipse
                {
                    Width = 1,
                    Height = 1,
                    Fill = ConvertColorIntoBrushes()
                };
                Canvas.SetLeft(ellipse, point.X + offsetX);
                Canvas.SetTop(ellipse, point.Y + offsetY);

                DrawingCanvas.Children.Add(ellipse);
            }
        }
        public void GetLineToPaint(Point currentPoint)
        {
            Line line = new Line
            {
                X1 = previousPoint.X,
                Y1 = previousPoint.Y,
                X2 = currentPoint.X,
                Y2 = currentPoint.Y,

                Stroke = IfErazing ? Brushes.White : _main.ColorToPaint,
                StrokeThickness = brushThickness,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeLineJoin = PenLineJoin.Round,
            };
            DrawingCanvas.Children.Add(line);
        }
        public Brush ConvertColorIntoBrushes()
        {
            return _main.ColorToPaint;
        }
        private void Paint_MouseUp(object sender, MouseEventArgs e)
        {
            IfDrawing = false;
            IfErazing = false;
            IfFiguring = false;
            IfFilling = false;
            IfSelection = false;

            CheckForFigurePainting();

            sprayTimer.Stop();
            SaveCanvasState();
        }
        private void CheckForFigurePainting()
        {
            if (_figType != FigureTypes.Polygon)
            {
                _figToPaint = null;
            }
            else if (_figType == FigureTypes.Polygon)
            {
                for (int i = 0; i < ((Polyline)_figToPaint).Points.Count; i++)
                {
                    Point point = new Point(((Polyline)_figToPaint).Points[i].X,
                        ((Polyline)_figToPaint).Points[i].Y);
                    poligonFigure.Points.Add(point);
                }
                for (int i = 0; i < poligonFigure.Points.Count; i++)
                {
                    Point point = new Point(poligonFigure.Points[i].X,
                        poligonFigure.Points[i].Y);
                }
            }
        }

        private void PerformFloodFill(int x, int y)
        {
            if (x < 0 || x >= _renderBitmap.PixelWidth || y < 0 || y >= _renderBitmap.PixelHeight)
                return;

            int stride = _renderBitmap.PixelWidth * (_renderBitmap.Format.BitsPerPixel / 8);
            byte[] pixels = new byte[stride * _renderBitmap.PixelHeight];
            _renderBitmap.CopyPixels(pixels, stride, 0);

            Color targetColor = GetPixelColor(pixels, stride, x, y);
            if (targetColor == _main.ColorToPaint.Color) return;

            Queue<Point> points = new Queue<Point>();
            points.Enqueue(new Point(x, y));

            while (points.Count > 0)
            {
                Point pt = points.Dequeue();
                int px = (int)pt.X;
                int py = (int)pt.Y;

                if (px < 0 || px >= _renderBitmap.PixelWidth || py < 0 || py >= _renderBitmap.PixelHeight)
                    continue;

                if (GetPixelColor(pixels, stride, px, py) != targetColor)
                    continue;

                SetPixelColor(pixels, stride, px, py, _main.ColorToPaint.Color);

                points.Enqueue(new Point(px - 1, py));
                points.Enqueue(new Point(px + 1, py));
                points.Enqueue(new Point(px, py - 1));
                points.Enqueue(new Point(px, py + 1));
            }

            UpdateBitmap(pixels, stride);
        }
        private Color GetPixelColor(byte[] pixels, int stride, int x, int y)
        {
            int index = (y * stride) + (x * (_renderBitmap.Format.BitsPerPixel / 8));
            byte blue = pixels[index];
            byte green = pixels[index + 1];
            byte red = pixels[index + 2];
            byte alpha = pixels[index + 3];
            return Color.FromArgb(alpha, red, green, blue);
        }

        private void SetPixelColor(byte[] pixels, int stride, int x, int y, Color color)
        {
            int index = (y * stride) + (x * (_renderBitmap.Format.BitsPerPixel / 8));
            pixels[index] = color.B;
            pixels[index + 1] = color.G;
            pixels[index + 2] = color.R;
            pixels[index + 3] = color.A;
        }
        private void UpdateBitmap(byte[] pixels, int stride)
        {
            // Создаем новое изображение на основе измененных пикселей
            BitmapSource bitmapSource = BitmapSource.Create(
                _renderBitmap.PixelWidth,
                _renderBitmap.PixelHeight,
                _renderBitmap.DpiX,
                _renderBitmap.DpiY,
                _renderBitmap.Format,
                null,
                pixels,
                stride
            );

            DrawingCanvas.Children.Clear();
            DrawingCanvas.Background = new ImageBrush(bitmapSource);
        }
        private void ValueCanvas_PreViewMouseUp(object sender, MouseEventArgs e)
        {
            valueDragElem = null;
            ValueCanvas.ReleaseMouseCapture();
        }
        private void ValueCanvas_PreViewMouseMove(object sender, MouseEventArgs e)
        {
            if (valueDragElem == null) return;
            var position = e.GetPosition(sender as IInputElement);

            if (position.Y < draggableButton.Height / 2)
            {
                position.Y = draggableButton.Height / 2;
            }
            if (position.Y > ValueCanvas.Height - draggableButton.Height / 2)
            {
                position.Y = ValueCanvas.Height - draggableButton.Height / 2;
            }
            Canvas.SetTop(valueDragElem, position.Y - valueOffset.Y);

            ChangeProgressBarValue(position.Y - valueOffset.Y);
        }
        public void ChangeProgressBarValue(double pos)
        {
            double onePointHeight = (ValueProgressBar.Height - draggableButton.Height) /
                ValueProgressBar.Maximum;
            double temp = pos / onePointHeight;

            double height = 250 - onePointHeight * temp;
            paintInBlueCan.Height = height;

            AAA.Content = Math.Abs(((int)temp) - 100).ToString();
            brushThickness = Math.Abs(((int)temp) - 100);
        }

        private double prevYPos;
        private bool IfMadeThickBigger = false;

        private void ValueDrag_PreViewMouseDown(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            Point buttonPosition = button.TransformToAncestor(ValueCanvas)
                                         .Transform(new Point(0, 0));

            double centerX = buttonPosition.X + (button.ActualWidth / 2);
            double centerY = buttonPosition.Y + (button.ActualHeight / 2);

            IfThinBiggerCheck(centerY);

            if (IfMadeThickBigger && prevYPos != 0)
            {
                paintInBlueCan.Margin = new Thickness(0, paintInBlueCan.Margin.Top, 0,
                paintInBlueCan.Margin.Bottom + 1);
            }
            else if (prevYPos != 0 && paintInBlueCan.Margin.Bottom > draggableButton.Height / 2)
            {
                paintInBlueCan.Margin = new Thickness(0, paintInBlueCan.Margin.Top, 0,
               paintInBlueCan.Margin.Bottom - 1);
            }
            valueDragElem = sender as UIElement;
            valueOffset = new Point((int)centerX, (int)centerY);

            valueOffset.Y -= Canvas.GetTop(valueDragElem);
            ValueCanvas.CaptureMouse();
        }
        public void IfThinBiggerCheck(double butYCord)
        {

            if (prevYPos == 0)
            {
                prevYPos = butYCord;
            }
            else if (prevYPos - butYCord > 0)
            {
                IfMadeThickBigger = true;
            }
            else if (prevYPos - butYCord < 0)
            {
                IfMadeThickBigger = false;
            }
        }
        private void BrushType_Click(object sender, EventArgs e)
        {
            if (sender is MenuItem item)
            {
                BrushType? type = GetBrushTypeByType(item.Tag.ToString());

                if (!(type is null))
                {
                    _main.TempBrushType = (BrushType)type;
                    string picPath = GetSourseForNewBrushType(_main.TempBrushType);
                    string asd = GetPathToNewBrushTypePic();

                    BrushTypePic.Source = BitmapFrame.Create(new Uri(asd));
                }
            }
            _type = ActionType.Drawing;
        }
        public string GetPathToNewBrushTypePic()
        {
            DirectoryInfo baseDirectoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            string imageDirectory = baseDirectoryInfo.Parent.Parent.FullName;
            string imagePath = System.IO.Path.Combine(imageDirectory, "Images");
            string brushDir = System.IO.Path.Combine(imagePath, "BrushType");
            return System.IO.Path.Combine(brushDir, $"{_main.TempBrushType.ToString()}.png");
        }
        public string GetSourseForNewBrushType(BrushType type)
        {
            return type == BrushType.UsualBrush ? "Images/BrushType/UsualBrush.png" :
                   type == BrushType.CalligraphyBrush ? "Images/BrushType/CalligraphyBrush.png" :
                   type == BrushType.FountainPen ? "Images/BrushType/FountainPen.png" :
                   type == BrushType.Spray ? "Images/BrushType/Spray.png" :
                   type == BrushType.OilPaintBrush ? "Images/BrushType/OilPaintBrush.png" :
                   type == BrushType.ColorPencil ? "Images/BrushType/ColorPencil.png" :
                   type == BrushType.Marker ? "Images/BrushType/Marker.png" :
                   type == BrushType.TexturePencil ? "Images/BrushType/TexturePencil.png" :
                    "Images/BrushType/WatercolorBrush.png";
        }
        public BrushType? GetBrushTypeByType(string brushName)
        {
            for (int i = 0; i <= (int)BrushType.WatercolorBrush; i++)
            {
                if (brushName == ((BrushType)i).ToString())
                {
                    return ((BrushType)i);
                }
            }
            return null;
        }
        private void PreviousCanvas_Click(object sender, EventArgs e)
        {
            if (currentIndex > 0)
            {
                DrawingCanvas.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                DrawingCanvas.Children.Clear();

                currentIndex--;

                BitmapSource bitmap = canvasHistory[currentIndex];

                Image image = new Image();
                image.Source = bitmap;

                Canvas.SetLeft(image, 0);
                Canvas.SetTop(image, 0);
                DrawingCanvas.Children.Add(image);
            }
            else if (currentIndex == 0)
            {
                DrawingCanvas.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                currentIndex--;
                DrawingCanvas.Children.Clear();
            }
        }
        private void NextCanvas_Click(object sender, EventArgs e)
        {
            if (currentIndex < canvasHistory.Count - 1)
            {
                DrawingCanvas.Children.Clear();

                currentIndex++;
                BitmapSource bitmap = canvasHistory[currentIndex];
                Image image = new Image();
                image.Source = bitmap;
                DrawingCanvas.Children.Add(image);
            }

        }
        private List<BitmapSource> canvasHistory = new List<BitmapSource>();
        private int currentIndex = -1;

        public void SaveCanvasState()
        {
            Size size = new Size(DrawingCanvas.ActualWidth, DrawingCanvas.ActualHeight);

            DrawingCanvas.Measure(size);
            DrawingCanvas.Arrange(new Rect(size));

            RenderTargetBitmap rtb = new RenderTargetBitmap(
                (int)size.Width,
                (int)size.Height,
                96d, 96d, PixelFormats.Pbgra32);
            rtb.Render(DrawingCanvas);
            BitmapSource bitmap = BitmapFrame.Create(rtb);

            if (currentIndex < canvasHistory.Count - 1)
            {
                canvasHistory.RemoveRange(currentIndex + 1, canvasHistory.Count - currentIndex - 1);
            }
            canvasHistory.Add(bitmap);

            currentIndex++;
        }
        private void Figure_Click(object sender, EventArgs e)
        {
            ClearBGs();
            if (sender is Button but)
            {
                but.BorderBrush = _clickedBorderColor;

                FigureTypes? figType = ConvertSrtingIntoEnum(but.Name);
                if (figType is null)
                {
                    MessageBox.Show("Cant find such fig type", "Mistake", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                _figType = figType;
            }
            _type = ActionType.Figuring;
        }
        public FigureTypes? ConvertSrtingIntoEnum(string figType)
        {
            for (int i = 0; i <= (int)FigureTypes.Lightning; i++)
            {
                if (figType == ((FigureTypes)i).ToString())
                {
                    return ((FigureTypes)i);
                }
            }
            return null;
        }
        private void SaveField_Click(object sender, EventArgs e)
        {
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)DrawingCanvas.ActualWidth,
                (int)DrawingCanvas.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(DrawingCanvas);

            PngBitmapEncoder pngImage = new PngBitmapEncoder();
            pngImage.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image|*.png";
            saveFileDialog.FileName = "IHateThisTask.png";
            if (saveFileDialog.ShowDialog() == true)
            {
                using (FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    pngImage.Save(fileStream);
                }
            }
        }
        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                BitmapImage bitmap = new BitmapImage(new Uri(filePath));
                Image image = new Image
                {
                    Source = bitmap,
                    Width = bitmap.PixelWidth,
                    Height = bitmap.PixelHeight
                };
                if (image.Width > DrawingCanvas.Width)
                {
                    image.Width = DrawingCanvas.Width;
                }
                if (image.Height > DrawingCanvas.Height)
                {
                    image.Height = DrawingCanvas.Height;
                }
                DrawingCanvas.Children.Clear();
                DrawingCanvas.Children.Add(image);

                Canvas.SetLeft(image, 0);
                Canvas.SetTop(image, 0);
            }
        }
        private void CreateNew_Click(object sender, EventArgs e)
        {
            DrawingCanvas.Children.Clear();
        }
        private void Color_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender is Button but)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    _main.FirstColor = new SolidColorBrush(ColorConvertor.HexToRGB(but.Background.ToString()));
                }
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    _main.SecondColor = new SolidColorBrush(ColorConvertor.HexToRGB(but.Background.ToString()));
                }
            }
        }

        private void Color_PreviewMouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            if (sender is Button but &&
                e.LeftButton == MouseButtonState.Pressed)
            {
                _main.FirstColor = new SolidColorBrush(ColorConvertor.HexToRGB(but.Background.ToString()));
                FirstColor.Background = but.Background;
            }
        }
        private void Color_PreviewMouseRightButtonDown(object sender, MouseEventArgs e)
        {
            if (sender is Button but &&
                e.RightButton == MouseButtonState.Pressed)
            {
                _main.SecondColor = new SolidColorBrush(ColorConvertor.HexToRGB(but.Background.ToString()));
                SecondColor.Background = but.Background;
            }
        }
        private void CloseApp_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void SquareSelection_Click(object sender, EventArgs e)
        {
            _type = ActionType.Selection;
            _selectionType = SelectionType.Rectangle;
        }

    }
}
