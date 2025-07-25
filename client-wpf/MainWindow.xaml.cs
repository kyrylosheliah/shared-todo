using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using TodoClientWpf.Models;
using TodoClientWpf.Services;

namespace TodoClientWpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    static readonly string _baseUrl = "http://localhost:5000";
    private readonly TaskCrudService _crud;
    private readonly TaskWsService _ws;
    private readonly ObservableCollection<TaskDto> _tasks = [];
    private int _version = 0;

    public MainWindow(TaskCrudService crud, TaskWsService ws)
    {
        _crud = crud;
        _ws = ws;
        _ws.OnMessageReceived += TryUpdateOnSocketVersion;
        Task.Run(() => _ws.ConnectAsync($"{_baseUrl}/ws/tasks"));
        InitializeComponent();
        TodoList.ItemsSource = _tasks;
        LoadTasks();
    }

    private void TryUpdateOnSocketVersion(string message)
    {
        if (!int.TryParse(message, out int newVersion)) return;
        if (newVersion == _version) return;
        _version = newVersion;
        LoadTasks();
    }

    private async void LoadTasks()
    {
        try
        {
            var result = await _crud.GetTasksAsync();
            _tasks.Clear();
            foreach (var item in result) _tasks.Add(item);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error loading tasks: " + ex.Message);
        }
    }

    private async void AddTask_Click(object sender, RoutedEventArgs e)
    {
        string title = TitleInput.Text.Trim();
        if (string.IsNullOrEmpty(title)) return;

        try
        {
            var newTask = await _crud.AddTaskAsync(title);
            if (newTask == null) return;
            TitleInput.Text = "";
            MessageBox.Show("New task created: " + newTask.ToString());
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error adding task: " + ex.Message);
        }
    }

    private async void TaskCheckBox_Changed(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox cb && cb.DataContext is TaskDto task)
        {
            try
            {
                string status = cb.IsChecked == true ? "Completed" : "Pending";
                await _crud.UpdateTaskAsync(new TaskDto{ Id = task.Id, Status = status });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating task: " + ex.Message);
            }
        }
    }
}