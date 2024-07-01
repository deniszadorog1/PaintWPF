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

namespace PaintWPF.CustomControls
{
    /// <summary>
    /// Логика взаимодействия для TextEditor.xaml
    /// </summary>
    public partial class TextEditor : UserControl
    {
        private readonly SolidColorBrush _mouseEnterBGColor =
            new SolidColorBrush(Color.FromArgb(240, 240, 240, 255));
        private readonly SolidColorBrush _mouseEnterBorderColor =
            new SolidColorBrush(Color.FromArgb(209, 209, 209, 255));
        private readonly SolidColorBrush _mouseClickBorderColor =
            new SolidColorBrush(Color.FromArgb(255, 103, 192, 255));
        private readonly Thickness _borderSize = new Thickness(2);

        private List<Button> _alignmetButs = new List<Button>();

        private readonly List<FontFamily> _fonts = Fonts.SystemFontFamilies.ToList();
        private readonly List<int> _fontsSizes = new List<int>()
        {
            8, 9, 10, 11, 12, 14, 16, 18, 
            20, 22, 24, 26, 28, 36, 48, 72
        };

        public FontFamily _chosenFont;
        public int _chosenFontSize;

        public TextEditor()
        {
            InitializeComponent();

            InitAlignmentList();
            InitComboboxes();
        }
        private void InitComboboxes()
        {
            for (int i = 0; i < _fonts.Count; i++)
            {
                Font.Items.Add(_fonts[i].ToString());
            }
            for (int i = 0; i < _fontsSizes.Count; i++)
            {
                FontSize.Items.Add(_fontsSizes[i]);
            }

            Font.SelectedIndex = 51;
            FontSize.SelectedIndex = 5;

            _chosenFont = _fonts[Font.SelectedIndex];
            _chosenFontSize =  _fontsSizes[FontSize.SelectedIndex];
        }
        public void InitAlignmentList()
        {
            _alignmetButs.Clear();
            _alignmetButs.Add(LeftPos);
            _alignmetButs.Add(CenterPos);
            _alignmetButs.Add(RightPos);
        }
        private void ButtonsTextType_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Button but &&
               but.BorderBrush != _mouseClickBorderColor)
            {
                but.BorderBrush = _mouseEnterBorderColor;
                but.Background = _mouseEnterBGColor;
            }
        }
        private void TextAlign_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Button but &&
              but.BorderBrush != _mouseClickBorderColor)
            {
                but.BorderBrush = _mouseEnterBorderColor;
                but.Background = _mouseEnterBGColor;
            }
        }
        private void Buttons_MouseLeave(object sender, EventArgs e)
        {
            if (sender is Button but &&
               but.BorderBrush != _mouseClickBorderColor)
            {
                but.BorderBrush = new SolidColorBrush(Colors.White);
                but.Background = new SolidColorBrush(Colors.White);
            }
        }
        private void TextType_Click(object sender, EventArgs e)
        {
            if (sender is Button but)
            {
                but.BorderThickness = but.BorderThickness == _borderSize ?
                new Thickness(0) : _borderSize;

                but.BorderBrush = but.BorderBrush == _mouseClickBorderColor ?
                    new SolidColorBrush(Colors.White) : _mouseClickBorderColor;
            }
        }
        private void TextAlignment_Click(object sender, EventArgs e)
        {
            ClearAlignmentUIThings();
            if (sender is Button but)
            {
                but.BorderBrush = _mouseClickBorderColor;

                but.BorderThickness = but.BorderThickness == _borderSize ?
                      new Thickness(0) : _borderSize;
            }
        }
        private void ClearAlignmentUIThings()
        {
            for (int i = 0; i < _alignmetButs.Count; i++)
            {
                ((Button)_alignmetButs[i]).BorderBrush = new SolidColorBrush(Colors.White);
                ((Button)_alignmetButs[i]).BorderThickness = new Thickness(0);
                ((Button)_alignmetButs[i]).Background = new SolidColorBrush(Colors.White);
            }
        }

    }
}
