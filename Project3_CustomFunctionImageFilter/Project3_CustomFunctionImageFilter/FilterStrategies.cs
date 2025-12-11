using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project3_CustomFunctionImageFilter.Helpers
{
    public static class FilterStrategies
    {
        public static PixelProcessor GetNoFilter()
        {
            return (byte r, byte g, byte b) =>
            {
                return (r, g, b);
            };
        }
        public static PixelProcessor GetNegationFilter()
        {
            return (byte r, byte g, byte b) =>
            {
                return ((byte)(byte.MaxValue - r), (byte)(byte.MaxValue - g), (byte)(byte.MaxValue - b));
            };
        }
        public static PixelProcessor GetBrightnessFilter(int delta)
        {
            return (byte r, byte g, byte b) =>
            {
                byte newR = Clamp(r + delta);
                byte newG = Clamp(g + delta);
                byte newB = Clamp(b + delta);
                return (newR, newG, newB);
            };
        }
        public static PixelProcessor GetContrastFilter(int delta)
        {
            float tangentCoefficient = (delta >= 0) ? (byte.MaxValue / (byte.MaxValue - 2.0F * delta)) :
                ((byte.MaxValue + 2.0F * delta) / byte.MaxValue);
            bool deltaIsPostive = delta >= 0;
            return (byte r, byte g, byte b) =>
            {
                if (deltaIsPostive)
                {
                    int newR = (int)((r > delta && r < byte.MaxValue - delta) ?
                        (tangentCoefficient * (r - delta)) : ((r <= delta) ? byte.MinValue : byte.MaxValue));
                    int newG = (int)((g > delta && g < byte.MaxValue - delta) ?
                        (tangentCoefficient * (g - delta)) : ((g <= delta) ? byte.MinValue : byte.MaxValue));
                    int newB = (int)((b > delta && b < byte.MaxValue - delta) ?
                        (tangentCoefficient * (b - delta)) : ((b <= delta) ? byte.MinValue : byte.MaxValue));
                    return (Clamp(newR), Clamp(newG), Clamp(newB));
                }
                else
                {
                    int newR = (int)(tangentCoefficient * r - delta);
                    int newG = (int)(tangentCoefficient * g - delta);
                    int newB = (int)(tangentCoefficient * b - delta);
                    return (Clamp(newR), Clamp(newG), Clamp(newB));  
                }
            };
        }
        public static PixelProcessor GetGammaFilter(float gamma)
        {
            byte[] gammaValues = new byte[256];
            for (int i = 0; i <= 255; i++)
            {
                float x = i / 255.0F;
                gammaValues[i] = Clamp((int)(Math.Pow(x, gamma) * 255.0F));
            }

            return (byte r, byte g, byte b) =>
            {
                return (gammaValues[r], gammaValues[g], gammaValues[b]);
            };
        }
        public static PixelProcessor GetCustomFunctionFilter()
        {
            return GetNoFilter();
            // TODO: apply custom function logic and calculate a color
        }

        private static byte Clamp(int value)
        {
            if (value < 0)
                value = 0;
            if (value > 255)
                value = 255;
            return (byte)value;
        }
    }
}
