namespace DevRunner;

public class MainForm : Form
{
    private TerminalPanel frontEndPanel = null!;
    private TerminalPanel backEndPanel = null!;
    private AppSettings settings = null!;
    private MenuStrip menuStrip = null!;
    private Button btnRunBoth = null!;
    private Button btnStopBoth = null!;
    private bool runBothConfirmMode = false;
    private bool stopBothConfirmMode = false;

    public MainForm()
    {
        InitializeComponent();
        LoadSettings();
    }

    private void InitializeComponent()
    {
        this.Text = "DevRunner - Dev Environment Manager";
        this.Size = new Size(1400, 800);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormClosing += MainForm_FormClosing;
        this.BackColor = Color.FromArgb(30, 30, 35);

        // Create menu bar
        CreateMenuBar();

        // Create split container
        var splitContainer = new SplitContainer
        {
            Dock = DockStyle.Fill,
            SplitterWidth = 3,
            BackColor = Color.FromArgb(45, 45, 50),
            IsSplitterFixed = false // Allow user to adjust if needed
        };

        // Set to 50/50 split after it's loaded
        this.Load += (s, e) =>
        {
            splitContainer.SplitterDistance = splitContainer.Width / 2;
        };

        // Maintain 50/50 on resize
        this.Resize += (s, e) =>
        {
            if (splitContainer.Width > 0)
            {
                splitContainer.SplitterDistance = splitContainer.Width / 2;
            }
        };

        // Create panels
        frontEndPanel = new TerminalPanel("Frontend");
        backEndPanel = new TerminalPanel("Backend");

        splitContainer.Panel1.Controls.Add(frontEndPanel);
        splitContainer.Panel2.Controls.Add(backEndPanel);

        // Add Run Both button to frontend panel
        btnRunBoth = new Button
        {
            Text = "⚡ Run Both",
            Location = new Point(frontEndPanel.Width - 120, 40),
            Width = 110,
            Height = 28,
            BackColor = Color.FromArgb(155, 89, 182),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };
        btnRunBoth.FlatAppearance.BorderSize = 0;
        btnRunBoth.Click += BtnRunBoth_Click;
        frontEndPanel.Controls.Add(btnRunBoth);

        // Add Stop Both button to frontend panel (below Run Both)
        btnStopBoth = new Button
        {
            Text = "⏹ Stop Both",
            Location = new Point(frontEndPanel.Width - 120, 72),
            Width = 110,
            Height = 28,
            BackColor = Color.FromArgb(231, 76, 60),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };
        btnStopBoth.FlatAppearance.BorderSize = 0;
        btnStopBoth.Click += BtnStopBoth_Click;
        frontEndPanel.Controls.Add(btnStopBoth);

        this.Controls.Add(splitContainer);
    }

    private void BtnStopBoth_Click(object? sender, EventArgs e)
    {
        if (!stopBothConfirmMode)
        {
            // First click - enter confirm mode
            stopBothConfirmMode = true;
            btnStopBoth.Text = "Confirm";
            btnStopBoth.BackColor = Color.FromArgb(178, 26, 83);
            
            // Reset after 3 seconds if not clicked
            var timer = new System.Windows.Forms.Timer { Interval = 3000 };
            timer.Tick += (s, ev) =>
            {
                ResetStopBothButton();
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }
        else
        {
            // Second click - stop both
            ResetStopBothButton();
            frontEndPanel.StopProcess();
            backEndPanel.StopProcess();
        }
    }

    private void ResetStopBothButton()
    {
        stopBothConfirmMode = false;
        btnStopBoth.Text = "⏹ Stop Both";
        btnStopBoth.BackColor = Color.FromArgb(231, 76, 60);
    }

    private async void BtnRunBoth_Click(object? sender, EventArgs e)
    {
        if (!runBothConfirmMode)
        {
            // First click - enter confirm mode
            runBothConfirmMode = true;
            btnRunBoth.Text = "Confirm";
            btnRunBoth.BackColor = Color.FromArgb(178, 26, 83);
            
            // Reset after 3 seconds if not clicked
            var timer = new System.Windows.Forms.Timer { Interval = 3000 };
            timer.Tick += (s, ev) =>
            {
                ResetRunBothButton();
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }
        else
        {
            // Second click - execute both
            ResetRunBothButton();
            btnRunBoth.Enabled = false;
            btnRunBoth.Text = "Running...";
            
            // Clear both terminals
            backEndPanel.ClearOutput();
            frontEndPanel.ClearOutput();
            
            // Stop both terminals
            backEndPanel.StopProcess();
            frontEndPanel.StopProcess();
            
            await Task.Delay(500); // Brief pause after stopping
            
            // Run backend
            backEndPanel.ExecuteRunCommand();
            
            // Wait 4 seconds
            await Task.Delay(4000);
            
            // Run frontend
            frontEndPanel.ExecuteRunCommand();
            
            btnRunBoth.Enabled = true;
            btnRunBoth.Text = "⚡ Run Both";
        }
    }

    private void ResetRunBothButton()
    {
        runBothConfirmMode = false;
        btnRunBoth.Text = "⚡ Run Both";
        btnRunBoth.BackColor = Color.FromArgb(155, 89, 182);
    }

    private void CreateMenuBar()
    {
        menuStrip = new MenuStrip
        {
            BackColor = Color.FromArgb(40, 40, 45),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10F)
        };

        // File Menu
        var fileMenu = new ToolStripMenuItem("File");
        fileMenu.DropDownItems.Add("Exit", null, (s, e) => Application.Exit());

        // Help Menu
        var helpMenu = new ToolStripMenuItem("Help");
        helpMenu.DropDownItems.Add("About", null, (s, e) => 
            MessageBox.Show("DevRunner v1.0\nDev Environment Manager", "About"));

        menuStrip.Items.Add(fileMenu);
        menuStrip.Items.Add(helpMenu);

        this.MainMenuStrip = menuStrip;
        this.Controls.Add(menuStrip);
    }

    private void LoadSettings()
    {
        settings = AppSettings.Load();
        
        frontEndPanel.LoadSettings(
            settings.FrontEndDirectory,
            settings.FrontEndRunCommand,
            settings.FrontEndBuildCommand
        );

        backEndPanel.LoadSettings(
            settings.BackEndDirectory,
            settings.BackEndRunCommand,
            settings.BackEndBuildCommand
        );
    }

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        // Save settings
        settings.FrontEndDirectory = frontEndPanel.WorkingDirectory;
        settings.FrontEndRunCommand = frontEndPanel.RunCommand;
        settings.FrontEndBuildCommand = frontEndPanel.BuildCommand;

        settings.BackEndDirectory = backEndPanel.WorkingDirectory;
        settings.BackEndRunCommand = backEndPanel.RunCommand;
        settings.BackEndBuildCommand = backEndPanel.BuildCommand;

        settings.Save();

        // Clean up processes
        frontEndPanel.Cleanup();
        backEndPanel.Cleanup();
    }
}
