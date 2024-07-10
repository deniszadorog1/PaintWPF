﻿using System;
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
using System.Security.AccessControl;
using System.Diagnostics.Eventing.Reader;
using System.Security.Policy;

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

        private bool _ifDrawing = false;
        private bool _ifFilling = false;
        private bool _ifErazing = false;
        private bool _ifFiguring = false;
        private bool _ifSelection = false;
        private bool _ifTexting = false;
        private bool _ifPickingColor = false;
        private bool _ifChangingFigureSize = false;

        private bool IfSelectionIsMaken = false;

        private FigureTypes? _figType = null;
        private Shape _figToPaint;
        private Polyline poligonFigure = null;
        private bool _ifCurvnessIsDone = false;

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
        Selection _figureSizing = null;
        LineSizing _lineSizeing = null;

        private TextEditor _text = null;
        private RichTextBox _richTexBox = null;

        private double _startThisHeight;
        private Selection _changedSizeText = null;

        private List<Button> _customColors = new List<Button>();
        private int _customColorIndex = 0;

        private Button _chosenToPaintButton = null;

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
            InitCustomColorButtons();
        }
        public void InitCustomColorButtons()
        {
            _customColors.Clear();
            _customColors.Add(OnePaletteColor);
            _customColors.Add(TwoPaletteColor);
            _customColors.Add(ThreePaletteColor);
            _customColors.Add(FourPaletteColor);
            _customColors.Add(FivePaletteColor);
            _customColors.Add(SixPaletteColor);
            _customColors.Add(SevenPaletteColor);
            _customColors.Add(EightPaletteColor);
            _customColors.Add(NinePaletteColor);
            _customColors.Add(TenPaletteColor);
        }
        public void InitStartHeight()
        {
            double height = Height;
            _startThisHeight = height;
        }
        public void InitForTextControl()
        {
            _text = new TextEditor(_main, _changedSizeText);

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
            Ellipse external = new Ellipse();
            external.Width = button.Width + 1;
            external.Height = button.Height + 1;

            external.Stroke = Brushes.Blue;
            external.StrokeThickness = 1;

            external.Margin = new Thickness(-10);

            button.Content = external;
            _chosenToPaintButton = button;
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
            Pallete pallete = new Pallete(_main.PalleteMod, GetSelectedColor());
            pallete.ShowDialog();

            InitColorInCUstomColor(_main.PalleteMod.ChosenColor);
        }
        private SolidColorBrush GetSelectedColor()
        {
            if (_chosenToPaintButton is null || 
                _chosenToPaintButton.Name == "FirstColor") return _main.FirstColor;
                return _main.SecondColor; 
        }
        private void InitColorInCUstomColor(SolidColorBrush color)
        {
            if (color is null) return;
            if (_customColorIndex == _customColors.Count - 1)
            {
                MoveColorsIntListInLeft(color);
                return;
            }
            _customColors[_customColorIndex].Background = color;
            _customColorIndex++;
            if (_chosenToPaintButton is null ||
                _chosenToPaintButton.Name == "FirstColor")
            {
                _main.FirstColor = new SolidColorBrush(color.Color);
                FirstColor.Background = new SolidColorBrush(color.Color);
            }
            else if (_chosenToPaintButton.Name == "SecondColor")
            {
                _main.SecondColor = new SolidColorBrush(color.Color);
                SecondColor.Background = new SolidColorBrush(color.Color);
            }
        }
        private void MoveColorsIntListInLeft(SolidColorBrush color)
        {
            for (int i = 0; i < _customColors.Count - 1; i++)
            {
                _customColors[i].Background = _customColors[i + 1].Background;
            }
            _customColors[_customColors.Count - 1].Background = color;
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
            EndWithPolygonFigures();
            ClearFigureSizingInClicks();
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
                else if (but.Name == "ColorDrop")
                {
                    _type = ActionType.PickingColor;
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

            if (_ifTexting)
            {

            }
            else if (_ifPickingColor)
            {
                EnterInitedColor(e);
            }
            else if (_ifSelection)
            {
                MakeSelection(e);
            }
            else if (_ifFilling)
            {
                RenderCanvasToBitmap();
                PerformFloodFill((int)previousPoint.X, (int)previousPoint.Y);
            }
            else if (_ifFiguring)
            {
                InitShapesToPaint(e);
            }
            else if (_ifChangingFigureSize)
            {

            }
            else
            {
                previousPoint = e.GetPosition(DrawingCanvas);
                SetMarkers(e);
            }
        }
        private void EnterInitedColor(MouseEventArgs e)
        {
            Color gotColor = GetColorAtTempPosition(e);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _main.FirstColor = new SolidColorBrush(gotColor);
                FirstColor.Background = _main.FirstColor;
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                _main.SecondColor = new SolidColorBrush(gotColor);
                SecondColor.Background = _main.SecondColor;
            }
        }
        private Color GetColorAtTempPosition(MouseEventArgs e)
        {
            Point position = e.GetPosition(DrawingCanvas);
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(
                (int)DrawingCanvas.ActualWidth,
                (int)DrawingCanvas.ActualHeight,
                96, 96,
                PixelFormats.Pbgra32);

            renderTargetBitmap.Render(DrawingCanvas);

            CroppedBitmap croppedBitmap = new CroppedBitmap(
                renderTargetBitmap,
                new Int32Rect((int)position.X, (int)position.Y, 1, 1));

            byte[] pixels = new byte[4];
            croppedBitmap.CopyPixels(pixels, 4, 0);

            return Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);
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
                _ifDrawing = true;
            }
            else if (_type == ActionType.Figuring)
            {
                _ifFiguring = true;
            }
            else if (_type == ActionType.Erazing)
            {
                _ifErazing = true;
            }
            else if (_type == ActionType.Filling)
            {
                _ifFilling = true;
            }
            else if (_type == ActionType.Selection)
            {
                _ifSelection = true;
                IfSelectionIsMaken = false;
            }
            else if (_type == ActionType.Text)
            {
                _ifTexting = true;
            }
            else if (_type == ActionType.PickingColor)
            {
                _ifPickingColor = true;
            }
            else if (_type == ActionType.ChangingFigureSize)
            {
                _ifChangingFigureSize = true;
            }
            else
            {
                _ifDrawing = true;
            }
        }
        private void MakeAllActionsNegative()
        {
            _ifSelection = false;
            _ifDrawing = false;
            _ifErazing = false;
            _ifFiguring = false;
            _ifFilling = false;
            _ifTexting = false;
            _ifPickingColor = false;
            _ifChangingFigureSize = false;

            IfSelectionIsMaken = false;
        }
        private Point _startPoint;
        private Point _endPoint;
        private Polyline _polyline;
        private PathFigure _pathFigure;
        private BezierSegment _bezierSegment;
        private bool _isDrawingLine;
        private bool _isAdjustingCurve;
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
                    {
                        if (!_isDrawingLine && !_isAdjustingCurve)
                        {
                            _startPoint = e.GetPosition(DrawingCanvas);
                            _polyline = new Polyline
                            {
                                Stroke = Brushes.Black,
                                StrokeThickness = 2,
                                Points = new PointCollection { _startPoint }
                            };

                            DrawingCanvas.Children.Add(_polyline);
                            _isDrawingLine = true;
                        }
                        else if (_isDrawingLine)
                        {
                            _endPoint = _polyline.Points[1];// e.GetPosition(DrawingCanvas);
                            _polyline.Points.Add(_endPoint);
                            DrawingCanvas.Children.Remove(_polyline);

                            _pathFigure = new PathFigure { StartPoint = _startPoint };
                            _bezierSegment = new BezierSegment
                            {
                                Point1 = _startPoint,
                                Point2 = _endPoint,
                                Point3 = _endPoint
                            };
                            _pathFigure.Segments.Add(_bezierSegment);

                            PathGeometry pathGeometry = new PathGeometry();
                            pathGeometry.Figures.Add(_pathFigure);

                            _figToPaint = new System.Windows.Shapes.Path
                            {
                                Stroke = Brushes.Black,
                                StrokeThickness = 2,
                                Data = pathGeometry
                            };

                            DrawingCanvas.Children.Add(_figToPaint);
                            _isDrawingLine = false;
                            _isAdjustingCurve = true;


                            _ifCurvnessIsDone = true;
                        }

                    }
                    return;
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

            Console.WriteLine(_figToPaint.ActualHeight);
            Console.WriteLine(_figToPaint.ActualWidth);

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

            if (_ifSelection)
            {
                ChangeSelectionSize(e);
            }
            else if (_ifDrawing || _ifErazing)
            {
                BrushPaint(e);
            }
