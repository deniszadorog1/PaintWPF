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
using PaintWPF.CustomControls;

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
        private bool IfTexting = false;

        private FigureTypes? _figType = null;
        private Shape _figToPaint;
        private Polyline poligonFigure = null;

        private Point previousPoint;
        private int brushThickness = 2;

        private bool IfShowBrushSize = false;

        Selection _selection = null;

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

        private string _oilBrushPath;
        private string _coloredBrushPath;
        private string _texturePencilBrushPath;
        private string _watercolorBrushPath;
        private Polyline _tempBrushLine;

        private TextEditor _text = null;
        private TextBox _textBox = null;

        private double _startThisHeight;
        private Selection _changedSizeText;

        public MainWindow()
        {
            InitializeComponent();

            InitStartHeight();
            InitBrushFilePathes();

            InitToolButsInList();
            InitBrushTypesInList();

            InitializeSprayTimer();
            CanvasSize.Content = $"{DrawingCanvas.Width} x {DrawingCanvas.Height}";

            InitForTextControl();
        }

        public void InitStartHeight()
        {
            double height = Height;
            _startThisHeight = height;
        }
        public void InitForTextControl()
        {
            _text = new TextEditor();

            UpdateTextLocation();

            Grid.SetRow(_text, 0);
            Grid.SetColumn(_text, 1);

            CenterWindowPanel.Children.Add(_text);
        }

        public void UpdateTextLocation()
        {
            double locX = _text.TextObject.Width;
            double locY = 5;

            double heightDiffer = Height - _startThisHeight;
            double resMargin = heightDiffer / 2;

            _text.Margin = new Thickness(0, 0, 0, resMargin);

            Canvas.SetLeft(_text, locX);
            Canvas.SetTop(_text, locY);
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
        public void InitBrushFilePathes()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            string startdir = dirInfo.Parent.Parent.FullName;
            string imgPath = System.IO.Path.Combine(startdir, "Images");
            string brushPath = System.IO.Path.Combine(imgPath, "Brushes");

            _oilBrushPath = System.IO.Path.Combine(brushPath, "OilBrush.png");
            _coloredBrushPath = System.IO.Path.Combine(brushPath, "ColoredBrush.png");
            _texturePencilBrushPath = System.IO.Path.Combine(brushPath, "TexturePencilBrush.png");
            _watercolorBrushPath = System.IO.Path.Combine(brushPath, "WatercolorBrush.png");
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
                else if (but.Name == "Text")
                {
                    _type = ActionType.Text;
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
            if (!(_selection is null)) return;
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

            if (IfTexting)
            {

            }
            else if (IfSelection)
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
                SetMarkers(e);
            }
        }

        public void SetMarkers(MouseEventArgs e)
        {
            if (_main.TempBrushType == BrushType.Marker)
            {
                SetPaintingMarker(e);
            }
            else if (_main.TempBrushType == BrushType.OilPaintBrush)
            {
                SetImageBrush(_oilBrushPath);
                InitBrushPolyline(e);
            }
            else if (_main.TempBrushType == BrushType.ColorPencil)
            {
                SetImageBrush(_coloredBrushPath);
                InitBrushPolyline(e);
            }
            else if (_main.TempBrushType == BrushType.WatercolorBrush)
            {
                SetImageBrush(_watercolorBrushPath);
                InitBrushPolyline(e);
            }
            else if (_main.TempBrushType == BrushType.TexturePencil)
            {
                SetImageBrush(_texturePencilBrushPath);
                InitBrushPolyline(e);
            }
        }
        private void InitBrushPolyline(MouseEventArgs e)
        {
            _tempBrushLine = new Polyline
            {
                Stroke = _tempBrush,
                StrokeThickness = brushThickness
            };
            _tempBrushLine.Points.Add(previousPoint);
            DrawingCanvas.Children.Add(_tempBrushLine);
        }
        private ImageBrush _tempBrush;
        public void SetImageBrush(string brushPngPath)
        {
            BitmapImage bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.UriSource = new Uri(brushPngPath, UriKind.RelativeOrAbsolute);
            bitmap.EndInit();

            WriteableBitmap writeableBitmap = new WriteableBitmap(bitmap);

            // Получаем ширину и высоту изображения
            int width = writeableBitmap.PixelWidth;
            int height = writeableBitmap.PixelHeight;
            int stride = width * (writeableBitmap.Format.BitsPerPixel / 8);
            byte[] pixelData = new byte[height * stride];

            // Чтение пикселей из WriteableBitmap
            writeableBitmap.CopyPixels(pixelData, stride, 0);

            // Изменяем цвет пикселей
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * stride + x * 4; // 4 байта на пиксель (BGRA)

                    // Извлекаем компоненты цвета из пикселя
                    byte b = pixelData[index];
                    byte g = pixelData[index + 1];
                    byte r = pixelData[index + 2];
                    byte a = pixelData[index + 3];

                    // Смешивание оригинального цвета с новым цветом
                    double alpha = a / 255.0;
                    byte newR = (byte)(r * alpha + _main.ColorToPaint.Color.R * (1 - alpha));
                    byte newG = (byte)(g * alpha + _main.ColorToPaint.Color.G * (1 - alpha));
                    byte newB = (byte)(b * alpha + _main.ColorToPaint.Color.B * (1 - alpha));

                    // Записываем новый цвет в массив пикселей
                    pixelData[index] = newB;
                    pixelData[index + 1] = newG;
                    pixelData[index + 2] = newR;
                    pixelData[index + 3] = a; // сохраняем оригинальную альфу
                }
            }

            // Записываем измененные пиксели обратно в WriteableBitmap
            writeableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixelData, stride, 0);

            _tempBrush = new ImageBrush();
            _tempBrush.ImageSource = writeableBitmap;

            _tempBrush.TileMode = TileMode.Tile;
            _tempBrush.Stretch = Stretch.None;

            _tempBrush.Viewport = new Rect(0, 0, 100, 100);
            _tempBrush.ViewportUnits = BrushMappingMode.Absolute;
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

            _firstSelectionStart = new Point(e.GetPosition(DrawingCanvas).X, e.GetPosition(DrawingCanvas).Y);

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
            MakeAllActionsNegative();
            if (_type == ActionType.Drawing)
            {
                IfDrawing = true;
            }
            else if (_type == ActionType.Figuring)
            {
                IfFiguring = true;
            }
            else if (_type == ActionType.Erazing)
            {
                IfErazing = true;
            }
            else if (_type == ActionType.Filling)
            {
                IfFilling = true;
            }
            else if (_type == ActionType.Selection)
            {
                IfSelection = true;
            }
            else if (_type == ActionType.Text)
            {
                IfTexting = true;
            }
            else
            {
                IfDrawing = true;
            }
        }
        private void MakeAllActionsNegative()
        {
            IfSelection = false;
            IfDrawing = false;
            IfErazing = false;
            IfFiguring = false;
            IfFilling = false;
            IfTexting = false;
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
                        if (!(_figToPaint is null)) return;
                        if (poligonFigure is null)
                        {
                            poligonFigure = new Polyline();
                        }
                        _figToPaint = new Polyline();
                        _amountOfPointInPolygon = 0;
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
            if (_selectionType == SelectionType.Rectangle)
            {
                FigureRotation(e, _selectionRect);
                _firstSelectionEnd = new Point(e.GetPosition(DrawingCanvas).X, e.GetPosition(DrawingCanvas).Y);
                return;
            }

        }
        private int _amountOfPointInPolygon = 0;
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
                if (_amountOfPointInPolygon != ((Polyline)_figToPaint).Points.Count)
                {
                    ((Polyline)_figToPaint).Points.RemoveAt(((Polyline)_figToPaint).Points.Count - 1);
                    ((Polyline)_figToPaint).Points.RemoveAt(((Polyline)_figToPaint).Points.Count - 1);
                }

                if (_amountOfPointInPolygon == 0)
                {
                    ((Polyline)_figToPaint).Points = new PointCollection();

                    ((Polyline)_figToPaint).Points.Add(new Point(0, 0));
                    ((Polyline)_figToPaint).Points.Add(new Point(e.GetPosition(DrawingCanvas).X - Canvas.GetLeft(_figToPaint),
                        e.GetPosition(DrawingCanvas).Y - Canvas.GetTop(_figToPaint)));

                }
                else
                {
                    ((Polyline)_figToPaint).Points.Add(((Polyline)_figToPaint).Points[((Polyline)_figToPaint).Points.Count - 1]);
                    ((Polyline)_figToPaint).Points.Add(new Point(e.GetPosition(DrawingCanvas).X - Canvas.GetLeft(_figToPaint),
                         e.GetPosition(DrawingCanvas).Y - Canvas.GetTop(_figToPaint)));
                    for (int i = 0; i < _amountOfPointInPolygon; i++)
                    {

                    }
                }
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
                PaintByPngBrush(e);
                //OilBrushPaint();
            }
            else if (_main.TempBrushType == BrushType.ColorPencil)
            {
                PaintByPngBrush(e);

                //ColorPencilBrushPaint(); // not working
            }
            else if (_main.TempBrushType == BrushType.Marker)
            {
                MarkerBrushPaint(e);
            }
            else if (_main.TempBrushType == BrushType.TexturePencil)
            {
                PaintByPngBrush(e);
                //TextureBrushPaint(e);
            }
            else if (_main.TempBrushType == BrushType.WatercolorBrush)
            {
                PaintByPngBrush(e);
            }
            previousPoint = currentPoint;
        }
        private void PaintByPngBrush(MouseEventArgs e)
        {
            Point currentPoint = e.GetPosition(DrawingCanvas);

            _tempBrushLine.Points.Add(currentPoint);

            previousPoint = currentPoint;
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
            //check for selection
            if (IfSelection) MakeSelection();

            IfDrawing = false;
            IfErazing = false;
            IfFiguring = false;
            IfFilling = false;
            IfSelection = false;

            CheckForFigurePainting();

            sprayTimer.Stop();
            SaveCanvasState();
        }
        private void MakeSelection()
        {
            if (_selectionType == SelectionType.Rectangle)
            {
                //int rect attrbs(loc, Size) for _selFigure
                //_selectionRect

                _selection = new Selection()
                {
                    Height = _selectionRect.Height + 10,
                    Width = _selectionRect.Width + 10
                };
                _selection.SelectionBorder.Height = _selectionRect.Height + 10;
                _selection.SelectionBorder.Width = _selectionRect.Width + 10;

                double xLoc = Canvas.GetLeft(_selectionRect);
                double yLoc = Canvas.GetTop(_selectionRect);

                Canvas.SetLeft(_selection, xLoc - 5);
                Canvas.SetTop(_selection, yLoc - 5);

                DrawingCanvas.Children.Remove(_selectionRect);
                DrawingCanvas.Children.Add(_selection);


                InitSelectedBgInCanvas();


                //InitEventsForSelection();
            }
        }

        private void CheckForFigurePainting()
        {
            if (_figType != FigureTypes.Polygon)
            {
                _figToPaint = null;
            }
            else if (_figType == FigureTypes.Polygon)
            {
                _amountOfPointInPolygon = ((Polyline)_figToPaint).Points.Count;
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

        private Point _firstSelectionStart;
        private Point _firstSelectionEnd;


        private void InitSelectedBgInCanvas()
        {
            RenderTargetBitmap copy = GetRenderedCopy();

            InsertBitmapToCanvas(copy);

            ConvertRegionToWhite();
        }
        private void ConvertRegionToWhite()
        {
            Rect selectedBounds = new Rect(_firstSelectionStart, _firstSelectionEnd); // Функция для расчета границ выделенной области

            // Очистка дочерних элементов, находящихся внутри или пересекающих выделенную область
            List<UIElement> elementsToRemove = new List<UIElement>();
            foreach (UIElement child in DrawingCanvas.Children)
            {
                Rect childBounds = VisualTreeHelper.GetDescendantBounds(child);
                Point childTopLeft = child.TranslatePoint(new Point(), DrawingCanvas);

                if (selectedBounds.IntersectsWith(childBounds))
                {
                    // Проверяем, находится ли вся область элемента внутри выделенной области
                    if (selectedBounds.Contains(childTopLeft) && selectedBounds.Contains(new Point(childTopLeft.X + childBounds.Width, childTopLeft.Y + childBounds.Height)))
                    {
                        // Если элемент полностью внутри выделенной области, удаляем его
                        elementsToRemove.Add(child);
                    }
                    else
                    {
                        // Иначе определяем части элемента, которые находятся внутри выделенной области
                        // и модифицируем элемент соответствующим образом
                        Rect intersection = Rect.Intersect(selectedBounds, childBounds);

                        if (child is Shape shape)
                        {
                            // Пример: изменение цвета фона части элемента
                            shape.Fill = Brushes.White;
                            shape.Clip = new RectangleGeometry(intersection);
                        }
                        else if (child is Control control)
                        {
                            control.Background = Brushes.White;
                            control.Clip = new RectangleGeometry(intersection);
                        }
                    }
                }
            }

            // Удаление элементов, которые полностью находятся внутри выделенной области
            foreach (UIElement element in elementsToRemove)
            {
                DrawingCanvas.Children.Remove(element);
            }
        }

        private void InsertBitmapToCanvas(RenderTargetBitmap bitmap)
        {
            var image = new Image
            {
                Source = bitmap,
                Width = _selection.Width,
                Height = _selection.Height
            };
            SwipeWhiteColorWithTranspaentInImage(image);
            Canvas.SetLeft(image, 0);
            Canvas.SetTop(image, 0);
            SetSelectionCanBgASImage(image);
        }
        private Image SwipeWhiteColorWithTranspaentInImage(Image image)
        {
            BitmapSource bitmapSource = image.Source as BitmapSource;
            WriteableBitmap writeableBitmap = new WriteableBitmap(bitmapSource);

            int width = writeableBitmap.PixelWidth;
            int height = writeableBitmap.PixelHeight;
            int stride = width * ((writeableBitmap.Format.BitsPerPixel + 7) / 8);
            byte[] pixels = new byte[height * stride];
            writeableBitmap.CopyPixels(pixels, stride, 0);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * stride + 4 * x;

                    byte blue = pixels[index];
                    byte green = pixels[index + 1];
                    byte red = pixels[index + 2];
                    byte alpha = pixels[index + 3];

                    if (red == 255 && green == 255 && blue == 255 && alpha == 255)
                    {
                        pixels[index] = 0;
                        pixels[index + 1] = 0;
                        pixels[index + 2] = 0;
                        pixels[index + 3] = 0;
                    }
                }
            }

            writeableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);

            image.Source = writeableBitmap;
            return image;
        }
        private void SetSelectionCanBgASImage(Image image)
        {
            ImageSource imageSource = image.Source;

            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = imageSource;
            imageBrush.Stretch = Stretch.UniformToFill;

            _selection.SelectCan.Background = imageBrush;
        }
        private RenderTargetBitmap GetRenderedCopy()
        {
            var renderTargetBitmap = new RenderTargetBitmap(
                (int)_selection.Width,
                (int)_selection.Height,
                96, 96,
                PixelFormats.Pbgra32
            );
            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                var visualBrush = new VisualBrush(DrawingCanvas)
                {
                    Stretch = Stretch.None,
                    ViewboxUnits = BrushMappingMode.Absolute,
                    Viewbox = new Rect(_firstSelectionStart, _firstSelectionEnd)
                };
                drawingContext.DrawRectangle(
                    visualBrush,
                    null,
                    new Rect(new Size(_selection.Width, _selection.Height))
                );
            }
            renderTargetBitmap.Render(drawingVisual);
            return renderTargetBitmap;
        }
     
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (!(_selection is null) &&
                _selection._isDraggingSelection)
            {
                _selection.ChangeSizeForSelection(e);
            }
            else if (!(_changedSizeText is null) &&
                    _changedSizeText._isDraggingSelection)
            {
                _changedSizeText.ChangeSizeForSelection(e);
            }
        }
        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!(_selection is null))
            {
                _selection._isDraggingSelection = false;
                _selection.IfSelectionIsClicked = false;
                _selection._selectionSizeToChangeSize = SelectionSide.Nothing;
            }
            else if (!(_changedSizeText is null))
            {
                _changedSizeText._isDraggingSelection = false;
                _changedSizeText.IfSelectionIsClicked = false;
                _changedSizeText._selectionSizeToChangeSize = SelectionSide.Nothing;
            }
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(_selection is null) && !_selection.IfSelectionIsClicked)
            {
                SetSelectedItemInDrawingCanvas();
                _selection = null;
                RemoveSelection();
            }
        }
        private void SetSelectedItemInDrawingCanvas()
        {
            Brush backgroundBrush = _selection.SelectCan.Background;

            if (backgroundBrush is ImageBrush imageBrush)
            {
                ImageSource imageSource = imageBrush.ImageSource;

                Image image = new Image
                {
                    Source = imageSource,
                    Width = _selection.SelectCan.ActualWidth,
                    Height = _selection.SelectCan.ActualHeight
                };

                Canvas.SetLeft(image, Canvas.GetLeft(_selection));
                Canvas.SetTop(image, Canvas.GetTop(_selection));
                DrawingCanvas.Children.Add(image);
            }
        }
        private void RemoveSelection()
        {
            for (int i = 0; i < DrawingCanvas.Children.Count; i++)
            {
                if (DrawingCanvas.Children[i] is Selection)
                {
                    DrawingCanvas.Children.RemoveAt(i);
                }
            }
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateTextLocation();
        }

        private bool IfTextBoxSizeChanging = false;
        private void PaintWindow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IfTexting)
            {
                InitSettingsForTextBox();
                Point mousePoint = e.GetPosition(DrawingCanvas);

                Canvas.SetLeft(_changedSizeText, mousePoint.X);
                Canvas.SetTop(_changedSizeText, mousePoint.Y);

                DrawingCanvas.Children.Add(_changedSizeText);
                _textBox.FontFamily = _text._chosenFont;
                _textBox.FontSize = _text._chosenFontSize;
                _textBox.Text = "woidfsljkro;tisfdgjirejfhgnusdfgjnvuosfdjnvupiogfjdnv";
            }
        }
        private void InitSettingsForTextBox()
        {
            _changedSizeText = new Selection();
            _changedSizeText.Height = 50;
            _changedSizeText.SelectionBorder.Height = 50;
            _changedSizeText.Width = 160;
            _changedSizeText.SelectionBorder.Width = 160;

            _textBox = new TextBox()
            {
                Visibility = Visibility.Visible,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                FontSize = 14,
                AcceptsReturn = true,
                Height = 40,
                Width = 150,
                BorderThickness = new Thickness(3),
                BorderBrush = new SolidColorBrush(Colors.Green)
            };

            _textBox.Width = _changedSizeText.Width - 10;
            _textBox.Height = _changedSizeText.Height - 10;

            Canvas.SetLeft(_textBox, 0);
            Canvas.SetTop(_textBox, 0);

            _changedSizeText.SelectCan.Children.Add(_textBox);
        }

    }
}

