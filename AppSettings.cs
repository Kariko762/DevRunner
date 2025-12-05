using System.Text.Json;

namespace DevRunner;

public class AppSettings
{
    public List<Profile> Profiles { get; set; } = new List<Profile>();
    public string CurrentProfileName { get; set; } = "Default";
    
    // Legacy properties for backward compatibility
    public string FrontEndDirectory { get; set; } = "";
    public string FrontEndRunCommand { get; set; } = "npm start";
    public string FrontEndBuildCommand { get; set; } = "npm run build";
    
    public string BackEndDirectory { get; set; } = "";
    public string BackEndRunCommand { get; set; } = "dotnet run";
    public string BackEndBuildCommand { get; set; } = "dotnet build";

    private static string SettingsPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "DevRunner",
        "settings.json"
    );

    public static AppSettings Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                var json = File.ReadAllText(SettingsPath);
                var settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                
                // Migrate legacy settings to profiles if needed
                if (settings.Profiles.Count == 0)
                {
                    settings.MigrateLegacySettings();
                }
                
                return settings;
            }
        }
        catch
        {
            // If loading fails, return default settings
        }
        
        var newSettings = new AppSettings();
        newSettings.CreateDefaultProfile();
        return newSettings;
    }

    private void MigrateLegacySettings()
    {
        var defaultProfile = new Profile
        {
            Name = "Default",
            Terminals = new List<TerminalConfig>()
        };

        // Only add terminals if they have configurations
        if (!string.IsNullOrEmpty(FrontEndDirectory) || !string.IsNullOrEmpty(FrontEndRunCommand))
        {
            defaultProfile.Terminals.Add(new TerminalConfig
            {
                Title = "Front End",
                Directory = FrontEndDirectory,
                RunCommand = FrontEndRunCommand,
                BuildCommand = FrontEndBuildCommand
            });
        }

        if (!string.IsNullOrEmpty(BackEndDirectory) || !string.IsNullOrEmpty(BackEndRunCommand))
        {
            defaultProfile.Terminals.Add(new TerminalConfig
            {
                Title = "Back End",
                Directory = BackEndDirectory,
                RunCommand = BackEndRunCommand,
                BuildCommand = BackEndBuildCommand
            });
        }

        // If no terminals were configured, create default structure
        if (defaultProfile.Terminals.Count == 0)
        {
            CreateDefaultTerminals(defaultProfile);
        }

        Profiles.Add(defaultProfile);
        CurrentProfileName = "Default";
    }

    private void CreateDefaultProfile()
    {
        var defaultProfile = new Profile
        {
            Name = "Default",
            Terminals = new List<TerminalConfig>()
        };

        CreateDefaultTerminals(defaultProfile);
        
        Profiles.Add(defaultProfile);
        CurrentProfileName = "Default";
    }

    private void CreateDefaultTerminals(Profile profile)
    {
        profile.Terminals.Add(new TerminalConfig
        {
            Title = "Front End",
            Directory = "",
            RunCommand = "npm start",
            BuildCommand = "npm run build"
        });

        profile.Terminals.Add(new TerminalConfig
        {
            Title = "Back End",
            Directory = "",
            RunCommand = "dotnet run",
            BuildCommand = "dotnet build"
        });
    }

    public Profile? GetCurrentProfile()
    {
        return Profiles.FirstOrDefault(p => p.Name == CurrentProfileName);
    }

    public void Save()
    {
        try
        {
            var directory = Path.GetDirectoryName(SettingsPath);
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsPath, json);
        }
        catch
        {
            // Silently fail if we can't save settings
        }
    }
}
