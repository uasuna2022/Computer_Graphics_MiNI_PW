using System.Windows.Forms.DataVisualization.Charting;

namespace Project3_CustomFunctionImageFilter
{
    public partial class MainWindow : Form
    {
        private bool _isPainting = false;
        private Point _lastMousePosition = Point.Empty;
        public MainWindow()
        {
            InitializeComponent();
            InitializeCharts();
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
                noFilterRadioButton.Enabled = negationFilterRadioButton.Enabled =
                wholeImageRadioButton.Enabled = brushRadioButton.Enabled = hasImage;
        }

        private void noFilterRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCheckProperties();
            UpdateEnableProperties();

            if (EditorWorkspace.Instance.OriginalImage != null)
            {
                EditorWorkspace.Instance.WorkingImage?.Dispose();
                EditorWorkspace.Instance.WorkingImage = (Bitmap)EditorWorkspace.Instance.OriginalImage.Clone();

                EditorWorkspace.Instance.RestoreHistogramFromOriginal();

                UpdateHistograms();
                workingPanel.Invalidate();
            }
        }

        private void negationFilterRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCheckProperties();
            UpdateEnableProperties();

            if (EditorWorkspace.Instance.WholeImage)
            {
                ImageProcessor.ApplyGlobal(FilterStrategies.GetNegationFilter());
                EditorWorkspace.Instance.CountPixels();
                UpdateHistograms();
                workingPanel.Invalidate();
            }
        }

        private void customFunctionRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCheckProperties();
            UpdateEnableProperties();
            // TODO: if checked, open a helper window to define a function

            if (EditorWorkspace.Instance.WholeImage)
            {
                ImageProcessor.ApplyGlobal(FilterStrategies.GetCustomFunctionFilter()); // TODO: add arguments
                EditorWorkspace.Instance.CountPixels();
                UpdateHistograms();
                workingPanel.Invalidate();
            }
        }

        private void brightnessFilterRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCheckProperties();
            UpdateEnableProperties();

            if (EditorWorkspace.Instance.WholeImage)
            {
                ImageProcessor.ApplyGlobal(FilterStrategies.GetBrightnessFilter(EditorWorkspace.Instance.Brightness));
                EditorWorkspace.Instance.CountPixels();
                UpdateHistograms();
                workingPanel.Invalidate();
            }
        }

        private void contrastFilterRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCheckProperties();
            UpdateEnableProperties();

            if (EditorWorkspace.Instance.WholeImage)
            {
                ImageProcessor.ApplyGlobal(FilterStrategies.GetContrastFilter(EditorWorkspace.Instance.Contrast));
                EditorWorkspace.Instance.CountPixels();
                UpdateHistograms();
                workingPanel.Invalidate();
            }
        }

        private void gammaCorrectionFilterRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCheckProperties();
            UpdateEnableProperties();

            if (EditorWorkspace.Instance.WholeImage)
            {
                ImageProcessor.ApplyGlobal(FilterStrategies.GetGammaFilter(EditorWorkspace.Instance.Gamma));
                EditorWorkspace.Instance.CountPixels();
                UpdateHistograms();
                workingPanel.Invalidate();
            }
        }

        private void brightnessFilterTrackBar_Scroll(object sender, EventArgs e)
        {
            int value = brightnessFilterTrackBar.Value;
            EditorWorkspace.Instance.Brightness = value;
            brightnessValueLabel.Text = value.ToString();

            if (EditorWorkspace.Instance.WholeImage)
            {
                ImageProcessor.ApplyGlobal(FilterStrategies.GetBrightnessFilter(EditorWorkspace.Instance.Brightness));
                EditorWorkspace.Instance.CountPixels();
                UpdateHistograms();
                workingPanel.Invalidate();
            }
        }

        private void contrastFilterTrackBar_Scroll(object sender, EventArgs e)
        {
            int value = contrastFilterTrackBar.Value;
            EditorWorkspace.Instance.Contrast = value;
            contrastValueLabel.Text = value.ToString();

            if (EditorWorkspace.Instance.WholeImage)
            {
                ImageProcessor.ApplyGlobal(FilterStrategies.GetContrastFilter(EditorWorkspace.Instance.Contrast));
                EditorWorkspace.Instance.CountPixels();
                UpdateHistograms();
                workingPanel.Invalidate();
            }
        }

        private void gammaCorrectionFilterTrackBar_Scroll(object sender, EventArgs e)
        {
            float value = gammaCorrectionFilterTrackBar.Value / 10.0F;
            EditorWorkspace.Instance.Gamma = value;
            gammaCorrectionValueLabel.Text = value.ToString("F1");

            if (EditorWorkspace.Instance.WholeImage)
            {
                ImageProcessor.ApplyGlobal(FilterStrategies.GetGammaFilter(EditorWorkspace.Instance.Gamma));
                EditorWorkspace.Instance.CountPixels();
                UpdateHistograms();
                workingPanel.Invalidate();
            }
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

            int panelWidth = workingPanel.ClientSize.Width;
            int panelHeight = workingPanel.ClientSize.Height;
            int imageWidth = EditorWorkspace.Instance.WorkingImage.Width;
            int imageHeight = EditorWorkspace.Instance.WorkingImage.Height;

            float scaleX = (float)panelWidth / imageWidth;
            float scaleY = (float)panelHeight / imageHeight;
            float scale = Math.Min(scaleX, scaleY);

            int newWidth = (int)(imageWidth * scale);
            int newHeight = (int)(imageHeight * scale);

            int posX = (panelWidth - newWidth) / 2;
            int posY = (panelHeight - newHeight) / 2;

            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImage(EditorWorkspace.Instance.WorkingImage, posX, posY, newWidth, newHeight);

            if (EditorWorkspace.Instance.CircleBrush && EditorWorkspace.Instance.WorkingImage != null &&
                !_lastMousePosition.IsEmpty)
            {
                float screenRadius = EditorWorkspace.Instance.BrushRadius * scale;

                float cx = _lastMousePosition.X;
                float cy = _lastMousePosition.Y;

                Color penColor = _isPainting ? Color.LimeGreen : Color.Red;

                using (Pen pen = new Pen(penColor, 2))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    e.Graphics.DrawEllipse(pen, cx - screenRadius, cy - screenRadius, screenRadius * 2, screenRadius * 2);
                }
            }
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

            if (EditorWorkspace.Instance.WholeImage)
            {
                ImageProcessor.ApplyGlobal(strategy);
                EditorWorkspace.Instance.CountPixels();
                UpdateHistograms();
                workingPanel.Invalidate();
            }
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

        private void wholeImageRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            EditorWorkspace.Instance.WholeImage = wholeImageRadioButton.Checked;
            if (EditorWorkspace.Instance.WholeImage)
            {
                if (EditorWorkspace.Instance.OriginalImage == null)
                    return;

                if (EditorWorkspace.Instance.NoFilter)
                {
                    EditorWorkspace.Instance.WorkingImage?.Dispose();
                    EditorWorkspace.Instance.WorkingImage = (Bitmap)EditorWorkspace.Instance.OriginalImage.Clone();
                    EditorWorkspace.Instance.RestoreHistogramFromOriginal();
                    EditorWorkspace.Instance.CountPixels();

                    UpdateHistograms();
                    workingPanel.Invalidate();
                }
                else ApplyCurrentFilter();
            }
        }

        private void brushRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            EditorWorkspace.Instance.CircleBrush = brushRadioButton.Checked;
            brushNumericUpDown.Enabled = brushRadioButton.Checked;

            if (EditorWorkspace.Instance.CircleBrush)
            {
                if (EditorWorkspace.Instance.OriginalImage == null)
                    return;

                EditorWorkspace.Instance.WorkingImage?.Dispose();
                EditorWorkspace.Instance.WorkingImage = (Bitmap)EditorWorkspace.Instance.OriginalImage.Clone();

                EditorWorkspace.Instance.RestoreHistogramFromOriginal();
                UpdateHistograms();
                workingPanel.Invalidate();
            }
        }

        private void brushNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            EditorWorkspace.Instance.BrushRadius = (int)brushNumericUpDown.Value;
        }
        private Point? GetImageCoordinates(Point mousePosition)
        {
            if (EditorWorkspace.Instance.WorkingImage == null)
                return null;

            int mouseX = mousePosition.X;
            int mouseY = mousePosition.Y;

            int panelWidth = workingPanel.ClientSize.Width;
            int panelHeight = workingPanel.ClientSize.Height;
            int imageWidth = EditorWorkspace.Instance.WorkingImage.Width;
            int imageHeight = EditorWorkspace.Instance.WorkingImage.Height;

            float scaleX = (float)panelWidth / imageWidth;
            float scaleY = (float)panelHeight / imageHeight;
            float scale = Math.Min(scaleX, scaleY);

            int newWidth = (int)(imageWidth * scale);
            int newHeight = (int)(imageHeight * scale);

            int startX = (panelWidth - newWidth) / 2;
            int startY = (panelHeight - newHeight) / 2;

            if (mouseX < startX || mouseX >= startX + newWidth || mouseY < startY || mouseY >= startY + newHeight)
                return null;

            int imageX = (int)((mouseX - startX) / scale);
            int imageY = (int)((mouseY - startY) / scale);

            if (imageX < 0)
                imageX = 0;
            if (imageX >= imageWidth)
                imageX = imageWidth - 1;
            if (imageY < 0)
                imageY = 0;
            if (imageY >= imageHeight)
                imageY = imageHeight - 1;

            return new Point(imageX, imageY);
        }
        private void PerformBrushPainting(Point mousePosition)
        {
            if (!EditorWorkspace.Instance.CircleBrush || EditorWorkspace.Instance.WorkingImage == null)
                return;

            Point? imgPoint = GetImageCoordinates(mousePosition);
            if (imgPoint == null)
                return;

            PixelProcessor? strategy = null;

            if (EditorWorkspace.Instance.NoFilter) strategy = FilterStrategies.GetNoFilter();
            else if (EditorWorkspace.Instance.NegationFilter)
                strategy = FilterStrategies.GetNegationFilter();
            else if (EditorWorkspace.Instance.BrightnessFilter)
                strategy = FilterStrategies.GetBrightnessFilter(EditorWorkspace.Instance.Brightness);
            else if (EditorWorkspace.Instance.ContrastFilter)
                strategy = FilterStrategies.GetContrastFilter(EditorWorkspace.Instance.Contrast);
            else if (EditorWorkspace.Instance.GammaFilter)
                strategy = FilterStrategies.GetGammaFilter(EditorWorkspace.Instance.Gamma);
            else if (EditorWorkspace.Instance.CustomFunctionFilter)
                strategy = FilterStrategies.GetCustomFunctionFilter();

            if (strategy != null)
            {
                ImageProcessor.ApplyBrush(imgPoint.Value, strategy);

                float scale = Math.Min((float)workingPanel.ClientSize.Width / EditorWorkspace.Instance.WorkingImage.Width,
                    (float)workingPanel.ClientSize.Height / EditorWorkspace.Instance.WorkingImage.Height);

                int screenRadius = (int)(EditorWorkspace.Instance.BrushRadius * scale) + 2;
                int size = screenRadius * 2 + 4;

                Rectangle dirtyRect = new Rectangle(mousePosition.X - screenRadius,
                    mousePosition.Y - screenRadius, size, size);

                workingPanel.Invalidate(dirtyRect);
            }
        }
        private void workingPanel_MouseMove(object sender, MouseEventArgs e)
        {
            Point oldPosition = _lastMousePosition;
            _lastMousePosition = e.Location;

            if (_isPainting)
                PerformBrushPainting(e.Location);

            if (EditorWorkspace.Instance.CircleBrush && EditorWorkspace.Instance.WorkingImage != null)
            {
                float scale = Math.Min((float)workingPanel.ClientSize.Width / EditorWorkspace.Instance.WorkingImage.Width,
                    (float)workingPanel.ClientSize.Height / EditorWorkspace.Instance.WorkingImage.Height);

                int radius = (int)(EditorWorkspace.Instance.BrushRadius * scale) + 2;
                int size = radius * 2 + 8;

                workingPanel.Invalidate(new Rectangle(oldPosition.X - radius - 2,
                    oldPosition.Y - radius - 2, size, size));

                workingPanel.Invalidate(new Rectangle(_lastMousePosition.X - radius - 2,
                    _lastMousePosition.Y - radius - 2, size, size));
            }
        }

        private void workingPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && EditorWorkspace.Instance.CircleBrush)
            {
                _isPainting = true;
                PerformBrushPainting(e.Location);
            }
        }

        private void workingPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (_isPainting)
            {
                _isPainting = false;
                UpdateHistograms(); 
            }
        }

        private void workingPanel_MouseLeave(object sender, EventArgs e)
        {
            _lastMousePosition = Point.Empty;
            workingPanel.Invalidate();
        }

        private void ConfigureSingleChart(Chart chart, Color color)
        {
            chart.Series.Clear();
            chart.ChartAreas.Clear();
            chart.Legends.Clear();

            ChartArea area = new ChartArea("Area1");

            area.AxisX.Minimum = 0;
            area.AxisX.Maximum = 255;
            area.AxisX.Interval = 50;
            area.AxisX.LabelStyle.Enabled = true;
            area.AxisX.LabelStyle.Font = new Font("Arial", 7F);
            area.AxisX.LabelStyle.ForeColor = Color.Gray;

            area.AxisX.MajorGrid.LineColor = Color.LightGray;
            area.AxisX.MajorGrid.LineWidth = 1;
            area.AxisX.MajorTickMark.Enabled = false;

            area.AxisY.Minimum = 0;
            area.AxisY.Maximum = 5000;
            area.AxisY.Interval = 1000;
            area.AxisY.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
            area.AxisY.LineColor = Color.Transparent;
            area.AxisY.LabelStyle.Font = new Font("Arial", 7F);
            area.AxisY.LabelStyle.ForeColor = Color.Gray;

            area.Position.Auto = true;

            chart.ChartAreas.Add(area);

            Series series = new Series("Data");
            series.ChartType = SeriesChartType.Column;
            series["PointWidth"] = "1";
            series.Color = color;
            series.BorderWidth = 0;

            chart.Series.Add(series);
        }
        private void InitializeCharts()
        {
            ConfigureSingleChart(chartRed, Color.Red);
            ConfigureSingleChart(chartGreen, Color.Green);
            ConfigureSingleChart(chartBlue, Color.Blue);
        }
        private void UpdateHistograms()
        {
            if (EditorWorkspace.Instance.WorkingImage == null)
                return;

            long totalPixels = (long)(EditorWorkspace.Instance.WorkingImage.Width * 
                EditorWorkspace.Instance.WorkingImage.Height);
            int maxPixelCount = (totalPixels < 2000000) ? 5000 : 50000;
            void AdjustYAxis(Chart chart, int maxVal)
            {
                ChartArea area = chart.ChartAreas[0];

                if (area.AxisY.Maximum == maxVal) 
                    return;

                area.AxisY.Maximum = maxVal;
                area.AxisY.Interval = maxVal / 5;

                area.AxisY.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
                area.AxisY.LineColor = Color.Transparent;
                area.AxisY.LabelStyle.Font = new Font("Arial", 7F);
                area.AxisY.LabelStyle.ForeColor = Color.Gray;
            }

            AdjustYAxis(chartRed, maxPixelCount);
            AdjustYAxis(chartGreen, maxPixelCount);
            AdjustYAxis(chartBlue, maxPixelCount);

            void FillData(Chart chart, int[] data)
            {
                if (chart.Series.Count > 0)
                {
                    chart.Series.SuspendUpdates();
                    chart.Series["Data"].Points.Clear();
                    for (int i = 0; i <= 255; i++)
                    {
                        int value = Math.Min(maxPixelCount, data[i]);
                        chart.Series["Data"].Points.AddXY(i, value);
                    }

                    chart.Series.ResumeUpdates();
                    chart.Invalidate();
                }
            }

            FillData(chartRed, EditorWorkspace.Instance.RedData);
            FillData(chartGreen, EditorWorkspace.Instance.GreenData);
            FillData(chartBlue, EditorWorkspace.Instance.BlueData);
        }
    }
}
