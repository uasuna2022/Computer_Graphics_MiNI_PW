using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Project2_BicubicBezierSurface.Models
{
    public sealed class Vertex
    {
        public int VertexID { get; }
        public float U {  get; set; }
        public float V { get; set; }
        public Vector3 Position_BR { get; set; }
        public Vector3 Position_AR { get; set; }
        public Vector3 TangentVectorU_BR { get; set; }   
        public Vector3 TangentVectorU_AR { get; set; }
        public Vector3 TangentVectorV_BR { get; set; }
        public Vector3 TangentVectorV_AR { get; set; }
        public Vector3 NormalVector_BR { get; set; }
        public Vector3 NormalVector_AR { get; set; }
    }
}
