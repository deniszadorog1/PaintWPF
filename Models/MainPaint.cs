using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PaintWPF.CustomControls;
using PaintWPF.Models.Enums;

namespace PaintWPF.Models
{
    public class MainPaint
    {
        public List<Image> ImagesHistory { get; set; }
        public PalleteModel PalleteMod { get; set; }
        public SolidColorBrush ColorToPaint { get; set; }
        public SolidColorBrush FirstColor { get; set; }
        public SolidColorBrush SecondColor { get; set; }
        public BrushType _tempBrushType { get; set; }

        public List<Canvas> CanvasStates = new List<Canvas>();

        public Color _almostWhite = Color.FromRgb(253, 253, 253);

        public ActionType _type = ActionType.Nothing;
        public Cursor _tempCursor;

        public readonly Cursor _crossCurs = new Cursor(
       Application.GetResourceStream(new Uri(
       "Models/Cursors/Cross.cur", UriKind.Relative)).Stream);

        public readonly Cursor _pencilCurs = new Cursor(
        Application.GetResourceStream(new Uri(
        "Models/Cursors/Pencil.cur", UriKind.Relative)).Stream);

        public readonly Cursor _pipetteCurs = new Cursor(
        Application.GetResourceStream(new Uri(
        "Models/Cursors/Pipette.cur", UriKind.Relative)).Stream);

        public readonly Cursor _sprayCurs = new Cursor(
        Application.GetResourceStream(new Uri(
        "Models/Cursors/Spray.cur", UriKind.Relative)).Stream);

        public readonly Cursor _textingCurs = new Cursor(
        Application.GetResourceStream(new Uri(
        "Models/Cursors/Texting.cur", UriKind.Relative)).Stream);

        public readonly Cursor _bucketCurs = new Cursor(
        Application.GetResourceStream(new Uri(
        "Models/Cursors/Bucket.cur", UriKind.Relative)).Stream);

        public bool _ifDrawing = false;
        public bool _ifFilling = false;
        public bool _ifErasing = false;
        public bool _ifFiguring = false;
        public bool _ifSelection = false;
        public bool _ifTexting = false;
        public bool _ifPickingColor = false;
        public bool _ifChangingFigureSize = false;
        public bool IfSelectionIsMaken = false;


        public bool _spaceDrawingPressed = false;

        public SelectionType _selectionType = SelectionType.Nothing;

        public Point _startPoint;
        public Point _endPoint;

        public Shape _figToPaint;
        public bool _ifCurveIsDone = false;

        public const string roundRectData = "M 20 10 L 160 10 A 10 10 0 0 1 170 20 " +
                      "L 170 140 A 10 10 0 0 1 160 150 " +
                      "L 20 150 A 10 10 0 0 1 10 140 " +
                      "L 10, 20 A 10 10 0 0 1 20 10";
        public const string triangleData = "M 10 150 L 170 150 L 90 10 Z";
        public const string rightTriangleData = "M 10 150 L 170 150 L 10 10 Z";
        public const string rhombusData = "M 90 10 L 170 80 L 90 150 L 10 80 Z";
        public const string pentagonData = "M 100 10 L 170 60 " +
                        "L 140 150 L 60 150 L 30 60 Z";
        public const string hexagonData = "M 100 10 L 150 40 " +
                        "L 150 100 L 100 130 L 50 100 L 50 40 Z";
        public const string rightArrowData = "M 10 50 L 100 50 L 100 30 " +
                        "L 150 70 L 100 110 L 100 90 L 10 90 Z";
        public const string leftArrowData = "M 150 50 L 50 50 L 50 30 " +
                        "L 0 70 L 50 110 L 50 90 L 150 90 Z";
        public const string upArrowData = "M 50 150 L 50 50 L 30 50 " +
                        "L 70 0 L 110 50 L 90 50 L 90 150 Z";
        public const string downArrowData = "M 50 50 L 50 150 L 30 150 " +
                        "L 70 200 L 110 150 L 90 150 L 90 50 Z";

        public const string fourPointedStar = "M 90 20 L 105 70 L 165 85 L 105 95    " +
                        "L 90 150 L 75 95 L 20 85 L 75 70 L 90 20";

        public const string fivePointStar = "M 100 10  L 130 90 L 200 90 L 145 130 " +
                        "L 170 200 L 100 160 L 30 200  L 55 130 L 0 90  L 70 90 Z";
        public const string sixPointStarData = "M 100 10 L 130 60 L 180 60 L 150 100 " +
                        "L 180 140 L 130 140 L 100 180 L 70 140 L 20 140 " +
                        "L 50 100 L 20 60 L 70 60 Z";
        public const string roundedCommentData = "M 120 10 L 150 10 Q 170, 10, 170 40 L 170 100 " +
                        "Q 170 120, 150 120 L 75 120 L 65 140 L 55 120 Q 25, 120, 25 100 " +
                        "L 25 40 Q 25 10, 40 10 Z";
        public const string ovalCommentData = "M 100,100 A 40, 30 0 1 1 200, 100 " +
                        "A 55, 35 1 0 1 130, 130 L 125 145 L 120 130 Q 100 120 100 100";
        public const string cloudCommentData = "M 80,50 A 20 10 1 1 1 150 45 A 20 10 1 1 1 210 40 " +
                        "A 10 5 1 1 1 270 70 A 10 10 1 1 1 270 120 A 10 10 1 1 1 240 155 " +
                        "A 15 12 1 1 1 180 165 A 18 10 1 1 1 90 170 A 45 35 1 0 1 30 130 " +
                        "A 15 20 1 0 1 25 90 A 20 20 0 0 1 80 50 " +
                        "M 30 180 A 20 10 0 0 1 70 180 A 20 10 0 0 1 30 180 " +
                        "M 20 200 A 20 10 0 0 1 60 200 A 20 10 0 0 1 20 200 " +
                        "M 15 215 A 20 20 0 0 1 45 215 A 20 20 0 0 1 15 215";
        public const string heartData = "M 50 100 C 50 0 150 0 150 100 " +
                        "C 150 200 50 250 50 350 C 50, 250 -50 200 -50 100 " +
                        "C -50 0 50 0 50 100 Z";
        public const string lightningData = "M 80,50 L 120 40 L 150 80 L 140 85 " +
                        "L 175 120 L 165 125 L 200 170 L 140 140 " +
                        "L 150 135 L 105 90 L 120 85 Z";

        public FigureTypes? _figType = null;


        public Polyline _polyline;
        public PathFigure _pathFigure;

        public BezierSegment _bezierSegment;
        public List<Point> _bezPoints;


        public bool _isDrawingLine;
        public bool _isAdjustingCurve;
        public int _amountOfPointInPolygon = 0;

        public List<Polyline> polylines = new List<Polyline>();
        public int brushThickness = 2;

        public string _oilBrushPath;
        public string _coloredBrushPath;
        public string _texturePencilBrushPath;
        public string _watercolorBrushPath;

        public string _checkPath;
        public string _checkOne;
        public string _checkTwo;

        public Point previousPoint;

        public Selection _selection = null;
        public Rectangle _selectionRect = new Rectangle();
        public Polyline _selectionLine = null;

        public Point _firstSelectionStart;
        public Point _firstSelectionEnd;

        public double _horizontalOffset;
        public bool _ifCutCheck = false;

        public bool _ifTransparencyIsActivated;
        public Color _whiteColor = Color.FromArgb(255, 255, 255, 255);
        public Color _transparentColor = Color.FromArgb(0, 0, 0, 0);

        public Image _comparator;
        public Polyline _copyPolyLine = null;

        const int pixStep = 4;
        const int getGStep = 1;
        const int getRStep = 2;
        const int getAlphaStep = 3;
        const int bitsAdd = 7;
        const int bitsInByte = 8;

        public MainPaint()
        {
            ColorToPaint = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            ImagesHistory = new List<Image>();
            PalleteMod = new PalleteModel();
            FirstColor = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            SecondColor = new SolidColorBrush(Color.FromRgb(253, 253, 253));
            CanvasStates = new List<Canvas>();
            _tempBrushType = BrushType.UsualBrush;

            InitBrushFilePaths();
        }

        public void InitBrushFilePaths()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            string startDir = dirInfo.Parent.Parent.FullName;
            string imgPath = System.IO.Path.Combine(startDir, "Images");
            string brushPath = System.IO.Path.Combine(imgPath, "Brushes");

            _oilBrushPath = System.IO.Path.Combine(brushPath, "OilBrushPaint.png");
            _coloredBrushPath = System.IO.Path.Combine(brushPath, "TexturePencilBrush.png");
            _texturePencilBrushPath = System.IO.Path.Combine(brushPath, "TexturePencilBrush.png");
            _watercolorBrushPath = System.IO.Path.Combine(brushPath, "WatercolorBrush.png");
            _checkPath = System.IO.Path.Combine(brushPath, "Check.png");
            _checkOne = System.IO.Path.Combine(brushPath, "CheckOne.png");
            _checkTwo = System.IO.Path.Combine(brushPath, "CheckTwo.png");
        }


        public void MakeAllActionsNegative()
        {
            _ifSelection = false;
            _ifDrawing = false;
            _ifErasing = false;
            _ifFiguring = false;
            _ifFilling = false;
            _ifTexting = false;
            _ifPickingColor = false;
            _ifChangingFigureSize = false;
            IfSelectionIsMaken = false;
        }

        public void InitDeed()
        {
            MakeAllActionsNegative();

            switch (_type)
            {
                case ActionType.Drawing:
                    {
                        _ifDrawing = true;
                        break;
                    }
                case ActionType.Figuring:
                    {
                        _ifFiguring = true;
                        break;
                    }
                case ActionType.Erazing:
                    {
                        _ifErasing = true;
                        break;
                    }
                case ActionType.Filling:
                    {
                        _ifFilling = true;
                        break;
                    }
                case ActionType.Selection:
                    {
                        _ifSelection = true;
                        IfSelectionIsMaken = false;
                        break;
                    }
                case ActionType.Text:
                    {
                        _ifTexting = true;
                        break;
                    }
                case ActionType.PickingColor:
                    {
                        _ifPickingColor = true;
                        break;
                    }
                case ActionType.ChangingFigureSize:
                    {
                        _ifChangingFigureSize = true;
                        break;
                    }
                default:
                    {
                        _ifDrawing = true;
                        break;
                    }
            }
            /* if (_type == ActionType.Drawing)
             {
                 _ifDrawing = true;
             }
             else if (_type == ActionType.Figuring)
             {
                 _ifFiguring = true;
             }
             else if (_type == ActionType.Erazing)
             {
                 _ifErasing = true;
             }
             else if (_type == ActionType.Filling)
             {
                 _ifFilling = true;
             }
             else if (_type == ActionType.Selection)
             {
                 _ifSelection = true;
                 IfSelectionIsMaken = false;
             }
             else if (_type == ActionType.Text)
             {
                 _ifTexting = true;
             }
             else if (_type == ActionType.PickingColor)
             {
                 _ifPickingColor = true;
             }
             else if (_type == ActionType.ChangingFigureSize)
             {
                 _ifChangingFigureSize = true;
             }
             else
             {
                 _ifDrawing = true;
             }*/
        }

        public void GetPathToPaint()
        {
            const int pathThickness = 3;
            const int transformParam = 1;
            _figToPaint = new System.Windows.Shapes.Path()
            {
                Stroke = Brushes.Black,
                StrokeThickness = pathThickness,
                RenderTransform = new ScaleTransform(transformParam, transformParam),
                Stretch = Stretch.Fill
            };
        }

