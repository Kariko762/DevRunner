namespace DevRunner;

public class TerminalConfigDialog : Form
{
    private TextBox txtName = null!;
    private ComboBox cmbColorScheme = null!;
    
    public string TerminalName => txtName.Text;
    public string ColorScheme => cmbColorScheme.SelectedItem?.ToString() ?? "Matrix Green";

    public TerminalConfigDialog(string title, string currentName = "", string currentColorScheme = "Matrix Green")
    {
        this.Text = title;
        this.Size = new Size(450, 250);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.BackColor = Color.FromArgb(30, 30, 35);

        // Terminal Name
        var lblName = new Label
        {
            Text = "Terminal Name:",
            Location = new Point(20, 20),
            AutoSize = true,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10F)
        };
        this.Controls.Add(lblName);

        txtName = new TextBox
        {
            Location = new Point(20, 45),
            Width = 390,
            Text = currentName,
            BackColor = Color.FromArgb(45, 45, 50),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10F),
            BorderStyle = BorderStyle.FixedSingle
        };
        txtName.SelectAll();
        this.Controls.Add(txtName);

        // Color Scheme
        var lblColorScheme = new Label
        {
            Text = "Color Scheme:",
            Location = new Point(20, 85),
            AutoSize = true,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10F)
        };
        this.Controls.Add(lblColorScheme);

        cmbColorScheme = new ComboBox
        {
            Location = new Point(20, 110),
            Width = 390,
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = Color.FromArgb(45, 45, 50),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10F),
            FlatStyle = FlatStyle.Flat
        };
        
        cmbColorScheme.DrawMode = DrawMode.OwnerDrawFixed;
        cmbColorScheme.DrawItem += CmbColorScheme_DrawItem;
        
        foreach (var scheme in ColorSchemes.Schemes.Keys)
        {
            cmbColorScheme.Items.Add(scheme);
        }
        
        cmbColorScheme.SelectedItem = currentColorScheme;
        if (cmbColorScheme.SelectedIndex == -1)
        {
            cmbColorScheme.SelectedIndex = 0;
        }
        
        this.Controls.Add(cmbColorScheme);

        // Buttons
        var btnOK = new Button
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            Location = new Point(240, 160),
            Width = 80,
            Height = 32,
            BackColor = Color.FromArgb(46, 204, 113),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10F)
        };
        btnOK.FlatAppearance.BorderSize = 0;
        this.Controls.Add(btnOK);

        var btnCancel = new Button
        {
            Text = "Cancel",
            DialogResult = DialogResult.Cancel,
            Location = new Point(330, 160),
            Width = 80,
            Height = 32,
            BackColor = Color.FromArgb(231, 76, 60),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10F)
        };
        btnCancel.FlatAppearance.BorderSize = 0;
        this.Controls.Add(btnCancel);

        this.AcceptButton = btnOK;
        this.CancelButton = btnCancel;
    }

    private void CmbColorScheme_DrawItem(object? sender, DrawItemEventArgs e)
    {
        if (e.Index < 0) return;

        e.DrawBackground();
        
        var schemeName = cmbColorScheme.Items[e.Index].ToString()!;
        var scheme = ColorSchemes.Schemes[schemeName];
        
        // Draw color preview box with output colors
        var previewRect = new Rectangle(e.Bounds.Left + 5, e.Bounds.Top + 3, 40, e.Bounds.Height - 6);
        using (var bgBrush = new SolidBrush(scheme.OutputBackground))
        {
            e.Graphics.FillRectangle(bgBrush, previewRect);
        }
        
        // Draw sample text in the preview
        using (var fgBrush = new SolidBrush(scheme.OutputForeground))
        {
            e.Graphics.DrawString("Aa", new Font("Consolas", 8F), fgBrush, 
                new RectangleF(previewRect.X + 8, previewRect.Y + 2, previewRect.Width, previewRect.Height));
        }
        
        // Draw border around preview using accent color
        using (var borderPen = new Pen(scheme.AccentColor))
        {
            e.Graphics.DrawRectangle(borderPen, previewRect);
        }
        
        // Draw scheme name in title color
        var textColor = (e.State & DrawItemState.Selected) == DrawItemState.Selected 
            ? Color.White 
            : scheme.TitleColor;
        
        using (var textBrush = new SolidBrush(textColor))
        {
            e.Graphics.DrawString(schemeName, e.Font!, textBrush, 
                new PointF(e.Bounds.Left + 55, e.Bounds.Top + 5));
        }
        
        e.DrawFocusRectangle();
    }
}
