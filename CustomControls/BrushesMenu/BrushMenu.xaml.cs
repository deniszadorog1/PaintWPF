using PaintWPF.Models.Enums;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PaintWPF.CustomControls.BrushesMenu
{
    public partial class BrushMenu : UserControl
    {
        private string _usualBrushString = "Кисть";
        private string _usualPath;//

        private string _calligraphyBrushString = "Калиграфическая кисть";
        private string _calligraphyPath;

        private string _fountainPenString = "Перьевая ручка";
        private string _fountainPenPath;

        private string _sprayString = "Распылитель";
        private string _sprayPath;

        private string _oilPaintBrushString = "Кисть для масляных красок";
        private string _oilPaintPath; // 

        private string _colorPencilString = "Цветной карандаш";
        private string _colorPencilPath;//

        private string _markerString = "Маркер";
        private string _markerPath;

        private string _texturePencilString = "Текстурный карандаш";
        private string _texturePencilPath;//

        private string _watercolorBrushString = "Киcть для акварели";
        private string _watercolorPath;//

        private List<(string, Image, string)> _menuItemsParts = new List<(string, Image, string)>();

        public BrushMenu()
        {
            InitializeComponent();

            InitImagePathes();
            InitParts();
            InitBrushes();
        }
        private void InitParts()
        {
            _menuItemsParts.Add((_usualBrushString, GetImageFromFile(_usualPath), "UsualBrush"));

            _menuItemsParts.Add((_calligraphyBrushString, GetImageFromFile(_calligraphyPath), "CalligraphyBrush"));
            _menuItemsParts.Add((_fountainPenString, GetImageFromFile(_fountainPenPath), "FountainPen"));
            _menuItemsParts.Add((_sprayString, GetImageFromFile(_sprayPath), "Spray"));
            _menuItemsParts.Add((_oilPaintBrushString, GetImageFromFile(_oilPaintPath), "OilPaintBrush"));
            _menuItemsParts.Add((_colorPencilString, GetImageFromFile(_colorPencilPath), "ColorPencil"));
            _menuItemsParts.Add((_markerString, GetImageFromFile(_markerPath), "Marker"));
            _menuItemsParts.Add((_texturePencilString, GetImageFromFile(_texturePencilPath), "TexturePencil"));
            _menuItemsParts.Add((_watercolorBrushString, GetImageFromFile(_watercolorPath), "WatercolorBrush"));
        }
        private Image GetImageFromFile(string path)
        {
            if (path is null) return null;
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
            bitmap.EndInit();

            Image img = new Image()
            {
                Source = bitmap
            };
            return img;
        }
        private void InitImagePathes()
        {
            DirectoryInfo info = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            string startdir = info.Parent.Parent.FullName;
            string imgPath = System.IO.Path.Combine(startdir, "Images");
            string brushPath = System.IO.Path.Combine(imgPath, "Brushes");

            _usualPath = System.IO.Path.Combine(brushPath, "ColoredBrush.png");
            _calligraphyPath = System.IO.Path.Combine(brushPath, "OilBrush.png");
            _fountainPenPath = System.IO.Path.Combine(brushPath, "ColoredBrush.png");
            _sprayPath = System.IO.Path.Combine(brushPath, "OilBrush.png");
            _oilPaintPath = System.IO.Path.Combine(brushPath, "ColoredBrush.png");
            _colorPencilPath = System.IO.Path.Combine(brushPath, "OilBrush.png");
            _markerPath = System.IO.Path.Combine(brushPath, "ColoredBrush.png");
            _texturePencilPath = System.IO.Path.Combine(brushPath, "OilBrush.png");
            _watercolorPath = System.IO.Path.Combine(brushPath, "ColoredBrush.png");
        }
        private void InitBrushes()
        {
            for (int i = 0; i < _menuItemsParts.Count; i++)
            {
                BrushMenuItem item = GetBrushMenuItem(_menuItemsParts[i].Item1,
                    _menuItemsParts[i].Item2, _menuItemsParts[i].Item3);
                BrushesListBox.Items.Add(item);
            }
        }
        private BrushMenuItem GetBrushMenuItem(string itemStr, Image itemImage, string itemName)
        {
            BrushMenuItem res = new BrushMenuItem();

            res.BrushName.Content = itemStr;
            if (!(itemImage is null))
            {
                res.BrushImage.Source = itemImage.Source;
            }
            res.Name = itemName;
            return res;
        }
    }
}
