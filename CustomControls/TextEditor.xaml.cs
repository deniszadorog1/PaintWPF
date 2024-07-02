using System;
using System.Collections.Generic;
using System.Drawing;
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
using Xceed.Wpf.Toolkit;
using Color = System.Windows.Media.Color;
using FontFamily = System.Windows.Media.FontFamily;
using RichTextBox = System.Windows.Controls.RichTextBox;

namespace PaintWPF.CustomControls
{
    /// <summary>
    /// Логика взаимодействия для TextEditor.xaml
    /// </summary>
    public partial class TextEditor : UserControl
    {
        private MainPaint _paintObj;

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
        public SolidColorBrush textBg = null;

        private RichTextBox _textBox = null;

        public TextEditor(MainPaint paint, RichTextBox textBox)
        {
            _textBox = textBox;
            if (!(_textBox is null))
            {
                _textBox.TextChanged += TextBox_TextChanged;
            }
            _paintObj = paint;

            InitializeComponent();

            InitAlignmentList();
            InitComboboxes();
        }
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            if (_textBox == null) return;
            TextSelection ts = _textBox.Selection;
            if (ts != null)
            {
                ts.ApplyPropertyValue(TextElement.ForegroundProperty, _paintObj.FirstColor);
            }
            _textBox.Focus();
        }

