using System.Diagnostics;
using System.Text;

namespace DevRunner;

public class TerminalPanel : Panel
{
    private TextBox txtRunCommand = null!;
    private TextBox txtBuildCommand = null!;
    private TextBox txtDirectory = null!;
    private Button btnBrowse = null!;
    private Button btnRun = null!;
    private Button btnBuild = null!;
    private Button btnCopy = null!;
    private Button btnStop = null!;
    private Button btnClear = null!;
    private Label lblProcessId = null!;
    private RichTextBox txtOutput = null!;
    private Process? currentProcess;
    private int currentProcessId = 0;
    private StringBuilder outputBuffer = new();
    private bool runConfirmMode = false;
    private bool buildConfirmMode = false;

    public string Title { get; set; }
    public string RunCommand => txtRunCommand.Text;
    public string BuildCommand => txtBuildCommand.Text;
    public string WorkingDirectory => txtDirectory.Text;

    public TerminalPanel(string title)
    {
        Title = title;
        InitializeControls();
    }

    public void ExecuteRunCommand()
    {
        ExecuteCommand(RunCommand);
    }

    public void StopProcess()
    {
        StopCurrentProcess();
    }

    public void ClearOutput()
    {
        if (txtOutput.InvokeRequired)
        {
            txtOutput.Invoke(ClearOutput);
            return;
        }
        txtOutput.Clear();
        outputBuffer.Clear();
    }

    private void InitializeControls()
    {
        this.Dock = DockStyle.Fill;
        this.Padding = new Padding(10, 35, 10, 10); // Added top padding for menu bar
        this.BackColor = Color.FromArgb(30, 30, 35);
        this.Paint += DrawGeometricBackground;

        // Title Label
        var lblTitle = new Label
        {
            Text = Title,
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            Location = new Point(10, 35),
            AutoSize = true,
            ForeColor = Color.FromArgb(100, 180, 255),
            BackColor = Color.Transparent
        };
        this.Controls.Add(lblTitle);

        const int inputWidth = 350;
        const int inputHeight = 28; // Same height as buttons
        const int buttonHeight = 25; // Slightly smaller than textboxes
        const int buttonWidth = 90;

        // Directory section
        var lblDir = new Label
        {
            Text = "Working Directory:",
            Location = new Point(10, 65),
            AutoSize = true,
            ForeColor = Color.FromArgb(200, 200, 200),
            BackColor = Color.Transparent
        };
        this.Controls.Add(lblDir);

        txtDirectory = new TextBox
        {
            Location = new Point(10, 85),
            Width = inputWidth,
            Height = inputHeight,
            PlaceholderText = "Select working directory...",
            BackColor = Color.FromArgb(45, 45, 50),
            ForeColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Font = new Font("Segoe UI", 10F)
        };
        this.Controls.Add(txtDirectory);

        btnBrowse = new Button
        {
            Text = "📁 Browse",
            Location = new Point(inputWidth + 15, 85), // Aligned with textbox
            Width = buttonWidth,
            Height = buttonHeight,
            BackColor = Color.FromArgb(60, 60, 65),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9F)
        };
        btnBrowse.FlatAppearance.BorderColor = Color.FromArgb(80, 80, 85);
        btnBrowse.Click += BtnBrowse_Click;
        this.Controls.Add(btnBrowse);

        // Run Command
        var lblRun = new Label
        {
            Text = "Run Command:",
            Location = new Point(10, 125),
            AutoSize = true,
            ForeColor = Color.FromArgb(200, 200, 200),
            BackColor = Color.Transparent
        };
        this.Controls.Add(lblRun);

        txtRunCommand = new TextBox
        {
            Location = new Point(10, 145),
            Width = inputWidth,
            Height = inputHeight,
            PlaceholderText = "e.g., npm start",
            BackColor = Color.FromArgb(45, 45, 50),
            ForeColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Font = new Font("Segoe UI", 10F)
        };
        this.Controls.Add(txtRunCommand);

        btnRun = new Button
        {
            Text = "▶ Run",
            Location = new Point(inputWidth + 15, 145), // Aligned with textbox
            Width = buttonWidth,
            Height = buttonHeight,
            BackColor = Color.FromArgb(40, 180, 99),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
        };
        btnRun.FlatAppearance.BorderSize = 0;
        btnRun.Click += BtnRun_Click;
        this.Controls.Add(btnRun);

