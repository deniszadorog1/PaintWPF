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


using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;
using System.IO;
using System.Windows.Threading;

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

        private bool isDrawing = false;
        private bool isEraser = false;
        private bool isFilling = false;
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
            if(sender is Grid grid)
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
                but.BorderBrush = transparentBrush;
                return;
            }
            if (sender is Border bord)
            {
                bord.Background = transparentBrush;
                bord.BorderBrush = transparentBrush;
                return;
            }
            if(sender is Grid grid)
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
                ClearBGForTools();
                _chosenTool = but;

                SolidColorBrush border = new SolidColorBrush(Color.FromRgb(0, 103, 192));
                but.BorderBrush = border;

                SolidColorBrush brush = new SolidColorBrush(Color.FromRgb(240, 240, 240));
                but.Background = brush;

                if (but.Name == "Pen")
                {
                    isEraser = false;
                    isFilling = false;
                }
                else if (but.Name == "Erazer")
                {
                    isEraser = true;
                    isFilling = false;
                }
                else if (but.Name == "Bucket")
                {
                    isFilling = true;
                    isEraser = false;
                }
            }
        }
        public void ClearBGForTools()
        {
            SolidColorBrush whiteColor = new SolidColorBrush(Color.FromRgb(255, 255, 255));

            for (int i = 0; i < _tools.Count; i++)
            {
                _tools[i].Background = whiteColor;
                _tools[i].BorderBrush = whiteColor;
            }
        }
        public void PaintField_MouseLeave(object sender, MouseEventArgs e)
        {
            CursCords.Content = "";
        }
        private void Field_MouseDown(object sender, MouseEventArgs e)
        {
            previousPoint = e.GetPosition(DrawingCanvas);

            if (isFilling)
            {
                Color color = (Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF000000");

                Point point = e.GetPosition(DrawingCanvas);
                FloodFill((int)point.X, (int)point.Y, Color.FromRgb(255, 0, 0), color);
            }
            else
            {
                isDrawing = true;
                previousPoint = e.GetPosition(DrawingCanvas);
                SetPaintingMarker(e);
            }
        }

        private const double CalligraphyBrushAngle = 135 * Math.PI / 180;
        private const double FountainBrushAngle = 45 * Math.PI / 180;

        private void Field_MouseMove(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition(DrawingCanvas);
            string cursPosInPaintField = $"{position.X}, {position.Y}";

            CursCords.Content = cursPosInPaintField;

            if (isDrawing)
            {
                currentPoint = e.GetPosition(DrawingCanvas);
                if (_main.TempBrushType == BrushType.UsualBrush)
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
                    Fill = new SolidColorBrush(Color.FromArgb((byte)(random.Next(50, 255)),
                                _main.FirstColor.TColor.R, _main.FirstColor.TColor.G, _main.FirstColor.TColor.B))
                };

                double x = point.X + offsetX;
                double y = point.Y + offsetY;
                Canvas.SetLeft(ellipse, x);
                Canvas.SetTop(ellipse, y);

                DrawingCanvas.Children.Add(ellipse);
                //drawnElements.Add(ellipse); // Сохранение элемента для возможности отмены
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
                Stroke = new SolidColorBrush(_main.FirstColor.TColor),
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
            SolidColorBrush brush = new SolidColorBrush(_main.FirstColor.TColor);

            int pointsCount = (int)(brushThickness * 2);
            for (int i = 0; i < pointsCount; i++)
            {
                double offsetX = random.NextDouble() * brushThickness - brushThickness / 2;
                double offsetY = random.NextDouble() * brushThickness - brushThickness / 2;

                Ellipse ellipse = new Ellipse
                {
                    Width = random.NextDouble() * (brushThickness / 2),
                    Height = random.NextDouble() * (brushThickness / 2),
                    Fill = new SolidColorBrush(Color.FromArgb((byte)(random.Next(50, 150)),
                            _main.FirstColor.TColor.R, _main.FirstColor.TColor.G, _main.FirstColor.TColor.B))
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

                Stroke = isEraser ? Brushes.White : Brushes.Black,
                StrokeThickness = brushThickness,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeLineJoin = PenLineJoin.Round,
            };
            DrawingCanvas.Children.Add(line);
        }
        public Brush ConvertColorIntoBrushes()
        {
            return new SolidColorBrush(_main.FirstColor.TColor);
        }

        private void Paint_MouseUp(object sender, MouseEventArgs e)
        {
            isDrawing = false;
            sprayTimer.Stop();

            SaveCanvasState();
        }

        private WriteableBitmap bitmap = new WriteableBitmap(1000, 400, 96, 96, PixelFormats.Bgra32, null);
        private byte[] pixels = new byte[1000 * 400 * 4]; // 400x400 image with 4 bytes per pixel (BGR32 format)

        private void FloodFill(int x, int y, Color newColor, Color targetColor)
        {
            if (x < 0 || x >= DrawingCanvas.Width || y < 0 || y >= DrawingCanvas.Height || GetPixelColor(x, y) != targetColor)
                return;

            SetPixelColor(x, y, newColor);
            DrawImage();

            FloodFill(x - 1, y, newColor, targetColor);
            FloodFill(x + 1, y, newColor, targetColor);
            FloodFill(x, y - 1, newColor, targetColor);
            FloodFill(x, y + 1, newColor, targetColor);
        }
        private void DrawImage()
        {
            Int32Rect rect = new Int32Rect(0, 0, 1000, 400);
            bitmap.WritePixels(rect, pixels, 400 * 4, 0);
            System.Windows.Controls.Image img = new System.Windows.Controls.Image
            {
                Source = bitmap
            };

            DrawingCanvas.Children.Clear();

            DrawingCanvas.Children.Add(img);
        }

        private Color GetPixelColor(int x, int y)
        {
            int index = (y * 1000 + x) * 4;
            Color asd = Color.FromRgb(pixels[index + 2], pixels[index + 1], pixels[index]);
            return asd;
        }

        private void SetPixelColor(int x, int y, Color color)
        {
            int index = (y * 400 + x) * 4;
            pixels[index] = color.B;
            pixels[index + 1] = color.G;
            pixels[index + 2] = color.R;
            pixels[index + 3] = color.A;
        }
        private void ValueCanvas_PreViewMouseUp(object sender, MouseEventArgs e)
        {
            valueDragElem = null;
            ValueCanvas.ReleaseMouseCapture();
        }
        private void ValueCanvas_PreViewMouseMove(object sender, MouseEventArgs e)
        {
            Point buttonPosRelativeToAncestor = draggableButton.TransformToAncestor(this).Transform(new Point(0, 0));

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
            valueOffset = new Point((int)centerX, (int)centerY); //e.GetPosition(ValueCanvas);

            valueOffset.Y -= Canvas.GetTop(valueDragElem);
            //valueOffset.X -= Canvas.GetLeft(valueDragElem);
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
                // Очищаем Canvas
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
        private int currentIndex = -1; // текущий индекс истории

        public void SaveCanvasState()
        {

            Size size = new Size(DrawingCanvas.ActualWidth, DrawingCanvas.ActualHeight);

            // Замеры и размещение канвы
            DrawingCanvas.Measure(size);
            DrawingCanvas.Arrange(new Rect(size));

            // Создание RenderTargetBitmap из текущего состояния Canvas
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
    }

}
