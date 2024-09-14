using Microsoft.Win32;
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
using System.Windows.Shapes;

namespace PaintWPF.Other
{
    /// <summary>
    /// Логика взаимодействия для SaveOffer.xaml
    /// </summary>
    public partial class SaveOffer : Window
    {
        public string _lastSavePath;
        private PngBitmapEncoder _toSave;
        public bool _ifClear = true;
        public SaveOffer(string lastSavePath, PngBitmapEncoder toSave)
        {
            _lastSavePath = lastSavePath;
            _toSave = toSave;

            InitializeComponent();
        }
        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            _ifClear = false;
            Close();
        }
        private void NotSaveBut_Click(object sender, RoutedEventArgs e)
        {
            _ifClear = true;
            Close();
        }
        private void SaveBut_Click(object sender, RoutedEventArgs e)
        {
            if (_lastSavePath is null)
            {
                SaveFileNew();
                Close();
                return;
            }
            FastSave();
            Close();
        }
        private void FastSave()
        {
            using (FileStream fileStream = new FileStream(_lastSavePath, FileMode.Create))
            {
                _toSave.Save(fileStream);
            }
            _ifClear = true;
        }
        private void SaveFileNew()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image|*.png";
            saveFileDialog.FileName = "IHateThisTask.png";
            if (saveFileDialog.ShowDialog() == true)
            {
                using (FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    _toSave.Save(fileStream);
                }
                _ifClear = true;
                return;
            }
            _ifClear = false;  
        }
    }
}
