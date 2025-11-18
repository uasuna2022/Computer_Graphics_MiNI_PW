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
        private static Mesh? _instance;
        public static Mesh Instance => _instance ??= new Mesh();

        public Vector3[,] ControlPoints { get; private set; }
        public List<Triangle> Triangles { get; set; }
        public Vertex[,] Vertices { get; set; }

        private Mesh()
        {
            Triangles = new List<Triangle>();
            Vertices = new Vertex[4, 4];
            ControlPoints = new Vector3[4, 4];
        }

        public void SetControlPoints(Vector3[,] cp) => ControlPoints = cp;
    }
}
