using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Project2_BicubicBezierSurface
{
    public static class RotationMatrix
    {
        private static float DegToRad(float angleInDeg) => angleInDeg * (float)(Math.PI / 180.0);

        public static Vector3 XRotation(float thetaInDeg, Vector3 oldPoint)
        {
            float theta = DegToRad(thetaInDeg);

            float newX = oldPoint.X;
            float newY = (float)Math.Cos(theta) * oldPoint.Y - (float)Math.Sin(theta) * oldPoint.Z;
            float newZ = (float)Math.Sin(theta) * oldPoint.Y + (float)Math.Cos(theta) * oldPoint.Z;

            return new Vector3(newX, newY, newZ);
        }
        public static Vector3 ZRotation(float thetaInDeg, Vector3 oldPoint)
        {
            float theta = DegToRad(thetaInDeg);

            float newX = (float)Math.Cos(theta) * oldPoint.X - (float)Math.Sin(theta) * oldPoint.Y;
            float newY = (float)Math.Sin(theta) * oldPoint.X + (float)Math.Cos(theta) * oldPoint.Y;
            float newZ = oldPoint.Z;

            return new Vector3(newX, newY, newZ);
        }
    }
}
