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
using System.Runtime.InteropServices;


using PaintWPF.Models.Tools;
using PaintWPF.CustomControls;

using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;


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
        private Polyline _selectionLine = null;

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

        private List<string> _savedPathes = new List<string>();
        private DateTime? _lastSaveTime = null;
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
            DeleteText();
            _text = new TextEditor(_main, _changedSizeText);

            UpdateTextLocation();
            Grid.SetRow(_text, 0);
            Grid.SetColumn(_text, 1);

            CenterWindowPanel.Children.Add(_text);
            //_text.Visibility = Visibility.Hidden;

            //ValueBorder.Visibility = Visibility.Hidden;
        }
        private void DeleteText()
        {
            for (int i = 0; i < CenterWindowPanel.Children.Count; i++)
            {
                if (CenterWindowPanel.Children[i] is TextEditor)
                {
                    CenterWindowPanel.Children.RemoveAt(i);
                    return;
                }
            }
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

            if (!(_main.PalleteMod.ChosenColor is null))
            {
                InitColorInCUstomColor(_main.PalleteMod.ChosenColor);
            }
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
                if ((!(_chosenTool is null) && but.Name == _chosenTool.Name) ||
                    ((SolidColorBrush)but.BorderBrush).Color == _clickedBorderColor.Color) return;

                but.Background = brush;
                but.BorderBrush = borderBrush;
                return;
            }
            else if (sender is Border bord)
            {
                bord.Background = brush;
                bord.BorderBrush = borderBrush;
                return;
            }
            else if (sender is Grid grid)
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
                if (!(_chosenTool is null) && but.Name == _chosenTool.Name ||
                    ((SolidColorBrush)but.BorderBrush).Color == _clickedBorderColor.Color) return;

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
                ClearDynamicValues(brushClicked: but.Name == "Pen" || but.Name == "Erazer",
                    textClicked: but.Name == "Text");

                if (!(_chosenTool is null) &&
                    but.Name == _chosenTool.Name) return;
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
                MakeCustomSelection(e);
            }
            else if (_selectionType == SelectionType.All)
            {
                //other event is working 
            }
        }
        private void MakeCustomSelection(MouseEventArgs e)
        {
            _selectionLine = new System.Windows.Shapes.Polyline()
            {
                Stroke = Brushes.LightBlue,
                StrokeThickness = 3,
                Fill = Brushes.Transparent
            };

            Canvas.SetLeft(_selectionLine, e.GetPosition(DrawingCanvas).X);
            Canvas.SetTop(_selectionLine, e.GetPosition(DrawingCanvas).Y);

            _firstSelectionStart = new Point(e.GetPosition(DrawingCanvas).X, e.GetPosition(DrawingCanvas).Y);

            DrawingCanvas.Children.Add(_selectionLine);
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
                        else if (_isDrawingLine && _polyline.Points.Count > 1)
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
                        Geometry.Parse("M 20 10 L 160 10 A 10 10 0 0 1 170 20 " +
                        "L 170 140 A 10 10 0 0 1 160 150 " +
                        "L 20 150 A 10 10 0 0 1 10 140 " +
                        "L 10, 20 A 10 10 0 0 1 20 10");
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
                        Geometry.Parse("M 100 10 L 170 60 " +
                        "L 140 150 L 60 150 L 30 60 Z");
                        break;
                    }
                case FigureTypes.Hexagon:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse("M 100 10 L 150 40 " +
                        "L 150 100 L 100 130 L 50 100 L 50 40 Z");
                        break;
                    }
                case FigureTypes.RightArrow:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse("M 10 50 L 100 50 L 100 30 " +
                        "L 150 70 L 100 110 L 100 90 L 10 90 Z");
                        break;
                    }
                case FigureTypes.LeftArrow:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse("M 150 50 L 50 50 L 50 30 " +
                        "L 0 70 L 50 110 L 50 90 L 150 90 Z");
                        break;
                    }
                case FigureTypes.UpArrow:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse("M 50 150 L 50 50 L 30 50 " +
                        "L 70 0 L 110 50 L 90 50 L 90 150 Z");
                        break;
                    }
                case FigureTypes.DownArrow:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse("M 50 50 L 50 150 L 30 150 " +
                        "L 70 200 L 110 150 L 90 150 L 90 50 Z");
                        break;
                    }
                case FigureTypes.FourPointedStar:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse("M 95 20 L 110 70 L 180 80 L 110 100 " +
                        "L 95 160 L 75 100 L 20 80 L 80 70 L 95 20");
                        break;
                    }
                case FigureTypes.FivePointedStar:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse("M 100 10  L 130 90 L 200 90 L 145 130 " +
                        "L 170 200 L 100 160 L 30 200  L 55 130 L 0 90  L 70 90 Z");
                        break;
                    }
                case FigureTypes.SixPointedStar:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse("M 100 10 L 130 60 L 180 60 L 150 100 " +
                        "L 180 140 L 130 140 L 100 180 L 70 140 L 20 140 " +
                        "L 50 100 L 20 60 L 70 60 Z");
                        break;
                    }
                case FigureTypes.RoundedRectangularLeader:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse("M 120 10 L 150 10 Q 170, 10, 170 40 L 170 100 " +
                        "Q 170 120, 150 120 L 75 120 L 65 140 L 55 120 Q 25, 120, 25 100 " +
                        "L 25 40 Q 25 10, 40 10 Z");
                        break;
                    }
                case FigureTypes.OvalCallout:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse("M 100,100 A 40, 30 0 1 1 200, 100 " +
                        "A 60, 60 1 0 1 140, 130 L 125 145 L 120 130 Q 100 120 100 100 ");
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
                        Geometry.Parse("M 50 100 C 50 0 150 0 150 100 " +
                        "C 150 200 50 250 50 350 C 50, 250 -50 200 -50 100 " +
                        "C -50 0 50 0 50 100 Z");
                        break;
                    }
                case FigureTypes.Lightning:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse("M 80,50 L 120 40 L 150 80 L 140 85 " +
                        "L 175 120 L 165 125 L 200 170 L 140 140 " +
                        "L 150 135 L 105 90 L 120 85 Z");
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

            if (_figType != FigureTypes.Line &&
                _figType != FigureTypes.Polygon)
            {
                _figToPaint.Height = 0;
                _figToPaint.Width = 0;
            }

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
            }
            else if (_selectionType == SelectionType.Custom)
            {
                //Init Point in polyline
                AddPointsInPolyline(e);

                _firstSelectionEnd = new Point(e.GetPosition(DrawingCanvas).X, e.GetPosition(DrawingCanvas).Y);
            }
        }
        private void AddPointsInPolyline(MouseEventArgs e)
        {
            Point point = e.GetPosition(DrawingCanvas);

            int x = (int)(e.GetPosition(DrawingCanvas).X - Canvas.GetLeft(_selectionLine));
            int y = (int)(e.GetPosition(DrawingCanvas).Y - Canvas.GetTop(_selectionLine));

            _selectionLine.Points.Add(new Point(x, y));
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
            polyline.Stroke = ConvertColorIntoBrushes();
            polylines.Add(polyline);
            polyline.Points.Add(e.GetPosition(DrawingCanvas));
            DrawingCanvas.Children.Add(polyline);
            polyline.StrokeThickness = brushThickness + 1;
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

            //Image image = ConvertLineToImage(line, DrawingCanvas.Width, DrawingCanvas.Height);

            DrawingCanvas.Children.Add(line);

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

            if (_ifDrawing) ConvertPaintingInImage();

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

                    CalculatePolylineSize(_figToPaint);
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
        public void CalculatePolylineSize(Shape shape)
        {
            if (shape is Polyline polyline)
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

                Console.WriteLine(polyline.ActualWidth);
                Console.WriteLine(polyline.ActualHeight);
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
                if (!IfFigureSizeIsAcceptable(createdFigure)) return;

                source = ConvertShapeToImage(createdFigure, (int)createdFigure.Width, (int)createdFigure.Height);
            }
            else
            {
                if (!IfFigureSizeIsAcceptable(createdFigure)) return;
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
        private bool IfShapeHasPoints(Shape shape)
        {
            return shape is Polyline line ? line.Points.Count > 0 :
                shape is Polygon polygon ? polygon.Points.Count > 0 :
                shape is System.Windows.Shapes.Path path ? !(path.Data is null) : false;

        }
        private bool IfFigureSizeIsAcceptable(Shape createdFigure)
        {
            if (createdFigure.Width is double.NaN || createdFigure.Width == 0 ||
                   createdFigure.Height is double.NaN || createdFigure.Height == 0)
            {
                DrawingCanvas.Children.RemoveAt(DrawingCanvas.Children.Count - 1);
                return false;
            }
            return true;
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
            if (shape is Polyline poly && poly.Points.Count > 0)
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
        private const int _selectionSizeCorelation = 15;
        private void MakeSelection()
        {
            if (_selectionType == SelectionType.Rectangle)
            {
                MakeRectSelection();
            }
            else if (_selectionType == SelectionType.Custom)
            {
                MakeCustomSelection();
            }
        }
        private void MakeCustomSelection()
        {
            //Add Last point(to make like a polygon)
            _selectionLine.Points.Add(_selectionLine.Points.First());

            //Calc polyline size //smth strange in there 
            //CalculatePolylineSize(_selectionLine);

            //Set polylines location
            SetPolylineLocation(_selectionLine);

            //Selection Creation
            if (!IfCustomSelectionIsCreated(_selectionLine)) return;

            //Add line in selection
            AddSelectionLineInSelection(_selectionLine);

            //Get polyline image
            //InitSelectedBgInRectCanvas();

            InitSelectionBgInCustomCanvas();

            IfSelectionIsMaken = true;
        }
        private void InitSelectionBgInCustomCanvas()
        {
            Image img = GetRenderOfCustomCanvas(_selectionLine);
            SwipeWhiteColorWithTranspaentInImage(img);
            SetSelectionCanBgASImage(img);

            DeleteAndTrimElements(_selectionLine);
            //DeleteAndTrimElements(start, end);
        }
        private void DeleteAndTrimElements(Polyline polyline)
        {
            // Рассчитываем границы полилинии
            double minX = polyline.Points.Min(p => p.X);
            double minY = polyline.Points.Min(p => p.Y);
            double maxX = polyline.Points.Max(p => p.X);
            double maxY = polyline.Points.Max(p => p.Y);

            var elementsToRemove = new List<UIElement>();
            var elementsToAdd = new List<UIElement>();

            for (int i = 0; i < DrawingCanvas.Children.Count; i++)
            {
                var bounds = VisualTreeHelper.GetDescendantBounds(DrawingCanvas.Children[i]);

                if (DrawingCanvas.Children[i] is Image)
                {
                    bounds = new Rect(0, 0, DrawingCanvas.Width, DrawingCanvas.Height);
                }

                var topLeft = DrawingCanvas.Children[i].TransformToAncestor(DrawingCanvas).Transform(new Point(0, 0));
                var bottomRight = new Point(topLeft.X + bounds.Width, topLeft.Y + bounds.Height);

/*
                if (IsPartiallyInsidePolygon(polyline, topLeft, bottomRight))
                {*/
                    if (DrawingCanvas.Children[i] is Image image)
                    {
                        //Make Image for Bg
                        var trimmedImages = TrimImageCustom(image, polyline);

                        if (trimmedImages != null)
                        {
                            elementsToAdd.Add(trimmedImages);
                            elementsToRemove.Add(DrawingCanvas.Children[i]);
                        }
                    }
                //}
            }

            for (int i = 0; i < elementsToRemove.Count; i++)
            {
                DrawingCanvas.Children.Remove(elementsToRemove[i]);
            }
            if (elementsToAdd.Count == 1 && elementsToAdd.First() is Image img)
            {
                DrawingCanvas.Background = new ImageBrush()
                {
                    ImageSource = img.Source
                };
            }
            /*
                        for (int i = 0; i < elementsToAdd.Count; i++)
                        {
                            DrawingCanvas.Children.Add(elementsToAdd[i]);
                        }*/
        }

        private bool IsFullyInsidePolygon(Polyline polyline, Point topLeft, Point bottomRight)
        {
            var topRight = new Point(bottomRight.X, topLeft.Y);
            var bottomLeft = new Point(topLeft.X, bottomRight.Y);
            return IsPointInPolygon(polyline.Points, topLeft) && IsPointInPolygon(polyline.Points, bottomRight) &&
                   IsPointInPolygon(polyline.Points, topRight) && IsPointInPolygon(polyline.Points, bottomLeft);
        }
        private bool IsPartiallyInsidePolygon(Polyline polyline, Point topLeft, Point bottomRight)
        {
            // Проверка, находится ли элемент частично внутри полилинии
            var topRight = new Point(bottomRight.X, topLeft.Y);
            var bottomLeft = new Point(topLeft.X, bottomRight.Y);
            return IsPointInPolygon(polyline.Points, topLeft) || IsPointInPolygon(polyline.Points, bottomRight) ||
                   IsPointInPolygon(polyline.Points, topRight) || IsPointInPolygon(polyline.Points, bottomLeft) ||
                   polyline.Points.Any(point => point.X >= topLeft.X && point.X <= bottomRight.X &&
                                                point.Y >= topLeft.Y && point.Y <= bottomRight.Y);
        }

        private bool IsPointInPolygon(PointCollection polygon, Point point)
        {
            int polygonLength = polygon.Count;
            bool inside = false;

            double pointX = point.X, pointY = point.Y;
            for (int i = 0, j = polygonLength - 1; i < polygonLength; j = i++)
            {
                double startX = polygon[i].X, startY = polygon[i].Y;
                double endX = polygon[j].X, endY = polygon[j].Y;

                bool intersect = ((startY > pointY) != (endY > pointY)) &&
                                 (pointX < (endX - startX) * (pointY - startY) / (endY - startY) + startX);
                if (intersect)
                {
                    inside = !inside;
                }
            }

            return inside;
        }
        private Image TrimImageCustom(Image originalImage, Polyline polyline)
        {
            // Убедитесь, что оригинальное изображение не null
            if (originalImage == null || originalImage.Source == null)
                return null;

            // Получаем исходное изображение как BitmapSource
            BitmapSource bitmapSource = originalImage.Source as BitmapSource;

            // Преобразуем изображение в формат Pbgra32 для редактирования
            FormatConvertedBitmap convertedBitmap = new FormatConvertedBitmap();
            convertedBitmap.BeginInit();
            convertedBitmap.Source = bitmapSource;
            convertedBitmap.DestinationFormat = PixelFormats.Pbgra32;
            convertedBitmap.EndInit();

            // Создаем WriteableBitmap для редактирования пикселей
            WriteableBitmap writeableBitmap = new WriteableBitmap(convertedBitmap);

            // Получаем размеры изображения
            int width = writeableBitmap.PixelWidth;
            int height = writeableBitmap.PixelHeight;
            int stride = writeableBitmap.BackBufferStride;

            // Создаем массив для хранения пикселей
            byte[] pixels = new byte[height * stride];

            // Копируем пиксели в массив
            writeableBitmap.CopyPixels(pixels, stride, 0);

            // Создаем геометрию полигона
            StreamGeometry geometry = new StreamGeometry();
            using (StreamGeometryContext ctx = geometry.Open())
            {
                ctx.BeginFigure(polyline.Points[0], true, true);
                ctx.PolyLineTo(polyline.Points.Skip(1).ToArray(), true, true);
            }

            geometry.Freeze();

            // Получаем границы полигона
            Rect bounds = geometry.Bounds;

            // Используем алгоритм сканирующей строки для перекрашивания пикселей внутри полигона
            for (int y = (int)bounds.Top; y < bounds.Bottom; y++)
            {
                if (y < 0 || y >= height) continue;
                for (int x = (int)bounds.Left; x < bounds.Right; x++)
                {
                    if (x < 0 || x >= width) continue;

                    Point p = new Point(x, y);
                    if (geometry.FillContains(p))
                    {
                        int index = (y * stride) + (x * 4);
                        pixels[index] = 255;     // Blue
                        pixels[index + 1] = 255; // Green
                        pixels[index + 2] = 255; // Red
                        pixels[index + 3] = 255; // Alpha
                    }
                }
            }

            // Копируем измененные пиксели обратно в WriteableBitmap
            writeableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);

            // Создаем новое изображение с измененным источником
            Image recoloredImage = new Image
            {
                Source = writeableBitmap,
                Width = originalImage.Width,
                Height = originalImage.Height
            };

            return recoloredImage;
        }

        private Image GetRenderOfCustomCanvas(Polyline polyline)
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            foreach (var point in polyline.Points)
            {
                Point transformedPoint = polyline.TransformToAncestor(_selection.SelectCan).Transform(point);

                if (transformedPoint.X < minX) minX = transformedPoint.X;
                if (transformedPoint.Y < minY) minY = transformedPoint.Y;
                if (transformedPoint.X > maxX) maxX = transformedPoint.X;
                if (transformedPoint.Y > maxY) maxY = transformedPoint.Y;
            }

            double width = maxX - minX;
            double height = maxY - minY;

            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)width, (int)height, 96, 96, PixelFormats.Pbgra32);

            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                PathGeometry pathGeometry = new PathGeometry();
                PathFigure pathFigure = new PathFigure
                {
                    StartPoint = new Point(polyline.Points[0].X - minX, polyline.Points[0].Y - minY),
                    IsClosed = true
                };

                for (int i = 0; i < polyline.Points.Count; i++)
                {
                    Point transformedPoint = polyline.TransformToAncestor(_selection.SelectCan).Transform(polyline.Points[i]);
                    pathFigure.Segments.Add(new LineSegment(new Point(transformedPoint.X - minX, transformedPoint.Y - minY), true));
                }

                pathGeometry.Figures.Add(pathFigure);

                drawingContext.PushClip(pathGeometry);

                VisualBrush visualBrush = new VisualBrush(DrawingCanvas)
                {
                    Stretch = Stretch.None,
                    ViewboxUnits = BrushMappingMode.Absolute,
                    Viewbox = new Rect(minX, minY, width, height)
                };

                drawingContext.DrawRectangle(visualBrush, null, new Rect(new Size(width, height)));
                drawingContext.Pop();
            }

            renderTargetBitmap.Render(drawingVisual);

            Image image = new Image
            {
                Source = renderTargetBitmap,
                Width = width,
                Height = height
            };
            return image;
        }
        private void AddSelectionLineInSelection(Polyline polyline)
        {
            DrawingCanvas.Children.Remove(polyline);

            _selection.SelectCan.Children.Add(polyline);

            double minXS = double.MaxValue;
            double minYS = double.MaxValue;

            foreach (var point in polyline.Points)
            {
                if (point.X < minXS)
                    minXS = point.X;
                if (point.Y < minYS)
                    minYS = point.Y;
            }
            Point firstPoint = polyline.Points[0];

            GeneralTransform transform = polyline.TransformToAncestor(_selection.SelectCan);
            Point canvasPosition = transform.Transform(new Point(0, 0));

            Canvas.SetLeft(polyline, 0 - minXS + 5);
            Canvas.SetTop(polyline, 0 - minYS + 5);
        }
        private bool IfCustomSelectionIsCreated(Polyline polyline)
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            foreach (var point in polyline.Points)
            {
                if (point.X < minX) minX = point.X;
                if (point.Y < minY) minY = point.Y;
                if (point.X > maxX) maxX = point.X;
                if (point.Y > maxY) maxY = point.Y;
            }

            double polylineWidth = maxX - minX;
            double polylineHeight = maxY - minY;

            _selection = new Selection
            {
                Height = polylineHeight + _selectionSizeCorelation,
                Width = polylineWidth + _selectionSizeCorelation
            };
            _selection.SelectionBorder.Height = polylineHeight + _selectionSizeCorelation;
            _selection.SelectionBorder.Width = polylineWidth + _selectionSizeCorelation;

            if (IfSelectionSizeIsZero()) return false;

            double xLoc = Canvas.GetLeft(polyline) + minX - 5;
            double yLoc = Canvas.GetTop(polyline) + minY - 5;

            Canvas.SetLeft(_selection, xLoc);
            Canvas.SetTop(_selection, yLoc);

            DrawingCanvas.Children.Remove(_selectionLine);
            AddBgImageInChildren();

            DrawingCanvas.Children.Add(_selection);
            return true;
        }
        private void SetPolylineLocation(Polyline polyline)
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;

            foreach (var point in polyline.Points)
            {
                if (point.X < minX)
                    minX = point.X;
                if (point.Y < minY)
                    minY = point.Y;
            }
            Point firstPoint = polyline.Points[0];

            GeneralTransform transform = polyline.TransformToAncestor(DrawingCanvas);
            Point canvasPosition = transform.Transform(new Point(0, 0));

            Canvas.SetLeft(polyline, canvasPosition.X - firstPoint.X);
            Canvas.SetTop(polyline, canvasPosition.Y - firstPoint.Y);
        }
        private void MakeRectSelection()
        {
            CreateSelection(_selectionRect);
            IfSelectionIsMaken = true;

            if (IfSelectionSizeIsZero()) return;

            InitSelectedBgInRectCanvas();
        }
        public bool IfSelectionSizeIsZero()
        {
            if (IfSelectionSizeIsNotAcceptable())
            {
                _selection.IfSelectionIsClicked = false;
                _selection = null;
                RemoveSelection();
                return true;
            }
            return false;
        }
        private bool IfSelectionSizeIsNotAcceptable()
        {
            return _selection.SelectionBorder.Width is double.NaN ||
                _selection.SelectionBorder.Height is double.NaN ||
                _selection.SelectionBorder.Width == _selectionSizeCorelation ||
                _selection.SelectionBorder.Height == _selectionSizeCorelation ||
                _selection.Height is double.NaN ||
                _selection.Width is double.NaN ||
                _selection.Height == _selectionSizeCorelation ||
                _selection.Width == _selectionSizeCorelation;
        }
        private void CreateSelection(Shape shape)
        {
            _selection = new Selection()
            {
                Height = shape.Height + _selectionSizeCorelation,
                Width = shape.Width + _selectionSizeCorelation
            };
            _selection.SelectionBorder.Height = shape.Height + _selectionSizeCorelation;
            _selection.SelectionBorder.Width = shape.Width + _selectionSizeCorelation;

            double xLoc = Canvas.GetLeft(shape);
            double yLoc = Canvas.GetTop(shape);

            Canvas.SetLeft(_selection, xLoc - 5);
            Canvas.SetTop(_selection, yLoc - 5);

            DrawingCanvas.Children.Remove(shape);

            AddBgImageInChildren();

            DrawingCanvas.Children.Add(_selection);
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

            //AAA.Content = Math.Abs(((int)temp) - 100).ToString();
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
                ClearDynamicValues(brushClicked: true);
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
            PaintButBordsInClickedColor(PaintingBut);
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
            ClearDynamicValues();

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
        private void AddSavedPath(string path)
        {
            if (_savedPathes.Count != 0 && _savedPathes.First() == path) return;
            _savedPathes.Add(path);
            _lastSaveTime = DateTime.Now;
        }
        private void SaveField_Click(object sender, EventArgs e)
        {
            PngBitmapEncoder pngImage = GetImageOfDrawingCanvas();

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image|*.png";
            saveFileDialog.FileName = "IHateThisTask.png";
            if (saveFileDialog.ShowDialog() == true)
            {
                AddSavedPath(saveFileDialog.FileName);
                using (FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    pngImage.Save(fileStream);
                }
            }
        }
        private PngBitmapEncoder GetImageOfDrawingCanvas()
        {
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)DrawingCanvas.ActualWidth,
            (int)DrawingCanvas.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(DrawingCanvas);

            PngBitmapEncoder pngImage = new PngBitmapEncoder();
            pngImage.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            return pngImage;
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
            ClearDynamicValues();
            if (sender is MenuItem item)
            {
                _selectionType = item.Name == "ZeroSelection" ? SelectionType.Rectangle :
                item.Name == "OneSelection" ? SelectionType.Custom :
                item.Name == "TwoSelection" ? SelectionType.All :
                item.Name == "ThreeSelection" ? SelectionType.Invert :
                item.Name == "FourSelection" ? SelectionType.Transparent :
                item.Name == "FiveSelection" ? SelectionType.Delete : SelectionType.Nothing;

                ChangeSelectionImage();
            }
            _type = ActionType.Selection;
            PaintButBordsInClickedColor(SelectionBut);
        }
        private void ChangeSelectionImage()
        {
            string asd = GetPathToNewSelectionType();
            SelectionImg.Source = BitmapFrame.Create(new Uri(asd));

            return;
            string path = GetSourceForNewSelectionType();
            SelectionImg.Source = BitmapFrame.Create(new Uri(path));
        }
        public string GetSourceForNewSelectionType()
        {
            return _selectionType == SelectionType.Custom ? "Images/Selection/SelectForm.png" :
                 "Images/Selection/Select.png";
        }
        public string GetPathToNewSelectionType()
        {
            string type = _selectionType == SelectionType.Custom ?
                _selectionType.ToString() :
                SelectionType.Rectangle.ToString();

            DirectoryInfo baseDirectoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            string imageDirectory = baseDirectoryInfo.Parent.Parent.FullName;
            string imagePath = System.IO.Path.Combine(imageDirectory, "Images");
            string selectionDir = System.IO.Path.Combine(imagePath, "Selection");
            return System.IO.Path.Combine(selectionDir, $"{type}.png");
        }
        private Point _firstSelectionStart;
        private Point _firstSelectionEnd;
        private void InitSelectedBgInRectCanvas()
        {
            RenderTargetBitmap copy = GetRenderedCopy();

            InsertBitmapToCanvas(copy);

            Point start = new Point(Canvas.GetLeft(_selectionRect), Canvas.GetTop(_selectionRect));
            Point end = new Point(start.X + _selectionRect.Width, start.Y + _selectionRect.Height);

            if (_selectionType == SelectionType.All)
            {
                DrawingCanvas.Children.Clear();
                DrawingCanvas.Background = new SolidColorBrush(Colors.White);
            }
            //Get Bg Image
            //Paint bg in all white
            //add image in children
            //Init got image as bg (delete from children)
            //AddBgImageInChildren();
            DeleteAndTrimElements(start, end);
        }
        private void AddBgImageInChildren()
        {
            Canvas can = GetAuxiliaryCanvas();
            Image img = ConvertCanvasInImage(can);

            ImageBrush bruh = new ImageBrush()
            {
                ImageSource = img.Source
            };
            can.Background = bruh;

            Image bgImg = ConvertBackgroundToImage(can);
            DrawingCanvas.Background = new SolidColorBrush(Colors.White);

            DrawingCanvas.Children.Add(bgImg);
            can.Children.Clear();


            /*            Image bgImg = ConvertBackgroundToImage(DrawingCanvas);
                        DrawingCanvas.Background = new SolidColorBrush(Colors.White);

                        DrawingCanvas.Children.Add(bgImg);*/
        }
        public Image ConvertBackgroundToImage(Canvas canvas)
        {
            // Убедитесь, что canvas не null
            if (canvas == null)
                return null;

            // Создаем RenderTargetBitmap с размерами холста
            RenderTargetBitmap renderTarget = new RenderTargetBitmap(
                (int)canvas.ActualWidth,
                (int)canvas.ActualHeight,
                96,
                96,
                PixelFormats.Pbgra32);

            // Рендерим холст в RenderTargetBitmap
            renderTarget.Render(canvas);

            // Создаем BitmapImage для использования в элементе Image
            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderTarget));
                encoder.Save(memoryStream);
                memoryStream.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }

            // Создаем новый элемент Image и устанавливаем его Source
            Image image = new Image()
            {
                Source = bitmapImage,
                Height = DrawingCanvas.Height,
                Width = DrawingCanvas.Width
            };

            Canvas.SetLeft(image, 0);
            Canvas.SetTop(image, 0);

            image.UpdateLayout();
            var bounds = VisualTreeHelper.GetDescendantBounds(image);

            return image;
        }
        private void DeleteAndTrimElements(Point point1, Point point2)
        {
            double minX = Math.Min(point1.X, point2.X);
            double minY = Math.Min(point1.Y, point2.Y);
            double maxX = Math.Max(point1.X, point2.X);
            double maxY = Math.Max(point1.Y, point2.Y);

            var elementsToRemove = new List<UIElement>();
            var elementsToAdd = new List<UIElement>();

            for (int i = 0; i < DrawingCanvas.Children.Count; i++)
            {
                var bounds = VisualTreeHelper.GetDescendantBounds(DrawingCanvas.Children[i]);

                var topLeft = DrawingCanvas.Children[i].
                    TransformToAncestor(DrawingCanvas).Transform(new Point(0, 0));
                if (DrawingCanvas.Children[i] is Image)
                {
                    ((Image)DrawingCanvas.Children[i]).Width = 1000;
                    ((Image)DrawingCanvas.Children[i]).Height = 400;
                    if (bounds == Rect.Empty)
                    {
                        bounds = new Rect(0, 0, DrawingCanvas.Width, DrawingCanvas.Height);
                    }
                }

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
            for (int i = 0; i < elementsToRemove.Count; i++)
            {
                DrawingCanvas.Children.Remove(elementsToRemove[i]); ;
            }
            for (int i = 0; i < elementsToAdd.Count; i++)
            {
                DrawingCanvas.Children.Add(elementsToAdd[i]);
            }
            DrawingCanvas.Children.Remove(_selection);
            Image img = ConvertCanvasInImage(DrawingCanvas); //ConvertListOfImagesIntoOne(elementsToAdd.OfType<Image>().ToList());
            DrawingCanvas.Children.Add(_selection);
            ImageBrush brush = new ImageBrush()
            {
                ImageSource = img.Source
            };
            DrawingCanvas.Background = brush;
        }
        private List<Image> GetImagesToMakeBg(List<UIElement> els)
        {
            List<Image> res = new List<Image>();

            return res;
        }

        public Image ReplaceTransparentWithWhite(Image originalImage)
        {
            // Убедитесь, что оригинальное изображение не null
            if (originalImage == null || originalImage.Source == null)
                return null;

            // Получаем исходное изображение как BitmapSource
            BitmapSource bitmapSource = originalImage.Source as BitmapSource;

            // Преобразуем изображение в формат Pbgra32 для редактирования
            FormatConvertedBitmap convertedBitmap = new FormatConvertedBitmap();
            convertedBitmap.BeginInit();
            convertedBitmap.Source = bitmapSource;
            convertedBitmap.DestinationFormat = PixelFormats.Pbgra32;
            convertedBitmap.EndInit();

            // Создаем WriteableBitmap для редактирования пикселей
            WriteableBitmap writeableBitmap = new WriteableBitmap(convertedBitmap);

            // Получаем размеры изображения
            int width = writeableBitmap.PixelWidth;
            int height = writeableBitmap.PixelHeight;
            int stride = writeableBitmap.BackBufferStride;

            // Создаем массив для хранения пикселей
            byte[] pixels = new byte[height * stride];

            // Копируем пиксели в массив
            writeableBitmap.CopyPixels(pixels, stride, 0);

            // Заменяем прозрачные пиксели на белые
            for (int i = 0; i < pixels.Length; i += 4)
            {
                if (pixels[i + 3] == 0) // Проверка прозрачности (Alpha == 0)
                {
                    pixels[i] = 255;     // Blue
                    pixels[i + 1] = 255; // Green
                    pixels[i + 2] = 255; // Red
                    pixels[i + 3] = 255; // Alpha
                }
            }

            // Копируем измененные пиксели обратно в WriteableBitmap
            writeableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);

            // Создаем новое изображение с измененным источником
            Image recoloredImage = new Image
            {
                Source = writeableBitmap,
                Width = originalImage.Width,
                Height = originalImage.Height
            };

            return recoloredImage;
        }
        private Image PaintImageInWhite(Image originalImage)
        {
            // Убедитесь, что оригинальное изображение не null
            if (originalImage == null || originalImage.Source == null)
                return null;

            // Получаем исходное изображение как BitmapSource
            BitmapSource bitmapSource = originalImage.Source as BitmapSource;

            // Преобразуем изображение в формат Pbgra32 для редактирования
            FormatConvertedBitmap convertedBitmap = new FormatConvertedBitmap();
            convertedBitmap.BeginInit();
            convertedBitmap.Source = bitmapSource;
            convertedBitmap.DestinationFormat = PixelFormats.Pbgra32;
            convertedBitmap.EndInit();

            // Создаем WriteableBitmap для редактирования пикселей
            WriteableBitmap writeableBitmap = new WriteableBitmap(convertedBitmap);

            // Получаем размеры изображения
            int width = writeableBitmap.PixelWidth;
            int height = writeableBitmap.PixelHeight;
            int stride = writeableBitmap.BackBufferStride;

            // Создаем массив для хранения пикселей
            byte[] pixels = new byte[height * stride];

            // Копируем пиксели в массив
            writeableBitmap.CopyPixels(pixels, stride, 0);

            // Перекрашиваем все пиксели в белый цвет
            for (int i = 0; i < pixels.Length; i += 4)
            {
                pixels[i] = 255;     // Blue
                pixels[i + 1] = 255; // Green
                pixels[i + 2] = 255; // Red
                pixels[i + 3] = 255; // Alpha
            }

            // Копируем измененные пиксели обратно в WriteableBitmap
            writeableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);

            // Создаем новое изображение с измененным источником
            Image recoloredImage = new Image
            {
                Source = writeableBitmap,
                Width = originalImage.Width,
                Height = originalImage.Height
            };

            return recoloredImage;

        }
        private Image ConvertListOfImagesIntoOne(List<Image> images)
        {
            Canvas can = new Canvas()
            {
                Height = DrawingCanvas.Height,
                Width = DrawingCanvas.Width
            };

            for (int i = 0; i < images.Count; i++)
            {
                can.Children.Add(images[i]);
            }

            return ConvertCanvasInImage(can);
        }
        private List<Image> TrimImage(Image image, double minX, double minY, double maxX, double maxY, Point topLeft)
        {
            var bitmap = image.Source as BitmapSource;
            if (bitmap == null) return null;

            var trimmedImages = new List<Image>();

            var areas = new List<Int32Rect>
            {
                new Int32Rect(0, 0, (int)Math.Max(0, minX - topLeft.X), bitmap.PixelHeight),
                new Int32Rect((int)Math.Min(bitmap.PixelWidth, maxX - topLeft.X), 0, (int)Math.Max(0, bitmap.PixelWidth - (maxX - topLeft.X)), bitmap.PixelHeight),
                new Int32Rect(0, 0, bitmap.PixelWidth, (int)Math.Max(0, minY - topLeft.Y)),
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

                    double offsetX = area.X == 0 ? topLeft.X : topLeft.X + area.X;
                    double offsetY = area.Y == 0 ? topLeft.Y : topLeft.Y + area.Y;

                    Canvas.SetLeft(newImage, offsetX);
                    Canvas.SetTop(newImage, offsetY);

                    trimmedImages.Add(newImage);

                    newImage = PaintImageInWhite(newImage);
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

        /* private void ConvertRegionToWhite()
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
         }*/
        private void InsertBitmapToCanvas(RenderTargetBitmap bitmap)
        {
            var image = new Image
            {
                Source = bitmap,
                Width = _selection.Width,
                Height = _selection.Height
            };
            /*            image.CaptureMouse();
                        Canvas.SetZIndex(image, 100);*/

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
            else if (!(_lineSizeing is null))
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

                UseAuxiliaryCanvasToCollectChildrenInDrawingCanvas();
                return;
            }
            if (!(_figureSizing is null) && !_figureSizing.IfSelectionIsClicked)
            {
                ClearFigureSizing();
                UseAuxiliaryCanvasToCollectChildrenInDrawingCanvas();
                return;
            }
            else if (!(_lineSizeing is null) && !_lineSizeing.IfSelectionIsClicked)
            {
                ClearLineSizing();
                UseAuxiliaryCanvasToCollectChildrenInDrawingCanvas();
                return;
            }
            CheckForTextSizeChanging();
            if (!(_changedSizeText is null) && _changedSizeText.IfSelectiongClicked)
            {
                _changedSizeText.IfSelectiongClicked = false;
            }
        }
        private void CheckForTextSizeChanging()
        {
            if (!(_changedSizeText is null) &&
                !_changedSizeText.IfSelectiongClicked && _ifDoubleClicked)
            {
                ConvertRichTextBoxIntoImage();
                _ifDoubleClicked = false;
                DrawingCanvas.Children.RemoveAt((int)_changedSizeText.Tag);
                _changedSizeText = null;
                UseAuxiliaryCanvasToCollectChildrenInDrawingCanvas();
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
            ClearSquaresBorders();
        }
        private void ClearSquaresBorders()
        {
            ClearBigSquares(SelectionBut);
            ClearBigSquares(PaintingBut);
        }
        private void ClearBigSquares(Button button)
        {
            button.BorderThickness = new Thickness(0);
            button.BorderBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)); ;
        }
        private void InitLineInCanvas()
        {
            Line lineToAdd = _lineSizeing.GetLineObject();

            Point linePoistion = GetCordsForLineImage(lineToAdd, new Point(0, 0));
            List<Point> points = new List<Point>
            {
                GetCordsForLineImage(lineToAdd, new Point(lineToAdd.X1, lineToAdd.Y1)),
                GetCordsForLineImage(lineToAdd, new Point(lineToAdd.X2, lineToAdd.Y2))
            };

            _lineSizeing.RemoveLineFromCanvas();

            Image img = ConvertLineToImage(lineToAdd);
            Canvas.SetLeft(img, Math.Min(points[0].X, points[1].X));
            Canvas.SetTop(img, Math.Min(points[0].Y, points[1].Y));

            if (lineToAdd is null) return;

            DrawingCanvas.Children.Add(img);
        }
        private Image ConvertLineToImage(Line line)
        {
            Canvas canvas = CreateCanvasWithLine(line);

            return ConvertCanvasInImage(canvas);
        }
        private Image ConvertCanvasInImage(Canvas canvas)
        {
            RenderTargetBitmap renderBitmap = ConvertCanvasToBitmap(canvas);

            Image image = new Image
            {
                Source = renderBitmap,
                Width = canvas.Width,
                Height = canvas.Height
            };
            return image;
        }
        private Canvas CreateCanvasWithLine(Line line)
        {
            double minX = Math.Min(line.X1, line.X2);
            double minY = Math.Min(line.Y1, line.Y2);
            double maxX = Math.Max(line.X1, line.X2);
            double maxY = Math.Max(line.Y1, line.Y2);

            Canvas canvas = new Canvas
            {
                Width = maxX - minX,
                Height = maxY - minY
            };

            TranslateTransform transform = new TranslateTransform(-minX, -minY);
            line.RenderTransform = transform;

            canvas.Children.Add(line);
            return canvas;
        }
        public Point GetCordsForLineImage(Line line, Point point)
        {
            GeneralTransform transform = line.TransformToAncestor(DrawingCanvas);
            Point canvasPosition = transform.Transform(point);

            return canvasPosition;
        }

        private void RemoveLineSizing()
        {
            for (int i = 0; i < DrawingCanvas.Children.Count; i++)
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
            DrawingCanvas.Children.Remove(_selectionLine);
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
            _richTexBox.Document.Blocks.Add(new Paragraph(new Run("Start sentace")));
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

            //if(_ifDrawing) ConvertPaintingInImage();

            if (!(_lineSizeing is null))
            {
                _lineSizeing._moveRect = LineSizingRectType.Nothing;
                _lineSizeing._isDraggingSelection = false;
            }
        }
        private void ConvertPaintingInImage()
        {
            if (DrawingCanvas.Children.Count == 0) return;
            var lastAddedObj = DrawingCanvas.Children[DrawingCanvas.Children.Count - 1];

            if (lastAddedObj is Line)
            {
                SwapLinesWithImages();
            }
            else if (lastAddedObj is Polygon)
            {
                SwapPolygonWithImage();
            }
            else if (lastAddedObj is Polyline)
            {
                SwapPolylineWIthImage();
            }
            UseAuxiliaryCanvasToCollectChildrenInDrawingCanvas();
        }
        private void SwapPolylineWIthImage()
        {
            List<Shape> res = GetAllPolylines();

            RemoveAllPolylinesFromDrawingCanvas();

            Image img = ConvertShapesToImage(res, DrawingCanvas.Width, DrawingCanvas.Height);

            DrawingCanvas.Children.Add(img);
        }
        private void RemoveAllPolylinesFromDrawingCanvas()
        {
            for (int i = 0; i < DrawingCanvas.Children.Count; i++)
            {
                if (DrawingCanvas.Children[i] is Polyline)
                {
                    DrawingCanvas.Children.RemoveAt(i);
                    if (i != 0) i--;
                    else i = -1;
                }
            }
        }
        private List<Shape> GetAllPolylines()
        {
            List<Shape> res = new List<Shape>();
            for (int i = 0; i < DrawingCanvas.Children.Count; i++)
            {
                if (DrawingCanvas.Children[i] is Polyline)
                {
                    res.Add((Polyline)DrawingCanvas.Children[i]);
                }
            }
            return res;
        }
        private void SwapPolygonWithImage()
        {
            List<Shape> res = GetAllPolygonsInDrawingImage();

            RemoveAllPolygonsFromDrawingCanvas();

            Image img = ConvertShapesToImage(res, DrawingCanvas.Width, DrawingCanvas.Height);

            DrawingCanvas.Children.Add(img);
        }
        private List<Shape> GetAllPolygonsInDrawingImage()
        {
            List<Shape> res = new List<Shape>();
            for (int i = 0; i < DrawingCanvas.Children.Count; i++)
            {
                if (DrawingCanvas.Children[i] is Polygon)
                {
                    res.Add((Polygon)DrawingCanvas.Children[i]);
                }
            }
            return res;
        }
        private void RemoveAllPolygonsFromDrawingCanvas()
        {
            for (int i = 0; i < DrawingCanvas.Children.Count; i++)
            {
                if (DrawingCanvas.Children[i] is Polygon)
                {
                    DrawingCanvas.Children.RemoveAt(i);
                    if (i != 0) i--;
                    else i = -1;
                }
            }
        }
        private void SwapLinesWithImages()
        {
            //Get all lines
            List<Shape> lines = GetAllDrawChildrenInLineType();

            //delete all lines
            DeleteAllLines();

            //init all lines in image and
            //add it to drawing canvas 
            Image img = ConvertShapesToImage(lines, DrawingCanvas.Width, DrawingCanvas.Height);

            DrawingCanvas.Children.Add(img);
        }
        private void ConnectAllChildrenInOneImageInCanvas(Canvas canvas)
        {
            Image image = ConvertCanvasInImage(canvas);
            Canvas.SetLeft(image, 0);
            Canvas.SetTop(image, 0);

            canvas.Children.Clear();
            canvas.Children.Add(image);
        }
        private void UseAuxiliaryCanvasToCollectChildrenInDrawingCanvas()
        {
            Canvas can = GetAuxiliaryCanvas();

            Image res = ConvertCanvasInImage(can);
            //DrawingCanvas.Children.Add(res);

            ASD(res, can);
        }
        private Canvas GetAuxiliaryCanvas()
        {
            Canvas can = new Canvas()
            {
                Height = DrawingCanvas.Height,
                Width = DrawingCanvas.Width,
                Background = DrawingCanvas.Background
            };
            List<UIElement> DrawingCanChildren = ReAssginChildrenInAuxiliaryCanvas();
            DrawingCanvas.Children.Clear();

            for (int i = 0; i < DrawingCanChildren.Count; i++)
            {
                can.Children.Add(DrawingCanChildren[i]);
            }

            return can;
        }
        private void ASD(Image res, Canvas can)
        {
            // Получаем текущее содержимое DrawingCanvas
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)can.Width, (int)can.Height, 96, 96, PixelFormats.Pbgra32);
            can.Measure(new Size((int)can.Width, (int)can.Height));
            can.Arrange(new Rect(new Size((int)can.Width, (int)can.Height)));
            rtb.Render(can);

            // Создаем ImageSource из текущего содержимого
            ImageSource currentImageSource = rtb;

            // Создаем DrawingGroup и добавляем текущее содержимое и новое изображение
            DrawingGroup drawingGroup = new DrawingGroup();
            drawingGroup.Children.Add(new ImageDrawing(currentImageSource, new Rect(0, 0, can.Width, can.Height)));
            drawingGroup.Children.Add(new ImageDrawing(res.Source, new Rect(0, 0, can.Width, can.Height)));

            // Создаем новый ImageBrush с объединенным содержимым
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new DrawingImage(drawingGroup);

            // Устанавливаем новый фон для DrawingCanvas
            DrawingCanvas.Background = imageBrush;
        }

        private List<UIElement> ReAssginChildrenInAuxiliaryCanvas()
        {
            List<UIElement> res = new List<UIElement>();
            for (int i = 0; i < DrawingCanvas.Children.Count; i++)
            {
                res.Add(DrawingCanvas.Children[i]);
            }
            return res;
        }

        private void DeleteAllLines()
        {
            for (int i = 0; i < DrawingCanvas.Children.Count; i++)
            {
                if (DrawingCanvas.Children[i] is Line)
                {
                    DrawingCanvas.Children.RemoveAt(i);
                    if (i != 0) i--;
                    else i = -1;
                }
            }
        }
        private List<Shape> GetAllDrawChildrenInLineType()
        {
            List<Shape> res = new List<Shape>();

            for (int i = 0; i < DrawingCanvas.Children.Count; i++)
            {
                if (DrawingCanvas.Children[i] is Line)
                {
                    res.Add((Line)DrawingCanvas.Children[i]);
                }
            }
            return res;
        }
        public Image ConvertShapesToImage(List<Shape> shapes, double width, double height)
        {
            Canvas canvas = new Canvas();

            //(width, height) = GetBoundingBox(shapes);

            canvas.Width = width;
            canvas.Height = height;
            canvas.Background = Brushes.Transparent;

            for (int i = 0; i < shapes.Count; i++)
            {
                canvas.Children.Add(shapes[i]);
            }
            canvas.Measure(new Size(width, height));
            canvas.Arrange(new Rect(new Size(width, height)));

            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                (int)width, (int)height, 96d, 96d, PixelFormats.Pbgra32);

            renderBitmap.Render(canvas);

            Image image = new Image();
            image.Source = renderBitmap;
            image.Width = width;
            image.Height = height;

            return image;
        }
        public (double, double) GetBoundingBox(List<Shape> shapes)
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            foreach (Shape shape in shapes)
            {
                double shapeMinX = Canvas.GetLeft(shape);
                double shapeMinY = Canvas.GetTop(shape);
                double shapeMaxX = shapeMinX + shape.ActualWidth;
                double shapeMaxY = shapeMinY + shape.ActualHeight;

                minX = Math.Min(minX, shapeMinX);
                minY = Math.Min(minY, shapeMinY);
                maxX = Math.Max(maxX, shapeMaxX);
                maxY = Math.Max(maxY, shapeMaxY);
            }

            double width = maxX - minX;
            double height = maxY - minY;

            return (width, height);
            //return new Rect(minX, minY, width, height);
        }
        private Point GetShapeLocation(Shape shape)
        {
            if (shape is Polyline polyline)
            {
                return polyline.Points.First();
            }
            return new Point(0, 0);
        }

        private void ChooseAllSelection_MouseLeftButtonDown(object sender, EventArgs e)
        {
            _selectionType = SelectionType.All;
            _selection = new Selection()
            {
                Height = DrawingCanvas.Height,
                Width = DrawingCanvas.Width
            };
            _selection.SelectionBorder.Height = DrawingCanvas.Height;
            _selection.SelectionBorder.Width = DrawingCanvas.Width;

            Canvas.SetTop(_selection, 0);
            Canvas.SetLeft(_selection, 0);

            _firstSelectionStart = new Point(0, 0);
            _firstSelectionEnd = new Point(DrawingCanvas.Width, DrawingCanvas.Height);

            _selectionRect.Height = DrawingCanvas.Height;
            _selectionRect.Width = DrawingCanvas.Width;

            Canvas.SetLeft(_selectionRect, 0);
            Canvas.SetTop(_selectionRect, 0);

            //init every item in _slection
            //Delter them from drawing canvas
            InitSelectedBgInRectCanvas();

            //DrawingCanvas.Children.Add(_selection);
        }
        private void SelectionMenu_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            EndWithPolygonFigures();
            ClearFigureSizingInClicks();
        }

        private void TurnClickedObjInBlue_MouseDown(object sender, MouseEventArgs e)
        {
            ClearDynamicValues(brushClicked: true);
            if (sender is Button but)
            {
                PaintButBordsInClickedColor(but);
                if (but.Name == "PaintingBut")
                {
                    _type = ActionType.Drawing;
                }
                else
                {
                    _type = ActionType.Selection;
                }
            }
        }
        private void PaintButBordsInClickedColor(Button but)
        {
            but.BorderThickness = new Thickness(1);
            but.BorderBrush = _clickedBorderColor;
        }

        private void ClearDynamicValues(bool brushClicked = false, bool textClicked = false)
        {
            EndWithPolygonFigures();
            ClearFigureSizingInClicks();
            ClearBGs();
            CheckForTextSizeChanging();

            if (brushClicked) ValueBorder.Visibility = Visibility.Visible;
            else ValueBorder.Visibility = Visibility.Hidden;
            if (textClicked) _text.Visibility = Visibility.Visible;
            else _text.Visibility = Visibility.Collapsed;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
        private void MakeBoOfWorkingTable_Click(object sender, EventArgs e)
        {
            const int SPI_SETDESKWALLPAPER = 20;
            const int SPIF_UPDATEINFILE = 1;
            const int SPIF_SENDCHANGE = 2;
            const string path = "B:\\GitHub\\PaintWPF\\toSaveFonts\\font.png";

            Image image = ConvertCanvasToImage(DrawingCanvas);
            SaveImageToDisk(image.Source, path);

            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINFILE | SPIF_SENDCHANGE);
            MessageBox.Show("Done!", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public string SaveImageToDisk(ImageSource imageSource, string filePath)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imageSource));

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                encoder.Save(fileStream);
            }

            return filePath;
        }
        private Image ConvertCanvasToImage(Canvas canvas)
        {
            RenderTargetBitmap bitmap = ConvertCanvasToBitmap(canvas);

            Image image = new Image
            {
                Source = bitmap,
                Width = canvas.Width,
                Height = canvas.Height
            };
            return image;
        }

        private RenderTargetBitmap ConvertCanvasToBitmap(Canvas canvas)
        {
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                (int)canvas.Width, (int)canvas.Height,
                96, 96, PixelFormats.Pbgra32);

            canvas.Measure(new Size((int)canvas.Width, (int)canvas.Height));
            canvas.Arrange(new Rect(new Size((int)canvas.Width, (int)canvas.Height)));
            renderBitmap.Render(canvas);

            return renderBitmap;
        }

        private void Settigns_Click(object sender, EventArgs e)
        {
            ImageSettings settigns = new ImageSettings(DrawingCanvas,
                _savedPathes.Count == 0 ? string.Empty : _savedPathes.First(), _lastSaveTime);
            settigns.ShowDialog();

        }
    }
}

