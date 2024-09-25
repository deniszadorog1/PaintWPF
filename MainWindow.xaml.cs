using System;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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
using System.Reflection.Emit;
using System.Xml.Linq;

using System.Net;
using System.Windows.Media.Animation;
using System.Text;
using System.Security.Cryptography;
using System.Diagnostics.Eventing.Reader;

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

        //private ActionType _type = ActionType.Nothing;
        //private SelectionType _selectionType = SelectionType.Nothing;

        private Rectangle _selectionRect = new Rectangle();
        private Polyline _selectionLine = null;
        /*
                private bool _ifDrawing = false;
                private bool _ifFilling = false;
                private bool _ifErasing = false;
                private bool _ifFiguring = false;
                private bool _ifSelection = false;
                private bool _ifTexting = false;
                private bool _ifPickingColor = false;
                private bool _ifChangingFigureSize = false;*/

        //private bool IfSelectionIsMacken = false;

        //private Shape _figToPaint;
        //private bool _ifCurveIsDone = false;

        //private Point previousPoint;

        Selection _selection = null;

        private UIElement valueDragElem = null;
        private Point valueOffset;

        private int sprayDensity = 30;
        private Random random = new Random();
        private DispatcherTimer sprayTimer;
        private Point currentPoint;

        //private List<Polyline> polylines = new List<Polyline>();

        private readonly SolidColorBrush _clickedBorderColor =
            new SolidColorBrush(Color.FromRgb(0, 103, 192));

        private const double CalligraphyBrushAngle = 135 * Math.PI / 180;
        private const double FountainBrushAngle = 45 * Math.PI / 180;
        /*
                private string _oilBrushPath;
                private string _coloredBrushPath;
                private string _texturePencilBrushPath;
                private string _watercolorBrushPath;*/
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

        private Color _whiteColor = Color.FromArgb(255, 255, 255, 255);
        private Color _pinkColor = Color.FromRgb(255, 194, 214);
        private Color _transparentColor = Color.FromArgb(0, 0, 0, 0);
        /*
                private readonly Cursor _crossCurs = new Cursor(
                Application.GetResourceStream(new Uri(
                "Models/Cursors/Cross.cur", UriKind.Relative)).Stream);

                private readonly Cursor _pencilCurs = new Cursor(
                Application.GetResourceStream(new Uri(
                "Models/Cursors/Pencil.cur", UriKind.Relative)).Stream);

                private readonly Cursor _pipetteCurs = new Cursor(
                Application.GetResourceStream(new Uri(
                "Models/Cursors/Pipette.cur", UriKind.Relative)).Stream);

                private readonly Cursor _sprayCurs = new Cursor(
                Application.GetResourceStream(new Uri(
                "Models/Cursors/Spray.cur", UriKind.Relative)).Stream);

                private readonly Cursor _textingCurs = new Cursor(
                Application.GetResourceStream(new Uri(
                "Models/Cursors/Texting.cur", UriKind.Relative)).Stream);

                private readonly Cursor _bucketCurs = new Cursor(
                Application.GetResourceStream(new Uri(
                "Models/Cursors/Bucket.cur", UriKind.Relative)).Stream);*/

        //private Cursor _tempCursor;

        public MainWindow()
        {
            InitializeComponent();

            InitStartHeight();
            InitBrushFilePaths();

            InitToolButsInList();
            InitBrushTypesInList();

            InitializeSprayTimer();
            CanvasSize.Content = $"{DrawingCanvas.Width} x {DrawingCanvas.Height}";

            InitForTextControl();
            InitCustomColorButtons();

            InitTransparentSelectionImagePath();
            InitValueForSizeAdaptation();

            InitBrushMenu();

            //this.MouseMove += OnMouseMove;
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

            //DrawingCanvas.Children.Remove(CheckRect);
        }
        private bool selCheck = false;
        private void CheckRect_MouseMove(object sender, MouseEventArgs e)
        {
            if (CheckRect.IsMouseCaptured)
            {
                (sender as UIElement).CaptureMouse();
                Point currentPoint = e.GetPosition(CheckRect.Parent as IInputElement);
                Point check = new Point(Canvas.GetLeft(CheckRect), Canvas.GetTop(CheckRect));
                double offsetX = currentPoint.X - _startPointSelection.X;
                double offsetY = currentPoint.Y - _startPointSelection.Y;

                Canvas.SetLeft(CheckRect, _anchorPointSelection.X + offsetX - CheckRect.Width / 2);
                Canvas.SetTop(CheckRect, _anchorPointSelection.Y + offsetY - CheckRect.Height / 2);
            }
            /*
                        if (_main._ifSelection)
                        {
                            _firstSelectionEnd = new Point(e.GetPosition(DrawingCanvas).X, e.GetPosition(DrawingCanvas).Y);

                            double widthDiffer = _firstSelectionEnd.X - _firstSelectionStart.X;
                            double heightDiffer = _firstSelectionEnd.Y - _firstSelectionStart.Y;
                        }*/

            if (_main._ifSelection)
            {
                ChangeSelectionSize(e);

                Point point = e.GetPosition(DrawingCanvas);

                double check = _firstSelectionEnd.X - _firstSelectionStart.X;
            }
            else if (!(_lineSizing is null))
            {
                _lineSizing.Line_MouseMove(_lineSizing, e);
            }


            //DrawingCanvas.Children.Remove(CheckRect);

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
                AddCheckRect(e);
            }

        }
        private void CheckRect_PreviewMouseUp(object sender, MouseEventArgs e)
        {
            sprayTimer.Stop();
            return;
            DrawingCanvas.Children.Remove(CheckRect);
            CheckRect.ReleaseMouseCapture();
            if (_main._ifSelection && !_main.IfSelectionIsMacken)
            {
                MakeSelection();
            }
            if (_main._ifFiguring)
            {
                bool polygonCheck = IfPolygonFigureIsDone();
                bool curveCheck = IfCurveFigureIsDone();
                if (polygonCheck && curveCheck)
                {
                    InitFigureInSizingBorder();
                    ReloadCurvePainting();
                    //_main.MakeAllActionsNegative();
                }
            }
            if (!(CheckRect is null) && !(CheckRect.Parent is null))
            {
                ((Canvas)CheckRect.Parent).Children.Remove(CheckRect);
                DrawingCanvas.Children.Add(CheckRect);
            }

            // DrawingCanvas.ClipToBounds = false;
        }
        private void InitBrushMenu()
        {
            InitBrushMenuMargin();
            InitBrushMenuEvents();
        }
        private void InitBrushMenuMargin()
        {
            const int leftMarginCorel = 25;
            BrushesMenu.Margin = new Thickness(SelectionPart.Width.Value +
                ToolsPart.Width.Value + (BrushPart.Width.Value / 2) - (BrushesMenu.Width / 2),
                SettingsRow.Height.Value + ToolsRow.Height.Value - leftMarginCorel,
                0,
                0);
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
        /*        private string _checkPath;
                private string _checkOne;
                private string _checkTwo;*/
        public void InitBrushFilePaths()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            string startDir = dirInfo.Parent.Parent.FullName;
            string imgPath = System.IO.Path.Combine(startDir, "Images");
            string brushPath = System.IO.Path.Combine(imgPath, "Brushes");

            _main._oilBrushPath = System.IO.Path.Combine(brushPath, "OilBrushPaint.png");
            _main._coloredBrushPath = System.IO.Path.Combine(brushPath, "TexturePencilBrush.png");
            _main._texturePencilBrushPath = System.IO.Path.Combine(brushPath, "TexturePencilBrush.png");
            _main._watercolorBrushPath = System.IO.Path.Combine(brushPath, "WatercolorBrush.png");
            _main._checkPath = System.IO.Path.Combine(brushPath, "Check.png");
            _main._checkOne = System.IO.Path.Combine(brushPath, "CheckOne.png");
            _main._checkTwo = System.IO.Path.Combine(brushPath, "CheckTwo.png");
        }
        private Image _tickImg;
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

            _tickImg = new Image
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
            Ellipse greyCircle = new Ellipse();
            greyCircle.Width = button.Width + _chosenMainPaintCircleSizeAdd;
            greyCircle.Height = button.Height + _chosenMainPaintCircleSizeAdd;
            greyCircle.Fill = Brushes.Transparent;

            greyCircle.Stroke = MainPanel.Background;
            greyCircle.StrokeThickness = _chosenMainPaintCircleThickness;

            greyCircle.Margin = new Thickness(_chosenMainPaintCircleMargin);

            button.Content = greyCircle;
        }
        private void MyButton_MouseEnter(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;

            Ellipse greyCircle = new Ellipse();
            greyCircle.Width = button.Width + _chosenMainPaintCircleSizeAdd;
            greyCircle.Height = button.Height + _chosenMainPaintCircleSizeAdd;

            greyCircle.Stroke = Brushes.DarkGray;
            greyCircle.StrokeThickness = _chosenMainPaintCircleThickness;

            greyCircle.Margin = new Thickness(_chosenMainPaintCircleMargin);

            button.Content = greyCircle;

            ChangeFigToPaintColor((SolidColorBrush)button.Background);
            ChangeTextBgColor((SolidColorBrush)button.Background);
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
            if ((_selection is null || _selection._shape is null ||
                _chosenToPaintButton == SecondColor || _selection._isDraggingSelection)) return;
            _selection._shape.Stroke = brush;
        }
        private void MyButton_MouseLeave(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;

            Ellipse greyCircle = new Ellipse();
            greyCircle.Width = button.Width + _chosenMainPaintCircleSizeAdd;
            greyCircle.Height = button.Height + _chosenMainPaintCircleSizeAdd;
            greyCircle.Fill = Brushes.Transparent;

            greyCircle.Stroke = MainPanel.Background;
            greyCircle.StrokeThickness = _chosenMainPaintCircleThickness;

            greyCircle.Margin = new Thickness(_chosenMainPaintCircleMargin);

            button.Content = greyCircle;

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
            if (_selection is null || _selection._shape is null) return;

            _selection._shape.Stroke = _main.FirstColor;
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
        private SolidColorBrush GetSelectedColor()
        {
            if (_chosenToPaintButton is null ||
                _chosenToPaintButton.Name == "FirstColor") return _main.FirstColor;
            return _main.SecondColor;
        }
        private void InitColorInCustomColor(SolidColorBrush color)
        {
            if (color is null) return;
            if (_customColorIndex == _customColors.Count - 1)
            {
                MoveColorsIntListInLeft(color);
                return;
            }
            _customColors[_customColorIndex].Background = color;
            _customColorIndex++;

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
            for (int i = 0; i < _customColors.Count - 1; i++)
            {
                _customColors[i].Background = _customColors[i + 1].Background;
            }
            _customColors[_customColors.Count - 1].Background = color;
        }
        private readonly SolidColorBrush _toolPanelBrush = new SolidColorBrush(Color.FromRgb(240, 240, 240));
        private readonly SolidColorBrush _toolPanelBorderBrush = new SolidColorBrush(Color.FromRgb(209, 209, 209));
        private void MainPanelBoxes_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button but)
            {
                if ((!(_chosenTool is null) && but.Name == _chosenTool.Name) ||
                    ((SolidColorBrush)but.BorderBrush).Color == _clickedBorderColor.Color) return;

                but.Background = _toolPanelBrush;
                but.BorderBrush = _toolPanelBorderBrush;
                return;
            }
            else if (sender is Border bord)
            {
                bord.Background = _toolPanelBrush;
                bord.BorderBrush = _toolPanelBorderBrush;
                return;
            }
            else if (sender is Grid grid)
            {
                grid.Background = _toolPanelBrush;
            }
        }
        private readonly SolidColorBrush _topPanelBrush =
            new SolidColorBrush(Color.FromRgb(230, 235, 240));

        private void MainPanelTop_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Button but)
            {
                but.Background = _topPanelBrush;
                return;
            }
            if (sender is Grid grid)
            {
                grid.Background = _topPanelBrush;
                return;
            }
        }
        private SolidColorBrush _mainPanelButtonsBrush = new SolidColorBrush(Color.FromArgb(0, 248, 249, 252));
        private void MainLabelBoxes_MouseLeave(object sender, MouseEventArgs e)
        {
            ResetRenderTransform();
            if (sender is Button but)
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
                UpdateRenderTransform();
                return;
            }
            if (sender is Border bord)
            {
                bord.Background = _mainPanelButtonsBrush;
                bord.BorderBrush = bord.BorderBrush != _clickedBorderColor ?
                    _mainPanelButtonsBrush : _clickedBorderColor;
                UpdateRenderTransform();
                return;
            }
            if (sender is Grid grid)
            {
                grid.Background = _mainPanelButtonsBrush;
                UpdateRenderTransform();
                return;
            }
            UpdateRenderTransform();
        }
        private void Tool_MouseClick(object sender, EventArgs e)
        {
            ResetRenderTransform();
            DrawingCanvas.RenderTransform = new TranslateTransform(0, 0);
            _main._tempCursor = null;
            IfSelectionContainsAndClearIt();
            RemoveRightClickMenus();
            FreeSelection();
            RemoveRazerMark();
            if (sender is Button but)
            {
                ClearDynamicValues(brushClicked: but.Name == "Pen" || but.Name == "Erazer",
                    textClicked: but.Name == "Text");

                if (!(_chosenTool is null) &&
                    but.Name == _chosenTool.Name)
                {
                    UpdateRenderTransform();
                    return;
                }
                _chosenTool = but;

                _main.SetActionTypeByButtonPressed(but);
            }
            UpdateRenderTransform();
        }
        private void ResetRenderTransform()
        {
            DrawingCanvas.RenderTransform = new TranslateTransform(0, 0);
        }
        private void UpdateRenderTransform()
        {
            DrawingCanvas.RenderTransform = new TranslateTransform(_horizontalOffset, 0);
        }
        public void ClearBGs()
        {
            ClearBGForTools();
            _chosenTool = null;
            ClearBgsForFigures();
        }
        private readonly SolidColorBrush _transparentBrush =
            new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));

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

            //AddCheckRect(e);
            sprayTimer.Stop();

            CursorCheck();
            Cursor = null;
        }

        private void Field_MouseDown(object sender, MouseEventArgs e)
        {
            if (IfThereIsReClick()) return;
            ResetRenderTransform();

            DrawingCanvas.Children.Remove(RazerMarker);
            _drawPolyline = null;
            _paintDirection = null;

            if (!(_selection is null) || !(_changedSizeText is null))
            {
                UpdateRenderTransform();
                return;
            }
            _ifThereAreUnsavedChangings = true;
            _ifSaved = false;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _main.ColorToPaint = _main.FirstColor;
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                _main.ColorToPaint = _main.SecondColor;
            }
            _main.previousPoint = e.GetPosition(DrawingCanvas);
            _main.InitDeed();
            ClipCheck();



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
                _main.InitShapesToPaint(e, DrawingCanvas);
                DrawingCanvas.ClipToBounds = true;
                AddCheckRect(e);
            }
            else
            {

                _main.previousPoint = e.GetPosition(DrawingCanvas);
                _main.SetMarkers(e, DrawingCanvas);
                if (_main._ifDrawing) BrushPaint(e);

                AddCheckRect(e);
            }
            UpdateRenderTransform();
        }
        private bool IfThereIsReClick()
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed &&
               Mouse.RightButton == MouseButtonState.Pressed)
            {
                if (!(_selection is null) && _selection.CheckCan.IsMouseCaptured)
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
            _main.SetActionTypeByButtonPressed(Pen);

        }
        private const int _dpiParam = 96;

        private void MakeSelection(MouseEventArgs e)
        {
            _main._selectionType = _savedType;
            if (_main._selectionType == SelectionType.Rectangle)
            {
                MakeRectangleSelection(e);
            }
            else if (_main._selectionType == SelectionType.Custom)
            {
                MakeCustomSelection(e);
            }
            else if (_main._selectionType == SelectionType.All)
            {
                //other event is working 
            }
            AddCheckRect(e);
        }
        private void AddCheckRect(MouseEventArgs e)
        {
            Point point = e.GetPosition(DrawingCanvas);

            if (DrawingCanvas.Children.Contains(CheckRect))
            {
                return;
                DrawingCanvas.Children.Remove(CheckRect);
            }
            DrawingCanvas.Children.Add(CheckRect);
            Canvas.SetLeft(CheckRect, point.X);
            Canvas.SetTop(CheckRect, point.Y);
            CheckRect.CaptureMouse();
        }
        private void MakeCustomSelection(MouseEventArgs e)
        {
            const int customLineThickness = 1;
            _selectionLine = new Polyline()
            {
                Stroke = Brushes.LightBlue,
                StrokeThickness = 1,
                Fill = Brushes.Transparent,
                RenderTransform = new TranslateTransform(0, 0)
            };
            Canvas.SetLeft(_selectionLine, e.GetPosition(DrawingCanvas).X);
            Canvas.SetTop(_selectionLine, e.GetPosition(DrawingCanvas).Y);

            _firstSelectionStart = new Point(e.GetPosition(DrawingCanvas).X, e.GetPosition(DrawingCanvas).Y);
            DrawingCanvas.Children.Add(_selectionLine);
        }

        private void MakeRectangleSelection(MouseEventArgs e)
        {
            const int rectSelectionThickness = 1;
            _selectionRect = new Rectangle
            {
                Stroke = Brushes.LightBlue,
                StrokeThickness = rectSelectionThickness,
                Fill = Brushes.Transparent
            };
            Canvas.SetLeft(_selectionRect, e.GetPosition(DrawingCanvas).X);
            Canvas.SetTop(_selectionRect, e.GetPosition(DrawingCanvas).Y);

            _firstSelectionStart = new Point(e.GetPosition(DrawingCanvas).X, e.GetPosition(DrawingCanvas).Y);
            DrawingCanvas.Children.Add(_selectionRect);
        }
        private void Field_MouseMove(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition(DrawingCanvas);
            currentPoint = position;
            position = _main.GetCursLoc(position, DrawingCanvas);

            string cursPosInPaintField = ((position.X == -1) || (position.Y == -1)) ? string.Empty : $"{(int)position.X}, {(int)position.Y}";
            CursCords.Content = cursPosInPaintField;

            AddErasingMarker(position);

            if (_main._ifSelection)
            {
                ChangeSelectionSize(e);
            }
            else if (_main._ifDrawing || _main._ifErasing)
            {
                BrushPaint(e);
            }
            else if (!(_selection is null) && _main._ifChangingFigureSize)
            {
                _firstSelectionEnd = new Point(e.GetPosition(DrawingCanvas).X, e.GetPosition(DrawingCanvas).Y);

                double widthDiffer = _firstSelectionEnd.X - _firstSelectionStart.X;
                double heightDiffer = _firstSelectionEnd.Y - _firstSelectionStart.Y;

                ObjSize.Content = $"{Math.Abs(widthDiffer)} x {Math.Abs(heightDiffer)} пкс";
            }
        }
        private const int mostZIndex = 1;
        private const int brushHalf = 2;
        private const int pngBrushPosCorel = 10;

        private void AddErasingMarker(Point posPoint)
        {
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
                _firstSelectionEnd = new Point(e.GetPosition(DrawingCanvas).X, e.GetPosition(DrawingCanvas).Y);
                double widthDiffer = _firstSelectionEnd.X - _firstSelectionStart.X;
                double heightDiffer = _firstSelectionEnd.Y - _firstSelectionStart.Y;

                ObjSize.Content = $"{Math.Abs(widthDiffer)} x {Math.Abs(heightDiffer)} пкс";
            }
            else if (_main._selectionType == SelectionType.Custom && !(_selectionLine is null))
            {
                if (!(_selection is null)) return;
                IfPointOutOfBorderCustomSelection(e);

                _firstSelectionEnd = new Point(e.GetPosition(DrawingCanvas).X, e.GetPosition(DrawingCanvas).Y);

                Point point = GetHighestPoints(_selectionLine);
                ObjSize.Content = $"{Math.Abs(point.X)} x {Math.Abs(point.Y)} пкс";
            }
        }
        private Point GetHighestPoints(Polyline line)
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            foreach (Point point in line.Points)
            {
                if (point.X < minX) minX = point.X;
                if (point.Y < minY) minY = point.Y;
                if (point.X > maxX) maxX = point.X;
                if (point.Y > maxY) maxY = point.Y;
            }
            return new Point(Math.Abs(minX) + Math.Abs(maxX), Math.Abs(minY) + Math.Abs(maxY));
        }
        private void IfPointOutOfBorderCustomSelection(MouseEventArgs e)
        {
            if (_selectionLine is null) return;
            _selectionLine.RenderTransform = new TranslateTransform(0, 0);
            double x = e.GetPosition(DrawingCanvas).X;
            double y = e.GetPosition(DrawingCanvas).Y;

            Point tempPoint = e.GetPosition(DrawingCanvas);

            if (x < 0)
            {
                tempPoint = new Point(0, tempPoint.Y);
            }
            if (x > DrawingCanvas.Width)
            {
                tempPoint = new Point(DrawingCanvas.Width + 1, tempPoint.Y);
            }
            if (y < 0)
            {
                tempPoint = new Point(tempPoint.X, 0);
            }
            if (y > DrawingCanvas.Height)
            {
                tempPoint = new Point(tempPoint.X, DrawingCanvas.Height);
            }


            int xRes = (int)(tempPoint.X - Canvas.GetLeft(_selectionLine));
            int yRes = (int)(tempPoint.Y - Canvas.GetTop(_selectionLine));

            _selectionLine.Points.Add(new Point(xRes, yRes));
        }
        private bool IfRotationOutOfBordersRectSelection(MouseEventArgs e)
        {
            double xLoc = -1;
            double yLoc = -1;
            Size shapeSize = new Size(_selectionRect.Width, _selectionRect.Height);
            //Point mousePoint = new Point(e.GetPosition(DrawingCanvas).X, e.GetPosition(DrawingCanvas).Y);
            if (FigureRotation(e, _selectionRect))
            {
                double x = e.GetPosition(DrawingCanvas).X;
                double y = e.GetPosition(DrawingCanvas).Y;
                bool check = false;
                if (x < 0)
                {
                    xLoc = 0;
                    check = true;

                    //double differ = Math.Abs(_firstSelectionStart.X - shapeSize.Width);
                    _selectionRect.Width =  _firstSelectionStart.X;// shapeSize.Width;
                    //_firstSelectionStart.X = shapeSize.Width;
                    Canvas.SetLeft(_selectionRect, xLoc);

                    //_firstSelectionStart.X = shapeSize.Width;
                }
                else if (x > DrawingCanvas.Width)
                {
                    xLoc = DrawingCanvas.Width;
                    check = true;
                    double width = DrawingCanvas.Width - _firstSelectionStart.X;
                    _selectionRect.Width = width;
                }
                if (y <= 0)
                {
                    yLoc = 0;
                    check = true;
                    _selectionRect.Height = _firstSelectionStart.Y;// shapeSize.Height;
                    Canvas.SetTop(_selectionRect, yLoc);
                }
                else if (y > DrawingCanvas.Height)
                {
                    yLoc = DrawingCanvas.Height;
                    check = true;
                    double height = DrawingCanvas.Height - _firstSelectionStart.Y;
                    _selectionRect.Height = height;
                }
                if (check)
                {
                    _firstSelectionEnd = new Point(
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
            if (CheckForSpace()) return;
            if (_main._figType == FigureTypes.Line)
            {
                if (_main._figToPaint is null) return;
                Point startPoint = new Point(0, 0);
                Point endPoint = new Point
                (e.GetPosition(DrawingCanvas).X - Canvas.GetLeft(_main._figToPaint),
                e.GetPosition(DrawingCanvas).Y - Canvas.GetTop(_main._figToPaint));

                InitPointInLine(e, startPoint, endPoint, _main._figToPaint as Polyline);

                if (((Polyline)_main._figToPaint).Points.Count > 1)
                {
                    Point firstPoint = ((Polyline)_main._figToPaint).Points.First();
                    Point lastPoint = ((Polyline)_main._figToPaint).Points.Last();

                    ObjSize.Content = $"{Math.Abs(lastPoint.X)} x {Math.Abs(lastPoint.Y)} пкс";
                }
            }
            else if (_main._figType == FigureTypes.Polygon)
            {
                if (_main._figToPaint is null) return;
                if (_main._amountOfPointInPolygon != ((Polyline)_main._figToPaint).Points.Count)
                {
                    ((Polyline)_main._figToPaint).Points.RemoveAt(((Polyline)_main._figToPaint).Points.Count - 1);
                    ((Polyline)_main._figToPaint).Points.RemoveAt(((Polyline)_main._figToPaint).Points.Count - 1);
                }
                if (_main._amountOfPointInPolygon == 0)
                {
                    ((Polyline)_main._figToPaint).Points = new PointCollection();
                    Point startPoint = new Point(0, 0);
                    Point endPoint = new Point(e.GetPosition(DrawingCanvas).X - Canvas.GetLeft(_main._figToPaint),
                        e.GetPosition(DrawingCanvas).Y - Canvas.GetTop(_main._figToPaint));

                    InitPointInLine(e, startPoint, endPoint, _main._figToPaint as Polyline);
                }
                else
                {
                    Point startPoint = ((Polyline)_main._figToPaint).Points.Last();// [((Polyline)_figToPaint).Points.Count - 1];
                    Point endPoint = new Point(e.GetPosition(DrawingCanvas).X - Canvas.GetLeft(_main._figToPaint),
                         e.GetPosition(DrawingCanvas).Y - Canvas.GetTop(_main._figToPaint));

                    InitPointInLine(e, startPoint, endPoint, _main._figToPaint as Polyline);
                }
            }
            else if (_main._figType == FigureTypes.Curve)
            {
                if (_main._isDrawingLine)
                {
                    Point currentPoint = e.GetPosition(DrawingCanvas);
                    if (_main._polyline.Points.Count > 1)
                        _main._polyline.Points[1] = currentPoint;
                    else
                        _main._polyline.Points.Add(currentPoint);
                    if (_main._polyline.Points.Count > 1)
                    {
                        InitPointInLine(e, _main._polyline.Points.First(), _main._polyline.Points.Last(), _main._polyline as Polyline);

                        Point first = _main._polyline.Points.First();
                        Point last = _main._polyline.Points.Last();

                        ObjSize.Content = $"{Math.Abs(last.X - first.X)} x {Math.Abs(last.Y - first.Y)}";
                    }
                }
                else if (_main._isAdjustingCurve)
                {
                    Point currentPoint = e.GetPosition(DrawingCanvas);
                    _main._bezierSegment.Point2 = currentPoint;

                    //RenderOptions.SetEdgeMode(_main._figToPaint, EdgeMode.Aliased);
                    //_main._figToPaint.SnapsToDevicePixels = true;


                    System.Windows.Shapes.Path curveFig =
                        _main._figToPaint as System.Windows.Shapes.Path;
                    Rect bounds = curveFig.Data.Bounds;
                    GeneralTransform transform = curveFig.TransformToAncestor((Visual)curveFig.Parent);
                    Rect transformedBounds = transform.TransformBounds(bounds);
                    ObjSize.Content = $"{Math.Abs((int)transformedBounds.Width)} x {Math.Abs((int)transformedBounds.Height)}";
                }
            }
            else if (!(_main._figToPaint is null))
            {
                if (_main._figToPaint is null) return;
                FigureRotation(e, _main._figToPaint);
            }
        }
        private Point TurnLineFigWithShift(Point tempPoint, Point startPoint)
        {
            //8 positions
            //need to find closest string point             
            _paintDirection = GetLineDivision(tempPoint, startPoint);
            switch (_paintDirection)
            {
                case LineDivision.Up:
                    {
                        return new Point(startPoint.X, tempPoint.Y);
                    }
                case LineDivision.UpRight:
                    {
                        return GetMiddlePointForCrossLoc(tempPoint, (LineDivision)_paintDirection, startPoint);
                    }
                case LineDivision.Right:
                    {
                        return new Point(tempPoint.X, startPoint.Y);
                    }
                case LineDivision.DownRight:
                    {
                        return GetMiddlePointForCrossLoc(tempPoint, (LineDivision)_paintDirection, startPoint);
                    }
                case LineDivision.Down:
                    {
                        return new Point(startPoint.X, tempPoint.Y);
                    }
                case LineDivision.DownLeft:
                    {
                        return GetMiddlePointForCrossLoc(tempPoint, (LineDivision)_paintDirection, startPoint);
                    }
                case LineDivision.Left:
                    {
                        return new Point(tempPoint.X, startPoint.Y);
                    }
                case LineDivision.UpLeft:
                    {
                        return GetMiddlePointForCrossLoc(tempPoint, (LineDivision)_paintDirection, startPoint);
                    }
                default:
                    {
                        MessageBox.Show("How did you get here?");
                        break;
                    }
            }
            return tempPoint;
        }
        private LineDivision GetLineDivision(Point endPoint, Point startPoint)
        {
            if (endPoint.X > startPoint.X && endPoint.Y < startPoint.Y)//up right
            {
                if (IfCrossDirection(endPoint)) return LineDivision.UpRight;
                return Math.Abs(endPoint.Y) > Math.Abs(endPoint.X) ? LineDivision.Up : LineDivision.Right;
            }
            else if (endPoint.X > startPoint.X && endPoint.Y > startPoint.Y) //down right
            {
                if (IfCrossDirection(endPoint)) return LineDivision.DownRight;
                return Math.Abs(endPoint.Y) > Math.Abs(endPoint.X) ? LineDivision.Down : LineDivision.Right;
            }
            else if (endPoint.X < startPoint.X && endPoint.Y > startPoint.Y) //down left
            {
                if (IfCrossDirection(endPoint)) return LineDivision.DownLeft;
                return Math.Abs(endPoint.Y) > Math.Abs(endPoint.X) ? LineDivision.Down : LineDivision.Left;
            }
            else if (endPoint.X < startPoint.X && endPoint.Y < startPoint.Y) //up left
            {
                if (IfCrossDirection(endPoint)) return LineDivision.UpLeft;
                return Math.Abs(endPoint.Y) > Math.Abs(endPoint.X) ? LineDivision.Up : LineDivision.Left;
            }
            return LineDivision.Right;
        }
        private bool IfCrossDirection(Point endPoint)
        {
            //get middle 
            //check distance difference between middle point and zeroX, zeroY
            Point absPoint = new Point(Math.Abs(endPoint.X), Math.Abs(endPoint.Y));
            Point checkPoint = default;
            if (absPoint.X > absPoint.Y)
            {
                checkPoint = new Point(absPoint.Y, absPoint.X);
            }
            else if (absPoint.X < absPoint.Y)
            {
                checkPoint = new Point(absPoint.X, absPoint.X);
            }
            else checkPoint = absPoint;
            //compare difference between checkPoint and endPoint

            return (absPoint.Y > checkPoint.Y && absPoint.X > absPoint.Y - checkPoint.Y) ||
                 (absPoint.X > checkPoint.X && absPoint.Y > absPoint.X - checkPoint.X);
        }
        private Point GetMiddlePointForCrossLoc(Point point, LineDivision division, Point startPoint)
        {
            point = point.X > point.Y ? new Point(point.Y, point.Y) :
                    point.X < point.Y ? new Point(point.X, point.X) : point;
            switch (division)
            {
                case LineDivision.UpRight:
                    {
                        point = new Point(Math.Abs(point.X), point.Y > startPoint.X ? point.Y * -1 : point.Y);
                        return point;
                    }
                case LineDivision.DownRight:
                    {
                        point = new Point(Math.Abs(point.X), point.Y < startPoint.X ? point.Y * -1 : point.Y);
                        return point;
                    }
                case LineDivision.DownLeft:
                    {
                        point = new Point(point.X, point.Y > startPoint.X ? point.Y : point.Y * -1);
                        return point;
                    }
                case LineDivision.UpLeft:
                    {
                        point = new Point(point.X, point.Y > startPoint.X ? point.Y * -1 : point.Y);
                        return point;
                    }
                default:
                    {
                        return point;
                    }
            }
        }
        private void InitPointInLine(MouseEventArgs e, Point startPoint, Point endPoint, Polyline poly)
        {
            bool keyCheck = Keyboard.IsKeyDown(Key.LeftShift);
            if (keyCheck)
            {
                Point start = startPoint;
                Point end = endPoint;

                if (_main._figType == FigureTypes.Curve ||
                    _main._figType == FigureTypes.Polygon ||
                    _main._type == ActionType.Drawing)
                {
                    //convert point to system of cords point 
                    // point (0;0) - start Point
                    start = new Point(0, 0);
                    end = new Point(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
                }
                endPoint = TurnLineFigWithShift(end, start);
                if (_main._figType == FigureTypes.Curve ||
                    _main._figType == FigureTypes.Polygon ||
                    _main._type == ActionType.Drawing)
                {
                    //reconvert point in usual view
                    endPoint = new Point(endPoint.X + startPoint.X, endPoint.Y + startPoint.Y);
                }
            }
            if (_main._type == ActionType.Drawing) return;
            if (_main._figType == FigureTypes.Polygon)
            {
                poly.Points.Add(startPoint);
                poly.Points.Add(endPoint);
                return;
            }
            poly.Points = new PointCollection()
            {
                startPoint,
                endPoint
            };
        }
        private bool FigureRotation(MouseEventArgs e, Shape shape)
        {
            Point loc = new Point
             (Math.Min(_main.previousPoint.X, e.GetPosition(DrawingCanvas).X),
             Math.Min(_main.previousPoint.Y, e.GetPosition(DrawingCanvas).Y));

            Point sizePoint = e.GetPosition(DrawingCanvas);

            bool keyCheck = Keyboard.IsKeyDown(Key.LeftShift);
            if (keyCheck)
            {
                (loc, sizePoint) = SetPositionForFigureWithShift(loc, sizePoint);
            }
            SetPositionForFigureWithoutShift(shape, loc.X, loc.Y);

            double width = Math.Abs(sizePoint.X - _main.previousPoint.X);
            double height = Math.Abs(sizePoint.Y - _main.previousPoint.Y);

            shape.Width = width;
            shape.Height = height;

            if (SetSIzeIfShapeIsSelectionRect(shape)) return true;
            ObjSize.Content = $"{Math.Abs(shape.Width)} x {Math.Abs(shape.Height)} пкс";
            return true;
        }
        private bool SetSIzeIfShapeIsSelectionRect(Shape shape)
        {
            if (shape == _selectionRect)
            {
                int widthDiffer = (int)_firstSelectionEnd.X - (int)_firstSelectionStart.X;
                int heightDiffer = (int)_firstSelectionEnd.Y - (int)_firstSelectionStart.Y;

                ObjSize.Content = $"{Math.Abs(widthDiffer)} x {Math.Abs(heightDiffer)} пкс";
                return true;
            }
            return false;
        }
        private (Point, Point) SetPositionForFigureWithShift(Point point, Point mousePoint)
        {
            double xDiffer = mousePoint.X - _main.previousPoint.X;
            double yDiffer = (mousePoint.Y - _main.previousPoint.Y) * -1; //cause syst of cords is reversed

            double checkY = _main.previousPoint.Y - mousePoint.Y;
            double checkX = mousePoint.X - _main.previousPoint.X;

            Point checkPoint = GetEqualPoint(new Point(checkX, checkY));
            if (xDiffer > 0 && yDiffer > 0)
            {
                Point resLocPoint = new Point(point.X, _main.previousPoint.Y - checkPoint.Y);
                Point resSizePoint = new Point(_main.previousPoint.X + checkPoint.X, _main.previousPoint.Y - checkPoint.Y);
                return (resLocPoint, resSizePoint);
            }
            else if (xDiffer > 0 && yDiffer < 0)
            {
                Point resLocPoint = _main.previousPoint;
                Point resSizePoint = new Point(_main.previousPoint.X - checkPoint.X, _main.previousPoint.Y + checkPoint.Y);
                return (resLocPoint, resSizePoint);
            }
            else if (xDiffer < 0 && yDiffer < 0)
            {
                Point resLocPoint = new Point(_main.previousPoint.X - checkPoint.X, _main.previousPoint.Y);
                Point resSizePoint = new Point(_main.previousPoint.X - checkPoint.X, _main.previousPoint.Y + checkPoint.Y);
                return (resLocPoint, resSizePoint);
            }
            else if (xDiffer < 0 && yDiffer > 0)
            {
                Point resLocPoint = new Point(_main.previousPoint.X - checkPoint.X, _main.previousPoint.Y - checkPoint.Y);
                Point resSizePoint = new Point(_main.previousPoint.X + checkPoint.X, _main.previousPoint.Y + checkPoint.Y);
                return (resLocPoint, resSizePoint);
            }
            return (point, mousePoint);
        }
        private Point GetEqualPoint(Point point)
        {
            point = new Point(Math.Abs(point.X), Math.Abs(point.Y));

            if (point.X > point.Y)
            {
                return new Point(point.Y, point.Y);
            }
            else if (point.X < point.Y)
            {
                return new Point(point.X, point.X);
            }
            return point;
        }
        private void SetPositionForFigureWithoutShift(Shape shape, double x, double y)
        {
            if (shape is null) return;
            Canvas.SetLeft(shape, x);
            Canvas.SetTop(shape, y);
        }
        private void BrushPaint(MouseEventArgs e)
        {
            DrawingCanvas.RenderTransform = new TranslateTransform(0, 0);
            currentPoint = e.GetPosition(DrawingCanvas);

            if (_main._tempBrushType == BrushType.UsualBrush || _main._ifErasing || Cursor == _main._pencilCurs)
            {
                GetLineToPaint(currentPoint, e);
            }
            else if (_main._tempBrushType == BrushType.CalligraphyBrush)
            {
                CalligraphyBrushPaint(CalligraphyBrushAngle);
                AddCheckRect(e);
            }
            else if (_main._tempBrushType == BrushType.FountainPen)
            {
                CalligraphyBrushPaint(FountainBrushAngle);
                AddCheckRect(e);
            }
            else if (_main._tempBrushType == BrushType.Spray)
            {
                sprayTimer.Start();
                SprayPaint(currentPoint);
                AddCheckRect(e);
            }
            else if (_main._tempBrushType == BrushType.OilPaintBrush)
            {
                PaintByPngBrush(e);
                AddCheckRect(e);
            }
            else if (_main._tempBrushType == BrushType.ColorPencil)
            {
                PaintByPngBrush(e);
                AddCheckRect(e);
            }
            else if (_main._tempBrushType == BrushType.Marker)
            {
                _main.MarkerBrushPaint(e, DrawingCanvas);
                CheckForMarkerAmountOfPoints();
                AddCheckRect(e);
            }
            else if (_main._tempBrushType == BrushType.TexturePencil)
            {
                PaintByPngBrush(e);
                AddCheckRect(e);
            }
            else if (_main._tempBrushType == BrushType.WatercolorBrush)
            {
                PaintByPngBrush(e);
                AddCheckRect(e);
            }
            _main.previousPoint = currentPoint;

            DrawingCanvas.RenderTransform = new TranslateTransform(_horizontalOffset, 0);
        }
        private void CheckForMarkerAmountOfPoints()
        {
            int count = 0;
            for (int i = 0; i < _main.polylines.Count; i++)
            {
                count += _main.polylines[i].Points.Count;
            }

            const int pointToSetBg = 400;

            if (count >= pointToSetBg)
            {
                SetCanvasBg(DrawingCanvas);
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

        private bool IsPointInRadius(Point center, Point pointToCheck, double radius)
        {
            const int formulaPower = 2;

            double distance = Math.Sqrt(Math.Pow(pointToCheck.X - center.X, formulaPower) +
                                        Math.Pow(pointToCheck.Y - center.Y, formulaPower));
            return distance <= radius;
        }
        const int _dividerInMiddle = 2;
        const int _maxChildren = 100;
        private void CalligraphyBrushPaint(double angle)
        {
            Vector offset = new Vector(Math.Cos(angle) * _main.brushThickness / _dividerInMiddle,
                                Math.Sin(angle) * _main.brushThickness / _dividerInMiddle);

            Point[] points = new Point[4];
            points[0] = new Point(_main.previousPoint.X + offset.X, _main.previousPoint.Y + offset.Y);
            points[1] = new Point(_main.previousPoint.X - offset.X, _main.previousPoint.Y - offset.Y);
            points[2] = new Point(currentPoint.X - offset.X, currentPoint.Y - offset.Y);
            points[3] = new Point(currentPoint.X + offset.X, currentPoint.Y + offset.Y);

            const double thickness = 0.5;
            RectangleGeometry clipGeometry = new RectangleGeometry(new Rect(0, 0, DrawingCanvas.Width, DrawingCanvas.Height));
            Polygon polygon = new Polygon
            {
                Points = new PointCollection(points),
                Fill = _main.ColorToPaint,
                Stroke = _main.ColorToPaint,
                StrokeThickness = thickness,
                ClipToBounds = true,
                Clip = clipGeometry
            };

            DrawingCanvas.Children.Add(polygon);

            if (DrawingCanvas.Children.Count > _maxChildren)
            {
                SetCanvasBg(DrawingCanvas);
            }
        }
        private void InitializeSprayTimer()
        {
            const int sprayerTime = 50;
            sprayTimer = new DispatcherTimer();
            sprayTimer.Interval = TimeSpan.FromMilliseconds(sprayerTime);
            sprayTimer.Tick += SprayTimer_Tick;
        }
        private void SprayTimer_Tick(object sender, EventArgs e)
        {
            SprayPaint(currentPoint);
        }
        private void SprayPaint(Point point)
        {
            const int angleMultiplier = 2;
            const int ellipseSize = 1;

            for (int i = 0; i < sprayDensity; i++)
            {
                double angle = random.NextDouble() * angleMultiplier * Math.PI;
                double radius = Math.Sqrt(random.NextDouble()) * _main.brushThickness / _dividerInMiddle;

                double offsetX = radius * Math.Cos(angle);
                double offsetY = radius * Math.Sin(angle);

                Ellipse ellipse = new Ellipse
                {
                    Width = ellipseSize,
                    Height = ellipseSize,
                    Fill = _main.ColorToPaint
                };
                Canvas.SetLeft(ellipse, point.X + offsetX);
                Canvas.SetTop(ellipse, point.Y + offsetY);

                DrawingCanvas.Children.Add(ellipse);
            }
            SetCanvasBg(DrawingCanvas, ifSpray: true);
        }
        Polyline _drawPolyline = null;
        LineDivision? _paintDirection = null;
        public void GetLineToPaint(Point currentPoint, MouseEventArgs e)
        {
            if (_main._ifErasing)
            {
                double deltaX = Math.Abs(currentPoint.X - _main.previousPoint.X);
                double deltaY = Math.Abs(currentPoint.Y - _main.previousPoint.Y);

                if (deltaX > deltaY)
                {
                    currentPoint.Y = _main.previousPoint.Y;
                }
                else
                {
                    currentPoint.X = _main.previousPoint.X;
                }
            }
            Line line = new Line
            {
                X1 = _main.previousPoint.X,
                Y1 = _main.previousPoint.Y,
                X2 = currentPoint.X,
                Y2 = currentPoint.Y,

                Stroke = _main._ifErasing ? Brushes.White : _main.ColorToPaint,
                StrokeThickness = _main.brushThickness,
                StrokeStartLineCap = _main._ifErasing ? PenLineCap.Square : PenLineCap.Round,
                StrokeEndLineCap = _main._ifErasing ? PenLineCap.Square : PenLineCap.Round,
                StrokeLineJoin = _main._ifErasing ? PenLineJoin.Bevel : PenLineJoin.Round,
            };
            if (_main._type == ActionType.Drawing)
            {
                const int pointAmountUpdate = 100;
                if (_drawPolyline is null || _drawPolyline.Points.Count > pointAmountUpdate && _paintDirection is null)
                {
                    RectangleGeometry clipGeometry = new RectangleGeometry(new Rect(0, 0,
                        DrawingCanvas.Width, DrawingCanvas.Height));

                    _drawPolyline = new Polyline()
                    {
                        Stroke = _main.ColorToPaint,
                        StrokeThickness = _main.brushThickness,
                        StrokeStartLineCap = PenLineCap.Round,
                        StrokeEndLineCap = PenLineCap.Round,
                        StrokeLineJoin = PenLineJoin.Round,
                        Clip = clipGeometry,
                        ClipToBounds = true
                    };
                    RenderOptions.SetEdgeMode(_drawPolyline, EdgeMode.Aliased);

                    if (_paintDirection is null)
                    {
                        SetCanvasBg(DrawingCanvas);
                    }
                    DrawingCanvas.Children.Add(_drawPolyline);
                    AddCheckRect(e);
                }
                if (_drawPolyline.Points.Count > 1 && _paintDirection is null)
                {
                    InitPointInLine(e, _drawPolyline.Points.Last(), currentPoint, _drawPolyline);
                }
                if (!Keyboard.IsKeyDown(Key.LeftShift))
                {
                    _paintDirection = null;
                }
                if (!(_paintDirection is null))
                {
                    //Get point here
                    if (_drawPolyline.Points.Count != 0)
                    {
                        currentPoint = GetTransformedPoint(currentPoint, _drawPolyline.Points.Last());
                    }
                }
                _drawPolyline.Points.Add(currentPoint);
                return;
            }

            if (_main._ifErasing) DrawingCanvas.Children.Add(line);
        }
        private Point GetTransformedPoint(Point point, Point prevPoint)
        {
            switch (_paintDirection)
            {
                case LineDivision.Up:
                    {
                        point = new Point(prevPoint.X, point.Y);
                        return point;
                    }
                case LineDivision.UpRight:
                    {
                        return (prevPoint.Y > point.Y || prevPoint.X < point.X) ?
                            new Point(prevPoint.X + 1, prevPoint.Y - 1) :
                            new Point(prevPoint.X - 1, prevPoint.Y + 1);
                    }
                case LineDivision.Right:
                    {
                        point = new Point(point.X, prevPoint.Y);
                        return point;
                    }
                case LineDivision.DownRight:
                    {
                        return (prevPoint.Y < point.Y || prevPoint.X < point.X) ?
                            new Point(prevPoint.X + 1, prevPoint.Y + 1) :
                            new Point(prevPoint.X - 1, prevPoint.Y - 1);
                    }
                case LineDivision.Down:
                    {
                        point = new Point(prevPoint.X, point.Y);
                        return point;
                    }
                case LineDivision.DownLeft:
                    {
                        return (prevPoint.Y < point.Y || prevPoint.X > point.X) ?
                            new Point(prevPoint.X - 1, prevPoint.Y + 1) :
                            new Point(prevPoint.X + 1, prevPoint.Y - 1);
                    }
                case LineDivision.Left:
                    {
                        point = new Point(point.X, prevPoint.Y);
                        return point;
                    }
                case LineDivision.UpLeft:
                    {
                        return (prevPoint.X > point.X || prevPoint.Y > point.Y) ?
                            new Point(prevPoint.X - 1, prevPoint.Y - 1) :
                            new Point(prevPoint.X + 1, prevPoint.Y + 1);
                    }
                default:
                    {
                        MessageBox.Show("How did you get here?");
                        break;
                    }
            }
            return point;
        }
        private void Field_MouseUp(object sender, MouseEventArgs e)
        {
            if (!(CheckRect.Parent is null))
            {
                DrawingCanvas.Children.Remove(CheckRect);
            }
            if (_main._ifSelection && !_main.IfSelectionIsMacken)
            {
                SetImageComporator(DrawingCanvas);
                MakeSelection();
            }
            if (_main._ifFiguring) FiguringMouseUp();
            if (_main._ifDrawing || _main._ifErasing) ConvertPaintingInImage();

            CheckForFigurePainting();

            sprayTimer.Stop();



            //return;
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


        private void SelLineCheck()
        {
            if (!DrawingCanvas.Children.Contains(_selectionLine) && !(_selectionLine is null))
            {
                DependencyObject parent = VisualTreeHelper.GetParent(_selectionLine);
                if (parent is null) return;
                ((Canvas)_selectionLine.Parent).Children.Remove(_selectionLine);
            }
        }
        private void SetImageComporator(Canvas canvas)
        {
            SelLineCheck();
            canvas.RenderTransform = new TranslateTransform(0, 0);

            if (canvas.Background is SolidColorBrush) return;
            DrawingCanvas.Children.Remove(_selectionLine);
            _comparator = new Image()
            {
                Width = canvas.Width,
                Height = canvas.Height,
                Source = ((ImageBrush)canvas.Background).ImageSource,
                RenderTransform = new TranslateTransform(0, 0)
            };

            ResetRenderTransform();

            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
            (int)canvas.Width, (int)canvas.Height,
            _dpiParam, _dpiParam, PixelFormats.Pbgra32);

            RenderOptions.SetEdgeMode(renderBitmap, EdgeMode.Aliased);
            canvas.Measure(new Size((int)canvas.Width, (int)canvas.Height));
            canvas.Arrange(new Rect(new Size((int)canvas.Width, (int)canvas.Height)));
            renderBitmap.Render(canvas);

            _comparator.Source = renderBitmap;

            if (!(_selectionLine is null))
            {
                DrawingCanvas.Children.Add(_selectionLine);
            }

            canvas.RenderTransform = new TranslateTransform(_horizontalOffset, 0);
            _comparator.RenderTransform = new TranslateTransform(_horizontalOffset, 0);
        }
        private void SaveInHistory()
        {
            if (!_main._ifFiguring &&
                !_main._ifChangingFigureSize &&
                !_main._ifSelection && _selection is null &&
                _changedSizeText is null)
            {
                SaveCanvasState();
            }
        }
        public void FiguringMouseUp()
        {
            bool polygonCheck = IfPolygonFigureIsDone();
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
            if (_main._ifCurveIsDone) SetPathSize();

            return _main._ifCurveIsDone;
        }
        private int _amountOfParalelBorders = 2;
        public void SetPathSize()
        {
            if (_main._figToPaint is System.Windows.Shapes.Path path)
            {
                Rect bounds = path.Data.Bounds;

                GeneralTransform transform = path.TransformToAncestor((Visual)path.Parent);
                Rect transformedBounds = transform.TransformBounds(bounds);

                _main._figToPaint.Width = transformedBounds.Width + _amountOfParalelBorders;
                _main._figToPaint.Height = transformedBounds.Height + _amountOfParalelBorders;
            }
        }
        public bool IfPolygonFigureIsDone()
        {
            if (_main._figType != FigureTypes.Polygon) return true;

            const int lineClose = 10;
            const int amountOfPointToEndFigure = 2;
            if (((Polyline)_main._figToPaint).Points.Count > amountOfPointToEndFigure)
            {
                // Check for differ between first and last poit
                double xDiffer = Math.Abs(((Polyline)_main._figToPaint).Points.Last().X -
                    ((Polyline)_main._figToPaint).Points.First().X);
                double yDiffer = Math.Abs(((Polyline)_main._figToPaint).Points.Last().Y -
                    ((Polyline)_main._figToPaint).Points.First().Y);

                if (xDiffer <= lineClose && yDiffer <= lineClose)
                {
                    ((Polyline)_main._figToPaint).Points.RemoveAt(((Polyline)_main._figToPaint).Points.Count - 1);
                    ((Polyline)_main._figToPaint).Points.Add(((Polyline)_main._figToPaint).Points.First());

                    ((Polyline)_main._figToPaint).InvalidateVisual();
                    DrawingCanvas.UpdateLayout();

                    CalculatePolylineSize(_main._figToPaint);
                    return true;
                }
            }
            return false;
        }
        public void GetBoundingBox(UserControl control)
        {
            if (_main._figToPaint is Polyline polyline)
            {
                double minX = double.MaxValue;
                double minY = double.MaxValue;

                foreach (var point in polyline.Points)
                {
                    if (point.X < minX) minX = point.X;
                    if (point.Y < minY) minY = point.Y;
                }
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

                Canvas.SetLeft(_selection, canvasPosition.X - _amountOfParalelBorders);
                Canvas.SetTop(_selection, canvasPosition.Y - _amountOfParalelBorders);
            }
        }

        public void CalculatePolylineSize(Shape shape)
        {
            if (shape is Polyline polyline)
            {
                double minX = double.MaxValue;
                double maxX = double.MinValue;
                double minY = double.MaxValue;
                double maxY = double.MinValue;

                foreach (var point in polyline.Points)
                {
                    if (point.X < minX)
                        minX = point.X;
                    if (point.X > maxX)
                        maxX = point.X;
                    if (point.Y < minY)
                        minY = point.Y;
                    if (point.Y > maxY)
                        maxY = point.Y;
                }
                polyline.Width = maxX - minX;
                polyline.Height = maxY - minY;

                Console.WriteLine(polyline.ActualWidth);
                Console.WriteLine(polyline.ActualHeight);
            }
        }
        private void InitFigureInSizingBorder()
        {
            if (DrawingCanvas.Children.Count == 0) return;
            if (!(DrawingCanvas.Children[DrawingCanvas.Children.Count - 1] is Shape)) return;
            Shape createdFigure = (Shape)DrawingCanvas.Children[DrawingCanvas.Children.Count - 1];

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
                    if (!IfFigureSizeIsAcceptable(createdFigure)) return;
                }
            }
            if (_main._figType == FigureTypes.Line)
            {
                _lineSizing = new LineSizing((Polyline)_main._figToPaint, ObjSize);
                GetLinePositionOnCanvas();

                DrawingCanvas.Children.RemoveAt(DrawingCanvas.Children.Count - 1);
                DrawingCanvas.Children.Add(_lineSizing);
                _lineSizing.ClipOutOfBoundariesGeo();
            }
            else
            {
                _selection = new Selection(ObjSize, createdFigure);

                InitLocationForFigure(_selection);
                DrawingCanvas.Children.RemoveAt(DrawingCanvas.Children.Count - 1);

                _selection.SelectCan.Children.Add(createdFigure);

                createdFigure.Stretch = Stretch.Fill;

                Canvas.SetLeft(createdFigure, 0);
                Canvas.SetTop(createdFigure, 0);

                DrawingCanvas.Children.Add(_selection);
                _selection.ClipOutOfBoundariesGeo();
            }
            _main._type = ActionType.ChangingFigureSize;
            DrawingCanvas.ClipToBounds = false;
        }
        private bool IfShapeHasPoints(Shape shape)
        {
            return shape is Polyline line ? line.Points.Count > 0 :
                shape is Polygon polygon ? polygon.Points.Count > 0 :
                shape is System.Windows.Shapes.Path path ? !(path.Data is null) : false;
        }
        private bool IfFigureSizeIsAcceptable(Shape createdFigure)
        {
            if (createdFigure.Width is double.NaN || createdFigure.Width == 0 ||
                   createdFigure.Height is double.NaN || createdFigure.Height == 0)
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
                Canvas.SetTop(_selection, Canvas.GetTop(DrawingCanvas.Children
                    [DrawingCanvas.Children.Count - 1]) - _selection.DashedBorder.StrokeThickness);
                Canvas.SetLeft(_selection, Canvas.GetLeft(DrawingCanvas.Children
                    [DrawingCanvas.Children.Count - 1]) - _selection.DashedBorder.StrokeThickness);
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
        private const int _selectionSizeCorrelation = 0;
        private void MakeSelection(bool invertselType = false)
        {
            ResetRenderTransform();
            _ifTransparencyIsActivated = false;
            SetTransparentSelectionImage();

            if (!(_copyPolyLine is null) && !(_selection is null)) _selection.CheckCan.Children.Remove(_copyPolyLine);

            if (_main._selectionType == SelectionType.Rectangle ||
                (_main._selectionType == SelectionType.Invert && !invertselType))
            {
                DrawingCanvas.Children.Remove(_selectionLine);
                _selectionLine = null;
                MakeRectSelection(_selectionRect, ref _selection, DrawingCanvas);
            }
            else if (_main._selectionType == SelectionType.Custom ||
                (_main._selectionType == SelectionType.Invert && invertselType))
            {
                SetImageComporator(DrawingCanvas);
                MakeCustomSelection(invertselType);

                InitImageAfterMakingCustomSelection();
            }
            UpdateRenderTransform();
        }
        private void InitImageAfterMakingCustomSelection()
        {
            List<UIElement> elems = GetAllUIElementsExceptImages(DrawingCanvas);
            RemoveAllChildrenExceptImages(DrawingCanvas);

            SetCanvasBg(DrawingCanvas);

            InitElementsInCanvas(DrawingCanvas, elems);
        }
        private List<UIElement> GetAllUIElementsExceptImages(Canvas canvas)
        {
            List<UIElement> res = new List<UIElement>();

            for (int i = 0; i < canvas.Children.Count; i++)
            {
                if (!(canvas.Children[i] is Image))
                {
                    res.Add(canvas.Children[i]);
                }
            }
            return res;
        }
        private void MakeCustomSelection(bool ifInvertial = false)
        {

            ResetRenderTransform();

            //Add Last point(to make like a polygon)
            if (_selectionLine is null || _selectionLine.Points.Count == 0) return;
            //Get all point between first and last
            ConnectTwoPoints();
            _selectionLine.Points.Add(_selectionLine.Points.First());

            if (!IfPointsAreCorrect()) return;

            //Set polylines location
            if (!ifInvertial) SetPolylineLocation(_selectionLine);

            //Selection Creation
            if (!IfCustomSelectionIsCreated(_selectionLine, ifInvertial)) return;

            //if (ifInvertial) return;

            //Add line in selection
            AddSelectionLineInSelection(_selectionLine, ifInvertial);

            //Get polyline image
            //InitSelectedBgInRectCanvas();

            //if (ifInvertial) return;

            InitSelectionBgInCustomCanvas(ifInvertial);

            if (_selectionLine is null)
            {
                _selection.CheckCan.Children.Remove(_selectionLine);
                FreeSelection();
                UpdateRenderTransform();
                return;
            };

            _selection.SelectCan.Children.Remove(_selectionLine);
            _selection.CheckCan.Children.Add(_selectionLine);
            _selectionLine.Stretch = Stretch.Fill;
            Canvas.SetLeft(_selectionLine, 1);
            Canvas.SetTop(_selectionLine, 1);

            _main.IfSelectionIsMacken = true;

            UpdateRenderTransform();
        }
        public void UpdateWhiteColorInCanvas()
        {
            _selection.CheckCan.Children.Remove(_selectionLine);

            Image bgImage = ConvertBackgroundToImage(_selection.SelectCan);
            Image res = SwipeColorsInImage(bgImage, _whiteColor, _transparentColor);
            _selection.SelectCan.Background = new ImageBrush()
            {
                ImageSource = res.Source
            };
            _selection.CheckCan.Children.Add(_selectionLine);
        }
        private void ConnectTwoPoints()
        {

            Point firstPoint = _selectionLine.Points.Last();
            Point lastPoint = _selectionLine.Points.First();

            if (firstPoint.Equals(lastPoint)) return;

            List<(int, int)> points = GetLinePoints((int)firstPoint.X, (int)firstPoint.Y, (int)lastPoint.X, (int)lastPoint.Y);

            for (int i = 0; i < points.Count; i++)
            {
                _selectionLine.Points.Add(new Point(points[i].Item1, points[i].Item2));
            }
        }
        public static List<(int x, int y)> GetLinePoints(int x0, int y0, int x1, int y1)
        {
            List<(int x, int y)> points = new List<(int x, int y)>();

            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                points.Add((x0, y0));

                if (x0 == x1 && y0 == y1)
                    break;
                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }
            return points;
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
        public Image CloneImage(Image originalImage)
        {
            Image clone = new Image
            {
                Source = originalImage.Source,
                Width = originalImage.Width,
                Height = originalImage.Height,

            };
            RenderOptions.SetEdgeMode(clone, EdgeMode.Aliased);
            return clone;
        }
        private Polygon _checkPolygon;
        private void InitSelectionBgInCustomCanvas(bool ifInvertion)
        {
            SetPointCollectionForFigure(ifInvertion);

            DrawingCanvas.Children.Remove(_selectionLine);
            DrawingCanvas.Children.Remove(_selection);

            Image img = GetRenderOfCustomCanvasVTwo(_selectionLine, DrawingCanvas, ifInvertion);

            Image check = CloneImage(img);

            img = MakeComporation(img);
            img = SwipeColorsInImage(img, _whiteColor, _transparentColor);

            check = RepaintImageInWhite(check, _whiteColor, _checkPointCollection);
            PaintInCanvas(DrawingCanvas, check);

            DrawingCanvas.Children.Add(_selection);
            SetSelectionCanBgASImage(img, _selection);
        }
        private void PaintInCanvas(Canvas canvas, Image img)
        {
            canvas.RenderTransform = new TranslateTransform(0, 0);
            img.RenderTransform = new TranslateTransform(0, 0);
            RenderTargetBitmap bitmap = _main.ConvertCanvasToBitmap(canvas);
            Image image = new Image
            {
                Source = bitmap,
                Width = canvas.Width,
                Height = canvas.Height,
                RenderTransform = new TranslateTransform(0, 0)
            };

            BitmapSource globalImgBitMaoSource = image.Source as BitmapSource;
            WriteableBitmap writeableBitmapGlobalSource = new WriteableBitmap(globalImgBitMaoSource);
            int widthGlobal = writeableBitmapGlobalSource.PixelWidth;
            int heightGlobal = writeableBitmapGlobalSource.PixelHeight;
            int strideGlobal = widthGlobal * ((writeableBitmapGlobalSource.Format.BitsPerPixel + 7) / 8);
            byte[] pixelsGlobal = new byte[heightGlobal * strideGlobal];
            writeableBitmapGlobalSource.CopyPixels(pixelsGlobal, strideGlobal, 0);

            WriteableBitmap writeableBitmap = new WriteableBitmap(img.Source as BitmapSource);
            int width = writeableBitmap.PixelWidth;
            int height = writeableBitmap.PixelHeight;
            int stride = width * ((writeableBitmap.Format.BitsPerPixel + 7) / 8);
            byte[] pixels = new byte[height * stride];
            writeableBitmap.CopyPixels(pixels, stride, 0);

            Point selLoc = new Point(Canvas.GetLeft(_selection), Canvas.GetTop(_selection));

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Point tempPoint = new Point(selLoc.X + x + 1.5, selLoc.Y + y + 1);

                    int indexGlobal = (int)tempPoint.Y * strideGlobal + 4 * (int)tempPoint.X;

                    int index = y * stride + 4 * x;

                    byte blue = pixels[index];
                    byte green = pixels[index + 1];
                    byte red = pixels[index + 2];
                    byte alpha = pixels[index + 3];


                    byte globalBlue = pixelsGlobal[indexGlobal];
                    byte globalGreen = pixelsGlobal[indexGlobal + 1];
                    byte globalRed = pixelsGlobal[indexGlobal + 2];
                    byte globalAlpha = pixelsGlobal[indexGlobal + 3];


                    if (alpha != 0)
                    {
                        pixelsGlobal[indexGlobal] = 255;
                        pixelsGlobal[indexGlobal + 1] = 255;
                        pixelsGlobal[indexGlobal + 2] = 255;
                        pixelsGlobal[indexGlobal + 3] = 255;
                    }
                }
            }
            writeableBitmapGlobalSource.WritePixels(new Int32Rect(0, 0, widthGlobal, heightGlobal), pixelsGlobal, strideGlobal, 0);
            image.Source = writeableBitmapGlobalSource;
            RenderOptions.SetEdgeMode(image, EdgeMode.Aliased);

            canvas.Children.Add(image);
        }
        private Image MakeComporation(Image toCompare)
        {
            if (_comparator is null) return toCompare;

            Point selLoc = new Point(Canvas.GetLeft(_selection), Canvas.GetTop(_selection));
            Point endSelLoc = new Point(selLoc.X + _selection.Width, selLoc.Y + _selection.Height);
            Point selCanLoc = new Point(_selection.SelectCan.Width, _selection.SelectCan.Height);

            BitmapSource bitmapSource = toCompare.Source as BitmapSource;
            WriteableBitmap writeableBitmap = new WriteableBitmap(bitmapSource);
            int width = writeableBitmap.PixelWidth;
            int height = writeableBitmap.PixelHeight;
            int stride = width * ((writeableBitmap.Format.BitsPerPixel + 7) / 8);
            byte[] pixels = new byte[height * stride];
            writeableBitmap.CopyPixels(pixels, stride, 0);

            BitmapSource globalImgBitMaoSource = _comparator.Source as BitmapSource;
            WriteableBitmap writeableBitmapGlobalSource = new WriteableBitmap(globalImgBitMaoSource);
            int widthGlobal = writeableBitmapGlobalSource.PixelWidth;
            int heightGlobal = writeableBitmapGlobalSource.PixelHeight;
            int strideGlobal = widthGlobal * ((writeableBitmapGlobalSource.Format.BitsPerPixel + 7) / 8);
            byte[] pixelsGlobal = new byte[heightGlobal * strideGlobal];
            writeableBitmapGlobalSource.CopyPixels(pixelsGlobal, strideGlobal, 0);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Point tempPoint = new Point(selLoc.X + x + 1.5, selLoc.Y + y + 1);
                    int indexGlobal = (int)tempPoint.Y * strideGlobal + 4 * (int)tempPoint.X;

                    (byte blue, byte green, byte red, byte alpha) globalRGB = (pixelsGlobal[indexGlobal],
                        pixelsGlobal[indexGlobal + 1], pixelsGlobal[indexGlobal + 2], pixelsGlobal[indexGlobal + 3]);

                    int index = y * stride + 4 * x;

                    byte blue = pixels[index];
                    byte green = pixels[index + 1];
                    byte red = pixels[index + 2];
                    byte alpha = pixels[index + 3];


                    if (alpha != 0 && (globalRGB.blue != blue || globalRGB.green != green ||
                        globalRGB.red != red || globalRGB.alpha != alpha))
                    {
                        pixels[index] = globalRGB.blue;
                        pixels[index + 1] = globalRGB.green;
                        pixels[index + 2] = globalRGB.red;
                        pixels[index + 3] = globalRGB.alpha;
                    }
                }
            }
            writeableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
            toCompare.Source = writeableBitmap;
            return toCompare;
        }
        private PointCollection _checkPointCollection;
        private void SetPointCollectionForFigure(bool ifInvertial)
        {
            DrawingCanvas.RenderTransform = new TranslateTransform(0, 0);
            _selection.SelectCan.Children.Remove(_selectionLine);
            DrawingCanvas.Children.Add(_selectionLine);

            Point check = new Point(Canvas.GetLeft(_selectionLine), Canvas.GetTop(_selectionLine));
            Point checkSel = new Point(Canvas.GetLeft(_selection), Canvas.GetTop(_selection));


            //set pointCollection points transferуd on drawingCanvas
            SetCheckPointCollection(ifInvertial, check);

            DrawingCanvas.Children.Remove(_selectionLine);
            _selection.SelectCan.Children.Add(_selectionLine);

            DrawingCanvas.RenderTransform = new TranslateTransform(_horizontalOffset, 0);
        }
        private void SetCheckPointCollection(bool ifInvertial, Point selPoint)
        {
            if (!ifInvertial)
            {
                _checkPointCollection = new PointCollection();
                for (int i = 0; i < _selectionLine.Points.Count; i++)
                {
                    Point transformedPoint = _selectionLine.TransformToAncestor(DrawingCanvas).Transform(_selectionLine.Points[i]);

                    transformedPoint = new Point(transformedPoint.X - selPoint.X, transformedPoint.Y - selPoint.Y);
                    _checkPointCollection.Add(transformedPoint);
                }
                return;
            }
            //Get min polyline points param
            Point minPolylinePointsParam = GetMinPointParamsInPolyline(_selectionLine);
            Point startPolyPoint = GetStartPolylinePoine(selPoint, minPolylinePointsParam);


            PointCollection col = new PointCollection();

            //move up/left bounds to get right selection point (selpoint + moved point boundes )    
            for (int i = 0; i < _selectionLine.Points.Count; i++)
            {
                Point transformedPoint = _selectionLine.TransformToAncestor(DrawingCanvas).Transform(_selectionLine.Points[i]);

                transformedPoint = new Point(transformedPoint.X + _selectionLine.StrokeThickness, transformedPoint.Y + _selectionLine.StrokeThickness);
                /*
                                Point newPoint = new Point(
                                    selPoint.X + _selectionLine.Points[i].X + _selectionLine.StrokeThickness,
                                    selPoint.Y + _selectionLine.Points[i].Y + _selectionLine.StrokeThickness);*/
                col.Add(transformedPoint);
            }
            _checkPointCollection = col;
        }
        private Point GetStartPolylinePoine(Point polyPoint, Point minPolyPointsParam)
        {
            double xPoint = minPolyPointsParam.X < 0 ? minPolyPointsParam.X : 0;
            double yPoint = minPolyPointsParam.Y < 0 ? minPolyPointsParam.Y : 0;

            return new Point(polyPoint.X /*- xPoint*/, polyPoint.Y /*- yPoint*/);
        }

        private Point GetMinPointParamsInPolyline(Polyline polyline)
        {
            double x = int.MaxValue;
            double y = int.MaxValue;

            for (int i = 0; i < polyline.Points.Count; i++)
            {
                if (polyline.Points[i].X < x)
                {
                    x = polyline.Points[i].X;
                }
                if (polyline.Points[i].Y < y)
                {
                    y = polyline.Points[i].Y;
                }
            }
            return new Point(x, y);
        }

        private bool IfPointsAreCorrect()
        {
            if (_selectionLine is null) return false;
            for (int i = 0; i < _selectionLine.Points.Count; i++)
            {
                if (_selectionLine.Points[i].X != 0 ||
                    _selectionLine.Points[i].Y != 0)
                {
                    return true;
                }
            }
            return false;
        }
        public static Point GetPolylineAbsolutePoint(Polyline polyline, Point point)
        {
            Point relativePoint = new Point(point.X, point.Y);
            GeneralTransform transform = polyline.TransformToAncestor((Canvas)polyline.Parent);
            Point absolutePoint = transform.Transform(relativePoint);
            return absolutePoint;
        }
        public static PointCollection GetPolylinePointsRelativeToCanvas(Polyline polyline, Canvas canvas)
        {
            Point absolutePolyPoint =
                GetPolylineAbsolutePoint(polyline, polyline.Points.First());

            double polylineLeft = absolutePolyPoint.X;
            double polylineTop = absolutePolyPoint.Y;

            PointCollection absolutePoints = new PointCollection();

            foreach (Point point in polyline.Points)
            {
                Point absolutePoint = new Point(point.X + polylineLeft, point.Y + polylineTop);
                absolutePoints.Add(absolutePoint);
            }
            return absolutePoints;
        }
        private Point GetRightIfOutOfBoundaries(Point checkPoint)
        {
            if (checkPoint.X < 0)
            {
                checkPoint.X = 0;
            }
            else if (checkPoint.X >= DrawingCanvas.Width)
            {
                checkPoint.X = DrawingCanvas.Width;
            }
            if (checkPoint.Y < 0)
            {
                checkPoint.Y = 0;
            }
            else if (checkPoint.Y >= DrawingCanvas.Height)
            {
                checkPoint.Y = DrawingCanvas.Height;
            }
            return checkPoint;
        }
        private PointCollection _renderPointCollection;
        private Image GetRenderOfCustomCanvasVTwo(Polyline polyline, Canvas toRender, bool ifInvertion = false)
        {
            toRender.RenderTransform = new TranslateTransform(0, 0);
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            Point polyPos = new Point(Canvas.GetLeft(polyline), Canvas.GetTop(polyline));
            Point selPos = new Point(Canvas.GetLeft(_selection), Canvas.GetTop(_selection));

            Point polyLoc = GetPointOfSelection(polyline);

            for (int i = 0; i < polyline.Points.Count; i++)
            {
                Point transformedPoint = polyline.TransformToAncestor(_selection.SelectCan).Transform(polyline.Points[i]);

                if (ifInvertion)
                {
                    transformedPoint = new Point(selLineLoc.X + polyline.Points[i].X, selLineLoc.Y + polyline.Points[i].Y);
                }

                transformedPoint = new Point((int)transformedPoint.X, (int)transformedPoint.Y);

                transformedPoint = GetRightIfOutOfBoundaries(transformedPoint);

                if (transformedPoint.X <= minX) minX = transformedPoint.X;
                if (transformedPoint.Y <= minY) minY = transformedPoint.Y;
                if (transformedPoint.X >= maxX) maxX = transformedPoint.X;
                if (transformedPoint.Y >= maxY) maxY = transformedPoint.Y;
            }

            Point selPoint = new Point(Canvas.GetLeft(_selection), Canvas.GetTop(_selection));
            Console.WriteLine(_selection.Width + _selection.SelectCan.Width);
            Console.WriteLine(_selection.Height + _selection.SelectCan.Height);

            double width = maxX - minX;
            double height = maxY - minY;

            if (width == 0 || height == 0) return null;

            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)width, (int)height, _dpiParam, _dpiParam, PixelFormats.Pbgra32);

            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.PushTransform(new TranslateTransform(0, 0));
                RenderOptions.SetEdgeMode(drawingVisual, EdgeMode.Aliased);

                PathGeometry pathGeometry = new PathGeometry();
                RenderOptions.SetEdgeMode(pathGeometry, EdgeMode.Aliased);

                PathFigure pathFigure = new PathFigure
                {
                    StartPoint = new Point(polyline.Points[0].X - minX, polyline.Points[0].Y - minY),
                    IsClosed = true
                };

                for (int i = 1; i < polyline.Points.Count; i++)
                {
                    Point transformedPoint = polyline.TransformToAncestor(_selection.SelectCan).Transform(polyline.Points[i]);

                    if (ifInvertion)
                    {
                        transformedPoint = new Point(selLineLoc.X + polyline.Points[i].X, selLineLoc.Y + polyline.Points[i].Y);
                    }

                    transformedPoint = GetRightIfOutOfBoundaries(transformedPoint);

                    if (i == 0 || i == polyline.Points.Count - 1)
                    {
                        pathFigure.StartPoint = new Point(transformedPoint.X - minX, transformedPoint.Y - minY);
                    }
                    else
                    {
                        pathFigure.Segments.Add(new LineSegment(new Point(transformedPoint.X - minX, transformedPoint.Y - minY), true));
                    }
                }

                pathGeometry.Figures.Add(pathFigure);
                drawingContext.PushClip(pathGeometry);

                VisualBrush visualBrush = new VisualBrush(toRender)
                {
                    Stretch = Stretch.None,
                    ViewboxUnits = BrushMappingMode.Absolute,
                    Viewbox = new Rect(minX, minY, width, height),
                };

                RenderOptions.SetEdgeMode(visualBrush, EdgeMode.Aliased);

                drawingContext.DrawRectangle(visualBrush, null, new Rect(new Size(width, height)));
                drawingContext.Pop();
            }
            RenderOptions.SetEdgeMode(renderTargetBitmap, EdgeMode.Aliased);

            renderTargetBitmap.Render(drawingVisual);

            Image image = new Image
            {
                Source = renderTargetBitmap,
                Width = width,
                Height = height,
                Stretch = Stretch.Fill,
                RenderTransform = new TranslateTransform(0, 0)
            };
            toRender.RenderTransform = new TranslateTransform(_horizontalOffset, 0);
            return image;
        }
        private void AddSelectionLineInSelection(Polyline polyline, bool ifInverted)
        {
            DrawingCanvas.Children.Remove(polyline);
            _selection.SelectCan.Children.Add(polyline);

            double minXS = double.MaxValue;
            double minYS = double.MaxValue;

            foreach (var point in polyline.Points)
            {
                if (point.X <= minXS)
                    minXS = point.X;
                if (point.Y <= minYS)
                    minYS = point.Y;
            }

            if (!ifInverted)
            {
                Point start = new Point(Canvas.GetLeft(polyline), Canvas.GetTop(polyline));
                Canvas.SetLeft(polyline, 0 - minXS);
                Canvas.SetTop(polyline, 0 - minYS);
                Point end = new Point(Canvas.GetLeft(polyline), Canvas.GetTop(polyline));
            }

            _selection.SelectCan.Width = _selection.Width;
            _selection.SelectCan.Height = _selection.Height;
            Canvas.SetLeft(_selection.SelectCan, 0);
            Canvas.SetTop(_selection.SelectCan, 0);
        }

        private Size polySize;
        private bool IfCustomSelectionIsCreated(Polyline polyline, bool ifInterval = false)
        {
            const double selectionParamCorrelation = 0;
            double selectionLocCorrelation = 1;

            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            //Correct point scale
            if (ifInterval)
            {
                minX = _selectionLine.Points.Min(p => p.X);
                minY = _selectionLine.Points.Min(p => p.Y);
                maxX = _selectionLine.Points.Max(p => p.X);
                maxY = _selectionLine.Points.Max(p => p.Y);

                double originalWidth = maxX - minX;
                double originalHeight = maxY - minY;

                double newWidth = _selectionLine.Width;
                double newHeight = _selectionLine.Height;

                double scaleX = newWidth / originalWidth;
                double scaleY = newHeight / originalHeight;

                for (int i = 0; i < _selectionLine.Points.Count; i++)
                {
                    Point oldPoint = _selectionLine.Points[i];

                    double newX = (oldPoint.X - minX) * scaleX + minX;
                    double newY = (oldPoint.Y - minY) * scaleY + minY;

                    _selectionLine.Points[i] = new Point(newX, newY);
                }
                _selectionLine.InvalidateVisual();
            }

            minX = double.MaxValue;
            minY = double.MaxValue;
            maxX = double.MinValue;
            maxY = double.MinValue;

            foreach (var point in polyline.Points)
            {
                if (point.X < minX) minX = point.X;
                if (point.Y < minY) minY = point.Y;
                if (point.X > maxX) maxX = point.X;
                if (point.Y > maxY) maxY = point.Y;
            }

            polySize = new Size(_selectionLine.ActualWidth, _selectionLine.ActualHeight);

            // Point lineLoc = new Point(Canvas.GetLeft(_selectionLine), Canvas.GetTop(_selectionLine));
            // Console.WriteLine(lineLoc.X + lineLoc.Y);
            // Console.WriteLine(_selectionLine.ActualWidth + _selectionLine.ActualHeight);

            double polylineWidth = maxX - minX;
            double polylineHeight = maxY - minY;

            _selection = new Selection(ObjSize)
            {
                Height = polylineHeight + selectionParamCorrelation,
                Width = polylineWidth + selectionParamCorrelation
            };
            selectionLocCorrelation = _selection.DashedBorder.StrokeThickness;
            _selection.SelectionBorder.Height = polylineHeight + selectionParamCorrelation;
            _selection.SelectionBorder.Width = polylineWidth + selectionParamCorrelation;

            if (IfSelectionSizeIsZero()) return false;
            int xLoc = (int)Canvas.GetLeft(polyline) + (int)minX;
            int yLoc = (int)Canvas.GetTop(polyline) + (int)minY;

            Point asd = new Point(Canvas.GetLeft(_selection), Canvas.GetTop(_selection));

            if (!ifInterval)
            {
                Canvas.SetLeft(_selection, xLoc - selectionLocCorrelation);
                Canvas.SetTop(_selection, yLoc - selectionLocCorrelation);
            }
            else
            {
                Canvas.SetLeft(_selection, Canvas.GetLeft(polyline));
                Canvas.SetTop(_selection, Canvas.GetTop(polyline));
            }
            asd = new Point(Canvas.GetLeft(_selection), Canvas.GetTop(_selection));

            DrawingCanvas.Children.Remove(_selectionLine);

            AddBgImageInChildren(DrawingCanvas, ifInterval);

            DrawingCanvas.Children.Add(_selection);
            return true;
        }
        private void SetPolylineLocation(Polyline polyline)
        {
            GeneralTransform transform = polyline.TransformToAncestor(DrawingCanvas);
            Point canvasPosition = transform.Transform(new Point(0, 0));

            Point point = new Point(Canvas.GetLeft(polyline), Canvas.GetTop(polyline));

            Canvas.SetLeft(polyline, canvasPosition.X);
            Canvas.SetTop(polyline, canvasPosition.Y);
        }
        private void MakeRectSelection(Rectangle rect, ref Selection selection, Canvas selParentCan)
        {
            Point point = new Point(Canvas.GetLeft(_selectionRect), Canvas.GetTop(_selectionRect));
            double checkWidth = Math.Abs(_firstSelectionEnd.X - _firstSelectionStart.X);
            double checkHeight = Math.Abs(_firstSelectionEnd.Y - _firstSelectionStart.Y);

            _selectionRect.Width = checkWidth;
            _selectionRect.Height = checkHeight;

            //SetSelectionRectSize();
            CreateSelection(rect, ref selection, selParentCan);

            _main.IfSelectionIsMacken = true;
            if (IfSelectionSizeIsZero()) return;

            InitSelectedBgInRectCanvas(rect, selection, selParentCan);
        }

        private void SetSelectionRectSize()
        {
            /*
                        double width = Math.Abs(_firstSelectionEnd.X - _firstSelectionStart.X);
                        double height = Math.Abs(_firstSelectionEnd.Y - _firstSelectionStart.Y);

                        _selectionRect.Width = width;
                        _selectionRect.Height = height;*/

            double x = 0;
            double y = 0;

            if (_firstSelectionEnd.X >= _firstSelectionStart.X)
            {
                x = _firstSelectionStart.X + _selectionRect.Width;
            }
            else
            {
                x = _firstSelectionStart.X - _selectionRect.Width;
            }

            if (_firstSelectionEnd.Y >= _firstSelectionStart.Y)
            {
                y = _firstSelectionStart.Y + _selectionRect.Height;
            }
            else
            {
                y = _firstSelectionStart.Y - _selectionRect.Height;
            }
            _firstSelectionEnd = new Point(x, y);
        }

        public bool IfSelectionSizeIsZero()
        {
            if (IfSelectionSizeIsNotAcceptable())
            {
                _selection.IfSelectionIsClicked = false;
                _selection = null;
                _selectionLine = null;
                RemoveSelection();
                return true;
            }
            return false;
        }
        private bool IfSelectionSizeIsNotAcceptable()
        {
            const int sizeCheckParam = 1;
            return _selection.SelectionBorder.Width is double.NaN ||
                _selection.SelectionBorder.Height is double.NaN ||
                _selection.SelectionBorder.Width == _selectionSizeCorrelation ||
                _selection.SelectionBorder.Height == _selectionSizeCorrelation ||
                _selection.Height is double.NaN ||
                _selection.Width is double.NaN ||
                _selection.Height == _selectionSizeCorrelation ||
                _selection.Width == _selectionSizeCorrelation ||
                _selection.Width == sizeCheckParam || _selection.Height == sizeCheckParam;
        }
        private void CreateSelection(Shape shape, ref Selection selection, Canvas selParentCan)
        {
            if (selection is null)
            {
                selection = new Selection(ObjSize);
                //selection.SelectCan.ClipToBounds = true;

                selection.SelectionBorder.Height = shape.Height + _selectionSizeCorrelation;
                selection.SelectionBorder.Width = shape.Width + _selectionSizeCorrelation;

                _selection.Height = shape.Height;
                _selection.Width = shape.Width;

                _selection.SelectCan.Height = shape.Height;
                _selection.SelectCan.Width = shape.Width;

                double xLoc = Canvas.GetLeft(shape);
                double yLoc = Canvas.GetTop(shape);

                Canvas.SetLeft(selection, xLoc);
                Canvas.SetTop(selection, yLoc);
            }
            selParentCan.Children.Remove(shape);

            AddBgImageInChildren(selParentCan);

            selParentCan.Children.Add(selection);
        }
        private void CheckForFigurePainting()
        {
            if (_main._figToPaint is null) return;
            if (_main._figType != FigureTypes.Polygon)
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
            Console.WriteLine(ValueProgressBar.Height + paintInBlueCan.Height);
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

            double onePointHeight = (ValueProgressBar.Height - draggableButton.Height) /
                ValueProgressBar.Maximum;
            double temp = (int)pos / onePointHeight;

            double height = 250 - onePointHeight * temp;

            paintInBlueCan.Height = height;

            int brushSize = Math.Abs(((int)temp) - maxValueInProgressBar) + brushAdder;

            brushSize = brushSize == 0 ? 1 : brushSize;

            _main.brushThickness = brushSize;
            BrushSizeLB.Content = $"{brushSize} px";

        }
        private double prevYPos;
        private bool IfMadeThickBigger = false;
        private void ValueDrag_PreViewMouseDown(object sender, MouseEventArgs e)
        {
            SetStartPositionForBrushSizer(sender as Button);
        }
        private void SetStartPositionForBrushSizer(Button sender)
        {
            Button button = sender as Button;
            Point buttonPosition = button.TransformToAncestor(ValueCanvas)
                                         .Transform(new Point(0, 0));
            double centerX = buttonPosition.X + (button.ActualWidth / _dividerInMiddle);
            double centerY = buttonPosition.Y + (button.ActualHeight / _dividerInMiddle);

            IfThinBiggerCheck(centerY);

            if (IfMadeThickBigger && prevYPos != 0)
            {
                paintInBlueCan.Margin = new Thickness(0, paintInBlueCan.Margin.Top, 0,
                paintInBlueCan.Margin.Bottom);
            }
            else if (prevYPos != 0 && paintInBlueCan.Margin.Bottom > draggableButton.Height / _dividerInMiddle)
            {
                paintInBlueCan.Margin = new Thickness(0, paintInBlueCan.Margin.Top, 0,
               paintInBlueCan.Margin.Bottom);
            }
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
            else if (prevYPos - butYCord > 0)
            {
                IfMadeThickBigger = true;
            }
            else if (prevYPos - butYCord < 0)
            {
                IfMadeThickBigger = false;
            }
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
                    //string picPath = GetSourseForNewBrushType(_main.TempBrushType);
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
        public string GetSourceForNewBrushType(BrushType type)
        {
            return type == BrushType.UsualBrush ? "Images/BrushType/UsualBrush.png" :
                   type == BrushType.CalligraphyBrush ? "Images/BrushType/CalligraphyBrush.png" :
                   type == BrushType.FountainPen ? "Images/BrushType/FountainPen.png" :
                   type == BrushType.Spray ? "Images/BrushType/Spray.png" :
                   type == BrushType.OilPaintBrush ? "Images/BrushType/OilPaintBrush.png" :
                   type == BrushType.ColorPencil ? "Images/BrushType/ColorPencil.png" :
                   type == BrushType.Marker ? "Images/BrushType/Marker.png" :
                   type == BrushType.TexturePencil ? "Images/BrushType/TexturePencil.png" :
                    "Images/BrushType/WatercolorBrush.png";
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
            //DrawingCanvas.Children.Remove(_main._figToPaint);
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

            SolidColorBrush bgBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));

            if (currentIndex > 0)
            {
                currentIndex--;
                Image img = _canvasHistory[currentIndex];
                DrawingCanvas.Background = new ImageBrush()
                {
                    ImageSource = img.Source
                };
            }
            else if (currentIndex == 0)
            {
                DrawingCanvas.Background = bgBrush;
                currentIndex--;
                DrawingCanvas.Children.Clear();
            }
            DrawingCanvas.RenderTransform =
            new TranslateTransform(_horizontalOffset, 0);
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
                Image img = _canvasHistory[currentIndex];

                DrawingCanvas.Background = new ImageBrush()
                {
                    ImageSource = img.Source
                };
            }
            DrawingCanvas.RenderTransform =
            new TranslateTransform(_horizontalOffset, 0);
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
                SetCanvasBg(DrawingCanvas);
                DrawingCanvas.Children.Remove(_main._figToPaint);
                _main._figToPaint = null;
                ReloadCurvePainting();
            }
            return true;
        }
        private bool RemoveSelectionFromHistory()
        {
            if (_selection is null && _lineSizing is null) return true;

            RemoveSelection();
            _main.MakeAllActionsNegative();
            _selection = null;
            _selectionLine = null;
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
            Image img = _canvasHistory[currentIndex];
            DrawingCanvas.Background = new ImageBrush()
            {
                ImageSource = img.Source
            };
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
            new TranslateTransform(_horizontalOffset, 0);
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

            SetCanvasBg(DrawingCanvas);

            Canvas.SetLeft(img, 0);
            Canvas.SetTop(img, 0);
        }
        private void ImportOnDrawingCanvas_Click(object sender, EventArgs e)
        {
            Image img = GetImageFile();
            if (img is null) return;

            //Delete Selection From Canvas
            DrawingCanvas.Children.Remove(_selection);

            //Set it in selection
            _selection = new Selection(ObjSize)
            {
                Width = img.Width,
                Height = img.Height
            };
            _selection.SelectionBorder.Width = _selection.Width;
            _selection.SelectionBorder.Height = _selection.Height;
            _selection.SelectCan.Background = new ImageBrush()
            {
                ImageSource = img.Source
            };
            Canvas.SetLeft(_selection, 0);
            Canvas.SetTop(_selection, 0);

            DrawingCanvas.Children.Add(_selection);
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
        private void SetCanvasBg(Canvas canvas, bool ifSpray = false)
        {
            canvas.RenderTransform = new TranslateTransform(0, 0);
            Image bg = _main.ConvertCanvasInImage(canvas);
            //RenderOptions.SetEdgeMode(bg, EdgeMode.Aliased);

            //CheckImageColors(bg);
            if (ifSpray) RemoveChildrenExceptCheckRect(canvas);
            else canvas.Children.Clear();
            canvas.Background = new ImageBrush()
            {
                ImageSource = bg.Source
            };
            //canvas.RenderTransform = new TranslateTransform(_horizontalOffset, 0);
        }
        private void RemoveChildrenExceptCheckRect(Canvas canvas)
        {
            List<UIElement> elementsToRemove = new List<UIElement>();
            foreach (UIElement child in DrawingCanvas.Children)
            {
                if (child != CheckRect)
                {
                    elementsToRemove.Add(child);
                }
            }

            foreach (UIElement element in elementsToRemove)
            {
                DrawingCanvas.Children.Remove(element);
            }
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
                RemoveSelection();
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
                    _main.FirstColor = new SolidColorBrush(ColorConvertor.HexToRGB(but.Background.ToString()));
                }
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    _main.SecondColor = new SolidColorBrush(ColorConvertor.HexToRGB(but.Background.ToString()));
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
                if (_chosenToPaintButton is null ||
                    _chosenToPaintButton == FirstColor)
                {
                    _main.FirstColor = new SolidColorBrush(
                    ColorConvertor.HexToRGB(but.Background.ToString()));
                    SetColorAndBut(but, FirstColor);

                    return;
                }
                _main.SecondColor = new SolidColorBrush(
                ColorConvertor.HexToRGB(but.Background.ToString()));
                SetColorAndBut(but, SecondColor);

            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                if (_chosenToPaintButton is null ||
                    _chosenToPaintButton == FirstColor)
                {
                    _main.SecondColor = new SolidColorBrush(
                    ColorConvertor.HexToRGB(but.Background.ToString()));
                    SetColorAndBut(but, SecondColor);

                    return;
                }
                _main.FirstColor = new SolidColorBrush(
                ColorConvertor.HexToRGB(but.Background.ToString()));
                SetColorAndBut(but, FirstColor);

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
            //if (!(_richTexBox is null)) _richTexBox.Foreground = _main.FirstColor;
        }
        private void Color_PreviewMouseRightButtonDown(object sender, MouseEventArgs e)
        {
            InitColorForChosen(sender as Button, e);
        }
        private void CloseApp_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void SquareSelection_Click(object sender, EventArgs e)
        {
            _main._tempCursor = null;
            ClearDynamicValues();
            if (sender is MenuItem item)
            {
                _main._selectionType = item.Name == "ZeroSelection" ? SelectionType.Rectangle :
                item.Name == "OneSelection" ? SelectionType.Custom :
                item.Name == "TwoSelection" ? SelectionType.All :
                item.Name == "ThreeSelection" ? SelectionType.Invert :
                item.Name == "FourSelection" ? SelectionType.Transparent :
                item.Name == "FiveSelection" ? SelectionType.Delete : SelectionType.Nothing;

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
        private Point _firstSelectionStart;
        private Point _firstSelectionEnd;
        private void InitSelectedBgInRectCanvas(Rectangle rect, Selection selection, Canvas selParentCanvas)
        {
            if (_main._selectionType == SelectionType.Invert)
            {
                if (selParentCanvas == DrawingCanvas)
                {
                    DrawingCanvas.Children.Remove(selection);
                    Console.WriteLine(DrawingCanvas.Name);
                }
            }
            if (!(selection.SelectCan.Background is ImageBrush))
            {
                _firstSelectionStart = new Point(_firstSelectionStart.X, _firstSelectionStart.Y);

                RenderTargetBitmap copy = GetRenderedCopy(
                    selection, selParentCanvas, _firstSelectionStart, _firstSelectionEnd);
                InsertBitmapToCanvas(copy, selection);
            }

            int xLocCorel = 2;
            int yLocCorel = 1;
            const int sizeCorelX = 1;
            int sizeCorelY = 1;

            if (Canvas.GetLeft(rect) <= xLocCorel)
            {
                xLocCorel = 0;
            }
            if (Canvas.GetTop(rect) <= yLocCorel)
            {
                yLocCorel = 0;
                sizeCorelY = 0;
            }

            Point start = new Point(Canvas.GetLeft(rect) + xLocCorel, Canvas.GetTop(rect) + yLocCorel);
            Point end = new Point(start.X + rect.Width - sizeCorelX, start.Y + rect.Height - sizeCorelY);

            //selParentCanvas.Background = new SolidColorBrush(Colors.White);
            /*            if (_main._selectionType == SelectionType.All)
                        {
                            //selParentCanvas.Children.Clear();
                            selParentCanvas.Background = new SolidColorBrush(Colors.White);
                            selParentCanvas.Children.Add(selection);

                            return;
                        }*/

            //Get Bg Image
            //Paint bg in all white
            //add image in children
            //Init got image as bg (delete from children)
            DeleteAndTrimElements(start, end, selection, selParentCanvas);
        }
        private void AddBgImageInChildren(Canvas canvas, bool ifInvert = false)
        {
            //List<UIElement> canvasElements = ReAssignChildrenInAuxiliaryCanvas(canvas);
            Canvas can = GetAuxiliaryCanvas(canvas);

            //if (_selectionType == SelectionType.Invert) can.Children.Clear();

            Image img = _main.ConvertCanvasInImage(can);

            can.Background = new ImageBrush()
            {
                ImageSource = img.Source
            };


            Image bgImg = ConvertBackgroundToImage(can);
            canvas.Background = new SolidColorBrush(Colors.White);

            canvas.Children.Add(bgImg);
            if (ifInvert)
            {
                SetCanvasBg(canvas);
                canvas.Children.Clear();
            }
            can.Children.Clear();
        }
        public Image ConvertBackgroundToImage(Canvas canvas)
        {
            RenderTargetBitmap renderTarget = new RenderTargetBitmap(
                (int)canvas.ActualWidth,
                (int)canvas.ActualHeight,
                _dpiParam,
                _dpiParam,
                PixelFormats.Pbgra32);
            renderTarget.Render(canvas);

            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderTarget));
                encoder.Save(memoryStream);
                memoryStream.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }

            Image image = new Image()
            {
                Source = bitmapImage,
                Height = canvas.ActualHeight,
                Width = canvas.ActualWidth
            };

            image.UpdateLayout();
            return image;
        }
        private void DeleteAndTrimElements(Point point1, Point point2, Selection selection, Canvas parentCanvas)
        {
            const int imageWidth = 1000;
            const int imageHeight = 400;
            double minX = Math.Min(point1.X, point2.X);
            double minY = Math.Min(point1.Y, point2.Y);
            double maxX = Math.Max(point1.X, point2.X);
            double maxY = Math.Max(point1.Y, point2.Y);

            var elementsToRemove = new List<UIElement>();
            var elementsToAdd = new List<UIElement>();

            for (int i = 0; i < DrawingCanvas.Children.Count; i++)
            {
                var bounds = VisualTreeHelper.GetDescendantBounds(DrawingCanvas.Children[i]);

                var topLeft = DrawingCanvas.Children[i].
                    TransformToAncestor(DrawingCanvas).Transform(new Point(0, 0));
                if (DrawingCanvas.Children[i] is Image)
                {
                    ((Image)DrawingCanvas.Children[i]).Width = imageWidth;
                    ((Image)DrawingCanvas.Children[i]).Height = imageHeight;
                    if (bounds == Rect.Empty)
                    {
                        bounds = new Rect(0, 0, DrawingCanvas.Width, DrawingCanvas.Height);
                    }
                }
                var bottomRight = new Point(
                    topLeft.X + bounds.Width, topLeft.Y + bounds.Height);

                if (topLeft.X >= minX && topLeft.Y >= minY &&
                    bottomRight.X <= maxX && bottomRight.Y <= maxY)
                {
                    elementsToRemove.Add(DrawingCanvas.Children[i]);
                }
                else if (IsPartiallyInRegion(topLeft, bottomRight, minX, minY, maxX, maxY))
                {
                    if (DrawingCanvas.Children[i] is Image image)
                    {
                        var trimmedImages = TrimImage(image, minX, minY, maxX, maxY, topLeft);
                        if (trimmedImages != null && trimmedImages.Count > 0)
                        {
                            elementsToAdd.AddRange(trimmedImages);
                            elementsToRemove.Add(DrawingCanvas.Children[i]);
                        }
                    }
                    else
                    {
                        var trimmedElement = TrimElement(DrawingCanvas.Children[i],
                            minX, minY, maxX, maxY);
                        if (trimmedElement != null)
                        {
                            elementsToAdd.Add(trimmedElement);
                            elementsToRemove.Add(DrawingCanvas.Children[i]);
                        }
                    }
                }
            }
            for (int i = 0; i < elementsToRemove.Count; i++)
            {
                DrawingCanvas.Children.Remove(elementsToRemove[i]);
            }
            for (int i = 0; i < elementsToAdd.Count; i++)
            {
                DrawingCanvas.Children.Add(elementsToAdd[i]);
            }
            parentCanvas.Children.Remove(selection);
            Canvas copy = GetAuxiliaryCanvas(DrawingCanvas);
            Image img = _main.ConvertCanvasInImage(copy);
            parentCanvas.Children.Add(selection);

            DrawingCanvas.Background = new ImageBrush()
            {
                ImageSource = img.Source
            };
        }
        public Image ReplaceTransparentWithWhite(Image originalImage)
        {
            if (originalImage == null || originalImage.Source == null)
                return null;

            BitmapSource bitmapSource = originalImage.Source as BitmapSource;

            FormatConvertedBitmap convertedBitmap = new FormatConvertedBitmap();
            convertedBitmap.BeginInit();
            convertedBitmap.Source = bitmapSource;
            convertedBitmap.DestinationFormat = PixelFormats.Pbgra32;
            convertedBitmap.EndInit();

            WriteableBitmap writeableBitmap = new WriteableBitmap(convertedBitmap);

            int width = writeableBitmap.PixelWidth;
            int height = writeableBitmap.PixelHeight;
            int stride = writeableBitmap.BackBufferStride;

            byte[] pixels = new byte[height * stride];

            writeableBitmap.CopyPixels(pixels, stride, 0);

            for (int i = 0; i < pixels.Length; i += 4)
            {
                if (pixels[i + 3] == 0)
                {
                    pixels[i] = maxColorParam;     // Blue
                    pixels[i + 1] = maxColorParam; // Green
                    pixels[i + 2] = maxColorParam; // Red
                    pixels[i + 3] = maxColorParam; // Alpha
                }
            }

            writeableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);

            Image recoloredImage = new Image
            {
                Source = writeableBitmap,
                Width = originalImage.Width,
                Height = originalImage.Height
            };

            return recoloredImage;
        }
        const int maxColorParam = 255;
        private Image PaintImageInWhite(Image originalImage)
        {
            BitmapSource bitmapSource = originalImage.Source as BitmapSource;

            FormatConvertedBitmap convertedBitmap = new FormatConvertedBitmap();
            convertedBitmap.BeginInit();
            convertedBitmap.Source = bitmapSource;
            convertedBitmap.DestinationFormat = PixelFormats.Pbgra32;
            convertedBitmap.EndInit();

            WriteableBitmap writeableBitmap = new WriteableBitmap(convertedBitmap);

            int width = writeableBitmap.PixelWidth;
            int height = writeableBitmap.PixelHeight;
            int stride = writeableBitmap.BackBufferStride;

            byte[] pixels = new byte[height * stride];

            writeableBitmap.CopyPixels(pixels, stride, 0);

            for (int i = 0; i < pixels.Length; i += 4)
            {
                pixels[i] = maxColorParam;     // Blue
                pixels[i + 1] = maxColorParam; // Green
                pixels[i + 2] = maxColorParam; // Red
                pixels[i + 3] = maxColorParam; // Alpha
            }

            writeableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);

            Image recoloredImage = new Image
            {
                Source = writeableBitmap,
                Width = originalImage.Width,
                Height = originalImage.Height
            };
            return recoloredImage;
        }
        private Image ConvertListOfImagesIntoOne(List<Image> images)
        {
            Canvas can = new Canvas()
            {
                Height = DrawingCanvas.Height,
                Width = DrawingCanvas.Width
            };

            for (int i = 0; i < images.Count; i++)
            {
                can.Children.Add(images[i]);
            }

            return _main.ConvertCanvasInImage(can);
        }
        private List<Image> TrimImage(Image image, double minX, double minY,
            double maxX, double maxY, Point topLeft)
        {
            var bitmap = image.Source as BitmapSource;
            if (bitmap == null) return null;

            var trimmedImages = new List<Image>();
            var areas = new List<Int32Rect>
            {
                new Int32Rect(0, 0, (int)Math.Max(0, minX - topLeft.X), bitmap.PixelHeight),
                new Int32Rect((int)Math.Min(bitmap.PixelWidth, maxX - topLeft.X), 0,
                (int)Math.Max(0, bitmap.PixelWidth - (maxX - topLeft.X)), bitmap.PixelHeight),
                new Int32Rect(0, 0, bitmap.PixelWidth, (int)Math.Max(0, minY - topLeft.Y)),
                new Int32Rect(0, (int)Math.Min(bitmap.PixelHeight, maxY - topLeft.Y),
                bitmap.PixelWidth, (int)Math.Max(0, bitmap.PixelHeight - (maxY - topLeft.Y)))
            };

            foreach (var area in areas)
            {
                if (area.Width > 0 && area.Height > 0)
                {
                    var croppedBitmap = new CroppedBitmap(bitmap, area);
                    var newImage = new Image
                    {
                        Source = croppedBitmap,
                        Width = area.Width,
                        Height = area.Height
                    };

                    double offsetX = area.X == 0 ? topLeft.X : topLeft.X + area.X;
                    double offsetY = area.Y == 0 ? topLeft.Y : topLeft.Y + area.Y;

                    Canvas.SetLeft(newImage, offsetX);
                    Canvas.SetTop(newImage, offsetY);

                    trimmedImages.Add(newImage);

                    newImage = PaintImageInWhite(newImage);
                }
            }
            return trimmedImages;
        }
        private bool IsPartiallyInRegion(Point topLeft, Point bottomRight,
            double minX, double minY, double maxX, double maxY)
        {
            return (topLeft.X < maxX && bottomRight.X > minX &&
                topLeft.Y < maxY && bottomRight.Y > minY);
        }
        private UIElement TrimElement(UIElement element, double minX,
            double minY, double maxX, double maxY)
        {
            if (element is Line line)
            {
                return TrimLine(line, minX, minY, maxX, maxY);
            }
            else if (element is Polyline polyline)
            {
                return TrimPolyline(polyline, minX, minY, maxX, maxY);
            }

            //return new UIElement();
            throw new NotSupportedException("Unsupported element type for trimming");
        }
        private Polyline TrimPolyline(Polyline polyline, double minX,
            double minY, double maxX, double maxY)
        {
            var trimmedPoints = new PointCollection();

            foreach (var point in polyline.Points)
            {
                if (point.X >= minX && point.X <= maxX &&
                    point.Y >= minY && point.Y <= maxY)
                {
                    trimmedPoints.Add(point);
                }
            }

            return new Polyline
            {
                Points = trimmedPoints,
                Stroke = polyline.Stroke,
                StrokeThickness = polyline.StrokeThickness
            };
        }
        private Line TrimLine(Line line, double minX, double minY, double maxX, double maxY)
        {
            if (line.X1 >= minX && line.X1 <= maxX &&
                line.Y1 >= minY && line.Y1 <= maxY)
            {
                line.X1 = Math.Max(minX, Math.Min(line.X1, maxX));
                line.Y1 = Math.Max(minY, Math.Min(line.Y1, maxY));
            }
            if (line.X2 >= minX && line.X2 <= maxX &&
                line.Y2 >= minY && line.Y2 <= maxY)
            {
                line.X2 = Math.Max(minX, Math.Min(line.X2, maxX));
                line.Y2 = Math.Max(minY, Math.Min(line.Y2, maxY));
            }
            return line;
        }
        private void InsertBitmapToCanvas(RenderTargetBitmap bitmap, Selection selection)
        {
            var image = new Image
            {
                Source = bitmap,
                Width = selection.Width,
                Height = selection.Height
            };

            SwipeColorsInImage(image, _whiteColor, _transparentColor);
            Canvas.SetLeft(image, 0);
            Canvas.SetTop(image, 0);
            SetSelectionCanBgASImage(image, selection);
        }
        private void CheckImageColors(Image image, bool reset = false)
        {
            return;
            BitmapSource bitmapSource = image.Source as BitmapSource;
            WriteableBitmap writeableBitmap = new WriteableBitmap(bitmapSource);

            int width = writeableBitmap.PixelWidth;

            int height = writeableBitmap.PixelHeight;
            int stride = width * ((writeableBitmap.Format.BitsPerPixel + 7) / 8);
            byte[] pixels = new byte[height * stride];
            writeableBitmap.CopyPixels(pixels, stride, 0);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * stride + 4 * x;

                    byte blue = pixels[index];
                    byte green = pixels[index + 1];
                    byte red = pixels[index + 2];
                    byte alpha = pixels[index + 3];

                    if ((blue > 0 && blue < 255) || (green > 0 && green < 255) || (red > 0 && red < 255))
                    {
                        pixels[index] = 0;
                        pixels[index + 1] = 255;
                        pixels[index + 2] = 0;
                        pixels[index + 3] = 255;
                    }
                    else if (blue == 0 && green == 0 && red == 0 && alpha == 255)
                    {
                        Console.WriteLine();
                    }
                }
            }
            writeableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
            image.Source = writeableBitmap;

            if (!reset) return;

            RenderOptions.SetEdgeMode(writeableBitmap, EdgeMode.Aliased);

            DrawingCanvas.Background = new ImageBrush()
            {
                ImageSource = image.Source,
            };
        }

        private const int _maxColorParam = 255;
        private Image SwipeColorsInImage(Image image, Color toBeChanged, Color toChangeOn)
        {
            BitmapSource bitmapSource = image.Source as BitmapSource;
            WriteableBitmap writeableBitmap = new WriteableBitmap(bitmapSource);

            int width = writeableBitmap.PixelWidth;

            int height = writeableBitmap.PixelHeight;
            int stride = width * ((writeableBitmap.Format.BitsPerPixel + 7) / 8);
            byte[] pixels = new byte[height * stride];
            writeableBitmap.CopyPixels(pixels, stride, 0);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * stride + 4 * x;

                    byte blue = pixels[index];
                    byte green = pixels[index + 1];
                    byte red = pixels[index + 2];
                    byte alpha = pixels[index + 3];

                    if ((IfColorParamsBetweenFivePoints(red, toChangeOn.R) &&
                        IfColorParamsBetweenFivePoints(green, toChangeOn.G) &&
                        IfColorParamsBetweenFivePoints(blue, toChangeOn.B) &&
                        IfColorParamsBetweenFivePoints(alpha, toChangeOn.A)) ||

                        (red == _maxColorParam && green == _maxColorParam &&
                        blue == _maxColorParam && alpha == _maxColorParam) ||

                        (_ifTransparencyIsActivated && alpha == toBeChanged.A &&
                        red == toBeChanged.R && green == toBeChanged.G && blue == toBeChanged.B))
                    {
                        pixels[index] = toChangeOn.B;
                        pixels[index + 1] = toChangeOn.G;
                        pixels[index + 2] = toChangeOn.R;
                        pixels[index + 3] = toChangeOn.A;
                    }
                    if (alpha != 255 || (red == 255 && blue == 255 && green == 255))
                    {
                        pixels[index] = toChangeOn.B;
                        pixels[index + 1] = toChangeOn.G;
                        pixels[index + 2] = toChangeOn.R;
                        pixels[index + 3] = toChangeOn.A;
                    }
                }
            }
            writeableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
            image.Source = writeableBitmap;
            return image;
        }
        private Image RepaintImageInWhite(Image originalImage, Color targetColor, PointCollection points)
        {
            originalImage.RenderTransform = new TranslateTransform(0, 0);
            BitmapSource bitmapSource = originalImage.Source as BitmapSource;
            WriteableBitmap writeableBitmap = new WriteableBitmap(bitmapSource);

            RenderOptions.SetEdgeMode(writeableBitmap, EdgeMode.Aliased);
            int width = writeableBitmap.PixelWidth;
            int height = writeableBitmap.PixelHeight;
            int stride = width * ((writeableBitmap.Format.BitsPerPixel + 7) / 8);
            byte[] pixels = new byte[height * stride];
            writeableBitmap.CopyPixels(pixels, stride, 0);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * stride + 4 * x;

                    byte blue = pixels[index];
                    byte green = pixels[index + 1];
                    byte red = pixels[index + 2];
                    byte alpha = pixels[index + 3];

                    if (alpha > 0 /*&& !check.Contains(new Point(x, y))*/)
                    {
                        pixels[index] = targetColor.B;
                        pixels[index + 1] = targetColor.G;
                        pixels[index + 2] = targetColor.R;
                        pixels[index + 3] = targetColor.A;
                    }
                }
            }
            writeableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
            originalImage.Source = writeableBitmap;

            originalImage.RenderTransform = new TranslateTransform(_horizontalOffset, 0);
            return originalImage;
        }
        private bool IfColorParamsBetweenFivePoints(byte color, int colorParam)
        {
            const int maxColorDiffer = 5;
            int res = Math.Abs(color - colorParam);
            return res <= maxColorDiffer;
        }
        private void SetSelectionCanBgASImage(Image image, Selection selection)
        {
            ImageBrush imageBrush = new ImageBrush();
            RenderOptions.SetEdgeMode(imageBrush, EdgeMode.Aliased);
            imageBrush.ImageSource = image.Source;
            imageBrush.Stretch = Stretch.Fill;
            selection.SelectCan.Background = imageBrush;
        }
        private RenderTargetBitmap GetRenderedCopy(Selection selection, Canvas canvas,
            Point start, Point end)
        {
            const int pointCorel = 2;
            start = new Point(start.X + pointCorel, start.Y + pointCorel);
            var renderTargetBitmap = new RenderTargetBitmap(
                (int)selection.Width,
                (int)selection.Height,
                _dpiParam, _dpiParam,
                PixelFormats.Pbgra32
            );
            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                var visualBrush = new VisualBrush(canvas)
                {
                    Stretch = Stretch.None,
                    ViewboxUnits = BrushMappingMode.Absolute,
                    Viewbox = new Rect(start, end)
                };
                drawingContext.DrawRectangle(
                    visualBrush,
                    null,
                    new Rect(new Size(selection.Width, selection.Height))
                );
            }
            renderTargetBitmap.Render(drawingVisual);

            return renderTargetBitmap;
        }
        Point _windowPoint;
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            _windowPoint = e.GetPosition(this);
            if (!(_selection is null))
                Cursor = _selection._tempCursor;

            if (_main._ifSelection)
            {
                ChangeSelectionSize(e);
            }
            if (!(_changedSizeText is null) && _changedSizeText._isDraggingSelection)
            {
                _changedSizeText.ChangeSizeForSelection(e);
            }
            if (!(_selection is null) &&
                _selection._isDraggingSelection)
            {
                _selection.ChangeSizeForSelection(e);
            }
            else if (!(_selection is null) && _main._ifChangingFigureSize &&
                e.LeftButton == MouseButtonState.Pressed)
            {
                _selection._isDraggingSelection = true;
                _selection.ChangeSizeForSelection(e);
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
            if (!(_selection is null) &&
                DrawingCanvas.Children.Contains(_selection))
            {
                MakeChangeButActive(CutChange);
                MakeChangeButActive(CopyChange);

                if (_clickMenu is null) return;
                MakeChangeButActive(_clickMenu.Copy);
                MakeChangeButActive(_clickMenu.Cut);
                if (_copyBuffer is null) MakeChangeButInActive(_clickMenu.Paste);
            }
            else
            {
                MakeChangeButInActive(CutChange);
                MakeChangeButInActive(CopyChange);

                if (_clickMenu is null) return;
                MakeChangeButInActive(_clickMenu.Copy);
                MakeChangeButInActive(_clickMenu.Cut);
                if (_copyBuffer is null) MakeChangeButInActive(_clickMenu.Paste);
                MakeChangeButInActive(_clickMenu.InvertSelection);
            }

        }
        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!(_selection is null))
            {
                _selection._isDraggingSelection = false;
                _selection.IfSelectionIsClicked = false;
                _selection._selectionSizeToChangeSize = SelectionSide.Nothing;
            }
            else if (!(_changedSizeText is null))
            {
                _changedSizeText._isDraggingSelection = false;
                _changedSizeText.IfSelectionIsClicked = false;
                _changedSizeText._selectionSizeToChangeSize = SelectionSide.Nothing;
            }
            else if (!(_lineSizing is null))
            {
                _lineSizing._isDraggingSelection = false;
                _lineSizing.IfSelectionIsClicked = false;
                _lineSizing._moveRect = LineSizingRectType.Nothing;
            }
            if (!(_selection is null))
            {
                ClearCursorForSizing(_selection);
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
                bool polygonCheck = IfPolygonFigureIsDone();
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
            if (_main._figToPaint is null || _main._figType != FigureTypes.Polygon) return false;
            Point pos = e.GetPosition(DrawingCanvas);
            if (_main._figType == FigureTypes.Polygon &&
               IfPointOutOfBoundaries(pos) &&
               ((Polyline)_main._figToPaint).Points.Count >= 2)
            {
                ((Polyline)_main._figToPaint).Points.Add(((Polyline)_main._figToPaint).Points.First());
                return true;
            }
            return false;
        }
        private bool IfPointOutOfBoundaries(Point point)
        {
            return point.X < 0 || point.Y < 0 ||
                point.X > DrawingCanvas.Width ||
                point.Y > DrawingCanvas.Height;
        }
        private bool IfSelectionContainsAndClearIt()
        {
            bool check = true;
            ResetRenderTransform();
            if (!(_selection is null) && !_selection.IfSelectionIsClicked &&
               _main._type != ActionType.ChangingFigureSize)
            {
                ClearUsualSelection();
                check = false;
            }
            if (!(_selection is null) && !_selection.IfSelectionIsClicked)
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
            _selection.CheckCan.Children.Remove(_selectionLine);
            _selection.CheckCan.Children.Remove(_copyPolyLine);
            FreeSelection();
        }
        private void ClearSelectionWithFigure()
        {
            _selectionLine = null;
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
            _selectionLine = null;
        }
        private void FreeSelection()
        {
            ResetRenderTransform();

            if (_selection is null)
            {
                UpdateRenderTransform();
                return;
            }
            if (_main._selectionType == SelectionType.Invert)
            {
                _main._selectionType = SelectionType.Rectangle;
                CopmbineAllInvertation();
            }

            SetSelectedItemInDrawingCanvas();

            DrawingCanvas.Children.Remove(_selection);
            RemoveSelection();

            _selection = null;

            Image img = _main.ConvertCanvasInImage(DrawingCanvas);

            DrawingCanvas.Background = new ImageBrush()
            {
                ImageSource = img.Source
            };
            DrawingCanvas.Children.Clear();

            UpdateRenderTransform();
        }
        private void CopmbineAllInvertation()
        {
            //Get the deepest selection
            Selection deepestSelection = null;
            GetTheDeepestSelection(DrawingCanvas, ref deepestSelection);


            PutEverySelectionInDeepInOne(deepestSelection, null);

            _selection = GetHighestSelection();
        }
        private void PutEverySelectionInDeepInOne(Selection selection, Selection child)//From the Deepest
        {
            if (selection is null) return;

            //Get location
            Point selPoint = new Point(Canvas.GetLeft(selection), Canvas.GetTop(selection));

            //Remove child
            if (!(child is null)) selection.SelectCan.Children.Remove(child);

            //Init tempSel BG       
            //Get Bg Image
            Image selBg = _main.ConvertCanvasInImage(selection.SelectCan);

            //Set loc for selBg
            Canvas.SetLeft(selBg, selPoint.X + selection.DashedBorder.StrokeThickness);
            Canvas.SetTop(selBg, selPoint.Y + selection.DashedBorder.StrokeThickness);

            selection.Background = new ImageBrush()
            {
                ImageSource = selBg.Source
            };
            selection.SelectCan.Children.Clear();

            Selection parentSelectio = null;
            if (selection.Parent != DrawingCanvas)
            {
                parentSelectio =
                    GetSelectionsSelectionParent((Canvas)selection.Parent);

                parentSelectio.SelectCan.Children.Clear();
                parentSelectio.SelectCan.Children.Add(selBg);

                parentSelectio.SelectCan.Background = new ImageBrush()
                {
                    ImageSource = _main.ConvertCanvasInImage(parentSelectio.SelectCan).Source
                };
            }
            PutEverySelectionInDeepInOne(parentSelectio, selection);
        }
        private void GetParentSelection(Selection tempSelection, Selection highest)
        {
            _selection = tempSelection;
            if (tempSelection.Parent == DrawingCanvas)
            {
                FreeSelection();
                return;
            }

            DrawingCanvas.Children.Remove(highest);

            Canvas parentCan = (Canvas)tempSelection.Parent;
            Border parentBorder = (Border)parentCan.Parent;
            Selection parentSel = (Selection)parentBorder.Parent;

            parentSel.SelectCan.Children.Remove(_selection);
            DrawingCanvas.Children.Add(_selection);

            FreeSelection();

            DrawingCanvas.Children.Add(highest);

            GetParentSelection(parentSel, highest);
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
                //DrawingCanvas.Children.RemoveAt((int)_changedSizeText.Tag);
                _changedSizeText = null;
                UseAuxiliaryCanvasToCollectChildrenInDrawingCanvas();
            }
            DrawingCanvas.RenderTransform = new TranslateTransform(_horizontalOffset, 0);
        }
        private bool _ifSaved = false;
        private void ClearFigureSizing()
        {
            ResetRenderTransform();
            if (!(_selection is null))
            {
                InitSizedFigureIntoCanvas();
                _selection = null;
                _main._type = ActionType.Figuring;
                _main._figToPaint = null;
            }
            if (_selection is null &&
                _main._type == ActionType.Figuring &&
                _main._figType == FigureTypes.Polygon &&
                !(_main._figToPaint is null))
            {
                SaveCanvasState();
                DrawingCanvas.Children.Add(_main._figToPaint);
                SetCanvasBg(DrawingCanvas);
                _selection = null;
                _main._figToPaint = null;
            }
            else if (!_ifSaved)
            {
                if (!(_lineSizing is null))
                {
                    InitLineInCanvas();
                }
                SetCanvasBg(DrawingCanvas);
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

            //Point linePoistion = GetCordsForLineImage(lineToAdd, new Point(0, 0));
            List<Point> points = new List<Point>
            {
                GetCordsForLineImage(lineToAdd, new Point(lineToAdd.X1, lineToAdd.Y1)),
                GetCordsForLineImage(lineToAdd, new Point(lineToAdd.X2, lineToAdd.Y2))
            };

            _lineSizing.RemoveLineFromCanvas();

            DrawingCanvas.Children.Remove(_lineSizing);

            DrawingCanvas.Children.Add(lineToAdd);
            Canvas.SetLeft(lineToAdd, Canvas.GetLeft(_lineSizing));
            Canvas.SetTop(lineToAdd, Canvas.GetTop(_lineSizing));
            _lineSizing = null;

            SetCanvasBg(DrawingCanvas);
        }

        public Point GetCordsForLineImage(Line line, Point point)
        {
            if (_lineSizing.Parent != DrawingCanvas)
            {
                DrawingCanvas.Children.Add(_lineSizing);
            }
            GeneralTransform transform = line.TransformToAncestor(DrawingCanvas);
            Point canvasPosition = transform.Transform(point);

            return canvasPosition;
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
            var deleteElem = _selection.GetShapeElement();

            if (deleteElem is null)
            {
                DrawingCanvas.Children.Clear();
                UpdateRenderTransform();
                return;
            }
            DrawingCanvas.Children.Clear();
            _selection.RemoveObj();

            DrawingCanvas.Children.Add(deleteElem);
            Canvas.SetLeft(deleteElem, Canvas.GetLeft(_selection) +
                _selection.DashedBorder.StrokeThickness);
            Canvas.SetTop(deleteElem, Canvas.GetTop(_selection) +
                _selection.DashedBorder.StrokeThickness);

            //SetCanvasBg(DrawingCanvas);

            UpdateRenderTransform();
        }
        private void SetSelectedItemInDrawingCanvas()
        {
            const double yLocCorel = 1;
            const double xLocCorel = 1;

            if (_main._selectionType == SelectionType.Rectangle) ChangeRectSelectionCorel();
            Brush backgroundBrush = _selection.SelectCan.Background;

            if (backgroundBrush is ImageBrush imageBrush || !(img is null))
            {
                _selection.RemoveSizingGrid();

                Image check = img is null ? _main.ConvertCanvasInImage(_selection.SelectCan) : img;

                Console.Write($"{_selection.Width},{_selection.Height}, " +
                    $"{_selection.SelectCan.Width}, {_selection.SelectCan.Height}, " +
                    $"{_selection.SelectionBorder.Width}, {_selection.SelectionBorder.Height}, " +
                    $"{check.Width}, {check.Height},");

                _selection.AddSizingGrid();

                Canvas.SetLeft(check, Canvas.GetLeft(_selection) + xLocCorel);
                Canvas.SetTop(check, Canvas.GetTop(_selection) + yLocCorel);

                _selection.SelectCan.Children.Remove(check);
                DrawingCanvas.Children.Add(check);
                img = null;
            }
        }
        private void ChangeRectSelectionCorel()
        {
            _selection.SelectCan.InvalidateMeasure();
            _selection.SelectCan.InvalidateVisual();
            _selection.SelectCan.UpdateLayout();
        }
        private void RemoveSelection()
        {
            _ifTransparencyIsActivated = false;
            SetTransparentSelectionImage();

            for (int i = 0; i < DrawingCanvas.Children.Count; i++)
            {
                if (DrawingCanvas.Children[i] is Selection)
                {
                    DrawingCanvas.Children.RemoveAt(i);
                }
            }
            DrawingCanvas.Children.Remove(_selectionLine);
            ObjSize.Content = string.Empty;
        }
        private double _toolPartStartWidth;
        private double _figuresPartStartWidth;
        private double _figurePanelStartWidth;
        private double _colorsPartStartWidth;

        private double _startAdaptValue;
        private AdaptationStages _adaptStages = AdaptationStages.Start;

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
        private const int _figuresStep = 44;

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateTextLocation();
            ClearAdaptFigure();

            if (this.Width > _startAdaptValue)
            {
                _adaptStages = AdaptationStages.Start;
                FiguresPart.Width = new GridLength(_figuresPartStartWidth);
                FigurePanel.Width = _figurePanelStartWidth;
                return;
            }
            //first step in adapt (figures changing)
            else if (this.Width < _startAdaptValue &&
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
            FigureCanvasToAdapt.Visibility = Visibility.Visible;

            double figPartXLoc = SelectionPart.ActualWidth +
                          BrushPart.ActualWidth;
            double figPartYLoc = MainPanel.ActualHeight;

            double colorsWidth = _colorPanel.ActualWidth;
            double colorsHeight = _colorPanel.ActualHeight;

            FigureCanvasToAdapt.Children.Add(_colorPanel);
            _colorPanel.Visibility = Visibility.Visible;

            FigureCanvasToAdapt.Height = colorsHeight;
            FigureCanvasToAdapt.Width = colorsWidth;

            Canvas.SetLeft(_colorPanel, 0);
            Canvas.SetTop(_colorPanel, 0);

            FigureCanvasToAdapt.Margin =
            new Thickness(figPartXLoc, figPartYLoc, 0, 0);
        }
        private void ToolsAdaptBut_Click(object sender, RoutedEventArgs e)
        {
            ClearAdaptFigure();
            FigureCanvasToAdapt.Visibility = Visibility.Visible;

            double figPartXLoc = this.Width - (SelectionPart.ActualWidth +
                BrushPart.ActualWidth + ToolsPart.ActualWidth) - _toolsPanel.ActualWidth / 2;
            double figPartYLoc = MainPanel.ActualHeight;

            double toolsWidth = _toolsPanel.ActualWidth;
            double toolsHeight = _toolsPanel.ActualHeight;

            FigureCanvasToAdapt.Children.Add(_toolsPanel);
            _toolsPanel.Visibility = Visibility.Visible;

            FigureCanvasToAdapt.Height = toolsHeight;
            FigureCanvasToAdapt.Width = toolsWidth;

            Canvas.SetLeft(_figurePanel, 0);
            Canvas.SetTop(_figurePanel, 0);

            FigureCanvasToAdapt.Margin =
                new Thickness(-figPartXLoc, figPartYLoc, 0, 0);
        }
        private void AdaptFigureBut_Click(object sender, RoutedEventArgs e)
        {
            const double figAdaptMultiplier = 0.7;
            ClearAdaptFigure();
            FigureCanvasToAdapt.Visibility = Visibility.Visible;
            double figPartXLoc = SelectionPart.ActualWidth + ToolsPart.ActualWidth +
                BrushPart.ActualWidth;
            double figPartYLoc = MainPanel.ActualHeight;

            FigureCanvasToAdapt.Children.Add(_figurePanel);
            _figurePanel.Visibility = Visibility.Visible;
            FigurePanel.Width = _figurePanelStartWidth;
            //FiguresBorder.Width = _figurePanelStartWidth;

            FigureCanvasToAdapt.Height = FigGridPart.ActualHeight * figAdaptMultiplier;
            FigureCanvasToAdapt.Width = _figuresPartStartWidth;

            Canvas.SetLeft(_figurePanel, 0);
            Canvas.SetTop(_figurePanel, 0);

            FigureCanvasToAdapt.Margin =
                new Thickness(0, figPartYLoc, 0, 0);
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
        private void MakeColorTransformation()
        {
            PaletteAdaptBut.Visibility = Visibility.Visible;
            ColorPart.Visibility = Visibility.Hidden;
            ColorsPart.Width = new GridLength(_adaptedButsWidth);
            ColorColGrid.Children.Remove(_colorPanel);
        }
        private void DisableToolsTransformation()
        {
            ToolsPartPanel.Visibility = Visibility.Visible;
            ToolsAdaptBut.Visibility = Visibility.Hidden;
            ToolsPart.Width = new GridLength(_toolPartStartWidth);
            if (!ToolsGrid.Children.Contains(_toolsPanel))
            {
                ToolsGrid.Children.Add(_toolsPanel);
            }
        }
        private void MakeToolsTransformation()
        {
            ToolsPartPanel.Visibility = Visibility.Hidden;
            ToolsAdaptBut.Visibility = Visibility.Visible;
            ToolsPart.Width = new GridLength(_adaptedButsWidth);
            ToolsGrid.Children.Remove(_toolsPanel);
        }
        private void MakeFigureTransformationInAdaptation()
        {
            FiguresBorder.Visibility = Visibility.Hidden;
            AdaptFigureBut.Visibility = Visibility.Visible;
            FiguresPart.Width = new GridLength(_adaptedButsWidth);
            FigGridPart.Children.Remove(_figurePanel);
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
            _adaptStages = stage;
            FiguresPart.Width = new GridLength(_figuresPartStartWidth - cutStep * (int)stage);
            double differ = FiguresPart.ActualWidth - difference;
            FigurePanel.Width = differ;
        }
        private bool _ifDoubleClicked = false;
        private void PaintWindow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_main._ifTexting && !_ifDoubleClicked)
            {
                // SetTextInRichTextBox();

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
        }
        private void InitSettingsForTextBox()
        {
            _changedSizeText = new Selection(ObjSize);
            _changedSizeText.Height = 50;
            _changedSizeText.SelectionBorder.Height = 50;
            _changedSizeText.Width = 160;
            _changedSizeText.SelectionBorder.Width = 160;

            const int fontSize = 14;
            const int textBoxHeight = 40;
            const int textBoxWidth = 150;
            const int widthCorel = 15;
            const int startLoc = 5;

            _richTexBox = new RichTextBox()
            {
                Visibility = Visibility.Visible,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                FontSize = fontSize,
                AcceptsReturn = true,
                Height = textBoxHeight,
                Width = textBoxWidth,
                BorderThickness = new Thickness(3),
                BorderBrush = new SolidColorBrush(Colors.Green)
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
            _richTexBox.Selection.Select(_richTexBox.Selection.Start, _richTexBox.Selection.Start);

            Keyboard.ClearFocus();
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));

            double leftSize = _changedSizeText.DashedBorder.StrokeThickness;
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

            Canvas.SetLeft(img, Canvas.GetLeft(_changedSizeText) + leftSize);
            Canvas.SetTop(img, Canvas.GetTop(_changedSizeText) + leftSize);

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
        Image _comparator;
        private void PaintWindow_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ResetRenderTransform();
            /*if (_main._ifSelection && !_main.IfSelectionIsMacken)
            {

                SetImageComporator(DrawingCanvas);
                MakeSelection();
            }*/
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

            CheckImageColors(_main.ConvertCanvasInImage(DrawingCanvas));
            Console.WriteLine();
        }
        private void SwapPolylineWIthImage()
        {
            List<Shape> res = GetAllPolylines();
            RemoveAllPolylinesFromDrawingCanvas();
            Image img = ConvertShapesToImage(res, DrawingCanvas.Width, DrawingCanvas.Height);
            DrawingCanvas.Children.Add(img);
        }
        private void RemoveAllPolylinesFromDrawingCanvas()
        {
            for (int i = 0; i < DrawingCanvas.Children.Count; i++)
            {
                if (DrawingCanvas.Children[i] is Polyline)
                {
                    DrawingCanvas.Children.RemoveAt(i);
                    if (i != 0) i--;
                    else i = -1;
                }
            }
        }
        private List<Shape> GetAllPolylines()
        {
            List<Shape> res = new List<Shape>();
            for (int i = 0; i < DrawingCanvas.Children.Count; i++)
            {
                if (DrawingCanvas.Children[i] is Polyline)
                {
                    res.Add((Polyline)DrawingCanvas.Children[i]);
                }
            }
            return res;
        }
        private void SwapPolygonWithImage()
        {
            List<Shape> res = GetAllPolygonsInDrawingImage();
            RemoveAllPolygonsFromDrawingCanvas();
            Image img = ConvertShapesToImage(res, DrawingCanvas.Width, DrawingCanvas.Height);
            DrawingCanvas.Children.Add(img);
        }
        private List<Shape> GetAllPolygonsInDrawingImage()
        {
            List<Shape> res = new List<Shape>();
            for (int i = 0; i < DrawingCanvas.Children.Count; i++)
            {
                if (DrawingCanvas.Children[i] is Polygon)
                {
                    res.Add((Polygon)DrawingCanvas.Children[i]);
                }
            }
            return res;
        }
        private void RemoveAllPolygonsFromDrawingCanvas()
        {
            for (int i = 0; i < DrawingCanvas.Children.Count; i++)
            {
                if (DrawingCanvas.Children[i] is Polygon)
                {
                    DrawingCanvas.Children.RemoveAt(i);
                    if (i != 0) i--;
                    else i = -1;
                }
            }
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
            Canvas can = GetAuxiliaryCanvas(DrawingCanvas);
            Image res = _main.ConvertCanvasInImage(can);
            //DrawingCanvas.Children.Add(res);
            RepaintBgInCanvas(res, can);
        }
        private Canvas GetAuxiliaryCanvas(Canvas canvas)
        {
            Canvas can = new Canvas()
            {
                Height = canvas.Height,
                Width = canvas.Width,
                Background = canvas.Background,
            };
            RenderOptions.SetBitmapScalingMode(can, BitmapScalingMode.NearestNeighbor);

            List<UIElement> DrawingCanChildren = ReAssignChildrenInAuxiliaryCanvas(canvas);
            canvas.Children.Clear();

            for (int i = 0; i < DrawingCanChildren.Count; i++)
            {
                can.Children.Add(DrawingCanChildren[i]);
            }

            return can;
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
        private List<UIElement> ReAssignChildrenInAuxiliaryCanvas(Canvas canvas)
        {
            List<UIElement> res = new List<UIElement>();
            for (int i = 0; i < canvas.Children.Count; i++)
            {
                res.Add(canvas.Children[i]);
            }
            return res;
        }
        private void RemoveAllChildrenExceptImages(Canvas canvas)
        {
            var childrenToRemove = canvas.Children
                                 .OfType<UIElement>()
                                 .Where(child => child.GetType() != typeof(Image))
                                 .ToList();

            foreach (var child in childrenToRemove)
            {
                if (!(child is null))
                {
                    canvas.Children.Remove((UIElement)child);
                }
            }
        }
        private void DeleteAllLines()
        {
            for (int i = 0; i < DrawingCanvas.Children.Count; i++)
            {
                if (DrawingCanvas.Children[i] is Line)
                {
                    DrawingCanvas.Children.RemoveAt(i);
                    if (i != 0) i--;
                    else i = -1;
                }
            }
        }
        private List<Shape> GetAllDrawChildrenInLineType()
        {
            List<Shape> res = new List<Shape>();

            for (int i = 0; i < DrawingCanvas.Children.Count; i++)
            {
                if (DrawingCanvas.Children[i] is Line)
                {
                    res.Add((Line)DrawingCanvas.Children[i]);
                }
            }
            return res;
        }
        public Image ConvertShapesToImage(List<Shape> shapes, double width, double height)
        {
            Canvas canvas = new Canvas();

            canvas.Width = width;
            canvas.Height = height;
            canvas.Background = Brushes.Transparent;
            //RenderOptions.SetBitmapScalingMode(canvas, BitmapScalingMode.NearestNeighbor);
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

            return image;
        }
        private void ChooseAllSelection_MouseLeftButtonDown(object sender, EventArgs e)
        {
            ResetRenderTransform();
            RemoveRightClickMenus();

            Console.WriteLine(_selection);

            if (!(_selection is null) && !(_selection._shape is null)) ClearDynamicValues();
            if (!(_selection is null))
            {
                FreeSelection();
                SetSelectionInWholeDrawingCanvas();

                SetCanvasBg(DrawingCanvas);

                if (!(_selection.SelectCan.Background is ImageBrush))
                {
                    Image canBG = ConvertBackgroundToImage(DrawingCanvas);

                    _selection.SelectCan.Background = new ImageBrush()
                    {
                        ImageSource = canBG.Source
                    };
                }

                DrawingCanvas.Background = new SolidColorBrush(Colors.White);
                DrawingCanvas.Children.Add(_selection);
                _selection.IfSelectionIsClicked = false;
                ObjSize.Content = $"{DrawingCanvas.Width} x {DrawingCanvas.Height} пкс";
                UpdateRenderTransform();
                return;
            }
            _main._selectionType = SelectionType.All;
            SelectAllDrawingSelection();
            UpdateRenderTransform();
        }
        private void SelectAllDrawingSelection()
        {
            DrawingCanvas.Children.Clear();
            if (!(_selection is null))
            {
                //FreeSelection();
            }
            SetSelectionInWholeDrawingCanvas();

            Image BG = _main.ConvertCanvasInImage(DrawingCanvas);

            DrawingCanvas.Background = new SolidColorBrush(Colors.White);
            DrawingCanvas.Children.Add(_selection);

            _selection.SelectCan.Background = new ImageBrush()
            {
                ImageSource = BG.Source
            };
            _selection.IfSelectionIsClicked = false;
            ObjSize.Content = $"{DrawingCanvas.Width} x {DrawingCanvas.Height} пкс";
        }
        private void SetSelectionInWholeDrawingCanvas()
        {
            _selection = new Selection(ObjSize)
            {
                Height = DrawingCanvas.Height,
                Width = DrawingCanvas.Width
            };
            _selection.SelectionBorder.Height = DrawingCanvas.Height;
            _selection.SelectionBorder.Width = DrawingCanvas.Width;

            Canvas.SetTop(_selection, -_selection.DashedBorder.StrokeThickness);
            Canvas.SetLeft(_selection, -_selection.DashedBorder.StrokeThickness);

            _firstSelectionStart = new Point(0, 0);
            _firstSelectionEnd = new Point(DrawingCanvas.Width, DrawingCanvas.Height);

            _selectionRect.Height = DrawingCanvas.Height;
            _selectionRect.Width = DrawingCanvas.Width;

            Canvas.SetLeft(_selectionRect, 0);
            Canvas.SetTop(_selectionRect, 0);
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
            _main._tempCursor = null;
            ClearDynamicValues(brushClicked: true);
            if (sender is Button but)
            {
                PaintButBordsInClickedColor(but);
                if (but.Name == "PaintingBut")
                {
                    _main._type = ActionType.Drawing;
                }
                else
                {
                    //ClearDynamicValues(brushClicked: false);

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
            but.BorderThickness = new Thickness(1);
            but.BorderBrush = _clickedBorderColor;
        }
        private void ClearDynamicValues(bool brushClicked = false, bool textClicked = false)
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
            const string path = "B:\\GitHub\\PaintWPF\\toSaveFonts\\font.png";

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
        private bool _ifTransparencyIsActivated;
        private void TransparentSelection_Click(object sender, EventArgs e)
        {
            const int selectionCorrelation = 1;
            if (_selection is null) return;
            if (_selection is null &&
                (_main._selectionType == SelectionType.Rectangle ||
                _main._selectionType == SelectionType.Custom)) return;
            if (_ifTransparencyIsActivated)
            {
                _selection.SelectCan.Background = new ImageBrush()
                {
                    ImageSource = _transparentImage.Source
                };
                _ifTransparencyIsActivated = false;
                SetTransparentSelectionImage();
                Canvas.SetTop(_selection, Canvas.GetTop(_selection) - selectionCorrelation);
                Canvas.SetLeft(_selection, Canvas.GetLeft(_selection) - selectionCorrelation);
                return;
            }
            _ifTransparencyIsActivated = true;
            SetTransparentSelectionImage();

            List<UIElement> selectionElems =
                ReAssignChildrenInAuxiliaryCanvas(_selection.SelectCan);
            _selection.SelectCan.Children.Clear();

            _transparentImage = ConvertBackgroundToImage(_selection.SelectCan);

            Image redoneBgIMG = SwipeColorsInImage(
                ConvertBackgroundToImage(_selection.SelectCan),
              _main.SecondColor.Color, _transparentColor);

            InitElemsInCanvas(_selection.SelectCan, selectionElems);

            _selection.SelectCan.Background = new ImageBrush()
            {
                ImageSource = redoneBgIMG.Source
            };
            Canvas.SetTop(_selection, Canvas.GetTop(_selection) - selectionCorrelation);
            Canvas.SetLeft(_selection, Canvas.GetLeft(_selection) - selectionCorrelation);
        }
        private void SetTransparentSelectionImage()
        {
            if (_ifTransparencyIsActivated)
            {
                TransSelectionIcon.Source = _tickImg.Source;
                return;
            }
            TransSelectionIcon.Source = null;
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
        private int _deepCounter = 0;

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
        private void InvertSelection_Click(object sender, EventArgs e)
        {
            RemoveRightClickMenus();
            
            if (_main._type == ActionType.ChangingFigureSize) return;
            RemoveRightClickMenus();
            if (_selection is null) return;
            _main._selectionType = SelectionType.Invert;

            List<UIElement> elems = ReAssignChildrenInAuxiliaryCanvas(DrawingCanvas);
            /*DrawingCanvas.Children.Clear();*/
            RemoveAllChildrenExceptImages(DrawingCanvas);
            SetCanvasBg(DrawingCanvas);
            AddListOfElemsWithoutImages(elems, DrawingCanvas);
            /*
                        if (_selection.CheckCan.Children.Contains(_selectionLine))
                        {
                            _selection.CheckCan.Children.Remove(_selectionLine);
                        }
            */
            //Selection in selection in selection...
            Selection highestBeforeAllSell = GetHighestSelection();

            //CheckSelectionsParams(DrawingCanvas);

            //go FROM the Deepest selection (get deepest selection)
            Selection deepestSelection = null;
            GetTheDeepestSelection(DrawingCanvas, ref deepestSelection);

            //Clear all selection (put their BGs in drawingCanvas)
            FreeAllSelectionsInInversionSelection(highestBeforeAllSell);
            //FreeSelectionInInvertion(highestBeforeAllSell, new Point(Canvas.GetLeft(highestBeforeAllSell), Canvas.GetTop(highestBeforeAllSell)));

            // return;// FIRST CHECKPOINT 


            //Init bg in every of them (!again! from deepest)
            InitBGForSelectionFromDeepest(deepestSelection, highestBeforeAllSell, 0);
            //MakeAllSelSBorderVisiable(_selection);

            //Get All drawingCanvas Children
            List<UIElement> drawCanvasElems = ReAssignChildrenInAuxiliaryCanvas(DrawingCanvas);
            //clear selections
            DrawingCanvas.Children.Clear();
            //Add all Selection

            _main._selectionType = SelectionType.All;
            SelectAllDrawingSelection();
            _main._selectionType = SelectionType.Invert;

            //Add all DrawingSelection Children To new all Selection
            InitElementsInCanvas(_selection.SelectCan, drawCanvasElems);

            //remove selection grid for all children selections()
            RemoveSelectionGridInDeep(_selection, 0);


            // Add selection int new all Selection
            //_selection.SelectCan.Children.Add(highestBeforAllSell);

            //DrawingCanvas.Background = new SolidColorBrush(Colors.White);
            //return; //THIRD CHECKPOINT 

            //CheckAllChildren(DrawingCanvas);

            //Make invertation
            //From all selection. Free every second selection(set selections bg in DrawingImage)

            MakeInvertSelection(DrawingCanvas);

            //clear highest
            //Set highest Sekection in SystemSelection
            SetHighestSelectionInSystemSelection();

            //Remove Sizing grids
            RemoveSelectionGridInDeep(highestBeforeAllSell, 0);
        }
        private void CheckAllChildren(Canvas canvas)
        {
            for (int i = 0; i < canvas.Children.Count; i++)
            {
                if (canvas.Children[i].GetType() == typeof(Selection))
                {
                    Console.WriteLine(((Selection)canvas.Children[i]).Name);
                    CheckAllChildren(((Selection)canvas.Children[i]).SelectCan);
                }
            }
        }
        private void TESTFreeingInvertSelection()
        {
            //MISHA, THIS IS TEST 
            DrawingCanvas.Children.Clear();

            _selection = InitSelection(990, 390, new Point(10, 10), Colors.Orange, "highest", Colors.Green);
            Selection middle = InitSelection(600, 300, new Point(50, 50), Colors.Red, "middle", Colors.Purple);
            Selection low = InitSelection(150, 150, new Point(50, 50), Colors.Yellow, "low", Colors.Blue);
            Selection lowest = InitSelection(100, 100, new Point(40, 40), Colors.LightGreen, "lowest", Colors.DarkGreen);

            _selection.CheckCan.Children.Add(middle);
            middle.SelectCan.Children.Add(low);
            low.SelectCan.Children.Add(lowest);

            DrawingCanvas.Children.Add(_selection);
            //_selection = highest;

            middle.RemoveSizingGrid();
            low.RemoveSizingGrid();
            lowest.RemoveSizingGrid();
        }
        private Selection InitSelection(double width, double height,
            Point loc, Color color, string name, Color borderBrush)
        {
            Selection res = new Selection(ObjSize)
            {
                Width = width,
                Height = height,
                Name = name
            };

            res.SelectCan.Width = width;
            res.SelectCan.Height = height;
            res.SelectionBorder.Width = width;
            res.SelectionBorder.Height = height;
            res.SelectionBorder.BorderBrush = new SolidColorBrush(borderBrush);

            res.SelectCan.Background = new SolidColorBrush(color);
            Canvas.SetLeft(res, loc.X);
            Canvas.SetTop(res, loc.Y);
            return res;
        }

        private void FreeSelectionInInvertations(Selection selection, Point selLoc)
        {
            if (selection is null) return;

            //Remove From Parent Canvas

            //Get and Remove children from it
            List<UIElement> selChildren = ReAssignChildrenInAuxiliaryCanvas(selection.SelectCan);
            selection.SelectCan.Children.Clear();

            //Convert SelectCan BG in Image
            selection.DashedBorder.Visibility = Visibility.Hidden;
            Image img = _main.ConvertCanvasInImage(selection.SelectCan);

            //Set It in drawingCanvas
            Canvas.SetLeft(img, selLoc.X + 1);
            Canvas.SetTop(img, selLoc.Y + 1);
            DrawingCanvas.Children.Add(img);

            //Get Temp Selection Loc + children Selection Loc
            Selection childSelection = GetSelectionFormList(selChildren);
            Point newSelectionPoint = childSelection is null ? new Point(0, 0) : new Point(Canvas.GetLeft(childSelection), Canvas.GetTop(childSelection));
            Point newSelectionGlobalPoint = new Point(selLoc.X + newSelectionPoint.X, selLoc.Y + newSelectionPoint.Y);

            //Set DrawingCanvas Bg
            SetCanvasBg(DrawingCanvas);

            //Return in start places(parent + children)
            AddListElemsInCanvas(selChildren, selection.SelectCan);


            //Recursion In selection + its loc 
            FreeSelectionInInvertations(childSelection, newSelectionGlobalPoint);
            //What to Do with selection Line?
            //So if it selection line, it will be in the deepest selection 
        }
        private void AddListOfElemsWithoutImages(List<UIElement> elems, Canvas canvas)
        {
            for (int i = 0; i < elems.Count; i++)
            {
                if (!(elems[i] is Image))
                {
                    canvas.Children.Add(elems[i]);
                }
            }
        }
        private void AddListElemsInCanvas(List<UIElement> elems, Canvas canvas)
        {
            if (elems.Any(x => x is Polyline)) return;
            for (int i = 0; i < elems.Count; i++)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(elems[i]);

                if (parent is null)
                {
                    canvas.Children.Add(elems[i]);
                }
            }
        }
        private Selection GetSelectionFormList(List<UIElement> elems)
        {
            for (int i = 0; i < elems.Count; i++)
            {
                if (elems[i] is Selection)
                {
                    return (Selection)elems[i];
                }
            }
            return null;
        }
        private void FreeAllSelectionsInInversionSelection(Selection selection)
        {
            if (selection is null) return;

            //Get selection point
            //Point selectionPoint = GetPointOfSelection(selection);
            (Point selectionPoint, int corelCounter) = GetSelectionPointComparedToDrawingCanvas(selection, new Point(0, 0), 0);
            selectionPoint = CorrelateGotPoint(selectionPoint, corelCounter);

            Canvas CanToMakeFree = selection.SelectCan;

            //remove children
            List<UIElement> elems = ReAssignChildrenInAuxiliaryCanvas(selection.CheckCan);
            selection.CheckCan.Children.Clear();

            //selection.DashedBorder.Visibility = Visibility.Hidden;
            List<UIElement> selectCanElems = ReAssignChildrenInAuxiliaryCanvas(selection.SelectCan);
            selection.SelectCan.Children.Clear();

            //Convert selection can in image
            Image bgImage = _main.ConvertCanvasInImage(CanToMakeFree);


            //Change color for temp selection
            selection.SelectCan.Background = new SolidColorBrush(Colors.Transparent);

            //Init image in drawingCanvas
            Canvas.SetLeft(bgImage, selectionPoint.X);
            Canvas.SetTop(bgImage, selectionPoint.Y);

            //Get All objs in drawingCanvas + delete them
            List<UIElement> canObjects = ReAssignChildrenInAuxiliaryCanvas(DrawingCanvas);
            DrawingCanvas.Children.Clear();


            //Add image to DrawingCanvas
            DrawingCanvas.Children.Add(bgImage);

            Console.WriteLine(bgImage.Width + bgImage.Height);

            //return;
            //convert canvas  in image + set it as DrawingCanvas background
            Image newDrawingCanBg = _main.ConvertCanvasInImage(DrawingCanvas);
            DrawingCanvas.Background = new ImageBrush()
            {
                ImageSource = newDrawingCanBg.Source
            };

            DrawingCanvas.Children.Clear();
            InitElementsInCanvas(DrawingCanvas, canObjects);

            //Return elems in temp canvas
            InitElementsInCanvas(selection.CheckCan, elems);
            AddListElemsInCanvas(selectCanElems, selection.SelectCan);

            //selection.SelectCan.Background = new SolidColorBrush(Colors.White);
            //selection.Background = new SolidColorBrush(Colors.White);

            Selection childSelection = GetSelectionFromCanvas(selection.SelectCan);

            FreeAllSelectionsInInversionSelection(childSelection);
        }
        private Point CorrelateGotPoint(Point point, int correlationCounter)
        {
            const int correlationLoc = 3;
            return new Point(point.X + correlationLoc * correlationCounter,
                point.Y + correlationLoc * correlationCounter);
        }
        private (Point, int) GetSelectionPointComparedToDrawingCanvas(UIElement element,
            Point tempRes, int correlationCounter)
        {
            if (element == DrawingCanvas)
            {
                return (tempRes, correlationCounter);
            }
            else if ((element.GetType() == typeof(Selection) &&
                ((Selection)element).Parent == DrawingCanvas))
            {
                return (new Point(Canvas.GetLeft((Selection)element) + tempRes.X,
                    Canvas.GetTop((Selection)element) + tempRes.Y), correlationCounter);
            }
            //get selection 
            Selection sel = null;
            if (element.GetType() == typeof(Selection))
            {
                sel = GetSelectionsSelectionParent((Canvas)((Selection)element).Parent);
            }
            Selection selElem = element as Selection;

            Point tempSelPoint = new Point(Canvas.GetLeft(selElem), Canvas.GetTop(selElem));

            correlationCounter++;
            (tempRes, correlationCounter) = GetSelectionPointComparedToDrawingCanvas(sel, tempRes, correlationCounter);

            return (new Point(tempSelPoint.X + tempRes.X, tempSelPoint.Y + tempRes.Y), correlationCounter);
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
        private void AddSelectionBgInDrawingCanvasCHECK(Canvas canvas)
        {
            if (canvas == DrawingCanvas) return;

            Selection selParent = GetSelectionsSelectionParent(canvas);

            List<UIElement> elements = ReAssignChildrenInAuxiliaryCanvas(canvas);
            canvas.Children.Clear();

            Point selectionPoint = GetPointOfSelection(selParent);

            Image img = _main.ConvertCanvasInImage(canvas);
            Canvas.SetLeft(img, selectionPoint.X);
            Canvas.SetTop(img, selectionPoint.Y);
            DrawingCanvas.Children.Add(img);

            canvas.Background = new SolidColorBrush(Colors.Transparent);

            AddSelectionBgInDrawingCanvasCHECK((Canvas)selParent.Parent);

            SetInDrawingCanvasAsBackground();
            InitElementsInCanvas(canvas, elements);
        }
        private void SetInDrawingCanvasAsBackground()
        {
            Selection sel = null;

            for (int i = 0; i < DrawingCanvas.Children.Count; i++)
            {
                if (DrawingCanvas.Children[i].GetType() == typeof(Selection))
                {
                    sel = (Selection)DrawingCanvas.Children[i];
                }
            }
            DrawingCanvas.Children.Remove(sel);

            Image img = _main.ConvertCanvasInImage(DrawingCanvas);
            DrawingCanvas.Background = new ImageBrush()
            {
                ImageSource = img.Source
            };

            DrawingCanvas.Children.Clear();
            DrawingCanvas.Children.Add(sel);
        }

        private void CheckSelectionsParams(Canvas can)
        {
            for (int i = 0; i < can.Children.Count; i++)
            {
                if (can.Children[i].GetType() == typeof(Selection))
                {
                    Selection sel = (Selection)can.Children[i];
                    CheckSelectionsParams(sel.SelectCan);
                }
            }
        }
        private void CheckStartDeepSelections(int tempCounter, int endValue, Selection tempSelection)
        {
            const int addCounterParam = 1;
            if (tempCounter == endValue)
            {
                tempSelection.SelectCan.Background = new SolidColorBrush(Colors.Yellow);
                return;
            }
            for (int i = 0; i < tempSelection.SelectCan.Children.Count; i++)
            {
                if (tempSelection.SelectCan.Children[i].GetType() == typeof(Selection))
                {
                    Selection sel = (Selection)tempSelection.SelectCan.Children[i];
                    tempCounter += addCounterParam;
                    CheckStartDeepSelections(tempCounter, endValue, sel);
                }
            }
        }
        private int CountSelectionsInDeep(Canvas can, ref int res)
        {
            for (int i = 0; i < can.Children.Count; i++)
            {
                if (can.Children[i].GetType() == typeof(Selection))
                {
                    res += 1;
                    CountSelectionsInDeep(((Selection)can.Children[i]).SelectCan, ref res);
                }
            }
            return res;
        }
        private void SetSelectionCanvasesSize()
        {
            _selection.SelectCan.Width = _selection.Width;
            _selection.SelectCan.Height = _selection.Height;

            _selection.CheckCan.Width = _selection.Width;
            _selection.CheckCan.Height = _selection.Height;
        }
        private void MakeInvertSelection(Canvas canvas, bool ifMakeInvalidation = true, int check = 0)
        {
            for (int i = 0; i < canvas.Children.Count; i++)
            {
                if (canvas.Children[i].GetType() == typeof(Selection))
                {
                    _selection = (Selection)canvas.Children[i];
                    SetSelectionCanvasesSize();

                    if (!ifMakeInvalidation)
                    {
                        InvertSelection();
                        ChangeWhiteSelectionPartInSelection();
                        MakeInvertSelection(_selection.SelectCan, !ifMakeInvalidation, check);
                    }
                    else
                    {
                        ChangeWhiteSelectionPartInSelection();
                        MakeInvertSelection(_selection.SelectCan, !ifMakeInvalidation, check);
                    }
                }
            }
        }
        private void ChangeWhiteSelectionPartInSelection()
        {
            //get SystemSelections Children + clear them
            List<UIElement> elems = ReAssignChildrenInAuxiliaryCanvas(_selection.SelectCan);
            _selection.SelectCan.Children.Clear();

            //swap white with Transparent in SystemSelection
            Image selBgImg = _main.ConvertCanvasInImage(_selection.SelectCan);
            Image recImg = SwipeColorsInImage(selBgImg, _whiteColor, _transparentColor);

            //Re init BG
            _selection.SelectCan.Background = new ImageBrush()
            {
                ImageSource = recImg.Source
            };

            //Set SystemsSelection children back
            InitElementsInCanvas(_selection.SelectCan, elems);
        }
        private void InvertSelection()
        {
            const int correlationParam = 1;
            //Get the highest selection
            Selection highest = GetHighestSelection();

            //Get temp location SystemSelection
            Point localLoc = new Point(Canvas.GetLeft(_selection), Canvas.GetTop(_selection));

            //Get global location of SystemLocation
            (Point selectionPoint, int corelCounter) =
                GetSelectionPointComparedToDrawingCanvas(_selection, new Point(0, 0), 0);
            selectionPoint = CorrelateGotPoint(selectionPoint, corelCounter + correlationParam);

            //Get SystemSelection Children + delete them
            List<UIElement> systemSelChildren =
                ReAssignChildrenInAuxiliaryCanvas(_selection.SelectCan);
            _selection.SelectCan.Children.Clear();

            //Get SystemSelection parent Canvas
            Canvas parentCan = (Canvas)_selection.Parent;

            //Remove SystemSelection from parent canvas
            parentCan.Children.Remove(_selection);

            //Clear drawingCanvas children
            DrawingCanvas.Children.Clear();

            //Add SystemSelection to DrawingCanvas
            DrawingCanvas.Children.Add(_selection);

            // Init global loc to SystemSelection
            Canvas.SetLeft(_selection, selectionPoint.X);
            Canvas.SetTop(_selection, selectionPoint.Y);

            //Get image BG of SystemSelection + Make BG in SystemSelection.SelectCan Transparent
            Image systemSelBgImg = _main.ConvertCanvasInImage(_selection.SelectCan);

            //Add image in DrawingCanvas
            DrawingCanvas.Children.Add(systemSelBgImg);

            //Set location to image
            Canvas.SetLeft(systemSelBgImg, selectionPoint.X - 4);
            Canvas.SetTop(systemSelBgImg, selectionPoint.Y - 4);

            //Remove SystemSelection from DrawingCanvas
            DrawingCanvas.Children.Remove(_selection);

            //Convert DrawingCanvas in Image 
            Image drawingCavasIMG = _main.ConvertCanvasInImage(DrawingCanvas);

            //Clear DrawingCanvas Children 
            DrawingCanvas.Children.Clear();

            //Set Image as BG of DrawingCanvas
            DrawingCanvas.Background = new ImageBrush()
            {
                ImageSource = drawingCavasIMG.Source
            };

            //Add Children of SystemSelection to SystemSelection children
            InitElementsInCanvas(_selection.SelectCan, systemSelChildren);

            //Add SystemSelection to parent canvas
            parentCan.Children.Add(_selection);

            //Set SystemSelection Local location
            Canvas.SetLeft(_selection, localLoc.X);
            Canvas.SetTop(_selection, localLoc.Y);

            //Add highest to Drawing Canvas
            DrawingCanvas.Children.Add(highest);

            _selection.SelectCan.Background = new SolidColorBrush(Colors.Transparent);
            _selection.Background = new SolidColorBrush(Colors.Transparent);
        }
        private void SetHighestSelectionInSystemSelection()
        {
            for (int i = 0; i < DrawingCanvas.Children.Count; i++)
            {
                if (DrawingCanvas.Children[i].GetType() == typeof(Selection))
                {
                    _selection = ((Selection)DrawingCanvas.Children[i]);
                    return;
                }
            }
        }
        private Selection GetHighestSelection()
        {
            for (int i = 0; i < DrawingCanvas.Children.Count; i++)
            {
                if (DrawingCanvas.Children[i].GetType() == typeof(Selection))
                {
                    return ((Selection)DrawingCanvas.Children[i]);
                }
            }
            return null;
        }
        private void RemoveSelectionGridInDeep(Selection selection, int count)
        {
            const int countParam = 1;
            if (count > 0)
            {
                selection.RemoveSizingGrid();
            }
            for (int i = 0; i < selection.SelectCan.Children.Count; i++)
            {
                if (selection.SelectCan.Children[i].GetType() == typeof(Selection))
                {
                    count += countParam;
                    RemoveSelectionGridInDeep((Selection)selection.SelectCan.Children[i], count);
                }
            }
        }
        private Point selLineLoc = new Point();
        private void InitBGForSelectionFromDeepest(Selection selection, Selection highest, int count)
        {
            if (!(_selectionLine is null))
            {
                Point startPointLoc = GetPointOfSelection(_selectionLine);
                Point minPointParams = GetMinPointParamsInPolyline(_selectionLine);
                Point resCheck = new Point(startPointLoc.X + Math.Abs(minPointParams.X), startPointLoc.Y + Math.Abs(minPointParams.Y));

                if (selection.CheckCan.Children.Contains(_selectionLine))
                {
                    selLineLoc = resCheck;
                }
            }
            //Add selection to drawingCanvas
            //Remove from Parent
            //Remove all fromDrawing Canvas
            //Make selection 
            //Return everything back;

            if (selection is null) return;
            _selection = selection;

            bool ifContainsSelectionLine = _selection.CheckCan.Children.Contains(_selectionLine);

            List<UIElement> selElems = ReAssignChildrenInAuxiliaryCanvas(_selection.CheckCan);
            _selection.CheckCan.Children.Clear();
            _selection.DashedBorder.Visibility = Visibility.Hidden;

            //Check if selection contains selection Line
            //Console.WriteLine(_selectionLine.Parent);

            List<UIElement> selCanElems = ReAssignChildrenInAuxiliaryCanvas(_selection.SelectCan);
            _selection.SelectCan.Children.Clear();

            //Parent
            Canvas parent = (Canvas)selection.Parent;

            //Got point for selection
            Point oldSelPoint = new Point(Canvas.GetLeft(selection), Canvas.GetTop(selection));
            (Point globalPoint, int corelCounterSmth) =
             GetSelectionPointComparedToDrawingCanvas(selection, new Point(0, 0), 0);
            globalPoint = CorrelateGotPoint(globalPoint, corelCounterSmth);

            // Remove from parent + remove DrawingCanvas children
            parent.Children.Remove(selection);
            List<UIElement> drawCanElems = ReAssignChildrenInAuxiliaryCanvas(DrawingCanvas);
            DrawingCanvas.Children.Clear();



            //Set selection in new Canvas
            Canvas.SetLeft(_selection, globalPoint.X);
            Canvas.SetTop(_selection, globalPoint.Y);
            //DrawingCanvas.Children.Add(selection);

            //Set Things for Selection
            SetParamsForSelection(_selection, ifContainsSelectionLine, globalPoint);

            //make Selection;
            DrawingCanvas.UpdateLayout();
            MakeSelection(ifContainsSelectionLine);

            //If custom selection need to remove and set and correct white part image 

            Console.WriteLine(DrawingCanvas.Children);
            //GetEverything Back
            DrawingCanvas.Children.Clear();
            Canvas.SetLeft(_selection, oldSelPoint.X);
            Canvas.SetTop(_selection, oldSelPoint.Y);
            parent.Children.Add(_selection);

            AddListElemsInCanvas(drawCanElems, DrawingCanvas);
            AddListElemsInCanvas(selCanElems, _selection.SelectCan);
            AddListElemsInCanvas(selElems, _selection.CheckCan);
            _selection.DashedBorder.Visibility = Visibility.Visible;

            if (parent == DrawingCanvas) return;

            count++;
            InitBGForSelectionFromDeepest(GetSelectionsSelectionParent((Canvas)_selection.Parent), highest, count);
        }
        private void SetParamsForSelection(Selection selection, bool ifSelLineContains, Point globalPoint)
        {
            if (ifSelLineContains) //Custom
            {

                Point asd = GetPointOfSelection(_selectionLine);
                Point check = GetMinPointParamsInPolyline(_selectionLine);
                Point resCheck = new Point(asd.X + Math.Abs(check.X), asd.Y + Math.Abs(check.Y));

                Canvas.SetLeft(_selectionLine, globalPoint.X);
                Canvas.SetTop(_selectionLine, globalPoint.Y);

                Point selPoint = new Point(Canvas.GetLeft(_selection), Canvas.GetTop(_selection));
                _selectionLine.RenderTransform = null;
                DrawingCanvas.RenderTransform = null;



                /*                DrawingCanvas.Children.Remove(_selection);
                                DrawingCanvas.Children.Remove(_selectionLine);
                                _selection.SelectCan.Children.Remove(_selectionLine);
                                _selection.CheckCan.Children.Remove(_selectionLine);*/

                _selection = null;
                _selectionLine.RenderTransform = null;
                DrawingCanvas.Children.Add(_selectionLine);

                DrawingCanvas.UpdateLayout();
                return;
            }
            _selectionRect = new Rectangle()
            {
                Width = _selection.Width,
                Height = _selection.Height
            };
            Canvas.SetLeft(_selection, globalPoint.X);
            Canvas.SetTop(_selection, globalPoint.Y);

            Canvas.SetLeft(_selectionRect, globalPoint.X);
            Canvas.SetTop(_selectionRect, globalPoint.Y);
            DrawingCanvas.Children.Add(_selectionRect);

            _firstSelectionStart = globalPoint;
            _firstSelectionEnd = new Point(globalPoint.X +
                _selection.Width, globalPoint.Y + _selection.Height);
            //Rect

        }

        private Selection GetSelectionsSelectionParent(Canvas selParent)
        {
            Border res = (Border)selParent.Parent;
            Canvas resCan = (Canvas)res.Parent;
            Grid resGrid = (Grid)resCan.Parent;
            Selection check = (Selection)resGrid.Parent;
            return check;
        }
        private Selection GetTheDeepestSelection(Canvas canvas, ref Selection res)
        {
            for (int i = 0; i < canvas.Children.Count; i++)
            {
                if (canvas.Children[i] is Selection selection)
                {
                    res = selection;
                    GetTheDeepestSelection(((Selection)canvas.Children[i]).SelectCan, ref res);
                }
            }
            return res;
        }
        private void MakeEverySecondSelecting(Canvas canvas, bool ifFreeSelection = true)
        {
            for (int i = 0; i < canvas.Children.Count; i++)
            {
                if (canvas.Children[i].GetType() == typeof(Selection))
                {
                    Canvas can = ((Selection)canvas.Children[i]).SelectCan;
                    MakeEverySecondSelecting(can, !ifFreeSelection);

                    if (ifFreeSelection)
                    {
                        //make selection
                        MakeSelectionInversion(((Selection)canvas.Children[i]));
                    }
                    else
                    {
                        AddSelectionBGInDrawingCanvas(((Selection)canvas.Children[i]));
                    }
                }
            }
        }
        private void MakeSelectionInversion(Selection selection)
        {
            //Get Selection parent(Should br Canvas)
            Canvas tempSelParent = (Canvas)selection.Parent;

            //Clear DrawingCanvas children, get them in Value
            List<UIElement> drawingCanvasElems = ReAssignChildrenInAuxiliaryCanvas(DrawingCanvas);
            DrawingCanvas.Children.Clear();

            //Remove selection from parent
            tempSelParent.Children.Remove(selection);

            //Add temp selection in drawingCanvas
            DrawingCanvas.Children.Add(selection);

            //Remove children from temp selection, get them in value 
            List<UIElement> tempSelectionElems = ReAssignChildrenInAuxiliaryCanvas(selection.SelectCan);
            selection.SelectCan.Children.Clear();

            //Make selection with temp selection
            //InitRectSelectionParamsForInversionSelection(tempSelLoc, selection);
            Rectangle rectSelection = GetRectForRectSelection(selection);
            //MakeRectSelection(rectSelection, ref selection, tempSelParent);
            //Re initSelectionInWholeDrawingCanvas();
            //init every item in _selection
            //Delete them from drawing canvas
            InitSelectedBgInRectCanvas(rectSelection, selection, DrawingCanvas);

            //Add removed children in temp selection
            InitElementsInCanvas(selection.SelectCan, tempSelectionElems);

            //remove it from DrawingCanvas
            DrawingCanvas.Children.Clear();

            if (tempSelParent != DrawingCanvas)
            {
                //Return temp selection to his parent
                tempSelParent.Children.Add(selection);
            }

            //Add removed drawingCanvas children in it
            InitElementsInCanvas(DrawingCanvas, drawingCanvasElems);
        }
        private void SetLocationForSelection(Selection selection)
        {
            Point loc = GetPointOfSelection(selection);
            Canvas.SetLeft(selection, loc.X);
            Canvas.SetTop(selection, loc.Y);
        }
        private Rectangle GetRectForRectSelection(Selection selection)
        {
            Rectangle res = new Rectangle();

            Point startPoint = GetPointOfSelection(selection);

            Canvas.SetLeft(res, startPoint.X);
            Canvas.SetTop(res, startPoint.Y);

            res.Width = selection.Width;
            res.Height = selection.Height;

            return res;
        }
        private void AddSelectionBGInDrawingCanvas(Selection selection)
        {
            //Get selection bg in image format
            Image selectionBGImg = GetClearImageOfCanvasBG(selection.SelectCan);

            //make selection bg transparent
            selection.SelectCan.Background = new SolidColorBrush(Colors.Transparent);

            //get elems + Clear DrawingCanvas 
            List<UIElement> drawingCanElems = ReAssignChildrenInAuxiliaryCanvas(DrawingCanvas);
            DrawingCanvas.Children.Clear();

            //add bgImage in drawingCanvas as a child 
            DrawingCanvas.Children.Add(selectionBGImg); //do something with location
                                                        //Init location for selectionBGIMG

            Point point = GetPointOfSelection(selection.SelectCan);

            Canvas.SetLeft(selectionBGImg, point.X);
            Canvas.SetTop(selectionBGImg, point.Y);

            //convert drawing canvas bg to image 
            Image drawingCanvasBgImg = _main.ConvertCanvasInImage(DrawingCanvas);
            DrawingCanvas.Children.Clear();

            //set it as bg 
            DrawingCanvas.Background = new ImageBrush()
            {
                ImageSource = drawingCanvasBgImg.Source
            };

            //return drawingCanvas children
            InitElementsInCanvas(DrawingCanvas, drawingCanElems);
        }
        private Image GetClearImageOfCanvasBG(Canvas canvas)
        {
            //clear canvas children + init in list
            List<UIElement> canElems = ReAssignChildrenInAuxiliaryCanvas(canvas);
            canvas.Children.Clear();

            //make clear image
            Image selectionBGImg = _main.ConvertCanvasInImage(canvas);

            //return children
            InitElementsInCanvas(canvas, canElems);

            return selectionBGImg;
        }
        private Point GetPointOfSelection(UIElement element)
        {
            if (element == DrawingCanvas)
            {
                return new Point(0, 0);
            }
            DependencyObject parent = VisualTreeHelper.GetParent(element);
            if (parent is UIElement parentElement)
            {
                GeneralTransform transform = element.TransformToVisual(parentElement);
                Point currentPosition = transform.Transform(new Point(0, 0));

                Point parentPosition = GetPointOfSelection(parentElement);

                return new Point(currentPosition.X + parentPosition.X, currentPosition.Y + parentPosition.Y);
            }
            return new Point(0, 0);
        }
        private void InitElementsInCanvas(Canvas canvas, List<UIElement> elems)
        {
            foreach (UIElement el in elems)
            {
                canvas.Children.Add(el);
            }
        }
        LeftClickSelectionMenu _clickMenu;
        private void DrawingCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
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
        /* private void FigureSizingSelectionCorrelation()
         {
             Image img = _main.ConvertCanvasInImage(_selection.SelectCan);

             _selection = new Selection(ObjSize)
             {
                 Height = _selection.SelectCan.ActualHeight,
                 Width = _selection.SelectCan.ActualWidth,
             };
             _selection.SelectCan.Background = new ImageBrush()
             {
                 ImageSource = img.Source
             };

             Canvas.SetLeft(_selection, Canvas.GetLeft(_selection));
             Canvas.SetTop(_selection, Canvas.GetTop(_selection));

             DrawingCanvas.Children.Remove(_selection);
             DrawingCanvas.Children.Add(_selection);

             //_type = ActionType.Selection;
             _selection = null;
         }*/
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
        private (Image, Canvas) GetImageForSpinning()
        {
            if (_selection is null)
            {
                return (ConvertBackgroundToImage(DrawingCanvas), DrawingCanvas);
            }
            _selection.RemoveSizingGrid();
            (Image img, Canvas can) res = (ConvertBackgroundToImage(_selection.SelectCan), _selection.SelectCan);
            _selection.AddSizingGrid();
            return res;
        }

        private RotateTransform _rotate = new RotateTransform();
        private ScaleTransform _scale = null;

        private TransformGroup _transformGroup = new TransformGroup();

        private int _tempRotateAngle = 0;
        public void FlipImage(bool flipHorizontally)
        {
            _selection.SelectCan.Children.Remove(_selectionLine);
            if (flipHorizontally)
            {
                _scale = _transformGroup.Children.OfType<ScaleTransform>().
                    FirstOrDefault(t => t.ScaleX == -1);

                if (_scale == null)
                {
                    _scale = new ScaleTransform();
                    _scale.ScaleX = -1;
                    _transformGroup.Children.Add(_scale);
                }
                else
                {
                    _scale.ScaleX = 1;
                }
                _scale.CenterX = _selection.Width / 2;
                _scale.CenterY = _selection.Height / 2;
            }
            else
            {
                _scale = _transformGroup.Children.OfType<ScaleTransform>().
                    FirstOrDefault(t => t.ScaleY == -1);
                if (_scale is null)
                {
                    _scale = new ScaleTransform();
                    _scale.ScaleY = -1;
                    _transformGroup.Children.Add(_scale);
                }
                else
                {
                    _scale.ScaleY = 1;
                }
                _scale.CenterX = _selection.Width / 2;
                _scale.CenterY = _selection.Height / 2;
            }

            if (_selection._shape is null)
            {
                ApplyNoShapeTransformation();
                return;
            }
            _selection._shape.LayoutTransform = _transformGroup;
        }
        private const int _rotateStep = 180;
        private void RotateImage()
        {
            _selection.CheckCan.Children.Remove(_selectionLine);
            _rotate = _transformGroup.Children.OfType<RotateTransform>().
                   FirstOrDefault();

            if (_rotate is null)
            {
                _tempRotateAngle = _rotateStep;
                _rotate = new RotateTransform(_tempRotateAngle);
                _rotate.Angle = _tempRotateAngle;
                _transformGroup.Children.Add(_rotate);
            }
            else
            {
                _rotate.Angle = _rotate.Angle == _rotateStep ? 0 : _rotateStep;
            }
            _rotate.CenterX = _selection.SelectCan.Width / 2;
            _rotate.CenterY = _selection.SelectCan.Height / 2;

            if (_selection._shape is null)
            {
                //Make check bg img
                ApplyNoShapeTransformation();
                return;
            }

            _selection._shape.LayoutTransform = _transformGroup;
        }
        private void ApplyTransformsForSelectionLine()
        {
            if (_selectionLine is null) return;
            _selectionLine.LayoutTransform = _transformGroup;

            _selection.SelectCan.Children.Remove(_selectionLine);
            _selection.CheckCan.Children.Remove(_selectionLine);

            _selection.CheckCan.Children.Add(_selectionLine);
        }

        private Image img = null;
        ImageBrush selectedBg = null;
        private void ApplyNoShapeTransformation()
        {
            if (!(img is null)) _selection.SelectCan.Children.Remove(img);

            ImageSource canvasImageSource = _selection.SelectCan.Background is null ?
                selectedBg.ImageSource : _main.ConvertCanvasInImage(_selection.SelectCan).Source;

            ImageBrush brush = new ImageBrush()
            {
                ImageSource = canvasImageSource
            };

            img = new Image()
            {
                Source = brush.ImageSource,
                LayoutTransform = _transformGroup,
                Width = _selection.SelectCan.Width,
                Height = _selection.SelectCan.Height
            };

            //brush.RelativeTransform = _transformGroup;
            _selection.SelectCan.Children.Add(img);

            if (!(_selection.SelectCan.Background is null))
            {
                selectedBg = new ImageBrush()
                {
                    ImageSource = ConvertBackgroundToImage(_selection.SelectCan).Source
                };
                _selection.SelectCan.Background = null;
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
        private bool IfDrawingCanvasContainsRightClickMenus()
        {
            for (int i = 0; i < DrawingCanvas.Children.Count; i++)
            {
                if (DrawingCanvas.Children[i] is LeftClickSelectionMenu ||
                    DrawingCanvas.Children[i] is RightClickSubMenu)
                {
                    return true;
                }
            }
            return false;
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
            if (!(_selectionLine is null))
            {
                _copyPolyLine = new Polyline()
                {
                    Points = _selectionLine.Points,
                    Stretch = Stretch.Fill,
                    Stroke = _selectionLine.Stroke,
                    StrokeThickness = _selectionLine.StrokeThickness
                };
            }

            _selectionLine = null;
        }
        Image _copyBuffer = null;
        Polyline _copyPolyLine = null;
        private void Cut_Click(object sender, EventArgs e)
        {
            _selection.CheckCan.Children.Remove(_selectionLine);
            RemoveRightClickMenus();
            if (_selection is null) return;
            _copyBuffer = _main.ConvertCanvasInImage(_selection.SelectCan);
            _selection = null;

            SetPolyLineInBuffer();

            RemoveClicked();
            //RemoveSelection();

            MakeChangeButActive(PasteChange);
        }
        private void Copy_Click(object sender, EventArgs e)
        {
            _selection.CheckCan.Children.Remove(_selectionLine);
            RemoveRightClickMenus();
            if (_selection is null) return;
            _copyBuffer = _main.ConvertCanvasInImage(_selection.SelectCan);

            SetPolyLineInBuffer();

            MakeChangeButActive(PasteChange);
        }
        private void Paste_Click(object sender, EventArgs e)
        {
            RemoveRightClickMenus();
            if (_copyBuffer is null) return;
            if (!(_selection is null)) FreeSelection();

            InitNewBgForSelection(_copyBuffer);
            Canvas.SetLeft(_selection, 0);
            Canvas.SetTop(_selection, 0);

            DrawingCanvas.Children.Add(_selection);
        }
        private void InitNewBgForSelection(Image img)
        {
            _selection = new Selection(ObjSize)
            {
                Width = img.Width,
                Height = img.Height
            };
            _selection.SelectionBorder.Width = img.Width;
            _selection.SelectionBorder.Height = img.Height;

            _selection.SelectCan.Background = new ImageBrush()
            {
                ImageSource = img.Source
            };

            if (!(_copyPolyLine is null))
            {
                _selection.CheckCan.Children.Remove(_copyPolyLine);

                if (!(_copyPolyLine.Parent is null))
                {
                    var parent = _copyPolyLine.Parent;
                    if (parent is Canvas) ((Canvas)parent).Children.Remove(_copyPolyLine);
                }
                _selection.CheckCan.Children.Add(_copyPolyLine);
            }
        }
        private void MakeChangeButActive(MenuItem changeItem)
        {
            const int opacity = 1;
            changeItem.Opacity = opacity;
            changeItem.IsEnabled = true;
        }
        private void MakeChangeButActive(Button but)
        {
            const int opacity = 1;
            but.Opacity = opacity;
            but.IsEnabled = true;
        }
        private void MakeChangeButInActive(MenuItem changeItem)
        {
            const double opacity = 0.5;
            changeItem.Opacity = opacity;
            changeItem.IsEnabled = false;
        }
        private void MakeChangeButInActive(Button but)
        {
            const double opacity = 0.5;
            but.Opacity = opacity;
            but.IsEnabled = false;
        }
        private void NotWorkingSend_Click(object sender, RoutedEventArgs e)
        {
            DoesntWork ifWork = new DoesntWork();
            ifWork.ShowDialog();
        }
        private double _horizontalOffset;
        private void DrawingAdapt_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ResetRenderTransform();
            _horizontalOffset = -e.HorizontalOffset;
            UpdateRenderTransform();
        }
        private void PaintWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space) //Relese mouse capture from selection
            {
                if (!(_selection is null) && !_selection.CheckCan.IsMouseCaptured)
                {
                    if (_main._type == ActionType.Selection)FreeSelection();
                    ClearAfterFastToolButPressed();
                    SaveInHistory();

                }
                if (!(_selection is null)) _selection._isDraggingSelection = false;
                else if (!(_lineSizing is null)) _lineSizing._isDraggingSelection = false;
                RemoveClicked();
            }
            if (e.Key == Key.Escape)
            {
                EscapePressedAction();
            }
        }
        private void EscapePressedAction()
        {
            if (!(_selection is null) && _selection._shape is null)
            {
                SetEscapeBgSelection();
            }
            RemoveClicked();
        }
        private void SetEscapeBgSelection()
        {
            if (currentIndex == 0)
            {
                DrawingCanvas.Background = new SolidColorBrush(_whiteColor);
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
        private void DisableTabsHandler(KeyEventArgs e)
        {
            if (e.Key == Key.Tab || e.Key == Key.Up || e.Key == Key.Down ||
                e.Key == Key.Left || e.Key == Key.Right)
            {
                e.Handled = true;
            }
        }
        private void SelectionArrowMove(KeyEventArgs e)
        {
            UIElement elem = null;

            if (_lineSizing is null)
            {
                elem = _selection;
            }
            else elem = _lineSizing;

            if (elem is null || !DrawingCanvas.Children.Contains(elem)) return;
            if (Keyboard.IsKeyDown(Key.Left))
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
            }
        }
        private void MoveSelectionByArrow(SelectionMoveByKeyDir dir, UIElement elem)
        {

            const int stepInMove = 6;
            if (dir == SelectionMoveByKeyDir.Up)
            {
                Canvas.SetTop(elem, Canvas.GetTop(elem) - stepInMove);
            }
            else if (dir == SelectionMoveByKeyDir.Down)
            {
                Canvas.SetTop(elem, Canvas.GetTop(elem) + stepInMove);
            }
            else if (dir == SelectionMoveByKeyDir.Left)
            {
                Canvas.SetLeft(elem, Canvas.GetLeft(elem) - stepInMove);
            }
            else if (dir == SelectionMoveByKeyDir.Right)
            {
                Canvas.SetLeft(elem, Canvas.GetLeft(elem) + stepInMove);
            }
            UpdateBoundaries();
        }
        private void UpdateBoundaries()
        {
            if (_lineSizing is null)
            {
                _selection.ClipOutOfBoundariesGeo();
                return;
            }
            _lineSizing.ClipOutOfBoundariesGeo();
        }
        private void HotKeysClickTools(KeyEventArgs e)
        {
            if (e.Key == Key.P) //Pencil
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
                SetSelectionFastChange();
            }
            else if (e.Key == Key.B)//Bucket
            {
                PressedToolButton(ToolTypes.Bucket);
            }
        }
        public void PressedToolButton(ToolTypes type)
        {
            sprayTimer.Stop();
            switch (type)
            {
                case (ToolTypes.Pencil):
                    {
                        SetOptionsForFastToolClick(Pen, _main._pencilCurs);
                        return;
                    }
                case (ToolTypes.Bucket):
                    {
                        SetOptionsForFastToolClick(Bucket, _main._bucketCurs);
                        return;
                    }
                case (ToolTypes.Text):
                    {
                        SetOptionsForFastToolClick(Text, _main._textingCurs);
                        return;
                    }
                case (ToolTypes.Razer):
                    {
                        SetOptionsForFastToolClick(Erazer, _main._crossCurs);
                        AddErasingMarker(currentPoint);
                        DrawingCanvas.ClipToBounds = true;
                        return;
                    }
                case (ToolTypes.Pipette):
                    {
                        SetOptionsForFastToolClick(ColorDrop, _main._pipetteCurs);
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
            if (_main._type == ActionType.Selection) FreeSelection();
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

            //Change selection pic
        }
        public void SetOptionsForFastToolClick(Button but, Cursor newCurs)
        {
            if (_chosenTool == but) return;
            RemoveRightClickMenus();

            if (_main._type == ActionType.Selection) FreeSelection();
            ClearAfterFastToolButPressed();
            _main.MakeAllActionsNegative();

            ClearDynamicValues(brushClicked: but == Pen || but == Erazer || but == Text);
            _chosenTool = but;

            _main.SetActionTypeByButtonPressed(but);
            _main._tempCursor = newCurs;
            Cursor = _main._tempCursor;

            //IfCurosrInDrawField();
        }
        private bool IfCurosrInDrawField()
        {
            GeneralTransform transform = DrawingCanvas.TransformToAncestor(this);

            Point topLeft = transform.Transform(new Point(0, 0));
            return true;
        }
        private void ClearAfterFastToolButPressed()
        {
            RemoveObject(_main._figToPaint);
            RemoveObject(_main._polyline);
            RemoveObject(_selectionRect);
            RemoveObject(_selectionLine);
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
            _selection = null;

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
            RemoveSelection();
            _selection = null;

            DrawingCanvas.Children.Remove(_main._polyline);
            _main._polyline = null;

            DrawingCanvas.Children.Remove(_lineSizing);
            _lineSizing = null;

            DrawingCanvas.Children.Remove(_selectionRect);
            _selectionRect = null;

            DrawingCanvas.Children.Remove(_changedSizeText);
            _changedSizeText = null;
            _ifDoubleClicked = false;

            ReloadCurvePainting();
        }

        private void PaintWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(RazerMarker.Parent is null) && RazerMarker.Parent is Canvas)
            {
                ((Canvas)RazerMarker.Parent).Children.Remove(RazerMarker);
            }
        }
        private void PaintWindow_MouseLeave(object sender, MouseEventArgs e)
        {
            ResetRenderTransform();

            //if (_main._ifSelection && !_main.IfSelectionIsMacken) MakeSelection();
            if (_main._ifSelection)
            {
                ChangeSelectionSize(e);
            }
            CursorCheck();
            UpdateRenderTransform();
        }
        private void CursorCheck()
        {
            if (_selection is null && _changedSizeText is null) return;

            if (!(_selection is null)) _selection._tempCursor = null;
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
            //_main._type = ActionType.Nothing;
        }

        private void MainPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (!(_selection is null))
            {
                _selection.SelectionBorder_MouseMove(_selection.CheckCan, e);
                //Cursor = _main._bucketCurs;

                if (_selection._isDraggingSelection)
                {
                    _selection._tempCursor = _selection._moveCurs;
                }
                else
                {
                    _selection._tempCursor = null;
                }
            }
        }


    }
}

