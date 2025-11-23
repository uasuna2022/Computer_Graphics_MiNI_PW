using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Project2_BicubicBezierSurface.Models;
using System.Reflection.Metadata.Ecma335;
using System.Diagnostics;

namespace Project2_BicubicBezierSurface.Algorithms
{
    public static class LightingCalculator
    {
        public static (float alpha, float beta, float gamma) 
            CalculateBarycentricCoefficients(Vector3 v1, Vector3 v2, Vector3 v3, Vector2 point)
        {
            Vector2 A = new Vector2(v1.X, v1.Y);
            Vector2 B = new Vector2(v2.X, v2.Y);
            Vector2 C = new Vector2(v3.X, v3.Y);

            Vector2 AB = B - A;
            Vector2 AC = C - A;
            Vector2 AP = point - A;

            float ABdotAB = Vector2.Dot(AB, AB);
            float ABdotAC = Vector2.Dot(AB, AC);
            float ACdotAC = Vector2.Dot(AC, AC);
            float APdotAB = Vector2.Dot(AP, AB);
            float APdotAC = Vector2.Dot(AP, AC);

            float denominator = ABdotAB * ACdotAC - ABdotAC * ABdotAC;
            if (denominator < 1e-6)
                return (-1.0F, -1.0F, -1.0F);

            float beta = (ACdotAC * APdotAB - ABdotAC * APdotAC) / denominator;
            float gamma = (ABdotAB * APdotAC - ABdotAC * APdotAB) / denominator;
            float alpha = 1.0F - beta - gamma;

            return (alpha, beta, gamma);
        }

        public static Vector3 InterpolateNormal(Vector3 NA, Vector3 NB, Vector3 NC,
            float alpha, float beta, float gamma) => alpha * NA + beta * NB + gamma * NC;
        public static Vector3 CalculateLightVersor(Vector3 lightPosition, Vector3 point) =>
            Vector3.Normalize(lightPosition - point);

        public static Vector3 CalculateColorVector(Vector3 lightColor, Vector3 objectColor, 
            float kd, Vector3 normalVector, Vector3 lightVersor, float ks, float m)
        {
            normalVector = Vector3.Normalize(normalVector);

            Vector3 R = 2 * Vector3.Dot(normalVector, lightVersor) * normalVector - lightVersor;
            R = Vector3.Normalize(R);            

            float cosNL = Math.Max(0, Vector3.Dot(normalVector, lightVersor));
            float cosVR = Math.Max(0, Vector3.Dot(Vector3.UnitZ, R));
            if (cosVR > 0)
                cosVR = (float)Math.Pow(cosVR, m);

            Vector3 color = kd * lightColor * objectColor * cosNL +
                ks * lightColor * objectColor * cosVR;
            color = Vector3.Clamp(color, Vector3.Zero, Vector3.One);

            return color;
        }
        public static Color GetRGBColor(Vector3 vectorColor)
        {
            int r = (int)(vectorColor.X * 255.0f);
            int g = (int)(vectorColor.Y * 255.0f);
            int b = (int)(vectorColor.Z * 255.0f);

            int finalR = Math.Min(255, r);
            int finalG = Math.Min(255, g);
            int finalB = Math.Min(255, b);

            return Color.FromArgb(finalR, finalG, finalB);
        }
            
    }
}
