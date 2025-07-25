using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TodoClientWpf.Models;
using static System.Net.WebRequestMethods;

namespace TodoClientWpf.Services;

public class TaskCrudService {
    
    static readonly string _baseUrl = "http://localhost:5000";
    private readonly HttpClient _http;

    public TaskCrudService(HttpClient http, TaskWsService ws)
    {
        _http = http;
        _http.BaseAddress = new Uri(_baseUrl);
    }

    public async Task<List<TaskDto>> GetTasksAsync()
    {
        var res = await _http.GetAsync("/api/tasks");
        if (!res.IsSuccessStatusCode) return [];
        var tasks = await res.Content.ReadFromJsonAsync<List<TaskDto>>();
        if (tasks == null) return [];
        foreach (var item in tasks)
        {
            item.IsCompleted = item.Status.ToLower() == "completed";
            tasks.Add(item);
        }
        return tasks;
    }

    public async Task<TaskDto?> AddTaskAsync(string description)
    {
        var response = await _http.PostAsJsonAsync("/api/task", new { description });
        if (!response.IsSuccessStatusCode) return null;
        var newTask = await response.Content.ReadFromJsonAsync<TaskDto>();
        return newTask;
    }

    public async Task<string?> UpdateTaskAsync(TaskDto task)
    {
        var response = await _http.PutAsJsonAsync($"/api/task", task);
        if (!response.IsSuccessStatusCode) return response.Content.ToString();
        return null;
    }

    public async Task<string?> DeleteTaskAsync(int id)
    {
        var response = await _http.DeleteAsync($"/api/task/{id}");
        if (!response.IsSuccessStatusCode) return response.Content.ToString();
        return null;
    }
}