        // Build Command
        var lblBuild = new Label
        {
            Text = "Build Command:",
            Location = new Point(10, 185),
            AutoSize = true,
            ForeColor = Color.FromArgb(200, 200, 200),
            BackColor = Color.Transparent
        };
        this.Controls.Add(lblBuild);

        txtBuildCommand = new TextBox
        {
            Location = new Point(10, 205),
            Width = inputWidth,
            Height = inputHeight,
            PlaceholderText = "e.g., npm run build",
            BackColor = Color.FromArgb(45, 45, 50),
            ForeColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Font = new Font("Segoe UI", 10F)
        };
        this.Controls.Add(txtBuildCommand);

        btnBuild = new Button
        {
            Text = "🔨 Build",
            Location = new Point(inputWidth + 15, 205), // Aligned with textbox
            Width = buttonWidth,
            Height = buttonHeight,
            BackColor = Color.FromArgb(52, 152, 219),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
        };
        btnBuild.FlatAppearance.BorderSize = 0;
        btnBuild.Click += BtnBuild_Click;
        this.Controls.Add(btnBuild);

        // Action buttons
        btnCopy = new Button
        {
            Text = "📋 Copy",
            Location = new Point(10, 235),
            Width = 100,
            Height = buttonHeight,
            BackColor = Color.FromArgb(60, 60, 65),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9F)
        };
        btnCopy.FlatAppearance.BorderColor = Color.FromArgb(80, 80, 85);
        btnCopy.Click += BtnCopy_Click;
        this.Controls.Add(btnCopy);

        btnStop = new Button
        {
            Text = "■ Stop",
            Location = new Point(115, 235),
            Width = 100,
            Height = buttonHeight,
            BackColor = Color.FromArgb(231, 76, 60),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold)
        };
        btnStop.FlatAppearance.BorderSize = 0;
        btnStop.Click += BtnStop_Click;
        this.Controls.Add(btnStop);

        btnClear = new Button
        {
            Text = "🗑 Clear",
            Location = new Point(220, 235),
            Width = 100,
            Height = buttonHeight,
            BackColor = Color.FromArgb(60, 60, 65),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9F)
        };
        btnClear.FlatAppearance.BorderColor = Color.FromArgb(80, 80, 85);
        btnClear.Click += BtnClear_Click;
        this.Controls.Add(btnClear);

        // Process ID Label (right-aligned)
        lblProcessId = new Label
        {
            Text = "ProcessId: -",
            Location = new Point(this.Width - 180, 235),
            Width = 170,
            Height = buttonHeight,
            TextAlign = ContentAlignment.MiddleRight,
            BackColor = Color.Transparent,
            ForeColor = Color.FromArgb(100, 180, 255),
            Font = new Font("Consolas", 9F),
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };
        this.Controls.Add(lblProcessId);

        // Output TextBox
        txtOutput = new RichTextBox
        {
            Location = new Point(10, 275),
            Width = this.Width - 25,
            Height = this.Height - 290,
            ReadOnly = true,
            BackColor = Color.FromArgb(20, 20, 25),
            ForeColor = Color.FromArgb(100, 255, 150),
            Font = new Font("Consolas", 9.5F),
            BorderStyle = BorderStyle.FixedSingle,
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
        };
        this.Controls.Add(txtOutput);

        // Handle resize
        this.Resize += (s, e) =>
        {
            txtOutput.Width = this.Width - 25;
            txtOutput.Height = this.Height - 290;
            lblProcessId.Left = this.Width - 190;
        };
    }

    private void DrawGeometricBackground(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        using var pen = new Pen(Color.FromArgb(15, 100, 180, 255), 1);
        using var brush = new SolidBrush(Color.FromArgb(8, 100, 180, 255));

        // Draw geometric patterns
        var random = new Random(Title.GetHashCode()); // Consistent pattern per panel
        for (int i = 0; i < 12; i++)
        {
            int x = random.Next(0, this.Width);
            int y = random.Next(0, this.Height);
            int size = random.Next(50, 200);

            // Draw circles and lines
            g.DrawEllipse(pen, x, y, size, size);
            
            int x2 = random.Next(0, this.Width);
            int y2 = random.Next(0, this.Height);
            g.DrawLine(pen, x, y, x2, y2);
        }

        // Draw grid
        for (int x = 0; x < this.Width; x += 100)
        {
            g.DrawLine(pen, x, 0, x, this.Height);
        }
        for (int y = 0; y < this.Height; y += 100)
        {
            g.DrawLine(pen, 0, y, this.Width, y);
        }
    }

    private void BtnBrowse_Click(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = $"Select {Title} Working Directory",
            UseDescriptionForTitle = true
        };

        if (!string.IsNullOrEmpty(txtDirectory.Text) && Directory.Exists(txtDirectory.Text))
        {
            dialog.SelectedPath = txtDirectory.Text;
        }

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            txtDirectory.Text = dialog.SelectedPath;
        }
    }

    private void BtnRun_Click(object? sender, EventArgs e)
    {
        if (!runConfirmMode)
        {
            // First click - enter confirm mode
            runConfirmMode = true;
            btnRun.Text = "Confirm";
            btnRun.BackColor = Color.FromArgb(178, 26, 83);
            
            // Reset after 3 seconds if not clicked
            var timer = new System.Windows.Forms.Timer { Interval = 3000 };
            timer.Tick += (s, ev) =>
            {
                ResetRunButton();
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }
        else
        {
            // Second click - execute command
            ResetRunButton();
            
            // Clear the output before running
            ClearOutput();
            
            // Stop current process if running, then execute
            if (currentProcess != null && !currentProcess.HasExited)
            {
                StopCurrentProcess();
                // Small delay to ensure process is fully stopped
                Task.Delay(500).ContinueWith(_ => 
                {
                    Invoke(() => ExecuteCommand(RunCommand));
                });
            }
            else
            {
                ExecuteCommand(RunCommand);
            }
        }
    }

    private void BtnBuild_Click(object? sender, EventArgs e)
    {
        if (!buildConfirmMode)
        {
            // First click - enter confirm mode
            buildConfirmMode = true;
            btnBuild.Text = "Confirm";
            btnBuild.BackColor = Color.FromArgb(178, 26, 83);
            
            // Reset after 3 seconds if not clicked
            var timer = new System.Windows.Forms.Timer { Interval = 3000 };
            timer.Tick += (s, ev) =>
            {
                ResetBuildButton();
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }
        else
        {
            // Second click - execute command
            ResetBuildButton();
            
            // Clear the output before building
            ClearOutput();
            
            // Stop current process if running, then execute
            if (currentProcess != null && !currentProcess.HasExited)
            {
                StopCurrentProcess();
                // Small delay to ensure process is fully stopped
                Task.Delay(500).ContinueWith(_ => 
                {
                    Invoke(() => ExecuteCommand(BuildCommand));
                });
            }
            else
            {
                ExecuteCommand(BuildCommand);
            }
        }
    }

    private void ResetRunButton()
    {
        runConfirmMode = false;
        btnRun.Text = "▶ Run";
        btnRun.BackColor = Color.FromArgb(40, 180, 99);
    }

    private void ResetBuildButton()
    {
        buildConfirmMode = false;
        btnBuild.Text = "🔨 Build";
        btnBuild.BackColor = Color.FromArgb(52, 152, 219);
    }

    private void StopCurrentProcess()
    {
        if (currentProcess != null && !currentProcess.HasExited)
        {
            try
            {
                // Kill the entire process tree - PowerShell and all children
                KillProcessTree(currentProcess.Id);
                AppendOutput("\n>>> Process terminated before starting new command.\n", Color.Yellow);
                currentProcessId = 0;
                UpdateProcessIdLabel();
                System.Threading.Thread.Sleep(500); // Give it a moment to clean up
            }
            catch (Exception ex)
            {
                AppendOutput($"Error stopping process: {ex.Message}\n", Color.Red);
            }
        }
    }

    private void KillProcessTree(int processId)
    {
        try
        {
            // Use taskkill to forcefully kill the process tree
            var killProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "taskkill",
                    Arguments = $"/PID {processId} /T /F",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            killProcess.Start();
            killProcess.WaitForExit(2000); // Wait up to 2 seconds
        }
        catch
        {
            // Fallback to Process.Kill if taskkill fails
            try
            {
                var process = Process.GetProcessById(processId);
                process.Kill(true);
            }
            catch { }
        }
    }

    private void ExecuteCommand(string command)
    {
        if (string.IsNullOrWhiteSpace(command))
        {
            AppendOutput("Error: No command specified.\n", Color.Red);
            return;
        }

        if (string.IsNullOrWhiteSpace(WorkingDirectory) || !Directory.Exists(WorkingDirectory))
        {
            AppendOutput("Error: Invalid working directory.\n", Color.Red);
            return;
        }

        // Safety check: if process is still running, don't start a new one
        if (currentProcess != null && !currentProcess.HasExited)
        {
            AppendOutput("Error: A process is still running. Wait for it to stop first.\n", Color.Orange);
            return;
        }

        try
        {
            AppendOutput($">>> Executing: {command}\n", Color.Cyan);
            AppendOutput($">>> Directory: {WorkingDirectory}\n\n", Color.Gray);

            currentProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -Command \"{command}\"",
                    WorkingDirectory = WorkingDirectory,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            currentProcess.OutputDataReceived += (s, e) =>
            {
                if (e.Data != null)
                {
                    var line = e.Data + "\n";
                    outputBuffer.Append(line);
                    Invoke(() => AppendOutput(line, Color.LightGreen));
                }
            };

            currentProcess.ErrorDataReceived += (s, e) =>
            {
                if (e.Data != null)
                {
                    var line = e.Data + "\n";
                    outputBuffer.Append(line);
                    Invoke(() => AppendOutput(line, Color.Orange));
                }
            };

            currentProcess.EnableRaisingEvents = true;
            currentProcess.Exited += (s, e) =>
            {
                Invoke(() => 
                {
                    AppendOutput($"\n>>> Process exited with code: {currentProcess.ExitCode}\n", Color.Yellow);
                    UpdateProcessIdLabel();
                });
            };

            currentProcess.Start();
            currentProcessId = currentProcess.Id;
            UpdateProcessIdLabel();
            currentProcess.BeginOutputReadLine();
            currentProcess.BeginErrorReadLine();
        }
        catch (Exception ex)
        {
            AppendOutput($"Error starting process: {ex.Message}\n", Color.Red);
            currentProcessId = 0;
            UpdateProcessIdLabel();
        }
    }

    private void UpdateProcessIdLabel()
    {
        if (lblProcessId.InvokeRequired)
        {
            lblProcessId.Invoke(UpdateProcessIdLabel);
            return;
        }
        
        if (currentProcessId > 0 && currentProcess != null && !currentProcess.HasExited)
        {
            lblProcessId.Text = $"ProcessId: {currentProcessId}";
            lblProcessId.ForeColor = Color.FromArgb(100, 255, 150);
        }
        else
        {
            lblProcessId.Text = "ProcessId: -";
            lblProcessId.ForeColor = Color.FromArgb(100, 100, 100);
            currentProcessId = 0;
        }
    }

    private void BtnStop_Click(object? sender, EventArgs e)
    {
        if (currentProcess != null && !currentProcess.HasExited)
        {
            try
            {
                // Kill the entire process tree
                KillProcessTree(currentProcess.Id);
                AppendOutput("\n>>> Process terminated by user.\n", Color.Yellow);
                currentProcessId = 0;
                UpdateProcessIdLabel();
            }
            catch (Exception ex)
            {
                AppendOutput($"Error stopping process: {ex.Message}\n", Color.Red);
            }
        }
        else
        {
            AppendOutput("No running process to stop.\n", Color.Gray);
        }
    }

    private void BtnClear_Click(object? sender, EventArgs e)
    {
        txtOutput.Clear();
        outputBuffer.Clear();
        UpdateProcessIdLabel(); // Refresh process ID status
    }

    private void BtnCopy_Click(object? sender, EventArgs e)
    {
        if (outputBuffer.Length > 0)
        {
            Clipboard.SetText(outputBuffer.ToString());
            AppendOutput(">>> Output copied to clipboard.\n", Color.Cyan);
        }
    }

    private void AppendOutput(string text, Color color)
    {
        if (txtOutput.InvokeRequired)
        {
            txtOutput.Invoke(() => AppendOutput(text, color));
            return;
        }

        txtOutput.SelectionStart = txtOutput.TextLength;
        txtOutput.SelectionLength = 0;
        txtOutput.SelectionColor = color;
        txtOutput.AppendText(text);
        txtOutput.SelectionColor = txtOutput.ForeColor;
        txtOutput.ScrollToCaret();
    }

    public void LoadSettings(string directory, string runCmd, string buildCmd)
    {
        txtDirectory.Text = directory;
        txtRunCommand.Text = runCmd;
        txtBuildCommand.Text = buildCmd;
    }

    public void Cleanup()
    {
        if (currentProcess != null && !currentProcess.HasExited)
        {
            try
            {
                KillProcessTree(currentProcess.Id);
            }
            catch { }
        }
    }
}
