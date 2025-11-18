using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Project2_BicubicBezierSurface.Models;

namespace Project2_BicubicBezierSurface.Algorithms
{
    public static class GenerateMesh
    {
        public static void GenerateMesh(int resolution)
        {
            if (Mesh.Instance.ControlPoints == null)
                return;

            int pointCount = resolution + 1;
            Vector3[,] vertices = new Vector3[pointCount, pointCount];
            List<Triangle> triangles = new List<Triangle>();

            int vertexID = 0;
            for (int i = 0; i < pointCount; i++)
            {
                for (int j = 0; j < pointCount; j++)
                {
                    float u = (float)i / resolution;
                    float v = (float)j / resolution;

                }
            }

        }

        public static Vector3 CalculateBezierPoint()
    }
}
