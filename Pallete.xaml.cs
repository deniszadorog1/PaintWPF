using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
using System.Windows.Shapes;

using PaintWPF.Models;
using PaintWPF.Models.Tools;

using Brushes = System.Windows.Media.Brushes;
using Color = System.Drawing.Color;
using Point = System.Windows.Point;

namespace PaintWPF
{
    /// <summary>
    /// Логика взаимодействия для Pallete.xaml
    /// </summary>
    public partial class Pallete : Window
    {
        private PalleteModel _pallete;
        private UIElement[,] UserColors;
        private (int, int) chosenCustomColor = (-1, -1);

        private System.Windows.Media.Color _chosenColor;
        public Pallete(PalleteModel pallete)
        {
            _pallete = pallete;
            InitializeComponent();

            InitBasicParamsAfterInitComps();
        }

        public void InitBasicParamsAfterInitComps()
        {
            Canvas.SetTop(draggableButton, 20);
            Canvas.SetLeft(draggableButton, 20);

            Canvas.SetTop(ValueBut, 1);
            Canvas.SetLeft(ValueBut, 1);

            InitUserColorsInArray();
        }
        public void InitUserColorsInArray()
        {
            UserColors = new UIElement[_pallete.UserColors.GetLength(0),
                _pallete.UserColors.GetLength(1)];
            int tempChildrenIndex = 0;
            for (int i = 0; i < UserColors.GetLength(0); i++)
            {
                for (int j = 0; j < UserColors.GetLength(1); j++)
                {
                    UserColors[i, j] = UserColorPanel.Children[tempChildrenIndex];
                    if (UserColorPanel.Children[tempChildrenIndex] is Button)
                    {
                        Button but = (Button)UserColorPanel.Children[tempChildrenIndex];
                        but.Tag = new Tuple<int, int>(i, j);
                    }
                    tempChildrenIndex++;
                    if (tempChildrenIndex >= UserColorPanel.Children.Count)
                    {
                        return;
                    }
                }
            }
        }
        private void RGBTextBox_TextChanged(object sender, EventArgs e)
        {
            const int maxAmountOfSymbols = 8;
            if (sender is TextBox box)
            {
                if (box.Text.Length == 0)
                {
                    box.Text = "#";
                    box.CaretIndex = box.Text.Length;
                }
                if (box.Text.Length != 0 && box.Text.First() != '#')
                {
                    box.Text = box.Text.Remove(0, 1);
                    if (!box.Text.Contains("#"))
                    {
                        box.Text = box.Text.Insert(0, "#");
                    }
                    box.CaretIndex = box.Text.Length;
                }
                if (box.Text.Length == maxAmountOfSymbols)
                {
                    box.Text = box.Text.Remove(box.Text.Length - 1);
                    box.CaretIndex = box.Text.Length;
                    return;
                }
            }
        }
        private void MyButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button)
            {
                Button button = sender as Button;
                AddEnterIngBorder(button);
            }
        }
        private void MyButton_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button)
            {
                Button button = sender as Button;
                ClearEnterColor(button);
            }
        }
        private void MainColor_Click(object sender, EventArgs e)
        {
            Button but = sender as Button;

            //Get color in media_Color
            System.Windows.Media.Color? color = null;
            if (but.Background is SolidColorBrush brush)
            {
                color = brush.Color;
            }
            if (color is null) return;

            //Get this in pallete [,] index 
            (int y, int x) mainColorIndex =
                _pallete.GetMainColorIndexByColor((System.Windows.Media.Color)color);
            if (mainColorIndex.y == -1 && mainColorIndex.x == -1) return;

            // get point int specImg
/*            System.Windows.Media.Color col =
                (System.Windows.Media.Color)color;*/


            Color col = ShowColorOnSpecForMainColor(mainColorIndex);

            

            (int, int)? asd = GetColorCord(col.R, col.G, col.B);
            if (asd is null) return;

            // int boxes right boxes 
            InitCordInSpec(asd.Value.Item1, asd.Value.Item2);

            specDragEl = null;
            SpecCanvas.ReleaseMouseCapture();
        }
        public Color ShowColorOnSpecForMainColor((int, int) mainColorInPalleteIndex)
        {
            (int h, int s, int v) hcvParam= 
                _pallete.MainColors[mainColorInPalleteIndex.Item1, 
                mainColorInPalleteIndex.Item2].HSVParam;

            Color first = ColorConvertor.hsvToRgb(hcvParam.h, hcvParam.s, hcvParam.v);

            Color color = ColorConvertor.HSVToRGB(hcvParam.h, hcvParam.s, hcvParam.v);

            return new Color();
        }
        public string GetHexFromColor(int r, int g, int b)
        {
            Color myColor = Color.FromArgb(r, g, b);
            string hex = "#" + myColor.R.ToString("X2") + myColor.G.ToString("X2") + myColor.B.ToString("X2");
            return hex;
        }
        public void InitCordInSpec(int y, int x)
        {
            this.specDragEl = draggableButton as UIElement;

            Canvas.SetTop(specDragEl, y);
            Canvas.SetLeft(specDragEl, x);
        }
        public (int, int)? GetColorCord(int r, int g, int b)
        {
            Color targetColor = Color.FromArgb(r, g, b);

            string imgPath = GetDirToPallete();

            using (Bitmap bitmap = new Bitmap(imgPath))
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        Color pixelColor = bitmap.GetPixel(x, y);
                        if (pixelColor.ToArgb() == targetColor.ToArgb())
                        {
                            return (y, x);
                        }
                    }
                }
            }
            return null;
        }
        private void UserColor_Click(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                Ellipse greyCircle = new Ellipse();
                greyCircle.Width = button.Width + 1;
                greyCircle.Height = button.Height + 1;

                greyCircle.Stroke = Brushes.Green;
                greyCircle.StrokeThickness = 1;

                greyCircle.Margin = new Thickness(-10);

                button.Content = greyCircle;

                object asd = button.Tag.GetType();

                if (button.Tag is Tuple<int, int> tag)
                {
                    chosenCustomColor = (tag.Item1, tag.Item2);
                }
                for (int i = 0; i < UserColors.GetLength(0); i++)
                {
                    for (int j = 0; j < UserColors.GetLength(1); j++)
                    {
                        if (UserColors[i, j] is Button but)
                        {
                            ClearEnterColor(but);
                        }
                    }
                }
            }
        }
        private void RoundButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            MessageBox.Show("Round button clicked!");
        }
        private void OK_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void OTMENA_Click(object sender, EventArgs e)
        {
            Close();
        }
        public void AddEnterIngBorder(Button button)
        {
            if (!(button is null) &&
                button.Tag is Tuple<int, int> tag &&
                chosenCustomColor == (tag.Item1, tag.Item2))
            {
                return;
            }
            Ellipse greyCircle = new Ellipse();
            greyCircle.Width = button.Width + 1;
            greyCircle.Height = button.Height + 1;

            greyCircle.Stroke = Brushes.DarkGray;
            greyCircle.StrokeThickness = 1;

            greyCircle.Margin = new Thickness(-10);

            button.Content = greyCircle;
        }
        public void ClearEnterColor(Button button)
        {
            if (button is null) return;
            if (button.Tag is Tuple<int, int> tag &&
                chosenCustomColor == (tag.Item1, tag.Item2))
            {
                return;
            }
            Ellipse greyCircle = new Ellipse();
            greyCircle.Width = button.Width + 1;
            greyCircle.Height = button.Height + 1;
            greyCircle.Fill = Brushes.Transparent;

            greyCircle.Stroke = PalletePanel.Background;
            greyCircle.StrokeThickness = 1;

            greyCircle.Margin = new Thickness(-10);

            button.Content = greyCircle;
        }
        private UIElement valueDragElem = null;
        private Point valueOffset;
        private void ValueDrag_PreViewMouseDown(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;

            Point buttonPosition = button.TransformToAncestor(ValueCanvas)
                                         .Transform(new Point(0, 0));

            double centerX = buttonPosition.X + (button.ActualWidth / 2);
            double centerY = buttonPosition.Y + (button.ActualHeight / 2);

            valueDragElem = sender as UIElement;
            valueOffset = new Point(centerX, centerY); //e.GetPosition(ValueCanvas);
            valueOffset.Y -= Canvas.GetTop(valueDragElem);
            //valueOffset.X -= Canvas.GetLeft(valueDragElem);
            ValueCanvas.CaptureMouse();
        }
        private void ValueCanvas_PreViewMouseMove(object sender, MouseEventArgs e)
        {
            Point buttonPosRelativeToAncestor = draggableButton.TransformToAncestor(this).Transform(new Point(0, 0));

            Point imagePosRelativeToAncestor = coloeSpecter.TransformToAncestor(this).Transform(new Point(0, 0));

            double relativeX = buttonPosRelativeToAncestor.X - imagePosRelativeToAncestor.X;
            double relativeY = buttonPosRelativeToAncestor.Y - imagePosRelativeToAncestor.Y;

            double centerX = relativeX + (draggableButton.ActualWidth / 2);
            double centerY = relativeY + (draggableButton.ActualHeight / 2);

            if (valueDragElem == null) return;
            var position = e.GetPosition(sender as IInputElement);

            if (position.Y < 8)
            {
                position.Y = 8;
            }
            if (position.Y > ValueBorder.Height - 8)
            {
                position.Y = ValueBorder.Height - 8;
            }
            Canvas.SetTop(valueDragElem, position.Y - valueOffset.Y);

            ChangeProgressBarValue(position.Y - valueOffset.Y);
        }
        public void ChangeProgressBarValue(double pos)
        {
            double onePointHeight = ValueProgressBar.Height / ValueProgressBar.Maximum;
            double temp = pos / onePointHeight;
            ThirdInfoBox.Text = Math.Abs(((int)temp) - 100).ToString();
        }
        private void ValueCanvas_PreViewMouseUp(object sender, MouseEventArgs e)
        {
            valueDragElem = null;
            ValueCanvas.ReleaseMouseCapture();
        }

        private UIElement specDragEl = null;
        private Point specCircleOffset;

        private void DraggableButton_PreViewMouseDown(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;

            Point buttonPosition = button.TransformToAncestor(SpecCanvas)
                                         .Transform(new Point(0, 0));

            double centerX = buttonPosition.X + (button.ActualWidth / 2);
            double centerY = buttonPosition.Y + (button.ActualHeight / 2);

            this.specDragEl = sender as UIElement;
            specCircleOffset = new Point(centerX, centerY); //e.GetPosition(this.SpecCanvas);
            specCircleOffset.Y -= Canvas.GetTop(specDragEl);
            specCircleOffset.X -= Canvas.GetLeft(specDragEl);
            SpecCanvas.CaptureMouse();
        }
        private void CanvasMain_PreViewMouseMove(object sender, MouseEventArgs e)
        {
            Point buttonPosRelativeToAncestor = draggableButton.TransformToAncestor(this).Transform(new Point(0, 0));

            Point imagePosRelativeToAncestor = coloeSpecter.TransformToAncestor(this).Transform(new Point(0, 0));

            double relativeX = buttonPosRelativeToAncestor.X - imagePosRelativeToAncestor.X;
            double relativeY = buttonPosRelativeToAncestor.Y - imagePosRelativeToAncestor.Y;

            double centerX = relativeX + (draggableButton.ActualWidth / 2);
            double centerY = relativeY + (draggableButton.ActualHeight / 2);

            if (specDragEl == null) return;
            var position = e.GetPosition(sender as IInputElement);

            if (position.X < 0)
            {
                position.X = 0;
            }
            if (position.X > coloeSpecter.Width - 1)
            {
                position.X = coloeSpecter.Width - 1;
            }
            if (position.Y < 0)
            {
                position.Y = 0;
            }
            if (position.Y > coloeSpecter.Height - 1)
            {
                position.Y = coloeSpecter.Height - 1;
            }
            Canvas.SetTop(specDragEl, position.Y - specCircleOffset.Y);
            Canvas.SetLeft(specDragEl, position.X - specCircleOffset.X);

            Color color = GetColorByCord(position);

            System.Windows.Media.Color mediaColor = System.Windows.Media.Color.FromArgb(
                  color.A, color.R, color.G, color.B);

            ChosenColorShow.Background = new SolidColorBrush(mediaColor);
            HexTable.Text = GetHexFromColor(mediaColor.R, mediaColor.G, mediaColor.B);
            UpdateValueBackGroud(mediaColor);
        }
        private void CanvasMain_PreViewMouseUp(object sender, MouseEventArgs e)
        {
            specDragEl = null;
            SpecCanvas.ReleaseMouseCapture();
        }
        private void asd_Click(object sender, EventArgs e)
        {
            Console.WriteLine();
        }
        public Color GetColorByCord(Point point)
        {
            Color color = GetColorAtPosition(coloeSpecter, (int)point.X, (int)point.Y);
            return color;
            /* string path = GetDirToPallete();
            using (Bitmap bitmap = new Bitmap(path))
            {
                return bitmap.GetPixel((int)point.X, (int)point.Y);
            }*/
        }
        private Color GetColorAtPosition(System.Windows.Shapes.Rectangle rectangle, int x, int y)
        {
            // Create a RenderTargetBitmap and render the rectangle into it
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(
                (int)rectangle.Width, (int)rectangle.Height, 96, 96, PixelFormats.Pbgra32);
            rectangle.Measure(new System.Windows.Size((int)rectangle.Width, (int)rectangle.Height));
            rectangle.Arrange(new Rect(new System.Windows.Size((int)rectangle.Width, (int)rectangle.Height)));
            renderTargetBitmap.Render(rectangle);

            // Copy the bitmap to a byte array
            var croppedBitmap = new CroppedBitmap(renderTargetBitmap, new Int32Rect(x, y, 1, 1));
            byte[] pixels = new byte[4];
            croppedBitmap.CopyPixels(pixels, 4, 0);

            // Convert the byte array to a Color
            return Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);
        }
        public string GetDirToPallete()
        {
            var scriptsDirectory = AppDomain.CurrentDomain.BaseDirectory;

            DirectoryInfo baseDirectoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            string imageDirectory = System.IO.Path.Combine(baseDirectoryInfo.Parent.Parent.FullName, "Images");
            string palleteDirectory = System.IO.Path.Combine(imageDirectory, "Pallete");
            string imagePath = System.IO.Path.Combine(palleteDirectory, "ColorSpectre.png");
            return imagePath;
            //return "";

        }
        public void UpdateValueBackGroud(System.Windows.Media.Color color)
        {
            LinearGradientBrush gradientBrush = (LinearGradientBrush)ValueBorder.Background;
            gradientBrush.GradientStops[0].Color = color;
            UpdateColorParams(color);
        }
        public void UpdateColorParams(System.Windows.Media.Color color)
        {
            _chosenColor = color;
            if (ChooseParamTypeBox.SelectedIndex == 0)
            {
                FirstInfoBox.Text = color.R.ToString();
                SecondInfoBox.Text = color.G.ToString();
                ThirdInfoBox.Text = color.B.ToString();
                return;
            }
            (int H, int S, int V) HSVParams = ColorConvertor.RGBtoHSV(color);
            FirstInfoBox.Text = HSVParams.H.ToString();
            SecondInfoBox.Text = HSVParams.S.ToString();
            ThirdInfoBox.Text = HSVParams.V.ToString();
        }
        private void ColorParamType_SelectionChanged(object sender, EventArgs e)
        {
            if (ChooseParamTypeBox.SelectedIndex == 0)
            {
                FirstLB.Content = "Красный";
                SecondLB.Content = "Зеленый";
                ThirdLB.Content = "Синий";

                FirstInfoBox.Text = _chosenColor.R.ToString();
                SecondInfoBox.Text = _chosenColor.G.ToString();
                ThirdInfoBox.Text = _chosenColor.B.ToString();
                return;
            }
            FirstLB.Content = "Оттенок";
            SecondLB.Content = "Насыщенность";
            ThirdLB.Content = "Значение";

            FirstInfoBox.Text = _chosenColor.R.ToString();
            SecondInfoBox.Text = _chosenColor.G.ToString();
            ThirdInfoBox.Text = _chosenColor.B.ToString();
        }



    }
}

