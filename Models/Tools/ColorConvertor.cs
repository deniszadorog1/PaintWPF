using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using Color = System.Drawing.Color;

namespace PaintWPF.Models.Tools
{
    public static class ColorConvertor
    {
        public static (int, int, int) RGBtoHCV(System.Windows.Media.Color color)
        {
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));

            double hue = 0;
            double saturation = 0;
            double value = max;

            double delta = max - min;

            if (delta > 0)
            {
                if (max == r)
                {
                    hue = 60 * (((g - b) / delta) % 6);
                }
                else if (max == g)
                {
                    hue = 60 * (((b - r) / delta) + 2);
                }
                else if (max == b)
                {
                    hue = 60 * (((r - g) / delta) + 4);
                }

                if (max > 0)
                {
                    saturation = delta / max;
                }
            }

            int h = (int)Math.Round(hue);
            int s = (int)Math.Round(saturation * 100);
            int v = (int)Math.Round(value * 100);

            return (h, s, v);
        }
        public static (double, double, double) RGBtoHSL(System.Windows.Media.Color color)
        {
            (double H, double S, double L) res;

            int r;
            int g;
            double num4;
            if (color.R > color.G)
            {
                r = color.R;
                g = color.G;
            }
            else
            {
                r = color.G;
                g = color.R;
            }
            if (color.B > r)
            {
                r = color.B;
            }
            else if (color.B < g)
            {
                g = color.B;
            }
            int num3 = r - g;
            res.L = r / 255.0;
            if (r == 0)
            {
                res.S = 0.0;
            }
            else
            {
                res.S = num3 / ((double)r);
            }
            if (num3 == 0)
            {
                num4 = 0.0;
            }
            else
            {
                num4 = 60.0 / num3;
            }
            if (r == color.R)
            {
                if (color.G < color.B)
                {
                    res.H = (360.0 + (num4 * (color.G - color.B))) / 360.0;
                }
                else
                {
                    res.H = (num4 * (color.G - color.B)) / 360.0;
                }
            }
            else if (r == color.G)
            {
                res.H = (120.0 + (num4 * (color.B - color.R))) / 360.0;
            }
            else if (r == color.B)
            {
                res.H = (240.0 + (num4 * (color.R - color.G))) / 360.0;
            }
            else
            {
                res.H = 0.0;
            }
            return res;
        }
        public static System.Windows.Media.Color HSLtoRGB(double h, double s, double l)
        {
            int num2;
            int red = Round(l * 255.0);
            int blue = Round(((1.0 - s) * (l / 1.0)) * 255.0);
            double num4 = (red - blue) / 255.0;
            if ((h >= 0.0) && (h <= 0.16666666666666666))
            {
                num2 = Round((((h - 0.0) * num4) * 1530.0) + blue);
                return System.Windows.Media.Color.FromArgb(255, (byte)red, (byte)num2, (byte)blue);
            }
            if (h <= 0.33333333333333331)
            {
                num2 = Round((-((h - 0.16666666666666666) * num4) * 1530.0) + red);
                return System.Windows.Media.Color.FromArgb(255, (byte)num2, (byte)red, (byte)blue);
            }
            if (h <= 0.5)
            {
                num2 = Round((((h - 0.33333333333333331) * num4) * 1530.0) + blue);
                return System.Windows.Media.Color.FromArgb(255, (byte)blue, (byte)red, (byte)num2);
            }
            if (h <= 0.66666666666666663)
            {
                num2 = Round((-((h - 0.5) * num4) * 1530.0) + red);
                return System.Windows.Media.Color.FromArgb(255, (byte)blue, (byte)num2, (byte)red);
            }
            if (h <= 0.83333333333333337)
            {
                num2 = Round((((h - 0.66666666666666663) * num4) * 1530.0) + blue);
                return System.Windows.Media.Color.FromArgb(255, (byte)num2, (byte)blue, (byte)red);
            }
            if (h <= 1.0)
            {
                num2 = Round((-((h - 0.83333333333333337) * num4) * 1530.0) + red);
                return System.Windows.Media.Color.FromArgb(255, (byte)red, (byte)blue, (byte)num2);
            }
            return System.Windows.Media.Color.FromArgb(100, 0, 0, 0);
        }
        private static int Round(double val)
        {
            return (int)(val + 0.5);
        }
        public static System.Windows.Media.Color HcvToRgb(double h, double c, double v)
        {
            double r = 0, g = 0, b = 0;

            double x = c * (1 - Math.Abs((h / 60.0) % 2 - 1));
            double m = v - c;

            if (h >= 0 && h < 60)
            {
                r = c; g = x; b = 0;
            }
            else if (h >= 60 && h < 120)
            {
                r = x; g = c; b = 0;
            }
            else if (h >= 120 && h < 180)
            {
                r = 0; g = c; b = x;
            }
            else if (h >= 180 && h < 240)
            {
                r = 0; g = x; b = c;
            }
            else if (h >= 240 && h < 300)
            {
                r = x; g = 0; b = c;
            }
            else if (h >= 300 && h < 360)
            {
                r = c; g = 0; b = x;
            }

            r = (r + m) * 255;
            g = (g + m) * 255;
            b = (b + m) * 255;

            return System.Windows.Media.Color.FromRgb((byte)r, (byte)g, (byte)b);
        }
        public static System.Windows.Media.Color HexToRGB(string hex)
        {
            // Удаляем символ '#' если он есть
            hex = hex.Replace("#", "");

            byte a = 255; // по умолчанию полностью непрозрачный
            byte r = 0;
            byte g = 0;
            byte b = 0;

            if (hex.Length == 8) // ARGB
            {
                a = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                r = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                g = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                b = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            }
            else if (hex.Length == 6) // RGB
            {
                r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            }

            return System.Windows.Media.Color.FromArgb(a, r, g, b);
        }


    }
}
