namespace DevRunner;

public class InputDialog : Form
{
    private TextBox txtInput = null!;
    public string InputValue => txtInput.Text;

    public InputDialog(string title, string prompt, string defaultValue = "")
    {
        this.Text = title;
        this.Size = new Size(400, 150);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.BackColor = Color.FromArgb(30, 30, 35);

        var lblPrompt = new Label
        {
            Text = prompt,
            Location = new Point(20, 20),
            AutoSize = true,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10F)
        };
        this.Controls.Add(lblPrompt);

        txtInput = new TextBox
        {
            Location = new Point(20, 50),
            Width = 340,
            Text = defaultValue,
            BackColor = Color.FromArgb(45, 45, 50),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10F)
        };
        txtInput.SelectAll();
        this.Controls.Add(txtInput);

        var btnOK = new Button
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            Location = new Point(200, 85),
            Width = 75,
            BackColor = Color.FromArgb(46, 204, 113),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        btnOK.FlatAppearance.BorderSize = 0;
        this.Controls.Add(btnOK);

        var btnCancel = new Button
        {
            Text = "Cancel",
            DialogResult = DialogResult.Cancel,
            Location = new Point(285, 85),
            Width = 75,
            BackColor = Color.FromArgb(231, 76, 60),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        btnCancel.FlatAppearance.BorderSize = 0;
        this.Controls.Add(btnCancel);

        this.AcceptButton = btnOK;
        this.CancelButton = btnCancel;
    }
}
