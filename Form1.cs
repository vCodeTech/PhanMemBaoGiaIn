using BitMiracle.LibTiff.Classic;
using SixLabors.ImageSharp.PixelFormats;
using System.Data;
using System.Text;

public partial class frmMain : Form
{
    public frmMain()
    {
        InitializeComponent();
    }

    private void BtnChonFile_Click(object sender, EventArgs e)
    {

        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.Filter = "Image Files (*.jpg;*.tif)|*.jpg;*.tif";
            openFileDialog.Title = "Chọn File Hình Ảnh";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                DataTable fileInfoTable = new DataTable();
                fileInfoTable.Columns.Add("File Name", typeof(string));
                fileInfoTable.Columns.Add("DPI", typeof(string));
                fileInfoTable.Columns.Add("Width", typeof(int));
                fileInfoTable.Columns.Add("Height", typeof(int));
                fileInfoTable.Columns.Add("Width (mm)", typeof(double));
                fileInfoTable.Columns.Add("Height (mm)", typeof(double));
                fileInfoTable.Columns.Add("Quantity", typeof(int));  // Thêm cột số lượng
                fileInfoTable.Columns.Add("Area (m²)", typeof(double));
                fileInfoTable.Columns.Add("Total Area (m²)", typeof(double));  // Thêm cột tổng diện tích
                fileInfoTable.Columns.Add("Color Mode", typeof(string));



                foreach (string filePath in openFileDialog.FileNames)
                {
                    if (!File.Exists(filePath))
                    {
                        MessageBox.Show($"File không tồn tại: {filePath}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }

                    try
                    {
                        if (Path.GetExtension(filePath).ToLower() == ".tif")
                        {
                            using (var tiff = BitMiracle.LibTiff.Classic.Tiff.Open(filePath, "r"))
                            {
                                if (tiff == null)
                                {
                                    MessageBox.Show($"Không thể đọc file TIFF: {filePath}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    continue;
                                }

                                // Đọc thông tin kích thước
                                int width = tiff.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
                                int height = tiff.GetField(TiffTag.IMAGELENGTH)[0].ToInt();

                                // Đọc thông tin DPI
                                var resUnit = tiff.GetField(TiffTag.RESOLUTIONUNIT);
                                var xRes = tiff.GetField(TiffTag.XRESOLUTION);
                                var yRes = tiff.GetField(TiffTag.YRESOLUTION);

                                string dpi = "N/A";
                                double widthMm = 0, heightMm = 0, areaSqM = 0;

                                if (resUnit != null && xRes != null && yRes != null)
                                {
                                    float xDpi = xRes[0].ToFloat();
                                    float yDpi = yRes[0].ToFloat();
                                    dpi = $"{xDpi:F0} x {yDpi:F0}";

                                    // Sử dụng DPI trung bình cho tính toán
                                    float avgDpi = (xDpi + yDpi) / 2;
                                    (widthMm, heightMm, areaSqM) = CalculateDimensions(width, height, avgDpi);
                                }

                                // Đọc thông tin hệ màu
                                var photometric = tiff.GetField(TiffTag.PHOTOMETRIC);
                                var samplesPerPixel = tiff.GetField(TiffTag.SAMPLESPERPIXEL);

                                string colorMode = "Unknown";
                                if (photometric != null && samplesPerPixel != null)
                                {
                                    int samples = samplesPerPixel[0].ToInt();
                                    if (samples == 4) colorMode = "CMYK";
                                    else if (samples == 3) colorMode = "RGB";
                                    else if (samples == 1) colorMode = "Grayscale";
                                }

                                fileInfoTable.Rows.Add(
                                    Path.GetFileName(filePath),
                                    dpi,
                                    width,
                                    height,
                                    Math.Round(widthMm, 1),
                                    Math.Round(heightMm, 1),
                                    1,  // Quantity mặc định là 1
                                    Math.Round(areaSqM, 4),  // Area (m²)
                                    Math.Round(areaSqM, 4),  // Total Area (m²) ban đầu bằng Area
                                    colorMode
                                );
                            }
                        }

                        else
                        {
                            // Xử lý file JPEG bằng ImageSharp
                            using (var image = SixLabors.ImageSharp.Image.Load<Rgba32>(filePath))
                            {
                                var metadata = image.Metadata;
                                string dpi = "N/A";
                                double widthMm = 0, heightMm = 0, areaSqM = 0;

                                if (metadata.ResolutionUnits == SixLabors.ImageSharp.Metadata.PixelResolutionUnit.PixelsPerInch)
                                {
                                    float xDpi = (float)metadata.HorizontalResolution;
                                    float yDpi = (float)metadata.VerticalResolution;
                                    dpi = $"{xDpi:F0} x {yDpi:F0}";

                                    // Sử dụng DPI trung bình cho tính toán
                                    float avgDpi = (xDpi + yDpi) / 2;
                                    (widthMm, heightMm, areaSqM) = CalculateDimensions(image.Width, image.Height, avgDpi);
                                }
                                // Trong phần xử lý file JPEG
                                fileInfoTable.Rows.Add(
                                    Path.GetFileName(filePath),
                                    dpi,
                                    image.Width,
                                    image.Height,
                                    Math.Round(widthMm, 1),
                                    Math.Round(heightMm, 1),
                                    1,  // Quantity mặc định là 1
                                    Math.Round(areaSqM, 4),  // Area (m²)
                                    Math.Round(areaSqM, 4),  // Total Area (m²) ban đầu bằng Area
                                    "RGB"
                                );
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi đọc file: {filePath}\nChi tiết: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                // Update the main form's DataGridView
                dataGridView1.DataSource = fileInfoTable;

                // Set default values for Quantity column
                foreach (DataRow row in fileInfoTable.Rows)
                {
                    row["Quantity"] = 1;
                    row["Total Area (m²)"] = Convert.ToDouble(row["Area (m²)"]);
                }
                // Wire up events (move the existing event handlers here)
                dataGridView1.DataBindingComplete += (s, ev) =>
                {
                    if (dataGridView1.Columns["Area (m²)"] != null)
                    {
                        dataGridView1.Columns["Area (m²)"].DefaultCellStyle.Format = "N4";
                    }
                    if (dataGridView1.Columns["Total Area (m²)"] != null)
                    {
                        dataGridView1.Columns["Total Area (m²)"].DefaultCellStyle.Format = "N4";
                        dataGridView1.Columns["Total Area (m²)"].ReadOnly = true;
                    }
                };
                

                //// Thiết lập các thuộc tính cho cột
                //dataGridView.Columns["Quantity"].DefaultCellStyle.Format = "N0";
                //dataGridView.Columns["Area (m²)"].DefaultCellStyle.Format = "N4";
                ////dataGridView.Columns["Total Area (m²)"].DefaultCellStyle.Format = "N4";
                //dataGridView.Columns["Total Area (m²)"].ReadOnly = true;
                //dataGridView.Columns["Quantity"].DefaultCellStyle.NullValue = 1;
                //dataGridView.Columns["Quantity"].ValueType = typeof(int);
                dataGridView1.CellValidating += (sender, e) =>
                {
                    if (e.ColumnIndex == dataGridView1.Columns["Quantity"].Index)
                    {
                        if (!int.TryParse(e.FormattedValue.ToString(), out int value) || value < 0)
                        {
                            e.Cancel = true;
                            MessageBox.Show("Vui lòng nhập số nguyên không âm!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                };
                dataGridView1.CellValueChanged += (sender, e) =>
                {
                    try
                    {
                        if (e.ColumnIndex == dataGridView1.Columns["Quantity"].Index && e.RowIndex >= 0)
                        {
                            var row = dataGridView1.Rows[e.RowIndex];
                            if (row?.Cells["Quantity"].Value == null || row?.Cells["Area (m²)"].Value == null)
                            {
                                return;
                            }

                            if (int.TryParse(row.Cells["Quantity"].Value.ToString(), out int quantity) &&
                                double.TryParse(row.Cells["Area (m²)"].Value.ToString(), out double area))
                            {
                                if (quantity < 0)
                                {
                                    MessageBox.Show("Số lượng không thể âm!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    row.Cells["Quantity"].Value = 1;
                                    return;
                                }

                                row.Cells["Total Area (m²)"].Value = quantity * area;
                                UpdateTotalArea(lblTotalArea, dataGridView1);
                            }
                            else
                            {
                                MessageBox.Show("Giá trị không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                row.Cells["Quantity"].Value = 1;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                dataGridView1.SelectionChanged += (sender, e) =>
                {
                    try
                    {
                        UpdateTotalArea(lblTotalArea, dataGridView1);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                btnCopyTotal.Click += (sender, e) =>
                {
                    double total = CalculateSelectedTotal(dataGridView1);
                    Clipboard.SetText($"Tổng diện tích: {total:N4} m²");
                };

                // Xử lý sự kiện copy chi tiết
                btnCopyDetail.Click += (sender, e) =>
                {
                    StringBuilder detail = new StringBuilder();
                    detail.AppendLine("Tên file | Kích thước ngang x dọc | SL | M2");
                    int totalQuantity = 0;
                    double totalArea = 0;

                    foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                    {
                        string fileName = row.Cells["File Name"].Value.ToString();
                        double width = Convert.ToDouble(row.Cells["Width (mm)"].Value);
                        double height = Convert.ToDouble(row.Cells["Height (mm)"].Value);
                        int quantity = Convert.ToInt32(row.Cells["Quantity"].Value);
                        double totalRowArea = Convert.ToDouble(row.Cells["Total Area (m²)"].Value);

                        detail.AppendLine($"{fileName} | {width:N1} x {height:N1} | {quantity} | {totalRowArea:N4}");
                        totalQuantity += quantity;
                        totalArea += totalRowArea;
                    }

                    detail.AppendLine($"Tổng cộng: {totalQuantity} tấm, {totalArea:N4} m²");
                    Clipboard.SetText(detail.ToString());
                };
            }
        }
    }
    private (double widthMm, double heightMm, double areaSqM) CalculateDimensions(int widthPixels, int heightPixels, float dpi)
    {
        // Công thức: 
        // 1 inch = 25.4 mm
        // Chiều rộng (mm) = (width_pixels / dpi) * 25.4
        // Chiều cao (mm) = (height_pixels / dpi) * 25.4
        // Diện tích (m²) = (chiều rộng_mm * chiều cao_mm) / 1,000,000

        double widthMm = (widthPixels / dpi) * 25.4;
        double heightMm = (heightPixels / dpi) * 25.4;
        double areaSqM = (widthMm * heightMm) / 1_000_000; // Chuyển từ mm² sang m²

        return (widthMm, heightMm, areaSqM);
    }

    private void UpdateTotalArea(Label lblTotalArea, DataGridView dataGridView)
    {
        try
        {
            double total = CalculateSelectedTotal(dataGridView);
            lblTotalArea.Text = $"Tổng diện tích: {total:N4} m²";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi cập nhật tổng diện tích: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private double CalculateSelectedTotal(DataGridView dataGridView)
    {
        try
        {
            if (dataGridView?.SelectedRows == null || dataGridView.SelectedRows.Count == 0)
                return 0;

            return dataGridView.SelectedRows.Cast<DataGridViewRow>()
                .Where(row => row?.Cells["Total Area (m²)"]?.Value != null)
                .Sum(row => Convert.ToDouble(row.Cells["Total Area (m²)"].Value));
        }
        catch (Exception)
        {
            return 0;
        }
    }
}
