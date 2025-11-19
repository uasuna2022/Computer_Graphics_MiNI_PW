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

        private Mesh()
        {
            Triangles = new List<Triangle>();
            Vertices = new Vertex[4, 4];
            ControlPoints = new Vector3[4, 4];
        }

        public void SetControlPoints(Vector3[,] cp) => ControlPoints = cp;
        public void SetVertices(Vertex[,] v) => Vertices = v;
        public void SetTriangles(List<Triangle> t) => Triangles = t;

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
