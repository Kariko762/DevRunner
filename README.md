# DevRunner - Dev Environment Manager

<p align="center">
  <img src="https://img.shields.io/badge/platform-Windows-blue" alt="Platform">
  <img src="https://img.shields.io/badge/.NET-8.0-purple" alt=".NET 8.0">
  <img src="https://img.shields.io/badge/license-MIT-green" alt="License">
  <img src="https://img.shields.io/github/v/release/Kariko762/DevRunner" alt="Release">
</p>

A sleek, modern Windows Forms application designed to manage Frontend and Backend development environments simultaneously during testing and development.

![DevRunner Screenshot](screenshot.png)

## ✨ Features

### 🎯 Core Functionality
- **Split Screen Interface** - Frontend on the left, Backend on the right with a modern dark theme
- **Parallel Execution** - Run both environments simultaneously in separate PowerShell processes
- **Real-time Output** - Live terminal output with color-coded messages (green=output, orange=errors, cyan=info)
- **Process Management** - Full control with process IDs displayed for monitoring

### 🎨 Modern UI
- Dark theme with geometric background art
- Color-coded buttons for intuitive operation
- Right-aligned process ID display
- Responsive design with proper sizing

### ⚡ Smart Controls
- **Run Both** - Orchestrated startup: Backend → Wait 4 seconds → Frontend
- **Stop Both** - Terminate all processes with one click
- **Confirm Actions** - Safety confirmation for Run, Build, and Stop operations (3-second timeout)
- **Auto-clear** - Terminal output clears automatically on new runs

### 💾 Persistent Settings
- Automatically saves working directories, run commands, and build commands
- Settings stored in `%APPDATA%\DevRunner\settings.json`
- Remembers your configuration between sessions

### 🔧 Process Control
- **Proper Process Termination** - Uses `taskkill /T /F` to kill entire process trees
- **Process ID Monitoring** - Real-time display of running process IDs
- **Copy Output** - Copy terminal output to clipboard
- **Clear Screen** - Clear terminal output
- **Stop Process** - Terminate running processes cleanly

## 🚀 Quick Start

### Prerequisites
- Windows OS
- .NET 8.0 SDK (for building from source)
- PowerShell

### Installation

#### Option 1: Download Release (Recommended)
1. Download the latest `DevRunner-win-x64.zip` from [Releases](https://github.com/Kariko762/DevRunner/releases)
2. Extract the ZIP file
3. Run `DevRunner.exe`

#### Option 2: Build from Source
```powershell
git clone https://github.com/Kariko762/DevRunner.git
cd DevRunner
dotnet build DevRunner.csproj
dotnet run --project DevRunner.csproj
```

## 📖 How to Use

### Basic Setup
1. **Set Working Directories**
   - Click "Browse..." for Frontend and Backend
   - Select the root directory of each project

2. **Configure Commands**
   - **Run Command**: e.g., `npm start`, `dotnet run`, `python app.py`
   - **Build Command**: e.g., `npm run build`, `dotnet build`, `npm install`

### Running Your Environments

#### Individual Run
1. Click **Run** button (green)
2. Click **Confirm** (appears for 3 seconds)
3. Process starts, terminal clears, output displays

#### Run Both (Orchestrated)
1. Click **⚡ Run Both** button
2. Click **Confirm**
3. Backend starts → Waits 4 seconds → Frontend starts

#### Stop Processes
- **Individual Stop**: Click **■ Stop** button → Confirm
- **Stop Both**: Click **⏹ Stop Both** button → Confirm

### Understanding the Interface

- **Green Buttons (▶ Run)** - Execute the run command
- **Blue Buttons (🔨 Build)** - Execute the build command
- **Purple Button (⚡ Run Both)** - Run both environments in sequence
- **Red Buttons (Stop/Stop Both)** - Terminate processes
- **ProcessId: XXXX** - Shows the current running process ID (green when active, gray when stopped)

## 🎨 Color Coding

The terminal output uses colors for easy reading:
- **Cyan** - Command execution and directory info
- **Light Green** - Standard output
- **Orange** - Errors and warnings
- **Yellow** - Process status messages
- **Gray** - General information

## 🔑 Keyboard Shortcuts & Tips

- All confirm buttons auto-reset after 3 seconds if not clicked
- Process IDs update in real-time
- Closing the app automatically terminates all running processes
- Settings are auto-saved on exit

## 🛠️ Building and Development

### VS Code
```powershell
# Build
Ctrl + Shift + B

# Run with debugging
F5
```

### Command Line
```powershell
# Build
dotnet build DevRunner.csproj

# Run
dotnet run --project DevRunner.csproj

# Publish (self-contained)
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true -o .\publish\win-x64
```

## 📁 Project Structure

```
DevRunner/
├── AppSettings.cs          # Persistent settings management
├── MainForm.cs             # Main application form
├── TerminalPanel.cs        # Reusable terminal panel component
├── Program.cs              # Application entry point
├── DevRunner.csproj        # Project file
├── installer.iss           # Inno Setup installer script
└── README.md
```

## 🔧 Configuration File

Settings are stored at: `%APPDATA%\DevRunner\settings.json`

```json
{
  "FrontEndDirectory": "C:\\Projects\\MyApp\\frontend",
  "FrontEndRunCommand": "npm start",
  "FrontEndBuildCommand": "npm run build",
  "BackEndDirectory": "C:\\Projects\\MyApp\\backend",
  "BackEndRunCommand": "dotnet run",
  "BackEndBuildCommand": "dotnet build"
}
```

## 🐛 Troubleshooting

**Process won't stop?**
- DevRunner uses `taskkill /T /F` to forcefully terminate process trees
- Check Task Manager if processes persist

**Terminal output not showing?**
- Ensure PowerShell is available in your PATH
- Check the working directory is valid

**Settings not saving?**
- Verify write permissions to `%APPDATA%\DevRunner\`

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📝 License

MIT License - Feel free to use and modify!

## 🙏 Acknowledgments

Built with:
- .NET 8.0 Windows Forms
- PowerShell integration
- Modern UI design principles

## 📧 Contact

- GitHub: [@Kariko762](https://github.com/Kariko762)
- Repository: [DevRunner](https://github.com/Kariko762/DevRunner)

---

<p align="center">Made with ❤️ for developers who run multiple environments</p>
