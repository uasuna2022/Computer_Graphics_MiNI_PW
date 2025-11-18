using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Project2_BicubicBezierSurface.Models
{
    public sealed class Mesh
    {
        public Vector3[,] ControlPoints { get; set; }
        public List<Triangle> Triangles { get; set; }
        public Vertex[,] Vertices { get; set; }

        public static readonly int ControlPointsPerOneCurve = 4;

        public Mesh()
        {
            Triangles = new List<Triangle>();
            Vertices = new Vertex[ControlPointsPerOneCurve, ControlPointsPerOneCurve];
            ControlPoints = new Vector3[ControlPointsPerOneCurve, ControlPointsPerOneCurve];
        }
    }
}
