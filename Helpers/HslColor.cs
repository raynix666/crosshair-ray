
using Avalonia.Media;
using System;

namespace CrosshairApp.Helpers
{
    public struct HslColor
    {
        public double H { get; set; }
        public double S { get; set; }
        public double L { get; set; }

        public HslColor(double h, double s, double l)
        {
            H = h;
            S = s;
            L = l;
        }

        public Color ToRgb()
        {
            double r = 0, g = 0, b = 0;
            if (S == 0)
            {
                r = g = b = L; // achromatic
            }
            else
            {
                double q = L < 0.5 ? L * (1 + S) : L + S - L * S;
                double p = 2 * L - q;
                r = HueToRgb(p, q, H / 360 + 1.0 / 3.0);
                g = HueToRgb(p, q, H / 360);
                b = HueToRgb(p, q, H / 360 - 1.0 / 3.0);
            }

            return Color.FromArgb(255, (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }

        private static double HueToRgb(double p, double q, double t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1.0 / 6.0) return p + (q - p) * 6.0 * t;
            if (t < 1.0 / 2.0) return q;
            if (t < 2.0 / 3.0) return p + (q - p) * (2.0 / 3.0 - t) * 6.0;
            return p;
        }

        public HslColor WithHue(double newHue)
        {
            return new HslColor(newHue, S, L);
        }
    }
}
