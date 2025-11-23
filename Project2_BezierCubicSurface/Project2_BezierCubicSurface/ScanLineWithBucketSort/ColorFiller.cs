using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Project2_BicubicBezierSurface.Models;
using Project2_BicubicBezierSurface.Helpers;
using Project2_BicubicBezierSurface.Algorithms;

namespace Project2_BicubicBezierSurface.ScanLineWithBucketSort
{
    public static class ColorFiller
    {
        public static void FillMesh(FastBitmap bitmap)
        {
            foreach (Triangle triangle in Mesh.Instance.Triangles)
            {
                Vertex v1 = Mesh.Instance.GetVertexByID(triangle.V1ID);
                Vertex v2 = Mesh.Instance.GetVertexByID(triangle.V2ID);
                Vertex v3 = Mesh.Instance.GetVertexByID(triangle.V3ID);

                FillTriangle(bitmap, v1, v2, v3);
            }
        }

        private static void FillTriangle(FastBitmap bitmap, Vertex v1, Vertex v2, Vertex v3)
        {
            // Rotated MainPanel-FastBitmap convertion
            float halfWidth = bitmap.Bitmap.Width / 2.0f;
            float halfHeight = bitmap.Bitmap.Height / 2.0f;

            Vector3 ProjectToScreen(Vector3 worldPos)
                => new Vector3(worldPos.X + halfWidth, halfHeight - worldPos.Y, worldPos.Z);

            Vector3 p1 = ProjectToScreen(v1.TransformedPosition);
            Vector3 p2 = ProjectToScreen(v2.TransformedPosition);
            Vector3 p3 = ProjectToScreen(v3.TransformedPosition);

            int minY = (int)Math.Min(p1.Y, Math.Min(p2.Y, p3.Y));
            int maxY = (int)Math.Max(p1.Y, Math.Max(p2.Y, p3.Y));

            // Ensure correctness (must never happen though)
            if (minY < 0) 
                minY = 0;
            if (maxY >= bitmap.Bitmap.Height) 
                maxY = bitmap.Bitmap.Height - 1;
            if (minY > maxY) 
                return;


            List<ScanLineEdge>[] edgeTable = new List<ScanLineEdge>[maxY - minY + 1];
            for (int i = 0; i < edgeTable.Length; i++) 
                edgeTable[i] = new List<ScanLineEdge>();

            // Fill edgeTable
            Vector3[] screenVertices = { p1, p2, p3 };
            for (int i = 0; i < 3; i++)
            {
                Vector3 start = screenVertices[i];
                Vector3 end = screenVertices[(i + 1) % 3];
                if ((int)start.Y == (int)end.Y) 
                    continue;
                if (start.Y > end.Y) 
                    (start, end) = (end, start);
                int yStart = (int)start.Y;
                int yEnd = (int)end.Y;
                float inverseSlope = (end.X - start.X) / (end.Y - start.Y);
                if (yStart >= minY && yStart < maxY)
                    edgeTable[yStart - minY].Add(new ScanLineEdge(yEnd, start.X, inverseSlope));
            }

            List<ScanLineEdge> activeEdges = new List<ScanLineEdge>();

            // main loop (runs from the highest 'y' coord to the lowest 'y' coord)
            for (int y = minY; y <= maxY; y++)
            {
                // update activeEdges
                int etIndex = y - minY;
                if (etIndex < edgeTable.Length) 
                    activeEdges.AddRange(edgeTable[etIndex]);
                activeEdges.RemoveAll(e => e.MaxY == y);
                // in common case it can be more than a triangle, so sort according to 'x' coord
                activeEdges.Sort((e1, e2) => e1.CurrentX.CompareTo(e2.CurrentX));

                // this loop will always run once, but for any polygon it can run more than once
                for (int i = 0; i < activeEdges.Count; i += 2) 
                {
                    if (i + 1 >= activeEdges.Count) 
                        break;
                    int xStart = (int)Math.Floor(activeEdges[i].CurrentX);
                    int xEnd = (int)Math.Ceiling(activeEdges[i + 1].CurrentX);
                    if (xStart < 0) 
                        xStart = 0;
                    if (xEnd >= bitmap.Bitmap.Width) 
                        xEnd = bitmap.Bitmap.Width - 1;

                    // this loop handles all pixels coloring that are situated between 2 edges on 'y' line
                    for (int x = xStart; x <= xEnd; x++)
                    {
                        var (alpha, beta, gamma) = LightingCalculator.CalculateBarycentricCoefficients(
                            p1, p2, p3, new Vector2(x, y));

                        // interpolates U and V image (texture) coordinates
                        float u = v1.U * alpha + v2.U * beta + v3.U * gamma;
                        float v = v1.V * alpha + v2.V * beta + v3.V * gamma;

                        Vector3 objectColor = Mesh.Instance.SurfaceColor;

                        if (Mesh.Instance.EnableImage && Mesh.Instance.CurrentTexture != null)
                        {
                            // set color from texture map using interpolated U and V 
                            objectColor = Mesh.Instance.CurrentTexture.GetColorAtUV(u, v);
                        }

                        Vector3 normalInPoint = LightingCalculator.InterpolateNormal(
                            v1.NormalVector_AR, v2.NormalVector_AR, v3.NormalVector_AR,
                            alpha, beta, gamma);

                        if (normalInPoint.LengthSquared() < 1e-6)
                        {
                            bitmap.SetPixel(x, y, Color.Black);
                            continue;
                        }

                        Vector3 interpolatedPoint = v1.TransformedPosition * alpha + 
                            v2.TransformedPosition * beta + v3.TransformedPosition * gamma;

                        Vector3 lightVersor = LightingCalculator.CalculateLightVersor(
                            Mesh.Instance.LightSourcePosition, interpolatedPoint);

                        Vector3 normalNormalized = Vector3.Normalize(normalInPoint);

                        Vector3 finalColor = LightingCalculator.CalculateColorVector(
                            Mesh.Instance.LightSourceColor,
                            objectColor,
                            Mesh.Instance.Kd,
                            normalNormalized,
                            lightVersor,
                            Mesh.Instance.Ks,
                            Mesh.Instance.M);

                        bitmap.SetPixel(x, y, LightingCalculator.GetRGBColor(finalColor));
                    }
                }
                foreach (var edge in activeEdges) 
                    edge.CurrentX += edge.InverseSlope;
            }
        }
    }
}
