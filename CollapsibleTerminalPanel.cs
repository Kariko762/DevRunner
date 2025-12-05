using System.Diagnostics;

namespace DevRunner;

public class CollapsibleTerminalPanel : Panel
{
    private TerminalPanel terminalPanel = null!;
    private Panel headerPanel = null!;
    private Label lblTitle = null!;
    private Button btnCollapse = null!;
    private Button btnRename = null!;
    private Button btnMoveUp = null!;
    private Button btnMoveDown = null!;
    private Button btnDelete = null!;
    private bool isCollapsed = false;
    private int expandedHeight = 300;

    public event EventHandler? RenameRequested;
    public event EventHandler? MoveUpRequested;
    public event EventHandler? MoveDownRequested;
    public event EventHandler? DeleteRequested;

    public string Title { get; set; }
    public string RunCommand => terminalPanel.RunCommand;
    public string BuildCommand => terminalPanel.BuildCommand;
    public string WorkingDirectory => terminalPanel.WorkingDirectory;
    public string ColorScheme => terminalPanel.ColorScheme;

    public CollapsibleTerminalPanel(string title)
    {
        Title = title;
        InitializeControls();
    }

    private void InitializeControls()
    {
        this.BackColor = Color.FromArgb(35, 35, 40);
        this.BorderStyle = BorderStyle.FixedSingle;
        this.Margin = new Padding(5);
        this.Height = expandedHeight;

        // Header panel
        headerPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 35,
            BackColor = Color.FromArgb(45, 45, 50)
        };

        lblTitle = new Label
        {
            Text = Title,
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            Location = new Point(10, 8),
            AutoSize = true,
            ForeColor = Color.FromArgb(100, 180, 255),
            BackColor = Color.Transparent
        };
        lblTitle.Click += LblTitle_Click;
        lblTitle.Cursor = Cursors.Hand;
        headerPanel.Controls.Add(lblTitle);

        // Button positioning from right to left
        int buttonX = headerPanel.Width - 35;
        const int buttonSize = 25;
        const int buttonSpacing = 5;

        btnCollapse = new Button
        {
            Text = "−",
            Location = new Point(buttonX, 5),
            Width = buttonSize,
            Height = buttonSize,
            BackColor = Color.FromArgb(60, 60, 65),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };
        btnCollapse.FlatAppearance.BorderSize = 0;
        btnCollapse.Click += BtnCollapse_Click;
        headerPanel.Controls.Add(btnCollapse);
        buttonX -= (buttonSize + buttonSpacing);

        btnDelete = new Button
        {
            Text = "✕",
            Location = new Point(buttonX, 5),
            Width = buttonSize,
            Height = buttonSize,
            BackColor = Color.FromArgb(231, 76, 60),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };
        btnDelete.FlatAppearance.BorderSize = 0;
        btnDelete.Click += BtnDelete_Click;
        headerPanel.Controls.Add(btnDelete);
        buttonX -= (buttonSize + buttonSpacing);

