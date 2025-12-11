using Project3_CustomFunctionImageFilter.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project3_CustomFunctionImageFilter
{
    public static class ImageProcessor
    {
        public static void ApplyGlobal(PixelProcessor processor)
        {
            if (EditorWorkspace.Instance.WorkingImage == null || 
                EditorWorkspace.Instance.OriginalImage == null)
                return;
            
            using (FastBitmap fb1 = new FastBitmap(EditorWorkspace.Instance.OriginalImage))
            {
                using (FastBitmap fb2 = new FastBitmap(EditorWorkspace.Instance.WorkingImage))
                {
                    fb1.Lock();
                    fb2.Lock();
                    for (int i = 0; i < EditorWorkspace.Instance.OriginalImage.Width; i++)
                    {
                        for (int j = 0; j < EditorWorkspace.Instance.OriginalImage.Height; j++)
                        {
                            Color color = fb1.GetPixel(i, j);
                            (byte newR, byte newG, byte newB) = processor(color.R, color.G, color.B);

                            Color newColor = Color.FromArgb(color.A, newR, newG, newB);
                            fb2.SetPixel(i, j, newColor);
                        }
                    }
                }
            }
        }
        public static void ApplyBrush(Point center, PixelProcessor processor)
        {
            int radius = EditorWorkspace.Instance.BrushRadius;
            if (EditorWorkspace.Instance.WorkingImage == null ||
                EditorWorkspace.Instance.OriginalImage == null)
                return;

            using (FastBitmap fb1 = new FastBitmap(EditorWorkspace.Instance.OriginalImage))
            {
                using (FastBitmap fb2 = new FastBitmap(EditorWorkspace.Instance.WorkingImage))
                {
                    fb1.Lock();
                    fb2.Lock();

                    int width = EditorWorkspace.Instance.OriginalImage.Width;
                    int height = EditorWorkspace.Instance.OriginalImage.Height;

                    int startX = Math.Max(0, center.X - radius);
                    int startY = Math.Max(0, center.Y - radius);
                    int endX = Math.Min(width, center.X + radius);
                    int endY = Math.Min(height, center.Y + radius);
                    int rSq = radius * radius;

                    for (int i = startX; i <= endX; i++)
                    {
                        for (int j = startY; j <= endY; j++)
                        {
                            int dx = center.X - i;
                            int dy = center.Y - j;

                            if (dx * dx + dy * dy <= rSq)
                            {
                                Color oldColor = fb1.GetPixel(i, j);

                                (byte newR, byte newG, byte newB) = processor(oldColor.R, oldColor.G, oldColor.B);

                                Color newColor = Color.FromArgb(oldColor.A, newR, newG, newB);
                                fb2.SetPixel(i, j, newColor);

                                EditorWorkspace.Instance.UpdatePixelCount(oldColor, newColor);
                            }
                        }
                    }
                }
            }
        }
    }
}
