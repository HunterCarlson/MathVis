using System;
using System.Drawing;

namespace MathVis
{
    public class ColorRGB
    {
        public byte A;
        public byte B;
        public byte G;
        public byte R;

        public ColorRGB()
        {
            R = 255;
            G = 255;
            B = 255;
            A = 255;
        }

        public ColorRGB(Color value)
        {
            R = value.R;
            G = value.G;
            B = value.B;
            A = value.A;
        }

        // Hue in range from 0.0 to 1.0
        // Use System.Drawing.Color.GetHue, but divide by 360.0F 
        // because System.Drawing.Color returns hue in degrees (0 - 360)
        // rather than a number between 0 and 1.
        public float H => ((Color) this).GetHue() / 360.0F;

        // Saturation in range 0.0 - 1.0
        public float S => ((Color) this).GetSaturation();

        // Lightness in range 0.0 - 1.0
        public float L => ((Color) this).GetBrightness();

        public static implicit operator Color(ColorRGB rgb)
        {
            Color c = Color.FromArgb(rgb.A, rgb.R, rgb.G, rgb.B);
            return c;
        }

        public static implicit operator System.Windows.Media.Color(ColorRGB rgb)
        {
            System.Windows.Media.Color c = System.Windows.Media.Color.FromArgb(rgb.A, rgb.R, rgb.G, rgb.B);
            return c;
        }

        public static explicit operator ColorRGB(Color c)
        {
            return new ColorRGB(c);
        }

        // Given H,S,L in range of 0-1
        // Returns a Color (RGB struct) in range of 0-255
        // ReSharper disable once InconsistentNaming
        public static ColorRGB FromHSL(double h, double s, double l)
        {
            return FromHSLA(h, s, l, 1.0);
        }

        // Given H,S,L,A in range of 0-1
        // Returns a Color (RGB struct) in range of 0-255
        public static ColorRGB FromHSLA(double h, double s, double l, double a)
        {
            if (a > 1.0)
            {
                a = 1.0;
            }

            double r = l;
            double g = l;
            double b = l;
            double v = l <= 0.5 ? l * (1.0 + s) : l + s - l * s;

            if (v > 0)
            {
                double m = l + l - v;
                double sv = (v - m) / v;
                h *= 6.0;
                int sextant = (int) h;
                double fract = h - sextant;
                double vsf = v * sv * fract;
                double mid1 = m + vsf;
                double mid2 = v - vsf;

                switch (sextant)
                {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;
                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;
                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;
                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;
                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;
                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;
                }
            }

            var rgb = new ColorRGB
            {
                R = Convert.ToByte(r * 255.0f),
                G = Convert.ToByte(g * 255.0f),
                B = Convert.ToByte(b * 255.0f),
                A = Convert.ToByte(a * 255.0f)
            };
            return rgb;
        }
    }
}