using Project2_BicubicBezierSurface;
using Project2_BicubicBezierSurface.Algorithms;
using Project2_BicubicBezierSurface.Models;
using System.IO;
using System.Numerics;
using System.Windows.Forms;
using System.Windows;

namespace Project2_BezierCubicSurface
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();

            // Magic code to enable Double Buffering on a Panel
            typeof(Panel).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                null, WorkspacePanel, new object[] { true });

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

                        /*
                        MessageBox.Show($"File '{Path.GetFileName(filePath)}' has been successfully read. " +
                            $"Control points loaded.", "Success!", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        MessageBox.Show($"{Mesh.Instance.CheckControlPoints()}");
                        */

                        EnableAll();
                        GenerateMesh.GetMesh();
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

            //GenerateMesh.GetMesh();
            if (Mesh.Instance.ShowControlPoints)
                DrawControlPoints(g);
            if (Mesh.Instance.ShowMesh)
            {
                DrawVertices(g);
                DrawTriangles(g);
            }
        }

        private void DrawVertices(Graphics g)
        {
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
        }

        private void DrawTriangles(Graphics g)
        {
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

        private void DrawControlPoints(Graphics g)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Vector3 cp = Mesh.Instance.ControlPoints[i, j];
                    float x = cp.X;
                    float y = cp.Y;
                    int vertexRadius = 4;

                    g.FillEllipse(Brushes.DarkOrange, x - vertexRadius, y - vertexRadius, vertexRadius * 2,
                        vertexRadius * 2);
                }
            }

            Pen pen = new Pen(Brushes.Orange);
            for (int i = 0; i <= 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Vector3 cp1 = Mesh.Instance.ControlPoints[i, j];
                    Vector3 cp2 = Mesh.Instance.ControlPoints[i, j + 1];
                    g.DrawLine(pen, cp1.X, cp1.Y, cp2.X, cp2.Y);
                }
            }

            for (int i = 0; i <= 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Vector3 cp1 = Mesh.Instance.ControlPoints[j, i];
                    Vector3 cp2 = Mesh.Instance.ControlPoints[j + 1, i];
                    g.DrawLine(pen, cp1.X, cp1.Y, cp2.X, cp2.Y);
                }
            }
        }

        private void EnableAll()
        {
            ShowMeshCheckBox.Enabled = true;
            ShowControlPointsCheckBox.Enabled = true;
            FillTrianglesCheckBox.Enabled = true;
            ResolutionTrackBar.Enabled = true;
            XAxisRotationTrackBar.Enabled = true;
            ZAxisRotationTrackBar.Enabled = true;
        }

        private void ShowMeshCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Mesh.Instance.ShowMesh = ShowMeshCheckBox.Checked;
            WorkspacePanel.Invalidate();
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            int newResolution = ResolutionTrackBar.Value;
            ResolutionValueLabel.Text = newResolution.ToString();
            Mesh.Instance.SetResolution(newResolution);
            GenerateMesh.GetMesh();
            WorkspacePanel.Invalidate();
        }

        private void ShowControlPointsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Mesh.Instance.ShowControlPoints = ShowControlPointsCheckBox.Checked;
            WorkspacePanel.Invalidate();
        }

        private void FillTrianglesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // TODO in the future
        }

        private void ZAxisRotationTrackBar_ValueChanged(object sender, EventArgs e)
        {
            int newZAngle = ZAxisRotationTrackBar.Value;
            ZRotationLabelValue.Text = newZAngle.ToString();
            Mesh.Instance.SetAngleZ(newZAngle);
            // TODO: Update vertices and triangles
            WorkspacePanel.Invalidate();
        }

        private void XAxisRotationTrackBar_ValueChanged(object sender, EventArgs e)
        {
            int newXAngle = XAxisRotationTrackBar.Value;
            XRotationLabelValue.Text = newXAngle.ToString();
            Mesh.Instance.SetAngleX(newXAngle);
            // TODO: Update vertices and triangles
            WorkspacePanel.Invalidate();
        }
    }
}
