using PaintWPF.Models;
using PaintWPF.Models.Tools;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data.SqlTypes;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit.PropertyGrid.Converters;
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
        private SolidColorBrush _colorToPaint;

        private UIElement[,] UserColors;
        private Point _colorPoint = new Point(0, 0);
        private TaskColor _tempColor = new TaskColor(255, 0, 0, 0);

        private System.Windows.Media.Color? _chosenColor = null;


        public Pallete(PalleteModel pallete, SolidColorBrush mainColor)
        {
            _pallete = pallete;
            _colorToPaint = mainColor;

            InitializeComponent();

            InitBasicParamsAfterInitComps();

            valueDragElem = ValueBut;

            InitParamsForGivenColor();
        }
        private void InitParamsForGivenColor()
        {
            _tempColor.R = _colorToPaint.Color.R;
            _tempColor.G = _colorToPaint.Color.G;
            _tempColor.B = _colorToPaint.Color.B;

            _tempColor.RGBtoHSL(_colorToPaint.Color);

            //init R 
            FirstInfoBox.Text = _tempColor.R.ToString();
            //int G
            SecondInfoBox.Text = _tempColor.G.ToString();
            //int B
            ThirdInfoBox.Text = _tempColor.B.ToString();
            //Init Hex
            HexTable.Text = _tempColor.GetHexFromRGB();
            //int showColor 
            ChosenColorShow.Background = _colorToPaint;

            System.Windows.Media.Color color =
                GetColorWithThHighestLuminosity(_colorToPaint.Color);
            //init Gradient 
            InitStartLuminanceGradientValue(color);

            //init position

            (int, int)? colorCord = GetColorCord(color.R, color.G, color.B);
            if (colorCord is null) return;
            InitCordInSpec(colorCord.Value.Item1, colorCord.Value.Item2);
        }
        public void InitBasicParamsAfterInitComps()
        {
            const int dragButLocParam = 20;
            Canvas.SetTop(draggableButton, dragButLocParam);
            Canvas.SetLeft(draggableButton, dragButLocParam);

            const int valueButStartParam = 1;
            Canvas.SetTop(ValueBut, valueButStartParam);
            Canvas.SetLeft(ValueBut, valueButStartParam);

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
                        System.Windows.Media.Color color = _pallete.UserColors[i, j].HSLtoRGB();
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
            const string hashMark = "#";
            if (sender is TextBox box)
            {
                if (box.Text.Length == 0)
                {
                    box.Text = hashMark;
                    box.CaretIndex = box.Text.Length;
                }
                if (box.Text.Length != 0 && box.Text.First() != hashMark.ToCharArray().First())
                {
                    box.Text = box.Text.Remove(0, 1);
                    if (!box.Text.Contains(hashMark))
                    {
                        box.Text = box.Text.Insert(0, hashMark);
                    }
                    box.CaretIndex = box.Text.Length;
                }
                if (box.Text.Length == maxAmountOfSymbols)
                {
                    box.Text = box.Text.Remove(box.Text.Length - 1);
                    box.CaretIndex = box.Text.Length;
                    return;
                }
                if (HexTable.IsFocused)
                {
                    InitColorsFromHex(_tempColor.ConvertHexIntoRGB(box.Text));
                }
            }
        }
        private void InitColorsFromHex(bool ifNeddConvertion)
        {
            if (!ifNeddConvertion) return;
            //get new color
            System.Windows.Media.Color color = System.Windows.Media.Color.FromRgb(
                (byte)_tempColor.R, (byte)_tempColor.G, (byte)_tempColor.B);

            //assign color to show color column 
            ChosenColorShow.Background = new SolidColorBrush(color);

            //assign color to gradient param
            //init HSL 
            _tempColor.RGBtoHSL(color);

            //correct luminance column
            InitLumCircleHeight((int)(_tempColor.L * 100));

            //init color location
            InitPositionForColor(color);

            //init rgb or hsl
            InitColors();
        }
        private void HexTable_LostFocus(object sender, RoutedEventArgs e)
        {
            const string ifHexFaildStr = "#000000";
            if (_tempColor.ConvertHexIntoRGB(HexTable.Text)) return;
            HexTable.Text = ifHexFaildStr;
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
                _pallete.ChosenColor = brush;
            }
            _colorToPaint = (SolidColorBrush)but.Background;
            InitParamsForGivenColor();

            if (color is null) return;
            (int y, int x) mainColorIndex =
                _pallete.GetMainColorIndexByColor((System.Windows.Media.Color)color);
            if (mainColorIndex.y == -1 && mainColorIndex.x == -1) return;

            System.Windows.Media.Color col = ShowColorOnSpecForMainColor(mainColorIndex);
            InitExplorerForColors(col);

            specDragEl = null;
            SpecCanvas.ReleaseMouseCapture();
        }
        private void InitExplorerForColors(System.Windows.Media.Color col)
        {
            //Get color with the highest luminance
            col = GetColorWithThHighestLuminosity(col);

            //Find it in specture
            (int, int)? asd = GetColorCord(col.R, col.G, col.B);
            if (asd is null) return;

            //Init cursor in this cords
            InitCordInSpec(asd.Value.Item1, asd.Value.Item2);

            //Init Values in field
            _colorPoint = new Point(asd.Value.Item2, asd.Value.Item1);
            UpdateShowColor();
        }
        private System.Windows.Media.Color GetColorWithThHighestLuminosity(System.Windows.Media.Color color)
        {
            const int startLum = 1;
            TaskColor tempColor = new TaskColor(color);
            tempColor.Luminance = startLum;
            System.Windows.Media.Color res = tempColor.HSLtoRGB();
            return res;
        }
        public System.Windows.Media.Color ShowColorOnSpecForMainColor((int, int) mainColorInPalleteIndex)
        {
            var colorParam = _pallete.MainColors[mainColorInPalleteIndex.Item1, mainColorInPalleteIndex.Item2];
            TaskColor color = new TaskColor(colorParam.TColor);
            System.Windows.Media.Color res = color.HSLtoRGB();
            return res;
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

            System.Windows.Controls.Image specimage = ConvertRectangleFillToImage();
            RenderTargetBitmap renderTarget = specimage.Source as RenderTargetBitmap;

            int width = renderTarget.PixelWidth;
            int height = renderTarget.PixelHeight;
            int stride = width * 4;
            byte[] pixels = new byte[height * stride];
            renderTarget.CopyPixels(pixels, stride, 0);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * stride + x * 4;
                    byte bCheck = pixels[index];
                    byte gCheck = pixels[index + 1];
                    byte rCheck = pixels[index + 2];
                    byte aCheck = pixels[index + 3];

                    if (rCheck == targetColor.R && gCheck == targetColor.G &&
                        bCheck == targetColor.B && aCheck == targetColor.A)
                    {
                        return (y, x);
                    }
                }
            }
            return null;
        }
        private const int _dpi = 96;
        public System.Windows.Controls.Image ConvertRectangleFillToImage()
        {
            int width = (int)coloeSpecter.Width;
            int height = (int)coloeSpecter.Height;

            RenderTargetBitmap renderTarget = new RenderTargetBitmap(
                width, height, _dpi, _dpi, PixelFormats.Pbgra32);

            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(coloeSpecter.Fill, null, new Rect(0, 0, width, height));
            }
            renderTarget.Render(drawingVisual);
            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            image.Source = renderTarget;
            return image;
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
            uponCircle.Width = button.Width + _sizeCorrel;
            uponCircle.Height = button.Height + _sizeCorrel;
            uponCircle.Stroke = Brushes.Green;
            uponCircle.StrokeThickness = _strokeThickness;
            uponCircle.Margin = new Thickness(_margin);

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
            if (_pallete.ChosenColor is null ||
                _pallete.ChosenColor != (SolidColorBrush)ChosenColorShow.Background) InitChosenColor();
            Close();
        }
        private SolidColorBrush _basicWhite =
            new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 255, 255));
        private SolidColorBrush _usageWhite =
            new SolidColorBrush(System.Windows.Media.Color.FromRgb(252, 252, 252));
        private void InitChosenColor()
        {
            _pallete.ChosenColor = (SolidColorBrush)ChosenColorShow.Background;
            Console.WriteLine(_pallete.ChosenColor.Color.R.ToString(),
                _pallete.ChosenColor.Color.G, _pallete.ChosenColor.Color.B);
            if (_pallete.ChosenColor.Color == _basicWhite.Color)
            {
                _pallete.ChosenColor = _usageWhite;
            }
        }
        private void OTMENA_Click(object sender, EventArgs e)
        {
            _pallete.ChosenColor = null;
            Close();
        }
        private const int _sizeCorrel = 1;
        private const int _margin = -10;
        private const int _strokeThickness = 1;
        public void AddEnterIngBorder(Button button)
        {

            if (!(button is null) &&
                button.Tag is Tuple<int, int> tag &&
                _pallete.ChosenCustomColorIndex == (tag.Item1, tag.Item2))
            {
                return;
            }
            Ellipse greyCircle = new Ellipse();
            greyCircle.Width = button.Width + _sizeCorrel;
            greyCircle.Height = button.Height + _sizeCorrel;

            greyCircle.Stroke = Brushes.DarkGray;
            greyCircle.StrokeThickness = _strokeThickness;

            greyCircle.Margin = new Thickness(_margin);
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

            greyCircle.Width = button.Width + _sizeCorrel;
            greyCircle.Height = button.Height + _sizeCorrel;
            greyCircle.Fill = Brushes.Transparent;
            greyCircle.Stroke = PalletePanel.Background;
            greyCircle.StrokeThickness = _strokeThickness;
            greyCircle.Margin = new Thickness(_margin);

            button.Content = greyCircle;
        }

        private UIElement valueDragElem = null;
        private bool _ifLumValueCanBeChanged = false;
        private Point valueOffset;
        private const int _middleDevider = 2;
        private void ValueDrag_PreViewMouseDown(object sender, MouseEventArgs e)
        {

            Button button = sender as Button;

            Point buttonPosition = button.TransformToAncestor(ValueCanvas)
                                         .Transform(new Point(0, 0));
            double centerX = buttonPosition.X + (button.ActualWidth / _middleDevider);
            double centerY = buttonPosition.Y + (button.ActualHeight / _middleDevider);

            _ifLumValueCanBeChanged = true;

            valueOffset = new Point(centerX, centerY);
            valueOffset.Y -= Canvas.GetTop(valueDragElem);
        }
        private const int _lumValueHeightDifferance = 8;
        private void ValueCanvas_PreViewMouseMove(object sender, MouseEventArgs e)
        {
            if (!_ifLumValueCanBeChanged) return;
            var pos = e.GetPosition(sender as IInputElement);

            if (pos.Y < _lumValueHeightDifferance)
            {
                pos.Y = _lumValueHeightDifferance;
            }
            if (pos.Y > ValueBorder.Height - _lumValueHeightDifferance)
            {
                pos.Y = ValueBorder.Height - _lumValueHeightDifferance;
            }
            Canvas.SetTop(valueDragElem, pos.Y - valueOffset.Y);
            ChangeProgressBarValue(pos.Y - valueOffset.Y);

            UpdateShowColor();
        }
        public void ChangeProgressBarValue(double pos)
        {
            double onePointHeight = ValueProgressBar.Height /
                ValueProgressBar.Maximum;
            double temp = pos / onePointHeight;

            int value = Math.Abs(((int)temp) - _maxLValue);
            ThirdInfoBox.Text = value.ToString();

            _pallete.TempL = value;
        }
        private void ValueCanvas_PreViewMouseUp(object sender, MouseEventArgs e)
        {
            //valueDragElem = null;
            //ValueCanvas.ReleaseMouseCapture();
        }
        private UIElement specDragEl = null;
        private Point specCircleOffset;
        private bool _ifCircleCanBeMoved = false;

        private void DraggableButton_PreViewMouseDown(object sender, MouseEventArgs e)
        {
            _ifCircleCanBeMoved = true;
            Button button = sender as Button;

            Point buttonPosition = button.TransformToAncestor(SpecCanvas)
                                         .Transform(new Point(0, 0));

            double centerX = buttonPosition.X + (button.ActualWidth / _middleDevider);
            double centerY = buttonPosition.Y + (button.ActualHeight / _middleDevider);

            this.specDragEl = sender as UIElement;
            specCircleOffset = new Point(centerX, centerY);
            specCircleOffset.Y -= Canvas.GetTop(specDragEl);
            specCircleOffset.X -= Canvas.GetLeft(specDragEl);
        }
        private void DraggableButton_PreViewMouseUp(object sender, MouseEventArgs e)
        {
            _ifCircleCanBeMoved = false;
        }
        private void SpecCanvas_PreViewMouseMove(object sender, MouseEventArgs e)
        {
            MoveCircle(sender, e);
        }
        public void UpdateShowColor()
        {
            System.Windows.Media.Color mediaColor =
            GetColorAtPosition(coloeSpecter,
            (int)_colorPoint.X, (int)_colorPoint.Y);

            //HexTable.Text = GetHexFromColor(mediaColor.R, mediaColor.G, mediaColor.B);
            ReInitColorShowColorPanel(mediaColor);
        }
        public void ReInitColorShowColorPanel(System.Windows.Media.Color color)
        {
            const int lumDevider = 100;
            _tempColor = new TaskColor(color);
            _tempColor.RGBtoHSL(color);
            double lum = _pallete.TempL;
            lum /= lumDevider;
            _tempColor.Luminance = lum;
            System.Windows.Media.Color newColor = _tempColor.HSLtoRGB();

            HexTable.Text = _tempColor.GetHexFromRGB();
            ChosenColorShow.Background = new SolidColorBrush(newColor);

            InitStartLuminanceGradientValue(color);
            UpdateColorParams(newColor);
        }
        private void SpecCanvas_PreViewMouseUp(object sender, MouseEventArgs e)
        {
            specDragEl = null;
            SpecCanvas.ReleaseMouseCapture();
        }
        private System.Windows.Media.Color GetColorAtPosition
            (System.Windows.Shapes.Rectangle rectangle, int x, int y)
        {
            const int startLocParam = 1;
            const int amountOfPixels = 4;

            const int firstpixelIndex = 3;
            const int secondPixelIndex = 2;
            const int thirdPixelIndex = 1;
            const int forthPixelIndex = 0;

            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(
                (int)rectangle.Width, (int)rectangle.Height, _dpi, _dpi, PixelFormats.Pbgra32);
            rectangle.Measure(new System.Windows.Size((int)rectangle.Width, (int)rectangle.Height));
            rectangle.Arrange(new Rect(new System.Windows.Size((int)rectangle.Width, (int)rectangle.Height)));
            renderTargetBitmap.Render(rectangle);

            var croppedBitmap = new CroppedBitmap(renderTargetBitmap,
                new Int32Rect(x, y, startLocParam, startLocParam));
            byte[] pixels = new byte[amountOfPixels];
            croppedBitmap.CopyPixels(pixels, amountOfPixels, 0);

            return System.Windows.Media.Color.FromArgb(pixels[firstpixelIndex],
                pixels[secondPixelIndex], pixels[thirdPixelIndex], pixels[forthPixelIndex]);
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
            InitColors();
        }
        private const int _hslMultiplier = 100;
        private const double _hMultiplier = 3.6;
        private void InitColors()
        {
            if (ChooseParamTypeBox.SelectedIndex == 0)
            {
                FirstInfoBox.Text = _tempColor.R.ToString();
                SecondInfoBox.Text = _tempColor.G.ToString();
                ThirdInfoBox.Text = _tempColor.B.ToString();
                return;
            }
            FirstInfoBox.Text = ((int)(_tempColor.H * _hslMultiplier * _hMultiplier)).ToString();
            SecondInfoBox.Text = ((int)(_tempColor.S * _hslMultiplier)).ToString();
            ThirdInfoBox.Text = ((int)(_tempColor.L * _hslMultiplier)).ToString();
        }
        private void ColorParamType_SelectionChanged(object sender, EventArgs e)
        {
            if (ChooseParamTypeBox.SelectedIndex == 0)
            {
                FirstLB.Content = "Красный";
                SecondLB.Content = "Зеленый";
                ThirdLB.Content = "Синий";

                FirstInfoBox.Text = _tempColor.R.ToString();
                SecondInfoBox.Text = _tempColor.G.ToString();
                ThirdInfoBox.Text = _tempColor.B.ToString();
                return;
            }
            FirstLB.Content = "Оттенок";
            SecondLB.Content = "Насыщенность";
            ThirdLB.Content = "Значение";

            FirstInfoBox.Text = ((int)(_tempColor.H * _hslMultiplier * _hMultiplier)).ToString();
            SecondInfoBox.Text = ((int)(_tempColor.S * _hslMultiplier)).ToString();
            ThirdInfoBox.Text = ((int)(_tempColor.L * _hslMultiplier)).ToString();
        }
        private void AddCustomColor_Click(object sender, EventArgs e)
        {
            if (_pallete.ChosenCustomColorIndex != (-1, -1) &&
                UserColors[_pallete.ChosenCustomColorIndex.Item1,
                _pallete.ChosenCustomColorIndex.Item2] is Button but)
            {
                System.Windows.Media.Color newColor = _tempColor.HSLtoRGB();
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
        private void CustomColor_Click(object sender, EventArgs e)
        {
            if (sender is Button but &&
                but.Background is SolidColorBrush brush)
            {
                Ellipse outerEllipse = but.Template.FindName("innerEllipse", but) as Ellipse;
                if (outerEllipse != null)
                {
                    _pallete.ChosenColor = new SolidColorBrush(((SolidColorBrush)outerEllipse.Fill).Color);
                }
                InitExplorerForColors(((SolidColorBrush)outerEllipse.Fill).Color);
            }
        }
        private void PalletePanel_Loaded(object sender, RoutedEventArgs e)
        {
            FilluserColors();
        }

        private const int _maxAmountOfDiftsInColorParams = 3;

        private const int _maxRGBValue = 255;
        private const int _maxHValue = 359;
        private const int _maxSValue = 100;
        private const int _maxLValue = 100;
        private void FirstParam_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckParamsForAmountOfDigits(sender as TextBox);

            if (sender is TextBox box &&
                FirstInfoBox.IsFocused)
            {
                if (ChooseParamTypeBox.SelectedIndex == 0)//RGB
                {
                    GetColorFromParamBox(FirstInfoBox);
                }
                else//HSL
                {
                    InitNewHueFromBox();
                    InitColorPosition();
                }
                HexTable.Text = _tempColor.GetHexFromRGB();
            }
        }
        private void GetColorFromParamBox(TextBox box)
        {
            //get inited color
            if (!int.TryParse(box.Text, out int colorValue)) return;

            //change color param in temp
            InitRgbValueByBox(box, colorValue);

            //get new color
            System.Windows.Media.Color color = System.Windows.Media.Color.FromRgb(
                (byte)_tempColor.R, (byte)_tempColor.G, (byte)_tempColor.B);

            //assign color to show color column 
            ChosenColorShow.Background = new SolidColorBrush(color);

            //assign color to gradient param
            //init HSL 
            _tempColor.RGBtoHSL(color);

            //correct luminance column
            InitLumCircleHeight((int)(_tempColor.L * _hslMultiplier));

            //init color location
            InitPositionForColor(color);
        }
        private void InitPositionForColor(System.Windows.Media.Color color)
        {
            System.Windows.Media.Color tempColor =
                GetColorWithThHighestLuminosity(color);

            InitColorPosWithGivenColor(tempColor);
            InitColorPosition();

            InitStartLuminanceGradientValue(tempColor);
        }
        private void InitRgbValueByBox(TextBox box, int value)
        {
            if (box.Name == "FirstInfoBox")
            {
                _tempColor.R = value;
            }
            else if (box.Name == "SecondInfoBox")
            {
                _tempColor.G = value;
            }
            else if (box.Name == "ThirdInfoBox")
            {
                _tempColor.B = value;
            }
        }
        private void InitNewHueFromBox()
        {
            CheckForAccaptableValues(FirstInfoBox, _maxHValue, _maxRGBValue);
            if (!int.TryParse(FirstInfoBox.Text, out int newValue)) return;

            double hue = newValue;
            hue /= _maxHValue;
            _tempColor.Hue = hue;
            System.Windows.Media.Color newColor = _tempColor.HSLtoRGB();
            ChosenColorShow.Background = new SolidColorBrush(newColor);
            InitStartLuminanceGradientValue(newColor);
        }
        private void SecondParam_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckParamsForAmountOfDigits(sender as TextBox);

            if (sender is TextBox box &&
                SecondInfoBox.IsFocused)
            {
                if (ChooseParamTypeBox.SelectedIndex == 0)
                {
                    GetColorFromParamBox(SecondInfoBox);
                }
                else
                {
                    InitNewSaturationFromBox();
                    InitColorPosition();
                }
                HexTable.Text = _tempColor.GetHexFromRGB();
            }
        }
        private void InitColorPosition()
        {
            System.Windows.Media.Color color = _tempColor.HSLtoRGB();
            (int x, int y)? cord = GetColorCord(color.R, color.G, color.B);

            if (cord is null) return;
            //Init cursor in this cords
            InitCordInSpec(cord.Value.x, cord.Value.y);
        }
        private void InitColorPosWithGivenColor(System.Windows.Media.Color color)
        {
            (int x, int y)? cord = GetColorCord(color.R, color.G, color.B);
            if (cord is null) return;
            InitCordInSpec(cord.Value.x, cord.Value.y);
        }
        private void InitNewSaturationFromBox()
        {
            CheckForAccaptableValues(SecondInfoBox, _maxSValue, _maxRGBValue);
            if (!int.TryParse(SecondInfoBox.Text, out int newValue)) return;

            double sat = newValue;
            sat /= _maxSValue;
            _tempColor.Saturation = sat;
            System.Windows.Media.Color newColor = _tempColor.HSLtoRGB();
            ChosenColorShow.Background = new SolidColorBrush(newColor);
            InitStartLuminanceGradientValue(newColor);
        }
        private void InitStartLuminanceGradientValue(System.Windows.Media.Color color)
        {
            LinearGradientBrush gradientBrush =
            (LinearGradientBrush)ValueBorder.Background;
            gradientBrush.GradientStops[0].Color = color;
        }
        private void ThirdParam_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckParamsForAmountOfDigits(sender as TextBox);

            if (sender is TextBox box &&
                ThirdInfoBox.IsFocused)
            {
                if (ChooseParamTypeBox.SelectedIndex == 0)
                {
                    GetColorFromParamBox(ThirdInfoBox);
                }
                else
                {
                    InitLuminanceByBoxCorrecting();
                    //Update Luminosity stick
                    ChangeLuminocityInTempColor();
                    InitLumCircleHeight(_pallete.TempL);
                }
                HexTable.Text = _tempColor.GetHexFromRGB();

            }
        }
        private void InitLuminanceByBoxCorrecting()
        {
            CheckForAccaptableValues(ThirdInfoBox, _maxLValue, _maxRGBValue);

            if (!int.TryParse(ThirdInfoBox.Text, out int newValue)) return;
            _pallete.TempL = newValue;
        }
        private void CheckParamsForAmountOfDigits(TextBox box)
        {
            int caretPosition = box.CaretIndex;

            string res = string.Empty;
            for (int i = 0; i < box.Text.Length; i++)
            {
                if (Char.IsDigit(box.Text[i]) &&
                    res.Count() < _maxAmountOfDiftsInColorParams)
                {
                    res += box.Text[i];
                }
            }
            box.Text = res;

            if (caretPosition > box.Text.Length)
            {
                caretPosition = box.Text.Length;
            }
            box.CaretIndex = caretPosition;
        }
        private void TextBoxFirstParam_LostFocus(object sender, EventArgs e)
        {
            CheckForAccaptableValues(sender as TextBox, _maxHValue, _maxRGBValue);
        }
        private void TextBoxSecondParam_LostFocus(object sender, EventArgs e)
        {
            CheckForAccaptableValues(sender as TextBox, _maxSValue, _maxRGBValue);
        }
        private void TextBoxThirdParam_LostFocus(object sender, EventArgs e)
        {
            CheckForAccaptableValues(sender as TextBox, _maxLValue, _maxRGBValue);
        }
        private void CheckForAccaptableValues(TextBox box, int HSLMaxValue, int RGBMaxValue)
        {
            CheckForZerosOnStart(box);
            CheckForMaxAmount(box, HSLMaxValue, RGBMaxValue);
        }
        private void CheckForMaxAmount(TextBox box, int maxHslAmount, int maxRgbAmount)
        {
            int rgbBoxIndex = 0;
            int hslBoxIndex = 1;
            string baseBoxParam = "0";

            bool ifSuccess = int.TryParse(box.Text, out int num);
            if (!ifSuccess) return;

            if (ChooseParamTypeBox.SelectedIndex == rgbBoxIndex &&
                num >= maxRgbAmount)
            {
                box.Text = maxRgbAmount.ToString();
            }
            else if (ChooseParamTypeBox.SelectedIndex == hslBoxIndex &&
                num > maxHslAmount)
            {
                box.Text = maxHslAmount.ToString();
            }
            else if (num < 0)
            {
                box.Text = baseBoxParam;
            }
        }
        private void CheckForZerosOnStart(TextBox box)
        {
            string res = string.Empty;
            const char zeroParam = '0';
            for (int i = 0; i < box.Text.Count(); i++)
            {
                if (Char.IsDigit(box.Text[i]) &&
                    box.Text[i] != zeroParam)
                {
                    for (int j = i; j < box.Text.Count(); j++)
                    {
                        res += box.Text[j];
                    }
                    box.Text = res;
                    return;
                }
            }
            box.Text = zeroParam.ToString();
        }
        private void ChangeLuminocityInTempColor()
        {
            if (!int.TryParse(ThirdInfoBox.Text, out int newValue)) return;
            if (newValue > _maxLValue || newValue < 0) return;

            int res = Math.Abs(((int)newValue) - _maxLValue);
            _pallete.TempL = newValue;

            UpdateLuminanceByValue();
        }
        private void UpdateLuminanceByValue()
        {
            const int LumDevider = 100;
            double lum = _pallete.TempL;
            lum /= LumDevider;
            _tempColor.Luminance = lum;
            System.Windows.Media.Color newColor = _tempColor.HSLtoRGB();

            ChosenColorShow.Background = new SolidColorBrush(newColor);
        }
        private void InitLumCircleHeight(int newLumValue)
        {
            const int doubleMultiplier = 2;
            _ifLumValueCanBeChanged = true;
            if (!_ifLumValueCanBeChanged) return;

            double onePartLumHeight = (ValueBorder.Height -
                _lumValueHeightDifferance * doubleMultiplier) / _maxLValue;

            int value = Math.Abs(((int)newLumValue) - _maxLValue);

            double posWithTempLuminocity = onePartLumHeight * value;
            Canvas.SetTop(valueDragElem, posWithTempLuminocity);
            _ifLumValueCanBeChanged = false;
        }
        private void PalletePanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _ifLumValueCanBeChanged = false;
            _ifCircleCanBeMoved = false;
        }
        private void FirstInfoBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckForZeroPreviewTextInput(FirstInfoBox, e);
        }
        private void SecondInfoBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckForZeroPreviewTextInput(SecondInfoBox, e);
        }
        private void ThirdInfoBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckForZeroPreviewTextInput(ThirdInfoBox, e);
        }
        private void CheckForZeroPreviewTextInput(TextBox box, TextCompositionEventArgs e)
        {
            const string zeroParam = "0";
            if (box.Text == zeroParam)
            {
                box.Text = string.Empty;
            }
            string newText = box.Text + e.Text;
            if (!IsTextAllowed(newText))
            {
                e.Handled = true;
            }
        }
        private bool IsTextAllowed(string text)
        {
            foreach (char c in text)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }
        private void SpecCanvas_LeftMouseDown(object sender, MouseEventArgs e)
        {
            const int hslDevider = 100;
            //get clicked point
            Point point = e.GetPosition(SpecCanvas);

            //Init circle position 
            InitCordInSpec((int)point.Y, (int)point.X);

            //Color on given position
            System.Windows.Media.Color color =
                GetColorAtPosition(coloeSpecter, (int)point.X, (int)point.Y);

            //Init gradient
            InitStartLuminanceGradientValue(GetColorWithThHighestLuminosity(color));

            //init rgb / hsl 
            _tempColor.RGBtoHSL(color);
            _tempColor.L = (double)_pallete.TempL / hslDevider;

            //Init showColor
            ChosenColorShow.Background = new SolidColorBrush(_tempColor.HSLtoRGB());

            //Fill boxes params
            InitColors();

            //init hex 
            HexTable.Text = _tempColor.GetHexFromRGB();

            //correct luminance column
            InitLumCircleHeight((int)(_tempColor.L * hslDevider));


            _ifCircleCanBeMoved = true;
            this.specDragEl = draggableButton;
            DraggableButton_PreViewMouseDown(draggableButton, e);
        }

        private void ColorSpecter_MouseMove(object sender, MouseEventArgs e)
        {
            MoveCircle(sender, e);
        }
        private void MoveCircle(object sender, MouseEventArgs e)
        {
            const int sizeCorrel = 1;
            const double locCorrel = 7.5;
            if (!_ifCircleCanBeMoved || specDragEl == null) return;
            _colorPoint = e.GetPosition(sender as IInputElement);
          
            if(_colorPoint.X < -200)
            {
                Console.WriteLine();
            }

            if (_colorPoint.X < 0)
            {
                _colorPoint.X = 0;
            }
            if (_colorPoint.X > coloeSpecter.Width - sizeCorrel)
            {
                _colorPoint.X = coloeSpecter.Width - sizeCorrel;
            }
            if (_colorPoint.Y < 0)
            {
                _colorPoint.Y = 0;
            }
            if (_colorPoint.Y > coloeSpecter.Height - sizeCorrel)
            {
                _colorPoint.Y = coloeSpecter.Height - sizeCorrel;
            }
            Canvas.SetTop(specDragEl, _colorPoint.Y - locCorrel);
            Canvas.SetLeft(specDragEl, _colorPoint.X - locCorrel);
            UpdateShowColor();
        }

    }
}

