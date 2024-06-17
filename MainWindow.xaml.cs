using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public MainWindow()
        {
            InitializeComponent();

            InitToolButsInList();
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
        private void Tools_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button but)
            {
                if (!(_chosenTool is null) && but.Name == _chosenTool.Name) return;
                SolidColorBrush brush = new SolidColorBrush(Color.FromRgb(240, 240, 240));
                but.Background = brush;

                SolidColorBrush borderBrush = new SolidColorBrush(Color.FromRgb(209, 209, 209));
                but.BorderBrush = borderBrush;
            }
        }
        private void Tools_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button but)
            {
                if (!(_chosenTool is null) && but.Name == _chosenTool.Name) return;
                SolidColorBrush whiteBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                but.Background = whiteBrush;
                but.BorderBrush = whiteBrush;
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

                if(but.Name == "Pen")
                {
                    isEraser = false;
                    isFilling = false;
                }
                else if(but.Name == "Erazer")
                {
                    isEraser = true;
                    isFilling = false;
                }
                else if(but.Name == "Bucket")
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
                    Stroke = isEraser ? Brushes.White : Brushes.Black,
                    StrokeThickness = isEraser ? 10 : 2,
                    X1 = previousPoint.X,
                    Y1 = previousPoint.Y,
                    X2 = currentPoint.X,
                    Y2 = currentPoint.Y
                };

                DrawingCanvas.Children.Add(line);
                previousPoint = currentPoint;
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
            Color asd =  Color.FromRgb(pixels[index + 2], pixels[index + 1], pixels[index]);
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




    }
}
