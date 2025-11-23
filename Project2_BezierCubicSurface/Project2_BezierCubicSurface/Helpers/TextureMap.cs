using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Project2_BicubicBezierSurface.Helpers
{
    public class TextureMap : IDisposable
    {
        private Bitmap _bitmap;
        private byte[]? _pixelData;
        private int _width;
        private int _height;
        private int _stride;
        private BitmapData? _bmpData;

        public TextureMap(string filePath)
        {
            using (Bitmap? temp = new Bitmap(filePath))
            {

                _bitmap = new Bitmap(temp.Width, temp.Height, PixelFormat.Format32bppArgb);
                using (var g = Graphics.FromImage(_bitmap))
                {
                    g.DrawImage(temp, 0, 0, temp.Width, temp.Height);
                }
            }

            _width = _bitmap.Width;
            _height = _bitmap.Height;

            LockBits();
        }

        private void LockBits()
        {
            Rectangle rect = new Rectangle(0, 0, _width, _height);
            _bmpData = _bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            _stride = _bmpData.Stride;
            int bytes = Math.Abs(_stride) * _height;
            _pixelData = new byte[bytes];
            Marshal.Copy(_bmpData.Scan0, _pixelData, 0, bytes);
        }

        public Vector3 GetColorAtUV(float u, float v)
        {
            u = Math.Clamp(u, 0f, 1f);
            v = Math.Clamp(v, 0f, 1f);

            int x = (int)(u * (_width - 1));
            int y = (int)(v * (_height - 1));

            int index = (y * _stride) + (x * 4);

            float b = _pixelData![index] / 255.0f;
            float g = _pixelData[index + 1] / 255.0f;
            float r = _pixelData[index + 2] / 255.0f;

            return new Vector3(r, g, b);
        }

        public Vector3 GetNormalFromMap(float u, float v)
        {
            Vector3 color = GetColorAtUV(u, v);

            // convert U,V from [0,1] to [-1,1] interval
            float nx = 2.0f * color.X - 1.0f; 
            float ny = 2.0f * color.Y - 1.0f; 
            float nz = 2.0f * color.Z - 1.0f; 

            return new Vector3(nx, ny, nz);
        }

        public void Dispose()
        {
            if (_bitmap != null)
            {
                _bitmap.UnlockBits(_bmpData!);
                _bitmap.Dispose();
            }
        }
    }
}
