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
        private Point _colorPoint = new Point(0, 0);
        private TaskColor _tempColor = new TaskColor(255, 0, 0, 0);

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
            FilluserColors();
        }
        public void FilluserColors()
        {
            for (int i = 0; i < _pallete.UserColors.GetLength(0); i++)
            {
                for (int j = 0; j < _pallete.UserColors.GetLength(1); j++)
                {
                    if (!(_pallete.UserColors[i, j] is null) &&
                        UserColors[i, j] is Button but)
                    {
                        System.Windows.Media.Color color = _pallete.UserColors[i, j].HSLtoRGB(); /*System.Windows.Media.Color.FromRgb((byte)_pallete.UserColors[i, j].R,
                            (byte)_pallete.UserColors[i, j].G, (byte)_pallete.UserColors[i, j].B);*/
                        ChangeBgForInnerEllipseForCustomColor(color, but);
                    }
                }
            }
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

            (int y, int x) mainColorIndex =
                _pallete.GetMainColorIndexByColor((System.Windows.Media.Color)color);
            if (mainColorIndex.y == -1 && mainColorIndex.x == -1) return;

            // get point int specImg
            /*            System.Windows.Media.Color col =
                            (System.Windows.Media.Color)color;*/


            System.Windows.Media.Color col = ShowColorOnSpecForMainColor(mainColorIndex);

            (int, int)? asd = GetColorCord(col.R, col.G, col.B);
            if (asd is null) return;

            // int boxes right boxes 
            InitCordInSpec(asd.Value.Item1, asd.Value.Item2);

            specDragEl = null;
            SpecCanvas.ReleaseMouseCapture();
        }
        public System.Windows.Media.Color ShowColorOnSpecForMainColor((int, int) mainColorInPalleteIndex)
        {
            var asd = _pallete.MainColors[mainColorInPalleteIndex.Item1, mainColorInPalleteIndex.Item2];

            TaskColor color = new TaskColor(asd.TColor);

            color.Hue = asd.HSVParam.H;
            color.Saturation = asd.HSVParam.S;
            color.Luminance = asd.HSVParam.V;

            System.Windows.Media.Color qwe = color.HSLtoRGB();



            return qwe;
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
                InitCircleUponUserColor(button);

                if (button.Tag is Tuple<int, int> tag)
                {
                    _pallete.ChosenCustomColorIndex = (tag.Item1, tag.Item2);
                }
                CleateUserColors();
            }
        }
        
        public void InitCircleUponUserColor(Button button)
        {
            Ellipse uponCircle = new Ellipse();
            uponCircle.Width = button.Width + 1;
            uponCircle.Height = button.Height + 1;

            uponCircle.Stroke = Brushes.Green;
            uponCircle.StrokeThickness = 1;

            uponCircle.Margin = new Thickness(-10);

            button.Content = uponCircle;
        }

        public void CleateUserColors()
        {
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
                _pallete.ChosenCustomColorIndex == (tag.Item1, tag.Item2))
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
                _pallete.ChosenCustomColorIndex == (tag.Item1, tag.Item2))
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
            if (valueDragElem == null) return;
            var pos = e.GetPosition(sender as IInputElement);

            if (pos.Y < 8)
            {
                pos.Y = 8;
            }
            if (pos.Y > ValueBorder.Height - 8)
            {
                pos.Y = ValueBorder.Height - 8;
            }
            Canvas.SetTop(valueDragElem, pos.Y - valueOffset.Y);
            ChangeProgressBarValue(pos.Y - valueOffset.Y);

            UpdateShowColor();
        }
        public void ChangeProgressBarValue(double pos)
        {
            double onePointHeight = ValueProgressBar.Height / ValueProgressBar.Maximum;
            double temp = pos / onePointHeight;

            int value = Math.Abs(((int)temp) - 100);
            ThirdInfoBox.Text = value.ToString();

            _pallete.TempL = value;
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
            if (specDragEl == null) return;
            _colorPoint = e.GetPosition(sender as IInputElement);
            if (_colorPoint.X < 0)
            {
                _colorPoint.X = 0;
            }
            if (_colorPoint.X > coloeSpecter.Width - 1)
            {
                _colorPoint.X = coloeSpecter.Width - 1;
            }
            if (_colorPoint.Y < 0)
            {
                _colorPoint.Y = 0;
            }
            if (_colorPoint.Y > coloeSpecter.Height - 1)
            {
                _colorPoint.Y = coloeSpecter.Height - 1;
            }
            Canvas.SetTop(specDragEl, _colorPoint.Y - specCircleOffset.Y);
            Canvas.SetLeft(specDragEl, _colorPoint.X - specCircleOffset.X);

            UpdateShowColor();
        }
        public void UpdateShowColor()
        {
            System.Windows.Media.Color mediaColor =
            GetColorAtPosition(coloeSpecter,
            (int)_colorPoint.X, (int)_colorPoint.Y);

            //HexTable.Text = GetHexFromColor(mediaColor.R, mediaColor.G, mediaColor.B);
            ReInitColorShowCOlorPanel(mediaColor);
        }
        public void ReInitColorShowCOlorPanel(System.Windows.Media.Color color)
        {
            _tempColor = new TaskColor(color);
            _tempColor.RGBtoHSL(color);
            double lum = _pallete.TempL;
            lum /= 100;
            _tempColor.Luminance = lum;
            System.Windows.Media.Color asd = _tempColor.HSLtoRGB();
            HexTable.Text = asd.ToString();
            ChosenColorShow.Background = new SolidColorBrush(asd);

            LinearGradientBrush gradientBrush =
            (LinearGradientBrush)ValueBorder.Background;
            gradientBrush.GradientStops[0].Color = color;
            UpdateColorParams(asd);
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

        private System.Windows.Media.Color GetColorAtPosition
            (System.Windows.Shapes.Rectangle rectangle, int x, int y)
        {
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(
                (int)rectangle.Width, (int)rectangle.Height, 96, 96, PixelFormats.Pbgra32);
            rectangle.Measure(new System.Windows.Size((int)rectangle.Width, (int)rectangle.Height));
            rectangle.Arrange(new Rect(new System.Windows.Size((int)rectangle.Width, (int)rectangle.Height)));
            renderTargetBitmap.Render(rectangle);

            var croppedBitmap = new CroppedBitmap(renderTargetBitmap,
                new Int32Rect(x, y, 1, 1));
            byte[] pixels = new byte[4];
            croppedBitmap.CopyPixels(pixels, 4, 0);

            return System.Windows.Media.Color.FromArgb(pixels[3],
                pixels[2], pixels[1], pixels[0]);
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

        public void UpdateColorParams(System.Windows.Media.Color color)
        {
            _chosenColor = color;
            //(int H, int S, int V) HCVParams = ColorConvertor.RGBtoHCV(color);

            System.Windows.Media.Color check = _tempColor.HSLtoRGB();

            if (ChooseParamTypeBox.SelectedIndex == 0)
            {
                FirstInfoBox.Text = check.R.ToString();
                SecondInfoBox.Text = check.G.ToString();
                ThirdInfoBox.Text = check.B.ToString();
                return;
            }
            FirstInfoBox.Text = _tempColor.H.ToString();
            SecondInfoBox.Text = _tempColor.S.ToString();
            ThirdInfoBox.Text = _tempColor.L.ToString();
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
        private void AddCustomColor_Click(object sender, EventArgs e)
        {
            if (_pallete.ChosenCustomColorIndex != (-1, -1) &&
                UserColors[_pallete.ChosenCustomColorIndex.Item1,
                _pallete.ChosenCustomColorIndex.Item2] is Button but)
            {

                System.Windows.Media.Color newColor = _tempColor.HSLtoRGB();
                /*System.Windows.Media.Color.FromArgb((byte)_tempColor.A,
                    (byte)_tempColor.R, (byte)_tempColor.G, (byte)_tempColor.B);
*/
                _pallete.UserColors[_pallete.ChosenCustomColorIndex.Item1,
                    _pallete.ChosenCustomColorIndex.Item2] = new TaskColor(newColor);

                ChangeBgForInnerEllipseForCustomColor(newColor, but);

                _pallete.MoveUserColorIndex();

                InitCircleUponUserColor((Button)UserColors[_pallete.ChosenCustomColorIndex.Item1,
                _pallete.ChosenCustomColorIndex.Item2]);
                CleateUserColors();
            }
        }
        public void ChangeBgForInnerEllipseForCustomColor(
            System.Windows.Media.Color color, Button but)
        {
            if (but.Template != null)
            {
                Ellipse outerEllipse = but.Template.FindName("innerEllipse", but) as Ellipse;
                if (outerEllipse != null)
                {
                    outerEllipse.Fill = new SolidColorBrush(color);
                    outerEllipse.Stroke = null;
                }
            }
        }

        private void PalletePanel_Loaded(object sender, RoutedEventArgs e)
        {
            FilluserColors();
        }
    }
}