        btnMoveDown = new Button
        {
            Text = "▼",
            Location = new Point(buttonX, 5),
            Width = buttonSize,
            Height = buttonSize,
            BackColor = Color.FromArgb(60, 60, 65),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 7F),
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };
        btnMoveDown.FlatAppearance.BorderSize = 0;
        btnMoveDown.Click += BtnMoveDown_Click;
        headerPanel.Controls.Add(btnMoveDown);
        buttonX -= (buttonSize + buttonSpacing);

        btnMoveUp = new Button
        {
            Text = "▲",
            Location = new Point(buttonX, 5),
            Width = buttonSize,
            Height = buttonSize,
            BackColor = Color.FromArgb(60, 60, 65),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 7F),
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };
        btnMoveUp.FlatAppearance.BorderSize = 0;
        btnMoveUp.Click += BtnMoveUp_Click;
        headerPanel.Controls.Add(btnMoveUp);
        buttonX -= (buttonSize + buttonSpacing);

        btnRename = new Button
        {
            Text = "✎",
            Location = new Point(buttonX, 5),
            Width = buttonSize,
            Height = buttonSize,
            BackColor = Color.FromArgb(60, 60, 65),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10F),
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };
        btnRename.FlatAppearance.BorderSize = 0;
        btnRename.Click += BtnRename_Click;
        headerPanel.Controls.Add(btnRename);

        this.Controls.Add(headerPanel);

        // Terminal panel
        terminalPanel = new TerminalPanel(Title)
        {
            Dock = DockStyle.Fill
        };
        this.Controls.Add(terminalPanel);
    }

    private void BtnCollapse_Click(object? sender, EventArgs e)
    {
        isCollapsed = !isCollapsed;
        
        // Suspend layout during changes
        var parent = this.Parent as FlowLayoutPanel;
        parent?.SuspendLayout();
        
        if (isCollapsed)
        {
            expandedHeight = this.Height;
            this.Height = 35;
            terminalPanel.Visible = false;
            this.BackColor = Color.FromArgb(50, 50, 55);
            btnCollapse.Text = "+";
        }
        else
        {
            this.Height = expandedHeight;
            terminalPanel.Visible = true;
            this.BackColor = Color.FromArgb(35, 35, 40);
            btnCollapse.Text = "−";
        }
        
        // Resume and force re-layout
        parent?.ResumeLayout(true);
        parent?.PerformLayout();
    }

    public void LoadSettings(string directory, string runCommand, string buildCommand, string colorScheme = "Matrix Green")
    {
        terminalPanel.LoadSettings(directory, runCommand, buildCommand, colorScheme);
        ApplyColorScheme(colorScheme);
    }

    private void ApplyColorScheme(string colorScheme)
    {
        if (ColorSchemes.Schemes.TryGetValue(colorScheme, out var scheme))
        {
            // Apply to header
            headerPanel.BackColor = scheme.HeaderBackground;
            lblTitle.ForeColor = scheme.TitleColor;
            
            // Apply to container
            this.BackColor = scheme.ContainerBackground;
            
            // Apply to all buttons
            btnRename.BackColor = scheme.ButtonBackground;
            btnMoveUp.BackColor = scheme.ButtonBackground;
            btnMoveDown.BackColor = scheme.ButtonBackground;
            btnCollapse.BackColor = scheme.ButtonBackground;
            
            // Delete button keeps red color but adjust shade based on scheme
            btnDelete.BackColor = Color.FromArgb(
                Math.Min(255, scheme.ButtonBackground.R + 100),
                Math.Max(0, scheme.ButtonBackground.G - 20),
                Math.Max(0, scheme.ButtonBackground.B - 20)
            );
        }
    }

    public void ExecuteRunCommand()
    {
        terminalPanel.ExecuteRunCommand();
    }

    public void StopProcess()
    {
        terminalPanel.StopProcess();
    }

    public void ClearOutput()
    {
        terminalPanel.ClearOutput();
    }

    public void Cleanup()
    {
        terminalPanel.Cleanup();
    }

    private void LblTitle_Click(object? sender, EventArgs e)
    {
        BtnRename_Click(sender, e);
    }

    private void BtnRename_Click(object? sender, EventArgs e)
    {
        RenameRequested?.Invoke(this, EventArgs.Empty);
    }

    private void BtnMoveUp_Click(object? sender, EventArgs e)
    {
        MoveUpRequested?.Invoke(this, EventArgs.Empty);
    }

    private void BtnMoveDown_Click(object? sender, EventArgs e)
    {
        MoveDownRequested?.Invoke(this, EventArgs.Empty);
    }

    private void BtnDelete_Click(object? sender, EventArgs e)
    {
        DeleteRequested?.Invoke(this, EventArgs.Empty);
    }

    public void UpdateTitle(string newTitle)
    {
        Title = newTitle;
        lblTitle.Text = newTitle;
    }
}
