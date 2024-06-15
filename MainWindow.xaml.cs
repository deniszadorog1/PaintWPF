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
using System.Windows.Navigation;
using System.Windows.Shapes;

using PaintWPF.Models;

namespace PaintWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainPaint _main = new MainPaint();

        public MainWindow()
        {
            InitializeComponent();
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

    }
}
