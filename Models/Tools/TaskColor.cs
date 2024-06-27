using System;
using System.Collections.Generic;
using System.Linq;
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
            Alpha = 255;
            Hue = h;
            Saturation = s;
            Luminance = l;
        }
        public TaskColor(Color color)
        {
            Alpha = 255;
            Hue = 0.0;
            Saturation = 0.0;
            Luminance = 0.0;
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
            return new TaskColor(a, 0.0, 0.0, 0.0);
        }

        public static TaskColor FromAhsl(int a, TaskColor hsl)
        {
            return new TaskColor(a, hsl.Hue, hsl.Saturation, hsl.Luminance);
        }

        public static TaskColor FromAhsl(double h, double s, double l)
        {
            return new TaskColor(255, h, s, l);
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
                Hue = (Hue > 1.0) ? 1.0 : ((Hue < 0.0) ? 0.0 : Hue);
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
                Saturation = (Saturation > 1.0) ? 1.0 : ((Saturation < 0.0) ? 0.0 : Saturation);
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
                Luminance = (Luminance > 1.0) ? 1.0 : ((Luminance < 0.0) ? 0.0 : Luminance);
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
                Alpha = (value > 255) ? 255 : ((value < 0) ? 0 : value);
            }
        }

        public bool IsEmpty
        {
            get
            {
                return ((((Alpha == 0) && (H == 0.0)) && (S == 0.0)) && (L == 0.0));
            }
        }

        public Color ToRgbColor()
        {
            return ToRgbColor(A);
        }

        public Color ToRgbColor(int alpha)
        {
            double q;
            if (L < 0.5)
            {
                q = L * (1 + S);
            }
            else
            {
                q = L + S - (L * S);
            }
            double p = 2 * L - q;
            double hk = H / 360;

            // r,g,b colors
            double[] tc = new[]
                    {
                      hk + (1d / 3d), hk, hk - (1d / 3d)
                    };
            double[] colors = new[]
                        {
                          0.0, 0.0, 0.0
                        };

            for (int color = 0; color < colors.Length; color++)
            {
                if (tc[color] < 0)
                {
                    tc[color] += 1;
                }
                if (tc[color] > 1)
                {
                    tc[color] -= 1;
                }

                if (tc[color] < (1d / 6d))
                {
                    colors[color] = p + ((q - p) * 6 * tc[color]);
                }
                else if (tc[color] >= (1d / 6d) && tc[color] < (1d / 2d))
                {
                    colors[color] = q;
                }
                else if (tc[color] >= (1d / 2d) && tc[color] < (2d / 3d))
                {
                    colors[color] = p + ((q - p) * 6 * (2d / 3d - tc[color]));
                }
                else
                {
                    colors[color] = p;
                }

                colors[color] *= 255;
            }
            return Color.FromArgb((byte)alpha, (byte)colors[0], (byte)colors[1], (byte)colors[2]);
        }
        public Color HSLtoRGB()
        {
            Color res = new Color();

            R = Round(Luminance * 255.0);
            B = Round(((1.0 - Saturation) * (Luminance / 1.0)) * 255.0);
            double num4 = (R - B) / 255.0;

            if ((Hue >= 0.0) && (Hue <= 0.16666666666666666))
            {
                G = Round((((Hue - 0.0) * num4) * 1530.0) + B);
                res = Color.FromArgb((byte)Alpha, (byte)R, (byte)G, (byte)B);
            }
            else if (Hue <= 0.33333333333333331)
            {
                G = Round((-((Hue - 0.16666666666666666) * num4) * 1530.0) + R);
                res = Color.FromArgb((byte)Alpha, (byte)G, (byte)R, (byte)B);
            }
            else if (Hue <= 0.5)
            {
                G = Round((((Hue - 0.33333333333333331) * num4) * 1530.0) + B);
                res = Color.FromArgb((byte)Alpha, (byte)B, (byte)R, (byte)G);
            }
            else if (Hue <= 0.66666666666666663)
            {
                G = Round((-((Hue - 0.5) * num4) * 1530.0) + R);
                res = Color.FromArgb((byte)Alpha, (byte)B, (byte)G, (byte)R);
            }
            else if (Hue <= 0.83333333333333337)
            {
                G = Round((((Hue - 0.66666666666666663) * num4) * 1530.0) + B);
                res = Color.FromArgb((byte)Alpha, (byte)G, (byte)B, (byte)R);
            }
            else if (Hue <= 1.0)
            {
                G = Round((-((Hue - 0.83333333333333337) * num4) * 1530.0) + R);
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
            Luminance = R / 255.0;
            if (R == 0)
            {
                Saturation = 0.0;
            }
            else
            {
                Saturation = num3 / ((double)R);
            }
            if (num3 == 0)
            {
                num4 = 0.0;
            }
            else
            {
                num4 = 60.0 / num3;
            }
            if (R == color.R)
            {
                if (color.G < color.B)
                {
                    Hue = (360.0 + (num4 * (color.G - color.B))) / 360.0;
                }
                else
                {
                    Hue = (num4 * (color.G - color.B)) / 360.0;
                }
            }
            else if (R == color.G)
            {
                Hue = (120.0 + (num4 * (color.B - color.R))) / 360.0;
            }
            else if (R == color.B)
            {
                Hue = (240.0 + (num4 * (color.R - color.G))) / 360.0;
            }
            else
            {
                Hue = 0.0;
            }
        }

        private int Round(double val)
        {
            return (int)(val + 0.5);
        }
    }
}
