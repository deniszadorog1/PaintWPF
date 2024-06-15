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

        public Pallete(PalleteModel pallete)
        {
            _pallete = pallete;

            InitializeComponent();

            Canvas.SetTop(draggableButton, 20);
            Canvas.SetLeft(draggableButton, 20);

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
                        Button but = ((Button)UserColorPanel.Children[tempChildrenIndex]);
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

        private UIElement dragEl = null;
        private Point offset;

        private void draggableButton_PreViewMouseDown(object sender, MouseEventArgs e)
        {
            /*            Point relativePoint = coloeSpecter.TransformToAncestor(this)
                                                     .Transform(new Point(0, 0));

                        Point pos = draggableButton.TransformToAncestor(this).Transform(new Point(0, 0));

                        // Рассчитываем центр кнопки
                        double centerX = pos.X + (draggableButton.ActualWidth / 2);
                        double centerY = pos.Y + (draggableButton.ActualHeight / 2);*/


            /*            if (centerX > relativePoint.X &&
                        centerX <= relativePoint.X + coloeSpecter.Width &&
                        centerY > relativePoint.Y &&
                        centerY <= relativePoint.Y + coloeSpecter.Height)
                        {*/
            this.dragEl = sender as UIElement;
            offset = e.GetPosition(this.CanvasMain);
            offset.Y -= Canvas.GetTop(dragEl);
            offset.X -= Canvas.GetLeft(dragEl);
            CanvasMain.CaptureMouse();
            //}
        }
        private void CanvasMain_PreViewMouseMove(object sender, MouseEventArgs e)
        {
            Point buttonPosRelativeToAncestor = draggableButton.TransformToAncestor(this).Transform(new Point(0, 0));

            Point imagePosRelativeToAncestor = coloeSpecter.TransformToAncestor(this).Transform(new Point(0, 0));

            double relativeX = buttonPosRelativeToAncestor.X - imagePosRelativeToAncestor.X;
            double relativeY = buttonPosRelativeToAncestor.Y - imagePosRelativeToAncestor.Y;

            double centerX = relativeX + (draggableButton.ActualWidth / 2);
            double centerY = relativeY + (draggableButton.ActualHeight / 2);

            if (dragEl == null) return;
            var position = e.GetPosition(sender as IInputElement);

            if (position.X < 0)
            {
                position.X = 5;
            }
            if (position.X > coloeSpecter.Width)
            {
                position.X = coloeSpecter.Width - 5;
            }
            if (position.Y < 0)
            {
                position.Y = 5;
            }
            if (position.Y > coloeSpecter.Height)
            {
                position.Y = coloeSpecter.Height;
            }

            Canvas.SetTop(dragEl, position.Y - offset.Y);
            Canvas.SetLeft(dragEl, position.X - offset.X);
            //GetColorByCord(position);
        }
        private void CanvasMain_PreViewMouseUp(object sender, MouseEventArgs e)
        {
            dragEl = null;
            CanvasMain.ReleaseMouseCapture();
        }
        private void asd_Click(object sender, EventArgs e)
        {
            Console.WriteLine();
        }

        public Color GetColorByCord(Point point)
        {
            using (Bitmap bitmap = new Bitmap(GetDirToPallete()))
            {
                return bitmap.GetPixel((int)point.X, (int)point.Y);
            }
        }
        public string GetDirToPallete()
        {
            /*        DirectoryInfo baseDirectoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                    string imageDirectory = System.IO.Path.Combine(baseDirectoryInfo.Parent.Parent.FullName, "Images");
                    string imagePath = System.IO.Path.Combine(imageDirectory, "ColorSpectre.png");
                    return imagePath;*/
            return "";

        }

        public (int, int)? GetColorCord(int r, int g, int b)
        {
            Color targetColor = Color.FromArgb(r, g, b);

            using (Bitmap bitmap = new Bitmap("Images/ColorSpectre.png"))
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        Color pixelColor = bitmap.GetPixel(x, y);
                        if (pixelColor.ToArgb() == targetColor.ToArgb())
                        {
                            return (x, y);
                        }
                    }
                }
            }
            return null;
        }

    }
}

