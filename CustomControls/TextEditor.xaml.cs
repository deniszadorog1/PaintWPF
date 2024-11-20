using PaintWPF.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.AccessControl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

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

            //_textSizeChanging.SelectCan.Background = new SolidColorBrush(Colors.Green);
            //_textBox.Background = new SolidColorBrush(Colors.Red);
            numberOfLines = 0;

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

            _textBox.Foreground = _paintObj.FirstColor;
            /*
                        _textBox.BorderBrush = new SolidColorBrush(Colors.Red);
                        _textBox.BorderThickness = new Thickness(1);*/
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
                //_textBox.BorderThickness = new Thickness(0.5);

                _textBox.TextChanged -= TextBox_TextChanged;
                _textBox.TextChanged += TextBox_TextChanged;

                _textBox.PreviewTextInput -= TextBox_PreviewTextInput;
                _textBox.PreviewTextInput += TextBox_PreviewTextInput;

                _textBox.SizeChanged -= TextBoxSizeChanged;
                _textBox.SizeChanged += TextBoxSizeChanged;
            }
        }
        private void TextBoxSizeChanged(object sender, EventArgs e)
        {
            double left = Canvas.GetLeft(_textBox);
            double top = Canvas.GetTop(_textBox);

            if (left != textLoc.X || top != textLoc.Y)
            {
                Console.WriteLine();
            }

            Canvas.SetLeft(_textBox, textLoc.X);
            Canvas.SetTop(_textBox, textLoc.Y);

            if (_textBox.Height == _textSizeChanging.Height - 10)
            {
                return;
            }
            _textBox.Height = _textSizeChanging.Height - 10;
        }


        private void TextBox_PreviewTextInput(object sender, EventArgs e)
        {
            //SetTempTextBoxHeight();

            InitForeColor();
            TextSelection ts = _textBox.Selection;
            ts.GetPropertyValue(Inline.TextDecorationsProperty);

            /*            double left = Canvas.GetLeft(_textBox);
                        double top = Canvas.GetTop(_textBox);

                        if (left != textLoc.X || top != textLoc.Y)
                        {
                            Console.WriteLine();
                        }


                        Canvas.SetLeft(_textBox, textLoc.X);
                        Canvas.SetTop(_textBox, textLoc.Y);*/

        }
        /*        private bool IsCaretAtEnd()
                {
                    TextPointer caretPos = _textBox.CaretPosition;
                    TextPointer endPos = _textBox.Document.ContentEnd.GetNextInsertionPosition(LogicalDirection.Backward);
                    return caretPos.CompareTo(endPos) == 0;
                }*/
        private void InitTextBoxHeight()
        {
            string checkText = "A";
            Size newSize = GetFontSize(checkText, _textSettings.ChosenFont, _textSettings.FontSize,
                new FontStyle(), new FontWeight());

            _textBox.Height = newSize.Height;
            InitSizeForSelectionBorder(newSize);
        }


        private double tempHeight = 0;
        private double numberOfLines = 0;
        private double tempLines = 1;

        Point textLoc = new Point(5, 5);

        private void SetTempTextBoxHeight()
        {
            const int borderDist = 0;
            _textBox.ScrollToHome();

            char? check = GetLastCharacterFromRichTextBox(_textBox);

            string checkText = check is null ? "W" : check.ToString();

            Size newSize = GetFontSize(checkText, _textSettings.ChosenFont, _textSettings.FontSize,
                new FontStyle(), new FontWeight());

            double maxWidth = _textBox.Width;
            Size contentSize = MeasureRichTextBoxContent(_textBox);

            double width = GetRichTextBoxContentWidth(_textBox);/* + numberOfLines * (Math.Ceiling(newSize.Width) + 2)*/;

            contentSize = new Size(width, Math.Round(contentSize.Height, 0));

            if (tempHeight == 0)
            {
                tempHeight = contentSize.Height;
            }

            double left = Canvas.GetLeft(_textBox);
            double top = Canvas.GetTop(_textBox);
            double size = _textBox.Height;
            double sizePArent = ((Canvas)_textBox.Parent).Width;


            //numberOfLines = Math.Ceiling(contentSize.Width / maxWidth);

            int lineWrapsAmount = CountLineWraps(_textBox);

            if (/*contentSize.Width / numberOfLines > maxWidth ||
                numberOfLines > tempLines*/ lineWrapsAmount > numberOfLines)
            {
                newSize = new Size(Math.Round(newSize.Width, 0), Math.Round(newSize.Height, 0));

                tempHeight += newSize.Height;
                contentSize.Height = tempHeight;
                //_textBox.Width = maxWidth;

                //_textBox.Height = tempHeight;
                double sizeDiffer = newSize.Height / 2 + borderDist ;

                double height = tempHeight + sizeDiffer;

                _textSizeChanging.Height = height;
                _textSizeChanging.SelectionBorder.Height = height;
                _textSizeChanging.SelectCan.Height = height;
                _textSizeChanging.CheckCan.Height = height;

                // _textBox.Height += 10;

                //tempLines = numberOfLines;

                numberOfLines++;
            }


            Canvas.SetLeft(_textBox, textLoc.X);
            Canvas.SetTop(_textBox, textLoc.Y);
        }

        private int CountLineWraps(RichTextBox richTextBox)
        {
            // Получаем начальный указатель на текст
            TextPointer pointer = richTextBox.Document.ContentStart;
            double? previousTop = null;
            int lineWraps = 0;

            // Проходим по тексту до конца
            while (pointer != null && pointer.CompareTo(richTextBox.Document.ContentEnd) < 0)
            {
                // Получаем прямоугольник символа
                Rect rect = pointer.GetCharacterRect(LogicalDirection.Forward);

                // Проверяем, если вертикальная позиция изменилась
                if (previousTop.HasValue && rect.Top > previousTop.Value)
                {
                    lineWraps++;
                }

                // Обновляем предыдущую позицию
                previousTop = rect.Top;

                // Переходим к следующему символу
                pointer = pointer.GetNextInsertionPosition(LogicalDirection.Forward);
            }

            return lineWraps;
        }


        // Метод для получения последнего символа
        private char? GetLastCharacterFromRichTextBox(RichTextBox richTextBox)
        {
            // Извлекаем весь текст из RichTextBox
            TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            string text = textRange.Text;

            // Проверяем, что текст не пустой
            if (!string.IsNullOrEmpty(text))
            {
                // Удаляем финальный символ перевода строки (RichTextBox добавляет его автоматически)
                text = text.TrimEnd('\r', '\n');

                // Возвращаем последний символ
                return text.Length > 0 ? text[text.Length - 1] : (char?)null;
            }

            // Если текста нет, возвращаем null
            return null;
        }

        private double GetRichTextBoxContentWidth(RichTextBox richTextBox)
        {
            double maxWidth = 0;

            // Проходим по всем блокам
            foreach (Block block in richTextBox.Document.Blocks)
            {
                if (block is Paragraph paragraph)
                {
                    double paragraphWidth = 0;

                    // Обрабатываем каждый Inline (часть текста с определённым стилем)
                    foreach (Inline inline in paragraph.Inlines)
                    {
                        if (inline is Run run)
                        {
                            // Создаём объект FormattedText для текущего текста
                            Typeface typeface = new Typeface(
                                run.FontFamily,
                                run.FontStyle,
                                run.FontWeight,
                                run.FontStretch
                            );

                            FormattedText formattedText = new FormattedText(
                                run.Text, // Берём весь текст Run целиком
                                System.Globalization.CultureInfo.CurrentCulture,
                                FlowDirection.LeftToRight,
                                typeface,
                                run.FontSize,
                                Brushes.Black,
                                VisualTreeHelper.GetDpi(richTextBox).PixelsPerDip
                            );

                            // Увеличиваем ширину текущего параграфа
                            paragraphWidth += formattedText.WidthIncludingTrailingWhitespace;
                        }
                    }

                    // Учитываем максимальную ширину параграфа
                    maxWidth = Math.Max(maxWidth, paragraphWidth);
                }
            }

            return maxWidth;
        }

        public void ClearSize()
        {
            tempHeight = 0;
            tempLines = 1;
        }

        Size MeasureRichTextBoxContent(RichTextBox richTextBox)
        {
            double totalWidth = 0;
            double totalHeight = 0;

            foreach (Block block in richTextBox.Document.Blocks)
            {
              
                if (block is Paragraph paragraph)
                { 
                    double paragraphWidth = 0;
                    double paragraphHeight = 0;

                    foreach (Inline inline in paragraph.Inlines)
                    {
                        if (inline is Run run)
                        {
                            // Создаём FormattedText для всего текста Run
                            Typeface typeface = new Typeface(
                                run.FontFamily,
                                run.FontStyle,
                                run.FontWeight,
                                run.FontStretch
                            );

                            FormattedText formattedText = new FormattedText(
                                run.Text, // Текст обрабатывается целиком
                                System.Globalization.CultureInfo.CurrentCulture,
                                FlowDirection.LeftToRight,
                                typeface,
                                run.FontSize,
                                Brushes.Black,
                                VisualTreeHelper.GetDpi(richTextBox).PixelsPerDip
                            );

                            // Учитываем ширину и высоту текста с межсимвольными интервалами
                            paragraphWidth += formattedText.WidthIncludingTrailingWhitespace;
                            paragraphHeight = Math.Max(paragraphHeight, formattedText.Height);
                        }
                    }

                    // Учитываем размеры текущего параграфа
                    totalWidth = Math.Max(totalWidth, paragraphWidth);
                    totalHeight += paragraphHeight;
                }
            }

            return new Size(totalWidth, totalHeight);
        }
        private void InitSizeForSelectionBorder(Size size)
        {
            const int borderSelDif = 15;
            const int selectionDif = 10;
            _textSizeChanging.Height = size.Height + borderSelDif;
            _textSizeChanging.SelectionBorder.Height = size.Height + borderSelDif;
            _textSizeChanging.SelectCan.Height = size.Height + selectionDif;
        }
        private Size GetFontSize(string text, FontFamily fontFamily, double fontSize, FontStyle fontStyle, FontWeight fontWeight)
        {
            FormattedText formattedText = new FormattedText(
               text,
               CultureInfo.CurrentCulture,
               FlowDirection.LeftToRight,
               new Typeface(fontFamily, fontStyle, fontWeight, FontStretches.Normal),
               fontSize,
               Brushes.Black,
               new NumberSubstitution(),
               1.0);

            return new Size(formattedText.Width, formattedText.Height);
        }
        private bool _loopCheck = true;
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            //return;

            SetTempTextBoxHeight();
            InitForeColor();

            double left = Canvas.GetLeft(_textBox);
            double top = Canvas.GetTop(_textBox);

            if (!_loopCheck)
            {
                _loopCheck = true;
                return;
            }
            if (_textBox == null) return;
        }
        private int counter = 0;
        private const int maxRepsLoop = 10;
        public void InitForeColor()
        {
            if (counter > maxRepsLoop)
            {
                _loopCheck = false;
                counter = 0;
                return;
            }
            counter++;
            TextSelection ts = _textBox.Selection;

            string asd = ts.Text;

            TextRange textRange = new TextRange(_textBox.Document.ContentStart, _textBox.Document.ContentEnd);
            string text = textRange.Text;
            if (text.Length > 0)
            {
                Console.WriteLine();
            }


            if (!(ts is null))
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
                TextSelection ts = _textBox.Selection;
                if (ts != null)
                    ts.ApplyPropertyValue(TextElement.ForegroundProperty, _paintObj.FirstColor);
            }
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
        private void ButtonsTextType_MouseEnter(object sender, EventArgs e) //-, +3 DUBL METHOD 
        {
            IfNeedToPaintButInWhite((UIElement)sender, _mouseEnterBorderColor, _mouseEnterBGColor);
        }
        private void TextAlign_MouseEnter(object sender, EventArgs e)
        {
            IfNeedToPaintButInWhite((UIElement)sender, _mouseEnterBorderColor, _mouseEnterBGColor);
        }
        private void Buttons_MouseLeave(object sender, EventArgs e)
        {
            IfNeedToPaintButInWhite((UIElement)sender, new SolidColorBrush(Colors.White), new SolidColorBrush(Colors.White));
        }
        private void IfNeedToPaintButInWhite(UIElement elem,
            Brush borderBrush, Brush background)
        {
            if (elem is Button but &&
             but.BorderBrush != _mouseClickBorderColor)
            {
                but.BorderBrush = borderBrush;
                but.Background = background;
            }
        }

        private void TextType_Click(object sender, EventArgs e)
        {
            if (sender is Button but)
            {
                ChangeColorOfButsBorder(but);
            }
        }
        private void Bold_Click(object sender, EventArgs e) //-, DUBL +4 METHODA
        {
            Button but = sender as Button;
            _textSettings.IfBald = !_textSettings.IfBald;
            ChangeColorOfButsBorder(but);
            BoldingText();
        }
        private void Italics_Click(object sender, EventArgs e)
        {
            /*            Button but = sender as Button;
                        _textSettings.IfItalic = !_textSettings.IfItalic;
                        ChangeColorOfButsBorder(but);*/

            _textSettings.IfItalic = GetTextParamValue(_textSettings.IfItalic, sender);
            IntalicingText();
        }
        private void Undeline_Click(object sender, EventArgs e)
        {
            /*            Button but = sender as Button;
                        _textSettings.IfUnderLined = !_textSettings.IfUnderLined;
                        ChangeColorOfButsBorder(but);*/

            _textSettings.IfUnderLined = GetTextParamValue(_textSettings.IfUnderLined, sender);
            UnderliningText();
        }
        private void Cross_Click(object sender, EventArgs e)
        {
            /*            Button but = sender as Button;
                        _textSettings.IfCrossed = !_textSettings.IfCrossed;
                        ChangeColorOfButsBorder(but);*/

            _textSettings.IfCrossed = GetTextParamValue(_textSettings.IfCrossed, sender);
            CrossingText();
        }
        private bool GetTextParamValue(bool param, object sender)
        {
            Button but = sender as Button;
            ChangeColorOfButsBorder(but);
            return !param;
        }

        private void ChangeColorOfButsBorder(Button but)
        {
            but.BorderThickness = but.BorderThickness == _borderSize ?
            new Thickness(0) : _borderSize;
            but.BorderBrush = but.BorderBrush == _mouseClickBorderColor ?
                new SolidColorBrush(Colors.White) : _mouseClickBorderColor;
        }
        private void CrossingText() //-DUBL
        {
            if (_textBox == null) return;
            TextSelection ts = _textBox.Selection;
            if (ts is null) return;

            object value = ts.GetPropertyValue(Inline.TextDecorationsProperty);
            TextDecorationCollection decorations = value is TextDecorationCollection currentDecorations &&
                _textSettings.IfCrossed ? new TextDecorationCollection(currentDecorations) :
                value == DependencyProperty.UnsetValue ? new TextDecorationCollection() :
                value is TextDecorationCollection current ? new TextDecorationCollection(current) : null;

            if (decorations is null)
            {
                _textBox.Focus();
                return;
            }

            if (decorations.Count > 0)
            {
                if (_textSettings.IfCrossed) ApplyDrcoration(TextDecorations.Strikethrough, ts, decorations);
                else
                {
                    decorations =
                    new TextDecorationCollection(decorations.Where(x =>
                    x.Location != TextDecorationLocation.Strikethrough).ToList());
                    ts.ApplyPropertyValue(Inline.TextDecorationsProperty, decorations);
                }
                _textBox.Focus();
                return;
            }
            if (_textSettings.IfCrossed) ApplyDrcoration(TextDecorations.Strikethrough, ts, decorations);
            else
            {
                decorations = new TextDecorationCollection();
                ts.ApplyPropertyValue(Inline.TextDecorationsProperty, decorations);
            }
            _textBox.Focus();
            //return;
            /*
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
                        _textBox.Focus();*/
        }
        private void ApplyDrcoration(TextDecorationCollection dec, TextSelection ts, TextDecorationCollection collection)
        {
            collection.Add(dec);
            ts.ApplyPropertyValue(Inline.TextDecorationsProperty, collection);
        }

        private void BoldingText()
        {
            SetTextDecor(true);
            /*            if (_textBox == null) return;

                        TextSelection ts = _textBox.Selection;
                        if (ts is null)
                        {
                            _textBox.Focus();
                            return;
                        }
                        FontWeight weight = _textSettings.IfBald ? FontWeights.Bold : FontWeights.Normal;
                        ts.ApplyPropertyValue(TextElement.FontWeightProperty, weight);
                        _textBox.Focus();*/
        }
        private void IntalicingText() //-, SOKRATIT
        {
            SetTextDecor(false);
            /*if (_textBox == null) return;
            TextSelection ts = _textBox.Selection;

            if (ts is null)
            {
                _textBox.Focus();
                return;
            }

            FontStyle style = _textSettings.IfItalic ? FontStyles.Italic : FontStyles.Normal;
            ts.ApplyPropertyValue(TextElement.FontStyleProperty, style);

            _textBox.Focus();*/
        }
        public void SetTextDecor(bool ifBald)
        {
            if (_textBox == null) return;
            TextSelection ts = _textBox.Selection;

            if (ts is null)
            {
                _textBox.Focus();
                return;
            }

            if (!ifBald)
            {
                FontStyle style = _textSettings.IfItalic ? FontStyles.Italic : FontStyles.Normal;
                ts.ApplyPropertyValue(TextElement.FontStyleProperty, style);
            }
            else
            {
                FontWeight weight = _textSettings.IfBald ? FontWeights.Bold : FontWeights.Normal;
                ts.ApplyPropertyValue(TextElement.FontWeightProperty, weight);
            }
            _textBox.Focus();
        }


        private void UnderliningText() //-, SOKRATIT
        {
            if (_textBox == null) return;
            TextSelection ts = _textBox.Selection;
            if (ts is null) return;

            object value = ts.GetPropertyValue(Inline.TextDecorationsProperty);
            TextDecorationCollection decorations = value is TextDecorationCollection currentDecorations &&
                _textSettings.IfUnderLined ? new TextDecorationCollection(currentDecorations) :
                value == DependencyProperty.UnsetValue ? new TextDecorationCollection() :
                value is TextDecorationCollection current ? new TextDecorationCollection(current) : null;

            if (decorations is null)
            {
                _textBox.Focus();
                return;
            }
            if (decorations.Count > 0)
            {
                if (_textSettings.IfUnderLined) ApplyDrcoration(TextDecorations.Underline, ts, decorations);
                else
                {
                    decorations =
                    new TextDecorationCollection(decorations.Where(x =>
                    x.Location != TextDecorationLocation.Underline).ToList());
                    ts.ApplyPropertyValue(Inline.TextDecorationsProperty, decorations);
                }
                _textBox.Focus();
                return;
            }
            if (_textSettings.IfUnderLined) ApplyDrcoration(TextDecorations.Underline, ts, decorations);
            else
            {
                decorations = new TextDecorationCollection();
                ts.ApplyPropertyValue(Inline.TextDecorationsProperty, decorations);
            }
            _textBox.Focus();

            /* if (value is TextDecorationCollection currentDecorations &&
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
             _textBox.Focus();*/
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
        private void AlignMentLeft_Click(object sender, EventArgs e) //-, +3 DUBL NEXT METHODS
        {
            SetAlignmentButton(sender, TextAlignment.Left);
            /*
                        Button but = sender as Button;
                        ClearAlignmentUIThings();
                        ChangeAlignmentBorder(but);
                        SetTextAlignment(TextAlignment.Left);
                        _textSettings.ParPosition = TextAlignment.Left;*/
        }
        private void AlignMentCenter_Click(object sender, EventArgs e)
        {
            SetAlignmentButton(sender, TextAlignment.Center);

            /*            Button but = sender as Button;
                        ClearAlignmentUIThings();
                        ChangeAlignmentBorder(but);
                        SetTextAlignment(TextAlignment.Center);
                        _textSettings.ParPosition = TextAlignment.Center;*/
        }
        private void AlignMentRight_Click(object sender, EventArgs e)
        {
            SetAlignmentButton(sender, TextAlignment.Right);

            /*            Button but = sender as Button;
                        ClearAlignmentUIThings();
                        ChangeAlignmentBorder(but);
                        SetTextAlignment(TextAlignment.Right);
                        _textSettings.ParPosition = TextAlignment.Right;*/
        }

        public void SetAlignmentButton(object sender, TextAlignment alignment)
        {
            Button but = sender as Button;
            ClearAlignmentUIThings();
            ChangeAlignmentBorder(but);
            SetTextAlignment(alignment);
            _textSettings.ParPosition = alignment;
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
                _alignmetButs[i].BorderBrush = new SolidColorBrush(Colors.White);
                _alignmetButs[i].BorderThickness = new Thickness(0);
                _alignmetButs[i].Background = new SolidColorBrush(Colors.White);
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
        private void FillBg_Checked(object sender, EventArgs e) //+2 DUBL, -
        {
            BgFilling(_paintObj.SecondColor, true);

            /*            if (_textBox is null) return;
                        textBg = _paintObj.SecondColor;
                        _textBox.Background = textBg;
                        _textSettings.IfFill = true;*/
        }
        private void FillBgUnChecked(object sender, EventArgs e)
        {
            BgFilling(new SolidColorBrush(Colors.Transparent), false);

            /*            if (_textBox is null) return;
                        textBg = new SolidColorBrush(Colors.Transparent);
                        _textBox.Background = textBg;
                        _textSettings.IfFill = false;*/
        }
        public void BgFilling(SolidColorBrush color, bool ifFill)
        {
            if (_textBox is null) return;
            textBg = color;
            _textBox.Background = textBg;
            _textSettings.IfFill = ifFill;
        }

    }
}
