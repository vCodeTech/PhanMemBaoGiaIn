partial class frmMain
{
    private System.ComponentModel.IContainer components = null;
    private Button btnChonFile;

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
    }
}