        public void InitShapesToPaint(MouseEventArgs e, Canvas DrawingCanvas)
        {
            switch (_figType)
            {
                case FigureTypes.Line:
                    {
                        _figToPaint = new Polyline();
                        break;
                    }
                case FigureTypes.Curve:
                    {
                        const int lineThickness = 2;
                        const int endLinePoint = 1;

                        if (!_isDrawingLine && !_isAdjustingCurve)
                        {
                            _startPoint = e.GetPosition(DrawingCanvas);
                            _polyline = new Polyline
                            {
                                Stroke = ColorToPaint,
                                StrokeThickness = lineThickness,
                                Points = new PointCollection { _startPoint }
                            };

                            DrawingCanvas.Children.Add(_polyline);
                            _isDrawingLine = true;
                        }
                        else if (_isDrawingLine && _polyline.Points.Count > endLinePoint)
                        {
                            _endPoint = _polyline.Points[endLinePoint];
                            _polyline.Points.Add(_endPoint);
                            DrawingCanvas.Children.Remove(_polyline);

                            _pathFigure = new PathFigure
                            {
                                StartPoint = _startPoint
                            };
                            _bezierSegment = new BezierSegment
                            {
                                Point1 = _startPoint,
                                Point2 = _endPoint,
                                Point3 = _endPoint,
                            };
                            _pathFigure.Segments.Add(_bezierSegment);

                            PathGeometry pathGeometry = new PathGeometry();
                            pathGeometry.Figures.Add(_pathFigure);

                            _figToPaint = new System.Windows.Shapes.Path
                            {
                                Stroke = _polyline.Stroke,
                                StrokeThickness = lineThickness,
                                Data = pathGeometry
                            };

                            DrawingCanvas.Children.Add(_figToPaint);
                            _isDrawingLine = false;
                            _isAdjustingCurve = true;

                            _ifCurveIsDone = true;

                            _bezPoints = new List<Point>();
                        }
                    }
                    return;
                case FigureTypes.Oval:
                    {
                        _figToPaint = new Ellipse();
                        break;
                    }
                case FigureTypes.Rectangle:
                    {
                        _figToPaint = new Rectangle();
                        break;
                    }
                case FigureTypes.RoundedRectangle:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse(roundRectData);
                        break;
                    }
                case FigureTypes.Polygon:
                    {
                        if (!(_figToPaint is null)) return;

                        _figToPaint = new Polyline();
                        _amountOfPointInPolygon = 0;
                        break;
                    }
                case FigureTypes.Triangle:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse(triangleData);
                        break;
                    }
                case FigureTypes.RightTriangle:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse(rightTriangleData);
                        break;
                    }
                case FigureTypes.Rhombus:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse(rhombusData);
                        break;
                    }
                case FigureTypes.Pentagon:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse(pentagonData);
                        break;
                    }
                case FigureTypes.Hexagon:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse(hexagonData);
                        break;
                    }
                case FigureTypes.RightArrow:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse(rightArrowData);
                        break;
                    }
                case FigureTypes.LeftArrow:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse(leftArrowData);
                        break;
                    }
                case FigureTypes.UpArrow:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse(upArrowData);
                        break;
                    }
                case FigureTypes.DownArrow:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse(downArrowData);
                        break;
                    }
                case FigureTypes.FourPointedStar:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse(fourPointedStar);
                        break;
                    }
                case FigureTypes.FivePointedStar:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse(fivePointStar);
                        break;
                    }
                case FigureTypes.SixPointedStar:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse(sixPointStarData);
                        break;
                    }
                case FigureTypes.RoundedRectangularLeader:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse(roundedCommentData);
                        break;
                    }
                case FigureTypes.OvalCallout:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse(ovalCommentData);
                        break;
                    }
                case FigureTypes.CalloutCloud:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse(cloudCommentData);
                        break;
                    }
                case FigureTypes.Heart:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse(heartData);
                        break;
                    }
                case FigureTypes.Lightning:
                    {
                        GetPathToPaint();
                        ((System.Windows.Shapes.Path)_figToPaint).Data =
                        Geometry.Parse(lightningData);
                        break;
                    }
                default:
                    {
                        MessageBox.Show("How did you get here?");
                        break;
                    }
            }

            RenderOptions.SetEdgeMode(_figToPaint, EdgeMode.Aliased);

            const int figureThickness = 3;
            _figToPaint.Stroke = ColorToPaint;
            _figToPaint.StrokeThickness = figureThickness;
            _figToPaint.Fill = Brushes.Transparent;

            Canvas.SetTop(_figToPaint, e.GetPosition(DrawingCanvas).Y);
            Canvas.SetLeft(_figToPaint, e.GetPosition(DrawingCanvas).X);

            if (_figType != FigureTypes.Line &&
                _figType != FigureTypes.Polygon)
            {
                _figToPaint.Height = 0;
                _figToPaint.Width = 0;
            }
            DrawingCanvas.Children.Add(_figToPaint);
        }

        public const int _brushSizeCorrel = 1;
        public void SetPaintingMarker(MouseEventArgs e, Canvas DrawingCanvas)
        {
            const double markerOpacity = 0.5;
            const int maxLinePoints = 50;
            var polyline = new Polyline()
            {
                StrokeStartLineCap = PenLineCap.Flat,
                StrokeEndLineCap = PenLineCap.Flat,
                StrokeLineJoin = PenLineJoin.Round,
                Stroke = _spaceDrawingPressed ? FirstColor : ColorToPaint,
                StrokeThickness = brushThickness + _brushSizeCorrel,
                Opacity = markerOpacity
            };

            RectangleGeometry clip = new RectangleGeometry(new Rect(0, 0, DrawingCanvas.Width, DrawingCanvas.Height));
            polyline.ClipToBounds = true;
            polyline.Clip = clip;

            if (polylines.Count > maxLinePoints)
            {
                polylines.Clear();
            }

            polylines.Add(polyline);
            DrawingCanvas.Children.Add(polyline);
        }

        public bool CheckMethod(Point point, Polyline poly)
        {
            const int formulaPower = 2;
            const int decCorrel = 30;
            for (int i = 0; i < poly.Points.Count - (brushThickness + _brushSizeCorrel); i++)
            {
                if (Math.Sqrt(Math.Pow(point.X - poly.Points[i].X, formulaPower) +
                   Math.Pow(point.Y - poly.Points[i].Y, formulaPower)) + decCorrel <= (brushThickness + _brushSizeCorrel))
                {
                    return true;
                }
            }
            return false;
        }

        public void MarkerBrushPaint(MouseEventArgs e, Canvas drawingCanvas)
        {
            if (polylines.Count == 0) return;

            var point = e.GetPosition(drawingCanvas);

            if (polylines.Last().Points.Count > 0 &&
                polylines.Last().Points.Last() == point)
            {
                return;
            }

            if (CheckMethod(point, polylines.Last()) || polylines.Last().Points.Count > drawingCanvas.Height)
            {
                SetPaintingMarker(e, drawingCanvas);
            }
            else
            {
                polylines.Last().Points.Add(e.GetPosition(drawingCanvas));
            }
        }

        private const int _dpiParam = 96;
        public RenderTargetBitmap ConvertCanvasToBitmap(Canvas canvas)
        {
            canvas.RenderTransform = new TranslateTransform(0, 0);
            RenderOptions.SetEdgeMode(canvas, EdgeMode.Aliased);
            RenderOptions.SetBitmapScalingMode(canvas, BitmapScalingMode.NearestNeighbor);

            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                (int)canvas.Width, (int)canvas.Height,
                _dpiParam, _dpiParam, PixelFormats.Pbgra32);

            canvas.Measure(new Size((int)canvas.Width, (int)canvas.Height));
            canvas.Arrange(new Rect(new Size((int)canvas.Width, (int)canvas.Height)));

            RenderOptions.SetEdgeMode(renderBitmap, EdgeMode.Aliased);
            renderBitmap.Render(canvas);

            return renderBitmap;
        }

        public Image ConvertCanvasInImage(Canvas canvas)
        {
            const int minCanvasSize = 1;
            if (canvas.Width <= minCanvasSize ||
                canvas.Height <= minCanvasSize) return new Image();

            //RenderOptions.SetEdgeMode(canvas, EdgeMode.Aliased);
            //RenderOptions.SetBitmapScalingMode(canvas, BitmapScalingMode.NearestNeighbor);

            RenderTargetBitmap renderBitmap = ConvertCanvasToBitmap(canvas);

            Image image = new Image
            {
                Source = renderBitmap,
                Width = canvas.Width,
                Height = canvas.Height,
            };

            //image.RenderTransform = new TranslateTransform(0, 0);
            RenderOptions.SetEdgeMode(image, EdgeMode.Aliased);
            return image;
        }

        public RenderTargetBitmap ConvertSelectionToBitmap(Selection selection)
        {
            selection.RenderTransform = new TranslateTransform(0, 0);
            RenderOptions.SetEdgeMode(selection, EdgeMode.Aliased);
            RenderOptions.SetBitmapScalingMode(selection, BitmapScalingMode.NearestNeighbor);

            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                (int)selection.Width, (int)selection.Height,
                _dpiParam, _dpiParam, PixelFormats.Pbgra32);

            selection.Measure(new Size((int)selection.Width, (int)selection.Height));
            selection.Arrange(new Rect(new Size((int)selection.Width, (int)selection.Height)));

            RenderOptions.SetEdgeMode(renderBitmap, EdgeMode.Aliased);
            renderBitmap.Render(selection);

            return renderBitmap;
        }

        public WriteableBitmap GetChangedBitMapColor(string brushPngPath)
        {
            /*            const int bitsInByte = 8;
                        const int pixelStep = 4;
                        const int gStep = 1;
                        const int rStep = 2;*/
            BitmapImage bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.UriSource = new Uri(brushPngPath, UriKind.RelativeOrAbsolute);
            bitmap.EndInit();

            WriteableBitmap writeableBitmap = new WriteableBitmap(bitmap);

            int width = writeableBitmap.PixelWidth;
            int height = writeableBitmap.PixelHeight;
            int stride = width * (writeableBitmap.Format.BitsPerPixel / bitsInByte);
            byte[] pixelData = new byte[height * stride];

            writeableBitmap.CopyPixels(pixelData, stride, 0);

            SolidColorBrush brush = _spaceDrawingPressed ? FirstColor : ColorToPaint;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * stride + x * pixStep;
                    pixelData[index] = brush.Color.B;
                    pixelData[index + getGStep] = brush.Color.G;
                    pixelData[index + getRStep] = brush.Color.R;
                }
            }
            writeableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixelData, stride, 0);
            return writeableBitmap;
        }
        public Image ConvertSelectionInImage(Selection selection)
        {
            const int minCanvasSize = 1;
            if (selection.Width <= minCanvasSize ||
                selection.Height <= minCanvasSize) return new Image();

            //RenderOptions.SetEdgeMode(canvas, EdgeMode.Aliased);
            //RenderOptions.SetBitmapScalingMode(canvas, BitmapScalingMode.NearestNeighbor);

            RenderTargetBitmap renderBitmap = ConvertSelectionToBitmap(selection);

            Image image = new Image
            {
                Source = renderBitmap,
                Width = selection.Width,
                Height = selection.Height,
            };

            //image.RenderTransform = new TranslateTransform(0, 0);
            RenderOptions.SetEdgeMode(image, EdgeMode.Aliased);

            return image;
        }

        public ImageBrush _paintBrush;
        private void InitBrushPolyline(string brush)
        {
            _paintBrush = new ImageBrush()
            {
                ImageSource = GetChangedBitMapColor(brush)
            };
        }

        public void SetMarkers(MouseEventArgs e, Canvas DrawingCanvas)
        {
            if (_tempBrushType == BrushType.Marker)
            {
                SetPaintingMarker(e, DrawingCanvas);
            }
            else if (_tempBrushType == BrushType.OilPaintBrush ||
                _tempBrushType == BrushType.TexturePencil)
            {
                InitBrushPolyline(_checkPath);
            }
            else if (_tempBrushType == BrushType.ColorPencil)
            {
                InitBrushPolyline(_checkTwo);
            }
            else if (_tempBrushType == BrushType.WatercolorBrush)
            {
                InitBrushPolyline(_checkOne);
            }
            /*            else if (_tempBrushType == BrushType.TexturePencil)
                        {
                            InitBrushPolyline(_checkPath);
                        }*/
        }

        public RenderTargetBitmap _renderBitmap;
        //private const int bitsInPixel = 8;
        public void PerformFloodFill(int x, int y, Canvas drawingCanvas)
        {
            const int locCorrector = 1;
            if (x < 0 || x >= _renderBitmap.PixelWidth || y < 0 || y >= _renderBitmap.PixelHeight)
                return;

            int stride = _renderBitmap.PixelWidth * (_renderBitmap.Format.BitsPerPixel / bitsInByte);
            byte[] pixels = new byte[stride * _renderBitmap.PixelHeight];

            _renderBitmap.CopyPixels(pixels, stride, 0);

            Color targetColor = GetPixelColor(pixels, stride, x, y);
            if (targetColor == ColorToPaint.Color) return;

            Queue<Point> points = new Queue<Point>();
            points.Enqueue(new Point(x, y));

            while (points.Count > 0)
            {
                Point pt = points.Dequeue();
                int px = (int)pt.X;
                int py = (int)pt.Y;

                if (px < 0 || px >= _renderBitmap.PixelWidth || py < 0 || py >= _renderBitmap.PixelHeight)
                    continue;
                if (GetPixelColor(pixels, stride, px, py) != targetColor)
                    continue;
                SetPixelColor(pixels, stride, px, py, ColorToPaint.Color);

                points.Enqueue(new Point(px - locCorrector, py));
                points.Enqueue(new Point(px + locCorrector, py));
                points.Enqueue(new Point(px, py - locCorrector));
                points.Enqueue(new Point(px, py + locCorrector));
            }
            UpdateBitmap(pixels, stride, drawingCanvas);
        }

        private void UpdateBitmap(byte[] pixels, int stride, Canvas DrawingCanvas)
        {
            BitmapSource bitmapSource = BitmapSource.Create(
                _renderBitmap.PixelWidth,
                _renderBitmap.PixelHeight,
                _renderBitmap.DpiX,
                _renderBitmap.DpiY,
                _renderBitmap.Format,
                null,
                pixels,
                stride
            );

            DrawingCanvas.Children.Clear();
            DrawingCanvas.Background = new ImageBrush(bitmapSource);
        }

        private void SetPixelColor(byte[] pixels, int stride, int x, int y, Color color)
        {
            /*            const int gStep = 1;
                        const int rStep = 2;
                        const int aStep = 3;*/

            int index = (y * stride) + (x * (_renderBitmap.Format.BitsPerPixel / bitsInByte));

            pixels[index] = color.B;
            pixels[index + getGStep] = color.G;
            pixels[index + getRStep] = color.R;
            pixels[index + getAlphaStep] = color.A;
        }

        public void RenderCanvasToBitmap(Canvas DrawingCanvas)
        {
            Canvas copyCanvas = new Canvas()
            {
                Height = DrawingCanvas.Height,
                Width = DrawingCanvas.Width,
                Background = DrawingCanvas.Background
            };

            DrawingCanvas.Children.Clear();
            Size size = new Size(DrawingCanvas.ActualWidth, DrawingCanvas.ActualHeight);

            _renderBitmap = new RenderTargetBitmap((int)size.Width,
                (int)size.Height, _dpiParam, _dpiParam, PixelFormats.Pbgra32);

            copyCanvas.Measure(size);
            copyCanvas.Arrange(new Rect(size));
            _renderBitmap.Render(copyCanvas);
            DrawingCanvas.Background = new ImageBrush(_renderBitmap);
        }

        public Color GetColorAtTempPosition(MouseEventArgs e, Canvas DrawingCanvas)
        {
            const int startSizeParam = 1;
            const int amountOfColorsInPixel = 4;
            const int aParamPixelIndex = 3;
            const int rParamPixelIndex = 2;
            const int gParamPixelIndex = 1;
            const int bParamPixelIndex = 0;

            Point position = e.GetPosition(DrawingCanvas);
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(
                (int)DrawingCanvas.ActualWidth,
                (int)DrawingCanvas.ActualHeight,
                _dpiParam, _dpiParam,
                PixelFormats.Pbgra32);

            renderTargetBitmap.Render(DrawingCanvas);
            CroppedBitmap croppedBitmap = new CroppedBitmap(
                renderTargetBitmap,
                new Int32Rect((int)position.X, (int)position.Y, startSizeParam, startSizeParam));

            byte[] pixels = new byte[amountOfColorsInPixel];
            croppedBitmap.CopyPixels(pixels, amountOfColorsInPixel, 0);

            Color check = Color.FromArgb(pixels[aParamPixelIndex], pixels[rParamPixelIndex], pixels[gParamPixelIndex], pixels[bParamPixelIndex]);

            return check == Colors.White ? _almostWhite : check;
        }

        private Color GetPixelColor(byte[] pixels, int stride, int x, int y)
        {
            const int gStep = 1;
            const int rStep = 2;
            const int aStep = 3;

            int index = (y * stride) + (x * (_renderBitmap.Format.BitsPerPixel / bitsInByte));

            byte blue = pixels[index];
            byte green = pixels[index + gStep];
            byte red = pixels[index + rStep];
            byte alpha = pixels[index + aStep];

            return Color.FromArgb(alpha, red, green, blue);
        }

        public void RemoveImagesFromCanvas(Canvas canvas)
        {
            List<Image> imgs = new List<Image>();
            for (int i = 0; i < canvas.Children.Count; i++)
            {
                if (canvas.Children[i] is Image img)
                {
                    imgs.Add(img);
                }
            }

            foreach (Image img in imgs)
            {
                canvas.Children.Remove(img);
            }
        }

        public Image ConvertSelectionBackgroundToImage(Selection selection)
        {
            RenderTargetBitmap renderTarget = new RenderTargetBitmap(
                (int)selection.ActualWidth,
                (int)selection.ActualHeight,
                _dpiParam,
                _dpiParam,
                PixelFormats.Pbgra32);

            renderTarget.Render(selection);
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
                Height = selection.ActualHeight,
                Width = selection.ActualWidth
            };

            image.UpdateLayout();
            return image;
        }

        private Point GetPointToMoveLine()
        {
            Point selPoint = new Point(Canvas.GetLeft(_selection), Canvas.GetTop(_selection));

            double minY = _selectionLine.Points.Min(y => y.Y);
            double minX = _selectionLine.Points.Min(x => x.X);

            Point firstGlobalPoint = new Point(selPoint.X + Math.Abs(minX), selPoint.Y + Math.Abs(minY));
            return firstGlobalPoint;
        }

        public double GetHeightToLowestPoint(PointCollection newPoints)
        {
            Point firstGlobalPoint = GetPointToMoveLine();
            double resMove = 0;

            double oldMaxY = _selectionLine.Points.Max(x => x.Y);
            double minYOnNewPoint = newPoints.Min(x => x.Y);

            resMove = firstGlobalPoint.Y + minYOnNewPoint;
            return resMove;
        }

        public void CutLeftPartOfCustomSelection(Point globalPoint)
        {
            PointCollection allPoints = LeftCutAndGetPoints(globalPoint);

            double LoAddPoint = GetHeightToLowestPoint(allPoints);
            _selectionLine.Points = allPoints;
            StartFromNewPoint(GetPointWithMinXAndLowestY());

            //set Size
            SetSizeForCustomSelection();
            ConnectFirstAndLastPoints();

            double minimunY = _selectionLine.Points.Min(p => p.Y);
            double maximumY = _selectionLine.Points.Max(p => p.Y);

            Point selPoint = new Point(Canvas.GetLeft(_selection), Canvas.GetTop(_selection));

            double oneMoreCheck = _selection.Height - maximumY;

            Point unChangedNewFirstPoint = new Point(0, LoAddPoint);
            SetCustomStartPoint(unChangedNewFirstPoint);
        }

        public void ConnectFirstAndLastPoints()
        {
            if (_selectionLine.Points.First() == new Point(0, 0) &&
            _selectionLine.Points.First() != _selectionLine.Points.Last())
            {
                Point firstPoint = _selectionLine.Points.First();
                Point lastPoint = _selectionLine.Points.Last();

                List<(int, int)> points =
                    GetLinePoints((int)lastPoint.X, (int)lastPoint.Y, (int)firstPoint.X, (int)firstPoint.Y);

                for (int i = 0; i < points.Count; i++)
                {
                    _selectionLine.Points.Add(new Point(points[i].Item1, points[i].Item2));
                }
            }
        }

        private PointCollection LeftCutAndGetPoints(Point globalPoint)
        {
            double minX = _selectionLine.Points.Min(p => p.X);
            //Get differ which is goes out of borders     
            double fromStartToZeroDist = minX - globalPoint.X;
            List<Point> pointsToRemove = new List<Point>();

            for (int i = 0; i < _selectionLine.Points.Count; i++)
            {
                if (_selectionLine.Points[i].X < fromStartToZeroDist)
                {
                    pointsToRemove.Add(_selectionLine.Points[i]);
                }
            }

            PointCollection allPoints = RemovePoints(pointsToRemove);
            return allPoints;
        }

        public PointCollection RemovePoints(List<Point> pointsToRemove)
        {
            PointCollection allPoints = new PointCollection();
            for (int i = 0; i < _selectionLine.Points.Count; i++)
            {
                allPoints.Add(_selectionLine.Points[i]);
            }

            foreach (Point point in _selectionLine.Points)
            {
                if (pointsToRemove.Contains(point))
                {
                    allPoints.Remove(point);
                }
            }
            return allPoints;
        }

        public Point selLineLoc = new Point();
        public void SetSizeForCustomSelection()
        {
            //ChangeDelataForLeftedPoints();
            double minX = _selectionLine.Points.Min(p => p.X);
            double minY = _selectionLine.Points.Min(p => p.Y);
            double maxX = _selectionLine.Points.Max(p => p.X);
            double maxY = _selectionLine.Points.Max(p => p.Y);

            Point firstPoint = _selectionLine.Points.First();

            Point selLinePoint = new Point(Canvas.GetLeft(_selectionLine), Canvas.GetTop(_selectionLine));
            Point selPoint = new Point(Canvas.GetLeft(_selection), Canvas.GetTop(_selection));
            Size selSize = new Size(_selection.Width, _selection.Height);
            Size selActSize = new Size(_selection.ActualWidth, _selection.ActualHeight);

            double totalHeight = Math.Abs(minY) + Math.Abs(maxY);
            double totalWidth = Math.Abs(minX) + Math.Abs(maxX);

            _selectionLine.Width = totalWidth;
            _selectionLine.Height = totalHeight;
            _selection.Height = totalHeight;
            _selection.Width = totalWidth;
        }

        private void SetCustomStartPoint(Point newStartPointLoc)
        {
            Canvas.SetLeft(_selectionLine, newStartPointLoc.X);
            Canvas.SetTop(_selectionLine, newStartPointLoc.Y);

            selLineLoc = new Point(newStartPointLoc.X, newStartPointLoc.Y);
        }

        public List<(int x, int y)> GetLinePoints(int x0, int y0, int x1, int y1)
        {
            List<(int x, int y)> points = new List<(int x, int y)>();
            const int oneSte = 1;
            const int pointInCross = 2;

            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? oneSte : -oneSte;
            int sy = y0 < y1 ? oneSte : -oneSte;
            int err = dx - dy;

            while (true)
            {
                points.Add((x0, y0));

                if (x0 == x1 && y0 == y1)
                    break;
                int e2 = pointInCross * err;
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

        private Point GetPointWithMinXAndLowestY()
        {
            Point res = new Point();
            for (int i = 0; i < _selectionLine.Points.Count; i++)
            {
                if (i == 0) res = _selectionLine.Points[i];

                if (res.X > _selectionLine.Points[i].X ||
                    (res.X == _selectionLine.Points[i].X && res.Y > _selectionLine.Points[i].Y))
                {
                    res = _selectionLine.Points[i];
                }
            }
            return res;
        }

/*        public Point GetPointWidthMinYAndLeftestX()
        {
            Point res = new Point();
            for (int i = 0; i < _selectionLine.Points.Count; i++)
            {
                if (i == 0) res = _selectionLine.Points[i];
                if (res.Y > _selectionLine.Points[i].Y ||
                  (res.Y == _selectionLine.Points[i].Y &&
                  res.X > _selectionLine.Points[i].X))
                {
                    res = _selectionLine.Points[i];
                }
            }
            return res;
        }*/

/*        public Point GetPointWidthMinYAndRightesX()
        {
            Point res = new Point();
            for (int i = 0; i < _selectionLine.Points.Count; i++)
            {
                if (i == 0) res = _selectionLine.Points[i];
                if (res.Y > _selectionLine.Points[i].Y ||
                  (res.Y == _selectionLine.Points[i].Y &&
                  res.X < _selectionLine.Points[i].X))
                {
                    res = _selectionLine.Points[i];
                }
            }
            return res;
        }*/


        public Point GetPointWithCorrectWidthStand(bool ifLeft)
        {
            Point res = new Point();
            for (int i = 0; i < _selectionLine.Points.Count; i++)
            {
                if (i == 0) res = _selectionLine.Points[i];
                if (res.Y > _selectionLine.Points[i].Y ||
                  (res.Y == _selectionLine.Points[i].Y &&
                  
                  (ifLeft ? res.X > _selectionLine.Points[i].X :
                  res.X < _selectionLine.Points[i].X)))
                {
                    res = _selectionLine.Points[i];
                }
            }
            return res;
        }


        private void StartFromNewPoint(Point newFirstPoint)
        {
            const int nextObgIndex = 1;
            int index = _selectionLine.Points.IndexOf(newFirstPoint);

            PointCollection collection = new PointCollection();
            for (int i = 0; i < _selectionLine.Points.Count; i++)
            {
                collection.Add(_selectionLine.Points[i]);
            }
            double maxY = _selectionLine.Points.Max(y => y.Y);
            double maxX = _selectionLine.Points.Max(x => x.Y);
            for (int i = 0; i < collection.Count; i++)
            {
                Point tempPoint = new Point(
                    collection[i].X - newFirstPoint.X,
                    collection[i].Y - newFirstPoint.Y);
                collection[i] = tempPoint;
            }
            PointCollection resColPoints = new PointCollection();

            for (int i = index; i >= 0; i--)
            {
                resColPoints.Add(collection[i]);
            }

            for (int i = collection.Count - 1; i >= index + nextObgIndex; i--)
            {
                resColPoints.Add(collection[i]);
            }
            _selectionLine.Points = resColPoints;
        }

        public void CutTopPartOfCustomSelection(Point globalPoint)
        {
            PointCollection allPoints = TopCutAndGetPoints(globalPoint);

            double minStartX = allPoints.Min(x => x.X);
            double minStartSelLine = _selectionLine.Points.Min(x => x.X);
            double resDiffer = Math.Abs(Math.Abs(minStartSelLine) - Math.Abs(minStartX));
            double LoAddPoint = GetHeightToLowestPoint(allPoints);
            _selectionLine.Points = allPoints;

            //Point point = GetPointWidthMinYAndLeftestX();
            Point point = GetPointWithCorrectWidthStand(true);
            StartFromNewPoint(point);
            SetSizeForCustomSelection();
            ConnectFirstAndLastPoints();

            double minX = _selectionLine.Points.Min(p => p.X);
            double maxX = _selectionLine.Points.Max(p => p.X);
            double maxY = _selectionLine.Points.Max(p => p.Y);

            Point selPoint = new Point(Canvas.GetLeft(_selection), Canvas.GetTop(_selection));
            double oneMoreCheck = _selection.Height - maxY;
            Point unChangedNewFirstPoint = new Point(
                selPoint.X + resDiffer, 0);

            SetCustomStartPoint(unChangedNewFirstPoint);
        }

        private PointCollection TopCutAndGetPoints(Point globalPoint)
        {
            double minY = _selectionLine.Points.Min(p => p.Y);

            //Get differ which is goes out of borders     
            double fromStartToZeroDist = minY - globalPoint.Y;

            List<Point> pointsToRemove = new List<Point>();
            for (int i = 0; i < _selectionLine.Points.Count; i++)
            {
                if (_selectionLine.Points[i].Y < fromStartToZeroDist)
                {
                    pointsToRemove.Add(_selectionLine.Points[i]);
                }
            }
            PointCollection allPoints = RemovePoints(pointsToRemove);

            return allPoints;
        }

        public void CutRightPartOfCustomSelection(Point globalPoint, double drawingCanvasWidth)
        {
            PointCollection allPoints = RightCutAndGetPoints(globalPoint, drawingCanvasWidth);

            double minStartY = allPoints.Min(y => y.Y);
            double minStartSelLineY = _selectionLine.Points.Min(y => y.Y);
            double resDifferY = Math.Abs(Math.Abs(minStartSelLineY) - Math.Abs(minStartY));
            double heightDiffer = _selection.Height - Math.Abs(minStartY);
            double LoAddPoint = GetHeightToLowestPoint(allPoints);
            _selectionLine.Points = allPoints;

            //Point point = GetPointWidthMinYAndRightesX();
            Point point = GetPointWithCorrectWidthStand(false);
            StartFromNewPointRightCut(point);
            SetSizeForCustomSelection();
            ConnectFirstAndLastPoints();

            double maxY = _selectionLine.Points.Max(p => p.Y);
            double newMinYPoint = _selectionLine.Points.Min(y => y.Y);
            double yDiffer = Math.Abs(minStartSelLineY) + Math.Abs(newMinYPoint);

            Point selPoint = new Point(Canvas.GetLeft(_selection), Canvas.GetTop(_selection));
            double newMinY = allPoints.Min(y => y.Y);
            double oneMoreCheck = _selection.Height - maxY;
            Point unChangedNewFirstPoint = new Point(
                selPoint.X, LoAddPoint);

            SetCustomStartPoint(unChangedNewFirstPoint);
        }

        private PointCollection RightCutAndGetPoints(Point globalPoint, double drawingCanvasWidth)
        {
            Point selPoint = new Point(Canvas.GetLeft(_selection), Canvas.GetTop(_selection));
            double maxX = _selectionLine.Points.Max(x => x.X);

            double differ = maxX - ((globalPoint.X + _selection.Width) - drawingCanvasWidth);
            List<Point> pointsToRemove = new List<Point>();
            for (int i = 0; i < _selectionLine.Points.Count; i++)
            {
                if (_selectionLine.Points[i].X > differ + _selection.DashedBorder.StrokeThickness)
                {
                    pointsToRemove.Add(_selectionLine.Points[i]);
                }
            }
            PointCollection allPoints = RemovePoints(pointsToRemove);
            return allPoints;
        }

        private void StartFromNewPointRightCut(Point newFirstPoint)
        {
            //new first point
            int index = _selectionLine.Points.IndexOf(newFirstPoint);

            PointCollection collection = new PointCollection();
            for (int i = 0; i < _selectionLine.Points.Count; i++)
            {
                collection.Add(_selectionLine.Points[i]);
            }
            double maxY = _selectionLine.Points.Max(y => y.Y);
            double maxX = _selectionLine.Points.Max(x => x.Y);

            for (int i = 0; i < collection.Count; i++)
            {
                Point tempPoint = new Point(
                    collection[i].X - newFirstPoint.X,
                    collection[i].Y - newFirstPoint.Y);
                collection[i] = tempPoint;
            }

            _selectionLine.Points = collection;
        }

        public void CutBottomPartOfCustomSelection(Point globalPoint, double drawCanHeight)
        {
            PointCollection allPoints = BottomCutAndGetPoints(globalPoint, drawCanHeight);

            double minStartX = allPoints.Min(x => x.X);
            double minStartSelLine = _selectionLine.Points.Min(x => x.X);
            double resDiffer = Math.Abs(Math.Abs(minStartSelLine) - Math.Abs(minStartX));

            //double LoAddPoint = GetHeightToLowestPoint(allPoints);
            _selectionLine.Points = allPoints;

            //Point point = GetPointWidthMinYAndLeftestX();
            Point point = GetPointWithCorrectWidthStand(true);
            StartFromNewPoint(point);
            SetSizeForCustomSelection();
            ConnectFirstAndLastPoints();

            double minX = _selectionLine.Points.Min(p => p.X);
            double maxX = _selectionLine.Points.Max(p => p.X);
            double maxY = _selectionLine.Points.Max(p => p.Y);
            double minY = _selectionLine.Points.Min(p => p.Y);

            Point selPoint = new Point(Canvas.GetLeft(_selection), Canvas.GetTop(_selection));
            Point linePoint = new Point(Canvas.GetLeft(_selectionLine), Canvas.GetTop(_selectionLine));
            double oneMoreCheck = selPoint.Y + Math.Abs((drawCanHeight - selPoint.Y - _selection.Height));
            Point unChangedNewFirstPoint = new Point(
                selPoint.X + resDiffer, selPoint.Y);
            SetCustomStartPoint(unChangedNewFirstPoint);

            double yPoint = Canvas.GetTop(_selectionLine);
            double hiehgtCheck = maxY + Math.Abs(minY);
            double res = yPoint + hiehgtCheck;
        }

        private PointCollection BottomCutAndGetPoints(Point globalPoint, double drawingCanHeight)
        {
            Point selPoint = new Point(Canvas.GetLeft(_selection), Canvas.GetTop(_selection));
            double maxY = _selectionLine.Points.Max(y => y.Y);
            double minY = _selectionLine.Points.Min(y => y.Y);

            double check = selPoint.Y + maxY + Math.Abs(minY);

            double differ = maxY - ((globalPoint.Y + _selection.Height) - drawingCanHeight);
            List<Point> pointsToRemove = new List<Point>();
            for (int i = 0; i < _selectionLine.Points.Count; i++)
            {
                if (_selectionLine.Points[i].Y > differ + _selection.DashedBorder.StrokeThickness)
                {
                    pointsToRemove.Add(_selectionLine.Points[i]);
                }
            }
            PointCollection allPoints = RemovePoints(pointsToRemove);

            maxY = allPoints.Max(y => y.Y);
            minY = allPoints.Min(y => y.Y);
            check = selPoint.Y + maxY + Math.Abs(minY);

            return allPoints;
        }

        public void RemoveSelectionGridInDeep(Selection selection, int count)
        {
            const int countParam = 1;
            if (count > 0)
            {
                if (count != 0)
                {
                    selection.SizingGrid.Visibility = Visibility.Hidden;

                    selection.RemoveSizingGrid();
                }
                if (selection.CheckCan.Children.OfType<Polyline>().Any())
                {
                    Polyline line = selection.CheckCan.Children.OfType<Polyline>().First();

                    selection.DashedBorder.Visibility = Visibility.Hidden;
                    Canvas.SetLeft(line, 0);
                    Canvas.SetTop(line, 0);
                    line.Stretch = Stretch.Fill;
                }
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

        public void ClearBgForSelectionInDeep(Selection selection)
        {
            Selection childSelection = null;
            for (int i = 0; i < selection.SelectCan.Children.Count; i++)
            {
                UIElement elem = selection.SelectCan.Children[i];
                if (elem is Selection sel)
                {
                    sel.SelectCan.Background = new SolidColorBrush(Colors.Transparent);
                    childSelection = sel;
                }
            }
            if (childSelection is null) return;
            ClearBgForSelectionInDeep(childSelection);
        }

        public void SetVisabilityForSelectionBorders(Canvas can, Visibility visibilityState)
        {
            Selection childSelection = null;
            for (int i = 0; i < can.Children.Count; i++)
            {
                UIElement elem = can.Children[i];
                if (elem is Selection sel)
                {
                    sel.DashedBorder.Visibility = visibilityState;
                    sel.SizingGrid.Visibility = visibilityState;
                    childSelection = sel;

                    if (visibilityState == Visibility.Visible &&
                        sel.CheckCan.Children.OfType<Polyline>().Any())
                    {
                        sel.DashedBorder.Visibility = Visibility.Hidden;
                        sel.SizingGrid.Visibility = Visibility.Hidden;
                    }
                }
            }

            if (!(childSelection is null))
            {
                Polyline polyline = childSelection.CheckCan.Children.OfType<Polyline>().FirstOrDefault();
                if (!(polyline is null)) polyline.Visibility = visibilityState;
            }
            if (childSelection is null) return;
            SetVisabilityForSelectionBorders(childSelection.SelectCan, visibilityState);
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


        public bool IfSelectionSizeIsNotAcceptable() //-, dublirovanie 
        {
            const int sizeCheckParam = 1;
            return _selection.SelectionBorder.Width is double.NaN ||
                _selection.SelectionBorder.Height is double.NaN ||
                _selection.SelectionBorder.Width <= sizeCheckParam ||
                _selection.SelectionBorder.Height <= sizeCheckParam ||
                _selection.Height is double.NaN ||
                _selection.Width is double.NaN ||
                _selection.Height <= sizeCheckParam ||
                _selection.Width <= sizeCheckParam;
        }

        public LineDivision GetLineDivision(Point endPoint, Point startPoint)
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

        public bool IfCrossDirection(Point endPoint)
        {
            //get middle 
            //check distance difference between middle point and zeroX, zeroY
            Point absPoint = new Point(Math.Abs(endPoint.X), Math.Abs(endPoint.Y));
            Point checkPoint;
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
        public (Point, Point) SetPositionForFigureWithShift(Point point, Point mousePoint)
        {
            const int reverseMark = -1;

            double xDiffer = mousePoint.X - previousPoint.X;
            double yDiffer = (mousePoint.Y - previousPoint.Y) * reverseMark; //cause syst of cords is reversed
            double checkY = previousPoint.Y - mousePoint.Y;
            double checkX = mousePoint.X - previousPoint.X;

            Point checkPoint = GetEqualPoint(new Point(checkX, checkY));
            if (xDiffer > 0 && yDiffer > 0)
            {
                Point resLocPoint = new Point(point.X, previousPoint.Y - checkPoint.Y);
                Point resSizePoint = new Point(previousPoint.X + checkPoint.X, previousPoint.Y - checkPoint.Y);
                return (resLocPoint, resSizePoint);
            }
            else if (xDiffer > 0 && yDiffer < 0)
            {
                Point resLocPoint = previousPoint;
                Point resSizePoint = new Point(previousPoint.X - checkPoint.X, previousPoint.Y + checkPoint.Y);
                return (resLocPoint, resSizePoint);
            }
            else if (xDiffer < 0 && yDiffer < 0)
            {
                Point resLocPoint = new Point(previousPoint.X - checkPoint.X, previousPoint.Y);
                Point resSizePoint = new Point(previousPoint.X - checkPoint.X, previousPoint.Y + checkPoint.Y);
                return (resLocPoint, resSizePoint);
            }
            else if (xDiffer < 0 && yDiffer > 0)
            {
                Point resLocPoint = new Point(previousPoint.X - checkPoint.X, previousPoint.Y - checkPoint.Y);
                Point resSizePoint = new Point(previousPoint.X + checkPoint.X, previousPoint.Y + checkPoint.Y);
                return (resLocPoint, resSizePoint);
            }
            return (point, mousePoint);
        }

        public Point GetEqualPoint(Point point)
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

        public Point GetHighestPoints(Polyline line)
        {
            double minX = line.Points.Min(x => x.X);
            double minY = line.Points.Min(y => y.Y);
            double maxX = line.Points.Max(x => x.X);
            double maxY = line.Points.Max(y => y.Y);

            return new Point(Math.Abs(minX) + Math.Abs(maxX), Math.Abs(minY) + Math.Abs(maxY));
        }

        public void IfPointOutOfBorderCustomSelection(MouseEventArgs e, Canvas drawingCanvas)
        {
            if (_selectionLine is null) return;
            _selectionLine.RenderTransform = new TranslateTransform(0, 0);
            double x = e.GetPosition(drawingCanvas).X;
            double y = e.GetPosition(drawingCanvas).Y;

            Point tempPoint = e.GetPosition(drawingCanvas);

            tempPoint = x < 0 ? new Point(0, tempPoint.Y) :
                x > drawingCanvas.Width ? new Point(drawingCanvas.Width + 1, tempPoint.Y) : tempPoint;

            tempPoint = y < 0 ? new Point(tempPoint.X, 0) :
                y > drawingCanvas.Height ? new Point(tempPoint.X, drawingCanvas.Height) : tempPoint;

            /*            if (x < 0)
                        {
                            tempPoint = new Point(0, tempPoint.Y);
                        }
                        if (x > drawingCanvas.Width)
                        {
                            tempPoint = new Point(drawingCanvas.Width + 1, tempPoint.Y);
                        }
                        if (y < 0)
                        {
                            tempPoint = new Point(tempPoint.X, 0);
                        }
                        if (y > drawingCanvas.Height)
                        {
                            tempPoint = new Point(tempPoint.X, drawingCanvas.Height);
                        }*/

            int xRes = (int)(tempPoint.X - Canvas.GetLeft(_selectionLine));
            int yRes = (int)(tempPoint.Y - Canvas.GetTop(_selectionLine));

            _selectionLine.Points.Add(new Point(xRes, yRes));
        }

        public void RemoveAllImagesFromCanvas(Canvas canvas)
        {
            List<Image> imgs = new List<Image>();
            for (int i = 0; i < canvas.Children.Count; i++)
            {
                if (canvas.Children[i] is Image img)
                {
                    imgs.Add(img);
                }
            }

            foreach (Image temp in imgs)
            {
                canvas.Children.Remove(temp);
            }
        }

        public Canvas GetAuxiliaryCanvas(Canvas canvas)
        {
            Canvas can = new Canvas()
            {
                Height = canvas.Height,
                Width = canvas.Width,
                Background = canvas.Background,
            };

            //RenderOptions.SetBitmapScalingMode(can, BitmapScalingMode.NearestNeighbor);
            List<UIElement> DrawingCanChildren = ReAssignChildrenInAuxiliaryCanvas(canvas);
            canvas.Children.Clear();

            for (int i = 0; i < DrawingCanChildren.Count; i++)
            {
                can.Children.Add(DrawingCanChildren[i]);
            }

            return can;
        }

        public List<UIElement> ReAssignChildrenInAuxiliaryCanvas(Canvas canvas)
        {
            List<UIElement> res = new List<UIElement>();
            for (int i = 0; i < canvas.Children.Count; i++)
            {
                res.Add(canvas.Children[i]);
            }
            return res;
        }

        public void MakeRectangleSelection(MouseEventArgs e, Canvas drawingCanvas)
        {
            const int rectSelectionThickness = 1;

            _selectionRect = new Rectangle
            {
                Stroke = Brushes.LightBlue,
                StrokeThickness = rectSelectionThickness,
                Fill = Brushes.Transparent
            };

            Canvas.SetLeft(_selectionRect, e.GetPosition(drawingCanvas).X);
            Canvas.SetTop(_selectionRect, e.GetPosition(drawingCanvas).Y);

            _firstSelectionStart = new Point(e.GetPosition(drawingCanvas).X, e.GetPosition(drawingCanvas).Y);
            drawingCanvas.Children.Add(_selectionRect);
        }

        public void AddBgImageInChildren(Canvas canvas)
        {
            canvas.RenderTransform = new TranslateTransform(0, 0);
            Canvas can = GetAuxiliaryCanvas(canvas);

            Image img = ConvertCanvasInImage(can);
            can.Background = new ImageBrush()
            {
                ImageSource = img.Source
            };

            Image bgImg = ConvertBackgroundToImage(can);

            canvas.Background = new SolidColorBrush(Colors.White);
            canvas.Children.Add(bgImg);

            can.Children.Clear();
            canvas.RenderTransform = new TranslateTransform(_horizontalOffset, 0);
        }

        public Point GetRightIfOutOfBoundaries(Point checkPoint, Canvas drawingCanvas)
        {
            checkPoint.X = checkPoint.X < 0 ? 0 :
                checkPoint.X >= drawingCanvas.Width ?
                drawingCanvas.Width : checkPoint.X;

            checkPoint.Y = checkPoint.Y < 0 ? 0 :
                checkPoint.Y >= drawingCanvas.Height ?
                drawingCanvas.Height : checkPoint.Y;

            /*            if (checkPoint.X < 0)
                        {
                            checkPoint.X = 0;
                        }
                        else if (checkPoint.X >= drawingCanvas.Width)
                        {
                            checkPoint.X = drawingCanvas.Width;
                        }
                        if (checkPoint.Y < 0)
                        {
                            checkPoint.Y = 0;
                        }
                        else if (checkPoint.Y >= drawingCanvas.Height)
                        {
                            checkPoint.Y = drawingCanvas.Height;
                        }*/
            return checkPoint;
        }

        public (Point, Point) GetMinAndMaxPointsInPolyline(Polyline polyline,
            bool ifInvertion, Canvas drawingCanvas)
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            for (int i = 0; i < polyline.Points.Count; i++)
            {
                Point transformedPoint = polyline.TransformToAncestor(_selection.SelectCan).Transform(polyline.Points[i]);

                if (ifInvertion || _ifCutCheck)
                {
                    transformedPoint = new Point(selLineLoc.X + polyline.Points[i].X, selLineLoc.Y + polyline.Points[i].Y);
                    transformedPoint = new Point(transformedPoint.X, transformedPoint.Y);
                }

                transformedPoint = new Point((int)transformedPoint.X, (int)transformedPoint.Y);
                transformedPoint = GetRightIfOutOfBoundaries(transformedPoint, drawingCanvas);

                if (transformedPoint.X <= minX) minX = transformedPoint.X;
                if (transformedPoint.Y <= minY) minY = transformedPoint.Y;
                if (transformedPoint.X >= maxX) maxX = transformedPoint.X;
                if (transformedPoint.Y >= maxY) maxY = transformedPoint.Y;
            }

            return (new Point(minX, minY), new Point(maxX, maxY));
        }

        public Image GetRenderOfCustomCanvasVTwo(Polyline polyline,
            Canvas toRender, bool ifInvertion = false)
        {
            const int minSizeParam = 1;
            toRender.RenderTransform = new TranslateTransform(0, 0);

            Point selPoint = new Point(Canvas.GetLeft(_selection) - _selection.DashedBorder.StrokeThickness, Canvas.GetTop(_selection));
            Point selLinePoint = new Point(Canvas.GetLeft(_selectionLine), Canvas.GetTop(_selectionLine));

            double selWidth = _selection.Width;
            double selHeight = _selection.Height;
            double selLineMinY = _selectionLine.Points.Min(y => y.Y);

            //Set min and max points
            (Point min, Point max) = GetMinAndMaxPointsInPolyline(polyline, ifInvertion, toRender);
            double minX = min.X;
            double minY = min.Y;
            double maxX = max.X;
            double maxY = max.Y;

            //polyline min maxp points 
            double checkMinX = polyline.Points.Min(x => x.X);
            double checkMaxX = polyline.Points.Max(x => x.X);
            double checkMinY = polyline.Points.Min(x => x.Y);
            double checkMaxY = polyline.Points.Max(x => x.Y);

            double width = maxX - minX;
            double height = maxY - minY;

            width = Math.Abs(checkMinX) + Math.Abs(checkMaxX);
            height = Math.Abs(checkMinY) + Math.Abs(checkMaxY);

            //update min max points is selection is cut
            if (_ifCutCheck)
            {
                minX = selPoint.X/*+ _main._selectionLine.StrokeThickness*/;
                minY = /*0;//*/ selPoint.Y /*+ _main._selectionLine.StrokeThickness*/;
                maxX = selPoint.X + selWidth;
                maxY = selPoint.Y + selHeight;
            }

            if (width < minSizeParam || height < minSizeParam /*|| width == 0 || height == 0*/)
            {
                return null;
            }
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)width, (int)height,
                _dpiParam, _dpiParam, PixelFormats.Pbgra32);


            // get drawingVisual for bg (poped)
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

                    transformedPoint = new Point((int)transformedPoint.X, (int)transformedPoint.Y);
                    if (ifInvertion || _ifCutCheck)
                    {
                        transformedPoint = new Point(selLineLoc.X + polyline.Points[i].X, selLineLoc.Y + polyline.Points[i].Y);
                    }
                    if (_ifCutCheck && _selectionType == SelectionType.Custom)
                    {
                        transformedPoint = new Point(transformedPoint.X + Math.Abs(checkMinX), transformedPoint.Y + Math.Abs(selLineMinY));
                    }
                    transformedPoint = GetRightIfOutOfBoundaries(transformedPoint, toRender);
                    if (i == 0 ||
                        i == polyline.Points.Count - 1)
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
            renderTargetBitmap.Render(drawingVisual);
            RenderOptions.SetEdgeMode(renderTargetBitmap, EdgeMode.Aliased);

            //Set bg image
            Image image = new Image
            {
                Source = renderTargetBitmap,
                Width = width,
                Height = height,
                Stretch = Stretch.Fill,
                RenderTransform = new TranslateTransform(0, 0)
            };
            RenderOptions.SetEdgeMode(image, EdgeMode.Aliased);

            toRender.RenderTransform = new TranslateTransform(_horizontalOffset, 0);
            return image;
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

        public void RemoveChildrenExceptCheckRect(Canvas drawingCanvas, Rectangle checkRect) //-, LINQ
        {
            List<UIElement> elementsToRemove = new List<UIElement>();
            elementsToRemove.AddRange(drawingCanvas.Children.OfType<UIElement>().Where(child => child != checkRect));
            elementsToRemove.ForEach(el => drawingCanvas.Children.Remove(el));

            /*            foreach (UIElement child in drawingCanvas.Children)
                        {
                            if (child != checkRect)
                            {
                                elementsToRemove.Add(child);
                            }
                        }*/
            /*                foreach (UIElement element in elementsToRemove)
                        {
                            drawingCanvas.Children.Remove(element);
                        }*/
        }

        public void SetCanvasBg(Canvas canvas, Rectangle checkRect, bool ifSpray = false)
        {
            //canvas.RenderTransform = new TranslateTransform(0, 0);
            Image bg = ConvertCanvasInImage(canvas);
            bg.RenderTransform = new TranslateTransform(0, 0);

            //bg.SnapsToDevicePixels = true;
            RenderOptions.SetEdgeMode(bg, EdgeMode.Aliased);
            if (ifSpray) RemoveChildrenExceptCheckRect(canvas, checkRect);
            else canvas.Children.Clear();

            canvas.Background = new ImageBrush()
            {
                ImageSource = bg.Source,
                //Transform = new TranslateTransform()
            };
            //canvas.RenderTransform = new TranslateTransform(_horizontalOffset, 0);
        }

        public Selection GetSelectionsSelectionParent(Canvas selParent)
        {
            Border res = (Border)selParent.Parent;
            Canvas resCan = (Canvas)res.Parent;

            Grid resGrid = (Grid)resCan.Parent;
            Selection check = (Selection)resGrid.Parent;

            return check;
        }

        public void SetParamsForSelection(bool ifSelLineContains, Point globalPoint,
            Canvas drawingCanvas)
        {
            if (ifSelLineContains) //Custom
            {
                Canvas.SetLeft(_selectionLine, globalPoint.X);
                Canvas.SetTop(_selectionLine, globalPoint.Y);

                _selectionLine.RenderTransform = null;
                drawingCanvas.RenderTransform = null;
                _selection = null;
                _selectionLine.RenderTransform = null;
                if (_selectionLine.Parent is null) drawingCanvas.Children.Add(_selectionLine);

                drawingCanvas.UpdateLayout();
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
            drawingCanvas.Children.Add(_selectionRect);

            _firstSelectionStart = globalPoint;
            _firstSelectionEnd = new Point(globalPoint.X + _selection.Width,
                                        globalPoint.Y + _selection.Height);
        }

        public void InitElementsInCanvas(Canvas canvas, List<UIElement> elems)
        { 
            foreach (UIElement el in elems)
            {
                canvas.Children.Add(el);
            }
        }

        public Point GetPointOfSelection(UIElement element, Canvas drawingCanvas)
        {
            if (element == drawingCanvas)
            {
                return new Point(0, 0);
            }
            DependencyObject parent = VisualTreeHelper.GetParent(element);
            if (parent is UIElement parentElement)
            {
                GeneralTransform transform = element.TransformToVisual(parentElement);
                Point currentPosition = transform.Transform(new Point(0, 0));
                Point parentPosition = GetPointOfSelection(parentElement, drawingCanvas);
                return new Point(currentPosition.X + parentPosition.X, currentPosition.Y + parentPosition.Y);
            }
            return new Point(0, 0);
        }

        public Image GetClearImageOfCanvasBG(Canvas canvas)
        {
            //clear canvas children + init in list
            List<UIElement> canElems = ReAssignChildrenInAuxiliaryCanvas(canvas);
            canvas.Children.Clear();

            //make clear image
            Image selectionBGImg = ConvertCanvasInImage(canvas);

            //return children
            InitElementsInCanvas(canvas, canElems);
            return selectionBGImg;
        }

        public void AddSelectionBGInDrawingCanvas(Selection selection, Canvas drawingCanvas)
        {
            //Get selection bg in image format
            Image selectionBGImg = GetClearImageOfCanvasBG(selection.SelectCan);

            //make selection bg transparent
            selection.SelectCan.Background = new SolidColorBrush(Colors.Transparent);

            //get elems + Clear DrawingCanvas 
            List<UIElement> drawingCanElems = ReAssignChildrenInAuxiliaryCanvas(drawingCanvas);
            drawingCanvas.Children.Clear();

            //add bgImage in drawingCanvas as a child 
            drawingCanvas.Children.Add(selectionBGImg); //do something with location
                                                        //Init location for selectionBGIMG
            Point point = GetPointOfSelection(selection.SelectCan, drawingCanvas);

            Canvas.SetLeft(selectionBGImg, point.X);
            Canvas.SetTop(selectionBGImg, point.Y);

            //convert drawing canvas bg to image 
            Image drawingCanvasBgImg = ConvertCanvasInImage(drawingCanvas);
            drawingCanvas.Children.Clear();

            //set it as bg 
            drawingCanvas.Background = new ImageBrush()
            {
                ImageSource = drawingCanvasBgImg.Source
            };

            //return drawingCanvas children
            InitElementsInCanvas(drawingCanvas, drawingCanElems);
        }

        public Rectangle GetRectForRectSelection(Selection selection, Canvas drawingCanvas)
        {
            Rectangle res = new Rectangle();
            Point startPoint = GetPointOfSelection(selection, drawingCanvas);

            Canvas.SetLeft(res, startPoint.X);
            Canvas.SetTop(res, startPoint.Y);

            res.Width = selection.Width;
            res.Height = selection.Height;
            return res;
        }

        public bool IsPartiallyInRegion(Point topLeft, Point bottomRight,
            double minX, double minY, double maxX, double maxY)
        {
            return (topLeft.X < maxX && bottomRight.X > minX &&
                topLeft.Y < maxY && bottomRight.Y > minY);
        }

        public List<Image> TrimImage(Image image, double minX, double minY,
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
                    PaintImageInWhite(newImage);
                }
            }
            return trimmedImages;
        }

        public const int maxColorParam = 255;
        public Image PaintImageInWhite(Image originalImage)
        {
            //const int pixStep = 4;
            //const int getGStep = 1;
            //const int getRStep = 2;
           // const int getAlphaStep = 3;

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

            for (int i = 0; i < pixels.Length; i += pixStep)
            {
                pixels[i] = maxColorParam;     // Blue
                pixels[i + getGStep] = maxColorParam; // Green
                pixels[i + getRStep] = maxColorParam; // Red
                pixels[i + getAlphaStep] = maxColorParam; // Alpha
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

        public Line TrimLine(Line line, double minX, double minY, double maxX, double maxY) //-, DUBL
        {
            Point starPoint = SetLinePoint(new Point(line.X1, line.Y1), minX, minY, maxX, maxY);
            Point endPoint = SetLinePoint(new Point(line.X2, line.Y2), minX, minY, maxX, maxY);

            line.X1 = starPoint.X;
            line.Y1 = starPoint.Y;
            line.X2 = endPoint.X;
            line.Y2 = endPoint.Y;


            /*  if (line.X1 >= minX && line.X1 <= maxX &&
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
              }*/
            return line;
        }
        private Point SetLinePoint(Point linePoint, double minX, double minY, double maxX, double maxY)
        {
            if (linePoint.X >= minX && linePoint.X <= maxX &&
                linePoint.Y >= minY && linePoint.Y <= maxY)
            {
                linePoint.X = Math.Max(minX, Math.Min(linePoint.X, maxX));
                linePoint.Y = Math.Max(minY, Math.Min(linePoint.Y, maxY));
            }
            return linePoint;
        }


        public Polyline TrimPolyline(Polyline polyline, double minX,
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

        public UIElement TrimElement(UIElement element, double minX,
            double minY, double maxX, double maxY)
        {
            /*            return element is Line line ? TrimLine(line, minX, minY, maxX, maxY) :
                            element is Polyline polyline ? TrimPolyline(polyline, minX, minY, maxX, maxY) : null;*/

            if (element is Line line)
            {
                return TrimLine(line, minX, minY, maxX, maxY);
            }
            else if (element is Polyline polyline)
            {
                return TrimPolyline(polyline, minX, minY, maxX, maxY);
            }
            return null;
        }

        public void DeleteAndTrimElements(Point point1, Point point2,
        Selection selection, Canvas parentCanvas, Canvas drawingCanvas) //-, linq
        {
            double imageWidth = drawingCanvas.Width;
            double imageHeight = drawingCanvas.Height;

            double minX = (int)Math.Min(point1.X, point2.X);
            double minY = (int)Math.Min(point1.Y, point2.Y);
            double maxX = (int)Math.Max(point1.X, point2.X);
            double maxY = (int)Math.Max(point1.Y, point2.Y);

            var elementsToRemove = new List<UIElement>();
            var elementsToAdd = new List<UIElement>();
            for (int i = 0; i < drawingCanvas.Children.Count; i++)
            {
                var bounds = VisualTreeHelper.GetDescendantBounds(drawingCanvas.Children[i]);

                var topLeft = drawingCanvas.Children[i].
                    TransformToAncestor(drawingCanvas).Transform(new Point(0, 0));

                if (drawingCanvas.Children[i] is Image)
                {
                    ((Image)drawingCanvas.Children[i]).Width = imageWidth;
                    ((Image)drawingCanvas.Children[i]).Height = imageHeight;
                    if (bounds == Rect.Empty)
                    {
                        bounds = new Rect(0, 0, drawingCanvas.Width, drawingCanvas.Height);
                    }
                }

                var bottomRight = new Point(
                    topLeft.X + bounds.Width, topLeft.Y + bounds.Height);

                if (topLeft.X >= minX && topLeft.Y >= minY &&
                    bottomRight.X <= maxX && bottomRight.Y <= maxY)
                {
                    elementsToRemove.Add(drawingCanvas.Children[i]);
                }
                else if (IsPartiallyInRegion(topLeft, bottomRight, minX, minY, maxX, maxY))
                {
                    if (drawingCanvas.Children[i] is Image image)
                    {
                        var trimmedImages = TrimImage(image, minX, minY, maxX, maxY, topLeft);
                        if (trimmedImages != null && trimmedImages.Count > 0)
                        {
                            elementsToAdd.AddRange(trimmedImages);
                            elementsToRemove.Add(drawingCanvas.Children[i]);
                        }
                    }
                    else
                    {
                        var trimmedElement = TrimElement(drawingCanvas.Children[i],
                            minX, minY, maxX, maxY);
                        if (trimmedElement != null)
                        {
                            elementsToAdd.Add(trimmedElement);
                            elementsToRemove.Add(drawingCanvas.Children[i]);
                        }
                    }
                }
            }



            elementsToRemove.ForEach(element => drawingCanvas.Children.Remove(element));
            elementsToAdd.ForEach(element => drawingCanvas.Children.Add(element));

            /*
                        for (int i = 0; i < elementsToRemove.Count; i++)
                        {
                            drawingCanvas.Children.Remove(elementsToRemove[i]);
                        }

                        for (int i = 0; i < elementsToAdd.Count; i++)
                        {
                            drawingCanvas.Children.Add(elementsToAdd[i]);
                        }*/

            parentCanvas.Children.Remove(selection);

            Canvas copy = GetAuxiliaryCanvas(drawingCanvas);
            Image img = ConvertCanvasInImage(copy);
            parentCanvas.Children.Add(selection);

            drawingCanvas.Background = new ImageBrush()
            {
                ImageSource = img.Source
            };
        }

        public void InitSelectedBgInRectCanvas(Rectangle rect, Selection selection,
           Canvas selParentCanvas, Canvas drawingCanvas) //- LINQ
        {
            if (_selectionType == SelectionType.Invert && selParentCanvas == drawingCanvas)
                drawingCanvas.Children.Remove(selection);

            if (!(selection.SelectCan.Background is ImageBrush))
            {
                _firstSelectionStart = new Point(_firstSelectionStart.X, _firstSelectionStart.Y);

                RenderTargetBitmap copy = GetRenderedCopy(
                    selection, selParentCanvas, _firstSelectionStart, _firstSelectionEnd);
                selection.UpdateLayout();

                Image checkImg = new Image()
                {
                    Source = copy
                };
                checkImg = SwipeColorsInImage(checkImg, _whiteColor, _transparentColor);

                selection.BgCanvas.Background = new ImageBrush()
                {
                    ImageSource = checkImg.Source,
                    Transform = new TranslateTransform(),
                    Stretch = Stretch.Fill
                };
                selection.RenderTransform = new TranslateTransform(0, 0);
                selection.SelectCan.RenderTransform = selection.RenderTransform;
            }

            Point start = new Point(Canvas.GetLeft(rect), Canvas.GetTop(rect));
            Point end = new Point(start.X + rect.Width, start.Y + rect.Height);

            DeleteAndTrimElements(start, end, selection, selParentCanvas, drawingCanvas);
        }
        public Image SwipeColorsInImage(Image image, Color toBeChanged, Color toChangeOn)
        {
            /*  const int bitsInByte = 8;
              const int bitsPerPixelStep = 7;
              const int pixStep = 4;
              const int gStep = 1;
              const int rStep = 2;
              const int aStep = 3;*/

            BitmapSource bitmapSource = image.Source as BitmapSource;
            if (bitmapSource is null) return image;

            WriteableBitmap writeableBitmap = new WriteableBitmap(bitmapSource);

            int width = writeableBitmap.PixelWidth;

            int height = writeableBitmap.PixelHeight;
            int stride = width * ((writeableBitmap.Format.BitsPerPixel + bitsAdd) / bitsInByte);
            byte[] pixels = new byte[height * stride];
            writeableBitmap.CopyPixels(pixels, stride, 0);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * stride + pixStep * x;

                    byte blue = pixels[index];
                    byte green = pixels[index + getGStep];
                    byte red = pixels[index + getRStep];
                    byte alpha = pixels[index + getAlphaStep];

                    if ((IfColorParamsBetweenFivePoints(red, toChangeOn.R) &&
                        IfColorParamsBetweenFivePoints(green, toChangeOn.G) &&
                        IfColorParamsBetweenFivePoints(blue, toChangeOn.B) &&
                        IfColorParamsBetweenFivePoints(alpha, toChangeOn.A)) ||

                        (red == maxColorParam && green == maxColorParam &&
                        blue == maxColorParam && alpha == maxColorParam) ||

                        (_ifTransparencyIsActivated && alpha == toBeChanged.A &&
                        red == toBeChanged.R && green == toBeChanged.G && blue == toBeChanged.B) ||

                        (alpha != maxColorParam || (red == maxColorParam && blue == maxColorParam && green == maxColorParam)))
                    {
                        pixels[index] = toChangeOn.B;
                        pixels[index + getGStep] = toChangeOn.G;
                        pixels[index + getRStep] = toChangeOn.R;
                        pixels[index + getAlphaStep] = toChangeOn.A;
                    }
                    /*                    if (alpha != maxColorParam || (red == maxColorParam && blue == maxColorParam && green == maxColorParam))
                                        {
                                            pixels[index] = toChangeOn.B;
                                            pixels[index + gStep] = toChangeOn.G;
                                            pixels[index + rStep] = toChangeOn.R;
                                            pixels[index + aStep] = toChangeOn.A;
                                        }*/
                }
            }

            writeableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
            image.Source = writeableBitmap;

            return image;
        }

        public bool IfColorParamsBetweenFivePoints(byte color, int colorParam)
        {
            const int maxColorDiffer = 5;
            int res = Math.Abs(color - colorParam);

            return res <= maxColorDiffer;
        }

        public RenderTargetBitmap GetRenderedCopy(Selection selection, Canvas canvas,
           Point start, Point end)
        {
            const int minSizeParam = 1;
            start = new Point(start.X, start.Y);
            Size size = new Size(selection.Width, selection.Height);
            if (size.Width < minSizeParam) size.Width = minSizeParam;
            if (size.Height < minSizeParam) size.Height = minSizeParam;

            var renderTargetBitmap = new RenderTargetBitmap(
                (int)size.Width,
                (int)size.Height,
                _dpiParam, _dpiParam,
                PixelFormats.Pbgra32
            );
            //RenderOptions.SetEdgeMode(renderTargetBitmap, EdgeMode.Aliased);
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

        public SolidColorBrush GetColorToRepaint(SolidColorBrush color)
        {
            int r = maxColorParam - color.Color.R;
            int g = maxColorParam - color.Color.G;
            int b = maxColorParam - color.Color.B;

            return new SolidColorBrush(Color.FromArgb(byte.MaxValue
                , (byte)r, (byte)g, (byte)b));
        }

        public void CreateSelection(Shape shape, ref Selection selection,
        Canvas selParentCan, (Label lb, ScrollViewer scrol) selParams)
        {
            if (selection is null)
            {
                selection = new Selection(selParams.lb, selParams.scrol);
                selection.RenderTransform = new TranslateTransform(0, 0);

                Size size = new Size(shape.Width, shape.Height);

                selection.SelectionBorder.Height = size.Height;
                selection.SelectionBorder.Width = size.Width;

                _selection.Height = size.Height;
                _selection.Width = size.Width;

                _selection.SelectCan.Height = size.Height;
                _selection.SelectCan.Width = size.Width;

                double xLoc = (int)Canvas.GetLeft(shape);
                double yLoc = (int)Canvas.GetTop(shape);

                Canvas.SetLeft(selection, xLoc);
                Canvas.SetTop(selection, yLoc);
            }

            selParentCan.Children.Remove(shape);
            AddBgImageInChildren(selParentCan);
            selParentCan.Children.Add(selection);
        }

        public void MakeRectSelection(Rectangle rect, ref Selection selection,
        Canvas drawingCanvas, Rectangle checkRect, Label sizeLb, Image selImg)
        {
            IfSelectionIsMaken = true;
            if (IfSelectionSizeIsZero(drawingCanvas, sizeLb, selImg))
            {
                _selection = null;
                SetCanvasBg(drawingCanvas, checkRect);
                drawingCanvas.Children.Clear();
                return;
            }

            _firstSelectionStart = new Point((int)_firstSelectionStart.X, (int)_firstSelectionStart.Y);
            _firstSelectionEnd = new Point((int)_firstSelectionEnd.X, (int)_firstSelectionEnd.Y);
            _firstSelectionStart = new Point(_firstSelectionStart.X, _firstSelectionStart.Y);

            InitSelectedBgInRectCanvas(rect, selection, drawingCanvas, drawingCanvas);
        }

        public bool IfSelectionSizeIsZero(Canvas drawingCanvas, Label sizeLb, Image selImage)
        {
            if (IfSelectionSizeIsNotAcceptable())
            {
                _selection.IfSelectionIsClicked = false;
                _selection = null;
                _selectionLine = null;
                RemoveSelection(drawingCanvas, sizeLb, selImage);
                return true;
            }
            return false;
        }

        public void RemoveSelection(Canvas parent, Label sizeCont, Image selIcon)
        {
            _ifTransparencyIsActivated = false;
            SetTransparentSelectionImage(selIcon);

            for (int i = 0; i < parent.Children.Count; i++)
            {
                if (parent.Children[i] is Selection)
                {
                    parent.Children.RemoveAt(i);
                }
            }

            parent.Children.Remove(_selectionLine);
            sizeCont.Content = string.Empty;
        }

        public Image _tickImg;
        public void SetTransparentSelectionImage(Image selectionIcon)
        {
            if (_ifTransparencyIsActivated)
            {
                selectionIcon.Source = _tickImg.Source;
                return;
            }
            selectionIcon.Source = null;
        }

        public void SelLineCheck(Canvas drawingCanvas)
        {
            if (!drawingCanvas.Children.Contains(_selectionLine) && !(_selectionLine is null))
            {
                DependencyObject parent = VisualTreeHelper.GetParent(_selectionLine);
                if (parent is null) return;
                ((Canvas)_selectionLine.Parent).Children.Remove(_selectionLine);
            }
        }

        public void SetImageComporator(Canvas drawingCanvas)
        {
            SelLineCheck(drawingCanvas);
            drawingCanvas.RenderTransform = new TranslateTransform(0, 0);

            if (drawingCanvas.Background is SolidColorBrush)
            {
                _comparator = null;
                return;
            }
            drawingCanvas.Children.Remove(_selectionLine);

            _comparator = new Image()
            {
                Width = drawingCanvas.Width,
                Height = drawingCanvas.Height,
                Source = ((ImageBrush)drawingCanvas.Background).ImageSource,
                RenderTransform = new TranslateTransform(0, 0)
            };

            _comparator.RenderTransform = new TranslateTransform(0, 0);
            drawingCanvas.RenderTransform = new TranslateTransform(0, 0);

            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
            (int)drawingCanvas.Width, (int)drawingCanvas.Height,
            _dpiParam, _dpiParam, PixelFormats.Pbgra32);

            RenderOptions.SetEdgeMode(renderBitmap, EdgeMode.Aliased);

            drawingCanvas.Measure(new Size((int)drawingCanvas.Width, (int)drawingCanvas.Height));
            drawingCanvas.Arrange(new Rect(new Size((int)drawingCanvas.Width, (int)drawingCanvas.Height)));
            renderBitmap.Render(drawingCanvas);

            _comparator.Source = renderBitmap;
            if (!(_selectionLine is null))
            {
                drawingCanvas.Children.Add(_selectionLine);
            }

            drawingCanvas.RenderTransform = new TranslateTransform(_horizontalOffset, 0);
            _comparator.RenderTransform = new TranslateTransform(_horizontalOffset, 0);
        }

        public void RemoveAllChildrenExceptImages(Canvas canvas) //-, MORE LINQ 
        {
            var childrenToRemove = canvas.Children
                                 .OfType<UIElement>()
                                 .Where(child => child.GetType() != typeof(Image))
                                 .ToList();


            childrenToRemove.ForEach(el => canvas.Children.Remove(el));

            /*            foreach (var child in childrenToRemove)
                        {
                            if (!(child is null))
                            {
                                canvas.Children.Remove((UIElement)child);
                            }
                        }*/
        }

        public List<UIElement> GetAllUIElementsExceptImages(Canvas canvas)
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

        public void InitImageAfterMakingCustomSelection(Canvas drawingCanvas, Rectangle checkRect)
        {
            List<UIElement> elems = GetAllUIElementsExceptImages(drawingCanvas);
            RemoveAllChildrenExceptImages(drawingCanvas);

            SetCanvasBg(drawingCanvas, checkRect);
            InitElementsInCanvas(drawingCanvas, elems);
        }

        public bool CheckForLittleSelectionLine()
        {
            const int maxDifferParam = 2;

            double xMin = _selectionLine.Points.Min(p => p.X);
            double xMax = _selectionLine.Points.Max(p => p.X);
            double yMin = _selectionLine.Points.Min(p => p.Y);
            double yMax = _selectionLine.Points.Max(p => p.Y);

            Point min = new Point(xMin, yMin);
            Point max = new Point(xMax, yMax);
            Point differ = new Point(Math.Abs(max.X - min.X), Math.Abs(max.Y - min.Y));
            return (differ.X < maxDifferParam ||
               differ.Y < maxDifferParam);
        }

        public void ConnectTwoPoints()
        {
            Point firstPoint = _selectionLine.Points.Last();
            Point lastPoint = _selectionLine.Points.First();

            if (firstPoint.Equals(lastPoint)) return;

            List<(int, int)> points = GetLinePoints((int)firstPoint.X,
                (int)firstPoint.Y, (int)lastPoint.X, (int)lastPoint.Y);
            for (int i = 0; i < points.Count; i++)
            {
                _selectionLine.Points.Add(new Point(points[i].Item1, points[i].Item2));
            }
        }

        public bool IfPointsAreCorrect()
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

        public void SetPolylineLocation(Polyline polyline, Canvas drawingCanvas)
        {
            GeneralTransform transform = polyline.TransformToAncestor(drawingCanvas);
            Point canvasPosition = transform.Transform(new Point(0, 0));

            Canvas.SetLeft(polyline, canvasPosition.X);
            Canvas.SetTop(polyline, canvasPosition.Y);
        }

        public void ChangePointCordesAfterChenageLineSize()
        {
            const int minSizeParam = 1;
            if (_selectionLine is null) return;

            double minX = _selectionLine.Points.Min(p => p.X);
            double minY = _selectionLine.Points.Min(p => p.Y);
            double maxX = _selectionLine.Points.Max(p => p.X);
            double maxY = _selectionLine.Points.Max(p => p.Y);

            const double sizeCorrelParam = 0;

            double originalWidth = maxX - minX + sizeCorrelParam;
            double originalHeight = maxY - minY + sizeCorrelParam;
            double newWidth = _selectionLine.Width;
            double newHeight = _selectionLine.Height;

            double scaleX;
            double scaleY;

            if (Math.Abs(newWidth - originalWidth) <= minSizeParam && Math.Abs(newHeight - originalHeight) <= minSizeParam)
                return;

            scaleX = newWidth / originalWidth;
            scaleY = newHeight / originalHeight;

            PointCollection asd = _selectionLine.Points;

            for (int i = 0; i < _selectionLine.Points.Count; i++)
            {
                Point oldPoint = _selectionLine.Points[i];
                double newX = (oldPoint.X - minX) * scaleX + minX;
                double newY = (oldPoint.Y - minY) * scaleY + minY;
                _selectionLine.Points[i] = new Point((int)(newX), (int)(newY));
            }
            _selectionLine.InvalidateVisual();
        }

        public void RemoveEqualPoints(Polyline line) //-, linq
        {
            //PointCollection points = new PointCollection(
            //line.Points.Where((point, index) => index == 0 || line.Points[index - 1] != point));

            PointCollection points = line.Points
            .Aggregate(new PointCollection(), (acc, point) =>
            {
                if (acc.Count == 0 || acc.Last() != point)
                {
                    acc.Add(point);
                }
                return acc;
            });

            /* PointCollection points = new PointCollection();
             for (int i = 0; i < line.Points.Count; i++)
             {
                 if (points.Count == 0 || points.Last() != line.Points[i])
                 {
                     points.Add(line.Points[i]);
                 }
             }*/
            line.Points = points;
        }

        public delegate bool Check(Point globalPoint, Canvas drawingCanvas);

        public bool DelMethod(Point globalPoint, Canvas drawingCanvas)
        {
            //left side
            if (globalPoint.X < 0)
            {
                CutLeftPartOfCustomSelection(globalPoint);
                globalPoint = new Point(Canvas.GetLeft(_selectionLine), Canvas.GetTop(_selectionLine));
                Canvas.SetLeft(_selection, 0);
                return true;
            }
            //Top Side
            if (globalPoint.Y < 0)
            {
                CutTopPartOfCustomSelection(globalPoint);
                globalPoint = new Point(Canvas.GetLeft(_selectionLine), Canvas.GetTop(_selectionLine));
                Canvas.SetTop(_selection, 0);
                return true;
            }
            //Right side 
            if (globalPoint.X + _selection.Width > drawingCanvas.Width)
            {
                CutRightPartOfCustomSelection(globalPoint, drawingCanvas.Width);
                globalPoint = new Point(Canvas.GetLeft(_selectionLine), Canvas.GetTop(_selectionLine));
                return true;
            }
            //bottom side
            if (globalPoint.Y + _selection.Height > drawingCanvas.Height)
            {
                CutBottomPartOfCustomSelection(globalPoint, drawingCanvas.Height);
                return true;
            }
            return false;
        }

        public bool CutFromCustomSelection(Point globalPoint, Canvas drawingCanvas) //-, try to use delegate
        {

            Check del = new Check(DelMethod);

            RemoveEqualPoints(_selectionLine);
            //bool res = false;

            /*            double minX = _selectionLine.Points.Min(x => x.X);
                        double maxX = _selectionLine.Points.Max(x => x.X);
                        double minY = _selectionLine.Points.Min(y => y.Y);
                        double maxY = _selectionLine.Points.Max(y => y.Y);

                        double totWidth = maxX + Math.Abs(minX);
                        double totHeigth = maxY + Math.Abs(minY);*/

            return del(globalPoint, drawingCanvas);

            /* //left side
             if (globalPoint.X < 0)
             {
                 CutLeftPartOfCustomSelection(globalPoint);
                 globalPoint = new Point(Canvas.GetLeft(_selectionLine), Canvas.GetTop(_selectionLine));
                 Canvas.SetLeft(_selection, 0);
                 res = true;
             }
             //Top Side
             if (globalPoint.Y < 0)
             {
                 CutTopPartOfCustomSelection(globalPoint);
                 globalPoint = new Point(Canvas.GetLeft(_selectionLine), Canvas.GetTop(_selectionLine));
                 Canvas.SetTop(_selection, 0);
                 res = true;
             }
             //Right side 
             if (globalPoint.X + _selection.Width > drawingCanvas.Width)
             {
                 CutRightPartOfCustomSelection(globalPoint, drawingCanvas.Width);
                 globalPoint = new Point(Canvas.GetLeft(_selectionLine), Canvas.GetTop(_selectionLine));
                 res = true;
             }
             //bottom side
             if (globalPoint.Y + _selection.Height > drawingCanvas.Height)
             {
                 CutBottomPartOfCustomSelection(globalPoint, drawingCanvas.Height);
                 res = true;
             }*/
            //return res;
        }

        public void CutLeftPartFromSelection(Selection selection, Point globalPoint)
        {
            double widthDiffer = selection.Width + globalPoint.X;//(globalPoint.X is NEGATIVE)
            //need to remove selection
            if (widthDiffer <= 0) return;

            //Cut part of it
            Canvas.SetLeft(selection, 0);

            selection.Width += globalPoint.X;
            selection.SelectCan.Width += globalPoint.X;
        }

        public void CutRightPartFromSelection(Selection selection, Point globalPoint, Canvas drawingCanvas)
        {
            //Need To Remove Selection
            if (globalPoint.X >= drawingCanvas.Width) return;

            Canvas.SetLeft(selection, globalPoint.X);
            double distToCut = selection.Width + globalPoint.X - drawingCanvas.Width;

            selection.Width -= distToCut;
            selection.SelectCan.Width -= distToCut;
        }

        public void CutTopPartFromSelection(Selection selection, Point globalPoint)
        {
            //to remove selection
            if (globalPoint.Y + selection.Height <= 0) return;
            Canvas.SetTop(selection, 0);
            selection.Height += globalPoint.Y;
            selection.SelectCan.Height += globalPoint.Y;
        }

        public void CutBottomPartFromSelection(Selection selection, Point globalPoint, Canvas drawingCanvas)
        {
            //to remove selection
            if (globalPoint.Y >= drawingCanvas.Height) return;

            Canvas.SetTop(selection, globalPoint.Y);
            double differ = globalPoint.Y + selection.Height - drawingCanvas.Height;

            selection.Height -= differ;
            selection.SelectCan.Height -= differ;
        }

        public void CutFromUsualSelection(Selection selection, Point globalPoint, Canvas drawingCanvas)
        {
            //Check sides
            //Left side
            if (globalPoint.X < 0)
            {
                CutLeftPartFromSelection(selection, globalPoint);
            }
            //right side
            if (globalPoint.X + selection.Width > drawingCanvas.Width)
            {
                CutRightPartFromSelection(selection, globalPoint, drawingCanvas);
            }
            //top Side
            if (globalPoint.Y < 0)
            {
                CutTopPartFromSelection(selection, globalPoint);
            }
            //bottom side
            if (globalPoint.Y + selection.Height > drawingCanvas.Height)
            {
                CutBottomPartFromSelection(selection, globalPoint, drawingCanvas);
            }
        }

        public bool CheckSizes(Selection selection, Point globalPoint,
            bool ifCustomSelection, Canvas drawingCanvas)
        {
            if (ifCustomSelection)
            {
                ChangePointCordesAfterChenageLineSize();
                bool asd = CutFromCustomSelection(globalPoint, drawingCanvas);

                return asd;
            }
            CutFromUsualSelection(selection, globalPoint, drawingCanvas);
            return false;
        }

        public bool IfCustomSelectionIsCreated(Polyline polyline, Canvas drawingCanvas,
            Label selSizeLB, Image selImage, ScrollViewer scroll, bool ifInvert = false)
        {
            /*            double minX = double.MaxValue;
                        double minY = double.MaxValue;
                        double maxX = double.MinValue;
                        double maxY = double.MinValue;*/

            double minX = _selectionLine.Points.Min(x => x.X);
            double maxX = _selectionLine.Points.Max(x => x.X);
            double minY = _selectionLine.Points.Min(y => y.Y);
            double maxY = _selectionLine.Points.Max(y => y.Y);


            //Correct point scale
            if (ifInvert)
            {
                ChangePointCordesAfterChenageLineSize();
            }
            /*            foreach (var point in polyline.Points)
                        {
                            if (point.X < minX) minX = point.X;
                            if (point.Y < minY) minY = point.Y;
                            if (point.X > maxX) maxX = point.X;
                            if (point.Y > maxY) maxY = point.Y;
                        }*/
            double pointCorrel = 0;// ifInvert ? 2 : 2;
            double polylineWidth = maxX - minX + pointCorrel;
            double polylineHeight = maxY - minY + pointCorrel;
            const int correlParam = 1;
            Size selSize = new Size(polylineWidth + correlParam, polylineHeight + correlParam);

            //create selection
            _selection = new Selection(selSizeLB, scroll)
            {
                Width = selSize.Width,
                Height = selSize.Height,
            };

            double selectionLocCorrelation = 0;// _selection.DashedBorder.StrokeThickness;
            _selection.SelectionBorder.Width = selSize.Width;
            _selection.SelectionBorder.Height = selSize.Height;

            if (IfSelectionSizeIsZero(drawingCanvas, selSizeLB, selImage)) return false;

            //set poly loc
            int xLoc = (int)Canvas.GetLeft(polyline) + (int)minX;
            int yLoc = (int)Canvas.GetTop(polyline) + (int)minY;

            Point polyPoint = new Point(Canvas.GetLeft(_selectionLine), Canvas.GetTop(_selectionLine));

            // set loc if invertion or selection cut
            if (!ifInvert && !_ifCutCheck)
            {
                Canvas.SetLeft(_selection, xLoc - selectionLocCorrelation);
                Canvas.SetTop(_selection, yLoc - selectionLocCorrelation);
            }
            else
            {
                Canvas.SetLeft(_selection, polyPoint.X);
                Canvas.SetTop(_selection, polyPoint.Y);
            }

            drawingCanvas.Children.Remove(_selectionLine);
            AddBgImageInChildren(drawingCanvas);
            drawingCanvas.Children.Add(_selection);

            return true;
        }

        public void AddSelectionLineInSelection(Polyline polyline, bool ifInverted, Canvas drawingCanvas)//-, dubl
        {
            drawingCanvas.Children.Remove(polyline);
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

            if (!ifInverted && !_ifCutCheck)
            {
                Canvas.SetLeft(polyline, 0 - minXS);
                Canvas.SetTop(polyline, 0 - minYS);
            }

            SetSelectionCanvasesSize(new Size(_selection.Width, _selection.Height));

            /*            _selection.SelectCan.Width = _selection.Width;
                        _selection.SelectCan.Height = _selection.Height;
                        _selection.CheckCan.Width = _selection.Width;
                        _selection.CheckCan.Height = _selection.Height;
                        _selection.BgCanvas.Width = _selection.Width;
                        _selection.BgCanvas.Height = _selection.Height;*/
        }
        public Image MakeComporation(Image toCompare)
        {
            /*            const int pixStep = 4;
                        const int getGStep = 1;
                        const int getRStep = 2;
                        const int getAlphaStep = 3;
                        const int bitsAdd = 7;
                        const int bitsInByte = 8;*/

            if (_comparator is null) return toCompare;

            Point selLoc = new Point(Canvas.GetLeft(_selection), Canvas.GetTop(_selection));

            BitmapSource bitmapSource = toCompare.Source as BitmapSource;
            WriteableBitmap writeableBitmap = new WriteableBitmap(bitmapSource);

            int width = writeableBitmap.PixelWidth;
            int height = writeableBitmap.PixelHeight;
            int stride = width * ((writeableBitmap.Format.BitsPerPixel + bitsAdd) / bitsInByte);
            byte[] pixels = new byte[height * stride];
            writeableBitmap.CopyPixels(pixels, stride, 0);

            BitmapSource globalImgBitMaoSource = _comparator.Source as BitmapSource;
            WriteableBitmap writeableBitmapGlobalSource = new WriteableBitmap(globalImgBitMaoSource);

            int widthGlobal = writeableBitmapGlobalSource.PixelWidth;
            int heightGlobal = writeableBitmapGlobalSource.PixelHeight;
            int strideGlobal = widthGlobal * ((writeableBitmapGlobalSource.Format.BitsPerPixel + bitsAdd) / bitsInByte);
            byte[] pixelsGlobal = new byte[heightGlobal * strideGlobal];
            writeableBitmapGlobalSource.CopyPixels(pixelsGlobal, strideGlobal, 0);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Point tempPoint = new Point(selLoc.X + x,
                        selLoc.Y + y);
                    int indexGlobal = (int)tempPoint.Y * strideGlobal + pixStep * (int)tempPoint.X;

                    if (indexGlobal + pixStep < pixelsGlobal.Length && indexGlobal > 0)
                    {
                        (byte blue, byte green, byte red, byte alpha) globalRGB = (pixelsGlobal[indexGlobal],
                            pixelsGlobal[indexGlobal + getGStep], pixelsGlobal[indexGlobal + getRStep], pixelsGlobal[indexGlobal + getAlphaStep]);

                        int index = y * stride + pixStep * x;

                        byte blue = pixels[index];
                        byte green = pixels[index + getGStep];
                        byte red = pixels[index + getRStep];
                        byte alpha = pixels[index + getAlphaStep];

                        if (alpha != 0 && (globalRGB.blue != blue || globalRGB.green != green ||
                            globalRGB.red != red || globalRGB.alpha != alpha))
                        {
                            pixels[index] = globalRGB.blue;
                            pixels[index + getGStep] = globalRGB.green;
                            pixels[index + getRStep] = globalRGB.red;
                            pixels[index + getAlphaStep] = globalRGB.alpha;
                        }
                    }
                }
            }

            writeableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
            toCompare.Source = writeableBitmap;
            return toCompare;
        }

        public Image RepaintImageInWhite(Image originalImage, Color targetColor)
        {
            /*            const int pixStep = 4;
                        const int getGStep = 1;
                        const int getRStep = 2;
                        const int getAlphaStep = 3;
                        const int bitsAdd = 7;
                        const int bitsInByte = 8;
            */
            originalImage.RenderTransform = new TranslateTransform(0, 0);
            BitmapSource bitmapSource = originalImage.Source as BitmapSource;
            WriteableBitmap writeableBitmap = new WriteableBitmap(bitmapSource);

            RenderOptions.SetEdgeMode(writeableBitmap, EdgeMode.Aliased);
            int width = writeableBitmap.PixelWidth;
            int height = writeableBitmap.PixelHeight;
            int stride = width * ((writeableBitmap.Format.BitsPerPixel + bitsAdd) / bitsInByte);
            byte[] pixels = new byte[height * stride];
            writeableBitmap.CopyPixels(pixels, stride, 0);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * stride + pixStep * x;
                    byte alpha = pixels[index + getAlphaStep];
                    if (alpha > 0)
                    {
                        pixels[index] = targetColor.B;
                        pixels[index + getGStep] = targetColor.G;
                        pixels[index + getRStep] = targetColor.R;
                        pixels[index + getAlphaStep] = targetColor.A;
                    }
                }
            }

            writeableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
            originalImage.Source = writeableBitmap;
            originalImage.RenderTransform = new TranslateTransform(_horizontalOffset, 0);

            return originalImage;
        }

        public void SetSelectionCanBgASImage(Image image, Selection selection, Rectangle checkRect)
        {
            const int xCutMove = 1;

            ImageBrush imageBrush = new ImageBrush();
            RenderOptions.SetEdgeMode(imageBrush, EdgeMode.Aliased);

            imageBrush.ImageSource = image.Source;
            imageBrush.Stretch = Stretch.Fill;

            selection.BgCanvas.Children.Add(image);

            Canvas lineParent = null;
            if (!(_selectionLine is null))
            {
                lineParent = (Canvas)_selectionLine.Parent;
                lineParent.Children.Remove(_selectionLine);
            }

            SetCanvasBg(selection.BgCanvas, checkRect);
            double xMoveCorrel = _ifCutCheck ? xCutMove : 0;
            const double yMoveCorrel = 0;

            if (selection.BgCanvas.Background is ImageBrush imgBrush)
            {
                imgBrush.Transform = new TranslateTransform()
                {
                    X = xMoveCorrel,
                    Y = yMoveCorrel
                };
            }
            selection.BgCanvas.Children.Clear();
            if (!(_selectionLine is null)) lineParent.Children.Add(_selectionLine);
        }

        public void InitSelectionBgInCustomCanvas(bool ifInvertion,
            Canvas drawingCanvas, Rectangle checkRect)
        {
            drawingCanvas.Children.Remove(_selectionLine);
            drawingCanvas.Children.Remove(_selection);
            Image img = GetRenderOfCustomCanvasVTwo(_selectionLine, drawingCanvas, ifInvertion);

            if (img is null)
            {
                return;
            }

            Image check = CloneImage(img);
            img = MakeComporation(img);
            img = SwipeColorsInImage(img, _whiteColor, _transparentColor);

            check = RepaintImageInWhite(check, _whiteColor);
            PaintInCanvas(drawingCanvas, check);
            drawingCanvas.Children.Add(_selection);
            SetSelectionCanBgASImage(img, _selection, checkRect);
        }

        private void ResetRenderTransform(Canvas canvas)
        {
            canvas.RenderTransform = new TranslateTransform(0, 0);
        }

        private void UpdateRenderTransform(Canvas canvas)
        {
            canvas.RenderTransform = new TranslateTransform(_horizontalOffset, 0);
        }

        public bool MakeCustomSelection(Canvas drawingCanvas, Label selSize,
            Image selImage, ScrollViewer scrol, Rectangle checkRect, bool ifInvertial = false)
        {
            //if selLine is alredy exist 
            if (!ifInvertial && CheckForLittleSelectionLine())
            {
                drawingCanvas.Children.Remove(_selectionLine);
                return false;
            }

            ResetRenderTransform(drawingCanvas);

            //Add Last point(to make like a polygon)
            if (_selectionLine is null || _selectionLine.Points.Count == 0) return false;

            //Get all point between first and last
            ConnectTwoPoints();
            _selectionLine.Points.Add(_selectionLine.Points.First());

            if (!IfPointsAreCorrect()) return false;

            //Set polylines location
            if (!ifInvertial && !_ifCutCheck) SetPolylineLocation(_selectionLine, drawingCanvas);

            //Selection Creation
            if (!IfCustomSelectionIsCreated(_selectionLine, drawingCanvas,
                selSize, selImage, scrol, ifInvertial)) return false;

            //Add line in selection
            AddSelectionLineInSelection(_selectionLine, ifInvertial, drawingCanvas);

            if (_ifCutCheck)
            {
                Canvas.SetTop(_selectionLine, 0);
                Canvas.SetLeft(_selectionLine, 0);
            }
            if (_ifCutCheck)
            {
                _selectionLine.Stretch = Stretch.None;
            }

            //set loc for child params 
            Canvas.SetLeft(_selection.CheckCan, 0);
            Canvas.SetTop(_selection.CheckCan, 0);
            Canvas.SetLeft(_selection.SelectCan, 0);
            Canvas.SetTop(_selection.SelectCan, 0);

            //Check if selection is out of boundaries 
            InitSelectionBgInCustomCanvas(ifInvertial, drawingCanvas, checkRect);
            if (_selectionLine is null)
            {
                _selection.CheckCan.Children.Remove(_selectionLine);
                FreeSelection(drawingCanvas, checkRect);
                UpdateRenderTransform(drawingCanvas);
                return false;
            };

            //Set line in checkCan
            drawingCanvas.Children.Remove(_selectionLine);
            _selection.SelectCan.Children.Remove(_selectionLine);
            _selection.CheckCan.Children.Add(_selectionLine);
            _selectionLine.Stretch = Stretch.Fill;

            //Set line location
            Canvas.SetLeft(_selectionLine, 0);
            Canvas.SetTop(_selectionLine, 0);

            IfSelectionIsMaken = true;

            Point point = new Point(Canvas.GetLeft(_selection), Canvas.GetTop(_selection));
            //int selLocCorrelX = _ifCutCheck ? 0 : 0;

            Canvas.SetLeft(_selection, point.X /*+ selLocCorrelX*/);
            Canvas.SetTop(_selection, point.Y);

            UpdateRenderTransform(drawingCanvas);
            return true;
        }

        public Image GetImageOfFreedSeletcions(Selection highest, Canvas drawingCanvas, bool ifClear = false)
        {
            if (highest is null) return null;

            //make size grid and dashBorders invisable
            SetVisabilityForSelectionBorders(drawingCanvas, Visibility.Hidden);

            //set drawing Canvas as Image
            Image res = ConvertBackgroundToImage(drawingCanvas);

            // make grid and border visiable
            if (!ifClear) SetVisabilityForSelectionBorders(drawingCanvas, Visibility.Visible);

            //clear every selection bg
            ClearBgForSelectionInDeep(highest);
            drawingCanvas.Background = new SolidColorBrush(Colors.Transparent);
            return res;
        }

        public void SetSelectionInCanvas(Canvas drawingCanvas, Rectangle checkRect)
        {
            _selection.DashedBorder.Visibility = Visibility.Hidden;
            _selection.SizingGrid.Visibility = Visibility.Hidden;

            SetCanvasBg(drawingCanvas, checkRect);

            drawingCanvas.Children.Remove(_selection);
            _selection = null;
        }

        public void FreeSelection(Canvas drawingCanvas, Rectangle checkRect)
        {
            if (_selection is null) return;
            ResetRenderTransform(drawingCanvas);

            if (_selection is null)
            {
                UpdateRenderTransform(drawingCanvas);
                return;
            }

            if (_selectionType == SelectionType.Invert ||
                GetAmountOfSelection(0, drawingCanvas.Children.OfType<Selection>().FirstOrDefault()))
            {
                _selectionType = SelectionType.Rectangle;

                Image bgImg = GetImageOfFreedSeletcions(drawingCanvas.Children.OfType<Selection>().FirstOrDefault(), drawingCanvas, true);
                drawingCanvas.Children.Clear();

                drawingCanvas.Background = new ImageBrush() { ImageSource = bgImg.Source };
                _selection = null;
                return;
            }

            SetSelectionInCanvas(drawingCanvas, checkRect);
            UpdateRenderTransform(drawingCanvas);
        }

        public bool GetAmountOfSelection(int counter, Selection selection)
        {
            if (selection is null) return counter > 0;
            Selection next = null;

            for (int i = 0; i < selection.SelectCan.Children.Count; i++)
            {
                if (selection.SelectCan.Children[i] is Selection sel)
                {
                    next = sel;
                }
            }

            if (!(next is null))
            {
                counter++;
            }
            else return counter > 0;

            return GetAmountOfSelection(counter, next);
        }

        public void AddListOfElemsWithoutImages(List<UIElement> elems, Canvas canvas)
        {
            for (int i = 0; i < elems.Count; i++)
            {
                if (!(elems[i] is Image))
                {
                    canvas.Children.Add(elems[i]);
                }
            }
        }

        public Selection GetHighestSelection(Canvas drawingCanvas)
        {
            for (int i = 0; i < drawingCanvas.Children.Count; i++)
            {
                if (drawingCanvas.Children[i].GetType() == typeof(Selection))
                {
                    return ((Selection)drawingCanvas.Children[i]);
                }
            }
            return null;
        }

        public Selection GetTheDeepestSelection(Canvas canvas, ref Selection res)
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

        public void MakeSelection(Canvas drawingCanvas, Label selSizeLB, Image selImage,
            Rectangle checkRect, ScrollViewer scrol, bool ifContainsLine = false)
        {
            ResetRenderTransform(drawingCanvas);
            _ifTransparencyIsActivated = false;
            SetTransparentSelectionImage(selImage);

            if (!(_copyPolyLine is null) && !(_selection is null)) _selection.CheckCan.Children.Remove(_copyPolyLine);

            //make square selection
            if (_selectionType == SelectionType.Rectangle ||
                (_selectionType == SelectionType.Invert && !ifContainsLine))
            {
                drawingCanvas.Children.Remove(_selectionLine);
                _selectionLine = null;

                CreateSelection(_selectionRect, ref _selection,
                    drawingCanvas, (selSizeLB, scrol));
                MakeRectSelection(_selectionRect, ref _selection,
                    drawingCanvas, checkRect, selSizeLB, selImage);
            }
            //make custom selection
            else if (_selectionType == SelectionType.Custom ||
                (_selectionType == SelectionType.Invert && ifContainsLine))
            {
                SetImageComporator(drawingCanvas);
                MakeCustomSelection(drawingCanvas, selSizeLB, selImage,
                    scrol, checkRect, ifContainsLine);
                InitImageAfterMakingCustomSelection(drawingCanvas, checkRect);
            }
            UpdateRenderTransform(drawingCanvas);
        }

        public Point GetSelectionPointComparedToDrawingCanvas(UIElement element, Point tempRes, Canvas drawingCanvas)
        {
            if (element == drawingCanvas)
            {
                return tempRes;
            }
            else if ((element.GetType() == typeof(Selection) &&
                ((Selection)element).Parent == drawingCanvas))
            {
                Point selElemPoint = new Point(Canvas.GetLeft(element), Canvas.GetTop(element));
                Point pointToReturn = (selElemPoint.X < 0 && selElemPoint.Y < 0) ? selElemPoint :
                    new Point(selElemPoint.X + tempRes.X, selElemPoint.Y + tempRes.Y);
                return pointToReturn;
            }

            //get selection 
            Selection sel = null;
            if (element.GetType() == typeof(Selection))
            {
                sel = GetSelectionsSelectionParent((Canvas)((Selection)element).Parent);
            }

            Selection selElem = element as Selection;
            Point tempSelPoint = new Point(Canvas.GetLeft(selElem), Canvas.GetTop(selElem));

            //if out of range, need comparation with start sel point(how to find it)
            tempRes = GetSelectionPointComparedToDrawingCanvas(sel, tempRes, drawingCanvas);

            Point toSendInDeep = (tempRes.X < 0 && tempRes.Y < 0 && tempSelPoint.X < 0 && tempSelPoint.Y < 0) ? tempRes :
                (tempRes.X < 0 && tempRes.Y < 0 && tempSelPoint.X > 0 && tempSelPoint.Y > 0) ? tempSelPoint :
                new Point(tempSelPoint.X + tempRes.X, tempSelPoint.Y + tempRes.Y);
            return toSendInDeep;
        }

        public void InvertSelection(Canvas drawingCanvas, Rectangle checkRect)
        {
            const int startLocParam = -1;

            //Get the highest selection
            Selection highest = GetHighestSelection(drawingCanvas);

            //Get temp location SystemSelection
            Point localLoc = new Point(Canvas.GetLeft(_selection), Canvas.GetTop(_selection));

            //Get global location of SystemLocation
            Point selectionPoint =
                GetSelectionPointComparedToDrawingCanvas(_selection, new Point(0, 0), drawingCanvas);

            if (selectionPoint.X < startLocParam && selectionPoint.Y == startLocParam &&
               _selection.Width == drawingCanvas.Width &&
               _selection.Height == drawingCanvas.Height)
            {
                selectionPoint = new Point(startLocParam, startLocParam);
            }

            //Get SystemSelection Children + delete them
            List<UIElement> systemSelChildren =
                ReAssignChildrenInAuxiliaryCanvas(_selection.SelectCan);
            _selection.SelectCan.Children.Clear();

            //Get SystemSelection parent Canvas
            Canvas parentCan = (Canvas)_selection.Parent;

            //Remove SystemSelection from parent canvas
            parentCan.Children.Remove(_selection);

            //Clear drawingCanvas children
            drawingCanvas.Children.Clear();

            //Add SystemSelection to DrawingCanvas
            drawingCanvas.Children.Add(_selection);

            // Init global loc to SystemSelection
            Canvas.SetLeft(_selection, selectionPoint.X);
            Canvas.SetTop(_selection, selectionPoint.Y);

            //Get image BG of SystemSelection + Make BG in SystemSelection.SelectCan Transparent
            Image systemSelBgImg = ConvertCanvasInImage(_selection.BgCanvas);
            systemSelBgImg = SwipeColorsInImage(systemSelBgImg, _whiteColor, _transparentColor);

            //Add image in DrawingCanvas
            drawingCanvas.Children.Add(systemSelBgImg);

            //Set location to image
            Canvas.SetLeft(systemSelBgImg, selectionPoint.X);
            Canvas.SetTop(systemSelBgImg, selectionPoint.Y);

            //Remove SystemSelection from DrawingCanvas
            drawingCanvas.Children.Remove(_selection);

            SetCanvasBg(drawingCanvas, checkRect);

            //Add Children of SystemSelection to SystemSelection children
            InitElementsInCanvas(_selection.SelectCan, systemSelChildren);

            //Add SystemSelection to parent canvas
            parentCan.Children.Add(_selection);

            //Set SystemSelection Local location
            Canvas.SetLeft(_selection, localLoc.X);
            Canvas.SetTop(_selection, localLoc.Y);

            //Add highest to Drawing Canvas
            drawingCanvas.Children.Add(highest);


            //set bg tracperency for children elements 
            _selection.BgCanvas.Background = new SolidColorBrush(Colors.Transparent);
            _selection.CheckCan.Background = new SolidColorBrush(Colors.Transparent);
            _selection.Background = new SolidColorBrush(Colors.Transparent);
            _selection.BgCanvas.Background = new SolidColorBrush(Colors.Transparent);
        }

        public void SetSelectionCanvasesSize(Size size) //- dubl + takoi method uje gde to bil(ili prosto kod)
        {

            _selection.SelectionBorder.Width = size.Width;
            _selection.SelectionBorder.Height = size.Height;

            _selection.SelectCan.Width = size.Width;// _selection.Width;
            _selection.SelectCan.Height = size.Height;// _selection.Height;

            _selection.CheckCan.Width = size.Width;// _selection.Width;
            _selection.CheckCan.Height = size.Height;//_selection.Height;

            _selection.BgCanvas.Width = size.Width;// _selection.Width;
            _selection.BgCanvas.Height = size.Height;// _selection.Height;
        }

        public void ChangeWhiteSelectionPartInSelection()
        {
            //get SystemSelections Children + clear them
            List<UIElement> elems = ReAssignChildrenInAuxiliaryCanvas(_selection.SelectCan);
            _selection.SelectCan.Children.Clear();

            //swap white with Transparent in SystemSelection            
            Image selBgImg = ConvertCanvasInImage(_selection.BgCanvas);
            Image recImg = SwipeColorsInImage(selBgImg, _whiteColor, _transparentColor);

            if (recImg is null) return;

            //Re init BG
            _selection.BgCanvas.Background = new ImageBrush()
            {
                ImageSource = recImg.Source
            };

            //Set SystemsSelection children back
            InitElementsInCanvas(_selection.SelectCan, elems);
        }

        public void MakeInvertSelection(Canvas canvas, Canvas drawingCanvas, Rectangle checkRect, bool ifMakeInvalidation = true)
        {
            for (int i = 0; i < canvas.Children.Count; i++)
            {
                if (canvas.Children[i].GetType() == typeof(Selection))
                {
                    _selection = (Selection)canvas.Children[i];
                    SetSelectionCanvasesSize(new Size(_selection.Width, _selection.Height));
                    if (!ifMakeInvalidation)
                    {
                        InvertSelection(drawingCanvas, checkRect);

                        ChangeWhiteSelectionPartInSelection();
                        MakeInvertSelection(_selection.SelectCan, drawingCanvas, checkRect, !ifMakeInvalidation);
                    }
                    else
                    {
                        ChangeWhiteSelectionPartInSelection();
                        MakeInvertSelection(_selection.SelectCan, drawingCanvas, checkRect, !ifMakeInvalidation);
                    }
                }
            }
        }

        public void SetHighestSelectionInSystemSelection(Canvas drawingCanvas)
        {
            for (int i = 0; i < drawingCanvas.Children.Count; i++)
            {
                if (drawingCanvas.Children[i].GetType() == typeof(Selection))
                {
                    _selection = ((Selection)drawingCanvas.Children[i]);
                    return;
                }
            }
        }

        public bool SetSelectionInWholeDrawingCanvas(Label selSizeLb, ScrollViewer scrol, Canvas drawingCanvas) //-, dbul
        {
            _selection = new Selection(selSizeLb, scrol)
            {
                Height = drawingCanvas.Height,
                Width = drawingCanvas.Width
            };


            SetSelectionCanvasesSize(new Size(drawingCanvas.Width, drawingCanvas.Height));

            /*            _selection.SelectionBorder.Height = drawingCanvas.Height;
                        _selection.SelectionBorder.Width = drawingCanvas.Width;

                        _selection.SelectCan.Width = drawingCanvas.Width;
                        _selection.SelectCan.Height = drawingCanvas.Height;

                        _selection.BgCanvas.Width = drawingCanvas.Width;
                        _selection.BgCanvas.Height = drawingCanvas.Height;*/

            Canvas.SetTop(_selection, 0);
            Canvas.SetLeft(_selection, 0);

            _firstSelectionStart = new Point(0, 0);
            _firstSelectionEnd = new Point(drawingCanvas.Width, drawingCanvas.Height);

            if (_selectionRect is null)
            {
                const int strokeThic = 2;
                _selectionRect = new Rectangle()
                {
                    Stroke = Brushes.LightBlue,
                    StrokeThickness = strokeThic,
                    Fill = Brushes.Transparent
                };
            }

            _selectionRect.Height = drawingCanvas.Height;
            _selectionRect.Width = drawingCanvas.Width;

            Canvas.SetLeft(_selectionRect, 0);
            Canvas.SetTop(_selectionRect, 0);
            return true;
        }

        public void SelectAllDrawingSelection(Label selSizeLb, ScrollViewer scrol, Canvas drawingCanvas, Rectangle checkRect)
        {
            drawingCanvas.Children.Clear();
            if (!SetSelectionInWholeDrawingCanvas(selSizeLb, scrol, drawingCanvas)) return;

            Image BG = ConvertCanvasInImage(drawingCanvas);
            drawingCanvas.Background = new SolidColorBrush(Colors.White);
            drawingCanvas.Children.Add(_selection);


            BG = SwipeColorsInImage(BG, _whiteColor, _transparentColor);
            _selection.BgCanvas.Children.Add(BG);
            //Point imgPoint = new Point(Canvas.GetLeft(_selection), Canvas.GetTop(_selection));
            SetCanvasBg(_selection.BgCanvas, checkRect);

            _selection.IfSelectionIsClicked = false;
        }

        public Point GetMinPointParamsInPolyline(Polyline polyline)
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

        public void AddListElemsInCanvas(List<UIElement> elems, Canvas canvas)
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

        public void InitBGForSelectionFromDeepest(Selection selection, Canvas drawingCanvas,
            Label selSizeLB, Image selImage, Rectangle checkRect, ScrollViewer scrol)
        {
            double minX;
            double maxX;
            double minY;
            double maxY;

            double totWidth;
            double totHeigth;

            double selWidth;
            double selHeight;

            if (!(_selectionLine is null))
            {
                Point startPointLoc = GetPointOfSelection(_selectionLine, drawingCanvas);
                Point minPointParams = GetMinPointParamsInPolyline(_selectionLine);
                Point resCheck = new Point(startPointLoc.X + Math.Abs(minPointParams.X), startPointLoc.Y + Math.Abs(minPointParams.Y));
                if (selection.CheckCan.Children.Contains(_selectionLine))
                {
                    selLineLoc = new Point(resCheck.X, resCheck.Y);
                }

                minX = _selectionLine.Points.Min(x => x.X);
                maxX = _selectionLine.Points.Max(x => x.X);
                minY = _selectionLine.Points.Min(y => y.Y);
                maxY = _selectionLine.Points.Max(y => y.Y);

                totWidth = maxX + Math.Abs(minX);
                totHeigth = maxY + Math.Abs(minY);

                selWidth = _selection.Width;
                selHeight = _selection.Height;
            }

            if (selection is null) return;
            _selection = selection;

            bool ifContainsSelectionLine = _selection.CheckCan.Children.Contains(_selectionLine);
            List<UIElement> selElems = ReAssignChildrenInAuxiliaryCanvas(_selection.CheckCan);
            _selection.CheckCan.Children.Clear();

            List<UIElement> selCanElems = ReAssignChildrenInAuxiliaryCanvas(_selection.SelectCan);
            _selection.SelectCan.Children.Clear();

            _selection.DashedBorder.Visibility = Visibility.Hidden;

            //Parent
            Canvas parent = (Canvas)_selection.Parent;

            //Got point for selection
            Point oldSelPoint = new Point(Canvas.GetLeft(_selection), Canvas.GetTop(_selection));
            Point globalPoint =
             GetSelectionPointComparedToDrawingCanvas(_selection, new Point(0, 0), drawingCanvas);

            // Remove from parent + remove DrawingCanvas children
            parent.Children.Remove(_selection);
            List<UIElement> drawCanElems = ReAssignChildrenInAuxiliaryCanvas(drawingCanvas);
            drawingCanvas.Children.Clear();

            //Set selection in new Canvas
            Canvas.SetLeft(_selection, globalPoint.X);
            Canvas.SetTop(_selection, globalPoint.Y);
            //Set Things for Selection
            SetParamsForSelection(ifContainsSelectionLine, globalPoint, drawingCanvas);

            //make Selection;
            drawingCanvas.UpdateLayout();
            _selection = null;

            MakeSelection(drawingCanvas, selSizeLB, selImage,
                checkRect, scrol, ifContainsSelectionLine);

            //GetEverything Back
            drawingCanvas.Children.Clear();

            Canvas.SetLeft(_selection, oldSelPoint.X);
            Canvas.SetTop(_selection, oldSelPoint.Y);

            parent.Children.Add(_selection);

            AddListElemsInCanvas(drawCanElems, drawingCanvas);
            AddListElemsInCanvas(selCanElems, _selection.SelectCan);

            _selection.DashedBorder.Visibility = Visibility.Visible;
            if (parent == drawingCanvas) return;
            InitBGForSelectionFromDeepest(GetSelectionsSelectionParent((Canvas)_selection.Parent),
                drawingCanvas, selSizeLB, selImage, checkRect, scrol);
        }

        public Point GetGlobalSelectionPoint(Selection sel, Point tempPoint, Canvas drawingCanvas)
        {
            Point selPoint = new Point(Canvas.GetLeft(sel), Canvas.GetTop(sel));

            tempPoint.X = tempPoint.X + selPoint.X;
            tempPoint.Y = tempPoint.Y + selPoint.Y;

            Canvas canParent = (Canvas)sel.Parent;
            if (canParent == drawingCanvas) return tempPoint;

            Selection parent = GetSelectionsSelectionParent(canParent);

            if (parent is null) return tempPoint;
            return GetGlobalSelectionPoint(parent, tempPoint, drawingCanvas);
        }

        public bool IfCustomSelectionShouldBeRemoved(Polyline line, Point globalPoint, Canvas drawingCanvas)
        {
            double thickness = line.StrokeThickness;
            return drawingCanvas.Width <= globalPoint.X + thickness ||
                 drawingCanvas.Height <= globalPoint.Y + thickness ||
                 globalPoint.X + line.Width - thickness <= 0 ||
                 globalPoint.Y + line.Height - thickness <= 0;
        }

        public void SetSelectionBgInDrawingCanvas(Selection selection, Canvas drawingCanvas, Rectangle checkRect)
        {
            selection.DashedBorder.Visibility = Visibility.Hidden;
            selection.SizingGrid.Visibility = Visibility.Hidden;

            Polyline sizeLine = selection.CheckCan.Children.OfType<Polyline>().FirstOrDefault();
            if (!(sizeLine is null)) selection.CheckCan.Children.Remove(sizeLine);

            SetCanvasBg(drawingCanvas, checkRect);
            selection.DashedBorder.Visibility = Visibility.Visible;
            selection.SizingGrid.Visibility = Visibility.Visible;

            if (!(sizeLine is null))
            {
                _selectionLine = sizeLine;
            }
            selection.SelectCan.Background = new SolidColorBrush(Colors.Transparent);
        }

        public void ReturnStartSelection(Selection selection, Point localPoint)
        {
            Point tempPoint = new Point(Canvas.GetLeft(selection), Canvas.GetTop(selection));

            double xLoc = tempPoint.X == 0 ? 0 : localPoint.X;
            double yLoc = tempPoint.Y == 0 ? 0 : localPoint.Y;

            Canvas.SetLeft(selection, xLoc);
            Canvas.SetTop(selection, yLoc);

            /*            if (tempPoint.X == 0)
                        {
                            Canvas.SetLeft(selection, 0);
                        }
                        if (tempPoint.X != 0)
                        {
                            Canvas.SetLeft(selection, localPoint.X);
                        }
                        if (tempPoint.Y == 0)
                        {
                            Canvas.SetTop(selection, 0);
                        }
                        if (tempPoint.Y != 0)
                        {
                            Canvas.SetTop(selection, localPoint.Y);
                        }*/
        }

        public Point ChangePointToMoveInLowerTurn(Point point)
        {
            return new Point(point.X > 0 ? 0 : point.X,
                             point.Y > 0 ? 0 : point.Y);
        }

        public Point ChangePointIntoBiggerTuren(Point point)
        {
            return new Point(point.X > 0 ? point.X : 0,
                             point.Y > 0 ? point.Y : 0);
        }

        public void MoveChildreninDeep(Selection selection, Point moveDist)
        {
            if (moveDist.X < 0) moveDist.X = 0;
            if (moveDist.Y < 0) moveDist.Y = 0;

            Selection childSel = null;
            for (int i = 0; i < selection.SelectCan.Children.Count; i++)
            {
                UIElement elem = selection.SelectCan.Children[i];
                Point loc = new Point(Canvas.GetLeft(elem), Canvas.GetTop(elem));
                loc = new Point(loc.X + moveDist.X, loc.Y + moveDist.Y);

                Canvas.SetLeft(elem, loc.X);
                Canvas.SetTop(elem, loc.Y);

                if (elem is Selection) selection = (Selection)elem;
            }
            if (childSel is null) return;
            MoveChildreninDeep(childSel, moveDist);
        }

        public void SetLocForChildren(Selection selection, Point movePoint)
        {
            movePoint = ChangePointToMoveInLowerTurn(movePoint);
            for (int i = 0; i < selection.SelectCan.Children.Count; i++)
            {
                UIElement elem = selection.SelectCan.Children[i];

                Point childPoint = new Point(Canvas.GetLeft(elem),
                    Canvas.GetTop(elem));

                childPoint.X += movePoint.X;
                childPoint.Y += movePoint.Y;

                if (childPoint.X < 0 || childPoint.Y < 0 &&
                    elem is Selection)
                {
                    MoveChildreninDeep((Selection)elem, childPoint);
                }

                childPoint = ChangePointIntoBiggerTuren(childPoint);

                Canvas.SetLeft(elem, childPoint.X);
                Canvas.SetTop(elem, childPoint.Y);
            }
        }

        private void MakeCutSelectionForSquareSelection(Selection selection, Canvas drawingCanvas,
            Rectangle checkRect, Image selImage, Label selSizeLB, ScrollViewer scrol)
        {
            if (selection is null) return;
            _selection = selection;

            //Start Points
            Size selSize = new Size(selection.Width, selection.Height);
            Point localPoint = new Point(Canvas.GetLeft(selection), Canvas.GetTop(selection));

            Point globalPoint = GetGlobalSelectionPoint(selection, new Point(0, 0), drawingCanvas);

            //Set parent
            Canvas parent = (Canvas)selection.Parent;
            bool ifContainsLine = selection.CheckCan.Children.OfType<Polyline>().Any();

            List<UIElement> mainCanvasElems = ReAssignChildrenInAuxiliaryCanvas(drawingCanvas);
            drawingCanvas.Children.Clear();

            //parent == drawingCanvas => remove selection from list
            if (parent == drawingCanvas) mainCanvasElems.Remove(mainCanvasElems.First(x => x is Selection));

            List<UIElement> selElems = ReAssignChildrenInAuxiliaryCanvas(selection.SelectCan);
            selection.SelectCan.Children.Clear();

            //set selection location
            Canvas.SetLeft(selection, globalPoint.X);
            Canvas.SetTop(selection, globalPoint.Y);

            //set selection in drawingCanvas
            parent.Children.Remove(selection);
            drawingCanvas.Children.Add(selection);

            SetSelectionBgInDrawingCanvas(selection, drawingCanvas, checkRect);

            //Cut parts of selection(if need)
            CheckSizes(selection, globalPoint, selection.CheckCan.Children.OfType<Polyline>().Any(), drawingCanvas);
            globalPoint = new Point(Canvas.GetLeft(selection), Canvas.GetTop(selection));

            drawingCanvas.Children.Clear();
            _selection = selection;
            SetParamsForSelection(ifContainsLine, globalPoint, drawingCanvas);

            _selection = null;
            selection = null;

            // create selection (with old borders)
            MakeSelection(drawingCanvas, selSizeLB, selImage, checkRect, scrol);

            //return _selection in old place
            ReturnStartSelection(_selection, localPoint);

            drawingCanvas.Children.Remove(_selection);
            parent.Children.Add(_selection);

            //return checldren in prev canvas
            AddListElemsInCanvas(selElems, _selection.SelectCan);
            AddListElemsInCanvas(mainCanvasElems, drawingCanvas);

            if (!RemoveSelectionIfItsOutOfBorders(_selection, globalPoint, drawingCanvas))
            {
                SetLocForChildren(_selection, localPoint);
            }

            _selection.SizingGrid.Visibility = Visibility.Hidden;
            if (parent == drawingCanvas) return;

            Selection sel = GetSelectionsSelectionParent(parent);

            CutSelectionsIfItOutOfBoundaries(sel, drawingCanvas, selSizeLB, selImage, checkRect, scrol);
        }

        public bool RemoveSelectionIfItsOutOfBorders(Selection selection, Point globalPoint, Canvas drawingCanvas)
        {
            if (globalPoint.X > drawingCanvas.Width || //Right
                 globalPoint.Y > drawingCanvas.Height || //Bottom
                 globalPoint.X + selection.Width < 0 || //Left
                 globalPoint.Y + selection.Height < 0) //Top
            {
                Canvas parent = (Canvas)selection.Parent;
                parent.Children.Remove(selection);
                return true;
            }
            return false;
        }

        private void MakeCutSelectionForCustomSelection(Selection selection, Canvas drawingCanvas,
            Label selSizeLB, Image selImg, Rectangle checkRect, ScrollViewer scrol)
        {
            Canvas parent = (Canvas)selection.Parent;

            //If seletcion is out of boundaries and need to delete it
            if (!IfSelectionIsOutOfBoundaries(selection, drawingCanvas))
            {
                if (parent == drawingCanvas) return;
                CutSelectionsIfItOutOfBoundaries(GetSelectionsSelectionParent(parent),
                    drawingCanvas, selSizeLB, selImg, checkRect, scrol);
                return;
            }

            Polyline line = selection.CheckCan.Children.OfType<Polyline>().FirstOrDefault();
            _selectionLine = line;
            _selection = selection;

            Point selPoint = new Point(Canvas.GetLeft(_selection), Canvas.GetTop(_selection));
            Point linePoint = new Point(Canvas.GetLeft(_selectionLine), Canvas.GetTop(_selectionLine));

            //Set location for line
            Canvas.SetLeft(_selectionLine, selPoint.X + linePoint.X);
            Canvas.SetTop(_selectionLine, selPoint.Y + linePoint.Y);

            Point globalPoint = GetGlobalSelectionPoint(selection, new Point(0, 0), drawingCanvas);
            Point localPoint = new Point(Canvas.GetLeft(selection), Canvas.GetTop(selection));

            List<UIElement> mainCanvasElems = ReAssignChildrenInAuxiliaryCanvas(drawingCanvas);
            drawingCanvas.Children.Clear();
            //parent == drawingCanvas => remove selection from list
            if (parent == drawingCanvas) mainCanvasElems.Remove(mainCanvasElems.First(x => x is Selection));

            selection.CheckCan.Children.Remove(line);
            selection = null;

            bool ifRemove = IfCustomSelectionShouldBeRemoved(line, globalPoint, drawingCanvas);

            //if part of the selection is stil in drawingCanvas
            if (!ifRemove)
            {
                double selWidth = _selection.Width;
                double selHeight = _selection.Height;

                Canvas.SetLeft(line, globalPoint.X);
                Canvas.SetTop(line, globalPoint.Y);
                parent.Children.Remove(selection);

                _selectionLine = line;
                drawingCanvas.Children.Add(_selectionLine);
                //ChangePointCordesAfterChenageLineSize();

                Point checkSelPoint = new Point(Canvas.GetLeft(_selection),
                    Canvas.GetTop(_selection));
                Point checkSelLinePoint = new Point(Canvas.GetLeft(_selectionLine),
                    Canvas.GetTop(_selectionLine));

                selWidth = _selection.Width;
                selHeight = _selection.Height;

                if (!CheckSizes(selection, globalPoint,
                    !(line is null), drawingCanvas))
                {
                    selLineLoc = globalPoint;
                    Canvas.SetLeft(line, globalPoint.X);
                    Canvas.SetTop(line, globalPoint.Y);
                }
                else
                {
                    globalPoint = new Point(Canvas.GetLeft(_selectionLine), Canvas.GetTop(_selectionLine));
                }

                selWidth = _selection.Width;
                selHeight = _selection.Height;

                SetParamsForSelection(true, globalPoint, drawingCanvas);

                MakeSelection(drawingCanvas, selSizeLB, selImg, checkRect, scrol, ifContainsLine: true);
                checkSelPoint = new Point(Canvas.GetLeft(_selection), Canvas.GetTop(_selection));
                checkSelLinePoint = new Point(Canvas.GetLeft(_selectionLine), Canvas.GetTop(_selectionLine));

                drawingCanvas.Children.Remove(_selection);
                parent.Children.Add(_selection);

                selWidth = _selection.Width;
                selHeight = _selection.Height;
            }
            else
            {
                parent.Children.Remove(_selection);
                _selectionLine = null;
            }

            AddListElemsInCanvas(mainCanvasElems, drawingCanvas);

            if (parent == drawingCanvas) return;
            Selection sel = GetSelectionsSelectionParent(parent);

            CutSelectionsIfItOutOfBoundaries(sel, drawingCanvas, selSizeLB, selImg, checkRect, scrol);
        }

        public bool IfSelectionIsOutOfBoundaries(Selection selection, Canvas drawingCanvas) //-, dubl uslovie(ego mnogo po kody)
        {
            Point globalPoint = GetGlobalSelectionPoint(selection, new Point(0, 0), drawingCanvas);
            return IfSelIsOutOfBoundaries(globalPoint, selection, drawingCanvas);
/*               globalPoint.X + selection.Width > drawingCanvas.Width ||
                   globalPoint.Y + selection.Height > drawingCanvas.Height ||
                   globalPoint.X < 0 || globalPoint.Y < 0;*/
        }

        public void CutSelectionsIfItOutOfBoundaries(Selection selection, Canvas drawingCanvas,
            Label selSizeLB, Image selImg, Rectangle checkRect, ScrollViewer scrol)
        {
            if (selection.CheckCan.Children.OfType<Polyline>().Any())
            {
                MakeCutSelectionForCustomSelection(selection, drawingCanvas, selSizeLB, selImg, checkRect, scrol);
                return;
            }
            MakeCutSelectionForSquareSelection(selection, drawingCanvas, checkRect, selImg, selSizeLB, scrol);
        }

        public bool IfSelectionsNeedToBeCut(Canvas drawingCanvas)//-, dubl uslovie(ego mnogo po kody)
        {
            Point selPoint = new Point(Canvas.GetLeft(_selection), Canvas.GetTop(_selection));
            return IfSelIsOutOfBoundaries(selPoint, _selection, drawingCanvas);
/*                selPoint.X < 0 || selPoint.Y < 0 ||
                    selPoint.X + _selection.Width > drawingCanvas.Width ||
                    selPoint.Y + _selection.Height > drawingCanvas.Height;*/
        }

        public bool IfSelIsOutOfBoundaries(Point point, Selection selection, Canvas drawingCanvas)
        {
            return point.X < 0 || point.Y < 0 ||
                point.X + selection.Width > drawingCanvas.Width ||
                point.Y + selection.Height > drawingCanvas.Height;
        }

        public void SelecitonCut(Canvas drawingCanvas, Label selSizeLb, Image selImage, Rectangle checkRect, ScrollViewer scroll)
        {
            Selection deepest = null;
            GetTheDeepestSelection(drawingCanvas, ref deepest);

            if (IfSelectionsNeedToBeCut(drawingCanvas))
            {
                _ifCutCheck = true;
                CutSelectionsIfItOutOfBoundaries(deepest, drawingCanvas, selSizeLb, selImage, checkRect, scroll);
                _ifCutCheck = false;
            }
        }

        public void MakeCustomSelection(MouseEventArgs e, Canvas drawingCanvas)
        {
            const int selThickness = 2;

            _selectionLine = new Polyline()
            {
                Stroke = Brushes.LightBlue,
                StrokeThickness = selThickness,
                Fill = Brushes.Transparent,
                RenderTransform = new TranslateTransform(0, 0)
            };

            Canvas.SetLeft(_selectionLine, e.GetPosition(drawingCanvas).X);
            Canvas.SetTop(_selectionLine, e.GetPosition(drawingCanvas).Y);

            _firstSelectionStart = new Point(e.GetPosition(drawingCanvas).X, e.GetPosition(drawingCanvas).Y);
            drawingCanvas.Children.Add(_selectionLine);
        }

        public void UpdateErazingLine(Point currentPoint, Canvas drawingCanvas)
        {
            double deltaX = Math.Abs(currentPoint.X - previousPoint.X);
            double deltaY = Math.Abs(currentPoint.Y - previousPoint.Y);

            if (deltaX > deltaY)
                currentPoint.Y = previousPoint.Y;
            else
                currentPoint.X = previousPoint.X;


            Line line = new Line
            {
                X1 = previousPoint.X,
                Y1 = previousPoint.Y,
                X2 = currentPoint.X,
                Y2 = currentPoint.Y,

                Stroke = _ifErasing ? Brushes.White : _spaceDrawingPressed ? FirstColor : ColorToPaint,
                StrokeThickness = brushThickness,
                StrokeStartLineCap = _ifErasing ? PenLineCap.Square : PenLineCap.Round,
                StrokeEndLineCap = _ifErasing ? PenLineCap.Square : PenLineCap.Round,
                StrokeLineJoin = _ifErasing ? PenLineJoin.Bevel : PenLineJoin.Round,
                //SnapsToDevicePixels = true
            };

            drawingCanvas.Children.Add(line);
        }

        public Polyline _drawPolyline = null;
        public LineDivision? _paintDirection = null;
        public Point currentPoint;
        private void ChangeDrawingPolyline(MouseEventArgs e, Canvas drawingCanvas, Rectangle checkRect)
        {
            const int pointAmountUpdate = 100;
            if ((_drawPolyline is null || _drawPolyline.Points.Count > pointAmountUpdate) &&
                _paintDirection is null)
            {
                if (!(_drawPolyline is null) && _drawPolyline.Points.Count > pointAmountUpdate)
                {
                    SetCanvasBg(drawingCanvas, checkRect);
                }
                RectangleGeometry clipGeometry = new RectangleGeometry(new Rect(0, 0,
                    drawingCanvas.Width, drawingCanvas.Height));

                Point? prevPoint = null;
                if (!(_drawPolyline is null))
                {
                    prevPoint = _drawPolyline.Points.Last();
                }
                _drawPolyline = new Polyline()
                {
                    Stroke = _spaceDrawingPressed ? FirstColor : ColorToPaint,
                    StrokeThickness = brushThickness,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Round,
                    StrokeLineJoin = PenLineJoin.Round,
                    Clip = clipGeometry,
                    ClipToBounds = true,
                    //SnapsToDevicePixels = true
                };
                RenderOptions.SetEdgeMode(_drawPolyline, EdgeMode.Aliased);

                if (!(prevPoint is null)) _drawPolyline.Points.Add((Point)prevPoint);

                drawingCanvas.Children.Add(_drawPolyline);
                AddCheckRect(e, drawingCanvas, checkRect);
            }

            if (_drawPolyline.Points.Count > 1 && _paintDirection is null)
            {
                InitPointInLine(_drawPolyline.Points.Last(), currentPoint, _drawPolyline);
            }
            if (!Keyboard.IsKeyDown(Key.LeftShift))
            {
                _paintDirection = null;
            }
            if (!(_paintDirection is null) && _drawPolyline.Points.Count != 0)
            {
                currentPoint = GetTransformedPoint(currentPoint, _drawPolyline.Points.Last());
            }
            _drawPolyline.Points.Add(currentPoint);
        }

        private Point GetTransformedPoint(Point point, Point prevPoint)
        {
            // cant be short

            const int pointStep = 1;
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
                            new Point(prevPoint.X + pointStep, prevPoint.Y - pointStep) :
                            new Point(prevPoint.X - pointStep, prevPoint.Y + pointStep);
                    }
                case LineDivision.Right:
                    {
                        point = new Point(point.X, prevPoint.Y);
                        return point;
                    }
                case LineDivision.DownRight:
                    {
                        return (prevPoint.Y < point.Y || prevPoint.X < point.X) ?
                            new Point(prevPoint.X + pointStep, prevPoint.Y + pointStep) :
                            new Point(prevPoint.X - pointStep, prevPoint.Y - pointStep);
                    }
                case LineDivision.Down:
                    {
                        point = new Point(prevPoint.X, point.Y);
                        return point;
                    }
                case LineDivision.DownLeft:
                    {
                        return (prevPoint.Y < point.Y || prevPoint.X > point.X) ?
                            new Point(prevPoint.X - pointStep, prevPoint.Y + pointStep) :
                            new Point(prevPoint.X + pointStep, prevPoint.Y - pointStep);
                    }
                case LineDivision.Left:
                    {
                        point = new Point(point.X, prevPoint.Y);
                        return point;
                    }
                case LineDivision.UpLeft:
                    {
                        return (prevPoint.X > point.X || prevPoint.Y > point.Y) ?
                            new Point(prevPoint.X - pointStep, prevPoint.Y - pointStep) :
                            new Point(prevPoint.X + pointStep, prevPoint.Y + pointStep);
                    }
                default:
                    {
                        MessageBox.Show("How did you get here?");
                        break;
                    }
            }
            return point;
        }

        public void InitPointInLine(Point startPoint, Point endPoint, Polyline poly) //-, dubl
        {
            bool keyCheck = Keyboard.IsKeyDown(Key.LeftShift);
            if (keyCheck)
            {
                Point start = startPoint;
                Point end = endPoint;

                bool ifChangePoint = _figType == FigureTypes.Curve ||
                    _figType == FigureTypes.Polygon ||
                    _type == ActionType.Drawing;


                if (ifChangePoint)
                {
                    //convert point to system of cords point 
                    // point (0;0) - start Point
                    start = new Point(0, 0);
                    end = new Point(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
                }

                endPoint = TurnLineFigWithShift(end, start);

                if (ifChangePoint)
                {
                    //reconvert point in usual view
                    endPoint = new Point(endPoint.X + startPoint.X, endPoint.Y + startPoint.Y);
                }
            }

            if (_type == ActionType.Drawing) return;
            if (_figType == FigureTypes.Polygon)
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

        public void AddCheckRect(MouseEventArgs e, Canvas drawingCanvas, Rectangle checkRect)
        {
            Point point = e.GetPosition(drawingCanvas);

            if (drawingCanvas.Children.Contains(checkRect))
            {
                return;
            }

            drawingCanvas.Children.Add(checkRect);

            Canvas.SetLeft(checkRect, point.X);
            Canvas.SetTop(checkRect, point.Y);

            checkRect.CaptureMouse();
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

        private Point GetMiddlePointForCrossLoc(Point point, LineDivision division, Point startPoint)
        {
            const int convertMark = 1;
            point = point.X > point.Y ? new Point(point.Y, point.Y) :
                    point.X < point.Y ? new Point(point.X, point.X) : point;

            return division == LineDivision.UpRight ? new Point(Math.Abs(point.X),
                point.Y > startPoint.X ? point.Y * -convertMark : point.Y) :
                division == LineDivision.DownRight ? new Point(Math.Abs(point.X),
                point.Y < startPoint.X ? point.Y * -convertMark : point.Y) :
                division == LineDivision.DownLeft ? new Point(point.X, point.Y >
                startPoint.X ? point.Y : point.Y * -convertMark) :
                division == LineDivision.UpLeft ? new Point(point.X, point.Y >
                startPoint.X ? point.Y * -convertMark : point.Y) : point;
            /*            switch (division)
                        {
                            case LineDivision.UpRight:
                                {
                                    point = new Point(Math.Abs(point.X), point.Y > startPoint.X ? point.Y * -convertMark : point.Y);
                                    return point;
                                }
                            case LineDivision.DownRight:
                                {
                                    point = new Point(Math.Abs(point.X), point.Y < startPoint.X ? point.Y * -convertMark : point.Y);
                                    return point;
                                }
                            case LineDivision.DownLeft:
                                {
                                    point = new Point(point.X, point.Y > startPoint.X ? point.Y : point.Y * -convertMark);
                                    return point;
                                }
                            case LineDivision.UpLeft:
                                {
                                    point = new Point(point.X, point.Y > startPoint.X ? point.Y * -convertMark : point.Y);
                                    return point;
                                }
                            default:
                                {
                                    return point;
                                }
                        }*/
        }

        public void GetLineToPaint(Point currentPoint, MouseEventArgs e, Canvas drawingCanvas, Rectangle checkRect)
        {
            if (_ifErasing)
            {
                UpdateErazingLine(currentPoint, drawingCanvas);
            }
            else if (_type == ActionType.Drawing)
            {
                ChangeDrawingPolyline(e, drawingCanvas, checkRect);
            }
        }
        public void CalligraphyBrushPaint(double angle, Canvas drawingCanvas, Rectangle checkRect)
        {
            const int maxChildren = 100;
            const int dividerInMiddle = 2;
            const int amountofBorders = 4;
            const int firstSideIndex = 0;
            const int secondSideIndex = 1;
            const int thirdSideIndex = 2;
            const int forthSideIndex = 3;

            Vector offset = new Vector(Math.Cos(angle) * brushThickness / dividerInMiddle,
                                Math.Sin(angle) * brushThickness / dividerInMiddle);
            Point[] points = new Point[amountofBorders];

            points[firstSideIndex] = new Point(previousPoint.X + offset.X, previousPoint.Y + offset.Y);
            points[secondSideIndex] = new Point(previousPoint.X - offset.X, previousPoint.Y - offset.Y);
            points[thirdSideIndex] = new Point(currentPoint.X - offset.X, currentPoint.Y - offset.Y);
            points[forthSideIndex] = new Point(currentPoint.X + offset.X, currentPoint.Y + offset.Y);

            const double thickness = 0.5;
            RectangleGeometry clipGeometry = new RectangleGeometry(new Rect(0,
                0, drawingCanvas.Width, drawingCanvas.Height));

            Polygon polygon = new Polygon
            {
                Points = new PointCollection(points),
                Fill = ColorToPaint,
                Stroke = ColorToPaint,
                StrokeThickness = thickness,
                ClipToBounds = true,
                Clip = clipGeometry
            };
            drawingCanvas.Children.Add(polygon);
            if (drawingCanvas.Children.Count > maxChildren)
            {
                SetCanvasBg(drawingCanvas, checkRect);
            }
        }

        public void SprayPaint(Point point, Canvas drawingCanvas, Rectangle checkRect)
        {
            Random random = new Random();
            const int angleMultiplier = 2;
            const int ellipseSize = 1;
            const int dividerInMiddle = 2;
            const int sprayDensity = 30;

            for (int i = 0; i < sprayDensity; i++)
            {
                double angle = random.NextDouble() * angleMultiplier * Math.PI;
                double radius = Math.Sqrt(random.NextDouble()) * brushThickness / dividerInMiddle;
                double offsetX = radius * Math.Cos(angle);
                double offsetY = radius * Math.Sin(angle);

                Ellipse ellipse = new Ellipse
                {
                    Width = ellipseSize,
                    Height = ellipseSize,
                    Fill = _spaceDrawingPressed ? FirstColor : ColorToPaint
                };
                Canvas.SetLeft(ellipse, point.X + offsetX);
                Canvas.SetTop(ellipse, point.Y + offsetY);

                drawingCanvas.Children.Add(ellipse);
            }
            SetCanvasBg(drawingCanvas, checkRect, ifSpray: true);
        }

        public void CalculatePolylineSize(Shape shape)
        {
            if (!(shape is Polyline)) return;

            Polyline polyline = (Polyline)shape;

            double minX = polyline.Points.Min(x => x.X);
            double maxX = polyline.Points.Max(x => x.X);
            double minY = polyline.Points.Min(y => y.Y);
            double maxY = polyline.Points.Max(y => y.Y);

            polyline.Width = maxX - minX;
            polyline.Height = maxY - minY;

        }

        public bool IfPolygonFigureIsDone(Canvas drawingCanvas)
        {
            if (_figType != FigureTypes.Polygon ||
                _figToPaint is null) return true;

            const int lineClose = 10;
            const int amountOfPointToEndFigure = 2;

            if (((Polyline)_figToPaint).Points.Count > amountOfPointToEndFigure)
            {
                // Check for differ between first and last poit
                double xDiffer = Math.Abs(((Polyline)_figToPaint).Points.Last().X -
                    ((Polyline)_figToPaint).Points.First().X);
                double yDiffer = Math.Abs(((Polyline)_figToPaint).Points.Last().Y -
                    ((Polyline)_figToPaint).Points.First().Y);

                if (xDiffer <= lineClose && yDiffer <= lineClose)
                {
                    ((Polyline)_figToPaint).Points.RemoveAt(((Polyline)_figToPaint).Points.Count - 1);
                    ((Polyline)_figToPaint).Points.Add(((Polyline)_figToPaint).Points.First());

                    ((Polyline)_figToPaint).InvalidateVisual();
                    drawingCanvas.UpdateLayout();

                    CalculatePolylineSize(_figToPaint);
                    return true;
                }
            }
            return false;
        }

        public bool IfPointOutOfBoundaries(Point point, Canvas drawingCanvas)
        {
            return point.X < 0 || point.Y < 0 ||
                point.X > drawingCanvas.Width ||
                point.Y > drawingCanvas.Height;
        }

        public void PaintInCanvas(Canvas canvas, Image img) //-, dubl
        {
            /*            const int pixStep = 4;
                        const int getGStep = 1;
                        const int getRStep = 2;
                        const int getAlphaStep = 3;
                        const int bitsAdd = 7;
                        const int bitsInByte = 8;*/

            canvas.RenderTransform = new TranslateTransform(0, 0);
            img.RenderTransform = new TranslateTransform(0, 0);

            RenderTargetBitmap bitmap = ConvertCanvasToBitmap(canvas);
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
            int strideGlobal = widthGlobal * ((writeableBitmapGlobalSource.Format.BitsPerPixel + bitsAdd) / bitsInByte);
            byte[] pixelsGlobal = new byte[heightGlobal * strideGlobal];
            writeableBitmapGlobalSource.CopyPixels(pixelsGlobal, strideGlobal, 0);

            WriteableBitmap writeableBitmap = new WriteableBitmap(img.Source as BitmapSource);
            int width = writeableBitmap.PixelWidth;
            int height = writeableBitmap.PixelHeight;
            int stride = width * ((writeableBitmap.Format.BitsPerPixel + bitsAdd) / bitsInByte);
            byte[] pixels = new byte[height * stride];
            writeableBitmap.CopyPixels(pixels, stride, 0);

            Point selLoc = new Point(Canvas.GetLeft(_selection), Canvas.GetTop(_selection));

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Point tempPoint = new Point(selLoc.X + x,
                        selLoc.Y + y);
                    int indexGlobal = (int)tempPoint.Y * strideGlobal + pixStep * (int)tempPoint.X;
                    if (indexGlobal + pixStep < pixelsGlobal.Length && indexGlobal > 0)
                    {
                        int index = y * stride + pixStep * x;
                        byte alpha = pixels[index + getAlphaStep];
                        if (alpha != 0)
                        {
                            pixelsGlobal[indexGlobal] = maxColorParam;
                            pixelsGlobal[indexGlobal + getGStep] = maxColorParam;
                            pixelsGlobal[indexGlobal + getRStep] = maxColorParam;
                            pixelsGlobal[indexGlobal + getAlphaStep] = maxColorParam;
                        }
                    }
                }
            }

            writeableBitmapGlobalSource.WritePixels(new Int32Rect(0, 0,
                widthGlobal, heightGlobal), pixelsGlobal, strideGlobal, 0);
            image.Source = writeableBitmapGlobalSource;
            RenderOptions.SetEdgeMode(image, EdgeMode.Aliased);
            canvas.Children.Add(image);
        }

        public void ReverseColorInSelection(Rectangle CheckRect)
        {
            const int pixStep = 4;
            //const int getGStep = 1;
            //const int getRStep = 2;
            //const int getAlphaStep = 3;
            const int bitsInByte = 8;
            const int bitsToAdd = 7;
            Point imgLoc = new Point(-1, -1);

            Image originalImage = ConvertBackgroundToImage(_selection.BgCanvas);
            _selection.CheckCan.Background = null;
            _selection.BgCanvas.Background = null;

            originalImage.RenderTransform = new TranslateTransform(0, 0);
            BitmapSource bitmapSource = originalImage.Source as BitmapSource;
            WriteableBitmap writeableBitmap = new WriteableBitmap(bitmapSource);
            RenderOptions.SetEdgeMode(writeableBitmap, EdgeMode.Aliased);

            int width = writeableBitmap.PixelWidth;
            int height = writeableBitmap.PixelHeight;
            int stride = width * ((writeableBitmap.Format.BitsPerPixel + bitsToAdd) / bitsInByte);
            byte[] pixels = new byte[height * stride];

            writeableBitmap.CopyPixels(pixels, stride, 0);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * stride + pixStep * x;

                    byte blue = pixels[index];
                    byte green = pixels[index + getGStep];
                    byte red = pixels[index + getRStep];
                    byte alpha = pixels[index + getAlphaStep];

                    if (alpha != 0)
                    {
                        SolidColorBrush targetColor = GetColorToRepaint(
                            new SolidColorBrush(Color.FromArgb(byte.MaxValue, red, green, blue)));
                        pixels[index] = targetColor.Color.B;
                        pixels[index + getGStep] = targetColor.Color.G;
                        pixels[index + getRStep] = targetColor.Color.R;
                        pixels[index + getAlphaStep] = targetColor.Color.A;
                    }
                }
            }

            writeableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);

            RenderOptions.SetEdgeMode(originalImage, EdgeMode.Aliased);
            originalImage.SnapsToDevicePixels = true;
            originalImage.Source = writeableBitmap;
            originalImage.RenderTransform = new TranslateTransform(_horizontalOffset, 0);

            _selection.BgCanvas.Children.Add(originalImage);

            Canvas.SetLeft(originalImage, imgLoc.X + _selection.DashedBorder.StrokeThickness);
            Canvas.SetTop(originalImage, imgLoc.Y + _selection.DashedBorder.StrokeThickness);

            SetCanvasBg(_selection.BgCanvas, CheckRect, false);
            RenderOptions.SetEdgeMode(_selection.BgCanvas.Background, EdgeMode.Aliased);
        }
    }
}
