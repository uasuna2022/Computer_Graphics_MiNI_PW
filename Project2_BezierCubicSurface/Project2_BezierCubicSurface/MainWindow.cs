using Project2_BicubicBezierSurface;
using Project2_BicubicBezierSurface.Algorithms;
using Project2_BicubicBezierSurface.Models;
using System.IO;
using System.Numerics;
using System.Windows.Forms;

namespace Project2_BezierCubicSurface
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void loadControlPointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                openFileDialog.Title = "Select a .txt file with control points";
                openFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Resources");

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    string fileContent = "";

                    try
                    {
                        fileContent = File.ReadAllText(filePath);

                        if (!BezierSurfaceFileLoader.TryParseDataFromFile(fileContent))
                            return;

                        MessageBox.Show($"File '{Path.GetFileName(filePath)}' has been successfully read. " +
                            $"Control points loaded.", "Success!", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        MessageBox.Show($"{Mesh.Instance.CheckControlPoints()}");

                        // TODO: invalidate everything and redraw main mesh etc.
                        GenerateMesh.GetMesh(20);
                        WorkspacePanel.Invalidate();
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show($"Message while reading a file: {ex.Message}", "I/O error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Unexpected error caught: {ex.Message}", "unexpected error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void WorkspacePanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.ScaleTransform(1, -1);
            g.TranslateTransform(WorkspacePanel.Width / 2, -WorkspacePanel.Height / 2);

            if (Mesh.Instance.Vertices == null || Mesh.Instance.Triangles == null ||
                Mesh.Instance.Triangles.Count == 0)
                return;

            int N = Mesh.Instance.Vertices.GetLength(0);
            int M = Mesh.Instance.Vertices.GetLength(1);
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    float x = Mesh.Instance.Vertices[i, j].Position.X;
                    float y = Mesh.Instance.Vertices[i, j].Position.Y;
                    int vertexRadius = 2;

                    g.FillEllipse(Brushes.AliceBlue, x - vertexRadius, y - vertexRadius, vertexRadius * 2,
                        vertexRadius * 2);
                }
            }

            Pen pen = new Pen(Brushes.Black);
            foreach (Triangle triangle in Mesh.Instance.Triangles)
            {
                Vector3 p1 = Mesh.Instance.GetVertexByID(triangle.V1ID).Position;
                Vector3 p2 = Mesh.Instance.GetVertexByID(triangle.V2ID).Position;
                Vector3 p3 = Mesh.Instance.GetVertexByID(triangle.V3ID).Position;

                g.DrawLine(pen, p1.X, p1.Y, p2.X, p2.Y);
                g.DrawLine(pen, p2.X, p2.Y, p3.X, p3.Y);
                g.DrawLine(pen, p3.X, p3.Y, p1.X, p1.Y);
            }
        }
    }
}
