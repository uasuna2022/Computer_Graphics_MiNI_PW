using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Project2_BicubicBezierSurface.Models;

namespace Project2_BicubicBezierSurface
{
    public static class BezierSurfaceFileLoader
    {
        public static bool TryParseDataFromFile(string fileContent, out Mesh mesh)
        {
            mesh = new Mesh();
            string errorMessage = "";
            Vector3[,]? ControlPointsFromFile = ParseFile(fileContent, out errorMessage); 
            if (ControlPointsFromFile == null)
            {
                MessageBox.Show(errorMessage, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            mesh.ControlPoints = ControlPointsFromFile;
            return true;
        }

        private static Vector3[,]? ParseFile(string fileContent, out string errorMessage)
        {
            Vector3[,]? controlPoints = new Vector3[4, 4];
            errorMessage = "Success!";

            string[] lines = fileContent.Split(new[] { "\r\n", "\r", "\n" }, 
                StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length != 16)
            {
                controlPoints = null;
                errorMessage = $"Wrong amount of lines ({lines.Length} instead of 16)";
                return controlPoints;
            }

            int i = 0;
            foreach(string line in lines)
            {
                if (string.IsNullOrEmpty(line)) 
                    continue;

                string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 3)
                {
                    controlPoints = null;
                    errorMessage = $"Wrong amount of coords in line nr. {i}";
                    return controlPoints;
                }

                float x, y, z;
                if (!float.TryParse(parts[0], out x) || !float.TryParse(parts[1], out y) ||
                    !float.TryParse(parts[2], out z))
                {
                    controlPoints = null;
                    errorMessage = $"Invalid float parse in line nr. {i}";
                    return controlPoints;
                }

                controlPoints[i / 4, i % 4] = new Vector3(x, y, z);
                i++;
            }

            return controlPoints;
        }
    }
}
