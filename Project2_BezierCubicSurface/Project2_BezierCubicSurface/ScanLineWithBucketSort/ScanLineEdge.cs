using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Project2_BicubicBezierSurface.ScanLineWithBucketSort
{
    public class ScanLineEdge
    {
        public int MaxY { get; set; }
        public float CurrentX { get; set; }
        public float InverseSlope { get; set; }
        public ScanLineEdge(int maxY, float currentX, float inverseSlope)
        {
            MaxY = maxY;
            CurrentX = currentX;
            InverseSlope = inverseSlope;
        }
    }
}
