partial class frmMain
{
    private System.ComponentModel.IContainer components = null;
    private Button btnChonFile;
    private DataGridView dataGridView1;
    private TableLayoutPanel buttonPanel;
    private Label lblTotalArea;
    private Button btnCopyTotal;
    private Button btnCopyDetail;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        btnChonFile = new Button();
        SuspendLayout();
        
        btnChonFile.Location = new Point(12, 12);
        btnChonFile.Name = "btnChonFile";
        btnChonFile.Size = new Size(82, 23);
        btnChonFile.TabIndex = 0;
        btnChonFile.Text = "Chọn File In";
        btnChonFile.UseVisualStyleBackColor = true;
        btnChonFile.Click += BtnChonFile_Click;

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 450);
        Controls.Add(btnChonFile);
        Name = "frmMain";
        Text = "Tính Tiền In";
        ResumeLayout(false);

        this.dataGridView1 = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = true,
            AllowUserToAddRows = false
        };

        this.buttonPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 40,
            Padding = new Padding(5),
            ColumnCount = 4
        };

        this.lblTotalArea = new Label
        {
            Text = "Tổng diện tích: 0 m²",
            AutoSize = true,
            Anchor = AnchorStyles.Left | AnchorStyles.Right
        };

        this.btnCopyTotal = new Button
        {
            Text = "Copy Tổng",
            AutoSize = true
        };

        this.btnCopyDetail = new Button
        {
            Text = "Copy Chi Tiết",
            AutoSize = true
        };

        // Add controls to form
        this.buttonPanel.Controls.Add(this.lblTotalArea, 0, 0);
        this.buttonPanel.Controls.Add(this.btnCopyTotal, 1, 0);
        this.buttonPanel.Controls.Add(this.btnCopyDetail, 2, 0);
        this.buttonPanel.Controls.Add(this.btnChonFile, 3, 0);
        this.Controls.Add(this.dataGridView1);
        this.Controls.Add(this.buttonPanel);
    }
}
