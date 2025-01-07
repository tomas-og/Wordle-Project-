using System.ComponentModel;

namespace Wordle_Project_;

public class ThemeManager : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private static ThemeManager instance;
    public static ThemeManager Current => instance ??= new ThemeManager();

    public static readonly Color DarkBackground = Color.FromArgb("#252525");
    public static readonly Color DarkText = Color.FromArgb("#eeeeee");
    public static readonly Color DarkKeyboard = Color.FromArgb("#9a9a9a");
    public static readonly Color DarkButton = Color.FromArgb("#333333");

    public static readonly Color LightBackground = Color.FromArgb("#ffffff");
    public static readonly Color LightText = Color.FromArgb("#252525");
    public static readonly Color LightKeyboard = Color.FromArgb("#d3d6da");
    public static readonly Color LightButton = Color.FromArgb("#252525");

    private Color textColor = DarkText;
    public Color TextColor
    {
        get => textColor;
        set
        {
            textColor = value;
            OnPropertyChanged();
        }
    }

    protected virtual void OnPropertyChanged(string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private Color backgroundColor = DarkBackground;
    public Color BackgroundColor
    {
        get => backgroundColor;
        set
        {
            backgroundColor = value;
            OnPropertyChanged();
        }
    }

    private Color keyboardColor = DarkKeyboard;
    public Color KeyboardColor
    {
        get => keyboardColor;
        set
        {
            keyboardColor = value;
            OnPropertyChanged();
        }
    }

    public static bool IsDarkMode { get; set; } = true;

    public static void ToggleTheme()
    {
        IsDarkMode = !IsDarkMode;
        ApplyTheme();
    }

    public static void ApplyTheme()
    {
        if (IsDarkMode)
        {
            Application.Current.UserAppTheme = AppTheme.Dark;
            Current.TextColor = DarkText;
            Current.BackgroundColor = DarkBackground;
            Current.KeyboardColor = DarkKeyboard;
        }
        else
        {
            Application.Current.UserAppTheme = AppTheme.Light;
            Current.TextColor = LightText;
            Current.BackgroundColor = LightBackground;
            Current.KeyboardColor = LightKeyboard;
        }
    }
}