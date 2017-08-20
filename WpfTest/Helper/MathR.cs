using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfTest
{
    public static class MathR
    {
        public static float Clamp01(float value)
        {
            if ((double)value < 0.0)
                return 0.0f;
            if ((double)value > 1.0)
                return 1f;
            return value;
        }

        public static double Clamp01(double value)
        {
            if (value < 0.0)
                return 0.0d;
            if (value > 1.0)
                return 1d;
            return value;
        }

        public static float Lerp(float value1, float value2, float ammount)
        {
            return value1 + (value2 - value1) * MathR.Clamp01(ammount);
        }

        public static double Lerp(double value1, double value2, double ammount)
        {
            return value1 + (value2 - value1) * MathR.Clamp01(ammount);
        }

        public static Color Lerp(Color color1, Color color2, double ammount)
        {
            var col = new Color();

            col.R = (byte) Lerp((int) color1.R, (int)color2.R, ammount);
            col.G = (byte) Lerp((int) color1.G, (int)color2.G, ammount);
            col.B = (byte) Lerp((int) color1.B, (int)color2.B, ammount);
            col.A = (byte) Lerp((int) color1.A, (int)color2.A, ammount);

            return col;
        }

        public static float InverseLerp(float from, float to, float value)
        {
            if (from < to)
            {
                if (value < from)
                    return 0.0f;
                else if (value > to)
                    return 1.0f;
            }
            else
            {
                if (value < to)
                    return 1.0f;
                else if (value > from)
                    return 0.0f;
            }
            return (value - from) / (to - from);
        }

        public static double InverseLerp(double from, double to, double value)
        {
            if (from < to)
            {
                if (value < from)
                    return 0.0d;
                else if (value > to)
                    return 1.0d;
            }
            else
            {
                if (value < to)
                    return 1.0d;
                else if (value > from)
                    return 0.0d;
            }
            return (value - from) / (to - from);
        }
    }
}
