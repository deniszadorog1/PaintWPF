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

using WpfAnimatedGif;

namespace PaintWPF.Other
{
    /// <summary>
    /// Логика взаимодействия для DoesntWork.xaml
    /// </summary>
    public partial class DoesntWork : Window
    {

        private Uri _falseMonkey = new Uri("pack://application:,,,/Gif/monkeyGif.gif");
        private Uri _trueMonkey = new Uri("pack://application:,,,/Gif/OtherMonkey.gif");
        private Uri _monkeyMusic = new Uri("pack://application:,,,/Gif/MonkeyMusic.gif");
        private Uri _tomato = new Uri("pack://application:,,,/Gif/Tomato.gif");
        public DoesntWork(string content, bool problemCheck)
        {
            InitializeComponent();
            ContentLB.Content = content;

            var image = new BitmapImage(_tomato /*problemCheck ? _trueMonkey : _falseMonkey*/);

            ImageBehavior.SetAnimatedSource(Gif, image);
        }
    }
}
