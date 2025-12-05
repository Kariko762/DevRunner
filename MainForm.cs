namespace DevRunner;

public class MainForm : Form
{
    private List<CollapsibleTerminalPanel> terminalPanels = new List<CollapsibleTerminalPanel>();
    private AppSettings settings = null!;
    private MenuStrip menuStrip = null!;
    private ToolStripComboBox profileSelector = null!;
    private FlowLayoutPanel terminalContainer = null!;
    private Button btnRunAll = null!;
    private Button btnStopAll = null!;
    private bool runAllConfirmMode = false;
    private bool stopAllConfirmMode = false;

    public MainForm()
    {
        InitializeComponent();
        LoadSettings();
        LoadCurrentProfile();
    }

    private void InitializeComponent()
    {
        this.Text = "DevRunner - Dev Environment Manager";
        this.Size = new Size(1400, 800);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormClosing += MainForm_FormClosing;
        this.BackColor = Color.FromArgb(30, 30, 35);

        // Create scrollable container for terminal rows FIRST (so it fills remaining space)
        terminalContainer = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            AutoScroll = true,
            WrapContents = false,
            BackColor = Color.FromArgb(30, 30, 35),
            Padding = new Padding(5)
        };
        terminalContainer.Resize += TerminalContainer_Resize;
        this.Controls.Add(terminalContainer);

