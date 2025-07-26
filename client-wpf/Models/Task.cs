using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace TodoClientWpf.Models;

public class TaskDto : INotifyPropertyChanged
{
    public int Id { get; set; } = 0;

    private string? _description;
    public string? Description
    {
        get => _description;
        set
        {
            if (_description != value)
            {
                _description = value;
                OnPropertyChanged();
            }
        }
    }

    private string? _status;
    public string? Status
    {
        get => _status;
        set
        {
            if (_status != value)
            {
                _status = value;
                OnPropertyChanged();
            }
        }
    }

    [JsonIgnore]
    private bool _isCompleted;
    [JsonIgnore]
    public bool IsCompleted
    {
        get => _isCompleted;
        set
        {
            if (_isCompleted != value)
            {
                _isCompleted = value;
                OnPropertyChanged();
            }
        }
    }

    [JsonIgnore]
    private bool _isPendingDone;
    [JsonIgnore]
    public bool IsPendingDone
    {
        get => _isPendingDone;
        set
        {
            if (_isPendingDone != value)
            {
                _isPendingDone = value;
                OnPropertyChanged();
            }
        }
    }

    [JsonIgnore]
    private double _completionProgress;
    [JsonIgnore]
    public double CompletionProgress
    {
        get => _completionProgress;
        set
        {
            if (Math.Abs(_completionProgress - value) > 0.1)
            {
                _completionProgress = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
}
