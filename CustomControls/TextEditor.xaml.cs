using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using PaintWPF.Models;
using PaintWPF.Models.Enums;
using Xceed.Wpf.Toolkit;
using static System.Net.Mime.MediaTypeNames;
using Color = System.Windows.Media.Color;
using FontFamily = System.Windows.Media.FontFamily;
using RichTextBox = System.Windows.Controls.RichTextBox;
using Size = System.Windows.Size;

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

        public SolidColorBrush textBg = null;

        public RichTextBox _textBox = null;
        private Selection _textSizeChanging;

        public TextBoxSetting _textSettings = new TextBoxSetting("AskAboutIt");

        public struct TextBoxSetting
        {
            public FontFamily ChosenFont { get; set; }
            public int FontSize { get; set; }
            public bool IfBald { get; set; }
            public bool IfItalic { get; set; }
            public bool IfCrossed { get; set; }
            public bool IfUnderLined { get; set; }
            public TextAlignment ParPosition { get; set; }
            public bool IfFill { get; set; }

            public TextBoxSetting(bool ifBald, bool ifItalic,
                bool ifCrossed, bool ifUnderLined,
                TextAlignment pos, FontFamily font, int fontSize, bool ifFill)
            {
                ChosenFont = font;
                FontSize = fontSize;
                IfBald = ifBald;
                IfItalic = ifItalic;
                IfCrossed = ifCrossed;
                IfUnderLined = ifUnderLined;
                ParPosition = pos;
                IfFill = ifFill;
            }
            public TextBoxSetting(object smth)
            {
                ChosenFont = new FontFamily("Segoe UI");
                FontSize = 14;
                IfBald = false;
                IfItalic = false;
                IfCrossed = false;
                IfUnderLined = false;
                ParPosition = TextAlignment.Left;
                IfFill = false;
            }
        }
        public TextEditor(MainPaint paint, Selection textSizeChanging)
        {
            InitTextBox(textSizeChanging);

            _paintObj = paint;

            InitializeComponent();

            InitAlignmentList();
            InitComboboxes();
        }
        public void InitForNewTextBox(Selection select)
        {
            InitTextBox(select);
            _textBox.BorderThickness = new Thickness(0);

            BoldingText();
            IntalicingText();
            CrossingText();
            UnderliningText();

            SetTextAlignment(_textSettings.ParPosition);
            MakeStartFilling();
            FontFamalyChanged();
            ChangeFontSize();
            InitTextBoxHeight();

            InitForeColor();

            _textBox.Focus();
            
        }
        private void MakeStartFilling()
        {
            _textSettings.IfFill = (bool)Fill.IsChecked;
            if (_textSettings.IfFill)
            {
                textBg = _paintObj.SecondColor;
                _textBox.Background = textBg;
                return;
            }
            textBg = new SolidColorBrush(Colors.Transparent);
            _textBox.Background = textBg;
        }
        private void InitTextBox(Selection textSizeChanging)
        {
            _textSizeChanging = textSizeChanging;
            _textBox = _textSizeChanging == null ? null : _textSizeChanging.GetRichTextBoxObject();

            if (!(_textBox is null))
            {
                _textBox.BorderThickness = new Thickness(0.5);

                _textBox.TextChanged -= TextBox_TextChanged;
                _textBox.TextChanged += TextBox_TextChanged;
            }
        }
        private void InitTextBoxHeight()
        {
            const string checkText = "A";

            Size newSize = GetFontSize(checkText, _textSettings.ChosenFont, _textSettings.FontSize,
                new System.Windows.FontStyle(), new System.Windows.FontWeight());

            _textBox.Height = newSize.Height;
            InitSizeForSelectionBorder(newSize);
        }
        private void InitSizeForSelectionBorder(Size size)
        {
            const int borderSelDif = 15;
            const int selectionDif = 10;

            _textSizeChanging.Height = size.Height + borderSelDif;
            _textSizeChanging.SelectionBorder.Height = size.Height + borderSelDif;
            _textSizeChanging.SelectCan.Height = size.Height + selectionDif;
        }
        private Size GetFontSize(string text, FontFamily fontFamily, double fontSize, System.Windows.FontStyle fontStyle, FontWeight fontWeight)
        {
            FormattedText formattedText = new FormattedText(
               text,
               CultureInfo.CurrentCulture,
               FlowDirection.LeftToRight,
               new Typeface(fontFamily, fontStyle, fontWeight, FontStretches.Normal),
               fontSize,
               System.Windows.Media.Brushes.Black,
               new NumberSubstitution(),
               1.0);

            return new Size(formattedText.Width, formattedText.Height);
        }

        private bool _loopCheck = true;
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            if (!_loopCheck) return;
            if (_textBox == null) return;

            InitForeColor();
        }
        private void InitForeColor()
        {
            TextSelection ts = _textBox.Selection;
            if (ts != null)
            {
                ts.ApplyPropertyValue(TextElement.ForegroundProperty, _paintObj.FirstColor);
            }
            _textBox.Focus();
        }
        public void ChangeTextColor()
        {
            if (_textBox is null) return;
            TextRange selectionTextRange = _textBox.Selection;
            if (selectionTextRange != null)
            {
                selectionTextRange.ApplyPropertyValue(TextElement.ForegroundProperty, _paintObj.FirstColor);
            }
            else
            {
                if (_textBox == null) return;
                TextSelection ts = _textBox.Selection;
                if (ts != null)
                    ts.ApplyPropertyValue(TextElement.ForegroundProperty, _paintObj.FirstColor);
            }
        }
        /*
              TextRange selectionTextRange = _textBox.Selection;
            if (!selectionTextRange.IsEmpty)
            {
                selectionTextRange.ApplyPropertyValue(TextElement.FontSizeProperty, (double)_textSettings.FontSize);
            }
            else
            {
                if (_textBox == null) return;
                TextSelection ts = _textBox.Selection;
                if (ts != null)
                    selectionTextRange.ApplyPropertyValue(TextElement.FontSizeProperty, (double)_textSettings.FontSize);
                _textBox.Focus();
            }
         */
        private bool IfOnlyOneSymbolInTextBox()
        {
            TextPointer start = _textBox.Document.ContentStart;
            TextPointer end = _textBox.Document.ContentEnd;

            string allText = new TextRange(start, end).Text;

            return !string.IsNullOrEmpty(allText.Trim());
        }
        private (char?, TextPointer) GetAddableCharAndPosition()
        {
            TextPointer caretPos = _textBox.CaretPosition;

            TextPointer position = caretPos.GetPositionAtOffset(-1, LogicalDirection.Backward);

            if (position != null)
            {
                string newText = new TextRange(position, caretPos).Text;

                if (!string.IsNullOrEmpty(newText) && newText.Length == 1)
                {
                    char newChar = newText.First();// [0];
                    return (newChar, position);
                }
            }
            return (null, null);
        }
        private void InitComboboxes()
        {
            const int fontIndex = 51;
            const int sizeIndex = 5;
            for (int i = 0; i < _fonts.Count; i++)
            {
                Label fontName = new Label()
                {
                    Content = _fonts[i].ToString(),
                    FontFamily = _fonts[i]
                };
                FontFamaly.Items.Add(fontName);
            }
            for (int i = 0; i < _fontsSizes.Count; i++)
            {
                FontSize.Items.Add(_fontsSizes[i]);
            }

            FontFamaly.SelectedIndex = fontIndex;
            FontSize.SelectedIndex = sizeIndex;

            _textSettings.ChosenFont = _fonts[FontFamaly.SelectedIndex];
            _textSettings.FontSize = _fontsSizes[FontSize.SelectedIndex];
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
                ChangeColorOfButsBorder(but);
            }
        }
        private void Bold_Click(object sender, EventArgs e)
        {
            Button but = sender as Button;
            _textSettings.IfBald = !_textSettings.IfBald;
            ChangeColorOfButsBorder(but);
            BoldingText();
        }
        private void Italics_Click(object sender, EventArgs e)
        {
            Button but = sender as Button;
            _textSettings.IfItalic = !_textSettings.IfItalic;
            ChangeColorOfButsBorder(but);
            IntalicingText();
        }
        private void Undeline_Click(object sender, EventArgs e)
        {
            Button but = sender as Button;
            _textSettings.IfUnderLined = !_textSettings.IfUnderLined;
            ChangeColorOfButsBorder(but);
            UnderliningText();
        }
        private void Cross_Click(object sender, EventArgs e)
        {
            Button but = sender as Button;
            _textSettings.IfCrossed = !_textSettings.IfCrossed;
            ChangeColorOfButsBorder(but);
            CrossingText();
        }
        private void ChangeColorOfButsBorder(Button but)
        {
            but.BorderThickness = but.BorderThickness == _borderSize ?
            new Thickness(0) : _borderSize;
            but.BorderBrush = but.BorderBrush == _mouseClickBorderColor ?
                new SolidColorBrush(Colors.White) : _mouseClickBorderColor;
        }
        private void CrossingText()
        {
            if (_textBox == null) return;
            TextSelection ts = _textBox.Selection;
            if (ts is null) return;

            object value = ts.GetPropertyValue(Inline.TextDecorationsProperty);
            TextDecorationCollection decorations;

            if (value is TextDecorationCollection currentDecorations &&
                _textSettings.IfCrossed)
            {
                decorations = new TextDecorationCollection(currentDecorations);
                decorations.Add(TextDecorations.Strikethrough);
                ts.ApplyPropertyValue(Inline.TextDecorationsProperty, decorations);
            }
            else if (value == DependencyProperty.UnsetValue && _textSettings.IfCrossed)
            {
                decorations = new TextDecorationCollection();
                decorations.Add(TextDecorations.Strikethrough);
                ts.ApplyPropertyValue(Inline.TextDecorationsProperty, decorations);
            }
            else if (value == DependencyProperty.UnsetValue && !_textSettings.IfCrossed)
            {
                decorations = new TextDecorationCollection();
                ts.ApplyPropertyValue(Inline.TextDecorationsProperty, decorations);
            }
            else if (value is TextDecorationCollection current)
            {
                decorations = new TextDecorationCollection(current);

                decorations =
                     new TextDecorationCollection(decorations.Where(x =>
                     x.Location != TextDecorationLocation.Strikethrough).ToList());
                ts.ApplyPropertyValue(Inline.TextDecorationsProperty, decorations);
            }
            _textBox.Focus();
        }
        private void BoldingText()
        {
            if (_textBox == null) return;

            TextSelection ts = _textBox.Selection;

            if (ts != null)
            {
                //var fontWeight = ts.GetPropertyValue(TextElement.FontWeightProperty);
                if (_textSettings.IfBald)
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
                //var fontStyle = ts.GetPropertyValue(TextElement.FontStyleProperty);
                if (_textSettings.IfItalic)
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
        private void UnderliningText()
        {
            if (_textBox == null) return;
            TextSelection ts = _textBox.Selection;
            if (ts is null) return;

            object value = ts.GetPropertyValue(Inline.TextDecorationsProperty);
            TextDecorationCollection decorations;

            if (value is TextDecorationCollection currentDecorations &&
                _textSettings.IfUnderLined)
            {
                decorations = new TextDecorationCollection(currentDecorations);
                decorations.Add(TextDecorations.Underline);
                ts.ApplyPropertyValue(Inline.TextDecorationsProperty, decorations);
            }
            else if (value == DependencyProperty.UnsetValue && _textSettings.IfUnderLined)
            {
                decorations = new TextDecorationCollection();
                decorations.Add(TextDecorations.Underline);
                ts.ApplyPropertyValue(Inline.TextDecorationsProperty, decorations);
            }
            else if (value == DependencyProperty.UnsetValue && !_textSettings.IfUnderLined)
            {
                decorations = new TextDecorationCollection();
                ts.ApplyPropertyValue(Inline.TextDecorationsProperty, decorations);
            }
            else if (value is TextDecorationCollection current)
            {
                decorations = new TextDecorationCollection(current);

                decorations =
                     new TextDecorationCollection(decorations.Where(x =>
                     x.Location != TextDecorationLocation.Underline).ToList());
                ts.ApplyPropertyValue(Inline.TextDecorationsProperty, decorations);
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
            }
        }
        private void AlignMentLeft_Click(object sender, EventArgs e)
        {
            Button but = sender as Button;
            ClearAlignmentUIThings();
            ChangeAlignmentBorder(but);
            SetTextAlignment(TextAlignment.Left);
            _textSettings.ParPosition = TextAlignment.Left;
        }
        private void AlignMentCenter_Click(object sender, EventArgs e)
        {
            Button but = sender as Button;
            ClearAlignmentUIThings();
            ChangeAlignmentBorder(but);
            SetTextAlignment(TextAlignment.Center);
            _textSettings.ParPosition = TextAlignment.Center;
        }
        private void AlignMentRight_Click(object sender, EventArgs e)
        {
            Button but = sender as Button;
            ClearAlignmentUIThings();
            ChangeAlignmentBorder(but);
            SetTextAlignment(TextAlignment.Right);
            _textSettings.ParPosition = TextAlignment.Right;
        }
        private void ChangeAlignmentBorder(Button but)
        {
            but.BorderBrush = _mouseClickBorderColor;
            but.BorderThickness = but.BorderThickness == _borderSize ?
                  new Thickness(0) : _borderSize;
        }
        private void SetTextAlignment(TextAlignment textAlign)
        {
            if (_textBox == null) return;
            TextSelection selection = _textBox.Selection;

            Paragraph paragraph = selection.Start.Paragraph;
            if (paragraph != null)
            {
                paragraph.TextAlignment = textAlign;
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
            _textSettings.ChosenFont = _fonts[FontFamaly.SelectedIndex];

            FontFamalyChanged();
        }
        private void FontFamalyChanged()
        {
            _textSettings.ChosenFont = new FontFamily(((Label)FontFamaly.SelectedValue).Content.ToString());

            TextRange selectionTextRange = _textBox.Selection;
            if (!selectionTextRange.IsEmpty)
            {
                selectionTextRange.ApplyPropertyValue(TextElement.FontFamilyProperty,
                    _textSettings.ChosenFont);
            }
            else
            {
                if (_textBox == null) return;
                TextSelection ts = _textBox.Selection;
                if (ts != null)
                    selectionTextRange.ApplyPropertyValue(TextElement.FontFamilyProperty,
                        _textSettings.ChosenFont);
                _textBox.Focus();
            }
        }
        private void FontSize_SelctionChanged(object sender, EventArgs e)
        {
            if (_textBox is null) return;
            _textSettings.FontSize = _fontsSizes[FontSize.SelectedIndex];

            ChangeFontSize();
        }
        private void ChangeFontSize()
        {
            _textSettings.FontSize = (int)FontSize.SelectedValue;

            TextRange selectionTextRange = _textBox.Selection;
            if (!selectionTextRange.IsEmpty)
            {
                selectionTextRange.ApplyPropertyValue(TextElement.FontSizeProperty, (double)_textSettings.FontSize);
            }
            else
            {
                if (_textBox == null) return;
                TextSelection ts = _textBox.Selection;
                if (ts != null)
                    selectionTextRange.ApplyPropertyValue(TextElement.FontSizeProperty, (double)_textSettings.FontSize);
                _textBox.Focus();
            }
        }
        private void FillBg_Checked(object sender, EventArgs e)
        {
            if (_textBox is null) return;
            textBg = _paintObj.SecondColor;
            _textBox.Background = textBg;
            _textSettings.IfFill = true;
        }
        private void FillBgUnChecked(object sender, EventArgs e)
        {
            if (_textBox is null) return;
            textBg = new SolidColorBrush(Colors.Transparent);
            _textBox.Background = textBg;
            _textSettings.IfFill = false;
        }
    }
}
