using Project2_BicubicBezierSurface;
using Project2_BicubicBezierSurface.Models;
using System.IO;
using System.Windows.Forms;

namespace Project2_BezierCubicSurface
{
    public partial class MainWindow : Form
    {
        public Mesh Mesh { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            Mesh = new Mesh();
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
                        Mesh mesh = new Mesh();

                        if (!BezierSurfaceFileLoader.TryParseDataFromFile(fileContent, out mesh))
                            return;

                        Mesh = mesh;

                        MessageBox.Show($"File '{Path.GetFileName(filePath)}' has been successfully read. " +
                            $"Control points loaded.", "Success!", MessageBoxButtons.OK, 
                            MessageBoxIcon.Information);

                        // TODO: invalidate everything and redraw main mesh etc.
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
    }
}
