namespace DevRunner;

public class Profile
{
    public string Name { get; set; } = "Default";
    public List<TerminalConfig> Terminals { get; set; } = new List<TerminalConfig>();
}
