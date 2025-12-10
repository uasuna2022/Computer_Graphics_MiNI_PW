using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Project3_CustomFunctionImageFilter.Helpers;

namespace Project3_CustomFunctionImageFilter
{
    public sealed class EditorWorkspace : IDisposable
    {
        private static EditorWorkspace? _instance;
        public static EditorWorkspace Instance => _instance ??= new EditorWorkspace();

        private EditorWorkspace() { }
        
        public Bitmap? OriginalImage { get; private set; }
        public Bitmap? WorkingImage { get; set; }

        public float Gamma { get; set; } = 1.0F; // [0.1; 5.0]
        public int Brightness { get; set; } = 0; // [-230; 230]
        public int Contrast { get; set; } = 0;   // [-127; 127]

        public bool NoFilter { get; set; } = true;
        public bool NegationFilter { get; set; } = false;
        public bool CustomFunctionFilter { get; set; } = false;
        public bool GammaFilter { get; set; } = false;
        public bool BrightnessFilter { get; set; } = false;
        public bool ContrastFilter { get; set; } = false;

        public int[] RedData { get; set; } = new int[256];
        public int[] GreenData { get; set; } = new int[256];
        public int[] BlueData { get; set; } = new int[256];

        public void SetImage(Bitmap image)
        {
            OriginalImage?.Dispose();
            WorkingImage?.Dispose();

            OriginalImage = new Bitmap(image);
            WorkingImage = new Bitmap(image);
        }
        public void Dispose()
        {
            OriginalImage?.Dispose();
            WorkingImage?.Dispose();
        }
        public void CountPixels()
        {
            if (WorkingImage == null)
            {
                Array.Clear(BlueData, 0, 256);
                Array.Clear(GreenData, 0, 256);
                Array.Clear(RedData, 0, 256);
                return;
            }

            using (FastBitmap fb = new FastBitmap(WorkingImage))
            {
                for (int i = 0; i < WorkingImage.Width; i++)
                {
                    for (int j = 0; j < WorkingImage.Height; j++)
                    {
                        fb.Lock();
                        Color c = fb.GetPixel(i, j);
                        RedData[c.A]++;
                        GreenData[c.G]++;
                        BlueData[c.B]++;
                    }
                }
            }
        }
    }
}
