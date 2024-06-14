using System;
using System.Collections.Generic;
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

namespace PaintWPF
{
    /// <summary>
    /// Логика взаимодействия для Pallete.xaml
    /// </summary>
    public partial class Pallete : Window
    {
        public Pallete()
        {
            InitializeComponent();
            //InitBorderForColorCpectre();
        }
        public void InitBorderForColorCpectre()
        {
            Border border = new Border
            {
                Width = 100,
                Height = 100,
                CornerRadius = new CornerRadius(50),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                ClipToBounds = true
            };

            // Создаем Ellipse
            Ellipse ellipse = new Ellipse
            {
                Width = 100,
                Height = 100
            };

            // Загружаем изображение
            ImageBrush imageBrush = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/Pallete/ColorSpectre.png")),
                Stretch = Stretch.Fill
            };

            ellipse.Fill = imageBrush;

            // Добавляем Ellipse в Border
            border.Child = ellipse;

            // Добавляем Border в Grid
            qwe.Children.Add(border);
        }
        private void RGBTextBox_TextChanged(object sender, EventArgs e)
        {
            const int maxAmountOfSymbols = 8;
            if (sender is TextBox box)
            {
                if(box.Text.Length == 0)
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
                    box.Text =  box.Text.Remove(box.Text.Length - 1);
                    box.CaretIndex = box.Text.Length;
                    return;
                }
            }
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

            greyCircle.Stroke = PalletePanel.Background;
            greyCircle.StrokeThickness = 1;

            greyCircle.Margin = new Thickness(-10);

            button.Content = greyCircle;
        }
    }
}