        private void InitComboboxes()
        {
            for (int i = 0; i < _fonts.Count; i++)
            {
                FontFamaly.Items.Add(_fonts[i].ToString());
            }
            for (int i = 0; i < _fontsSizes.Count; i++)
            {
                FontSize.Items.Add(_fontsSizes[i]);
            }

            FontFamaly.SelectedIndex = 51;
            FontSize.SelectedIndex = 5;

            _chosenFont = _fonts[FontFamaly.SelectedIndex];
            _chosenFontSize = _fontsSizes[FontSize.SelectedIndex];
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

                InitTextType(but, but.BorderBrush == _mouseClickBorderColor);
            }
        }
        public void InitTextType(Button but, bool ifActive)
        {
            if (but.Name == "Bold")
            {
                BoldingText();
            }
            else if (but.Name == "Italics")
            {
                IntalicingText();
            }
            else if (but.Name == "Underline")
            {
                UnerliningText();
            }
            else if (but.Name == "Crossed")
            {
                CrossingText();
            }
        }
        private void CrossingText()
        {
            if (_textBox == null) return;
            TextSelection ts = _textBox.Selection;
            if (ts != null)
            {
                bool isStrikethrough = false;
                object value = ts.GetPropertyValue(Inline.TextDecorationsProperty);
                if (value != DependencyProperty.UnsetValue && value is TextDecorationCollection decorations)
                {
                    isStrikethrough = decorations.Any(td => td.Location == TextDecorationLocation.Strikethrough);
                }
                if (!isStrikethrough)
                {
                    TextDecorationCollection strikethrough = new TextDecorationCollection();
                    strikethrough.Add(TextDecorations.Strikethrough.First());
                    ts.ApplyPropertyValue(Inline.TextDecorationsProperty, strikethrough);
                }
                else
                {
                    ts.ApplyPropertyValue(Inline.TextDecorationsProperty, null);
                }
            }
            _textBox.Focus();
        }
        private void BoldingText()
        {
            //EditingCommands.ToggleBold.Execute(null, _textBox);
            if (_textBox == null) return;
            TextSelection ts = _textBox.Selection;
            if (ts != null)
            {
                var fontWeight = ts.GetPropertyValue(TextElement.FontWeightProperty);
                if (fontWeight == DependencyProperty.UnsetValue || fontWeight.ToString() != "Bold")
                {
                    ts.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                }
                else
                {
                    ts.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
                }
            }
            _textBox.Focus();
        }
        private void IntalicingText()
        {
            if (_textBox == null) return;
            TextSelection ts = _textBox.Selection;
            if (ts != null)
            {
                var fontStyle = ts.GetPropertyValue(TextElement.FontStyleProperty);
                if (fontStyle == DependencyProperty.UnsetValue || fontStyle.ToString() != "Italic")
                {
                    ts.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);
                }
                else
                {
                    ts.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Normal);
                }
            }
            _textBox.Focus();
        }
        private void UnerliningText()
        {
            if (_textBox == null) return;

            TextSelection ts = _textBox.Selection;
            if (ts != null)
            {
                bool isUnderlined = false;
                object value = ts.GetPropertyValue(Inline.TextDecorationsProperty);
                if (value != DependencyProperty.UnsetValue && value is TextDecorationCollection decorations)
                {
                    isUnderlined = decorations.Any(td => td.Location == TextDecorationLocation.Underline);
                }

                if (!isUnderlined)
                {
                    TextDecorationCollection underline = new TextDecorationCollection();
                    underline.Add(TextDecorations.Underline.First());
                    ts.ApplyPropertyValue(Inline.TextDecorationsProperty, underline);
                }
                else
                {
                    ts.ApplyPropertyValue(Inline.TextDecorationsProperty, null);
                }
            }
            _textBox.Focus();
        }
        private void TextAlignment_Click(object sender, EventArgs e)
        {
            ClearAlignmentUIThings();
            if (sender is Button but)
            {
                but.BorderBrush = _mouseClickBorderColor;
                but.BorderThickness = but.BorderThickness == _borderSize ?
                      new Thickness(0) : _borderSize;

                SetTextAlignMent(but.Name);
            }
        }
        private void SetTextAlignMent(string butName)
        {
            if (butName == "LeftPos")
            {
                SetTextAlignment(TextAlignment.Left);
            }
            else if (butName == "CenterPos")
            {
                SetTextAlignment(TextAlignment.Center);
            }
            else if (butName == "RightPos")
            {
                SetTextAlignment(TextAlignment.Right);
            }
        }
        private void SetTextAlignment(TextAlignment textAlign)
        {
            if (_textBox == null) return;

            TextSelection selection = _textBox.Selection;
            if (selection != null && !selection.IsEmpty)
            {
                // Получение текущего параграфа и установка выравнивания
                Paragraph paragraph = selection.Start.Paragraph;
                if (paragraph != null)
                {
                    paragraph.TextAlignment = textAlign;
                }
            }

            _textBox.Focus();
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
        private void FontFamaly_SelectionChanged(object sender, EventArgs e)
        {
            if (_textBox is null) return;
            _chosenFont = _fonts[FontFamaly.SelectedIndex];

            FontFamalyChanged();
        }
        private void FontFamalyChanged()
        {
            TextRange selectionTextRange = _textBox.Selection;
            if (!selectionTextRange.IsEmpty)
            {
                selectionTextRange.ApplyPropertyValue(TextElement.FontFamilyProperty, _chosenFont);
            }
            else
            {
                if (_textBox == null) return;
                TextSelection ts = _textBox.Selection;
                if (ts != null)
                    selectionTextRange.ApplyPropertyValue(TextElement.FontFamilyProperty, _chosenFont);
                _textBox.Focus();
            }
        }

        private void FontSize_SelctionChanged(object sender, EventArgs e)
        {
            if (_textBox is null) return;
            _chosenFontSize = _fontsSizes[FontSize.SelectedIndex];

            ChangeFontSize(_chosenFontSize);
        }
        private void ChangeFontSize(double fontSize)
        {
            TextRange selectionTextRange = _textBox.Selection;
            if (!selectionTextRange.IsEmpty)
            {
                selectionTextRange.ApplyPropertyValue(TextElement.FontSizeProperty, fontSize);
            }
            else
            {
                if (_textBox == null) return;
                TextSelection ts = _textBox.Selection;
                if (ts != null)
                    selectionTextRange.ApplyPropertyValue(TextElement.FontSizeProperty, fontSize);
                _textBox.Focus();
            }
        }
        private void FillBg_Checked(object sender, EventArgs e)
        {
            if (_textBox is null) return;
            textBg = _paintObj.SecondColor;
            _textBox.Background = textBg;
        }
        private void FillBgUnChecked(object sender, EventArgs e)
        {
            if (_textBox is null) return;
            textBg = new SolidColorBrush(Colors.Transparent);
            _textBox.Background = textBg;
        }

    }
}
