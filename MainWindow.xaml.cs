using System;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Linq;
using PaintWPF.Models;
using PaintWPF.Models.Enums;
using System.IO;
using System.Windows.Threading;
using Microsoft.Win32;
using System.Runtime.InteropServices;

using PaintWPF.Models.Tools;
using PaintWPF.CustomControls;
using PaintWPF.Other;

using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;
using PaintWPF.CustomControls.SubMenu;
using PaintWPF.CustomControls.BrushesMenu;
using System.Runtime.Remoting.Messaging;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Windows.Documents;

namespace PaintWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainPaint _main = new MainPaint();

        private List<Button> _tools = new List<Button>();
        private List<MenuItem> _brushTypes = new List<MenuItem>();
        private Button _chosenTool = null;

        private UIElement valueDragElem = null;
        private Point valueOffset;
        private DispatcherTimer sprayTimer;

        private readonly SolidColorBrush _clickedBorderColor =
            new SolidColorBrush(Color.FromRgb(0, 103, 192));

        private const double CalligraphyBrushAngle = 135 * Math.PI / 180;
        private const double FountainBrushAngle = 45 * Math.PI / 180;

        LineSizing _lineSizing = null;

        private TextEditor _text = null;
        private RichTextBox _richTexBox = null;

        private double _startThisHeight;
        private Selection _changedSizeText = null;

        private List<Button> _customColors = new List<Button>();
        private int _customColorIndex = 0;

        private Button _chosenToPaintButton = null;

        private List<string> _savedPaths = new List<string>();
        private DateTime? _lastSaveTime = null;

        private MouseEventArgs drawFieldEvent;

        public MainWindow()
        {
            InitializeComponent();

            InitStartHeight();
            InitToolButsInList();
            InitBrushTypesInList();

            InitializeSprayTimer();
            CanvasSize.Content = $"{DrawingCanvas.Width} x {DrawingCanvas.Height}";

            InitForTextControl();
            InitCustomColorButtons();

            InitTransparentSelectionImagePath();
            InitValueForSizeAdaptation();

            InitBrushMenu();

            InitEventsForCheckRect();
        }

        private void InitEventsForCheckRect()
        {
            Canvas.SetLeft(CheckRect, 0);
            Canvas.SetTop(CheckRect, 0);

            //Mouse move
            CheckRect.MouseMove += CheckRect_MouseMove;
            CheckRect.PreviewMouseDown += CheckRect_PreviewMouseDown;
            CheckRect.PreviewMouseUp += CheckRect_PreviewMouseUp;
        }

        private void CheckRect_MouseMove(object sender, MouseEventArgs e)
        {
            if (CheckRect.IsMouseCaptured)
            {
                const int divideToCenter = 2;
                (sender as UIElement).CaptureMouse();

                Point currentPoint = e.GetPosition(CheckRect.Parent as IInputElement);
                double offsetX = currentPoint.X - _startPointSelection.X;
                double offsetY = currentPoint.Y - _startPointSelection.Y;

                Canvas.SetLeft(CheckRect, _anchorPointSelection.X + offsetX - CheckRect.Width / divideToCenter);
                Canvas.SetTop(CheckRect, _anchorPointSelection.Y + offsetY - CheckRect.Height / divideToCenter);
            }
            if (_main._ifSelection)
            {
                ChangeSelectionSize(e);
            }
            else if (!(_lineSizing is null))
            {
                _lineSizing.Line_MouseMove(_lineSizing, e);
            }
        }

        private Point _anchorPointSelection;
        private Point _startPointSelection;
        private void CheckRect_PreviewMouseDown(object sender, MouseEventArgs e)
        {
            if (IfThereIsReClick()) return;
            _anchorPointSelection = new Point(Canvas.GetLeft(CheckRect), Canvas.GetTop(CheckRect));
            _startPointSelection = e.GetPosition(CheckRect.Parent as IInputElement);
            CheckRect.CaptureMouse();

            if (_main._ifFiguring)
            {
                _main.InitShapesToPaint(e, DrawingCanvas);

                DrawingCanvas.ClipToBounds = true;
                _main.AddCheckRect(e, DrawingCanvas, CheckRect);
            }
        }

        private void CheckRect_PreviewMouseUp(object sender, MouseEventArgs e)
        {
            sprayTimer.Stop();
        }

        private void InitBrushMenu()
        {
            InitBrushMenuMargin();
            InitBrushMenuEvents();
        }

        private void InitBrushMenuMargin()
        {
            const int leftMarginCorel = 25;
            const int centerDivider = 2;

            BrushesMenu.Margin = new Thickness(SelectionPart.Width.Value +
                ToolsPart.Width.Value + (BrushPart.Width.Value / centerDivider) - (BrushesMenu.Width / centerDivider),
                SettingsRow.Height.Value + ToolsRow.Height.Value - leftMarginCorel, 0, 0);
        }

        private void InitBrushMenuEvents()
        {
            for (int i = 0; i < BrushesMenu.BrushesListBox.Items.Count; i++)
            {
                if (BrushesMenu.BrushesListBox.Items[i].GetType() == typeof(BrushMenuItem))
                {
                    Button but = ((BrushMenuItem)BrushesMenu.BrushesListBox.Items[i]).ItemBut;
                    but.Tag = ((BrushType)i).ToString();
                    but.Click += BrushType_Click;
                }
            }
        }
        private void BrushButton_Click(object sender, EventArgs e)
        {
            BrushesMenu.Visibility = BrushesMenu.Visibility == Visibility.Visible ?
                Visibility.Hidden : Visibility.Visible;
        }
        public void InitCustomColorButtons()
        {
            _customColors.Clear();
            _customColors.Add(OnePaletteColor);
            _customColors.Add(TwoPaletteColor);
            _customColors.Add(ThreePaletteColor);
            _customColors.Add(FourPaletteColor);
            _customColors.Add(FivePaletteColor);
            _customColors.Add(SixPaletteColor);
            _customColors.Add(SevenPaletteColor);
            _customColors.Add(EightPaletteColor);
            _customColors.Add(NinePaletteColor);
            _customColors.Add(TenPaletteColor);
        }

        public void InitStartHeight()
        {
            double height = Height;
            _startThisHeight = height;
        }

        public void InitForTextControl()
        {
            const int rowToAdd = 0;
            const int columnToAdd = 1;

            DeleteText();
            _text = new TextEditor(_main, _changedSizeText);

            UpdateTextLocation();
            Grid.SetRow(_text, rowToAdd);
            Grid.SetColumn(_text, columnToAdd);

            CenterWindowPanel.Children.Add(_text);
            _text.Visibility = Visibility.Hidden;
        }
        private void DeleteText()
        {
            for (int i = 0; i < CenterWindowPanel.Children.Count; i++)
            {
                if (CenterWindowPanel.Children[i] is TextEditor)
                {
                    CenterWindowPanel.Children.RemoveAt(i);
                    return;
                }
            }
        }
        public void UpdateTextLocation()
        {
            double locX = _text.TextObject.Width;
            const double locY = 5;
            const int heightDivider = 2;

            double heightDiffer = Height - _startThisHeight;
            double resMargin = heightDiffer / heightDivider;

            _text.Margin = new Thickness(0, 0, 0, resMargin);

            Canvas.SetLeft(_text, locX);
            Canvas.SetTop(_text, locY);
        }
        public void InitBrushTypesInList()
        {
            _brushTypes.Clear();
            InitTagsForBrushesTypes();
        }
        public void InitTransparentSelectionImagePath()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            string startDir = dirInfo.Parent.Parent.FullName;
            string imgPath = System.IO.Path.Combine(startDir, "Images");
            string selectionPath = System.IO.Path.Combine(imgPath, "Selection");
            string tickPath = System.IO.Path.Combine(selectionPath, "Tick.png");

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(tickPath, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();

            _main._tickImg = new Image
            {
                Source = bitmap,
                Width = TransSelectionIcon.Width,
                Height = TransSelectionIcon.Width
            };
        }
        public void InitTagsForBrushesTypes()
        {
            for (int i = 0; i < _brushTypes.Count; i++)
            {
                _brushTypes[i].Tag = ((BrushType)i).ToString();
            }
        }
        public void InitToolButsInList()
        {
            _tools.Clear();
            _tools.Add(Pen);
            _tools.Add(Bucket);
            _tools.Add(Text);
            _tools.Add(Erazer);
            _tools.Add(ColorDrop);
            _tools.Add(Glass);
        }

        private const int _chosenMainPaintCircleMargin = -10;
        private const int _chosenMainPaintCircleSizeAdd = 1;
        private const int _chosenMainPaintCircleThickness = 1;
        private void PaintColor_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            ChangedChosenColor(button);
        }

        private void ChangedChosenColor(Button button)
        {
            ClearMainColorBorders();

            Ellipse external = new Ellipse();
            external.Width = button.Width + _chosenMainPaintCircleSizeAdd;
            external.Height = button.Height + _chosenMainPaintCircleSizeAdd;

            external.Stroke = Brushes.Blue;
            external.StrokeThickness = _chosenMainPaintCircleThickness;

            external.Margin = new Thickness(_chosenMainPaintCircleMargin);

            button.Content = external;
            _chosenToPaintButton = button;
        }

        public void ClearMainColorBorders()
        {
            ClearMainColorBorder(FirstColor);
            ClearMainColorBorder(SecondColor);
        }

        public void ClearMainColorBorder(Button button)
        {
            /*            Ellipse greyCircle = new Ellipse();

                        greyCircle.Width = button.Width + _chosenMainPaintCircleSizeAdd;
                        greyCircle.Height = button.Height + _chosenMainPaintCircleSizeAdd;
                        greyCircle.Fill = Brushes.Transparent;

                        greyCircle.Stroke = MainPanel.Background;
                        greyCircle.StrokeThickness = _chosenMainPaintCircleThickness;

                        greyCircle.Margin = new Thickness(_chosenMainPaintCircleMargin);

                        button.Content = greyCircle;*/

            ChangeELipseColor(button, new SolidColorBrush(Colors.Transparent));

        }

        private void MyButton_MouseEnter(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;

            /*           Ellipse greyCircle = new Ellipse();
                       greyCircle.Width = button.Width + _chosenMainPaintCircleSizeAdd;
                       greyCircle.Height = button.Height + _chosenMainPaintCircleSizeAdd;

                       greyCircle.Stroke = Brushes.DarkGray;
                       greyCircle.StrokeThickness = _chosenMainPaintCircleThickness;

                       greyCircle.Margin = new Thickness(_chosenMainPaintCircleMargin);

                       button.Content = greyCircle;*/


            ChangeELipseColor(button, new SolidColorBrush(Colors.DarkGray));

            if (_chosenToPaintButton == SecondColor) return;
            ChangeFigToPaintColor((SolidColorBrush)button.Background);
            ChangeTextBgColor((SolidColorBrush)button.Background);
        }
        private void ChangeELipseColor(Button button, SolidColorBrush color)
        {

            Ellipse greyCircle = new Ellipse();
            greyCircle.Width = button.Width + _chosenMainPaintCircleSizeAdd;
            greyCircle.Height = button.Height + _chosenMainPaintCircleSizeAdd;

            greyCircle.Stroke = color; //ifTrancparent ? Brushes.Transparent : Brushes.DarkGray;
            greyCircle.StrokeThickness = _chosenMainPaintCircleThickness;

            greyCircle.Margin = new Thickness(_chosenMainPaintCircleMargin);

            button.Content = greyCircle;
        }

        private void ChangeTextBgColor(SolidColorBrush brush)
        {
            if (_text is null || !_text._textSettings.IfFill) return;
            _text._textBox.Background = brush;
        }

        private void ChangeFigToPaintColor(SolidColorBrush brush)
        {
            if (!(_lineSizing is null))
            {
                _lineSizing.Line.Stroke = brush;
                return;
            }
            if ((_main._selection is null || _main._selection._shape is null ||
                _main._selection._isDraggingSelection)) return;
            _main._selection._shape.Stroke = brush;
        }

        private void MyButton_MouseLeave(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;

            /*            Ellipse greyCircle = new Ellipse();
                        greyCircle.Width = button.Width + _chosenMainPaintCircleSizeAdd;
                        greyCircle.Height = button.Height + _chosenMainPaintCircleSizeAdd;
                        greyCircle.Fill = Brushes.Transparent;

                        greyCircle.Stroke = MainPanel.Background;
                        greyCircle.StrokeThickness = _chosenMainPaintCircleThickness;

                        greyCircle.Margin = new Thickness(_chosenMainPaintCircleMargin);

                        button.Content = greyCircle;*/

            ChangeELipseColor(button, new SolidColorBrush(Colors.Transparent));

            if (_chosenToPaintButton == SecondColor) return;
            RemoveFigToPaintStrokeChangings();
            RemoveChangingsTextBgColor();
        }

        private void RemoveChangingsTextBgColor()
        {
            if (_text is null || !_text._textSettings.IfFill) return;
            _text._textBox.Background = _main.SecondColor;
        }

        private void RemoveFigToPaintStrokeChangings()
        {
            if (!(_lineSizing is null))
            {
                _lineSizing.Line.Stroke = _main.FirstColor;
                return;
            }
            if (_main._selection is null || _main._selection._shape is null) return;
            _main._selection._shape.Stroke = _main.FirstColor;
        }

        private void Palette_Click(object sender, EventArgs e)
        {
            Pallete palette = new Pallete(_main.PalleteMod, GetSelectedColor());
            palette.ShowDialog();

            if (!(_main.PalleteMod.ChosenColor is null))
            {
                InitColorInCustomColor(_main.PalleteMod.ChosenColor);
            }
        }
        private SolidColorBrush GetSelectedColor() //-
        {
            return (_chosenToPaintButton is null ||
                _chosenToPaintButton.Name == FirstColor.Name /*"FirstColor"*/) ? _main.FirstColor : _main.SecondColor;

        }

        private void InitColorInCustomColor(SolidColorBrush color)
        {
            const int getLastIndexSubtraction = 1;
            if (color is null) return;
            if (_customColorIndex == _customColors.Count - getLastIndexSubtraction)
            {
                MoveColorsIntListInLeft(color);
                return;
            }

            bool ifContains = false;
            int colorIndex = -1;
            for(int i = 0; i < _customColors.Count; i++)
            {
                if (_customColors[i].Background.ToString() == color.ToString())
                {
                    ifContains = true;
                    colorIndex = i;
                }
            }

            if (ifContains)
            {
                for(int i = colorIndex; i < _customColors.Count; i++)
                {
                    if(i == _customColors.Count - 1)
                    {
                        _customColors[_customColorIndex - 1].Background = color;
                    }
                    else
                    {
                        _customColors[i].Background = _customColors[i + 1].Background;
                    }
                }
            }
            else
            {
                _customColors[_customColorIndex].Background = color;
                _customColorIndex++;
            }


            if (_chosenToPaintButton is null ||
                _chosenToPaintButton == FirstColor)
            {
                _main.FirstColor = new SolidColorBrush(color.Color);
                FirstColor.Background = new SolidColorBrush(color.Color);
            }
            else if (_chosenToPaintButton == SecondColor)
            {
                _main.SecondColor = new SolidColorBrush(color.Color);
                SecondColor.Background = new SolidColorBrush(color.Color);
            }
        }

        private void MoveColorsIntListInLeft(SolidColorBrush color)
        {
            const int getLastIndexSubtraction = 1;
            for (int i = 0; i < _customColors.Count - getLastIndexSubtraction; i++)
            {
                _customColors[i].Background = _customColors[i + 1].Background;
            }
            _customColors[_customColors.Count - getLastIndexSubtraction].Background = color;
        }

        private void MainPanelBoxes_MouseEnter(object sender, MouseEventArgs e)
        {
            SolidColorBrush _toolPanelBrush = new SolidColorBrush(Color.FromRgb(240, 240, 240));
            SolidColorBrush _toolPanelBorderBrush = new SolidColorBrush(Color.FromRgb(209, 209, 209));

            ChangeLabelBoxColor(sender, _toolPanelBrush, _toolPanelBorderBrush);

            if (sender is Button but)
            {
                if ((!(_chosenTool is null) && but.Name == _chosenTool.Name) ||
                    ((SolidColorBrush)but.BorderBrush).Color == _clickedBorderColor.Color) return;

                but.Background = _toolPanelBrush;
                but.BorderBrush = _toolPanelBorderBrush;
            }
            else if (sender is Border bord)
            {
                bord.Background = _toolPanelBrush;
                bord.BorderBrush = _toolPanelBorderBrush;
            }
            else if (sender is Grid grid)
            {
                grid.Background = _toolPanelBrush;
            }
        }
        private void MainPanelTop_MouseEnter(object sender, EventArgs e) //-
        {
            SolidColorBrush _topPanelBrush =
                        new SolidColorBrush(Color.FromRgb(230, 235, 240));
            if (sender is Button but)
            {
                but.Background = _topPanelBrush;
            }
            else if (sender is Grid grid)
            {
                grid.Background = _topPanelBrush;
            }
        }
        private void MainLabelBoxes_MouseLeave(object sender, MouseEventArgs e) //-
        {
            SolidColorBrush _mainPanelButtonsBrush = new SolidColorBrush(Color.FromArgb(0, 248, 249, 252));

            SolidColorBrush borderColor = null;

            if (sender is Button button)
            {
                borderColor = button.BorderBrush != _clickedBorderColor ?
                        _mainPanelButtonsBrush : _clickedBorderColor;
            }
            else if (sender is Border border)
            {
                borderColor = border.BorderBrush != _clickedBorderColor ?
                        _mainPanelButtonsBrush : _clickedBorderColor;
            }

            ResetRenderTransform();

            ChangeLabelBoxColor(sender, _mainPanelButtonsBrush, borderColor);

            /*            if (sender is Button but)
                        {
                            if (!(_chosenTool is null) && but.Name == _chosenTool.Name ||
                                ((SolidColorBrush)but.BorderBrush).Color == _clickedBorderColor.Color)
                            {
                                UpdateRenderTransform();
                                return;
                            }
                            but.Background = _mainPanelButtonsBrush;
                            but.BorderBrush = but.BorderBrush != _clickedBorderColor ?
                                _mainPanelButtonsBrush : _clickedBorderColor;
                        }
                        else if (sender is Border bord)
                        {
                            bord.Background = _mainPanelButtonsBrush;
                            bord.BorderBrush = bord.BorderBrush != _clickedBorderColor ?
                                _mainPanelButtonsBrush : _clickedBorderColor;
                        }
                        else if (sender is Grid grid)
                        {
                            grid.Background = _mainPanelButtonsBrush;
                        }*/
            UpdateRenderTransform();
        }
        public void ChangeLabelBoxColor(object sender, SolidColorBrush bgColor, SolidColorBrush borderColor)
        {
            if (sender is Button but)
            {
                if (!(_chosenTool is null) && but.Name == _chosenTool.Name ||
                    ((SolidColorBrush)but.BorderBrush).Color == _clickedBorderColor.Color)
                {
                    UpdateRenderTransform();
                    return;
                }
                but.Background = bgColor;
                but.BorderBrush = borderColor;
            }
            else if (sender is Border bord)
            {
                bord.Background = bgColor;
                bord.BorderBrush = borderColor;
            }
            else if (sender is Grid grid)
            {
                grid.Background = bgColor;
            }
        }

        private void Tool_MouseClick(object sender, EventArgs e)
        {
            ResetRenderTransform();
            DrawingCanvas.RenderTransform = new TranslateTransform(0, 0);
            _main._tempCursor = null;

            IfSelectionContainsAndClearIt();
            RemoveRightClickMenus();
            _main.FreeSelection(DrawingCanvas, CheckRect);
            RemoveRazerMark();

            if (sender is Button but)
            {
                ClearDynamicValues(brushClicked: but.Name == Pen.Name.ToString() || but.Name == Erazer.Name.ToString(),
                    textClicked: but.Name == Text.Name.ToString());
                if (!(_chosenTool is null) &&
                    but.Name == _chosenTool.Name)
                {
                    UpdateRenderTransform();
                    return;
                }
                _chosenTool = but;
                SetActionTypeByButtonPressed(but);
            }
            UpdateRenderTransform();
        }

        private void ResetRenderTransform()
        {
            DrawingCanvas.RenderTransform = new TranslateTransform(0, 0);
        }

        private void UpdateRenderTransform()
        {
            DrawingCanvas.RenderTransform = new TranslateTransform(_main._horizontalOffset, 0);
        }

        public void ClearBGs()
        {
            ClearBGForTools();
            _chosenTool = null;
            ClearBgsForFigures();
        }

        private readonly SolidColorBrush _transparentBrush =
            new SolidColorBrush(Color.FromArgb(0, byte.MaxValue, byte.MaxValue, byte.MaxValue));
        public void ClearBGForTools()
        {
            for (int i = 0; i < _tools.Count; i++)
            {
                _tools[i].Background = _transparentBrush;
                _tools[i].BorderBrush = _transparentBrush;
            }
        }

        public void ClearBgsForFigures()
        {
            for (int i = 0; i < FigurePanel.Children.Count; i++)
            {
                if (FigurePanel.Children[i] is Button but)
                {
                    but.Background = _transparentBrush;
                    but.BorderBrush = _transparentBrush;
                }
            }
        }

        private bool _ifCursorInsideDrawingCan = false;
        public void PaintField_MouseLeave(object sender, MouseEventArgs e)
        {
            _ifCursorInsideDrawingCan = false;
            RazerMarker.Visibility = Visibility.Hidden;
            CursCords.Content = string.Empty;
            sprayTimer.Stop();
            CursorCheck();
            Cursor = null;
        }
        private void Field_MouseDown(object sender, MouseEventArgs e) //-
        {
            //const int baseTextLines = 1;

            //Check if textwindow is exist + clear it
            if (_changedSizeText is null && !(_text is null))
            {
                _text.ClearSize();
                /*                _text.tempHeight = 0;
                                _text.tempLines = baseTextLines;*/
            }

            // if there was a reclick (right + left)
            if (IfThereIsReClick()) return;

            ResetRenderTransform();
            //clear some dynamyc values
            DrawingCanvas.Children.Remove(RazerMarker);
            _main._drawPolyline = null;
            _main._paintDirection = null;

            if (!(_main._selection is null) || !(_changedSizeText is null))
            {
                UpdateRenderTransform();
                return;
            }

            //Set main clolor
            CheckForColors(e);
            _ifThereAreUnsavedChangings = true;
            _ifSaved = false;
            _main.previousPoint = e.GetPosition(DrawingCanvas);

            //set flag for active thing
            _main.InitDeed();

            //Check for ClipToBounds for drawingCanvas
            ClipCheck();

            //set chosen activeness 
            if (_main._ifTexting && !DrawingCanvas.Children.Contains(_richTexBox))
            {
                CreateTextPart(e);
            }
            if (_main._ifPickingColor)
            {
                EnterInitedColor(e);
            }
            else if (_main._ifSelection)
            {
                MakeSelection(e);
            }
            else if (_main._ifFilling)
            {
                _main.RenderCanvasToBitmap(DrawingCanvas);
                _main.PerformFloodFill((int)_main.previousPoint.X, (int)_main.previousPoint.Y, DrawingCanvas);
            }
            else if (_main._ifFiguring)
            {
                RemoveCheckRect();
                _main.InitShapesToPaint(e, DrawingCanvas);

                DrawingCanvas.ClipToBounds = true;
                _main.AddCheckRect(e, DrawingCanvas, CheckRect);
            }
            else
            {
                _main.previousPoint = e.GetPosition(DrawingCanvas);
                _main.SetMarkers(e, DrawingCanvas);
                if (_main._ifDrawing) BrushPaint(e);

                _main.AddCheckRect(e, DrawingCanvas, CheckRect);
            }
            UpdateRenderTransform();
        }

        private void CheckForColors(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _main.ColorToPaint = _main.FirstColor;
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                _main.ColorToPaint = _main.SecondColor;
            }
        }

        private bool IfThereIsReClick()
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed &&
               Mouse.RightButton == MouseButtonState.Pressed)
            {
                if (_main._ifDrawing)
                {
                    return true;
                }
                if (!(_main._selection is null) && _main._selection.CheckCan.IsMouseCaptured)
                {
                    SetEscapeBgSelection();
                    Cursor = null;
                }
                RemoveClicked();
                DrawingCanvas.Children.Remove(CheckRect);
                return true;
            }
            return false;
        }

        public void ClipCheck()
        {
            if (!(_main._ifErasing) || _main._type != ActionType.Erazing)
            {
                DrawingCanvas.ClipToBounds = false;
            }
        }

        private void EnterInitedColor(MouseEventArgs e)
        {
            Color gotColor = _main.GetColorAtTempPosition(e, DrawingCanvas);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _main.FirstColor = new SolidColorBrush(gotColor);
                FirstColor.Background = _main.FirstColor;
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                _main.SecondColor = new SolidColorBrush(gotColor);
                SecondColor.Background = _main.SecondColor;
            }
            _main.MakeAllActionsNegative();
            _main._ifDrawing = true;

            _main._tempCursor = _main._pencilCurs;
            Cursor = _main._pencilCurs;

            _main._type = ActionType.Drawing;
            ValueBorder.Visibility = Visibility.Visible;

            ClearDynamicValues();
            SetActionTypeByButtonPressed(Pen);
        }

        private const int _dpiParam = 96;
        private void MakeSelection(MouseEventArgs e)
        {
            _main._selectionType = _savedType;
            if (_main._selectionType == SelectionType.Rectangle)
            {
                _main.MakeRectangleSelection(e, DrawingCanvas);
            }
            else if (_main._selectionType == SelectionType.Custom)
            {
                _main.MakeCustomSelection(e, DrawingCanvas);
            }
            _main.AddCheckRect(e, DrawingCanvas, CheckRect);
        }

        private void Field_MouseMove(object sender, MouseEventArgs e)
        {
            CheckForColors(e);

            Point position = e.GetPosition(DrawingCanvas);
            _main.currentPoint = position;

            drawFieldEvent = e;
            SetCursorPosition(position);

            AddErasingMarker(position);
            if (_main._ifSelection)
            {
                ChangeSelectionSize(e);
            }
            else if (_main._ifDrawing || _main._ifErasing)
            {
                if (_main._ifErasing)
                {
                    _main.AddCheckRect(e, DrawingCanvas, CheckRect);
                }
                BrushPaint(e);
            }
            else if (!(_main._selection is null) && _main._ifChangingFigureSize)
            {
                _main._firstSelectionEnd = new Point(e.GetPosition(DrawingCanvas).X, e.GetPosition(DrawingCanvas).Y);

                double widthDiffer = _main._firstSelectionEnd.X - _main._firstSelectionStart.X;
                double heightDiffer = _main._firstSelectionEnd.Y - _main._firstSelectionStart.Y;

                SetObgjContent(new Size(Math.Abs(widthDiffer), Math.Abs(heightDiffer)));
            }
        }
        public void SetCursorPosition(Point position)
        {
            string cursPosInPaintField = ((position.X == -1) || (position.Y == -1)) ?
             string.Empty : $"{(int)position.X}, {(int)position.Y}";
            CursCords.Content = cursPosInPaintField;
        }


        private void AddErasingMarker(Point posPoint)
        {
            const int mostZIndex = 1;
            const int brushHalf = 2;
            const int pngBrushPosCorel = 10;

            if (posPoint.X == -1 && posPoint.Y == -1)
            {
                RazerMarker.Width = 0;
                return;
            }
            else
            {
                RazerMarker.Width = _main.brushThickness;
            }
            if (!_main._ifErasing && _main._type != ActionType.Erazing)
            {
                RazerMarker.Visibility = Visibility.Visible;
                return;
            }
            if (!DrawingCanvas.Children.Contains(RazerMarker) && _ifCursorInsideDrawingCan)
            {
                DrawingCanvas.Children.Clear();
                var parent = RazerMarker.Parent;
                if (!(parent is null)) ((Canvas)parent).Children.Remove(RazerMarker);
                DrawingCanvas.Children.Add(RazerMarker);
                Panel.SetZIndex(RazerMarker, mostZIndex);
            }

            RazerMarker.Visibility = Visibility.Visible;
            RazerMarker.Width = _main.brushThickness;
            RazerMarker.Height = _main.brushThickness;

            Canvas.SetLeft(RazerMarker, posPoint.X - _main.brushThickness / brushHalf - pngBrushPosCorel);
            Canvas.SetTop(RazerMarker, posPoint.Y - _main.brushThickness / brushHalf - pngBrushPosCorel);

        }

        private void ChangeSelectionSize(MouseEventArgs e)
        {
            if (_main._selectionType == SelectionType.Rectangle)
            {
                if (!IfRotationOutOfBordersRectSelection(e)) return;
                _main._firstSelectionEnd = new Point(e.GetPosition(DrawingCanvas).X, e.GetPosition(DrawingCanvas).Y);

                double widthDiffer = _main._firstSelectionEnd.X - _main._firstSelectionStart.X;
                double heightDiffer = _main._firstSelectionEnd.Y - _main._firstSelectionStart.Y;

                SetObgjContent(new Size(Math.Abs(widthDiffer), Math.Abs(heightDiffer)));
            }
            else if (_main._selectionType == SelectionType.Custom && !(_main._selectionLine is null))
            {
                if (!(_main._selection is null)) return;
                _main.IfPointOutOfBorderCustomSelection(e, DrawingCanvas);
                _main._firstSelectionEnd = new Point(e.GetPosition(DrawingCanvas).X, e.GetPosition(DrawingCanvas).Y);

                Point point = _main.GetHighestPoints(_main._selectionLine);

                SetObgjContent(new Size(Math.Abs(point.X), Math.Abs(point.Y)));
            }
        }
        private bool IfRotationOutOfBordersRectSelection(MouseEventArgs e) //+-, check name
        {
            double xLoc = -1;
            double yLoc = -1;
            if (FigureRotation(e, _main._selectionRect))
            {
                double x = e.GetPosition(DrawingCanvas).X;
                double y = e.GetPosition(DrawingCanvas).Y;
                bool ifOutOfBorders = false;
                if (x <= 0)
                {
                    xLoc = 0;
                    ifOutOfBorders = true;
                    _main._selectionRect.Width = _main._firstSelectionStart.X;
                    Canvas.SetLeft(_main._selectionRect, xLoc);
                }
                else if (x > DrawingCanvas.Width)
                {
                    xLoc = DrawingCanvas.Width;
                    ifOutOfBorders = true;
                    double width = DrawingCanvas.Width - _main._firstSelectionStart.X;
                    _main._selectionRect.Width = width;
                }
                if (y <= 0)
                {
                    yLoc = 0;
                    ifOutOfBorders = true;
                    _main._selectionRect.Height = _main._firstSelectionStart.Y;
                    Canvas.SetTop(_main._selectionRect, yLoc);
                }
                else if (y > DrawingCanvas.Height)
                {
                    yLoc = DrawingCanvas.Height;
                    ifOutOfBorders = true;
                    double height = DrawingCanvas.Height - _main._firstSelectionStart.Y;
                    _main._selectionRect.Height = height;
                }
                if (ifOutOfBorders)
                {
                    _main._firstSelectionEnd = new Point(
                        xLoc != -1 ? xLoc : x,
                        yLoc != -1 ? yLoc : y);
                    return false;
                }
            }
            return true;
        }

        private bool CheckForSpace()
        {
            if (Keyboard.IsKeyDown(Key.Space) || Keyboard.IsKeyDown(Key.Escape))
            {
                ClearFigure();
                ClearAfterFastToolButPressed();
                return true;
            }
            return false;
        }

        private void ClearFigure()
        {
            DrawingCanvas.Children.Remove(_main._figToPaint);
            DrawingCanvas.Children.Remove(_main._polyline);

            _main._figToPaint = null;
            _main._polyline = null;

            ReloadCurvePainting();
        }

        private void PaintFigures(MouseEventArgs e)
        {
            const int minPointsAmount = 1;
            if (CheckForSpace()) return;

            if (_main._figType == FigureTypes.Line)
            {
                if (_main._figToPaint is null) return;

                Point startPoint = new Point(0, 0);
                Point endPoint = new Point
                (e.GetPosition(DrawingCanvas).X - Canvas.GetLeft(_main._figToPaint),
                e.GetPosition(DrawingCanvas).Y - Canvas.GetTop(_main._figToPaint));

                _main.InitPointInLine(startPoint, endPoint, _main._figToPaint as Polyline);

                if (((Polyline)_main._figToPaint).Points.Count > minPointsAmount)
                {
                    Point lastPoint = ((Polyline)_main._figToPaint).Points.Last();

                    SetObgjContent(new Size(Math.Abs(lastPoint.X), Math.Abs(lastPoint.Y)));
                }
            }
            else if (_main._figType == FigureTypes.Polygon)
            {
                const int lastIndexSubst = 1;

                if (_main._figToPaint is null) return;
                if (_main._amountOfPointInPolygon > 0 && !(_main._figToPaint is System.Windows.Shapes.Path) &&
                    _main._amountOfPointInPolygon != ((Polyline)_main._figToPaint).Points.Count)
                {
                    ((Polyline)_main._figToPaint).Points.RemoveAt(((Polyline)_main._figToPaint).Points.Count - lastIndexSubst);
                    ((Polyline)_main._figToPaint).Points.RemoveAt(((Polyline)_main._figToPaint).Points.Count - lastIndexSubst);
                }
                if (_main._amountOfPointInPolygon == 0)
                {
                    ((Polyline)_main._figToPaint).Points = new PointCollection();
                    Point startPoint = new Point(0, 0);
                    Point endPoint = new Point(e.GetPosition(DrawingCanvas).X - Canvas.GetLeft(_main._figToPaint),
                        e.GetPosition(DrawingCanvas).Y - Canvas.GetTop(_main._figToPaint));
                    _main.InitPointInLine(startPoint, endPoint, _main._figToPaint as Polyline);
                }
                else
                {
                    Point startPoint = ((Polyline)_main._figToPaint).Points.Last();
                    Point endPoint = new Point(e.GetPosition(DrawingCanvas).X - Canvas.GetLeft(_main._figToPaint),
                         e.GetPosition(DrawingCanvas).Y - Canvas.GetTop(_main._figToPaint));
                    _main.InitPointInLine(startPoint, endPoint, _main._figToPaint as Polyline);
                }
            }
            else if (_main._figType == FigureTypes.Curve)
            {
                const int seconPointInCureveIndex = 1;
                if (_main._isDrawingLine)
                {
                    Point currentPoint = e.GetPosition(DrawingCanvas);
                    if (_main._polyline.Points.Count > minPointsAmount)
                        _main._polyline.Points[seconPointInCureveIndex] = currentPoint; //-
                    else
                        _main._polyline.Points.Add(currentPoint);

                    if (_main._polyline.Points.Count > minPointsAmount)
                    {
                        _main.InitPointInLine(_main._polyline.Points.First(),
                            _main._polyline.Points.Last(), _main._polyline as Polyline);
                        Point first = _main._polyline.Points.First();
                        Point last = _main._polyline.Points.Last();
                        ObjSize.Content = $"{Math.Abs(last.X - first.X)} x {Math.Abs(last.Y - first.Y)}";
                    }
                }
                else if (_main._isAdjustingCurve)
                {
                    Point currentPoint = e.GetPosition(DrawingCanvas);
                    if (_main._bezPoints.Count == 0) _main._bezierSegment.Point1 = currentPoint;
                    else _main._bezierSegment.Point2 = currentPoint;

                    RenderOptions.SetEdgeMode(_main._figToPaint, EdgeMode.Aliased);
                    System.Windows.Shapes.Path curveFig =
                        _main._figToPaint as System.Windows.Shapes.Path;
                    Rect bounds = curveFig.Data.Bounds;

                    GeneralTransform transform = curveFig.TransformToAncestor((Visual)curveFig.Parent);
                    Rect transformedBounds = transform.TransformBounds(bounds);
                    ObjSize.Content = $"{Math.Abs((int)transformedBounds.Width)} x " +
                        $"{Math.Abs((int)transformedBounds.Height)}";
                }
            }
            else if (!(_main._figToPaint is null))
            {
                if (_main._figToPaint is null) return;
                FigureRotation(e, _main._figToPaint);
            }
        }

        private bool FigureRotation(MouseEventArgs e, Shape shape)
        {
            const int theLowestDiffer = 1;
            Point loc = new Point
             (Math.Min(_main.previousPoint.X, e.GetPosition(DrawingCanvas).X),
             Math.Min(_main.previousPoint.Y, e.GetPosition(DrawingCanvas).Y));

            Point sizePoint = e.GetPosition(DrawingCanvas);
            bool keyCheck = Keyboard.IsKeyDown(Key.LeftShift);
            if (keyCheck)
            {
                (loc, sizePoint) = _main.SetPositionForFigureWithShift(loc, sizePoint);
            }
            SetPositionForFigureWithoutShift(shape, loc.X, loc.Y);

            double width = Math.Abs(sizePoint.X - _main.previousPoint.X);
            double height = Math.Abs(sizePoint.Y - _main.previousPoint.Y);

            if (width < theLowestDiffer ||
                height < theLowestDiffer) return true;
            shape.Width = width;
            shape.Height = height;

            if (SetSizeIfShapeIsSelectionRect(shape)) return true;

            SetObgjContent(new Size(Math.Abs(shape.Width), Math.Abs(shape.Height)));
            return true;
        }

        private bool SetSizeIfShapeIsSelectionRect(Shape shape)
        {
            if (shape == _main._selectionRect)
            {
                int widthDiffer = (int)_main._firstSelectionEnd.X - (int)_main._firstSelectionStart.X;
                int heightDiffer = (int)_main._firstSelectionEnd.Y - (int)_main._firstSelectionStart.Y;

                SetObgjContent(new Size(Math.Abs(widthDiffer), Math.Abs(heightDiffer)));
                return true;
            }
            return false;
        }

        private void SetPositionForFigureWithoutShift(Shape shape, double x, double y)
        {
            if (shape is null) return;
            Canvas.SetLeft(shape, x);
            Canvas.SetTop(shape, y);
        }

        private void BrushPaint(MouseEventArgs e) //-
        {
            DrawingCanvas.RenderTransform = new TranslateTransform(0, 0);
            _main.currentPoint = e.GetPosition(DrawingCanvas);

            if (_main._tempBrushType == BrushType.UsualBrush || _main._ifErasing || Cursor == _main._pencilCurs)
            {
                _main.GetLineToPaint(_main.currentPoint, e, DrawingCanvas, CheckRect);
            }
            else if (_main._tempBrushType == BrushType.CalligraphyBrush)
            {
                SetCaligraphyBrush(e, CalligraphyBrushAngle);

                /*               _main.CalligraphyBrushPaint(CalligraphyBrushAngle, DrawingCanvas, CheckRect);
                               _main.AddCheckRect(e, DrawingCanvas, CheckRect);*/
            }
            else if (_main._tempBrushType == BrushType.FountainPen)
            {
                SetCaligraphyBrush(e, FountainBrushAngle);
                /*                _main.CalligraphyBrushPaint(FountainBrushAngle, DrawingCanvas, CheckRect);
                                _main.AddCheckRect(e, DrawingCanvas, CheckRect);*/
            }
            else if (_main._tempBrushType == BrushType.Spray)
            {
                sprayTimer.Start();
                _main.SprayPaint(_main.currentPoint, DrawingCanvas, CheckRect);
                _main.AddCheckRect(e, DrawingCanvas, CheckRect);
            }
            else if (_main._tempBrushType == BrushType.OilPaintBrush)
            {
                SetPngBrush(e);

                /*                PaintByPngBrush(e);
                                _main.AddCheckRect(e, DrawingCanvas, CheckRect);*/
            }
            else if (_main._tempBrushType == BrushType.ColorPencil)
            {
                SetPngBrush(e);

                /*                PaintByPngBrush(e);
                                _main.AddCheckRect(e, DrawingCanvas, CheckRect);*/
            }
            else if (_main._tempBrushType == BrushType.Marker)
            {
                _main.MarkerBrushPaint(e, DrawingCanvas);
                CheckForMarkerAmountOfPoints();
                _main.AddCheckRect(e, DrawingCanvas, CheckRect);
            }
            else if (_main._tempBrushType == BrushType.TexturePencil)
            {
                SetPngBrush(e);
                /*  PaintByPngBrush(e);
                  _main.AddCheckRect(e, DrawingCanvas, CheckRect);*/
            }
            else if (_main._tempBrushType == BrushType.WatercolorBrush)
            {
                SetPngBrush(e);

                /*                PaintByPngBrush(e);
                                _main.AddCheckRect(e, DrawingCanvas, CheckRect);*/
            }
            _main.previousPoint = _main.currentPoint;
            DrawingCanvas.RenderTransform = new TranslateTransform(_main._horizontalOffset, 0);
        }

        private void SetPngBrush(MouseEventArgs e)
        {
            PaintByPngBrush(e);
            _main.AddCheckRect(e, DrawingCanvas, CheckRect);
        }

        private void SetCaligraphyBrush(MouseEventArgs e, double angle)
        {
            _main.CalligraphyBrushPaint(angle, DrawingCanvas, CheckRect);
            _main.AddCheckRect(e, DrawingCanvas, CheckRect);
        }

        private void CheckForMarkerAmountOfPoints()
        {
            int count = 0;
            int pointToSetBg = (int)DrawingCanvas.Height;

            for (int i = 0; i < _main.polylines.Count; i++)
            {
                count += _main.polylines[i].Points.Count;
            }
            if (count >= pointToSetBg)
            {
                _main.SetCanvasBg(DrawingCanvas, CheckRect);
                DrawingCanvas.Children.Clear();

                Polyline line = _main.polylines.Last();
                Point lastPoint = line.Points.Last();
                line.Points.Clear();
                line.Points.Add(lastPoint);

                _main.polylines.Clear();

                DrawingCanvas.Children.Add(line);
                _main.polylines.Add(line);
            }
        }

        private void PaintByPngBrush(MouseEventArgs e)
        {
            DrawingCanvas.Children.Remove(CheckRect);
            const int centerDivider = 2;
            Point currentPoint = e.GetPosition(DrawingCanvas);

            if (_main._paintBrush is null) return;

            Image res = new Image()
            {
                Source = _main._paintBrush.ImageSource,
                Height = _main.brushThickness,
                Width = _main.brushThickness
            };

            Canvas.SetLeft(res, currentPoint.X - _main.brushThickness / centerDivider);
            Canvas.SetTop(res, currentPoint.Y - _main.brushThickness / centerDivider);
            DrawingCanvas.Children.Add(res);
            DrawingCanvas.Background = new ImageBrush()
            {
                ImageSource = _main.ConvertCanvasInImage(DrawingCanvas).Source
            };
            DrawingCanvas.Children.Clear();
        }

        const int _dividerInMiddle = 2;
        private void InitializeSprayTimer()
        {
            const int sprayerTime = 50;
            sprayTimer = new DispatcherTimer();
            sprayTimer.Interval = TimeSpan.FromMilliseconds(sprayerTime);
            sprayTimer.Tick += SprayTimer_Tick;
        }

        private void SprayTimer_Tick(object sender, EventArgs e)
        {
            _main.SprayPaint(_main.currentPoint, DrawingCanvas, CheckRect);
        }

        private void ConnectEveryPointOfSelectionLine()
        {
            PointCollection points = new PointCollection();
            for (int i = 0; i < _main._selectionLine.Points.Count; i++)
            {
                Point tempPoint = _main._selectionLine.Points[i];
                if (points.Count == 0 ||
                    IfTwoPointsAreCloseToEachOther(points.Last(), tempPoint))
                {
                    points.Add(_main._selectionLine.Points[i]);
                }
                else
                {
                    List<(int, int)> connetcionPoints = _main.GetLinePoints(
                        (int)points.Last().X, (int)points.Last().Y,
                        (int)tempPoint.X, (int)tempPoint.Y);
                    for (int j = 0; j < connetcionPoints.Count; j++)
                    {
                        points.Add(new Point(connetcionPoints[j].Item1, connetcionPoints[j].Item2));
                    }
                }
            }
            _main._selectionLine.Points = points;
            _main.RemoveEqualPoints(_main._selectionLine);
        }

        public bool IfTwoPointsAreCloseToEachOther(Point first, Point second) //-, for?
        {
            //const int closeDivision = 1;
            //const int circle = 8;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;
                    if (first.X + i == second.X && first.Y + j == second.Y) return true;
                }
            }

            return false;

            //return (first.X - closeDivision == second.X && first.Y - closeDivision == second.Y) || //up left
            //       (first.X == second.X && first.Y - closeDivision == second.Y) ||  //up
            //       (first.X + closeDivision == second.X && first.Y - closeDivision == second.Y) || //Up right
            //       (first.X + closeDivision == second.X && first.Y == second.Y) || //right
            //       (first.X + closeDivision == second.X && first.Y + closeDivision == second.Y) || //bottom right
            //       (first.X == second.X && first.Y + closeDivision == second.Y) || //bottom
            //       (first.X - closeDivision == second.X && first.Y + closeDivision == second.Y) || //bottom left
            //       (first.X - closeDivision == second.X && first.Y == second.Y);// left 
        }

        private void Field_MouseUp(object sender, MouseEventArgs e)
        {
            if (!(CheckRect.Parent is null))
            {
                DrawingCanvas.Children.Remove(CheckRect);
            }
            if (_main._ifSelection && !_main.IfSelectionIsMaken)
            {
                if (_main._selectionType == SelectionType.Custom) ConnectEveryPointOfSelectionLine();

                _main.SetImageComporator(DrawingCanvas);
                _main.MakeSelection(DrawingCanvas, BrushSizeLB, TransSelectionIcon, CheckRect, DraiwngScroll);
            }

            if (_main._ifFiguring) FiguringMouseUp();
            if (_main._ifDrawing || _main._ifErasing)
            {
                if (_main._spaceDrawingPressed)
                {
                    check = false;
                }
                if (_main._ifErasing) ConvertPaintingInImage();
                _main.SetCanvasBg(DrawingCanvas, CheckRect);
            }
            CheckForFigurePainting();

            sprayTimer.Stop();

            if (!IfDrawingCanvasContainsRightClickMenus())
            {
                SaveInHistory();
            }

            _main.MakeAllActionsNegative();

            if (!(CheckRect is null) && !(CheckRect.Parent is null))
            {
                ((Canvas)CheckRect.Parent).Children.Remove(CheckRect);
                DrawingCanvas.Children.Add(CheckRect);
            }
        }

        private void SaveInHistory()
        {
            if (!_main._ifFiguring &&
                !_main._ifChangingFigureSize &&
                !_main._ifSelection && _main._selection is null &&
                _changedSizeText is null)
            {
                SaveCanvasState();
            }
        }

        public void FiguringMouseUp()
        {
            bool polygonCheck = _main.IfPolygonFigureIsDone(DrawingCanvas);
            bool curveCheck = IfCurveFigureIsDone();
            if (polygonCheck && curveCheck)
            {
                InitFigureInSizingBorder();
                ReloadCurvePainting();
            }
        }

        private void ReloadCurvePainting()
        {
            _main._ifCurveIsDone = false;
            _main._isDrawingLine = false;
            _main._isAdjustingCurve = false;
        }

        public bool IfCurveFigureIsDone()
        {
            if (_main._figType != FigureTypes.Curve) return true;

            if (!(_main._bezPoints is null) && _main._bezPoints.Count == 0)
            {
                _main._bezPoints.Add(_main._bezierSegment.Point1);
                return false;
            }

            if (_main._ifCurveIsDone) SetPathSize();
            return _main._ifCurveIsDone;
        }

        public void SetPathSize()
        {
            const int _amountOfParalelBorders = 2;
            if (_main._figToPaint is System.Windows.Shapes.Path path)
            {
                Rect bounds = path.Data.Bounds;
                GeneralTransform transform = path.TransformToAncestor((Visual)path.Parent);
                Rect transformedBounds = transform.TransformBounds(bounds);
                _main._figToPaint.Width = transformedBounds.Width + _amountOfParalelBorders;
                _main._figToPaint.Height = transformedBounds.Height + _amountOfParalelBorders;
            }
        }

        public void GetBoundingBox(UserControl control) //-, linq
        {
            const int correlLoc = 1;
            if (_main._figToPaint is Polyline polyline)
            {
                double minX = double.MaxValue;
                double minY = double.MaxValue;

                minX = polyline.Points.Min(x => x.X);
                minY = polyline.Points.Min(x => x.Y);

                /*                foreach (var point in polyline.Points)
                                {
                                    if (point.X < minX) minX = point.X;
                                    if (point.Y < minY) minY = point.Y;
                                }*/

                Point polylineStartPoint = new Point(minX, minY);
                Point canvasPosition = polyline.TransformToAncestor(DrawingCanvas).Transform(polylineStartPoint);

                Canvas.SetLeft(control, canvasPosition.X);
                Canvas.SetTop(control, canvasPosition.Y);
            }
            else if (_main._figToPaint is System.Windows.Shapes.Path path)
            {
                Rect bounds = path.Data.Bounds;
                Point pathStartPoint = new Point(bounds.X, bounds.Y);
                Point canvasPosition = path.TransformToAncestor(DrawingCanvas).Transform(pathStartPoint);
                Canvas.SetLeft(_main._selection, canvasPosition.X - correlLoc);
                Canvas.SetTop(_main._selection, canvasPosition.Y - correlLoc);
            }
        }

        private void InitFigureInSizingBorder()
        {
            const int lastIndexSubst = 1;
            Point startPoint = new Point(0, 0);
            const int selLineThick = 2;

            if (DrawingCanvas.Children.Count == 0) return;
            if (!(DrawingCanvas.Children[DrawingCanvas.Children.Count - lastIndexSubst] is Shape)) return;
            Shape createdFigure = (Shape)DrawingCanvas.Children[DrawingCanvas.Children.Count - lastIndexSubst];

            if (_main._figType != FigureTypes.Line &&
                _main._figType != FigureTypes.Curve)
            {
                if (createdFigure.Width is double.NaN || createdFigure.Height is double.NaN)
                {
                    GetShapeSize(createdFigure);
                    if (!IfFigureSizeIsAcceptable(createdFigure)) return;
                }
                else
                {
                    if (!IfFigureSizeIsAcceptable(createdFigure))
                    {
                        _main._figToPaint = null;
                        _main._ifFiguring = true;
                        return;
                    }
                }
            }
            if (_main._figType == FigureTypes.Line)
            {
                _lineSizing = new LineSizing((Polyline)_main._figToPaint, ObjSize);
                GetLinePositionOnCanvas();
                DrawingCanvas.Children.RemoveAt(DrawingCanvas.Children.Count - lastIndexSubst);
                DrawingCanvas.Children.Add(_lineSizing);
                _lineSizing.ClipOutOfBoundariesGeo();
            }
            else
            {
                createdFigure.UpdateLayout();
                _main._selection = new Selection(ObjSize, createdFigure);

                _main._selection._shape.ClipToBounds = true;

                InitLocationForFigure(_main._selection);

                Point selPoint = new Point(Canvas.GetLeft(_main._selection), Canvas.GetTop(_main._selection));
                if (_main._figType == FigureTypes.Polygon)
                {
                    createdFigure.Height += selLineThick;
                    createdFigure.Width += selLineThick;
                    Canvas.SetLeft(_main._selection, selPoint.X - _main._selection.DashedBorder.StrokeThickness);
                    Canvas.SetTop(_main._selection, selPoint.Y - _main._selection.DashedBorder.StrokeThickness);
                }

                DrawingCanvas.Children.RemoveAt(DrawingCanvas.Children.Count - lastIndexSubst);
                _main._selection.BgCanvas.Children.Add(createdFigure);
                //_main._selection.SelectCan.ClipToBounds = true;

                createdFigure.Stretch = Stretch.Fill;
                Canvas.SetLeft(createdFigure, startPoint.X);
                Canvas.SetTop(createdFigure, startPoint.Y);

                DrawingCanvas.Children.Add(_main._selection);
                _main._selection.ClipOutOfBoundariesGeo();
            }
            _main._type = ActionType.ChangingFigureSize;
            DrawingCanvas.ClipToBounds = false;
        }

        private bool IfFigureSizeIsAcceptable(Shape createdFigure)
        {
            const int minSizeParam = 0;
            if (createdFigure.Width is double.NaN || createdFigure.Width == minSizeParam ||
                   createdFigure.Height is double.NaN || createdFigure.Height == minSizeParam)
            {
                DrawingCanvas.Children.RemoveAt(DrawingCanvas.Children.Count - 1);
                return false;
            }
            return true;
        }

        public void GetLinePositionOnCanvas()
        {
            if (DrawingCanvas.Children[DrawingCanvas.Children.Count - 1] is Polyline line)
            {
                double xLoc = Canvas.GetLeft(line);
                double yLoc = Canvas.GetTop(line);

                Canvas.SetLeft(_lineSizing, xLoc);
                Canvas.SetTop(_lineSizing, yLoc);
            }
        }

        private void InitLocationForFigure(UserControl control)
        {
            if (_main._figType != FigureTypes.Polygon &&
                _main._figType != FigureTypes.Curve &&
                _main._figType != FigureTypes.Line)
            {
                Canvas.SetTop(_main._selection, Canvas.GetTop(DrawingCanvas.Children
                    [DrawingCanvas.Children.Count - 1]));
                Canvas.SetLeft(_main._selection, Canvas.GetLeft(DrawingCanvas.Children
                    [DrawingCanvas.Children.Count - 1]));
                return;
            }
            GetBoundingBox(control);
        }

        private void GetShapeSize(Shape shape)
        {
            if (shape is Polyline poly && poly.Points.Count > 0)
            {
                shape.Width = Math.Abs(poly.Points.Last().X -
                 poly.Points.First().X);
                shape.Height = Math.Abs(poly.Points.Last().Y -
                    poly.Points.First().Y);
            }
        }

        public void UpdateWhiteColorInCanvas()
        {
            _main._selection.CheckCan.Children.Remove(_main._selectionLine);

            Image bgImage = _main.ConvertBackgroundToImage(_main._selection.SelectCan);
            Image res = _main.SwipeColorsInImage(bgImage, _main._whiteColor, _main._transparentColor);
            _main._selection.BgCanvas.Background = new ImageBrush()
            {
                ImageSource = res.Source
            };
            _main._selection.CheckCan.Children.Add(_main._selectionLine);
        }

        public void RemoveFirstImageFromCanvas(Canvas canvas)
        {
            for (int i = 0; i < canvas.Children.Count; i++)
            {
                if (canvas.Children[i] is Image)
                {
                    canvas.Children.RemoveAt(i);
                    return;
                }
            }
        }

        private void CheckForFigurePainting()
        {
            if (_main._figToPaint is null) return;
            if (_main._figType != FigureTypes.Polygon && !(_main._bezPoints is null) && _main._bezPoints.Count > 1)
            {
                _main._figToPaint = null;
            }
            else if (_main._figType == FigureTypes.Polygon)
            {
                _main._amountOfPointInPolygon = ((Polyline)_main._figToPaint).Points.Count;
            }
        }

        private void ValueCanvas_PreViewMouseUp(object sender, MouseEventArgs e)
        {
            valueDragElem = null;
            ValueCanvas.ReleaseMouseCapture();
            Check.IsOpen = false;
        }

        private void ValueCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            SetStartPositionForBrushSizer(draggableButton);
            var position = e.GetPosition(ValueCanvas);
            SetBrushSizeBrushPoint(position);
        }

        private void ValueCanvas_PreViewMouseMove(object sender, MouseEventArgs e)
        {
            if (valueDragElem == null) return;
            var position = e.GetPosition(sender as IInputElement);
            SetBrushSizeBrushPoint(position);
            Check.IsOpen = true;
        }

        private void SetBrushSizeBrushPoint(Point position)
        {
            if (position.Y < draggableButton.Height / _dividerInMiddle)
            {
                position.Y = draggableButton.Height / _dividerInMiddle;
            }
            if (position.Y > ValueCanvas.Height - draggableButton.Height / _dividerInMiddle)
            {
                position.Y = ValueCanvas.Height - draggableButton.Height / _dividerInMiddle;
            }

            Canvas.SetTop(valueDragElem, position.Y - valueOffset.Y);
            Check.VerticalOffset = position.Y - valueOffset.Y - ValueCanvas.Height;
            CheckValuePopupForXPosition();
            ChangeProgressBarValue(position.Y - valueOffset.Y);
        }

        private void CheckValuePopupForXPosition()
        {
            const int leftOffset = -70;
            const int rightOfset = 30;
            Point windowPoint = new Point(this.Left, this.Top);

            if (windowPoint.X < rightOfset)
            {
                Check.HorizontalOffset = rightOfset;
            }
            else if (Check.HorizontalOffset > 0)
            {
                Check.HorizontalOffset = leftOffset;
            }
        }

        public void ChangeProgressBarValue(double pos)
        {
            const int maxValueInProgressBar = 100;
            const int brushAdder = 0;
            const int barHeight = 250;
            const int minBrushSize = 1;

            double onePointHeight = (ValueProgressBar.Height - draggableButton.Height) /
                ValueProgressBar.Maximum;
            double temp = (int)pos / onePointHeight;
            double height = barHeight - onePointHeight * temp;
            paintInBlueCan.Height = height;
            int brushSize = Math.Abs(((int)temp) - maxValueInProgressBar) + brushAdder;
            brushSize = brushSize == 0 ? minBrushSize : brushSize;

            _main.brushThickness = brushSize;
            BrushSizeLB.Content = $"{brushSize} px";
        }

        private double prevYPos;
        private bool IfMadeThickBigger = false;
        private void ValueDrag_PreViewMouseDown(object sender, MouseEventArgs e)
        {
            SetStartPositionForBrushSizer(sender as Button);
        }

        private void SetStartPositionForBrushSizer(Button sender) //-
        {
            Button button = sender as Button;
            Point buttonPosition = button.TransformToAncestor(ValueCanvas)
                                         .Transform(new Point(0, 0));

            double centerX = buttonPosition.X + (button.ActualWidth / _dividerInMiddle);
            double centerY = buttonPosition.Y + (button.ActualHeight / _dividerInMiddle);
            IfThinBiggerCheck(centerY);

            if ((IfMadeThickBigger || paintInBlueCan.Margin.Bottom > draggableButton.Height / _dividerInMiddle) && prevYPos != 0)
            {
                paintInBlueCan.Margin = new Thickness(0, paintInBlueCan.Margin.Top, 0,
                paintInBlueCan.Margin.Bottom);
            }
            /*            else if (prevYPos != 0 && paintInBlueCan.Margin.Bottom > draggableButton.Height / _dividerInMiddle)
                        {
                            paintInBlueCan.Margin = new Thickness(0, paintInBlueCan.Margin.Top, 0,
                            paintInBlueCan.Margin.Bottom);
                        }*/

            valueDragElem = sender as UIElement;
            valueOffset = new Point((int)centerX, (int)centerY);
            valueOffset.Y -= Canvas.GetTop(valueDragElem);
            ValueCanvas.CaptureMouse();
        }

        public void IfThinBiggerCheck(double butYCord)
        {
            if (prevYPos == 0)
            {
                prevYPos = butYCord;
            }

            IfMadeThickBigger = prevYPos - butYCord > 0 ? true :
                prevYPos - butYCord < 0 ? false : IfMadeThickBigger;
        }

        private void BrushType_Click(object sender, EventArgs e)
        {
            _main._tempCursor = null;
            if (sender is Button but)
            {
                ClearDynamicValues(brushClicked: true);
                BrushType? type = GetBrushTypeByType(but.Tag.ToString());

                if (!(type is null))
                {
                    _main._tempBrushType = (BrushType)type;
                    string picPath = GetPathToNewBrushTypePic();

                    BrushTypePic.Source = BitmapFrame.Create(new Uri(picPath));
                }
                if (!(type is null) && type == BrushType.Spray)
                {
                    _main._tempCursor = _main._sprayCurs;
                }
                else
                {
                    _main._tempCursor = null;
                }
            }
            _main._type = ActionType.Drawing;
            PaintButBordsInClickedColor(PaintingBut);
            BrushesMenu.Visibility = Visibility.Hidden;
        }

        public string GetPathToNewBrushTypePic()
        {
            DirectoryInfo baseDirectoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

            string imageDirectory = baseDirectoryInfo.Parent.Parent.FullName;
            string imagePath = System.IO.Path.Combine(imageDirectory, "Images");
            string brushDir = System.IO.Path.Combine(imagePath, "BrushType");
            return System.IO.Path.Combine(brushDir, $"{_main._tempBrushType.ToString()}.png");
        }

        public BrushType? GetBrushTypeByType(string brushName)
        {
            for (int i = 0; i <= (int)BrushType.WatercolorBrush; i++)
            {
                if (brushName == ((BrushType)i).ToString())
                {
                    return ((BrushType)i);
                }
            }
            return null;
        }

        private bool RemoveNotPaintedFiure()
        {
            if (_main._figToPaint is null && _main._isDrawingLine == false) return false;

            DrawingCanvas.Children.RemoveAt(DrawingCanvas.Children.Count - 1);
            _main._figToPaint = null;
            ReloadCurvePainting();
            return true;
        }

        private void PreviousCanvas_Click(object sender, EventArgs e)
        {
            if (RemoveNotPaintedFiure()) return;
            if (!RemoveSelectionFromHistory()) return;

            DrawingCanvas.RenderTransform =
            new TranslateTransform(0, 0);
            SolidColorBrush bgBrush = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));

            if (currentIndex > 0)
            {
                currentIndex--;
                DrawingCanvasBGFromHistory();
            }
            else if (currentIndex == 0)
            {
                DrawingCanvas.Background = bgBrush;
                currentIndex--;
                DrawingCanvas.Children.Clear();
            }
            DrawingCanvas.RenderTransform =
            new TranslateTransform(_main._horizontalOffset, 0);
        }

        private void NextCanvas_Click(object sender, EventArgs e)
        {
            if (SetNotEndedFigure()) return;

            DrawingCanvas.RenderTransform =
             new TranslateTransform(0, 0);

            if (currentIndex < _canvasHistory.Count - 1)
            {
                DrawingCanvas.Children.Clear();

                currentIndex++;
                DrawingCanvasBGFromHistory();
            }
            DrawingCanvas.RenderTransform =
            new TranslateTransform(_main._horizontalOffset, 0);
        }

        private void DrawingCanvasBGFromHistory()
        {
            Image img = _canvasHistory[currentIndex];

            DrawingCanvas.Background = new ImageBrush()
            {
                ImageSource = img.Source
            };
        }

        private bool SetNotEndedFigure()
        {
            if (_main._figType != FigureTypes.Curve &&
                _main._figType != FigureTypes.Polygon || _main._figToPaint is null) return false;

            if (_main._figType == FigureTypes.Polygon)
            {
                ((Polyline)_main._figToPaint).Points.Add(((Polyline)_main._figToPaint).Points.First());
            }
            if (_main._figType == FigureTypes.Curve ||
                _main._figType == FigureTypes.Polygon)
            {
                _main.SetCanvasBg(DrawingCanvas, CheckRect);
                DrawingCanvas.Children.Remove(_main._figToPaint);
                _main._figToPaint = null;
                ReloadCurvePainting();
            }
            return true;
        }

        private bool RemoveSelectionFromHistory()
        {
            if (_canvasHistory.Count == 0) return true;
            if (_main._selection is null && _lineSizing is null) return true;

            _main.RemoveSelection(DrawingCanvas, ObjSize, TransSelectionIcon);
            _main.MakeAllActionsNegative();

            _main._selection = null;
            _main._selectionLine = null;

            DrawingCanvas.Children.Remove(_lineSizing);
            _lineSizing = null;

            if (_main._type == ActionType.ChangingFigureSize ||
                _main._type == ActionType.Figuring)
            {
                _main._ifFiguring = true;
                _main._type = ActionType.Figuring;
                if (_main._figType == FigureTypes.Polygon)
                {
                    DrawingCanvas.Children.Remove(_main._figToPaint);
                    _main._figToPaint = null;
                }
            }
            if (currentIndex == -1) return false;
            DrawingCanvasBGFromHistory();
            return false;
        }

        private List<Image> _canvasHistory = new List<Image>();
        private int currentIndex = -1;
        public void SaveCanvasState()
        {
            if (_ifSaved) return;
            DrawingCanvas.RenderTransform =
            new TranslateTransform(0, 0);

            CorrectHistory();
            DrawingCanvas.Children.Clear();
            Image img = _main.ConvertCanvasInImage(DrawingCanvas);

            _canvasHistory.Add(img);
            currentIndex = _canvasHistory.Count - 1;

            _ifSaved = true;
            DrawingCanvas.RenderTransform =
            new TranslateTransform(_main._horizontalOffset, 0);
        }

        private void CorrectHistory()
        {
            List<Image> res = new List<Image>();
            for (int i = 0; i < _canvasHistory.Count; i++)
            {
                if (currentIndex >= i)
                {
                    res.Add(_canvasHistory[i]);
                }
            }
            _canvasHistory = res;
        }

        private void Figure_Click(object sender, EventArgs e)
        {
            _main._tempCursor = null;
            ClearDynamicValues();

            ClearCurveVals();
            if (sender is Button but)
            {
                but.BorderBrush = _clickedBorderColor;

                FigureTypes? figType = ConvertStringIntoEnum(but.Name);
                if (figType is null)
                {
                    MessageBox.Show("Cant find such fig type", "Mistake", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                _main._figType = figType;
            }
            _main._type = ActionType.Figuring;
        }

        public void ClearCurveVals()
        {
            _main._isAdjustingCurve = false;
            _main._polyline = null;
            _main._bezierSegment = null;
            _main._figToPaint = null;
            _main._ifCurveIsDone = false;
            _main._isDrawingLine = false;
            _main._bezPoints = null;

            DrawingCanvas.Children.Remove(_main._figToPaint);
        }

        public FigureTypes? ConvertStringIntoEnum(string figType)
        {
            for (int i = 0; i <= (int)FigureTypes.Lightning; i++)
            {
                if (figType == ((FigureTypes)i).ToString())
                {
                    return ((FigureTypes)i);
                }
            }
            return null;
        }

        private void AddSavedPath(string path)
        {
            if (_savedPaths.Count != 0 && _savedPaths.First() == path) return;
            _savedPaths.Add(path);
            _lastSaveTime = DateTime.Now;
        }

        private void SaveAs_Click(object sender, EventArgs e)
        {
            SaveFileNew();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (_savedPaths.Count == 0)
            {
                SaveFileNew();
                return;
            }
            FastSave();
        }

        private void FastSave()
        {
            PngBitmapEncoder pngImage = GetImageOfDrawingCanvas();
            using (FileStream fileStream = new FileStream(_savedPaths.Last(), FileMode.Create))
            {
                pngImage.Save(fileStream);
            }
            _ifThereAreUnsavedChangings = false;
        }

        private void SaveFileNew()
        {
            PngBitmapEncoder pngImage = GetImageOfDrawingCanvas();

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image|*.png";
            saveFileDialog.FileName = "IHateThisTask.png";
            if (saveFileDialog.ShowDialog() == true)
            {
                AddSavedPath(saveFileDialog.FileName);
                using (FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    pngImage.Save(fileStream);
                }
                _ifThereAreUnsavedChangings = false;
            }
        }

        private PngBitmapEncoder GetImageOfDrawingCanvas()
        {
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)DrawingCanvas.ActualWidth,
            (int)DrawingCanvas.ActualHeight, _dpiParam, _dpiParam, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(DrawingCanvas);

            PngBitmapEncoder pngImage = new PngBitmapEncoder();
            pngImage.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            return pngImage;
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            Image img = GetImageFile();
            if (img is null) return;

            DrawingCanvas.Children.Clear();
            DrawingCanvas.Children.Add(img);
            currentIndex = 0;
            _canvasHistory = new List<Image>();
            _main.SetCanvasBg(DrawingCanvas, CheckRect);

            Canvas.SetLeft(img, 0);
            Canvas.SetTop(img, 0);
        }

        private void ImportOnDrawingCanvas_Click(object sender, EventArgs e)
        {
            Image img = GetImageFile();
            if (img is null) return;
            //Delete Selection From Canvas
            DrawingCanvas.Children.Remove(_main._selection);
            //Set it in selection
            _main._selection = new Selection(ObjSize, DraiwngScroll)
            {
                Width = img.Width,
                Height = img.Height
            };
            _main._selection.SelectionBorder.Width = _main._selection.Width;
            _main._selection.SelectionBorder.Height = _main._selection.Height;
            _main._selection.BgCanvas.Background = new ImageBrush()
            {
                ImageSource = img.Source
            };
            Canvas.SetLeft(_main._selection, 0);
            Canvas.SetTop(_main._selection, 0);
            DrawingCanvas.Children.Add(_main._selection);
        }

        private Image GetImageFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                BitmapImage bitmap = new BitmapImage(new Uri(filePath));
                Image image = new Image
                {
                    Source = bitmap,
                    Width = bitmap.PixelWidth,
                    Height = bitmap.PixelHeight
                };
                if (image.Width > DrawingCanvas.Width)
                {
                    image.Width = DrawingCanvas.Width;
                }
                if (image.Height > DrawingCanvas.Height)
                {
                    image.Height = DrawingCanvas.Height;
                }
                return image;
            }
            return null;
        }

        private bool _ifThereAreUnsavedChangings = false;
        private void CreateNew_Click(object sender, EventArgs e)
        {
            SaveOffer save = new SaveOffer(_savedPaths.Count == 0 ?
                null : _savedPaths.Last(), GetImageOfDrawingCanvas());
            if (_ifThereAreUnsavedChangings)
            {
                save.ShowDialog();
                _ifThereAreUnsavedChangings = !save._ifClear;
            }
            if (save._ifClear)
            {
                _main.RemoveSelection(DrawingCanvas, ObjSize, TransSelectionIcon);
                _main._figToPaint = null;
                ReloadCurvePainting();
                IfSelectionContainsAndClearIt();
                DrawingCanvas.Children.Clear();
                DrawingCanvas.Background = new SolidColorBrush(Colors.White);
                _canvasHistory = new List<Image>();
                currentIndex = 0;
                SaveCanvasState();
            }
        }

        private void Color_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender is Button but)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    _main.FirstColor = new SolidColorBrush(TaskColor.HexToRGB(but.Background.ToString()));
                }
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    _main.SecondColor = new SolidColorBrush(TaskColor.HexToRGB(but.Background.ToString()));
                }
            }
        }

        private void Color_PreviewMouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            InitColorForChosen(sender as Button, e);
        }

        private void InitColorForChosen(Button but, MouseEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SetChosenColor(false, FirstColor, SecondColor, but);

                /*                if (_chosenToPaintButton is null ||
                                    _chosenToPaintButton == FirstColor)
                                {
                                    _main.FirstColor = new SolidColorBrush(
                                    TaskColor.HexToRGB(but.Background.ToString()));
                                    SetColorAndBut(but, FirstColor);
                                    SetColorToFigure();

                                    return;
                                }
                                _main.SecondColor = new SolidColorBrush(
                                TaskColor.HexToRGB(but.Background.ToString()));
                                SetColorAndBut(but, SecondColor);*/
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                SetChosenColor(true, SecondColor, FirstColor, but);
                /*
                                if (_chosenToPaintButton is null ||
                                    _chosenToPaintButton == FirstColor)
                                {
                                    _main.SecondColor = new SolidColorBrush(
                                    TaskColor.HexToRGB(but.Background.ToString()));
                                    SetColorAndBut(but, SecondColor);
                                    return;
                                }
                                _main.FirstColor = new SolidColorBrush(
                                TaskColor.HexToRGB(but.Background.ToString()));
                                SetColorAndBut(but, FirstColor);*/
                SetColorToFigure();
            }
        }

        private void SetChosenColor(bool ifFirst, Button firstBut, Button secondBut, Button butColor)
        {
            if (_chosenToPaintButton is null ||
                 _chosenToPaintButton == FirstColor)
            {
                SolidColorBrush secondBrush = new SolidColorBrush(
                TaskColor.HexToRGB(butColor.Background.ToString()));

                if (!ifFirst)
                {
                    _main.FirstColor = secondBrush;
                }
                else _main.SecondColor = secondBrush;

                SetColorAndBut(butColor, firstBut);
                return;
            }
            SolidColorBrush chosenBrsuh = new SolidColorBrush(
            TaskColor.HexToRGB(butColor.Background.ToString()));

            if (ifFirst)
            {
                _main.FirstColor = chosenBrsuh;
            }
            else _main.SecondColor = chosenBrsuh;


            SetColorAndBut(butColor, secondBut);
        }

        public void SetColorToFigure()
        {
            if (!(_main._selection is null) &&
                !(_main._selection._shape is null))
            {
                _main._selection._shape.Stroke = _main.FirstColor;
            }
            if (!(_main._figToPaint is null))
            {
                _main._figToPaint.Stroke = _main.FirstColor;
            }
            if (!(_main._polyline is null))
            {
                _main._polyline.Stroke = _main.FirstColor;
            }
        }

        private void SetColorAndBut(Button but, Button chosenBut)
        {
            chosenBut.Background = but.Background;
            if (_chosenToPaintButton is null)
            {
                ChangedChosenColor(chosenBut);
            }
            if (!(_text is null)) _text.ChangeTextColor();
        }

        private void Color_PreviewMouseRightButtonDown(object sender, MouseEventArgs e)
        {
            InitColorForChosen(sender as Button, e);
        }

        private void CloseApp_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SquareSelection_Click(object sender, EventArgs e) //-
        {
            _main._tempCursor = null;
            ClearDynamicValues();
            if (sender is MenuItem item)
            {
                _main._selectionType = item.Name == ZeroSelection.Name ? SelectionType.Rectangle :
                item.Name == OneSelection.Name ? SelectionType.Custom :
                item.Name == TwoSelection.Name ? SelectionType.All :
                item.Name == ThreeSelection.Name ? SelectionType.Invert :
                item.Name == FourSelection.Name ? SelectionType.Transparent :
                item.Name == FiveSelection.Name ? SelectionType.Delete : SelectionType.Nothing;

                if (_main._selectionType == SelectionType.Rectangle) _savedType = _main._selectionType;
                if (_main._selectionType == SelectionType.Custom) _savedType = _main._selectionType;

                ChangeSelectionImage();
            }
            _main._type = ActionType.Selection;
            PaintButBordsInClickedColor(SelectionBut);
        }

        private void ChangeSelectionImage()
        {
            SelectionImg.Source = BitmapFrame.Create(new Uri(GetPathToNewSelectionType()));
        }

        public string GetSourceForNewSelectionType()
        {
            return _main._selectionType == SelectionType.Custom ? "Images/Selection/SelectForm.png" :
                 "Images/Selection/Select.png";
        }

        public string GetPathToNewSelectionType()
        {
            string type = _main._selectionType == SelectionType.Custom ?
                _main._selectionType.ToString() :
                SelectionType.Rectangle.ToString();

            DirectoryInfo baseDirectoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            string imageDirectory = baseDirectoryInfo.Parent.Parent.FullName;
            string imagePath = System.IO.Path.Combine(imageDirectory, "Images");
            string selectionDir = System.IO.Path.Combine(imagePath, "Selection");
            return System.IO.Path.Combine(selectionDir, $"{type}.png");
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (!(_main._selection is null) || _main._type == ActionType.Selection)
                Cursor = null;// _main._selection._tempCursor;

            if (_main._ifSelection)
            {
                ChangeSelectionSize(e);
            }
            if (!(_changedSizeText is null) && _changedSizeText._isDraggingSelection)
            {
                _changedSizeText.ChangeSizeForSelection(e);
            }
            if (!(_main._selection is null) &&
                _main._selection._isDraggingSelection)
            {
                _main._selection.ChangeSizeForSelection(e);
            }
            else if (!(_main._selection is null) && _main._ifChangingFigureSize &&
                e.LeftButton == MouseButtonState.Pressed)
            {
                _main._selection._isDraggingSelection = true;
                _main._selection.ChangeSizeForSelection(e);
            }
            else if (!(_lineSizing is null) &&
                e.LeftButton == MouseButtonState.Pressed)
            {
                _lineSizing.ChangeSizeForSelection(e);
            }
            if (_main._ifFiguring && (!(_main._figToPaint is null) ||
                 !(_main._pathFigure is null) || !(_main._polyline is null)))
            {
                PaintFigures(e);
            }
            if (!(_main._selection is null) &&
                DrawingCanvas.Children.Contains(_main._selection))
            {
                SetButtonActiveness(true);
            }
            else
            {
                SetButtonActiveness(false);
            }
        }

        private void SetButtonActiveness(bool ifActive) //-
        {
            SetMenuItemActiveness(CutChange, ifActive);
            SetMenuItemActiveness(CopyChange, ifActive);

            if (_clickMenu is null) return;
            SetButActiveness(_clickMenu.Copy, ifActive);
            SetButActiveness(_clickMenu.Cut, ifActive);
            CheckReverseForActiveness(ifActive);

            if (_copyBuffer is null) SetButActiveness(_clickMenu.Paste, false);

            SetButActiveness(_clickMenu.InvertSelection, ifActive);
        }
        /*        private void MakeButtonsInActive() //-
                {
                    MakeChangeButInActive(CutChange);
                    MakeChangeButInActive(CopyChange);

                    if (_clickMenu is null) return;
                    MakeChangeButInActive(_clickMenu.Copy);
                    MakeChangeButInActive(_clickMenu.Cut);
                    if (_copyBuffer is null) MakeChangeButInActive(_clickMenu.Paste);

                    CheckReverseForActiveness();
                    MakeChangeButInActive(_clickMenu.InvertSelection);
                }*/

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!(_main._selection is null))
            {
                _main._selection.ClearDragging();
                /*                _main._selection._isDraggingSelection = false;
                                _main._selection.IfSelectionIsClicked = false;
                                _main._selection._selectionSizeToChangeSize = SelectionSide.Nothing;*/
            }
            else if (!(_changedSizeText is null))
            {
                _changedSizeText.ClearDragging();
                /*                _changedSizeText._isDraggingSelection = false;
                                _changedSizeText.IfSelectionIsClicked = false;
                                _changedSizeText._selectionSizeToChangeSize = SelectionSide.Nothing;*/
            }
            else if (!(_lineSizing is null))
            {
                _lineSizing.ClearDragging();
                /*                _lineSizing._isDraggingSelection = false;
                                _lineSizing.IfSelectionIsClicked = false;
                                _lineSizing._moveRect = LineSizingRectType.Nothing;*/
            }
            if (!(_main._selection is null))
            {
                ClearCursorForSizing(_main._selection);
            }
            if (!(_changedSizeText is null))
            {
                ClearCursorForSizing(_changedSizeText);
            }
        }


        private void ClearCursorForSizing(Selection sel)
        {
            sel._ifMoveCursFlagouseDown = false;
            sel._tempCursor = null;
            sel.Cursor = null;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ResetRenderTransform();
            BrushesMenu.Visibility = Visibility.Hidden;

            ClearAdaptFigure();
            RemoveRightClickMenus();

            if (!IfSelectionContainsAndClearIt())
            {
                UpdateRenderTransform();
                return;
            }
            CheckForTextSizeChanging();
            if (!(_changedSizeText is null) && _changedSizeText.IfSelectiongClicked)
            {
                _changedSizeText.IfSelectiongClicked = false;
            }
            if (IfFIgureClickeOutOfDrawingCanvas(e))
            {
                DrawingCanvas.Children.Remove(CheckRect);
                bool polygonCheck = _main.IfPolygonFigureIsDone(DrawingCanvas);
                bool curveCheck = IfCurveFigureIsDone();

                if (polygonCheck && curveCheck)
                {
                    InitFigureInSizingBorder();
                    ReloadCurvePainting();
                }
            }
            UpdateRenderTransform();
        }

        private bool IfFIgureClickeOutOfDrawingCanvas(MouseEventArgs e)
        {
            const int minLinePoints = 2;
            if (_main._figToPaint is null || _main._figType != FigureTypes.Polygon) return false;
            Point pos = e.GetPosition(DrawingCanvas);

            if (_main.IfPointOutOfBoundaries(pos, DrawingCanvas) &&
               ((Polyline)_main._figToPaint).Points.Count >= minLinePoints)
            {
                ((Polyline)_main._figToPaint).Points.Add(((Polyline)_main._figToPaint).Points.First());
                return true;
            }
            return false;
        }

        private bool IfSelectionContainsAndClearIt()
        {
            bool check = true;
            ResetRenderTransform();

            if (!(_main._selection is null) && !_main._selection.IfSelectionIsClicked &&
               _main._type != ActionType.ChangingFigureSize)
            {
                ClearUsualSelection();
                return false;
            }
            if (!(_main._selection is null) && !_main._selection.IfSelectionIsClicked)
            {
                ClearSelectionWithFigure();
                check = false;
            }
            else if (!(_lineSizing is null) && !_lineSizing.IfSelectionIsClicked)
            {
                ClearLineSelection();
                check = false;
            }
            UpdateRenderTransform();
            return check;
        }

        private void ClearUsualSelection()
        {
            _main._selection.CheckCan.Children.Remove(_main._selectionLine);
            _main._selection.CheckCan.Children.Remove(_copyPolyLine);
            _main.FreeSelection(DrawingCanvas, CheckRect);
        }

        private void ClearSelectionWithFigure()
        {
            _main._selectionLine = null;
            _lineSizing = null;

            ClearFigureSizing();

            if (_main._type == ActionType.ChangingFigureSize)
                _main._type = ActionType.Figuring;
        }

        private void ClearLineSelection()
        {
            ClearLineSizing();
            UseAuxiliaryCanvasToCollectChildrenInDrawingCanvas();

            _main._ifChangingFigureSize = false;
            _main._selectionLine = null;
        }

        private void CheckForTextSizeChanging()
        {
            DrawingCanvas.RenderTransform = new TranslateTransform(0, 0);

            if (!(_changedSizeText is null) &&
                !_changedSizeText.IfSelectiongClicked && _ifDoubleClicked)
            {
                ConvertRichTextBoxIntoImage();
                _ifDoubleClicked = false;
                DrawingCanvas.Children.Remove(_changedSizeText);
                _changedSizeText = null;
                UseAuxiliaryCanvasToCollectChildrenInDrawingCanvas();
            }

            DrawingCanvas.RenderTransform = new TranslateTransform(_main._horizontalOffset, 0);
        }

        private bool _ifSaved = false;
        private void ClearFigureSizing()
        {
            ResetRenderTransform();
            if (!(_main._selection is null))
            {
                InitSizedFigureIntoCanvas();
                _main._selection = null;
                if (_main._type == ActionType.ChangingFigureSize) _main._type = ActionType.Figuring;
                _main._figToPaint = null;
            }

            if (_main._selection is null &&
                _main._type == ActionType.Figuring &&
                _main._figType == FigureTypes.Polygon &&
                !(_main._figToPaint is null))
            {
                SaveCanvasState();
                DrawingCanvas.Children.Add(_main._figToPaint);
                _main.SetCanvasBg(DrawingCanvas, CheckRect);
                _main._selection = null;
                _main._figToPaint = null;
            }
            else if (!_ifSaved)
            {
                if (!(_lineSizing is null))
                {
                    InitLineInCanvas();
                }
                _main.SetCanvasBg(DrawingCanvas, CheckRect);
                SaveCanvasState();
            }
            UpdateRenderTransform();
        }

        private void ClearLineSizing()
        {
            if (!(_lineSizing is null))
            {
                InitLineInCanvas();
                _lineSizing = null;
                _main._type = ActionType.Figuring;
                _main._figToPaint = null;
                RemoveLineSizing();
            }
        }

        private void ClearFigureSizingInClicks()
        {
            ResetRenderTransform();
            ClearFigureSizing();
            ClearLineSizing();
            ClearSquaresBorders();
            UpdateRenderTransform();
        }

        private void ClearSquaresBorders()
        {
            ClearBigSquares(SelectionBut);
            ClearBigSquares(PaintingBut);
        }

        private void ClearBigSquares(Button button)
        {
            button.BorderThickness = new Thickness(0);
            button.BorderBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }

        private void InitLineInCanvas()
        {
            if (DrawingCanvas.Children.Count == 0) return;

            Line lineToAdd = _lineSizing.GetLineObject();
            _lineSizing.RemoveLineFromCanvas();

            DrawingCanvas.Children.Remove(_lineSizing);
            DrawingCanvas.Children.Add(lineToAdd);

            Canvas.SetLeft(lineToAdd, Canvas.GetLeft(_lineSizing));
            Canvas.SetTop(lineToAdd, Canvas.GetTop(_lineSizing));

            _lineSizing = null;
            _main.SetCanvasBg(DrawingCanvas, CheckRect);
        }

        private void RemoveLineSizing()
        {
            for (int i = 0; i < DrawingCanvas.Children.Count; i++)
            {
                if (DrawingCanvas.Children[i] is LineSizing)
                {
                    DrawingCanvas.Children.RemoveAt(i);
                }
            }
        }

        private void InitSizedFigureIntoCanvas()
        {
            ResetRenderTransform();
            var deleteElem = _main._selection.GetShapeElement();

            if (deleteElem is null)
            {
                DrawingCanvas.Children.Clear();
                UpdateRenderTransform();
                return;
            }

            DrawingCanvas.Children.Clear();
            _main._selection.RemoveShapea();
            DrawingCanvas.Children.Add(deleteElem);

            Canvas.SetLeft(deleteElem, Canvas.GetLeft(_main._selection));
            Canvas.SetTop(deleteElem, Canvas.GetTop(_main._selection));

            UpdateRenderTransform();
        }

        private double _toolPartStartWidth;
        private double _figuresPartStartWidth;
        private double _figurePanelStartWidth;
        private double _colorsPartStartWidth;
        private double _startAdaptValue;
        private double _toolsAdaptInter;
        private Border _figurePanel;
        private WrapPanel _toolsPanel;
        private Grid _colorPanel;
        private void InitValueForSizeAdaptation()
        {
            const int toolLocMultiplier = 2;
            _startAdaptValue = SelectionPart.Width.Value + ToolsPart.Width.Value +
            BrushPart.Width.Value + FiguresPart.Width.Value + ColorsPart.Width.Value;

            _toolPartStartWidth = ToolsPart.Width.Value;
            _figuresPartStartWidth = FiguresPart.Width.Value;
            _figurePanelStartWidth = FigurePanel.Width;
            _colorsPartStartWidth = ColorsPart.Width.Value;

            _toolsAdaptInter = _colorsPartStartWidth * toolLocMultiplier;
            _figurePanel = FiguresBorder;
            _toolsPanel = ToolsPartPanel;
            _colorPanel = ColorPart;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e) //-
        {
            UpdateTextLocation();
            ClearAdaptFigure();
            const int _figuresStep = 44;

            if (this.Width > _startAdaptValue)
            {
                FiguresPart.Width = new GridLength(_figuresPartStartWidth);
                FigurePanel.Width = _figurePanelStartWidth;
                return;
            }

            AdaptationStages stage = (this.Width < _startAdaptValue &&
                this.Width > _startAdaptValue - _figuresStep * (int)AdaptationStages.FirstFigures) ? AdaptationStages.FirstFigures :
                (this.Width < _startAdaptValue - _figuresStep * (int)AdaptationStages.FirstFigures &&
                this.Width > _startAdaptValue - _figuresStep * (int)AdaptationStages.SecondFigures) ? AdaptationStages.SecondFigures :
                 (this.Width < _startAdaptValue - _figuresStep * (int)AdaptationStages.SecondFigures &&
                this.Width > _startAdaptValue - _figuresStep * (int)AdaptationStages.ThirdFigures) ? AdaptationStages.ThirdFigures :
                (this.Width < _startAdaptValue - _figuresStep * (int)AdaptationStages.ThirdFigures &&
                this.Width > _startAdaptValue - _figuresStep * (int)AdaptationStages.ForthFigures) ? AdaptationStages.ForthFigures : AdaptationStages.FiguresTransformation;

            //first step in adapt (figures changing)
            /*            if (this.Width < _startAdaptValue &&
                            this.Width > _startAdaptValue - _figuresStep * (int)AdaptationStages.FirstFigures)
                        {
                            DisableTransformationsAdapt();
                            MakeFiguresPartAdapt(AdaptationStages.FirstFigures, _figuresStep);
                        }
                        else if (this.Width < _startAdaptValue - _figuresStep * (int)AdaptationStages.FirstFigures &&
                            this.Width > _startAdaptValue - _figuresStep * (int)AdaptationStages.SecondFigures)
                        {
                            DisableTransformationsAdapt();
                            MakeFiguresPartAdapt(AdaptationStages.SecondFigures, _figuresStep);
                        }
                        else if (this.Width < _startAdaptValue - _figuresStep * (int)AdaptationStages.SecondFigures &&
                            this.Width > _startAdaptValue - _figuresStep * (int)AdaptationStages.ThirdFigures)
                        {
                            DisableTransformationsAdapt();
                            MakeFiguresPartAdapt(AdaptationStages.ThirdFigures, _figuresStep);
                        }
                        else if (this.Width < _startAdaptValue - _figuresStep * (int)AdaptationStages.SecondFigures &&
                            this.Width > _startAdaptValue - _figuresStep * (int)AdaptationStages.ThirdFigures)
                        {
                            DisableTransformationsAdapt();
                            MakeFiguresPartAdapt(AdaptationStages.ThirdFigures, _figuresStep);
                        }
                        else if (this.Width < _startAdaptValue - _figuresStep * (int)AdaptationStages.ThirdFigures &&
                            this.Width > _startAdaptValue - _figuresStep * (int)AdaptationStages.ForthFigures)
                        {
                            DisableTransformationsAdapt();
                            MakeFiguresPartAdapt(AdaptationStages.ForthFigures, _figuresStep);
                        }*/

            if (stage == AdaptationStages.FirstFigures || stage == AdaptationStages.SecondFigures ||
                stage == AdaptationStages.ThirdFigures || stage == AdaptationStages.ForthFigures)
            {
                DisableTransformationsAdapt();
                MakeFiguresPartAdapt(stage, _figuresStep);
            }
            else if (this.Width < _startAdaptValue - _figuresStep * (int)AdaptationStages.ForthFigures &&
                this.Width > _startAdaptValue - _figuresStep * (int)AdaptationStages.FiguresTransformation)
            {
                DisableColorTransformation();
                DisableToolsTransformation();

                MakeFigureTransformationInAdaptation();
            }
            else if (this.Width < _startAdaptValue - _figuresStep * (int)AdaptationStages.FiguresTransformation &&
                    this.Width > _toolsAdaptInter)
            {
                DisableColorTransformation();
                MakeToolsTransformation();
            }
            else if (this.Width < _toolsAdaptInter)
            {
                MakeColorTransformation();
            }
        }

        private void PaletteAdaptBut_Click(object sender, RoutedEventArgs e)
        {
            ClearAdaptFigure();

            double figPartXLoc = SelectionPart.ActualWidth +
                          BrushPart.ActualWidth;
            double figPartYLoc = MainPanel.ActualHeight;

            ClickButAdaptation(AdaptFigureBut, figPartXLoc, figPartYLoc, _figurePanel);

            SetFigCanvasToAdapt(_colorPanel, _colorPanel.ActualHeight, _colorPanel.ActualWidth);

            /*            FigureCanvasToAdapt.Children.Add(_colorPanel);
                        FigureCanvasToAdapt.Height = _colorPanel.ActualHeight;
                        FigureCanvasToAdapt.Width = _colorPanel.ActualWidth;*/
        }

        private void ToolsAdaptBut_Click(object sender, RoutedEventArgs e)
        {
            ClearAdaptFigure();

            double figPartXLoc = this.Width - (SelectionPart.ActualWidth +
                BrushPart.ActualWidth + ToolsPart.ActualWidth) - _toolsPanel.ActualWidth / 2;
            double figPartYLoc = MainPanel.ActualHeight;

            ClickButAdaptation(AdaptFigureBut, figPartXLoc, figPartYLoc, _figurePanel);

            double toolsWidth = _toolsPanel.ActualWidth;
            double toolsHeight = _toolsPanel.ActualHeight;

            SetFigCanvasToAdapt(_toolsPanel, toolsHeight, toolsWidth);
            /*
                        FigureCanvasToAdapt.Children.Add(_toolsPanel);
                        FigureCanvasToAdapt.Height = toolsHeight;
                        FigureCanvasToAdapt.Width = toolsWidth;*/
        }
        private void SetFigCanvasToAdapt(UIElement toAdd, double height, double width)
        {
            FigureCanvasToAdapt.Children.Add(toAdd);
            FigureCanvasToAdapt.Height = height;
            FigureCanvasToAdapt.Width = width;
        }

        private void AdaptFigureBut_Click(object sender, RoutedEventArgs e)
        {
            const double figAdaptMultiplier = 0.7;
            ClearAdaptFigure();
            double figPartYLoc = MainPanel.ActualHeight;

            ClickButAdaptation(AdaptFigureBut, 0, figPartYLoc, _figurePanel);

            FigureCanvasToAdapt.Children.Add(_figurePanel);

            SetAdaptationCanvSize(_figurePanelStartWidth, (FigGridPart.ActualHeight *
                figAdaptMultiplier), _figuresPartStartWidth);
        }

        private void SetAdaptationCanvSize(double figurePanelWidth, double canvasAdaptHeight, double canvasAdaptWidth)
        {
            FigurePanel.Width = figurePanelWidth;
            FigureCanvasToAdapt.Height = canvasAdaptHeight;
            FigureCanvasToAdapt.Width = canvasAdaptWidth;
        }

        private void ClickButAdaptation(Button but, double thicLeft, double thicTop, UIElement toChangeVisable)
        {
            ClearAdaptFigure();

            FigureCanvasToAdapt.Visibility = Visibility.Visible;
            toChangeVisable.Visibility = Visibility.Visible;

            Canvas.SetLeft(_figurePanel, 0);
            Canvas.SetTop(_figurePanel, 0);


            FigureCanvasToAdapt.Margin =
                new Thickness(thicLeft, thicLeft, 0, 0);
        }



        private void ClearAdaptFigure()
        {
            FigureCanvasToAdapt.Visibility = Visibility.Hidden;
            FigureCanvasToAdapt.Children.Clear();
        }

        private void DisableTransformationsAdapt()
        {
            DisableColorTransformation();
            DisableToolsTransformation();
            DisableTransformForAllAdaptation();
        }

        private const int _adaptedButsWidth = 85;
        private void DisableColorTransformation()
        {
            PaletteAdaptBut.Visibility = Visibility.Hidden;
            ColorPart.Visibility = Visibility.Visible;
            ColorsPart.Width = new GridLength(_colorsPartStartWidth);

            if (!ColorColGrid.Children.Contains(_colorPanel))
            {
                ColorColGrid.Children.Add(_colorPanel);
            }
        }

        //- и 4 следующих метода - придумать как сделать универсально
        private void MakeColorTransformation()
        {
            PaletteAdaptBut.Visibility = Visibility.Visible;
            ColorPart.Visibility = Visibility.Hidden;
            ColorsPart.Width = new GridLength(_adaptedButsWidth);


            ColorColGrid.Children.Remove(_colorPanel);
        }

        private void DisableToolsTransformation()
        {
            /*            ToolsPartPanel.Visibility = Visibility.Visible;
                        ToolsAdaptBut.Visibility = Visibility.Hidden;
                        ToolsPart.Width = new GridLength(_toolPartStartWidth);*/

            SetVisParams(ToolsPartPanel, Visibility.Visible, ToolsAdaptBut,
            Visibility.Hidden, ToolsPart, _toolPartStartWidth);

            if (!ToolsGrid.Children.Contains(_toolsPanel))
            {
                ToolsGrid.Children.Add(_toolsPanel);
            }
        }

        private void MakeToolsTransformation()
        {
            /*            ToolsPartPanel.Visibility = Visibility.Hidden;
                        ToolsAdaptBut.Visibility = Visibility.Visible;
                        ToolsPart.Width = new GridLength(_adaptedButsWidth);*/

            SetVisParams(ToolsPartPanel, Visibility.Hidden, ToolsAdaptBut,
            Visibility.Visible, ToolsPart, _adaptedButsWidth);

            ToolsGrid.Children.Remove(_toolsPanel);
        }

        private void MakeFigureTransformationInAdaptation()
        {
            /*            FiguresBorder.Visibility = Visibility.Hidden;
                        AdaptFigureBut.Visibility = Visibility.Visible;
                        FiguresPart.Width = new GridLength(_adaptedButsWidth);*/
            SetVisParams(FiguresBorder, Visibility.Hidden, AdaptFigureBut,
                Visibility.Visible, FiguresPart, _adaptedButsWidth);

            FigGridPart.Children.Remove(_figurePanel);
        }

        private void SetVisParams(UIElement first, Visibility firstVis,
            UIElement second, Visibility secondVis, ColumnDefinition elem, double sizeParam)
        {
            first.Visibility = firstVis;
            second.Visibility = secondVis;
            elem.Width = new GridLength(sizeParam);
        }

        private void DisableTransformForAllAdaptation()
        {
            FiguresBorder.Visibility = Visibility.Visible;
            AdaptFigureBut.Visibility = Visibility.Hidden;

            if (!FigGridPart.Children.Contains(_figurePanel))
            {
                FigGridPart.Children.Add(_figurePanel);
            }
            if (!ToolsGrid.Children.Contains(_toolsPanel))
            {
                ToolsGrid.Children.Add(_toolsPanel);
            }
            if (!ColorColGrid.Children.Contains(_colorPanel))
            {
                ColorColGrid.Children.Add(_colorPanel);
            }
        }

        private void MakeFiguresPartAdapt(AdaptationStages stage, int cutStep)
        {
            const int difference = 30;

            FiguresPart.Width = new GridLength(_figuresPartStartWidth - cutStep * (int)stage);
            double differ = FiguresPart.ActualWidth - difference;
            FigurePanel.Width = differ;
        }

        private bool _ifDoubleClicked = false;
        private void PaintWindow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_main._ifTexting && !_ifDoubleClicked)
            {
                CreateTextPart(e);
            }
        }

        private void CreateTextPart(MouseEventArgs e)
        {
            InitSettingsForTextBox();
            Point mousePoint = e.GetPosition(DrawingCanvas);

            Canvas.SetLeft(_changedSizeText, mousePoint.X);
            Canvas.SetTop(_changedSizeText, mousePoint.Y);

            DrawingCanvas.Children.Add(_changedSizeText);

            _changedSizeText.Tag = DrawingCanvas.Children.Count - 1;

            _richTexBox.FontFamily = _text._textSettings.ChosenFont;
            _richTexBox.FontSize = _text._textSettings.FontSize;
            _ifDoubleClicked = true;
        }

        private void InitSettingsForTextBox()
        {
            _changedSizeText = new Selection(ObjSize, DraiwngScroll);

            _changedSizeText.CheckCan.Children.Remove(_changedSizeText.BgCanvas);

            const int textboxBasHeight = 50;
            const int textboxBasWidth = 160;

            _changedSizeText.Height = textboxBasHeight;
            _changedSizeText.SelectionBorder.Height = textboxBasHeight;
            _changedSizeText.Width = textboxBasWidth;
            _changedSizeText.SelectionBorder.Width = textboxBasWidth;

            const int fontSize = 14;
            const int textBoxHeight = 40;
            const int textBoxWidth = 150;
            const int widthCorel = 10;
            const int startLoc = 5;

            _richTexBox = new RichTextBox()
            {
                Visibility = Visibility.Visible,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                FontSize = fontSize,
                AcceptsReturn = true,
                Height = textBoxHeight,
                Width = textBoxWidth
            };

            _richTexBox.Width = _changedSizeText.Width - widthCorel;
            _richTexBox.Height = _changedSizeText.Height - widthCorel;

            Canvas.SetLeft(_richTexBox, startLoc);
            Canvas.SetTop(_richTexBox, startLoc);

            _richTexBox.Focusable = true;
            _richTexBox.Focus();
            _changedSizeText.SelectCan.Children.Add(_richTexBox);

            _text.Visibility = Visibility.Visible;

            Dispatcher.InvokeAsync(() =>
            {
                _richTexBox.Focus();
                Keyboard.Focus(_richTexBox);
            }, DispatcherPriority.ApplicationIdle);

            _text.Visibility = Visibility.Visible;

            _text.InitForNewTextBox(_changedSizeText);
        }

        private void ConvertRichTextBoxIntoImage()
        {
            const int sizeCorel = 10;
            RichTextBox toSave = _changedSizeText.GetRichTextBoxObject();
            toSave.ScrollToHome();

            _richTexBox.Selection.Select(_richTexBox.Selection.Start, _richTexBox.Selection.Start);
            Keyboard.ClearFocus();

            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
            toSave.BorderThickness = new Thickness(0);
            if (toSave is null)
            {
                this.Focus();
                return;
            }
            double width = toSave.ActualWidth + sizeCorel;
            double height = toSave.ActualHeight + sizeCorel;

            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(
                (int)width, (int)height, _dpiParam, _dpiParam, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(toSave);
            Image img = new Image()
            {
                Source = renderTargetBitmap
            };

            Canvas.SetLeft(img, Canvas.GetLeft(_changedSizeText));
            Canvas.SetTop(img, Canvas.GetTop(_changedSizeText));
            DrawingCanvas.Children.Add(img);
            this.Focus();
        }

        public void EndWithPolygonFigures()
        {
            if (_main._figType == FigureTypes.Polygon && !(_main._figToPaint is null))
            {
                ((Polyline)_main._figToPaint).Points.Add(((Polyline)_main._figToPaint).Points.First());
            }
        }

        private void PaintWindow_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ResetRenderTransform();
            if (_main._ifFiguring)
            {
                FiguringMouseUp();
                CheckForFigurePainting();
            }
            if (!(_lineSizing is null))
            {
                _lineSizing._moveRect = LineSizingRectType.Nothing;
                _lineSizing._isDraggingSelection = false;
            }
            UpdateRenderTransform();
        }

        private void ConvertPaintingInImage()
        {
            if (DrawingCanvas.Children.Count == 0) return;
            var lastAddedObj = DrawingCanvas.Children[DrawingCanvas.Children.Count - 1];

            if (lastAddedObj is Line)
            {
                SwapLinesWithImages();
            }
            else if (lastAddedObj is Polygon)
            {
                SwapPolygonWithImage();
            }
            else if (lastAddedObj is Polyline)
            {
                SwapPolylineWIthImage();
            }
            UseAuxiliaryCanvasToCollectChildrenInDrawingCanvas();
        }

        private void SwapPolylineWIthImage()
        {
            List<Shape> res = GetAllPolylines().Select(x => (Shape)x).ToList();
            RemoveAllPolylinesFromDrawingCanvas();
            Image img = ConvertShapesToImage(res, DrawingCanvas.Width, DrawingCanvas.Height);

            Canvas.SetLeft(img, 0);
            Canvas.SetTop(img, 0);

            DrawingCanvas.Children.Add(img);
        }

        private void RemoveAllPolylinesFromDrawingCanvas()
        {
            List<Polyline> lines = DrawingCanvas.Children.OfType<Polyline>().ToList();

            foreach (var line in lines)
            {
                DrawingCanvas.Children.Remove(line);
            }
            /*
                        for (int i = 0; i < DrawingCanvas.Children.Count; i++)
                        {
                            if (DrawingCanvas.Children[i] is Polyline)
                            {
                                DrawingCanvas.Children.RemoveAt(i);
                                if (i != 0) i--;
                                else i = -1;
                            }
                        }*/
        }

        private List<Polyline> GetAllPolylines()
        {
            return DrawingCanvas.Children.OfType<Polyline>().ToList();
            /*            List<Polyline> res = new List<Polyline>();
                        for (int i = 0; i < DrawingCanvas.Children.Count; i++)
                        {
                            if (DrawingCanvas.Children[i] is Polyline)
                            {
                                res.Add((Polyline)DrawingCanvas.Children[i]);
                            }
                        }
                        return res;*/
        }

        private void SwapPolygonWithImage()
        {
            List<Shape> res = GetAllPolygonsInDrawingImage().Select(x => (Shape)x).ToList();
            RemoveAllPolygonsFromDrawingCanvas();
            Image img = ConvertShapesToImage(res, DrawingCanvas.Width, DrawingCanvas.Height);
            DrawingCanvas.Children.Add(img);
        }

        private List<Polygon> GetAllPolygonsInDrawingImage()
        {
            return DrawingCanvas.Children.OfType<Polygon>().ToList();

            /*List<Polygon> res = new List<Polygon>();
            for (int i = 0; i < DrawingCanvas.Children.Count; i++)
            {
                if (DrawingCanvas.Children[i] is Polygon)
                {
                    res.Add((Polygon)DrawingCanvas.Children[i]);
                }
            }
            return res;*/
        }

        private void RemoveAllPolygonsFromDrawingCanvas() //-, ИСПОЛЬЗОВАТЬ LINQQQQQQQQ!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        {
            List<Polygon> polygons = DrawingCanvas.Children.OfType<Polygon>().ToList();

            foreach (Polygon gon in polygons)
            {
                DrawingCanvas.Children.Remove(gon);
            }


            /*            for (int i = 0; i < DrawingCanvas.Children.Count; i++)
                        {
                            if (DrawingCanvas.Children[i] is Polygon)
                            {
                                DrawingCanvas.Children.RemoveAt(i);
                                if (i != 0) i--;
                                else i = -1;
                            }
                        }*/
        }

        private void SwapLinesWithImages()
        {
            RazerMarker.Visibility = Visibility.Hidden;

            //Get all lines
            List<Shape> lines = GetAllDrawChildrenInLineType();

            //delete all lines
            DeleteAllLines();

            //init all lines in image and
            //add it to drawing canvas 
            Image img = ConvertShapesToImage(lines, DrawingCanvas.Width, DrawingCanvas.Height);
            DrawingCanvas.Children.Add(img);
        }

        private void UseAuxiliaryCanvasToCollectChildrenInDrawingCanvas()
        {
            Canvas can = _main.GetAuxiliaryCanvas(DrawingCanvas);
            Image res = _main.ConvertCanvasInImage(can);
            RepaintBgInCanvas(res, can);
        }

        private void RepaintBgInCanvas(Image res, Canvas can)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap(
                (int)can.Width, (int)can.Height, _dpiParam, _dpiParam, PixelFormats.Pbgra32);
            can.Measure(new Size((int)can.Width, (int)can.Height));
            can.Arrange(new Rect(new Size((int)can.Width, (int)can.Height)));
            RenderOptions.SetEdgeMode(rtb, EdgeMode.Aliased);
            rtb.Render(can);

            ImageSource currentImageSource = rtb;

            DrawingGroup drawingGroup = new DrawingGroup();
            drawingGroup.Children.Add(new ImageDrawing(
                currentImageSource, new Rect(0, 0, can.Width, can.Height)));
            drawingGroup.Children.Add(new ImageDrawing(
                res.Source, new Rect(0, 0, can.Width, can.Height)));

            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new DrawingImage(drawingGroup);
            DrawingCanvas.Background = imageBrush;
        }

        private void DeleteAllLines() //-, LINQ!
        {
            List<Line> polygons = DrawingCanvas.Children.OfType<Line>().ToList();

            foreach (Line gon in polygons)
            {
                DrawingCanvas.Children.Remove(gon);
            }

            /*const int stepBack = 1;
            for (int i = 0; i < DrawingCanvas.Children.Count; i++)
            {
                if (DrawingCanvas.Children[i] is Line)
                {
                    DrawingCanvas.Children.RemoveAt(i);
                    if (i != 0) i--;
                    else i -= stepBack;
                }
            }*/
        }

        private List<Shape> GetAllDrawChildrenInLineType() //-, linq!
        {
            return DrawingCanvas.Children.OfType<Shape>().ToList();

            /*            List<Shape> res = new List<Shape>();
                        for (int i = 0; i < DrawingCanvas.Children.Count; i++)
                        {
                            if (DrawingCanvas.Children[i] is Line)
                            {
                                res.Add((Line)DrawingCanvas.Children[i]);
                            }
                        }
                        return res;*/
        }

        public Image ConvertShapesToImage(List<Shape> shapes, double width, double height)
        {
            Canvas canvas = new Canvas()
            {
                Width = width,
                Height = height,
                Background = Brushes.Transparent
            };

            RenderOptions.SetBitmapScalingMode(canvas, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetEdgeMode(canvas, EdgeMode.Aliased);

            for (int i = 0; i < shapes.Count; i++)
            {
                canvas.Children.Add(shapes[i]);
            }
            canvas.Measure(new Size(width, height));
            canvas.Arrange(new Rect(new Size(width, height)));

            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                (int)width, (int)height, _dpiParam, _dpiParam, PixelFormats.Pbgra32);
            RenderOptions.SetBitmapScalingMode(renderBitmap, BitmapScalingMode.NearestNeighbor);
            renderBitmap.Render(canvas);

            Image image = new Image();
            image.Source = renderBitmap;
            image.Width = width;
            image.Height = height;
            //SnapsToDevicePixels = true;
            return image;
        }

        private void ChooseAllSelection_MouseLeftButtonDown(object sender, EventArgs e) //-
        {
            if (!(_changedSizeText is null)) return;
            _text.Visibility = Visibility.Hidden;

            //Remove menues  
            ResetRenderTransform();
            RemoveRightClickMenus();

            //Remove line figure
            ClearLineSizing();

            //Clear Dynamyc values
            if (!(_main._selection is null)) ClearUsualSelection();
            ClearBgsForFigures();

            //_main.FreeSelection(DrawingCanvas, CheckRect);

            if (!(_main._selection is null) && !(_main._selection._shape is null)) ClearDynamicValues();
            if (!(_main._selection is null))
            {
                //remove selection parts 
                _main._selection.CheckCan.Children.Remove(_main._selectionLine);
                _main.FreeSelection(DrawingCanvas, CheckRect);

                //set curv figure 
                ClearCurveVals();

                //Set canvas (if figureing or selecting)
                if (!_main.SetSelectionInWholeDrawingCanvas(ObjSize, DraiwngScroll, DrawingCanvas)) return;
                _main.SetCanvasBg(DrawingCanvas, CheckRect);


                //set all drawingCanvas in new selection bg
                if (!(_main._selection.BgCanvas.Background is ImageBrush))
                {
                    Image canBG = _main.ConvertBackgroundToImage(DrawingCanvas);
                    _main._selection.SelectCan.Children.Add(canBG);
                }

                //clear DrawingCanvas bg 
                DrawingCanvas.Background = new SolidColorBrush(Colors.White);

                //Add selection in drawingCanvas 
                DrawingCanvas.Children.Add(_main._selection);
                _main._selection.IfSelectionIsClicked = false;

                //set sizes
                SetObgjContent(new Size(Math.Abs(DrawingCanvas.Width), Math.Abs(DrawingCanvas.Height)));
                UpdateRenderTransform();
                return;
            }

            // set dynamyc vals for allSelection
            _main._selectionType = SelectionType.All;
            _main._type = ActionType.Selection;

            //Make selection
            _main.SelectAllDrawingSelection(ObjSize, DraiwngScroll, DrawingCanvas, CheckRect);

            //Set sizes
            SetObgjContent(new Size(DrawingCanvas.Width, DrawingCanvas.Height));

            //Set curve figure
            ClearCurveVals();

            DrawingCanvas.ClipToBounds = false;
            UpdateRenderTransform();
        }

        private void ChangeCustomSelectionStretch()
        {
            if (_main._selection is null || _main._selectionLine is null
                || !_main._selection.CheckCan.Children.OfType<Polyline>().Any()) return;

            Polyline selLine = _main._selection.CheckCan.Children.OfType<Polyline>().First();

            if (selLine != _main._selectionLine) return;
            _main._selectionLine.Stretch = Stretch.Fill;
        }

        private void SelectionMenu_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            ResetRenderTransform();
            EndWithPolygonFigures();
            if (_main._type == ActionType.Figuring)
            {
                ClearFigureSizingInClicks();
            }
            UpdateRenderTransform();
        }

        private SelectionType _savedType;
        private void TurnClickedObjInBlue_MouseDown(object sender, MouseEventArgs e)
        {
            RemoveRightClickMenus();
            _main._tempCursor = null;
            ClearDynamicValues(brushClicked: true);

            if (sender is Button but)
            {
                PaintButBordsInClickedColor(but);
                if (but.Name == PaintingBut.Name.ToString())
                {
                    _main._type = ActionType.Drawing;
                }
                else
                {
                    _main._type = ActionType.Selection;
                    _main._selectionType = _savedType;
                    ValueBorder.Visibility = Visibility.Hidden;
                }
                if (_main._tempBrushType == BrushType.Spray)
                {
                    _main._tempCursor = _main._sprayCurs;
                }
                else
                {
                    _main._tempCursor = _main._crossCurs;
                }
            }
        }

        private void PaintButBordsInClickedColor(Button but)
        {
            const int borderThickness = 1;
            but.BorderThickness = new Thickness(borderThickness);
            but.BorderBrush = _clickedBorderColor;
        }

        private void ClearDynamicValues(bool brushClicked = false,
            bool textClicked = false)
        {
            ResetRenderTransform();

            IfSelectionContainsAndClearIt();
            CheckForTextSizeChanging();
            EndWithPolygonFigures();
            ClearFigureSizingInClicks();
            ClearBGs();

            if (brushClicked) ValueBorder.Visibility = Visibility.Visible;
            else ValueBorder.Visibility = Visibility.Hidden;
            if (textClicked) _text.Visibility = Visibility.Visible;
            else _text.Visibility = Visibility.Collapsed;

            UpdateRenderTransform();
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
        private void MakeBoOfWorkingTable_Click(object sender, EventArgs e)
        {
            const int SPI_SETDESKWALLPAPER = 20;
            const int SPIF_UPDATEINFILE = 1;
            const int SPIF_SENDCHANGE = 2;

            //const string path = "B:\\GitHub\\PaintWPF\\toSaveFonts\\font.png";

            DirectoryInfo dirInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            string startDir = dirInfo.Parent.Parent.FullName;
            string fontsPath = System.IO.Path.Combine(startDir, "toSaveFonts");
            string path = System.IO.Path.Combine(fontsPath, "font.png");

            Image image = _main.ConvertCanvasInImage(DrawingCanvas);
            SaveImageToDisk(image.Source, path);

            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINFILE | SPIF_SENDCHANGE);
            MessageBox.Show("Done!", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public string SaveImageToDisk(ImageSource imageSource, string filePath)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imageSource));
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                encoder.Save(fileStream);
            }
            return filePath;
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            ImageSettings settings = new ImageSettings(DrawingCanvas,
                _savedPaths.Count == 0 ? string.Empty : _savedPaths.First(), _lastSaveTime);
            settings.ShowDialog();
        }

        private Image _transparentImage = null;
        private void TransparentSelection_Click(object sender, EventArgs e)
        {
            const int selectionCorrelation = 0;

            //conds to exit
            if (_main._selection is null ||
                _main._selectionType == SelectionType.Invert) return;
            /* if (_main._selectionType == SelectionType.Rectangle ||
                 _main._selectionType == SelectionType.Custom) return;*/

            //if tracparent secltion is activated 
            if (_main._ifTransparencyIsActivated)
            {
                _main._selection.BgCanvas.Background = new ImageBrush()
                {
                    ImageSource = _transparentImage.Source
                };

                _main._ifTransparencyIsActivated = false;
                //Make selction tracparent
                _main.SetTransparentSelectionImage(TransSelectionIcon);
                //Set location for selection
                Canvas.SetTop(_main._selection, Canvas.GetTop(_main._selection) - selectionCorrelation);
                Canvas.SetLeft(_main._selection, Canvas.GetLeft(_main._selection) - selectionCorrelation);
                return;
            }

            _main._ifTransparencyIsActivated = true;
            _main.SetTransparentSelectionImage(TransSelectionIcon);

            //get all elems in selecion canvas
            List<UIElement> selectionElems =
                _main.ReAssignChildrenInAuxiliaryCanvas(_main._selection.SelectCan);
            _main._selection.SelectCan.Children.Clear();

            //get state of start canvas background
            _transparentImage = _main.ConvertBackgroundToImage(_main._selection.BgCanvas);

            //make trancparency magic with chosen color
            Image redoneBgIMG = _main.SwipeColorsInImage(
                _main.ConvertCanvasInImage(_main._selection.BgCanvas),
              _main.SecondColor.Color, _main._transparentColor);

            //Get elems back in select canvas 
            InitElemsInCanvas(_main._selection.SelectCan, selectionElems);

            //set new image as bg
            _main._selection.BgCanvas.Background = new ImageBrush()
            {
                ImageSource = redoneBgIMG.Source
            };
            //_main._selection.BgCanvas.Children.Add(redoneBgIMG);

            //set selection position
            Canvas.SetTop(_main._selection, Canvas.GetTop(_main._selection) - selectionCorrelation);
            Canvas.SetLeft(_main._selection, Canvas.GetLeft(_main._selection) - selectionCorrelation);
        }

        private void InitElemsInCanvas(Canvas can, List<UIElement> elements)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                can.Children.Add(elements[i]);
            }
        }

        private void DeleteSelectionContainment_Click(object sender, EventArgs e)
        {
            RemoveClicked();
        }

        private void Print_Click(object sender, EventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            Image printImg = _main.ConvertCanvasInImage(DrawingCanvas);
            if (printDialog.ShowDialog() == true)
            {
                Size pageSize = new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);

                Grid grid = new Grid();
                grid.Width = pageSize.Width;
                grid.Height = pageSize.Height;
                grid.Children.Add(printImg);

                printImg.Stretch = Stretch.Uniform;
                printImg.Width = pageSize.Width;
                printImg.Height = pageSize.Height;

                printDialog.PrintVisual(grid, "Печать изображения");
            }
        }

        private void MakeAllSelSBorderVisiable(Selection selection)
        {
            selection.DashedBorder.Visibility = Visibility.Visible;
            for (int i = 0; i < selection.SelectCan.Children.Count; i++)
            {
                if (selection.SelectCan.Children[i] is Selection)
                {
                    MakeAllSelSBorderVisiable((Selection)selection.SelectCan.Children[i]);
                }
            }
        }

        public bool IfAllSelectionBeforeReverse()
        {
            return (_main._selection.Width == DrawingCanvas.Width &&
                _main._selection.Height == DrawingCanvas.Height);
        }

        private int _amountOfReverces = 0;
        public void SetAmountOfRevereces(Canvas canvas)
        {
            if (canvas == DrawingCanvas) _amountOfReverces = 0;
            for (int i = 0; i < canvas.Children.Count; i++)
            {
                if (canvas.Children[i] is Selection sel)
                {
                    _amountOfReverces++;
                    SetAmountOfRevereces(sel.SelectCan);
                    return;
                }
            }
        }

        private void ManyReversesAction()
        {
            DoesntWork ifWork = new DoesntWork("MISHA ITS TOO MUSH. STOP PLS ヽ(・∀・)ﾉ", true);
            ifWork.ShowDialog();
            _main.FreeSelection(DrawingCanvas, CheckRect);
            _main._type = ActionType.Selection;
            DrawingCanvas.Background = new SolidColorBrush(Colors.White);
        }

        private void InvertSelection_Click(object sender, EventArgs e)
        {
            const int maxInvertions = 3;
            SetAmountOfRevereces(DrawingCanvas);
            if (_amountOfReverces == maxInvertions)
            {
                ManyReversesAction();
                return;
            }

            RemoveRightClickMenus();

            //Check For all selection(if thats it => return
            if (_main._type == ActionType.ChangingFigureSize) return;
            if (_main._selection is null &&
                DrawingCanvas.Children.OfType<Selection>().FirstOrDefault() is null) return;
            if (_main._selection.CheckCan.Children.Contains(_main._selectionLine)) ChangeCustomSelectionStretch();
            _main._selectionType = SelectionType.Invert;

            //Need to clear selection(put them into drawingCanvas), and get image
            Image bgImg = _main.GetImageOfFreedSeletcions(DrawingCanvas.Children.OfType<Selection>().FirstOrDefault(), DrawingCanvas);
            _main.SelecitonCut(DrawingCanvas, ObjSize, TransSelectionIcon, CheckRect, DraiwngScroll);

            Canvas.SetLeft(_main._selection.SelectCan, 0);
            Canvas.SetTop(_main._selection.SelectCan, 0);

            List<UIElement> elems = _main.ReAssignChildrenInAuxiliaryCanvas(DrawingCanvas);
            _main.RemoveAllChildrenExceptImages(DrawingCanvas);
            _main.SetCanvasBg(DrawingCanvas, CheckRect);
            _main.AddListOfElemsWithoutImages(elems, DrawingCanvas);

            //Selection in selection in selection...
            Selection highestBeforeAllSell = _main.GetHighestSelection(DrawingCanvas);

            //go FROM the Deepest selection (get deepest selection)
            Selection deepestSelection = null;
            _main.GetTheDeepestSelection(DrawingCanvas, ref deepestSelection);

            if (!(deepestSelection is null) && deepestSelection.CheckCan.Children.OfType<Polyline>().Any())
            {
                Polyline line = deepestSelection.CheckCan.Children.OfType<Polyline>().First();
                _main._selectionLine = line;
            }

            //Clear all selection (put their BGs in drawingCanvas)
            DrawingCanvas.Background = new ImageBrush() { ImageSource = bgImg.Source };

            _main.InitBGForSelectionFromDeepest(deepestSelection, DrawingCanvas,
                ObjSize, TransSelectionIcon, CheckRect, DraiwngScroll);

            //Get All drawingCanvas Children
            List<UIElement> drawCanvasElems = _main.ReAssignChildrenInAuxiliaryCanvas(DrawingCanvas);
            //clear selections
            DrawingCanvas.Children.Clear();
            //Add all Selection

            _main._selectionType = SelectionType.All;
            _main.SelectAllDrawingSelection(ObjSize, DraiwngScroll, DrawingCanvas, CheckRect);
            SetObgjContent(new Size(DrawingCanvas.Width, DrawingCanvas.Height));
            _main._selectionType = SelectionType.Invert;

            _main.InitElementsInCanvas(_main._selection.SelectCan, drawCanvasElems);

            //remove selection grid for all children selections()
            _main.RemoveSelectionGridInDeep(_main._selection, 0);

            //Make invertation
            //From all selection. Free every second selection(set selections bg in DrawingImage)
            _main.MakeInvertSelection(DrawingCanvas, DrawingCanvas, CheckRect);

            //clear highest
            //Set highest Sekection in SystemSelection
            _main.SetHighestSelectionInSystemSelection(DrawingCanvas);

            //Remove Sizing grids
            if (!(highestBeforeAllSell is null)) _main.RemoveSelectionGridInDeep(highestBeforeAllSell, 0);
        }

        private void CheckAllChildren(Canvas canvas)
        {
            for (int i = 0; i < canvas.Children.Count; i++)
            {
                if (canvas.Children[i].GetType() == typeof(Selection))
                {
                    CheckAllChildren(((Selection)canvas.Children[i]).SelectCan);
                }
            }
        }

        private void SetSelectionChildren(Selection selection)
        {
            Selection sel = GetSelectionFromCanvas(DrawingCanvas);
            DrawingCanvas.Children.Remove(sel);
            Grid grid = selection.CheckCan.Children.OfType<Grid>().FirstOrDefault();
            if (!(grid is null))
            {
                selection.CheckCan.Children.Remove(grid);
            }
            _main.SetCanvasBg(DrawingCanvas, CheckRect);
            DrawingCanvas.Children.Add(sel);
        }

        private void FreeCustomSelection(Selection selection)
        {
            const int correlLoc = 1;

            Point tempSelPoint = _main.GetPointOfSelection(selection, DrawingCanvas);
            _main.RemoveImagesFromCanvas(DrawingCanvas);

            Image res = _main.ConvertCanvasInImage(selection.SelectCan);

            Canvas.SetLeft(res, tempSelPoint.X + correlLoc);
            Canvas.SetTop(res, tempSelPoint.Y + correlLoc);

            DrawingCanvas.Children.Add(res);
            SetSelectionChildren(selection);
        }

        private void FreeAllSelectionsInInversionSelection(Selection selection, int reverseIndex)
        {
            if (selection is null) return;

            //Get selection point
            //Point selectionPoint = GetPointOfSelection(selection);
            Point selectionPoint = _main.GetSelectionPointComparedToDrawingCanvas(selection, new Point(0, 0), DrawingCanvas);

            if (selection.CheckCan.Children.OfType<Polyline>().Any())
            {
                FreeCustomSelection(selection);
                Selection nextRow = GetSelectionFromCanvas(selection.SelectCan);
                FreeAllSelectionsInInversionSelection(nextRow, reverseIndex);
                return;
            }

            List<UIElement> selectCanElems = _main.ReAssignChildrenInAuxiliaryCanvas(selection.SelectCan);
            selection.SelectCan.Children.Clear();
            selection.DashedBorder.Visibility = Visibility.Hidden;

            //Convert selection can in image
            Image bgImage = _main.ConvertSelectionInImage(selection);

            //Change color for temp selection
            selection.SelectCan.Background = new SolidColorBrush(Colors.Transparent);
            selection.Background = new SolidColorBrush(Colors.Transparent);
            //Init image in drawingCanvas
            Point pointCorrel = new Point(0, 0);

            Canvas.SetLeft(bgImage, selectionPoint.X + pointCorrel.X);
            Canvas.SetTop(bgImage, selectionPoint.Y + pointCorrel.Y);

            //Get All objs in drawingCanvas + delete them
            List<UIElement> canObjects = _main.ReAssignChildrenInAuxiliaryCanvas(DrawingCanvas);
            DrawingCanvas.Children.Clear();

            //Add image to DrawingCanvas
            DrawingCanvas.Children.Add(bgImage);

            _main.SetCanvasBg(DrawingCanvas, CheckRect);
            _main.InitElementsInCanvas(DrawingCanvas, canObjects);

            //Return elems in temp canvas
            _main.AddListElemsInCanvas(selectCanElems, selection.SelectCan);

            selection.DashedBorder.Visibility = Visibility.Visible;
            Selection childSelection = GetSelectionFromCanvas(selection.SelectCan);

            FreeAllSelectionsInInversionSelection(childSelection, ++reverseIndex);
        }

        private Selection GetSelectionFromCanvas(Canvas canvas)
        {
            for (int i = 0; i < canvas.Children.Count; i++)
            {
                if (canvas.Children[i].GetType() == typeof(Selection))
                {
                    return (Selection)canvas.Children[i];
                }
            }
            return null;
        }

        LeftClickSelectionMenu _clickMenu;
        private void DrawingCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DrawingCanvas.Children.OfType<Selection>().FirstOrDefault() is null)
            {
                _main._selectionType = SelectionType.Nothing;
            }

            if (e.LeftButton == MouseButtonState.Pressed ||
                (_main._type != ActionType.Selection &&
                _main._type != ActionType.ChangingFigureSize &&
               _main._selectionType != SelectionType.All))
            {
                Field_MouseDown(sender, e);
                return;
            }
            if (!(_clickMenu is null)) _clickMenu.RemoveSubMenu();

            _clickMenu = new LeftClickSelectionMenu(DrawingCanvas);
            InitEventsForRightClickMenu();
            Point point = e.GetPosition(DrawingCanvas);

            Canvas.SetLeft(_clickMenu, point.X);
            Canvas.SetTop(_clickMenu, point.Y);

            //Make correct visualisation
            double lastWidthPoint = point.X + _clickMenu.Width;
            double lastHeightPoint = point.Y + _clickMenu.Height;

            if (lastWidthPoint > DrawingCanvas.Width)
            {
                double widthDiffer = lastWidthPoint - DrawingCanvas.Width;
                Canvas.SetLeft(_clickMenu, point.X - widthDiffer);
            }
            if (lastHeightPoint > DrawingCanvas.Height)
            {
                double heightDiffer = lastHeightPoint - DrawingCanvas.Height;
                Canvas.SetTop(_clickMenu, point.Y - heightDiffer);
            }

            RemoveClickMenuFromDrawingCanvas();
            DrawingCanvas.Children.Add(_clickMenu);
        }

        private void InitEventsForRightClickMenu()
        {
            if (_clickMenu is null) return;
            //Main !-----
            //Cut
            _clickMenu.Cut.Click += Cut_Click;
            //Copy
            _clickMenu.Copy.Click += Copy_Click;
            //Paste
            _clickMenu.Paste.Click += Paste_Click;
            //Choose all 
            _clickMenu.ChooseAll.Click += ChooseAllSelection_MouseLeftButtonDown;
            //Invert
            _clickMenu.InvertSelection.Click += InvertSelection_Click;
            //delete
            _clickMenu.Delete.Click += DeleteSelectionContainment_Click;
            //init rotation but with subMenu
            _clickMenu.ToTurn.Click += InitSubMenuEvents_Click;
            _clickMenu.Swap.Click += InitSubMenuEvents_Click;
            //Revese Color
            _clickMenu.ReverceColor.Click += MakeReverseColor_Click;
        }

        private void MakeReverseColor_Click(object sender, EventArgs e)
        {
            RemoveRightClickMenus();

            if (_main._selection is null || _main._selection is null ||
                _main._selectionType == SelectionType.Invert) return;

            //repaint image by bits
            if (_main._selection._shape is null)
            {
                _main.ReverseColorInSelection(CheckRect);
                return;
            }

            //change figure Color
            SolidColorBrush brush = _main._selection.GetShapeColor();
            SolidColorBrush res = _main.GetColorToRepaint(brush);
            _main._selection.SetStrokeForFigure(res);
        }



        private void InitSubMenuEvents_Click(object sender, EventArgs e)
        {
            //SubMenu!------
            //swap in 180 / flip in vertical / flip in horizontal
            InitSubMenuEvents();
        }

        private void InitSubMenuEvents()
        {
            if (_clickMenu._subMenu is null) return;

            for (int i = 0; i < _clickMenu._subMenu._items.Count; i++)
            {
                SubMenuItems itemType = _clickMenu._subMenu._items[i];

                if (_clickMenu._subMenu.SubMenu.Items[i].GetType() == typeof(SubMenuElement))
                {
                    Button but = ((SubMenuElement)_clickMenu._subMenu.SubMenu.Items[i]).SelfBut;
                    if (itemType == SubMenuItems.TurnIn180)
                    {
                        but.Click += RotateIn180Degree_Click;
                    }
                    else if (itemType == SubMenuItems.FlipInVertical)
                    {
                        but.Click += FlipInVertical_Click;
                    }
                    else if (itemType == SubMenuItems.FlipInHorizontal)
                    {
                        but.Click += FlipInHorizontal;
                    }
                }
            }
        }

        private void RotateIn180Degree_Click(object sender, EventArgs e)
        {
            RemoveRightClickMenus();
            RotateImage();
        }

        private void FlipInVertical_Click(object sender, EventArgs e)
        {
            RemoveRightClickMenus();
            FlipImage(false);
        }

        private void FlipInHorizontal(object sender, EventArgs e)
        {
            RemoveRightClickMenus();
            FlipImage(true);
        }

        private RotateTransform _rotate = new RotateTransform();
        private ScaleTransform _scale = null;
        private TransformGroup _transformGroup = new TransformGroup();
        private int _tempRotateAngle = 0;
        public void FlipImage(bool flipHorizontally)
        {
            if (_main._selection is null ||
                DrawingCanvas.Children.OfType<Selection>().FirstOrDefault() is null) return;

            const int chooseWay = 1;
            const int rotCenterDivider = 2;

            _main._selection.SelectCan.Children.Remove(_main._selectionLine);

            _scale = flipHorizontally ? _transformGroup.Children.OfType<ScaleTransform>().
                    FirstOrDefault(t => t.ScaleX == -chooseWay) :
                    _transformGroup.Children.OfType<ScaleTransform>().
                    FirstOrDefault(t => t.ScaleY == -chooseWay);


            int scale = GetScale(chooseWay);
            if (flipHorizontally)
            {
                _scale.ScaleX = scale;

                /*                if (_scale == null)
                                {
                                    _scale = new ScaleTransform();

                                    _scale.ScaleX = -chooseWay;
                                    _transformGroup.Children.Add(_scale);
                                }
                                else
                                {
                                    _scale.ScaleX = chooseWay;
                                }*/
            }
            else
            {
                _scale.ScaleY = scale;
                /*                if (_scale is null)
                                {
                                    _scale = new ScaleTransform();
                                    _scale.ScaleY = -chooseWay;
                                    _transformGroup.Children.Add(_scale);
                                }
                                else
                                {
                                    _scale.ScaleY = chooseWay;
                                }*/
            }

            _scale.CenterX = _main._selection.Width / rotCenterDivider;
            _scale.CenterY = _main._selection.Height / rotCenterDivider;

            if (_main._selection._shape is null)
            {
                ApplyNoShapeTransformation();
                return;
            }

            _main._selection._shape.LayoutTransform = _transformGroup;
        }

        private int GetScale(int chooseWay)
        {
            if (_scale is null)
            {
                _scale = new ScaleTransform();
                _transformGroup.Children.Add(_scale);
                return -chooseWay;

            }
            else
            {
                return chooseWay;
            }
        }

        private const int _rotateStep = 180;
        private void RotateImage()
        {
            if (_main._selection is null ||
                DrawingCanvas.Children.OfType<Selection>().FirstOrDefault() is null) return;

            const int startAngle = 0;
            const int rotCenterDivider = 2;

            _main._selection.CheckCan.Children.Remove(_main._selectionLine);
            _rotate = _transformGroup.Children.OfType<RotateTransform>().FirstOrDefault();

            if (_rotate is null)
            {
                _tempRotateAngle = _rotateStep;
                _rotate = new RotateTransform(_tempRotateAngle);
                _rotate.Angle = _tempRotateAngle;
                _transformGroup.Children.Add(_rotate);
            }
            else
            {
                _rotate.Angle = _rotate.Angle == _rotateStep ? startAngle : _rotateStep;
            }

            _rotate.CenterX = _main._selection.SelectCan.Width / rotCenterDivider;
            _rotate.CenterY = _main._selection.SelectCan.Height / rotCenterDivider;

            if (_main._selection._shape is null)
            {
                //Make check bg img
                ApplyNoShapeTransformation();
                return;
            }
            _main._selection._shape.LayoutTransform = _transformGroup;
        }

        private void ApplyTransformsForSelectionLine()
        {
            if (_main._selectionLine is null) return;
            _main._selectionLine.LayoutTransform = _transformGroup;
            _main._selection.SelectCan.Children.Remove(_main._selectionLine);
            _main._selection.CheckCan.Children.Remove(_main._selectionLine);
            _main._selection.CheckCan.Children.Add(_main._selectionLine);
        }

        private Image img = null;
        ImageBrush selectedBg = null;
        private void ApplyNoShapeTransformation()
        {
            if (!(img is null)) _main._selection.SelectCan.Children.Remove(img);

            ImageSource canvasImageSource = _main._selection.BgCanvas.Background is null ?
                selectedBg.ImageSource : _main.ConvertCanvasInImage(_main._selection.BgCanvas).Source;

            ImageBrush brush = new ImageBrush()
            {
                ImageSource = canvasImageSource
            };

            img = new Image()
            {
                Source = brush.ImageSource,
                LayoutTransform = _transformGroup,
                Width = _main._selection.SelectCan.Width,
                Height = _main._selection.SelectCan.Height
            };

            _main._selection.BgCanvas.Children.Add(img);
            if (!(_main._selection.BgCanvas.Background is null))
            {
                selectedBg = new ImageBrush()
                {
                    ImageSource = _main.ConvertBackgroundToImage(_main._selection.BgCanvas).Source
                };
                _main._selection.BgCanvas.Background = null;
            }
            ApplyTransformsForSelectionLine();
        }

        private void RemoveClickMenuFromDrawingCanvas()
        {
            for (int i = 0; i < DrawingCanvas.Children.Count; i++)
            {
                if (DrawingCanvas.Children[i] is LeftClickSelectionMenu)
                {
                    DrawingCanvas.Children.RemoveAt(i);
                    return;
                }
            }
        }

        private void RemoveRightClickMenus()
        {
            if (!(_clickMenu is null)) _clickMenu.RemoveSubMenu();
            DrawingCanvas.Children.Remove(_clickMenu);
            RemoveSubMenu();
        }

        private bool IfDrawingCanvasContainsRightClickMenus() //-, LINQ!
        {
            return DrawingCanvas.Children.OfType<LeftClickSelectionMenu>().ToList().Any() ||
                DrawingCanvas.Children.OfType<RightClickSubMenu>().ToList().Any();

            /*            for (int i = 0; i < DrawingCanvas.Children.Count; i++)
                        {
                            if (DrawingCanvas.Children[i] is LeftClickSelectionMenu ||
                                DrawingCanvas.Children[i] is RightClickSubMenu)
                            {
                                return true;
                            }
                        }
                        return false;*/
        }

        private void RemoveSubMenu()
        {
            for (int i = 0; i < DrawingCanvas.Children.Count; i++)
            {
                if (DrawingCanvas.Children[i] is RightClickSubMenu)
                {
                    DrawingCanvas.Children.RemoveAt(i);
                    return;
                }
            }
        }

        private void SetPolyLineInBuffer()
        {
            _copyPolyLine = null;
            if (!(_main._selectionLine is null))
            {
                _copyPolyLine = new Polyline()
                {
                    Points = _main._selectionLine.Points,
                    Stretch = Stretch.Fill,
                    Stroke = _main._selectionLine.Stroke,
                    StrokeThickness = _main._selectionLine.StrokeThickness
                };
            }
            _main._selectionLine = null;
        }

        Image _copyBuffer = null;
        Polyline _copyPolyLine = null;
        private void Cut_Click(object sender, EventArgs e)
        {
            _main._selection.CheckCan.Children.Remove(_main._selectionLine);
            RemoveRightClickMenus()
                ;
            if (_main._selection is null) return;
            _copyBuffer = _main.ConvertCanvasInImage(_main._selection.BgCanvas);
            _main._selection = null;

            SetPolyLineInBuffer();
            RemoveClicked();
            SetMenuItemActiveness(PasteChange, true);
        }

        private void Copy_Click(object sender, EventArgs e)
        {
            _main._selection.CheckCan.Children.Remove(_main._selectionLine);
            RemoveRightClickMenus();

            if (_main._selection is null) return;

            _copyBuffer = _main.ConvertCanvasInImage(_main._selection.BgCanvas);

            SetPolyLineInBuffer();
            SetMenuItemActiveness(PasteChange, true);
        }

        private void Paste_Click(object sender, EventArgs e)
        {
            RemoveRightClickMenus();
            if (_copyBuffer is null) return;

            if (!(_main._selection is null)) _main.FreeSelection(DrawingCanvas, CheckRect);
            InitNewBgForSelection(_copyBuffer);

            Canvas.SetLeft(_main._selection, 0);
            Canvas.SetTop(_main._selection, 0);

            DrawingCanvas.Children.Add(_main._selection);
        }

        private void InitNewBgForSelection(Image img)
        {
            _main._selection = new Selection(ObjSize, DraiwngScroll)
            {
                Width = img.Width,
                Height = img.Height
            };

            _main._selection.SelectionBorder.Width = img.Width;
            _main._selection.SelectionBorder.Height = img.Height;

            _main._selection.BgCanvas.Background = new ImageBrush()
            {
                ImageSource = img.Source
            };

            if (!(_copyPolyLine is null))
            {
                _main._selection.CheckCan.Children.Remove(_copyPolyLine);

                if (!(_copyPolyLine.Parent is null))
                {
                    var parent = _copyPolyLine.Parent;
                    if (parent is Canvas) ((Canvas)parent).Children.Remove(_copyPolyLine);
                }
                _main._selection.CheckCan.Children.Add(_copyPolyLine);
            }
        }

        const int _activeOpacity = 1;
        const double _inActiveOpacity = 0.5;
        private void SetMenuItemActiveness(MenuItem changeItem, bool ifActive)
        {
            if (ifActive)
            {
                changeItem.Opacity = _activeOpacity;
                changeItem.IsEnabled = true;
                return;
            }
            changeItem.Opacity = _inActiveOpacity;
            changeItem.IsEnabled = false;
        }
        private void SetButActiveness(Button but, bool ifActive)
        {
            if (ifActive)
            {
                but.Opacity = _activeOpacity;
                but.IsEnabled = true;
                return;
            }
            but.Opacity = _inActiveOpacity;
            but.IsEnabled = false;
        }

        private void CheckReverseForActiveness(bool ifActive)
        {
            if (!ifActive) return;

            if (_main._selection is null || !(_main._selection._shape is null))
            {
                _clickMenu.InvertSelection.Opacity = _inActiveOpacity;
                ThreeSelection.Opacity = _inActiveOpacity;
                return;
            }

            _clickMenu.InvertSelection.Opacity = _activeOpacity;
            ThreeSelection.Opacity = _activeOpacity;
        }

        private void NotWorkingSend_Click(object sender, RoutedEventArgs e)
        {
            DoesntWork ifWork = new DoesntWork("It Doesnt work btw!", false);
            ifWork.ShowDialog();
        }

        private void DrawingAdapt_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ResetRenderTransform();
            _main._horizontalOffset = -e.HorizontalOffset;
            UpdateRenderTransform();
        }

        private void EscapePressedAction()
        {
            if (!(_main._selection is null) && _main._selection._shape is null)
            {
                SetEscapeBgSelection();
            }
            RemoveClicked();
        }

        private void SetEscapeBgSelection()
        {
            if (currentIndex == 0)
            {
                DrawingCanvas.Background = new SolidColorBrush(_main._whiteColor);
                return;
            }
            DrawingCanvas.Background = new ImageBrush
            {
                ImageSource = _canvasHistory[currentIndex].Source
            };
        }

        private void FigurePanel_KeyDown(object sender, KeyEventArgs e)
        {
            DisableTabsHandler(e);
            SelectionArrowMove(e);
        }

        private void PaintWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (!(_richTexBox is null) && _richTexBox.IsFocused) return;
            DisableTabsHandler(e);
            if (e.Key == Key.Delete)
            {
                ReleaseAllTouchCaptures();
                ReleaseMouseCapture();
                RemoveClicked();
            }
            HotKeysClickTools(e);
            SelectionArrowMove(e);
        }

        private bool check = false;
        private void PaintWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (_main._type == ActionType.Text) return;

            //Space pressed action
            if (e.Key == Key.Space)
            {
                if (_main._type == ActionType.Drawing || _main._ifDrawing)
                {
                    if (!_main._ifDrawing && !_main._spaceDrawingPressed)
                    {
                        _main.ColorToPaint = _main.FirstColor;
                        //set temp position
                        _main.previousPoint = drawFieldEvent.GetPosition(DrawingCanvas);

                        //Set marker to paint
                        _main.SetMarkers(drawFieldEvent, DrawingCanvas);

                        //Set check rect to go out of borders
                        _main.AddCheckRect(drawFieldEvent, DrawingCanvas, CheckRect);

                        //set drawing flag
                        if (!check)
                        {
                            _main._ifDrawing = true;
                            check = true;
                        }
                        _main._spaceDrawingPressed = true;
                    }
                    e.Handled = true;
                }
                else //Relese mouse capture from selection
                {
                    if (!(_main._selection is null) && !_main._selection.CheckCan.IsMouseCaptured)
                    {
                        //remove selection line
                        if (!(_main._selectionLine is null))
                        {
                            _main._selection.CheckCan.Children.Remove(_main._selectionLine);
                            _main._selectionLine = null;
                        }

                        //set selcetion bg in drawingCanvas 
                        if (_main._type == ActionType.Selection) _main.FreeSelection(DrawingCanvas, CheckRect);

                        //clear tool borders buts (change color) 
                        ClearAfterFastToolButPressed();

                        //save changed canvas in history 
                        SaveInHistory();
                    }
                    if (!(_main._selection is null)) _main._selection._isDraggingSelection = false;
                    else if (!(_lineSizing is null)) _lineSizing._isDraggingSelection = false;
                    //clear dynam val adter space pressed
                    RemoveClicked();
                }
            }
            //escape pressed action
            if (e.Key == Key.Escape)
            {
                EscapePressedAction();
            }
        }

        private void PaintWindow_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && (_main._ifDrawing || _main._spaceDrawingPressed))
            {
                DrawingSpaceUpSettings();
            }
        }

        private void DrawingSpaceUpSettings()
        {
            _main._ifDrawing = false;
            _main._spaceDrawingPressed = false;
            RemoveCheckRect();
            ConvertPaintingInImage();
            _main._type = ActionType.Drawing;
            if (!IfDrawingCanvasContainsRightClickMenus())
            {
                SaveInHistory();
            }
            _main.MakeAllActionsNegative();
            check = false;
        }

        private void RemoveCheckRect()
        {
            if (!(CheckRect.Parent is null))
            {
                DrawingCanvas.Children.Remove(CheckRect);
            }
        }

        private void DisableTabsHandler(KeyEventArgs e)
        {
            if (e.Key == Key.Tab || e.Key == Key.Up || e.Key == Key.Down ||
                e.Key == Key.Left || e.Key == Key.Right)
            {
                e.Handled = true;
            }
        }

        private void SelectionArrowMove(KeyEventArgs e) //-, DUBL
        {
            UIElement elem;
            if (_lineSizing is null)
            {
                elem = _main._selection;
            }
            else elem = _lineSizing;

            if (elem is null || !DrawingCanvas.Children.Contains(elem)) return;

            Direction dir = Keyboard.IsKeyDown(Key.Left) ? Direction.Left :
                Keyboard.IsKeyDown(Key.Up) ? Direction.Up :
                Keyboard.IsKeyDown(Key.Right) ? Direction.Right :
                Keyboard.IsKeyDown(Key.Down) ? Direction.Down : Direction.Nothing;

            if (!(dir is Direction.Nothing))
            {
                MoveSelectionByArrow(dir, elem);
            }

            /*            if (Keyboard.IsKeyDown(Key.Left))
                        {
                            MoveSelectionByArrow(SelectionMoveByKeyDir.Left, elem);
                        }
                        if (Keyboard.IsKeyDown(Key.Up))
                        {
                            MoveSelectionByArrow(SelectionMoveByKeyDir.Up, elem);
                        }
                        if (Keyboard.IsKeyDown(Key.Right))
                        {
                            MoveSelectionByArrow(SelectionMoveByKeyDir.Right, elem);
                        }
                        if (Keyboard.IsKeyDown(Key.Down))
                        {
                            MoveSelectionByArrow(SelectionMoveByKeyDir.Down, elem);
                        }*/
        }

        private void MoveSelectionByArrow(Direction dir, UIElement elem) //- DUBL
        {
            const int stepInMove = 1;
            double step = dir == Direction.Up ||
                dir == Direction.Left ? -stepInMove : stepInMove;

            if (dir == Direction.Up ||
                dir == Direction.Down)
            {
                Canvas.SetTop(elem, Canvas.GetTop(elem) + step);
            }
            /*            else if (dir == SelectionMoveByKeyDir.Down)
                        {
                            Canvas.SetTop(elem, Canvas.GetTop(elem) + stepInMove);
                        }*/
            else if (dir == Direction.Left ||
                dir == Direction.Right)
            {
                Canvas.SetLeft(elem, Canvas.GetLeft(elem) + step);
            }
            /*            else if (dir == SelectionMoveByKeyDir.Right)
                        {
                            Canvas.SetLeft(elem, Canvas.GetLeft(elem) + stepInMove);
                        }*/
            UpdateBoundaries();
        }

        private void UpdateBoundaries()
        {
            if (_lineSizing is null)
            {
                _main._selection.ClipOutOfBoundariesGeo();
                return;
            }
            _lineSizing.ClipOutOfBoundariesGeo();
        }

        private void HotKeysClickTools(KeyEventArgs e) //-, SWITCH
        {
            Key key = e.Key;
            switch (key)
            {
                case Key.P:
                    {
                        PressedToolButton(ToolTypes.Pencil);
                        return;
                    }
                case Key.E:
                    {
                        PressedToolButton(ToolTypes.Razer);
                        return;
                    }
                case Key.T:
                    {
                        PressedToolButton(ToolTypes.Text);
                        return;
                    }
                case Key.I:
                    {
                        PressedToolButton(ToolTypes.Pipette);
                        return;
                    }
                case Key.S:
                    {
                        PressedToolButton(ToolTypes.Selection);
                        return;
                    }
                case Key.B:
                    {
                        PressedToolButton(ToolTypes.Bucket);
                        return;
                    }
            }

            /* if (e.Key == Key.P) //Pencil
             {
                 PressedToolButton(ToolTypes.Pencil);
             }
             else if (e.Key == Key.E)//Razer
             {
                 PressedToolButton(ToolTypes.Razer);
             }
             else if (e.Key == Key.T)//text
             {
                 PressedToolButton(ToolTypes.Text);
             }
             else if (e.Key == Key.I)//pipette
             {
                 PressedToolButton(ToolTypes.Pipette);
             }
             else if (e.Key == Key.S)//Selection
             {
                 Cursor = null;
                 PressedToolButton(ToolTypes.Selection);
             }
             else if (e.Key == Key.B)//Bucket
             {
                 PressedToolButton(ToolTypes.Bucket);
             }*/
        }

        private void RemoveSelectionLine()
        {
            if (!(_main._selection is null) && _main._selection.CheckCan.Children.Contains(_main._selectionLine))
            {
                _main._selection.CheckCan.Children.Remove(_main._selectionLine);
            }
        }

        public void PressedToolButton(ToolTypes type)
        {
            sprayTimer.Stop();
            RemoveSelectionLine();
            switch (type)
            {
                case ToolTypes.Pencil:
                    {
                        SetOptionsForFastToolClick(Pen, _main._pencilCurs);
                        return;
                    }
                case ToolTypes.Bucket:
                    {
                        SetOptionsForFastToolClick(Bucket, _main._bucketCurs);
                        return;
                    }
                case ToolTypes.Text:
                    {
                        SetOptionsForFastToolClick(Text, _main._textingCurs);
                        return;
                    }
                case ToolTypes.Razer:
                    {
                        SetOptionsForFastToolClick(Erazer, _main._crossCurs);
                        AddErasingMarker(_main.currentPoint);
                        DrawingCanvas.ClipToBounds = true;
                        return;
                    }
                case ToolTypes.Pipette:
                    {
                        SetOptionsForFastToolClick(ColorDrop, _main._pipetteCurs);
                        return;
                    }
                case ToolTypes.Selection:
                    {
                        SetSelectionFastChange();
                        return;
                    }
                default:
                    {
                        return;
                    }
            }
        }

        private void SetSelectionFastChange()
        {
            //Make removements 
            RemoveRightClickMenus();
            if (_main._type == ActionType.Selection) _main.FreeSelection(DrawingCanvas, CheckRect);
            ClearAfterFastToolButPressed();
            _main.MakeAllActionsNegative();

            //Change temp selection type
            _main._tempCursor = null;
            ClearDynamicValues();

            _main._selectionType = _main._selectionType == SelectionType.Rectangle ?
                SelectionType.Custom : SelectionType.Rectangle;

            if (_main._selectionType == SelectionType.Rectangle) _savedType = _main._selectionType;
            if (_main._selectionType == SelectionType.Custom) _savedType = _main._selectionType;

            ChangeSelectionImage();
            _main._type = ActionType.Selection;
            PaintButBordsInClickedColor(SelectionBut);
        }

        public void SetOptionsForFastToolClick(Button but, Cursor newCurs)
        {
            _main._selectionType = SelectionType.Nothing;

            if (_chosenTool == but) return;
            RemoveRightClickMenus();
            if (_main._type == ActionType.Selection) _main.FreeSelection(DrawingCanvas, CheckRect);

            ClearAfterFastToolButPressed();
            _main.MakeAllActionsNegative();

            ClearDynamicValues(brushClicked: but == Pen || but == Erazer || but == Text);

            _chosenTool = but;
            SetActionTypeByButtonPressed(but);

            _main._tempCursor = newCurs;
            Cursor = _main._tempCursor;
        }

        private void ClearAfterFastToolButPressed()
        {
            RemoveObject(_main._figToPaint);
            RemoveObject(_main._polyline);
            RemoveObject(_main._selectionRect);
            RemoveObject(_main._selectionLine);
            RemoveObject(_changedSizeText);
            RemoveObject(_richTexBox);
            DrawingCanvas.Children.Remove(RazerMarker);
        }

        private void RemoveObject(UIElement elem)
        {
            DrawingCanvas.Children.Remove(elem);
            elem = null;
        }

        public void RemoveClicked()
        {
            RemoveRightClickMenus();
            _main.MakeAllActionsNegative();
            _main._selection = null;

            DrawingCanvas.Children.Remove(_main._figToPaint);
            _main._figToPaint = null;
            if (_main._type == ActionType.ChangingFigureSize)
            {
                _main._ifFiguring = true;
                _main._type = ActionType.Figuring;
            }
            else if (_main._type == ActionType.Text)
            {
                _main._ifTexting = true;
            }
            _main.RemoveSelection(DrawingCanvas, ObjSize, TransSelectionIcon);
            _main._selection = null;

            DrawingCanvas.Children.Remove(_main._polyline);
            _main._polyline = null;

            DrawingCanvas.Children.Remove(_lineSizing);
            _lineSizing = null;

            DrawingCanvas.Children.Remove(_main._selectionRect);
            _main._selectionRect = null;

            DrawingCanvas.Children.Remove(_changedSizeText);
            _changedSizeText = null;
            _ifDoubleClicked = false;
            ReloadCurvePainting();
        }

        private void PaintWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(RazerMarker.Parent is null) &&
                RazerMarker.Parent is Canvas)
            {
                ((Canvas)RazerMarker.Parent).Children.Remove(RazerMarker);
            }
        }

        private void PaintWindow_MouseLeave(object sender, MouseEventArgs e)
        {

            ResetRenderTransform();
            if (_main._ifSelection)
            {
                ChangeSelectionSize(e);
            }
            CursorCheck();
            UpdateRenderTransform();
        }

        private void CursorCheck()
        {
            if (_main._selection is null && _changedSizeText is null) return;
            if (!(_main._selection is null)) _main._selection._tempCursor = null;
            if (!(_changedSizeText is null)) _changedSizeText._tempCursor = null;
            Cursor = null;
        }

        private void DrawingCanvas_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = _main._tempCursor;
            _ifCursorInsideDrawingCan = true;

            if (!_main._ifErasing || _main._type != ActionType.Erazing)
            {
                RemoveRazerMark();
            }
        }

        private void RemoveRazerMark()
        {
            if (_main._type == ActionType.Erazing) DrawingCanvas.ClipToBounds = true;
            RazerMarker.Visibility = Visibility.Hidden;

            DrawingCanvas.Children.Remove(RazerMarker);
            _main._ifErasing = false;
        }

        private void MainPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (!(_main._selection is null))
            {
                _main._selection.SelectionBorder_MouseMove(_main._selection.CheckCan, e);
                if (_main._selection._isDraggingSelection)
                {
                    _main._selection._tempCursor = _main._selection._moveCurs;
                }
                else
                {
                    _main._selection._tempCursor = null;
                }
            }
        }
        public void SetActionTypeByButtonPressed(Button but)
        {
            SolidColorBrush borderBrush = new SolidColorBrush(Color.FromRgb(0, 103, 192));
            but.BorderBrush = borderBrush;

            SolidColorBrush bgBrush = new SolidColorBrush(Color.FromRgb(240, 240, 240));
            but.Background = bgBrush;

            if (but.Name == Pen.Name)
            {
                _main._tempCursor = _main._pencilCurs;
                _main._type = ActionType.Drawing;
            }
            else if (but.Name == Erazer.Name)
            {
                _main._tempCursor = _main._crossCurs;
                _main._type = ActionType.Erazing;
            }
            else if (but.Name == Bucket.Name)
            {
                _main._tempCursor = _main._bucketCurs;
                _main._type = ActionType.Filling;
            }
            else if (but.Name == Text.Name)
            {
                _main._tempCursor = _main._textingCurs;
                _main._type = ActionType.Text;
            }
            else if (but.Name == ColorDrop.Name)
            {
                _main._tempCursor = _main._pipetteCurs;
                _main._type = ActionType.PickingColor;
            }
        }
        public void SetObgjContent(Size size)
        {
            size = new Size(Math.Round(size.Width, 0), Math.Round(size.Height, 0));
            ObjSize.Content = $"{size.Width} x {size.Height} пкс";
        }
    }
}

