using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Security.Policy;

namespace Project2_BicubicBezierSurface.Models
{
    public sealed class Mesh
    {
        private static Mesh? _instance;
        public static Mesh Instance => _instance ??= new Mesh();

        public Vector3[,] ControlPoints { get; private set; }
        public Vector3[,] RotatedControlPoints { get; set; }
        public List<Triangle> Triangles { get; private set; }
        public Vertex[,] Vertices { get; private set; }

        public int Resolution { get; private set; } = 30;
        public float AngleX { get; private set; } = 0;
        public float AngleZ { get; private set; } = 0;

        public bool ShowMesh { get; set; } = false;
        public bool ShowControlPoints { get; set; } = true;
        public bool FillTriangles { get; set; } = false;

        public int M { get; private set; } = 50;
        public float Kd { get; private set; } = 0.5F;
        public float Ks { get; private set; } = 0.5F;

        public Vector3 SurfaceColor { get; private set; } = new Vector3(192.0F / 255.0F, 1.0F, 192.0F / 255.0F);
        public Vector3 LightSourceColor { get; private set; } = new Vector3(1.0F, 1.0F, 192.0F / 255.0F);

        public bool EnableAnimation { get; set; } = false;
        public bool EnableSurfaceColor { get; set; } = false;
        public bool EnableImage { get; set; } = false;
        public bool EnableNormalMap { get; set; } = false;

        public int LightSourceZCoord { get; private set; } = 100;

        private Mesh()
        {
            Triangles = new List<Triangle>();
            Vertices = new Vertex[4, 4];
            ControlPoints = new Vector3[4, 4];
            RotatedControlPoints = new Vector3[4, 4];
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
        public void SetAngleX(float angleX) => AngleX = angleX;
        public void SetAngleZ(float angleZ) => AngleZ = angleZ;
        public void SetM(int m) => M = m;
        public void SetKd(float kd) => Kd = kd;
        public void SetKs(float ks) => Ks = ks;
        public void SetSurfaceColor(Vector3 color) => SurfaceColor = color;
        public void SetLightSourceColor(Vector3 color) => LightSourceColor = color;
        public void SetLightSourceZCoord(int z) => LightSourceZCoord = z;

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
