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
        public List<Triangle> Triangles { get; private set; }
        public Vertex[,] Vertices { get; private set; }

        public int Resolution { get; private set; }

        public bool ShowMesh { get; set; } = false;
        public bool ShowControlPoints { get; set; } = true;
        public bool FillTriangles { get; set; } = false;

        private Mesh()
        {
            Triangles = new List<Triangle>();
            Vertices = new Vertex[4, 4];
            ControlPoints = new Vector3[4, 4];
            Resolution = 20;
        }

        public void SetControlPoints(Vector3[,] cp) => ControlPoints = cp;
        public void SetVertices(Vertex[,] v) => Vertices = v;
        public void SetTriangles(List<Triangle> t) => Triangles = t;
        public void SetResolution(int resolution)
        {
            if (resolution < 1)
                Resolution = 1;
            else Resolution = resolution;
        }

        public string CheckControlPoints() // Made for debug purposes
        {
            string ans = "";
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    ans += ControlPoints[i, j].ToString();
                    ans += "\n";
                }
            }

            return ans;
        }

        public Vertex GetVertexByID(int vertexID)
        {
            int width = Vertices.GetLength(1);
            return Vertices[vertexID / width, vertexID % width];
        }
    }
}
