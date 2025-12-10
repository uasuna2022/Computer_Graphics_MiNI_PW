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
    public unsafe class FastBitmap : IDisposable
    {
        public Bitmap Bitmap { get; set; }

        private bool _isLocked = false;
        private BitmapData? _bitmapData;
        private byte* _ptr;
        private int _width;
        private int _height;
        private int _bytesPerPixel;

        public FastBitmap(Bitmap bitmap)
        {
            Bitmap = bitmap;
            _width = bitmap.Width;
            _height = bitmap.Height;
        }
        public FastBitmap(int width, int height)
        {
            _width = width;
            _height = height;
            Bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        }
        public void Lock()
        {
            if (_isLocked)
                return;

            Rectangle rect = new Rectangle(0, 0, _width, _height);
            _bitmapData = Bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            _ptr = (byte*)_bitmapData.Scan0;

            _bytesPerPixel = 4;
            _isLocked = true;
        }
        public void Unlock()
        {
            if (!_isLocked)
                return;

            Bitmap.UnlockBits(_bitmapData!);
            _isLocked = false;
            _bitmapData = null;
            _ptr = null;
        }
        public Color GetPixel(int x, int y)
        {
            if (x < 0 || y < 0 || x >= _width || y >= _height)
                return Color.Transparent;

            byte* currentPixel = _ptr + (y * _bitmapData!.Stride) + (x * _bytesPerPixel);

            byte b = currentPixel[0];
            byte g = currentPixel[1];
            byte r = currentPixel[2];
            byte a = currentPixel[3];

            return Color.FromArgb(a, r, g, b);
        }
        public void SetPixel(int x, int y, Color color)
        {
            if (x < 0 || y < 0 || x >= _width || y >= _height)
                return;

            byte* currentPixel = _ptr + (y * _bitmapData!.Stride) + (x * _bytesPerPixel);

            currentPixel[0] = color.B;
            currentPixel[1] = color.G;
            currentPixel[2] = color.R;
            currentPixel[3] = color.A;
        }
        
        

            
        public void Dispose()
        {
            if (_isLocked)
                Unlock();
        }
    }
}
