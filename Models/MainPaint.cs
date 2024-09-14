using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        public bool IfSelectionIsMacken = false;

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
        public const string fourPointedStar = "M 95 20 L 110 70 L 180 80 L 110 100 " +
                        "L 95 160 L 75 100 L 20 80 L 80 70 L 95 20";
        public const string fivePointStar = "M 100 10  L 130 90 L 200 90 L 145 130 " +
                        "L 170 200 L 100 160 L 30 200  L 55 130 L 0 90  L 70 90 Z";
        public const string sixPointStarData = "M 100 10 L 130 60 L 180 60 L 150 100 " +
                        "L 180 140 L 130 140 L 100 180 L 70 140 L 20 140 " +
                        "L 50 100 L 20 60 L 70 60 Z";
        public const string roundedCommentData = "M 120 10 L 150 10 Q 170, 10, 170 40 L 170 100 " +
                        "Q 170 120, 150 120 L 75 120 L 65 140 L 55 120 Q 25, 120, 25 100 " +
                        "L 25 40 Q 25 10, 40 10 Z";
        public const string ovalCommentData = "M 100,100 A 40, 30 0 1 1 200, 100 " +
                        "A 60, 60 1 0 1 140, 130 L 125 145 L 120 130 Q 100 120 100 100";
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

        public MainPaint()
        {
            ColorToPaint = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            ImagesHistory = new List<Image>();
            PalleteMod = new PalleteModel();
            FirstColor = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            SecondColor = new SolidColorBrush(Color.FromRgb(253, 253, 253));
            CanvasStates = new List<Canvas>();
            _tempBrushType = BrushType.UsualBrush;
        }


        public void SetActionTypeByButtonPressed(Button but)
        {
            SolidColorBrush borderBrush = new SolidColorBrush(Color.FromRgb(0, 103, 192));
            but.BorderBrush = borderBrush;

            SolidColorBrush bgBrush = new SolidColorBrush(Color.FromRgb(240, 240, 240));
            but.Background = bgBrush;

            if (but.Name == "Pen")
            {
                _tempCursor = _pencilCurs;
                _type = ActionType.Drawing;
            }
            else if (but.Name == "Erazer")
            {
                _tempCursor = _crossCurs;
                _type = ActionType.Erazing;
            }
            else if (but.Name == "Bucket")
            {
                _tempCursor = _bucketCurs;
                _type = ActionType.Filling;
            }
            else if (but.Name == "Text")
            {
                _tempCursor = _textingCurs;
                _type = ActionType.Text;
            }
            else if (but.Name == "ColorDrop")
            {
                _tempCursor = _pipetteCurs;
                _type = ActionType.PickingColor;
            }
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
            IfSelectionIsMacken = false;
        }
        public void InitDeed()
        {
            MakeAllActionsNegative();
            if (_type == ActionType.Drawing)
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
                IfSelectionIsMacken = false;
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
            }
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
                        if (!_isDrawingLine && !_isAdjustingCurve)
                        {
                            _startPoint = e.GetPosition(DrawingCanvas);
                            _polyline = new Polyline
                            {
                                Stroke = ColorToPaint,
                                StrokeThickness = 2,
                                Points = new PointCollection { _startPoint }
                            };

                            DrawingCanvas.Children.Add(_polyline);
                            _isDrawingLine = true;
                        }
                        else if (_isDrawingLine && _polyline.Points.Count > 1)
                        {
                            _endPoint = _polyline.Points[1];
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
                                Point3 = _endPoint
                            };
                            _pathFigure.Segments.Add(_bezierSegment);

                            PathGeometry pathGeometry = new PathGeometry();
                            pathGeometry.Figures.Add(_pathFigure);

                            const int curveThickness = 2;
                            _figToPaint = new System.Windows.Shapes.Path
                            {
                                Stroke = ColorToPaint,
                                StrokeThickness = curveThickness,
                                Data = pathGeometry
                            };

                            DrawingCanvas.Children.Add(_figToPaint);
                            _isDrawingLine = false;
                            _isAdjustingCurve = true;

                            _ifCurveIsDone = true;
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
            var polyline = new Polyline();
            polyline.Stroke = ColorToPaint;

            RectangleGeometry clip = new RectangleGeometry(new Rect(0, 0, DrawingCanvas.Width, DrawingCanvas.Height));
            polyline.ClipToBounds = true;
            polyline.Clip = clip;

            polylines.Add(polyline);

            polyline.Points.Add(e.GetPosition(DrawingCanvas));

            DrawingCanvas.Children.Add(polyline);

            polyline.StrokeThickness = brushThickness + _brushSizeCorrel;
            polyline.Opacity = markerOpacity;
        }
        public bool CheckMethod(Point point, Polyline poly)
        {
            const int formulaPower = 2;
            for (int i = 0; i < poly.Points.Count - (brushThickness + _brushSizeCorrel); i++)
            {
                if ((Math.Sqrt(Math.Pow(point.X - poly.Points[i].X, formulaPower) +
                                        Math.Pow(point.Y - poly.Points[i].Y, formulaPower))) <= (brushThickness + _brushSizeCorrel))
                {
                    return true;
                }
            }
            return false;
        }
        public void MarkerBrushPaint(MouseEventArgs e, Canvas DrawingCanvas)
        {
            if (polylines.Last().Points.Last() ==
                e.GetPosition(DrawingCanvas))
            {
                return;
            }
            var point = e.GetPosition(DrawingCanvas);
            if (CheckMethod(point, polylines.Last()))
            {
                SetPaintingMarker(e, DrawingCanvas);
            }
            polylines.Last().Points.Add(point);
        }
        private const int _dpiParam = 96;
        public RenderTargetBitmap ConvertCanvasToBitmap(Canvas canvas)
        {
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

            RenderOptions.SetEdgeMode(canvas, EdgeMode.Aliased);
            RenderOptions.SetBitmapScalingMode(canvas, BitmapScalingMode.NearestNeighbor);

            RenderTargetBitmap renderBitmap = ConvertCanvasToBitmap(canvas);

            Image image = new Image
            {
                Source = renderBitmap,
                Width = canvas.Width,
                Height = canvas.Height,
            };
            RenderOptions.SetEdgeMode(image, EdgeMode.Aliased);

            return image;
        }
        private ImageBrush _tempBrush;
        public void SetImageBrush(string brushPngPath)
        {
            //Not useing it. CAN BE DELETED
            WriteableBitmap writeableBitmap = GetChangedBitMapColor(brushPngPath);

            _tempBrush = new ImageBrush();

            _tempBrush.ImageSource = writeableBitmap;

            _tempBrush.TileMode = TileMode.Tile;
            _tempBrush.Stretch = Stretch.None;

            _tempBrush.Viewport = new Rect(0, 0, 100, 100);
            _tempBrush.ViewportUnits = BrushMappingMode.Absolute;
        }
        public WriteableBitmap GetChangedBitMapColor(string brushPngPath)
        {
            BitmapImage bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.UriSource = new Uri(brushPngPath, UriKind.RelativeOrAbsolute);
            bitmap.EndInit();

            WriteableBitmap writeableBitmap = new WriteableBitmap(bitmap);

            int width = writeableBitmap.PixelWidth;
            int height = writeableBitmap.PixelHeight;
            int stride = width * (writeableBitmap.Format.BitsPerPixel / 8);
            byte[] pixelData = new byte[height * stride];

            writeableBitmap.CopyPixels(pixelData, stride, 0);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * stride + x * 4;

                    byte b = pixelData[index];
                    byte g = pixelData[index + 1];
                    byte r = pixelData[index + 2];
                    byte a = pixelData[index + 3];

                    double alpha = a / 255.0;
                    byte newR = (byte)(r * alpha + ColorToPaint.Color.R * (1 - alpha));
                    byte newG = (byte)(g * alpha + ColorToPaint.Color.G * (1 - alpha));
                    byte newB = (byte)(b * alpha + ColorToPaint.Color.B * (1 - alpha));

                    pixelData[index] = newB;
                    pixelData[index + 1] = newG;
                    pixelData[index + 2] = newR;
                    pixelData[index + 3] = a;
                }
            }

            writeableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixelData, stride, 0);
            return writeableBitmap;
        }
        public ImageBrush _paintBrush;
        private void InitBrushPolyline(MouseEventArgs e, string brush)
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
            else if (_tempBrushType == BrushType.OilPaintBrush)
            {
                SetImageBrush(_oilBrushPath);
                InitBrushPolyline(e, _checkPath);
            }
            else if (_tempBrushType == BrushType.ColorPencil)
            {
                SetImageBrush(_coloredBrushPath);
                InitBrushPolyline(e, _checkTwo);
            }
            else if (_tempBrushType == BrushType.WatercolorBrush)
            {
                SetImageBrush(_watercolorBrushPath);
                InitBrushPolyline(e, _checkOne);
            }
            else if (_tempBrushType == BrushType.TexturePencil)
            {
                SetImageBrush(_texturePencilBrushPath);
                InitBrushPolyline(e, _checkPath);
            }
        }
        public RenderTargetBitmap _renderBitmap;
        private const int bitsInPixel = 8;

        public void PerformFloodFill(int x, int y, Canvas drawingCanvas)
        {
            const int locCorrector = 1;
            if (x < 0 || x >= _renderBitmap.PixelWidth || y < 0 || y >= _renderBitmap.PixelHeight)
                return;

            int stride = _renderBitmap.PixelWidth * (_renderBitmap.Format.BitsPerPixel / bitsInPixel);
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
            int index = (y * stride) + (x * (_renderBitmap.Format.BitsPerPixel / bitsInPixel));
            pixels[index] = color.B;
            pixels[index + 1] = color.G;
            pixels[index + 2] = color.R;
            pixels[index + 3] = color.A;
        }
        private Color GetPixelColor(byte[] pixels, int stride, int x, int y)
        {
            int index = (y * stride) + (x * (_renderBitmap.Format.BitsPerPixel / bitsInPixel));
            byte blue = pixels[index];
            byte green = pixels[index + 1];
            byte red = pixels[index + 2];
            byte alpha = pixels[index + 3];
            return Color.FromArgb(alpha, red, green, blue);
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
        public Point GetCursLoc(Point tempCursLoc, Canvas drawingCanvas)
        {
            return
            (tempCursLoc.X < 0 || tempCursLoc.Y < 0 ||
            tempCursLoc.X > drawingCanvas.Width ||
            tempCursLoc.Y > drawingCanvas.Height) ? new Point(-1, -1) : tempCursLoc;
        }
    }
}
