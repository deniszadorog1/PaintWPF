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
        public static (int, int, int) RGBtoHSV(System.Windows.Media.Color color)
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
        public static Color HSVToRGB(double hue, double saturation, double value)
        {
            double c = value * saturation;
            double x = c * (1 - Math.Abs((hue / 60) % 2 - 1));
            double m = value - c;

            double r = 0, g = 0, b = 0;

            if (0 <= hue && hue < 60)
            {
                r = c; g = x; b = 0;
            }
            else if (60 <= hue && hue < 120)
            {
                r = x; g = c; b = 0;
            }
            else if (120 <= hue && hue < 180)
            {
                r = 0; g = c; b = x;
            }
            else if (180 <= hue && hue < 240)
            {
                r = 0; g = x; b = c;
            }
            else if (240 <= hue && hue < 300)
            {
                r = x; g = 0; b = c;
            }
            else if (300 <= hue && hue < 360)
            {
                r = c; g = 0; b = x;
            }

            r = (r + m) * 255;
            g = (g + m) * 255;
            b = (b + m) * 255;

            return Color.FromArgb((byte)r, (byte)g, (byte)b);
        }
        public static Color hsvToRgb(double h, double s, double v)
        {
            byte[] rgb = new byte[3];
            double r = 0, g = 0, b = 0;

            int i = (int)(h * 6);
            double f = h * 6 - i;
            double p = v * (1 - s);
            double q = v * (1 - f * s);
            double t = v * (1 - (1 - f) * s);

            switch (i % 6)
            {
                case 0: r = v; g = t; b = p; break;
                case 1: r = q; g = v; b = p; break;
                case 2: r = p; g = v; b = t; break;
                case 3: r = p; g = q; b = v; break;
                case 4: r = t; g = p; b = v; break;
                case 5: r = v; g = p; b = q; break;
            }

            rgb[0] = (byte)(r * 255);
            rgb[1] = (byte)(g * 255);
            rgb[2] = (byte)(b * 255);

            return Color.FromArgb(rgb[0], rgb[1], rgb[2]);
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
