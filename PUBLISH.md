# Publishing Terminal Guru

## What You Have Now

✅ **Distributable ZIP**: `TerminalGuru-win-x64.zip` (62.63 MB)
- Self-contained (includes .NET runtime)
- Works on any Windows x64 machine without .NET installed
- Extract and run `TerminalGuru.exe`

## Quick Distribution Options

### Option 1: ZIP Distribution (Easiest)
1. Share `TerminalGuru-win-x64.zip`
2. Users extract and run `TerminalGuru.exe`
3. No installation needed

### Option 2: Inno Setup Installer (Professional)
1. Download [Inno Setup](https://jrsoftware.org/isdl.php)
2. Right-click `installer.iss` and choose "Compile"
3. Output: `TerminalGuru-Setup.exe` (professional installer)
4. Features:
   - Installs to Program Files
   - Creates Start Menu shortcuts
   - Optional Desktop icon
   - Uninstall support

To customize the installer, edit `installer.iss`:
- Change `MyAppPublisher` to your name
- Change `MyAppURL` to your website/repo
- Add an icon: set `SetupIconFile=path\to\icon.ico`

## Optional: Code Signing (Recommended for Public Distribution)

If you have a code signing certificate, sign the EXE to avoid Windows SmartScreen warnings:

```powershell
# Example with certificate file
signtool sign /fd SHA256 /a /f "C:\path\to\cert.pfx" /p "password" "publish\win-x64\TerminalGuru.exe"

# Or with certificate from Windows certificate store
signtool sign /fd SHA256 /n "Your Certificate Name" /t http://timestamp.digicert.com "publish\win-x64\TerminalGuru.exe"
```

After signing, re-create the ZIP:
```powershell
Compress-Archive -Path .\publish\win-x64\* -DestinationPath .\TerminalGuru-win-x64.zip -Force
```

## Testing Before Distribution

Test on a clean Windows VM without .NET installed:
1. Copy the ZIP to the VM
2. Extract and run `TerminalGuru.exe`
3. Verify it launches, runs PowerShell commands, and can start/stop processes

## File Locations

- **Published files**: `C:\TerminalGuru\publish\win-x64\`
- **ZIP package**: `C:\TerminalGuru\TerminalGuru-win-x64.zip`
- **Installer script**: `C:\TerminalGuru\installer.iss`
- **Main executable**: `publish\win-x64\TerminalGuru.exe` (154 MB)

## Distribution Checklist

- [x] Build self-contained executable
- [x] Create ZIP package
- [x] Create installer script
- [ ] Sign executable (optional but recommended)
- [ ] Test on clean Windows VM
- [ ] Upload to GitHub Releases / website
- [ ] Write release notes

## Reducing File Size (Optional)

If 62 MB is too large, try:

### Framework-dependent build (smaller, requires .NET on target):
```powershell
dotnet publish -c Release -o .\publish\framework-dependent
```
Result: ~500 KB (but users need .NET 8 installed)

### Trimmed build (risky, test thoroughly):
```powershell
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:PublishTrimmed=true -o .\publish\win-x64-trimmed
```
Result: ~40-50 MB (but may break if reflection is used)

## Next Steps

1. **Test the ZIP** on another machine
2. **Optional**: Build installer with Inno Setup
3. **Optional**: Sign the EXE if distributing publicly
4. **Share**: Upload to GitHub Releases, website, or cloud storage
