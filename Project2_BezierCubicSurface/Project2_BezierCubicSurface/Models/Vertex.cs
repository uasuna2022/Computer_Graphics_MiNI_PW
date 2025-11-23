using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Project2_BicubicBezierSurface.Algorithms;

namespace Project2_BicubicBezierSurface.Models
{
    public sealed class Vertex
    {
        public int VertexID { get; }
        public Vector3 OriginalPosition { get; set; }
        public float U {  get; set; }
        public float V { get; set; }
        public Vector3 TransformedPosition { get; set; }
        public Vector3 TangentVectorU_BR { get; set; }   
        public Vector3 TangentVectorU_AR { get; set; }
        public Vector3 TangentVectorV_BR { get; set; }
        public Vector3 TangentVectorV_AR { get; set; }
        public Vector3 NormalVector_BR { get; set; }
        public Vector3 NormalVector_AR { get; set; }

        public Vertex(int vertexID, Vector3 position, float u, float v)
        {
            VertexID = vertexID;
            OriginalPosition = position;
            TransformedPosition = position;
            U = u;
            V = v;

            TangentVectorU_BR = MeshProcessor.CalculateTangentU(u, v);
            TangentVectorV_BR = MeshProcessor.CalculateTangentV(u, v);
            NormalVector_BR = Vector3.Normalize(Vector3.Cross(TangentVectorU_BR, TangentVectorV_BR));

            TangentVectorU_AR = TangentVectorU_BR;
            TangentVectorV_AR = TangentVectorV_BR;
            NormalVector_AR = NormalVector_BR;
        }
    }
}
