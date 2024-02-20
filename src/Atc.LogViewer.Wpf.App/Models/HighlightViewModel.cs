namespace Atc.LogViewer.Wpf.App.Models;

public class HighlightViewModel : ViewModelBase
{
    private string text = string.Empty;
    private bool ignoreCasing;
    private SolidColorBrush foreground = new(Colors.Green);
    private SolidColorBrush background = new(Colors.Transparent);

    public string Text
    {
        get => text;
        set
        {
            if (value == text)
            {
                return;
            }

            text = value;
            RaisePropertyChanged();
        }
    }

    public bool IgnoreCasing
    {
        get => ignoreCasing;
        set
        {
            if (value == ignoreCasing)
            {
                return;
            }

            ignoreCasing = value;
            RaisePropertyChanged();
        }
    }

    public SolidColorBrush Foreground
    {
        get => foreground;
        set
        {
            if (Equals(value, foreground))
            {
                return;
            }

            foreground = value;
            RaisePropertyChanged();
        }
    }

    public SolidColorBrush Background
    {
        get => background;
        set
        {
            if (Equals(value, background))
            {
                return;
            }

            background = value;
            RaisePropertyChanged();
        }
    }

    public override string ToString()
        => $"{nameof(Text)}: {Text}, {nameof(IgnoreCasing)}: {IgnoreCasing}, {nameof(Foreground)}: {Foreground}, {nameof(Background)}: {Background}";
}