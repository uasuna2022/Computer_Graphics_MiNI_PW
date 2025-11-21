using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Project2_BicubicBezierSurface.Helpers
{
    public class FastBitmap : IDisposable
    {
        public Bitmap Bitmap { get; private set; }
        private BitmapData? _bitmapData;
        private byte[]? _pixels;
        private IntPtr _ptr;
        private int _width;
        private int _height;
        private bool _isLocked = false;

        public FastBitmap(int width, int height)
        {
            _width = width;
            _height = height;
            // Format32bppArgb is easiest (4 bytes: B, G, R, A)
            Bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        }

        public void Lock()
        {
            if (_isLocked) 
                return;
            Rectangle rect = new Rectangle(0, 0, _width, _height);
            _bitmapData = Bitmap.LockBits(rect, ImageLockMode.ReadWrite, Bitmap.PixelFormat);
            _ptr = _bitmapData.Scan0;
            int bytes = Math.Abs(_bitmapData.Stride) * _height;
            _pixels = new byte[bytes];
            Marshal.Copy(_ptr, _pixels, 0, bytes);
            _isLocked = true;
        }

        public void Unlock()
        {
            if (!_isLocked) 
                return;
            Marshal.Copy(_pixels!, 0, _ptr, _pixels!.Length);
            Bitmap.UnlockBits(_bitmapData!);
            _isLocked = false;
        }

        // Safe SetPixel that ignores coordinates outside the screen
        public void SetPixel(int x, int y, Color color)
        {
            if (!_isLocked) 
                return;
            if (x < 0 || x >= _width || y < 0 || y >= _height) 
                return;

            // Calculate index in the 1D byte array
            // Stride is the real width of the image row in memory
            int index = (y * _bitmapData!.Stride) + (x * 4);

            _pixels![index] = color.B;     // Blue
            _pixels[index + 1] = color.G; // Green
            _pixels[index + 2] = color.R; // Red
            _pixels[index + 3] = 255;     // Alpha (Fully opaque)
        }

        public void Dispose()
        {
            if (_isLocked) 
                Unlock();
            Bitmap?.Dispose();
        }
    }
}
