using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Project3_CustomFunctionImageFilter.Helpers
{
    public class FastBitmap : IDisposable
    {
        public Bitmap Bitmap { get; set; }

        private bool _isLocked = false;
        private BitmapData? bitmapData;
        private byte[]? _pixels;
        private int _width;
        private int _height;
        private IntPtr _ptr;

        public FastBitmap(int width, int height)
        {
            _width = width;
            _height = height;
            Bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        }
        public void Dispose()
        {

        }
    }
}
