namespace Project3_CustomFunctionImageFilter
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void UpdateCheckProperties()
        {
            EditorWorkspace.Instance.CustomFunctionFilter = customFunctionRadioButton.Checked;
            EditorWorkspace.Instance.ContrastFilter = contrastFilterRadioButton.Checked;
            EditorWorkspace.Instance.BrightnessFilter = brightnessFilterRadioButton.Checked;
            EditorWorkspace.Instance.GammaFilter = gammaCorrectionFilterRadioButton.Checked;
            EditorWorkspace.Instance.NoFilter = noFilterRadioButton.Checked;
            EditorWorkspace.Instance.NegationFilter = negationFilterRadioButton.Checked;
        }
        private void UpdateEnableProperties()
        {
            brightnessFilterTrackBar.Enabled = brightnessFilterRadioButton.Checked;
            contrastFilterTrackBar.Enabled = contrastFilterRadioButton.Checked;
            gammaCorrectionFilterTrackBar.Enabled = gammaCorrectionFilterRadioButton.Checked;
        }

        private void noFilterRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCheckProperties();
            UpdateEnableProperties();
            // TODO: apply changes
        }

        private void negationFilterRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCheckProperties();
            UpdateEnableProperties();
            // TODO: apply changes
        }

        private void customFunctionRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCheckProperties();
            UpdateEnableProperties();
            // TODO: if checked, open a helper window to define a function
            // TODO: apply changes
        }

        private void brightnessFilterRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCheckProperties();
            UpdateEnableProperties();
            // TODO: apply changes
        }

        private void contrastFilterRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCheckProperties();
            UpdateEnableProperties();
            // TODO: apply changes
        }

        private void gammaCorrectionFilterRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCheckProperties();
            UpdateEnableProperties();
            // TODO: apply changes
        }

        private void brightnessFilterTrackBar_Scroll(object sender, EventArgs e)
        {
            int value = brightnessFilterTrackBar.Value;
            EditorWorkspace.Instance.Brightness = value;
            brightnessValueLabel.Text = value.ToString();
            // TODO: apply changes
        }

        private void contrastFilterTrackBar_Scroll(object sender, EventArgs e)
        {
            int value = contrastFilterTrackBar.Value;
            EditorWorkspace.Instance.Contrast = value;
            contrastValueLabel.Text = value.ToString();
            // TODO: apply changes
        }

        private void gammaCorrectionFilterTrackBar_Scroll(object sender, EventArgs e)
        {
            float value = (float)gammaCorrectionFilterTrackBar.Value / 10.0F;
            EditorWorkspace.Instance.Gamma = value;
            gammaCorrectionValueLabel.Text = value.ToString("F1");
            // TODO: apply changes
        }

        private void openImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Choose an image";
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (Bitmap tempBitmap = new Bitmap(openFileDialog.FileName))
                    {
                        EditorWorkspace.Instance.SetImage(tempBitmap);
                    }

                    workingPanel.Invalidate();
                }
            }
        }

        private void workingPanel_Paint(object sender, PaintEventArgs e)
        {
            if (EditorWorkspace.Instance.WorkingImage == null)
                return;

            int panelWidth = workingPanel.Width;
            int panelHeight = workingPanel.Height;  
            int imageWidth = EditorWorkspace.Instance.WorkingImage.Width;
            int imageHeight = EditorWorkspace.Instance.WorkingImage.Height;

            // Scaling
            float scaleX = (float)panelWidth / imageWidth;
            float scaleY = (float)panelHeight / imageHeight;
            float scale = Math.Min(scaleX, scaleY);

            int newWidth = (int)(imageWidth * scale);
            int newHeight = (int)(imageHeight * scale);

            // Centering
            int posX = (panelWidth - newWidth) / 2;
            int posY = (panelHeight - newHeight) / 2;

            e.Graphics.DrawImage(EditorWorkspace.Instance.WorkingImage, posX, posY, newWidth, newHeight);
        }
    }
}
