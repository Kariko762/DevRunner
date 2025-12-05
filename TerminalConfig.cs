namespace DevRunner;

public class TerminalConfig
{
    public string Title { get; set; } = "Terminal";
    public string Directory { get; set; } = "";
    public string RunCommand { get; set; } = "";
    public string BuildCommand { get; set; } = "";
    public string ColorScheme { get; set; } = "Matrix Green";
}

public class TerminalColorScheme
{
    public Color HeaderBackground { get; set; }
    public Color TitleColor { get; set; }
    public Color ContainerBackground { get; set; }
    public Color OutputBackground { get; set; }
    public Color OutputForeground { get; set; }
    public Color ButtonBackground { get; set; }
    public Color AccentColor { get; set; }
}

public static class ColorSchemes
{
    public static readonly Dictionary<string, TerminalColorScheme> Schemes = new()
    {
        { "Matrix Green", new TerminalColorScheme
            {
                HeaderBackground = Color.FromArgb(25, 50, 30),
                TitleColor = Color.FromArgb(100, 255, 150),
                ContainerBackground = Color.FromArgb(18, 35, 22),
                OutputBackground = Color.FromArgb(10, 20, 12),
                OutputForeground = Color.FromArgb(100, 255, 150),
                ButtonBackground = Color.FromArgb(40, 80, 50),
                AccentColor = Color.FromArgb(50, 200, 100)
            }
        },
        { "Ocean Blue", new TerminalColorScheme
            {
                HeaderBackground = Color.FromArgb(20, 35, 50),
                TitleColor = Color.FromArgb(120, 200, 255),
                ContainerBackground = Color.FromArgb(15, 25, 35),
                OutputBackground = Color.FromArgb(8, 18, 28),
                OutputForeground = Color.FromArgb(100, 200, 255),
                ButtonBackground = Color.FromArgb(30, 60, 90),
                AccentColor = Color.FromArgb(40, 150, 255)
            }
        },
        { "Amber Terminal", new TerminalColorScheme
            {
                HeaderBackground = Color.FromArgb(45, 35, 20),
                TitleColor = Color.FromArgb(255, 200, 100),
                ContainerBackground = Color.FromArgb(30, 25, 15),
                OutputBackground = Color.FromArgb(20, 15, 8),
                OutputForeground = Color.FromArgb(255, 180, 50),
                ButtonBackground = Color.FromArgb(70, 55, 30),
                AccentColor = Color.FromArgb(255, 160, 40)
            }
        },
        { "Purple Haze", new TerminalColorScheme
            {
                HeaderBackground = Color.FromArgb(40, 25, 50),
                TitleColor = Color.FromArgb(200, 150, 255),
                ContainerBackground = Color.FromArgb(28, 18, 35),
                OutputBackground = Color.FromArgb(18, 10, 25),
                OutputForeground = Color.FromArgb(200, 150, 255),
                ButtonBackground = Color.FromArgb(60, 40, 80),
                AccentColor = Color.FromArgb(180, 100, 255)
            }
        },
        { "Ruby Red", new TerminalColorScheme
            {
                HeaderBackground = Color.FromArgb(50, 20, 20),
                TitleColor = Color.FromArgb(255, 140, 140),
                ContainerBackground = Color.FromArgb(35, 15, 15),
                OutputBackground = Color.FromArgb(25, 8, 8),
                OutputForeground = Color.FromArgb(255, 120, 120),
                ButtonBackground = Color.FromArgb(80, 30, 30),
                AccentColor = Color.FromArgb(255, 80, 80)
            }
        },
        { "Cyberpunk Pink", new TerminalColorScheme
            {
                HeaderBackground = Color.FromArgb(40, 15, 40),
                TitleColor = Color.FromArgb(255, 100, 220),
                ContainerBackground = Color.FromArgb(28, 10, 28),
                OutputBackground = Color.FromArgb(18, 5, 20),
                OutputForeground = Color.FromArgb(255, 100, 200),
                ButtonBackground = Color.FromArgb(70, 25, 70),
                AccentColor = Color.FromArgb(255, 50, 180)
            }
        },
        { "Arctic Frost", new TerminalColorScheme
            {
                HeaderBackground = Color.FromArgb(25, 40, 45),
                TitleColor = Color.FromArgb(150, 230, 255),
                ContainerBackground = Color.FromArgb(18, 28, 32),
                OutputBackground = Color.FromArgb(10, 20, 24),
                OutputForeground = Color.FromArgb(150, 220, 240),
                ButtonBackground = Color.FromArgb(35, 65, 75),
                AccentColor = Color.FromArgb(100, 200, 240)
            }
        },
        { "Golden Sunset", new TerminalColorScheme
            {
                HeaderBackground = Color.FromArgb(50, 40, 20),
                TitleColor = Color.FromArgb(255, 215, 120),
                ContainerBackground = Color.FromArgb(35, 28, 15),
                OutputBackground = Color.FromArgb(25, 20, 10),
                OutputForeground = Color.FromArgb(255, 200, 100),
                ButtonBackground = Color.FromArgb(75, 60, 30),
                AccentColor = Color.FromArgb(255, 180, 60)
            }
        }
    };
}
