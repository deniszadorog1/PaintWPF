using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
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
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using Image = System.Windows.Controls.Image;

namespace PaintWPF
{
    /// <summary>
    /// Логика взаимодействия для ImageSettings.xaml
    /// </summary>
    public partial class ImageSettings : Window
    {
        private Canvas _drawingCanvas;
        private string _lastDrawImgSave;

        private const int _weightDivider = 1024;
        private readonly List<string> _weightValues = new List<string>()
        {
            "Byte",
            "KB",
            "MB",
            "GB"
        };
        private int _finalWeightValueIndex = 0;
        private DateTime? _lastSaveDate;
        private int _dpi = 96;
        public ImageSettings(Canvas drawingCanvas, string lastSavePath, DateTime? lastSaveDate)
        {
            _drawingCanvas = drawingCanvas;
            _lastDrawImgSave = lastSavePath;
            _lastSaveDate = lastSaveDate;

            InitializeComponent();

            InitBaseParams();
        }
        private void InitBaseParams()
        {
            //init DrawingCanvas Size in pixels
            DrawingHeight.Content = _drawingCanvas.Height;
            DrawingWidth.Content = _drawingCanvas.Width;

            //Init Image wieght
            ImageWeight.Content = GetImageWeightInString() + 
                (_finalWeightValueIndex == -1 ? "" : " " +  _weightValues[_finalWeightValueIndex]);

            LastSave.Content = GetLastSaveDate();

            Dpi.Content = _dpi.ToString() + " точек на дюйм";
        }
        private string GetLastSaveDate()
        {
            return _lastSaveDate is null ? LastSave.Content.ToString() : 
            _lastSaveDate.Value.Day + "." + _lastSaveDate.Value.Month + "." + _lastSaveDate.Value.Year + " " +  
                _lastSaveDate.Value.Hour + ":" + _lastSaveDate.Value.Minute;
        }
        private string GetImageWeightInString()
        {
            double res = GetImageWeight();
            ConvertBytesIntoAnoutherValue(ref res);
            if (res == -1) _finalWeightValueIndex = -1;
            return res == -1 ? ImageWeight.Content.ToString() : (Math.Round(res, 3)).ToString();
        }
        private double ConvertBytesIntoAnoutherValue(ref double weight)
        {
            if (weight <= _weightDivider) return weight;
            do
            {
                weight /= _weightDivider;
                _finalWeightValueIndex++;
            } while (weight >= _weightDivider);
            return weight;
        }
        private double GetImageWeight()
        {
            if (_lastDrawImgSave == string.Empty) return -1;
            Image image = new Image();
            BitmapImage bitmap = new BitmapImage(new Uri(_lastDrawImgSave));
            image.Source = bitmap;

            // Размер изображения в байтах = ширина * высота * (глубина цвета / 8)
            long size = bitmap.PixelWidth * bitmap.PixelHeight * (bitmap.Format.BitsPerPixel / 8);
            return size;
        }
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private bool IfSizeParamsIsNull()
        {
            return DrawingWidth is null || DrawingHeight is null;
        }
        private void SantiRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (IfSizeParamsIsNull()) return;
            RadioButton but = sender as RadioButton;

            InitDPI();
            DrawingWidth.Content = Math.Round(ConvertPixelsToCentimetersX(_drawingCanvas.Width), 3);
            DrawingHeight.Content = Math.Round(ConvertPixelsToCentimetersY(_drawingCanvas.Height), 3);
        }
        double ConvertInchesToCentimeters(double inches)
        {
            return inches * 2.54;
        }
        double ConvertPixelsToCentimetersX(double pixels)
        {
            double inches = ConvertPixelsToInchesX(pixels);
            return ConvertInchesToCentimeters(inches);
        }

        double ConvertPixelsToCentimetersY(double pixels)
        {
            double inches = ConvertPixelsToInchesY(pixels);
            return ConvertInchesToCentimeters(inches);
        }

        private void PixelsRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (IfSizeParamsIsNull()) return;
            RadioButton but = sender as RadioButton;

            DrawingWidth.Content = _drawingCanvas.Width;
            DrawingHeight.Content = _drawingCanvas.Height;
        }
        private void InchRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (IfSizeParamsIsNull()) return;
            RadioButton but = sender as RadioButton;

            InitDPI();
            DrawingWidth.Content = Math.Round(ConvertPixelsToInchesX(_drawingCanvas.Width), 3);
            DrawingHeight.Content = Math.Round(ConvertPixelsToInchesY(_drawingCanvas.Height), 3);
        }
        private double ConvertPixelsToInchesX(double pixels)
        {
            return pixels / dpiX;
        }
        private double ConvertPixelsToInchesY(double pixels)
        {
            return pixels / dpiY;
        }
        private double dpiX, dpiY;
        private void InitDPI()
        {
            var source = PresentationSource.FromVisual(Application.Current.MainWindow);
            if (source != null)
            {
                dpiX = _dpi * source.CompositionTarget.TransformToDevice.M11;
                dpiY = _dpi * source.CompositionTarget.TransformToDevice.M22;
            }
            else
            {
                dpiX = _dpi;
                dpiY = _dpi;
            }
        }
    }
}
