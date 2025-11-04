# DevRunner - Dev Environment Manager

A simple Windows Forms application to manage Frontend and Backend development environments during testing.

## Features

- **Split Screen Interface**: Frontend on the left, Backend on the right
- **Configurable Commands**: Set custom run and build commands for each environment
- **Directory Selection**: Browse and select working directories for each environment
- **Parallel Execution**: Run both environments simultaneously in separate processes
- **Terminal Output**: Real-time output from PowerShell commands
- **Persistent Settings**: Automatically saves your commands and directories between sessions
- **Process Control**: Start, stop, and monitor your development processes
- **Run Both**: Orchestrate both environments - starts Backend, waits 4 seconds, then starts Frontend
- **Confirm Actions**: Safety confirmation for Run, Build, and Stop operations

## How to Use

1. **Set Working Directories**: Click "Browse..." to select the root directory for your Frontend and Backend projects
2. **Configure Commands**: Enter your run and build commands (e.g., `npm start`, `dotnet run`)
3. **Run Your Environments**: 
   - Click Run/Build buttons (confirm on second click) to execute commands
   - Use "Run Both" to start both environments in sequence
   - Use "Stop Both" to terminate all processes
4. **Monitor Output**: Watch real-time output with Process IDs displayed
5. **Control Processes**: Use Stop to terminate running processes, Clear to clean output, or Copy to copy output to clipboard

## Building and Running

### In VS Code:
1. Press `F5` to build and run
2. Or use `Ctrl+Shift+B` to just build

### From Command Line:
```powershell
dotnet build DevRunner.csproj
dotnet run --project DevRunner.csproj
```

## Requirements

- .NET 8.0 SDK
- Windows OS
- PowerShell

## Settings Location

Settings are automatically saved to:
`%APPDATA%\DevRunner\settings.json`

## Screenshots

Modern dark theme with geometric background art, color-coded buttons, and real-time process monitoring.

## License

MIT License - Feel free to use and modify!
