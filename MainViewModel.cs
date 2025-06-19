using Microsoft.UI.Xaml;
using System.ComponentModel;
using System;

public class MainViewModel : INotifyPropertyChanged
{
    private string _currentTime;
    private DispatcherTimer _timer;

    public string CurrentTime
    {
        get => _currentTime;
        set
        {
            if (_currentTime != value)
            {
                _currentTime = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentTime)));
            }
        }
    }

    public MainViewModel()
    {
        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += (s, e) => CurrentTime = DateTime.Now.ToString("dd/MM/yyyy - HH:mm:ss");
        _timer.Start();

        CurrentTime = DateTime.Now.ToString("dd/MM/yyyy - HH:mm:ss");
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
