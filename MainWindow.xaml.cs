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

using PaintWPF.Models;
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
        private Button _chosenTool = null;

        private bool isDrawing = false;
        private bool isEraser = false;
        private bool isFilling = false;
        private Point previousPoint;
        private int brushThickness = 2;
        
        private bool IfShowBrushSize = false;
        
        private UIElement valueDragElem = null;
        private Point valueOffset;

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


            CanvasSize.Content = $"{DrawingCanvas.Width} x {DrawingCanvas.Height}";
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
        private void Button_Click(object sender, RoutedEventArgs e)
        {

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
            }

        }
        private void MainPabelBoxes_MouseLeave(object sender, MouseEventArgs e)
        {
            SolidColorBrush whiteBrush = new SolidColorBrush(Color.FromRgb(248, 249, 252));
            if (sender is Button but)
            {
                if (!(_chosenTool is null) && but.Name == _chosenTool.Name) return;
                but.Background = whiteBrush;
                but.BorderBrush = whiteBrush;
            }
            if (sender is Border bord)
            {
                bord.Background = whiteBrush;

                bord.BorderBrush = whiteBrush;
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
            prevPoint = e.GetPosition(DrawingCanvas);

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
            }
        }
        private Point prevPoint;

        private void Field_MouseMove(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition(DrawingCanvas);
            string cursPosInPaintField = $"{position.X}, {position.Y}";

            CursCords.Content = cursPosInPaintField;

            if (isDrawing)
            {
                Point currentPoint = e.GetPosition(DrawingCanvas);
                Line line = new Line
                {
                    X1 = prevPoint.X,
                    Y1 = prevPoint.Y,
                    X2 = currentPoint.X,
                    Y2 = currentPoint.Y,

                    Stroke = isEraser ? Brushes.White : Brushes.Black,
                    StrokeThickness = brushThickness,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Round,
                    StrokeLineJoin = PenLineJoin.Round
                };
                DrawingCanvas.Children.Add(line);
                prevPoint = currentPoint;
            }
        }
        private void Paint_MouseUp(object sender, MouseEventArgs e)
        {
            isDrawing = false;
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
    }
}
