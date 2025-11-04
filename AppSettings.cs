using System.Text.Json;

namespace DevRunner;

public class AppSettings
{
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
                return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
        }
        catch
        {
            // If loading fails, return default settings
        }
        return new AppSettings();
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