/*            else if (_ifFiguring && (!(_figToPaint is null) ||
                !(_pathFigure is null) || !(_polyline is null)))
            {
                PaintFigures(e);
            }*/
            else if (!(_figureSizing is null) && _ifChangingFigureSize)
            {
                _firstSelectionEnd = new Point(e.GetPosition(DrawingCanvas).X, e.GetPosition(DrawingCanvas).Y);
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
                    e.GetPosition(DrawingCanvas).Y - Canvas.GetTop(_figToPaint)),
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
                }
            }
            else if (_figType == FigureTypes.Curve)
            {
                if (_isDrawingLine)
                {
                    Point currentPoint = e.GetPosition(DrawingCanvas);
                    if (_polyline.Points.Count > 1)
                        _polyline.Points[1] = currentPoint;
                    else
                        _polyline.Points.Add(currentPoint);
                }
                else if (_isAdjustingCurve)
                {
                    Point currentPoint = e.GetPosition(DrawingCanvas);
                    _bezierSegment.Point2 = currentPoint;
                }
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
            if (_main.TempBrushType == BrushType.UsualBrush || _ifErazing)
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
            }
            else if (_main.TempBrushType == BrushType.ColorPencil)
            {
                PaintByPngBrush(e);
            }
            else if (_main.TempBrushType == BrushType.Marker)
            {
                MarkerBrushPaint(e);
            }
            else if (_main.TempBrushType == BrushType.TexturePencil)
            {
                PaintByPngBrush(e);
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

                Stroke = _ifErazing ? Brushes.White : _main.ColorToPaint,
                StrokeThickness = brushThickness,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeLineJoin = PenLineJoin.Round,
            };

            Image image = ConvertLineToImage(line, DrawingCanvas.Width, DrawingCanvas.Height);
            


            DrawingCanvas.Children.Add(line);

        }
        public Image ConvertLineToImage(Line line, double canvasWidth, double canvasHeight)
        {
            // Создаем DrawingVisual для рисования линии
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawLine(
                    new Pen(line.Stroke, line.StrokeThickness)
                    {
                        StartLineCap = line.StrokeStartLineCap,
                        EndLineCap = line.StrokeEndLineCap,
                        LineJoin = line.StrokeLineJoin
                    },
                    new Point(line.X1, line.Y1),
                    new Point(line.X2, line.Y2)
                );
            }

            // Определяем размеры для RenderTargetBitmap
            Rect bounds = new Rect(new Point(Math.Min(line.X1, line.X2), Math.Min(line.Y1, line.Y2)),
                                   new Size(Math.Abs(line.X2 - line.X1), Math.Abs(line.Y2 - line.Y1)));

            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)canvasWidth, (int)canvasHeight, 96, 96, PixelFormats.Pbgra32);
            renderBitmap.Render(drawingVisual);

            // Создаем Image с полученным RenderTargetBitmap
            Image image = new Image
            {
                Source = renderBitmap,
                Width = bounds.Width,
                Height = bounds.Height
            };

            // Устанавливаем позицию Image на Canvas
            Canvas.SetLeft(image, bounds.X);
            Canvas.SetTop(image, bounds.Y);

            return image;
        }
        public Brush ConvertColorIntoBrushes()
        {
            return _main.ColorToPaint;
        }
        private void Field_MouseUp(object sender, MouseEventArgs e)
        {
            //check for selection
            if (_ifSelection && !IfSelectionIsMaken) MakeSelection();
            if (_ifFiguring) FiguringMouseUp();

            CheckForFigurePainting();

            MakeAllActionsNegative();

            sprayTimer.Stop();
            SaveCanvasState();
        }
        public void FiguringMouseUp()
        {
            bool polygonCheck = IfPolygonFigureIsDone();
            bool curveCheck = IfCurveFigureIsDone();
             
            if (polygonCheck && curveCheck)
            {
                 InitFigureInSizingBorder();

                ReloadCurvePainting();
            }
        }
        private void ReloadCurvePainting()
        {
            _ifCurvnessIsDone = false;
            _isDrawingLine = false;
            _isAdjustingCurve = false;
        }
        public bool IfCurveFigureIsDone()
        {
            if (_figType != FigureTypes.Curve) return true;

            if (_ifCurvnessIsDone) GetPathSize();

            return _ifCurvnessIsDone;
        }
        public void GetPathSize()
        {
            if (_figToPaint is System.Windows.Shapes.Path path)
            {
                Rect bounds = path.Data.Bounds;

                GeneralTransform transform = path.TransformToAncestor((Visual)path.Parent);
                Rect transformedBounds = transform.TransformBounds(bounds);

                _figToPaint.Width = transformedBounds.Width;
                _figToPaint.Height = transformedBounds.Height;
            }
        }
        public bool IfPolygonFigureIsDone()
        {
            if (_figType != FigureTypes.Polygon) return true;

            if (((Polyline)_figToPaint).Points.Count > 2)
            {
                // Check for differ between first and last poit
                double xDiffer = Math.Abs(((Polyline)_figToPaint).Points.Last().X -
                    ((Polyline)_figToPaint).Points.First().X);
                double yDiffer = Math.Abs(((Polyline)_figToPaint).Points.Last().Y -
                    ((Polyline)_figToPaint).Points.First().Y);

                if (xDiffer <= 10 && yDiffer <= 10)
                {
                    ((Polyline)_figToPaint).Points.RemoveAt(((Polyline)_figToPaint).Points.Count - 1);
                    ((Polyline)_figToPaint).Points.Add(((Polyline)_figToPaint).Points.First());

                    ((Polyline)_figToPaint).InvalidateVisual();
                    DrawingCanvas.UpdateLayout();

                    CalculatePolylineSize();
                    return true;
                }
            }
            return false;
        }
        public void GetBoundingBox(UserControl control)
        {
            if (_figToPaint is Polyline polyline)
            {
                double minX = double.MaxValue;
                double minY = double.MaxValue;

                foreach (var point in polyline.Points)
                {
                    if (point.X < minX) minX = point.X;
                    if (point.Y < minY) minY = point.Y;
                }

                Point polylineStartPoint = new Point(minX, minY);
                Point canvasPosition = polyline.TransformToAncestor(DrawingCanvas).Transform(polylineStartPoint);

                Canvas.SetLeft(control, canvasPosition.X);
                Canvas.SetTop(control, canvasPosition.Y);
            }
            else if (_figToPaint is System.Windows.Shapes.Path path)
            {
                Rect bounds = path.Data.Bounds;

                Point pathStartPoint = new Point(bounds.X, bounds.Y);

                Point canvasPosition = path.TransformToAncestor(DrawingCanvas).Transform(pathStartPoint);

                Canvas.SetLeft(_figureSizing, canvasPosition.X);
                Canvas.SetTop(_figureSizing, canvasPosition.Y);
            }
        }
        public void CalculatePolylineSize()
        {
            if (_figToPaint is Polyline polyline)
            {
                double minX = double.MaxValue;
                double maxX = double.MinValue;
                double minY = double.MaxValue;
                double maxY = double.MinValue;

                foreach (var point in polyline.Points)
                {
                    if (point.X < minX)
                        minX = point.X;
                    if (point.X > maxX)
                        maxX = point.X;
                    if (point.Y < minY)
                        minY = point.Y;
                    if (point.Y > maxY)
                        maxY = point.Y;
                }
                polyline.Width = maxX - minX;
                polyline.Height = maxY - minY;
            }
        }
        private void InitFigureInSizingBorder()
        {
            if (!(DrawingCanvas.Children[DrawingCanvas.Children.Count - 1] is Shape)) return;
            Shape createdFigure = (Shape)DrawingCanvas.Children[DrawingCanvas.Children.Count - 1];

            ImageSource source = null;
            if (createdFigure.Width is double.NaN || createdFigure.Height is double.NaN)
            {
                GetShapeSize(createdFigure);
                source = ConvertShapeToImage(createdFigure, (int)createdFigure.Width, (int)createdFigure.Height);
            }
            else
            {
                source = ConvertShapeToImage(createdFigure, (int)createdFigure.Width, (int)createdFigure.Height);
            }
            Image shapeImg = new Image()
            {
                Source = source,
                Height = source.Height,
                Width = source.Width
            };

            if (_figType == FigureTypes.Line)
            {
                _lineSizeing = new LineSizing((Polyline)_figToPaint);

                //InitLocationForFigure(_lineSizeing);
                GetLinePositionOnCanvas();

                DrawingCanvas.Children.RemoveAt(DrawingCanvas.Children.Count - 1);
                DrawingCanvas.Children.Add(_lineSizeing);
            }
            else
            {
                _figureSizing = new Selection(shapeImg);

                _figureSizing.Height = _figureSizing.SelectionBorder.Height;
                _figureSizing.Width = _figureSizing.SelectionBorder.Width;

                InitLocationForFigure(_figureSizing);

                DrawingCanvas.Children.RemoveAt(DrawingCanvas.Children.Count - 1);
                DrawingCanvas.Children.Add(_figureSizing);
            }
            _type = ActionType.ChangingFigureSize;
        }
        public void GetLinePositionOnCanvas()
        {
            if (DrawingCanvas.Children[DrawingCanvas.Children.Count - 1] is Polyline line)
            {
                double xLoc = Canvas.GetLeft(line);
                double yLoc = Canvas.GetTop(line);

                Canvas.SetLeft(_lineSizeing, xLoc);
                Canvas.SetTop(_lineSizeing, yLoc);
            }
        }
        private void InitLocationForFigure(UserControl control)
        {
            if (_figType != FigureTypes.Polygon &&
                _figType != FigureTypes.Curve && 
                _figType != FigureTypes.Line)
            {
                Canvas.SetTop(_figureSizing, Canvas.GetTop(DrawingCanvas.Children[DrawingCanvas.Children.Count - 1]));
                Canvas.SetLeft(_figureSizing, Canvas.GetLeft(DrawingCanvas.Children[DrawingCanvas.Children.Count - 1]));
                return;
            }
            GetBoundingBox(control);
        }
        private void GetShapeSize(Shape shape)
        {
            if (shape is Polyline poly)
            {
                shape.Width = Math.Abs(poly.Points.Last().X -
                 poly.Points.First().X);
                shape.Height = Math.Abs(poly.Points.Last().Y -
                    poly.Points.First().Y);
            }
        }

        private ImageSource ConvertShapeToImage(Shape shape, int width, int height)
        {
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            DrawingVisual drawingVisual = new DrawingVisual();

            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                // Render the shape onto the DrawingContext
                VisualBrush visualBrush = new VisualBrush(shape);
                drawingContext.DrawRectangle(visualBrush, null, new Rect(0, 0, width, height));
            }

            renderTargetBitmap.Render(drawingVisual);

            return renderTargetBitmap;
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
                IfSelectionIsMaken = true;
            }
        }
        private void CheckForFigurePainting()
        {
            if (_figToPaint is null) return;
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
            EndWithPolygonFigures();
            ClearFigureSizingInClicks();

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

            Point start = new Point(Canvas.GetLeft(_selectionRect), Canvas.GetTop(_selectionRect));
            Point end = new Point(start.X + _selectionRect.Width, start.Y + _selectionRect.Height);
            
            DeleteAndTrimElements(start, end);

            //ConvertRegionToWhite();
        }
        private void DeleteAndTrimElements(Point point1, Point point2)
        {
            double minX = Math.Min(point1.X, point2.X);
            double minY = Math.Min(point1.Y, point2.Y);
            double maxX = Math.Max(point1.X, point2.X);
            double maxY = Math.Max(point1.Y, point2.Y);

            var elementsToRemove = new List<UIElement>();
            var elementsToAdd = new List<UIElement>();

            for(int i = 0; i < DrawingCanvas.Children.Count; i++)
            {
                var bounds = VisualTreeHelper.GetDescendantBounds(DrawingCanvas.Children[i]);

                var topLeft = DrawingCanvas.Children[i].
                    TransformToAncestor(DrawingCanvas).Transform(new Point(0, 0));

                var bottomRight = new Point(
                    topLeft.X + bounds.Width, topLeft.Y + bounds.Height);

                if (topLeft.X >= minX && topLeft.Y >= minY && 
                    bottomRight.X <= maxX && bottomRight.Y <= maxY)
                {
                    elementsToRemove.Add(DrawingCanvas.Children[i]);
                }
                else if (IsPartiallyInRegion(topLeft, bottomRight, minX, minY, maxX, maxY))
                {
                    if (DrawingCanvas.Children[i] is Image image)
                    {
                        var trimmedImages = TrimImage(image, minX, minY, maxX, maxY, topLeft);
                        if (trimmedImages != null && trimmedImages.Count > 0)
                        {
                            elementsToAdd.AddRange(trimmedImages);
                            elementsToRemove.Add(DrawingCanvas.Children[i]);
                        }
                    }
                    else
                    {
                        var trimmedElement = TrimElement(DrawingCanvas.Children[i],
                            minX, minY, maxX, maxY);
                        if (trimmedElement != null)
                        {
                            elementsToAdd.Add(trimmedElement);
                            elementsToRemove.Add(DrawingCanvas.Children[i]);
                        }
                    }
                }
            }

            for(int i = 0; i < elementsToRemove.Count; i++)
            {
                DrawingCanvas.Children.Remove(elementsToRemove[i]);
            }
            for(int i = 0; i < elementsToAdd.Count; i++)
            {
                DrawingCanvas.Children.Add(elementsToAdd[i]);
            }
        }
        private List<Image> TrimImage(Image image, double minX, double minY, double maxX, double maxY, Point topLeft)
        {
            var bitmap = image.Source as BitmapSource;
            if (bitmap == null) return null;

            var trimmedImages = new List<Image>();

            // Определяем области для обрезки и сохранения
            var areas = new List<Int32Rect>
    {
        // Левая часть
        new Int32Rect(0, 0, (int)Math.Max(0, minX - topLeft.X), bitmap.PixelHeight),
        // Правая часть
        new Int32Rect((int)Math.Min(bitmap.PixelWidth, maxX - topLeft.X), 0, (int)Math.Max(0, bitmap.PixelWidth - (maxX - topLeft.X)), bitmap.PixelHeight),
        // Верхняя часть
        new Int32Rect(0, 0, bitmap.PixelWidth, (int)Math.Max(0, minY - topLeft.Y)),
        // Нижняя часть
        new Int32Rect(0, (int)Math.Min(bitmap.PixelHeight, maxY - topLeft.Y), bitmap.PixelWidth, (int)Math.Max(0, bitmap.PixelHeight - (maxY - topLeft.Y)))
    };

            foreach (var area in areas)
            {
                if (area.Width > 0 && area.Height > 0)
                {
                    var croppedBitmap = new CroppedBitmap(bitmap, area);
                    var newImage = new Image
                    {
                        Source = croppedBitmap,
                        Width = area.Width,
                        Height = area.Height
                    };

                    // Расчет позиции нового изображения на Canvas
                    double offsetX = area.X == 0 ? topLeft.X : topLeft.X + area.X;
                    double offsetY = area.Y == 0 ? topLeft.Y : topLeft.Y + area.Y;

                    Canvas.SetLeft(newImage, offsetX);
                    Canvas.SetTop(newImage, offsetY);

                    trimmedImages.Add(newImage);
                }
            }

            return trimmedImages;
        }
        private bool IsPartiallyInRegion(Point topLeft, Point bottomRight, 
            double minX, double minY, double maxX, double maxY)
        {
            return (topLeft.X < maxX && bottomRight.X > minX && 
                topLeft.Y < maxY && bottomRight.Y > minY);
        }
        private UIElement TrimElement(UIElement element, double minX, 
            double minY, double maxX, double maxY)
        {
            if (element is Line line)
            {
                return TrimLine(line, minX, minY, maxX, maxY);
            }
            else if (element is Polyline polyline)
            {
                return TrimPolyline(polyline, minX, minY, maxX, maxY);
            }

            //return new UIElement();
            throw new NotSupportedException("Unsupported element type for trimming");
        }
        private Polyline TrimPolyline(Polyline polyline, double minX, 
            double minY, double maxX, double maxY)
        {
            var trimmedPoints = new PointCollection();

            foreach (var point in polyline.Points)
            {
                if (point.X >= minX && point.X <= maxX && 
                    point.Y >= minY && point.Y <= maxY)
                {
                    trimmedPoints.Add(point);
                }
            }

            return new Polyline { Points = trimmedPoints, Stroke = polyline.Stroke, StrokeThickness = polyline.StrokeThickness };
        }
        private Line TrimLine(Line line, double minX, double minY, double maxX, double maxY)
        {

            if (line.X1 >= minX && line.X1 <= maxX && 
                line.Y1 >= minY && line.Y1 <= maxY)
            {
                line.X1 = Math.Max(minX, Math.Min(line.X1, maxX));
                line.Y1 = Math.Max(minY, Math.Min(line.Y1, maxY));
            }
            if (line.X2 >= minX && line.X2 <= maxX && 
                line.Y2 >= minY && line.Y2 <= maxY)
            {
                line.X2 = Math.Max(minX, Math.Min(line.X2, maxX));
                line.Y2 = Math.Max(minY, Math.Min(line.Y2, maxY));
            }
            return line;
        }

        private void ConvertRegionToWhite()
        {
            Rect selectedBounds = new Rect(_firstSelectionStart, _firstSelectionEnd);
            List<UIElement> elementsToRemove = new List<UIElement>();
            foreach (UIElement child in DrawingCanvas.Children)
            {
                Rect childBounds = VisualTreeHelper.GetDescendantBounds(child);
                Point childTopLeft = child.TranslatePoint(new Point(), DrawingCanvas);
                if (selectedBounds.IntersectsWith(childBounds))
                {
                    if (selectedBounds.Contains(childTopLeft) && 
                        selectedBounds.Contains(new Point(childTopLeft.X + childBounds.Width, 
                        childTopLeft.Y + childBounds.Height)))
                    {
                        elementsToRemove.Add(child);
                    }
                    else
                    {
                        Rect intersection = Rect.Intersect(selectedBounds, childBounds);
                        if (child is Shape shape)
                        {
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
            else if (!(_figureSizing is null) && _ifChangingFigureSize &&
                e.LeftButton == MouseButtonState.Pressed)
            {
                _figureSizing._isDraggingSelection = true;
                _figureSizing.ChangeSizeForSelection(e);
            }
            else if (!(_lineSizeing is null) &&
                e.LeftButton == MouseButtonState.Pressed)
            {
                //_lineSizeing._isDraggingSelection = true;
                _lineSizeing.ChangeSizeForSelection(e);
            }

            if (_ifFiguring && (!(_figToPaint is null) ||
                 !(_pathFigure is null) || !(_polyline is null)))
            {
                PaintFigures(e);
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
            else if (!(_figureSizing is null))
            {
                _figureSizing._isDraggingSelection = false;
                _figureSizing.IfSelectionIsClicked = false;
                _figureSizing._selectionSizeToChangeSize = SelectionSide.Nothing;
            }
            else if(!(_lineSizeing is null))
            {
                _lineSizeing._isDraggingSelection = false;
                _lineSizeing.IfSelectionIsClicked = false;
                _lineSizeing._moveRect = LineSizingRectType.Nothing;
            }
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(_selection is null) && !_selection.IfSelectionIsClicked)
            {
                SetSelectedItemInDrawingCanvas();
                _selection = null;
                RemoveSelection();
                return;
            }
            if (!(_figureSizing is null) && !_figureSizing.IfSelectionIsClicked)
            {
                ClearFigureSizing();
                return;
            }
            else if(!(_lineSizeing is null) && !_lineSizeing.IfSelectionIsClicked)
            {
                ClearLineSizing();
                return;
            }
            if (!(_changedSizeText is null) &&
                !_changedSizeText.IfSelectiongClicked && _ifDoubleClicked)
            {
                ConvertRichTextBoxIntoImage();
                _ifDoubleClicked = false;
                DrawingCanvas.Children.RemoveAt((int)_changedSizeText.Tag);
                _changedSizeText = null;

            }
            if (!(_changedSizeText is null) && _changedSizeText.IfSelectiongClicked)
            {
                _changedSizeText.IfSelectiongClicked = false;
            }
        }
        private void ClearFigureSizing()
        {
            if (!(_figureSizing is null))
            {
                InitSizedFigureIntoCanvas();
                _figureSizing = null;
                _type = ActionType.Figuring;
                _figToPaint = null;
                RemoveSelection();
            }
        }
        private void ClearLineSizing()
        {
            if (!(_lineSizeing is null))
            {
                InitLineInCanvas();
                _lineSizeing = null;
                _type = ActionType.Figuring;
                _figToPaint = null;
                RemoveLineSizing();
            }
        }
        private void ClearFigureSizingInClicks()
        {
            ClearFigureSizing();
            ClearLineSizing();
        }

        private void InitLineInCanvas()
        {
            //Get line to add
            Line lineToAdd = _lineSizeing.GetPolyLineObject();
            if (lineToAdd is null) return;

            //Init codes for positionation
            InitCordForLine(lineToAdd);

            //Delete line from _lineSizing
            _lineSizeing.RemoveLineFromCanvas();

            //Add in DrawingCanvas
            DrawingCanvas.Children.Add(lineToAdd);
        }
        public void InitCordForLine(Line line)
        {
            //Point lineStartPoint = new Point(Math.Min(line.X1, line.X2), Math.Min(line.Y1, line.Y2));
            Point lineStartPoint = new Point(line.X1, line.Y1);

            GeneralTransform transform = line.TransformToAncestor(DrawingCanvas);
            Point canvasPosition = transform.Transform(lineStartPoint);

            Canvas.SetLeft(line, canvasPosition.X);
            Canvas.SetTop(line, canvasPosition.Y);
        }
        private void RemoveLineSizing()
        {
            for(int i = 0; i < DrawingCanvas.Children.Count; i++)
            {
                if (DrawingCanvas.Children[i] is LineSizing)
                {
                    DrawingCanvas.Children.RemoveAt(i);
                }
            }
        }
        private void InitSizedFigureIntoCanvas()
        {
            Image image = _figureSizing.GetShapeImageObject();

            Point imgLoc = _figureSizing.GetImageLocation();
            if (imgLoc.X == -1 && imgLoc.Y == -1) return;
            _figureSizing.RemoveImagesFromCanvas();

            DrawingCanvas.Children.Add(image);
            Canvas.SetLeft(image, Canvas.GetLeft(_figureSizing) + imgLoc.X);
            Canvas.SetTop(image, Canvas.GetTop(_figureSizing) + imgLoc.Y);
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
        private bool _ifDoubleClicked = false;
        private void PaintWindow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_ifTexting && !_ifDoubleClicked)
            {
                InitSettingsForTextBox();
                Point mousePoint = e.GetPosition(DrawingCanvas);

                Canvas.SetLeft(_changedSizeText, mousePoint.X);
                Canvas.SetTop(_changedSizeText, mousePoint.Y);

                DrawingCanvas.Children.Add(_changedSizeText);
                _changedSizeText.Tag = DrawingCanvas.Children.Count - 1;

                _richTexBox.FontFamily = _text._chosenFont;
                _richTexBox.FontSize = _text._chosenFontSize;
                SetTextInRichTextBox();
                _ifDoubleClicked = true;
            }
        }
        private void SetTextInRichTextBox()
        {
            _richTexBox.Document.Blocks.Clear();
            _richTexBox.Document.Blocks.Add(new Paragraph(new Run("woidfsljkro;tisfdgjirejfhgnusdfgjnvuosfdjnvupiogfjdnv")));
        }
        private void InitSettingsForTextBox()
        {
            _changedSizeText = new Selection();
            _changedSizeText.Height = 50;
            _changedSizeText.SelectionBorder.Height = 50;
            _changedSizeText.Width = 160;
            _changedSizeText.SelectionBorder.Width = 160;

            _richTexBox = new RichTextBox()
            {
                Visibility = Visibility.Visible,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                FontSize = 14,
                AcceptsReturn = true,
                Height = 40,
                Width = 150,
                BorderThickness = new Thickness(3),
                BorderBrush = new SolidColorBrush(Colors.Green)
            };

            _richTexBox.Width = _changedSizeText.Width - 15;
            _richTexBox.Height = _changedSizeText.Height - 15;

            Canvas.SetLeft(_richTexBox, 5);
            Canvas.SetTop(_richTexBox, 5);

            _changedSizeText.SelectCan.Children.Add(_richTexBox);

            InitForTextControl();
        }
        private void ConvertRichTextBoxIntoImage()
        {
            RichTextBox toSave = _changedSizeText.GetRichTextBoxObject();
            toSave.BorderThickness = new Thickness(0);

            if (toSave is null) return;
            double width = toSave.ActualWidth + 10;
            double height = toSave.ActualHeight + 10;

            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(
                (int)width, (int)height, 96, 96, PixelFormats.Pbgra32);

            renderTargetBitmap.Render(toSave);
            Image img = new Image()
            {
                Source = renderTargetBitmap
            };

            Canvas.SetTop(img, Canvas.GetTop(_changedSizeText));
            Canvas.SetLeft(img, Canvas.GetLeft(_changedSizeText));

            DrawingCanvas.Children.Add(img);
        }
        public void EndWithPolygonFigures()
        {
            if (_figType == FigureTypes.Polygon && !(_figToPaint is null))
            {
                ((Polyline)_figToPaint).Points.Add(((Polyline)_figToPaint).Points.First());
            }
        }

        private void PaintWindow_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_ifSelection && !IfSelectionIsMaken) MakeSelection();
            if (_ifFiguring) FiguringMouseUp();

            if (!(_lineSizeing is null))
            {
                _lineSizeing._moveRect = LineSizingRectType.Nothing;
                _lineSizeing._isDraggingSelection = false;
            }
        }
        private void ChooseAllSelection_MouseLeftButtonDown(object sender, EventArgs e)
        {
            _selection = new Selection()
            {
                Height = DrawingCanvas.Height,
                Width = DrawingCanvas.Width
            };
            _selection.SelectionBorder.Height = DrawingCanvas.Height;
            _selection.SelectionBorder.Width = DrawingCanvas.Width;

            Canvas.SetTop(_selection, 0);
            Canvas.SetLeft(_selection, 0);

            DrawingCanvas.Children.Add(_selection);
        }
        private void SelectionMenu_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            EndWithPolygonFigures();
            ClearFigureSizingInClicks();
        }

    }
}

