using System.Net.Http;
using System.Net.Http.Json;
using TodoClientWpf.Models;

namespace TodoClientWpf.Services;

public class TaskCrudService
{
    private readonly HttpClient _httpClient;
    private const string _baseUrl = "http://localhost:5000";

    public TaskCrudService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<List<TaskDto>> GetTasksAsync()
    {
        var res = await _httpClient.GetAsync($"{_baseUrl}/api/tasks");
        if (!res.IsSuccessStatusCode) return [];
        var tasks = await res.Content.ReadFromJsonAsync<List<TaskDto>>();
        return tasks ?? [];
    }

    public async Task<bool> AddTaskAsync(TaskDto task)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/task", task);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateTaskAsync(TaskDto task)
    {
        var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/api/task/{task.Id}", task);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"{_baseUrl}/api/task/{id}");
        return response.IsSuccessStatusCode;
    }
}
