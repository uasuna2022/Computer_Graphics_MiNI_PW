using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Project2_BicubicBezierSurface.Algorithms
{
    public static class BernsteinCoefficients
    {
        private static int[][] _binomCoeffs = [[1, 1, 1, 1], [0, 1, 2, 3], [0, 0, 1, 3], [0, 0, 0, 1]]; 
        private static float BinomCoefficient(int i, int n) => _binomCoeffs[i][n];
        public static float Coefficient(float t, int i, int n)
        {
            return (float)Math.Pow(1 - t, n - i) * (float)Math.Pow(t, i) * BinomCoefficient(i, n);
        }
    }
}