        // Create toolbar panel for Run All/Stop All buttons
        var toolbarPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 50,
            BackColor = Color.FromArgb(50, 50, 55),
            Padding = new Padding(10, 10, 10, 10)
        };

        btnRunAll = new Button
        {
            Text = "⚡ Run All",
            Location = new Point(10, 10),
            Width = 110,
            Height = 28,
            BackColor = Color.FromArgb(155, 89, 182),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
        };
        btnRunAll.FlatAppearance.BorderSize = 0;
        btnRunAll.Click += BtnRunAll_Click;
        toolbarPanel.Controls.Add(btnRunAll);

        btnStopAll = new Button
        {
            Text = "⏹ Stop All",
            Location = new Point(130, 10),
            Width = 110,
            Height = 28,
            BackColor = Color.FromArgb(231, 76, 60),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
        };
        btnStopAll.FlatAppearance.BorderSize = 0;
        btnStopAll.Click += BtnStopAll_Click;
        toolbarPanel.Controls.Add(btnStopAll);

        // Add Terminal button
        var btnAddTerminal = new Button
        {
            Text = "+ Add Terminal",
            Location = new Point(260, 10),
            Width = 120,
            Height = 28,
            BackColor = Color.FromArgb(46, 204, 113),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
        };
        btnAddTerminal.FlatAppearance.BorderSize = 0;
        btnAddTerminal.Click += BtnAddTerminal_Click;
        toolbarPanel.Controls.Add(btnAddTerminal);

        // Remove Terminal button
        var btnRemoveTerminal = new Button
        {
            Text = "− Remove Last",
            Location = new Point(390, 10),
            Width = 120,
            Height = 28,
            BackColor = Color.FromArgb(230, 126, 34),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
        };
        btnRemoveTerminal.FlatAppearance.BorderSize = 0;
        btnRemoveTerminal.Click += BtnRemoveTerminal_Click;
        toolbarPanel.Controls.Add(btnRemoveTerminal);

        this.Controls.Add(toolbarPanel);

        // Create menu bar LAST (so it docks on top of everything)
        CreateMenuBar();
    }

    private void BtnStopAll_Click(object? sender, EventArgs e)
    {
        if (!stopAllConfirmMode)
        {
            // First click - enter confirm mode
            stopAllConfirmMode = true;
            btnStopAll.Text = "Confirm";
            btnStopAll.BackColor = Color.FromArgb(178, 26, 83);
            
            // Reset after 3 seconds if not clicked
            var timer = new System.Windows.Forms.Timer { Interval = 3000 };
            timer.Tick += (s, ev) =>
            {
                ResetStopAllButton();
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }
        else
        {
            // Second click - stop all
            ResetStopAllButton();
            foreach (var panel in terminalPanels)
            {
                panel.StopProcess();
            }
        }
    }

    private void ResetStopAllButton()
    {
        stopAllConfirmMode = false;
        btnStopAll.Text = "⏹ Stop All";
        btnStopAll.BackColor = Color.FromArgb(231, 76, 60);
    }

    private async void BtnRunAll_Click(object? sender, EventArgs e)
    {
        if (!runAllConfirmMode)
        {
            // First click - enter confirm mode
            runAllConfirmMode = true;
            btnRunAll.Text = "Confirm";
            btnRunAll.BackColor = Color.FromArgb(178, 26, 83);
            
            // Reset after 3 seconds if not clicked
            var timer = new System.Windows.Forms.Timer { Interval = 3000 };
            timer.Tick += (s, ev) =>
            {
                ResetRunAllButton();
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }
        else
        {
            // Second click - execute all
            ResetRunAllButton();
            btnRunAll.Enabled = false;
            btnRunAll.Text = "Running...";
            
            // Clear and stop all terminals
            foreach (var panel in terminalPanels)
            {
                panel.ClearOutput();
                panel.StopProcess();
            }
            
            await Task.Delay(500); // Brief pause after stopping
            
            // Run each terminal in sequence with delays
            for (int i = 0; i < terminalPanels.Count; i++)
            {
                terminalPanels[i].ExecuteRunCommand();
                
                // Wait between terminals (except after the last one)
                if (i < terminalPanels.Count - 1)
                {
                    await Task.Delay(4000);
                }
            }
            
            btnRunAll.Enabled = true;
            btnRunAll.Text = "⚡ Run All";
        }
    }

    private void ResetRunAllButton()
    {
        runAllConfirmMode = false;
        btnRunAll.Text = "⚡ Run All";
        btnRunAll.BackColor = Color.FromArgb(155, 89, 182);
    }

    private void CreateMenuBar()
    {
        menuStrip = new MenuStrip
        {
            BackColor = Color.FromArgb(60, 60, 70),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10F),
            Dock = DockStyle.Top
        };

        // File Menu
        var fileMenu = new ToolStripMenuItem("File");
        fileMenu.DropDownItems.Add("New Profile", null, NewProfile_Click);
        fileMenu.DropDownItems.Add("Save Profile", null, SaveProfile_Click);
        fileMenu.DropDownItems.Add("Save As New Profile", null, SaveAsProfile_Click);
        fileMenu.DropDownItems.Add("Delete Profile", null, DeleteProfile_Click);
        fileMenu.DropDownItems.Add(new ToolStripSeparator());
        fileMenu.DropDownItems.Add("Exit", null, (s, e) => Application.Exit());

        // Profile selector in menu bar
        profileSelector = new ToolStripComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Width = 200,
            BackColor = Color.FromArgb(45, 45, 50),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        profileSelector.SelectedIndexChanged += ProfileSelector_Changed;

        // Help Menu
        var helpMenu = new ToolStripMenuItem("Help");
        helpMenu.DropDownItems.Add("About", null, (s, e) => 
            MessageBox.Show("DevRunner v2.0\nDev Environment Manager\nNow with Profile Support!", "About"));

        menuStrip.Items.Add(fileMenu);
        menuStrip.Items.Add(new ToolStripLabel("Profile:") { ForeColor = Color.White });
        menuStrip.Items.Add(profileSelector);
        menuStrip.Items.Add(helpMenu);

        this.MainMenuStrip = menuStrip;
        this.Controls.Add(menuStrip);
    }

    private void NewProfile_Click(object? sender, EventArgs e)
    {
        var dialog = new InputDialog("New Profile", "Enter profile name:");
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            var profileName = dialog.InputValue;
            if (string.IsNullOrWhiteSpace(profileName))
            {
                MessageBox.Show("Profile name cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (settings.Profiles.Any(p => p.Name == profileName))
            {
                MessageBox.Show("A profile with this name already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var newProfile = new Profile
            {
                Name = profileName,
                Terminals = new List<TerminalConfig>
                {
                    new TerminalConfig { Title = "Terminal 1", Directory = "", RunCommand = "", BuildCommand = "" }
                }
            };

            settings.Profiles.Add(newProfile);
            settings.CurrentProfileName = profileName;
            settings.Save();
            
            RefreshProfileSelector();
            LoadCurrentProfile();
        }
    }

    private void SaveProfile_Click(object? sender, EventArgs e)
    {
        SaveCurrentProfile();
        MessageBox.Show("Profile saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void SaveAsProfile_Click(object? sender, EventArgs e)
    {
        // Ask for new profile name
        InputDialog inputDialog = new InputDialog("Enter new profile name:", "Save As New Profile", "");
        if (inputDialog.ShowDialog() == DialogResult.OK)
        {
            string newProfileName = inputDialog.InputValue.Trim();
            
            if (string.IsNullOrEmpty(newProfileName))
            {
                MessageBox.Show("Profile name cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            if (settings.Profiles.Any(p => p.Name == newProfileName))
            {
                MessageBox.Show("A profile with this name already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            // Get current profile and create a deep copy
            var currentProfile = settings.GetCurrentProfile();
            var newProfile = new Profile
            {
                Name = newProfileName,
                Terminals = currentProfile.Terminals.Select(t => new TerminalConfig
                {
                    Title = t.Title,
                    Directory = t.Directory,
                    RunCommand = t.RunCommand,
                    BuildCommand = t.BuildCommand,
                    ColorScheme = t.ColorScheme
                }).ToList()
            };
            
            // Add new profile and switch to it
            settings.Profiles.Add(newProfile);
            settings.CurrentProfileName = newProfileName;
            settings.Save();
            
            RefreshProfileSelector();
            
            MessageBox.Show($"Profile '{newProfileName}' created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void DeleteProfile_Click(object? sender, EventArgs e)
    {
        if (settings.Profiles.Count <= 1)
        {
            MessageBox.Show("Cannot delete the last profile.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var result = MessageBox.Show($"Delete profile '{settings.CurrentProfileName}'?", "Confirm Delete", 
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        
        if (result == DialogResult.Yes)
        {
            settings.Profiles.RemoveAll(p => p.Name == settings.CurrentProfileName);
            settings.CurrentProfileName = settings.Profiles[0].Name;
            settings.Save();
            
            RefreshProfileSelector();
            LoadCurrentProfile();
        }
    }

    private void ProfileSelector_Changed(object? sender, EventArgs e)
    {
        if (profileSelector.SelectedItem != null)
        {
            var selectedProfile = profileSelector.SelectedItem.ToString();
            if (selectedProfile != settings.CurrentProfileName)
            {
                settings.CurrentProfileName = selectedProfile!;
                LoadCurrentProfile();
            }
        }
    }

    private void RefreshProfileSelector()
    {
        profileSelector.Items.Clear();
        foreach (var profile in settings.Profiles)
        {
            profileSelector.Items.Add(profile.Name);
        }
        profileSelector.SelectedItem = settings.CurrentProfileName;
    }

    private void LoadSettings()
    {
        settings = AppSettings.Load();
        RefreshProfileSelector();
    }

    private void LoadCurrentProfile()
    {
        // Clear existing terminals
        foreach (var panel in terminalPanels)
        {
            panel.Cleanup();
        }
        terminalPanels.Clear();
        terminalContainer.Controls.Clear();

        var currentProfile = settings.GetCurrentProfile();
        if (currentProfile == null || currentProfile.Terminals.Count == 0)
        {
            // Create default profile if none exists
            if (currentProfile == null)
            {
                currentProfile = new Profile { Name = settings.CurrentProfileName };
                settings.Profiles.Add(currentProfile);
            }
            
            currentProfile.Terminals.Add(new TerminalConfig 
            { 
                Title = "Terminal 1", 
                Directory = "", 
                RunCommand = "", 
                BuildCommand = "" 
            });
        }

        // Create terminal panels dynamically
        foreach (var terminalConfig in currentProfile.Terminals)
        {
            var panel = new CollapsibleTerminalPanel(terminalConfig.Title);
            panel.Width = terminalContainer.Width - 25;
            
            panel.LoadSettings(
                terminalConfig.Directory,
                terminalConfig.RunCommand,
                terminalConfig.BuildCommand,
                terminalConfig.ColorScheme
            );

            // Wire up event handlers
            panel.RenameRequested += Panel_RenameRequested;
            panel.MoveUpRequested += Panel_MoveUpRequested;
            panel.MoveDownRequested += Panel_MoveDownRequested;
            panel.DeleteRequested += Panel_DeleteRequested;

            terminalPanels.Add(panel);
            terminalContainer.Controls.Add(panel);
        }

        this.Text = $"DevRunner - {settings.CurrentProfileName}";
    }

    private void SaveCurrentProfile()
    {
        var currentProfile = settings.GetCurrentProfile();
        if (currentProfile != null)
        {
            currentProfile.Terminals.Clear();
            
            foreach (var panel in terminalPanels)
            {
                currentProfile.Terminals.Add(new TerminalConfig
                {
                    Title = panel.Title,
                    Directory = panel.WorkingDirectory,
                    RunCommand = panel.RunCommand,
                    BuildCommand = panel.BuildCommand,
                    ColorScheme = panel.ColorScheme
                });
            }

            settings.Save();
        }
    }

    private void BtnAddTerminal_Click(object? sender, EventArgs e)
    {
        var dialog = new TerminalConfigDialog("Add Terminal");
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            var terminalName = dialog.TerminalName;
            if (string.IsNullOrWhiteSpace(terminalName))
            {
                terminalName = $"Terminal {terminalPanels.Count + 1}";
            }

            var newPanel = new CollapsibleTerminalPanel(terminalName);
            newPanel.Width = terminalContainer.Width - 25;
            
            newPanel.LoadSettings("", "", "", dialog.ColorScheme);
            
            // Wire up event handlers
            newPanel.RenameRequested += Panel_RenameRequested;
            newPanel.MoveUpRequested += Panel_MoveUpRequested;
            newPanel.MoveDownRequested += Panel_MoveDownRequested;
            newPanel.DeleteRequested += Panel_DeleteRequested;
            
            terminalPanels.Add(newPanel);
            terminalContainer.Controls.Add(newPanel);
            
            SaveCurrentProfile();
        }
    }

    private void BtnRemoveTerminal_Click(object? sender, EventArgs e)
    {
        if (terminalPanels.Count <= 1)
        {
            MessageBox.Show("Cannot remove the last terminal.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var result = MessageBox.Show($"Remove terminal '{terminalPanels.Last().Title}'?", "Confirm Remove", 
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        
        if (result == DialogResult.Yes)
        {
            var lastPanel = terminalPanels.Last();
            lastPanel.Cleanup();
            terminalPanels.Remove(lastPanel);
            terminalContainer.Controls.Remove(lastPanel);
            
            SaveCurrentProfile();
        }
    }

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        // Save current profile settings
        SaveCurrentProfile();

        // Clean up processes
        foreach (var panel in terminalPanels)
        {
            panel.Cleanup();
        }
    }

    private void TerminalContainer_Resize(object? sender, EventArgs e)
    {
        // Resize all terminal panels to match container width
        foreach (var panel in terminalPanels)
        {
            panel.Width = terminalContainer.Width - 25;
        }
    }

    private void Panel_RenameRequested(object? sender, EventArgs e)
    {
        if (sender is CollapsibleTerminalPanel panel)
        {
            var dialog = new TerminalConfigDialog("Edit Terminal", panel.Title, panel.ColorScheme);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var newName = dialog.TerminalName;
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    panel.UpdateTitle(newName);
                }
                
                // Apply new color scheme
                panel.LoadSettings(panel.WorkingDirectory, panel.RunCommand, panel.BuildCommand, dialog.ColorScheme);
                
                SaveCurrentProfile();
            }
        }
    }

    private void Panel_MoveUpRequested(object? sender, EventArgs e)
    {
        if (sender is CollapsibleTerminalPanel panel)
        {
            int index = terminalPanels.IndexOf(panel);
            if (index > 0)
            {
                // Swap in list
                terminalPanels.RemoveAt(index);
                terminalPanels.Insert(index - 1, panel);

                // Swap in UI
                terminalContainer.SuspendLayout();
                terminalContainer.Controls.SetChildIndex(panel, index - 1);
                terminalContainer.ResumeLayout(true);

                SaveCurrentProfile();
            }
        }
    }

    private void Panel_MoveDownRequested(object? sender, EventArgs e)
    {
        if (sender is CollapsibleTerminalPanel panel)
        {
            int index = terminalPanels.IndexOf(panel);
            if (index < terminalPanels.Count - 1)
            {
                // Swap in list
                terminalPanels.RemoveAt(index);
                terminalPanels.Insert(index + 1, panel);

                // Swap in UI
                terminalContainer.SuspendLayout();
                terminalContainer.Controls.SetChildIndex(panel, index + 1);
                terminalContainer.ResumeLayout(true);

                SaveCurrentProfile();
            }
        }
    }

    private void Panel_DeleteRequested(object? sender, EventArgs e)
    {
        if (sender is CollapsibleTerminalPanel panel)
        {
            if (terminalPanels.Count <= 1)
            {
                MessageBox.Show("Cannot delete the last terminal.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show($"Delete terminal '{panel.Title}'?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                panel.Cleanup();
                terminalPanels.Remove(panel);
                terminalContainer.Controls.Remove(panel);

                SaveCurrentProfile();
            }
        }
    }
}
