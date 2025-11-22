using Project2_BicubicBezierSurface.Algorithms;
using Project2_BicubicBezierSurface.Models;
using System.IO;
using System.Numerics;
using System.Windows.Forms;
using System.Windows;
using Project2_BicubicBezierSurface.Helpers;

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

                        EnableAll();
                        MeshProcessor.CreateMesh();
                        MeshProcessor.RotateMesh(); // here rotate function in fact does nothing, however it fullfills RotatedCP array
                        ZAxisRotationTrackBar.Value = 0;
                        XAxisRotationTrackBar.Value = 0;
                        ResolutionTrackBar.Value = 30;
                        ShininessExponentTrackBar.Value = 50;
                        DiffuseCoefficientTrackBar.Value = 50;
                        SpecularCoefficientTrackBar.Value = 50;
                        FillTrianglesCheckBox.Checked = false;
                        ShowMeshCheckBox.Checked = false;
                        ImageCheckBox.Checked = false;
                        NormalMapCheckBox.Checked = false;
                        ColorCheckBox.Checked = false;
                        AnimationCheckBox.Checked = false;
                        LightSourceDistanceTrackbar.Value = 100;
                        WorkspacePanel.Invalidate();
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show($"Message while reading a file: {ex.Message}", "I/O error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Unexpected error caught: {ex.Message}", "Unexpected error",
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
                    float x = Mesh.Instance.Vertices[i, j].TransformedPosition.X;
                    float y = Mesh.Instance.Vertices[i, j].TransformedPosition.Y;
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
                Vector3 p1 = Mesh.Instance.GetVertexByID(triangle.V1ID).TransformedPosition;
                Vector3 p2 = Mesh.Instance.GetVertexByID(triangle.V2ID).TransformedPosition;
                Vector3 p3 = Mesh.Instance.GetVertexByID(triangle.V3ID).TransformedPosition;

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
                    Vector3 cp = Mesh.Instance.RotatedControlPoints[i, j];
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
                    Vector3 cp1 = Mesh.Instance.RotatedControlPoints[i, j];
                    Vector3 cp2 = Mesh.Instance.RotatedControlPoints[i, j + 1];
                    g.DrawLine(pen, cp1.X, cp1.Y, cp2.X, cp2.Y);
                }
            }

            for (int i = 0; i <= 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Vector3 cp1 = Mesh.Instance.RotatedControlPoints[j, i];
                    Vector3 cp2 = Mesh.Instance.RotatedControlPoints[j + 1, i];
                    g.DrawLine(pen, cp1.X, cp1.Y, cp2.X, cp2.Y);
                }
            }
        }

        private void EnableAll()
        {
            ShowMeshCheckBox.Enabled = true;
            ShowControlPointsCheckBox.Enabled = true;
            FillTrianglesCheckBox.Enabled = true;
            XAxisRotationTrackBar.Enabled = true;
            ZAxisRotationTrackBar.Enabled = true;
            SurfaceColorPanel.Enabled = true;
            ImagePanel.Enabled = true;
            NormalMapPanel.Enabled = true;
            ChooseLightSourceColorPanel.BackColor = Color.FromArgb(255, 255, 192);
            SurfaceColorPanel.BackColor = Color.FromArgb(192, 255, 192);
            ImagePanel.BackColor = Color.Transparent;
            NormalMapPanel.BackColor = Color.Transparent;
            ChooseLightSourceColorPanel.Enabled = true;
        }

        private void ShowMeshCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Mesh.Instance.ShowMesh = ShowMeshCheckBox.Checked;
            ResolutionTrackBar.Enabled = ShowMeshCheckBox.Checked;
            WorkspacePanel.Invalidate();
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            int newResolution = ResolutionTrackBar.Value;
            ResolutionValueLabel.Text = newResolution.ToString();
            Mesh.Instance.SetResolution(newResolution);
            MeshProcessor.CreateMesh();
            MeshProcessor.RotateMesh();
            WorkspacePanel.Invalidate();
        }

        private void ShowControlPointsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Mesh.Instance.ShowControlPoints = ShowControlPointsCheckBox.Checked;
            XAxisRotationTrackBar.Enabled = ShowControlPointsCheckBox.Checked;
            ZAxisRotationTrackBar.Enabled = ShowControlPointsCheckBox.Checked;
            WorkspacePanel.Invalidate();
        }

        private void FillTrianglesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            DiffuseCoefficientTrackBar.Enabled = FillTrianglesCheckBox.Checked;
            SpecularCoefficientTrackBar.Enabled = FillTrianglesCheckBox.Checked;
            ShininessExponentTrackBar.Enabled = FillTrianglesCheckBox.Checked;
            ColorCheckBox.Enabled = FillTrianglesCheckBox.Checked;
            ImageCheckBox.Enabled = FillTrianglesCheckBox.Checked; // TODO: && image loaded
            NormalMapCheckBox.Enabled = FillTrianglesCheckBox.Checked; // TODO: && normal map loaded
            LightSourceDistanceTrackbar.Enabled = FillTrianglesCheckBox.Checked;
            AnimationCheckBox.Enabled = FillTrianglesCheckBox.Checked;
            Mesh.Instance.FillTriangles = FillTrianglesCheckBox.Checked;

            // TODO: fill the triangles with the given color
        }

        private void ZAxisRotationTrackBar_ValueChanged(object sender, EventArgs e)
        {
            int newZAngle = ZAxisRotationTrackBar.Value;
            ZRotationLabelValue.Text = newZAngle.ToString();
            Mesh.Instance.SetAngleZ(newZAngle);
            MeshProcessor.RotateMesh();
            WorkspacePanel.Invalidate();
        }

        private void XAxisRotationTrackBar_ValueChanged(object sender, EventArgs e)
        {
            int newXAngle = XAxisRotationTrackBar.Value;
            XRotationLabelValue.Text = newXAngle.ToString();
            Mesh.Instance.SetAngleX(newXAngle);
            MeshProcessor.RotateMesh();
            WorkspacePanel.Invalidate();
        }

        private void ShininessExponentTrackBar_ValueChanged(object sender, EventArgs e)
        {
            int newM = ShininessExponentTrackBar.Value;
            ShininessExponentValueLabel.Text = newM.ToString();
            Mesh.Instance.SetM(newM);
            // TODO: refill the triangles with the new appropriate colors
        }

        private void DiffuseCoefficientTrackBar_ValueChanged(object sender, EventArgs e)
        {
            float newKd = DiffuseCoefficientTrackBar.Value / 100F;
            float newKs = SpecularCoefficientTrackBar.Value / 100F;
            if (newKd + newKs > 1.0F)
                newKs = 1.0F - newKd;

            DiffuseCoefficientValueLabel.Text = newKd.ToString();
            Mesh.Instance.SetKd(newKd);
            SpecularCoefficientValueLabel.Text = newKs.ToString();
            if (newKs != SpecularCoefficientTrackBar.Value / 100F)
                SpecularCoefficientTrackBar.Value = (int)(newKs * 100);
            Mesh.Instance.SetKs(newKs);

            // TODO: refill the triangles with the new appropriate colors
        }

        private void SpecularCoefficientTrackBar_ValueChanged(object sender, EventArgs e)
        {
            float newKd = DiffuseCoefficientTrackBar.Value / 100F;
            float newKs = SpecularCoefficientTrackBar.Value / 100F;
            if (newKd + newKs > 1.0F)
                newKd = 1.0F - newKs;

            SpecularCoefficientValueLabel.Text = newKs.ToString();
            Mesh.Instance.SetKs(newKs);
            DiffuseCoefficientValueLabel.Text = newKd.ToString();
            if (newKd != DiffuseCoefficientTrackBar.Value / 100F)
                DiffuseCoefficientTrackBar.Value = (int)(newKd * 100);
            Mesh.Instance.SetKd(newKd);
            // TODO: refill the triangles with the new appropriate colors
        }

        private void SurfaceColorPanel_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.Color = SurfaceColorPanel.BackColor;
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    SurfaceColorPanel.BackColor = colorDialog.Color;

                    Vector3 newSurfaceColor = new Vector3(colorDialog.Color.R / 255.0F,
                        colorDialog.Color.G / 255.0F, colorDialog.Color.B / 255.0F);

                    Mesh.Instance.SetSurfaceColor(newSurfaceColor);

                    // TODO: Invalidate and apply the color 
                }
            }
        }

        private void ImagePanel_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Load an image logic");
            // TODO: load an image
        }

        private void NormalMapPanel_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Load a NormalMap logic");
            // TODO: load NormalMap 
        }

        private void ChooseLightSourceColorPanel_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.Color = ChooseLightSourceColorPanel.BackColor;
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    ChooseLightSourceColorPanel.BackColor = colorDialog.Color;

                    Vector3 newLightSourceColor = new Vector3(colorDialog.Color.R / 255.0F,
                        colorDialog.Color.G / 255.0F, colorDialog.Color.B / 255.0F);

                    Mesh.Instance.SetLightSourceColor(newLightSourceColor);

                    // TODO: Invalidate and apply the color 
                }
            }
        }

        private void LightSourceDistanceTrackbar_ValueChanged(object sender, EventArgs e)
        {
            int newZCoord = LightSourceDistanceTrackbar.Value;
            LightSourceDistanceLabelValue.Text = newZCoord.ToString();
            Mesh.Instance.SetLightSourceZCoord(newZCoord);

            // TODO: Invalidate and apply new z coord
        }

        private void ColorCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // TODO
        }

        private void ImageCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // TODO
        }

        private void NormalMapCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // TODO
        }

        private void AnimationCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // TODO
        }
    }
}
