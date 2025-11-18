using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Project2_BicubicBezierSurface.Models
{
    public sealed class Triangle
    {
        public int V1ID { get; set; }
        public int V2ID { get; set; }
        public int V3ID { get; set; }

        public Triangle(int v1ID, int v2ID, int v3ID)
        {
            V1ID = v1ID;
            V2ID = v2ID;
            V3ID = v3ID;
        }
    }
}
