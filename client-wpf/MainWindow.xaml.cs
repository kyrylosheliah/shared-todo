using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TodoClientWpf;

public class TodoItem
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public string Status { get; set; } = "";
    public bool IsCompleted { get; set; }
}

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    static readonly string _baseUrl = "http://localhost:5000";
    private HttpClient _http = new() { BaseAddress = new Uri(_baseUrl) };
    private ObservableCollection<TodoItem> _tasks = [];

    public MainWindow()
    {
        InitializeComponent();
        TodoList.ItemsSource = _tasks;
        LoadTodos();
    }

    private async void LoadTodos()
    {
        try
        {
            var result = await _http.GetFromJsonAsync<TodoItem[]>("/api/tasks");
            _tasks.Clear();
            foreach (var item in result)
            {
                item.IsCompleted = item.Status == "Completed";
                _tasks.Add(item);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error loading todos: " + ex.Message);
        }
    }

    private async void AddTodo_Click(object sender, RoutedEventArgs e)
    {
        string title = TitleInput.Text.Trim();
        if (string.IsNullOrEmpty(title)) return;

        try
        {
            var response = await _http.PostAsJsonAsync("/api/tasks", new { title });
            var newTodo = await response.Content.ReadFromJsonAsync<TodoItem>();
            newTodo.IsCompleted = newTodo.Status == "Completed";
            _tasks.Add(newTodo);
            TitleInput.Text = "";
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error adding todo: " + ex.Message);
        }
    }

    private async void TodoCheckBox_Changed(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox cb && cb.Tag is int id)
        {
            try
            {
                string status = cb.IsChecked == true ? "Completed" : "Pending";
                await _http.PutAsJsonAsync($"/api/tasks/{id}", new { status });

                var todo = _tasks.FirstOrDefault(t => t.Id == id);
                if (todo != null)
                {
                    todo.Status = status;
                    todo.IsCompleted = cb.IsChecked == true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating todo: " + ex.Message);
            }
        }
    }
}