using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PaintWPF.Models.Tools
{
    public class TaskColor
    {
        public static readonly TaskColor Empty;
        public double Hue { get; set; }
        public double Saturation { get; set; }
        public double Luminance { get; set; }
        public int Alpha { get; set; }
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }

        private const int _maxRGB = byte.MaxValue;
        const double _maxRGBNotRounded = 255.0;

        const double _startLittleStep = 0.0;
        const double _endLittleStep = 1.0;

        public TaskColor(int a, double h, double s, double l)
        {
            Alpha = a;
            Hue = h;
            Saturation = s;
            Luminance = l;
            A = a;
            H = Hue;
            S = Saturation;
            L = Luminance;

        }

        public TaskColor(double h, double s, double l)
        {
            Alpha = _maxRGB;
            Hue = h;
            Saturation = s;
            Luminance = l;
        }

        public TaskColor(Color color)
        {
            Alpha = _maxRGB;
            Hue = _startLittleStep;
            Saturation = _startLittleStep;
            Luminance = _startLittleStep;
            RGBtoHSL(color);
        }

        public TaskColor FromArgb(int a, int r, int g, int b)
        {
            return new TaskColor(Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b));
        }

        public static TaskColor FromColor(Color color)
        {
            return new TaskColor(color);
        }

        public static TaskColor FromAhsl(int a)
        {
            return new TaskColor(a, _startLittleStep, _startLittleStep, _startLittleStep);
        }

        public static TaskColor FromAhsl(int a, TaskColor hsl)
        {
            return new TaskColor(a, hsl.Hue, hsl.Saturation, hsl.Luminance);
        }

        public static TaskColor FromAhsl(double h, double s, double l)
        {
            return new TaskColor(_maxRGB, h, s, l);
        }

        public static TaskColor FromAhsl(int a, double h, double s, double l)
        {
            return new TaskColor(a, h, s, l);
        }

        public static bool operator ==(TaskColor left, TaskColor right)
        {
            return (((left.A == right.A) && (left.H == right.H)) && ((left.S == right.S) && (left.L == right.L)));
        }

        public static bool operator !=(TaskColor left, TaskColor right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj is TaskColor)
            {
                TaskColor color = (TaskColor)obj;
                if (((A == color.A) && (H == color.H)) && ((S == color.S) && (L == color.L)))
                {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (((Alpha.GetHashCode() ^ Hue.GetHashCode()) ^ Saturation.GetHashCode()) ^ Luminance.GetHashCode());
        }

        public double H
        {
            get
            {
                return Hue;
            }
            set
            {
                Hue = value;
                Hue = (Hue > _endLittleStep) ? _endLittleStep : 
                    ((Hue < _startLittleStep) ? _startLittleStep : Hue);
            }
        }

        public double S
        {
            get
            {
                return Saturation;
            }
            set
            {
                Saturation = value;
                Saturation = (Saturation > _endLittleStep) ? _endLittleStep : 
                    ((Saturation < _startLittleStep) ? _startLittleStep : Saturation);
            }
        }

        public double L
        {
            get
            {
                return Luminance;
            }
            set
            {
                Luminance = value;
                Luminance = (Luminance > _endLittleStep) ? _endLittleStep : 
                    ((Luminance < _startLittleStep) ? _startLittleStep : Luminance);
            }
        }

        public Color RgbValue
        {
            get
            {
                return HSLtoRGB();
            }
            set
            {
                RGBtoHSL(value);
            }
        }

        public int A
        {
            get
            {
                return Alpha;
            }
            set
            {
                Alpha = (value > _maxRGB) ? _maxRGB : ((value < 0) ? 0 : value);
            }
        }

        public bool IsEmpty
        {
            get
            {
                return ((((Alpha == 0) && (H == _startLittleStep)) && (S == _startLittleStep)) && (L == _startLittleStep));
            }
        }

        public Color ToRgbColor()
        {
            return ToRgbColor(A);
        }

        public Color ToRgbColor(int alpha)
        {
            const int satAdder = 1;
            const double lengthBorder = 0.5;
            const int lengthMult = 2;
            const double firstDVal = 1d;
            const double secondDVal = 2d;
            const double threeDVal = 3d;
            const double fourDVal = 6d;
            const double valMultiplier = 6;
            const int hueDevider = 360;
            const int colorColrel = 1;

            
            double q;
            if (L < lengthBorder)
            {
                q = L * (satAdder + S);
            }
            else
            {
                q = L + S - (L * S);
            }
            double p = lengthMult * L - q;
            double hk = H / hueDevider;

            // r,g,b colors
            double[] tc = new[]
                    {
                      hk + (firstDVal / threeDVal), hk, hk - (firstDVal / threeDVal)
                    };
            double[] colors = new[]
                        {
                          _startLittleStep, _startLittleStep, _startLittleStep
                        };

            for (int color = 0; color < colors.Length; color++)
            {
                if (tc[color] < 0)
                {
                    tc[color] += colorColrel;
                }
                if (tc[color] > colorColrel)
                {
                    tc[color] -= colorColrel;
                }

                if (tc[color] < (firstDVal / fourDVal))
                {
                    colors[color] = p + ((q - p) * valMultiplier * tc[color]);
                }
                else if (tc[color] >= (firstDVal / fourDVal) && tc[color] < (firstDVal / secondDVal))
                {
                    colors[color] = q;
                }
                else if (tc[color] >= (firstDVal / secondDVal) && tc[color] < (secondDVal / threeDVal))
                {
                    colors[color] = p + ((q - p) * valMultiplier * (secondDVal / threeDVal - tc[color]));
                }
                else
                {
                    colors[color] = p;
                }

                colors[color] *= _maxRGB;
            }
            return Color.FromArgb((byte)alpha, (byte)colors[0], (byte)colors[1], (byte)colors[2]);
        }

        public Color HSLtoRGB()
        {
            const double startHue = 0.0;
            const double firstStepHue = 0.16666666666666666;
            const double secondStepHue = 0.33333333333333331;
            const double thirdSteoHue = 0.5;
            const double fourthStepHue = 0.66666666666666663;
            const double fifthStepHue = 0.83333333333333337;
            const double sixthStepHue = 1.0;
            const double colorMultiplier = 1530.0;

            Color res = new Color();

            R = Round(Luminance * _maxRGBNotRounded);
            B = Round(((sixthStepHue - Saturation) * (Luminance / sixthStepHue)) * _maxRGBNotRounded);
            double num4 = (R - B) / _maxRGBNotRounded;

            if ((Hue >= startHue) && (Hue <= firstStepHue))
            {
                G = Round((((Hue - startHue) * num4) * colorMultiplier) + B);
                res = Color.FromArgb((byte)Alpha, (byte)R, (byte)G, (byte)B);
            }
            else if (Hue <= secondStepHue)
            {
                G = Round((-((Hue - firstStepHue) * num4) * colorMultiplier) + R);
                res = Color.FromArgb((byte)Alpha, (byte)G, (byte)R, (byte)B);
            }
            else if (Hue <= thirdSteoHue)
            {
                G = Round((((Hue - secondStepHue) * num4) * colorMultiplier) + B);
                res = Color.FromArgb((byte)Alpha, (byte)B, (byte)R, (byte)G);
            }
            else if (Hue <= fourthStepHue)
            {
                G = Round((-((Hue - thirdSteoHue) * num4) * colorMultiplier) + R);
                res = Color.FromArgb((byte)Alpha, (byte)B, (byte)G, (byte)R);
            }
            else if (Hue <= fifthStepHue)
            {
                G = Round((((Hue - fourthStepHue) * num4) * colorMultiplier) + B);
                res = Color.FromArgb((byte)Alpha, (byte)G, (byte)B, (byte)R);
            }
            else if (Hue <= sixthStepHue)
            {
                G = Round((-((Hue - fifthStepHue) * num4) * colorMultiplier) + R);
                res = Color.FromArgb((byte)Alpha, (byte)R, (byte)B, (byte)G);
            }
            if (res != new Color())
            {
                A = ((Color)res).A;
                R = ((Color)res).R;
                G = ((Color)res).G;
                B = ((Color)res).B;
                return res;
            }
            return Color.FromArgb((byte)Alpha, 0, 0, 0);
        }

        public void RGBtoHSL(Color color)
        {
            const double firstStep = 60.0;
            const double secondStep = 120.0;
            const double thirdStep = 240.0;
            const double fourthStep = 360.0;

            Alpha = color.A;
            R = color.R;
            G = color.G;
            B = color.B;
            double num4;
            if (color.R > color.G)
            {
                R = color.R;
                G = color.G;
            }
            else
            {
                R = color.G;
                G = color.R;
            }
            if (color.B > R)
            {
                R = color.B;
            }
            else if (color.B < G)
            {
                G = color.B;
            }
            int num3 = R - G;
            Luminance = R / _maxRGBNotRounded;
            if (R == 0)
            {
                Saturation = _startLittleStep;
            }
            else
            {
                Saturation = num3 / ((double)R);
            }
            if (num3 == 0)
            {
                num4 = _startLittleStep;
            }
            else
            {
                num4 = firstStep / num3;
            }
            if (R == color.R)
            {
                if (color.G < color.B)
                {
                    Hue = (fourthStep + (num4 * (color.G - color.B))) / fourthStep;
                }
                else
                {
                    Hue = (num4 * (color.G - color.B)) / fourthStep;
                }
            }
            else if (R == color.G)
            {
                Hue = (secondStep + (num4 * (color.B - color.R))) / fourthStep;
            }
            else if (R == color.B)
            {
                Hue = (thirdStep + (num4 * (color.R - color.G))) / fourthStep;
            }
            else
            {
                Hue = _startLittleStep;
            }
        }

        public string GetHexFromRGB()
        {
            return $"#{R:X2}{G:X2}{B:X2}";
        }

        private const int _hexLength = 6;
        private const string _hexSymbols = "0123456789ABCDEFabcdef";
        public bool IfNeedToConvertHexIntoRGB(string hex)
        {
            const int startSubPoint = 0;
            const int firstStepSubPoint = 2;
            const int seondStepSubPoint = 4;

            hex = hex.Replace("#", "");
            if (hex == string.Empty) return false;

            hex = HexFilling(hex);

            if (!hex.All(x => _hexSymbols.Contains(x))) return false;

            R = byte.Parse(hex.Substring(startSubPoint, firstStepSubPoint), System.Globalization.NumberStyles.HexNumber);
            G = byte.Parse(hex.Substring(firstStepSubPoint, firstStepSubPoint), System.Globalization.NumberStyles.HexNumber);
            B = byte.Parse(hex.Substring(seondStepSubPoint, firstStepSubPoint), System.Globalization.NumberStyles.HexNumber);
            return true;
        }

        private string HexFilling(string tempHex)
        {
            string res = tempHex;
            if (res.Length == _hexLength) return res;

            int differ = res.Length;
            for (int i = 0; i < _hexLength - differ; i++)
            {
                res = res.Insert(0, "0");
            }
            return res;
        }

        private int Round(double val)
        {
            const double colorRounder = 0.5;
            return (int)(val + colorRounder);
        }

        public static System.Windows.Media.Color HexToRGB(string hex)
        {
            const int ARGBHexLength = 8;
            const int RGBHexLength = 6;
            const int substLength = 2;
            const int firstSubsStep = 0;
            const int twoSubsStep = 2;
            const int threeSubsStep = 4;
            const int fourSubsStep = 6;

            hex = hex.Replace("#", "");

            byte a = _maxRGB;
            byte r = 0;
            byte g = 0;
            byte b = 0;

            if (hex.Length == ARGBHexLength) // ARGB
            {
                a = byte.Parse(hex.Substring(firstSubsStep, substLength), System.Globalization.NumberStyles.HexNumber);
                r = byte.Parse(hex.Substring(twoSubsStep, substLength), System.Globalization.NumberStyles.HexNumber);
                g = byte.Parse(hex.Substring(threeSubsStep, substLength), System.Globalization.NumberStyles.HexNumber);
                b = byte.Parse(hex.Substring(fourSubsStep, substLength), System.Globalization.NumberStyles.HexNumber);
            }
            else if (hex.Length == RGBHexLength) // RGB
            {
                r = byte.Parse(hex.Substring(firstSubsStep, substLength), System.Globalization.NumberStyles.HexNumber);
                g = byte.Parse(hex.Substring(twoSubsStep, substLength), System.Globalization.NumberStyles.HexNumber);
                b = byte.Parse(hex.Substring(threeSubsStep, substLength), System.Globalization.NumberStyles.HexNumber);
            }
            return System.Windows.Media.Color.FromArgb(a, r, g, b);
        }
    }
}
