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

            bool hasImage = (EditorWorkspace.Instance.WorkingImage != null) ? true : false;
            brightnessFilterRadioButton.Enabled = contrastFilterRadioButton.Enabled =
                customFunctionRadioButton.Enabled = gammaCorrectionFilterRadioButton.Enabled =
                noFilterRadioButton.Enabled = negationFilterRadioButton.Enabled = hasImage;
        }

        private void noFilterRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCheckProperties();
            UpdateEnableProperties();

            // TODO: now it's hardcoded to apply global all time
            ImageProcessor.ApplyGlobal(FilterStrategies.GetNoFilter());
            EditorWorkspace.Instance.CountPixels();
            workingPanel.Invalidate();
        }

        private void negationFilterRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCheckProperties();
            UpdateEnableProperties();

            // TODO: now it's hardcoded to apply global all time
            ImageProcessor.ApplyGlobal(FilterStrategies.GetNegationFilter());
            EditorWorkspace.Instance.CountPixels();
            workingPanel.Invalidate();
        }

        private void customFunctionRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCheckProperties();
            UpdateEnableProperties();
            // TODO: if checked, open a helper window to define a function

            // TODO: now it's hardcoded to apply global all time
            ImageProcessor.ApplyGlobal(FilterStrategies.GetCustomFunctionFilter()); // TODO: add arguments
            EditorWorkspace.Instance.CountPixels();
            workingPanel.Invalidate();
        }

        private void brightnessFilterRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCheckProperties();
            UpdateEnableProperties();

            // TODO: now it's hardcoded to apply global all time
            ImageProcessor.ApplyGlobal(FilterStrategies.GetBrightnessFilter(EditorWorkspace.Instance.Brightness));
            EditorWorkspace.Instance.CountPixels();
            workingPanel.Invalidate();
        }

        private void contrastFilterRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCheckProperties();
            UpdateEnableProperties();

            // TODO: now it's hardcoded to apply global all time
            ImageProcessor.ApplyGlobal(FilterStrategies.GetContrastFilter(EditorWorkspace.Instance.Contrast));
            EditorWorkspace.Instance.CountPixels();
            workingPanel.Invalidate();
        }

        private void gammaCorrectionFilterRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCheckProperties();
            UpdateEnableProperties();

            // TODO: now it's hardcoded to apply global all time
            ImageProcessor.ApplyGlobal(FilterStrategies.GetGammaFilter(EditorWorkspace.Instance.Gamma));
            EditorWorkspace.Instance.CountPixels();
            workingPanel.Invalidate();
        }

        private void brightnessFilterTrackBar_Scroll(object sender, EventArgs e)
        {
            int value = brightnessFilterTrackBar.Value;
            EditorWorkspace.Instance.Brightness = value;
            brightnessValueLabel.Text = value.ToString();

            // TODO: now it's hardcoded to apply global all time
            ImageProcessor.ApplyGlobal(FilterStrategies.GetBrightnessFilter(EditorWorkspace.Instance.Brightness));
            EditorWorkspace.Instance.CountPixels();
            workingPanel.Invalidate();
        }

        private void contrastFilterTrackBar_Scroll(object sender, EventArgs e)
        {
            int value = contrastFilterTrackBar.Value;
            EditorWorkspace.Instance.Contrast = value;
            contrastValueLabel.Text = value.ToString();

            // TODO: now it's hardcoded to apply global all time
            ImageProcessor.ApplyGlobal(FilterStrategies.GetContrastFilter(EditorWorkspace.Instance.Contrast));
            EditorWorkspace.Instance.CountPixels();
            workingPanel.Invalidate();
        }

        private void gammaCorrectionFilterTrackBar_Scroll(object sender, EventArgs e)
        {
            float value = gammaCorrectionFilterTrackBar.Value / 10.0F;
            EditorWorkspace.Instance.Gamma = value;
            gammaCorrectionValueLabel.Text = value.ToString("F1");

            // TODO: now it's hardcoded to apply global all time
            ImageProcessor.ApplyGlobal(FilterStrategies.GetGammaFilter(EditorWorkspace.Instance.Gamma));
            EditorWorkspace.Instance.CountPixels();
            workingPanel.Invalidate();
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

                    UpdateEnableProperties();
                    ApplyCurrentFilter();
                    saveImageToolStripMenuItem.Enabled = true;
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

        private void ApplyCurrentFilter()
        {
            if (EditorWorkspace.Instance.WorkingImage == null)
                return;

            PixelProcessor strategy = FilterStrategies.GetNoFilter();

            if (EditorWorkspace.Instance.NoFilter)
                strategy = FilterStrategies.GetNoFilter();
            else if (EditorWorkspace.Instance.NegationFilter)
                strategy = FilterStrategies.GetNegationFilter();
            else if (EditorWorkspace.Instance.BrightnessFilter)
                strategy = FilterStrategies.GetBrightnessFilter(EditorWorkspace.Instance.Brightness);
            else if (EditorWorkspace.Instance.GammaFilter)
                strategy = FilterStrategies.GetGammaFilter(EditorWorkspace.Instance.Gamma);
            else if (EditorWorkspace.Instance.ContrastFilter)
                strategy = FilterStrategies.GetContrastFilter(EditorWorkspace.Instance.Contrast);
            else if (EditorWorkspace.Instance.CustomFunctionFilter)
                strategy = FilterStrategies.GetCustomFunctionFilter(); // TODO: for sure add appropriate arguments

            // if (radiobutton "Full image" chosen)
            ImageProcessor.ApplyGlobal(strategy);

            EditorWorkspace.Instance.CountPixels();
            workingPanel.Invalidate();
        }
        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (EditorWorkspace.Instance.WorkingImage == null)
                return;
            

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Title = "Save Image";
                saveFileDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg;*.jpeg|Bitmap Image|*.bmp";
                saveFileDialog.DefaultExt = "png";
                saveFileDialog.AddExtension = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Bitmap imageToSave = EditorWorkspace.Instance.WorkingImage;

                    System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Png;

                    string ext = System.IO.Path.GetExtension(saveFileDialog.FileName).ToLower();
                    switch (ext)
                    {
                        case ".jpg":
                        case ".jpeg":
                            format = System.Drawing.Imaging.ImageFormat.Jpeg;
                            break;
                        case ".bmp":
                            format = System.Drawing.Imaging.ImageFormat.Bmp;
                            break;
                    }

                    try
                    {
                        imageToSave.Save(saveFileDialog.FileName, format);
                        MessageBox.Show("Image saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to save image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
