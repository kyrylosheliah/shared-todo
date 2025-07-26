using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using TodoClientWpf.Models;
using TodoClientWpf.Services;

namespace TodoClientWpf;

public partial class MainWindow : Window
{
    private readonly TaskCrudService _crud;
    private readonly TaskWsService _ws;
    private readonly ObservableCollection<TaskDto> _tasks = [];
    private int _version = 0;
    private Dictionary<TaskDto, CancellationTokenSource> _pendingTasks = [];
    private readonly int debounceDurationMs = 3000;
    private readonly int debounceIntervalMs = 30;

    public MainWindow(TaskCrudService crud, TaskWsService ws)
    {
        _crud = crud;
        _ws = ws;
        _ws.OnMessageReceived.Add(TryUpdateOnSocketVersion);
        Task.Run(() => _ws.ConnectAsync());
        InitializeComponent();
        TodoList.ItemsSource = _tasks;
        LoadTasks();
    }

    private void TryUpdateOnSocketVersion(string message)
    {
        Debug.WriteLine(message);
        try
        {
            using JsonDocument doc = JsonDocument.Parse(message);
            JsonElement root = doc.RootElement;

            if (root.TryGetProperty("version", out JsonElement versionElement)
                && versionElement.ValueKind == JsonValueKind.Number
                && versionElement.TryGetInt32(out int newVersion))
            {
                if (newVersion == _version) return;
                _version = newVersion;
                Dispatcher.Invoke(LoadTasks);
            }
            else
            {
                Debug.WriteLine("Socket version: invalid 'version' field type");
                return;
            }
        }
        catch (JsonException)
        {
            Debug.WriteLine("Socket version: invalid JSON format");
        }
    }

    private async void LoadTasks()
    {
        try
        {
            var result = await _crud.GetTasksAsync();
            //foreach (var cts in _pendingTasks.Values) cts.Cancel();
            var resultIds = result.Select(r => r.Id).ToHashSet();
            // filter the task list
            for (int i = _tasks.Count - 1; i >= 0; --i)
            {
                if (!resultIds.Contains(_tasks[i].Id))
                    _tasks.RemoveAt(i);
            }
            // filter the cancellation token list
            _pendingTasks = _pendingTasks.Where(kvp => {
                var pending = kvp.Key;
                bool keep = resultIds.Contains(pending.Id);
                if (!keep)
                {
                    kvp.Value.Cancel();
                }
                return keep;
            }).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            // update existing values
            foreach (var refetched in result)
            {
                var existing = _tasks.FirstOrDefault(t => t.Id == refetched.Id);
                if (existing != null)
                {
                    existing.Description = refetched.Description;
                    existing.Status = refetched.Status;
                    existing.IsCompleted = refetched.Status != null && refetched.Status.Equals("completed", StringComparison.CurrentCultureIgnoreCase);
                }
                else
                {
                    _tasks.Add(refetched);
                }
            }
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
            MessageBox.Show($"New task created, id {newTask.Id}");
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
            string status = cb.IsChecked == true ? "Completed" : "Pending";
            TaskDto fields = new() { Id = task.Id, Status = status };
            await _crud.UpdateTaskAsync(fields);
            try
            {
                if (cb.IsChecked == true)
                {
                    if (_pendingTasks.ContainsKey(task)) return;
                    task.IsPendingDone = true;
                    var cts = new CancellationTokenSource();
                    _pendingTasks[task] = cts;
                    try
                    {
                        for (int i = 0; i <= debounceIntervalMs; i++)
                        {
                            task.CompletionProgress = 100 - i * 100.0 / debounceIntervalMs;
                            await Task.Delay(debounceDurationMs / debounceIntervalMs, cts.Token);
                        }
                        task.IsPendingDone = false;
                        _pendingTasks.Remove(task);
                        await _crud.DeleteTaskAsync(task.Id);
                    }
                    catch (TaskCanceledException)
                    {
                        task.IsPendingDone = false;
                        task.CompletionProgress = 0;
                    }
                }
                else
                {
                    if (_pendingTasks.TryGetValue(task, out var cts))
                    {
                        cts.Cancel();
                        _pendingTasks.Remove(task);
                    }
                    task.IsPendingDone = false;
                    task.CompletionProgress = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating task: " + ex.Message);
            }
        }
    }
}