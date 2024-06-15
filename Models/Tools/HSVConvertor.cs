using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintWPF.Models.Tools
{
    public static class HSVConvertor
    {
        public static (int Hue, int Saturation, int Value) RGBtoHSV(Color color)
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

            return ((int)Math.Round(hue), (int)Math.Round(saturation * 100), (int)Math.Round(value * 100));
        }
    }
}
