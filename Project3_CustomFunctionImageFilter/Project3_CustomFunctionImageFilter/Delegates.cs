using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project3_CustomFunctionImageFilter
{
    public delegate (byte oldR, byte oldG, byte oldB) PixelProcessor(byte newR, byte newG, byte newB);
}
