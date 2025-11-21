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
        public static void GetMesh()
        {
            if (Mesh.Instance.ControlPoints == null)
                return;

            int resolution = Mesh.Instance.Resolution;

            int pointCount = resolution + 1;
            Vertex[,] newVertices = new Vertex[pointCount, pointCount];
            List<Triangle> triangles = new List<Triangle>();

            int vertexID = 0;
            for (int i = 0; i < pointCount; i++)
            {
                for (int j = 0; j < pointCount; j++)
                {
                    float u = (float)i / resolution;
                    float v = (float)j / resolution;

                    Vector3 pos = CalculateBezierPoint(u, v);
                    newVertices[i, j] = new Vertex(vertexID, pos, u, v);
                    vertexID++;
                }
            }

            Mesh.Instance.SetVertices(newVertices);

            for (int i = 0; i < pointCount - 1; i++)
            {
                for (int j = 0; j < pointCount - 1; j++)
                {
                    Vertex A = Mesh.Instance.Vertices[i, j];
                    Vertex B = Mesh.Instance.Vertices[i, j + 1];
                    Vertex C = Mesh.Instance.Vertices[i + 1, j + 1];
                    Vertex D = Mesh.Instance.Vertices[i + 1, j];

                    triangles.Add(new Triangle(A.VertexID, B.VertexID, D.VertexID));
                    triangles.Add(new Triangle(B.VertexID, C.VertexID, D.VertexID));
                }
            }

            Mesh.Instance.SetTriangles(triangles);
        }

        public static Vector3 CalculateBezierPoint(float u, float v)
        {
            Vector3 bp = Vector3.Zero;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    bp += Mesh.Instance.ControlPoints[i, j] * BernsteinCoefficients.Coefficient(u, i, 3) *
                        BernsteinCoefficients.Coefficient(v, j, 3);
                }
            }

            return bp;
        }

        public static void RotateMesh()
        {
            float angleX = Mesh.Instance.AngleX;
            float angleZ = Mesh.Instance.AngleZ;

            int N = Mesh.Instance.Vertices.GetLength(0);
            int M = Mesh.Instance.Vertices.GetLength(1);    

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    Vector3 originalPosition = Mesh.Instance.Vertices[i, j].OriginalPosition;
                    Vector3 transformedPosition = RotationMatrix.ZRotation(angleZ, originalPosition);
                    transformedPosition = RotationMatrix.XRotation(angleX, transformedPosition);

                    Mesh.Instance.Vertices[i, j].TransformedPosition = transformedPosition;
                }
            }
        }
    }
}
