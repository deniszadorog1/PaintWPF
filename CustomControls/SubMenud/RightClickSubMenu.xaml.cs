using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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

using PaintWPF.Models.Enums;

namespace PaintWPF.CustomControls.SubMenu
{
    /// <summary>
    /// Логика взаимодействия для RightClickSubMenu.xaml
    /// </summary>
    public partial class RightClickSubMenu : UserControl
    {
        public  List<SubMenuItems> _items;

        private const string _turnIn180 = "Повернуть на 180";
        private string _turnIn180Path = "";

        private const string _flipInVertical = "Отразить по вертикали";
        private string _flipInVerticalPath = "";

        private const string _flipInHorizontal = "Отразить по горизонтали";
        private string _flipInHorizontalPath = "";

        public RightClickSubMenu(List<SubMenuItems> items)
        {
            _items = items;
            InitializeComponent();

            InitImagesPaths();
            InitItems();
        }

        private const int _menuWidthCor = 5;
        private void InitItems()
        {
            const int multAdd = 1;
            const int multMultiplier = 5;
            SubMenuElement newElem = new SubMenuElement();
            double menuHeight = newElem.Height * _items.Count + multAdd + _items.Count * multMultiplier;
            for (int i = 0; i < _items.Count; i++)
            {
                newElem = new SubMenuElement();
               (string path, string text) itemParams = GetItemParams(_items[i]);
                newElem.ItemText.Content = itemParams.text;
                GetImageFromFile(newElem.ItemImage, itemParams.path);
               
                SubMenu.Items.Add(newElem);
            }
            InitSize(menuHeight, newElem.Width);
        }

        private void InitSize(double height, double width)
        {
            const int heightAdd = 50;
            SubMenu.Height = height + heightAdd;
            Height = height;
            SubMenu.Width = width;
            Width = width;
        }

        private void GetImageFromFile(Image img, string path)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
            bitmap.EndInit();
    
            img.Source = bitmap;
        }

        private (string imgPath, string text) GetItemParams(SubMenuItems item)
        {
            return item == SubMenuItems.TurnIn180 ? (_turnIn180Path, _turnIn180) :
                item == SubMenuItems.FlipInVertical ? (_flipInVerticalPath, _flipInVertical) :
                (_flipInHorizontalPath, _flipInHorizontal);
        }

        private void InitImagesPaths()
        {
            DirectoryInfo info = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            string startdir = info.Parent.Parent.FullName;
            string imgPath = System.IO.Path.Combine(startdir, "Images");
            string clickPath = System.IO.Path.Combine(imgPath, "ClickMenu");

            _turnIn180Path = System.IO.Path.Combine(clickPath, "TurnIn180.png");
            _flipInVerticalPath = System.IO.Path.Combine(clickPath, "FlipVertical.png");
            _flipInHorizontalPath = System.IO.Path.Combine(clickPath, "FlipHorizontal.png");
        }
    }
}
