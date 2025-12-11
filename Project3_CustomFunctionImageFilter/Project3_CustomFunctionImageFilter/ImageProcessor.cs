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
        public static void ApplyLocally() { } // TODO
    }
}
