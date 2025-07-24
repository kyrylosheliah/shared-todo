using TodoClientWpf.Services;

namespace TodoClientWpf.ViewModels;

public class TodoViewModel
{
    private readonly TaskWsService _webSocketService = new();

    public TodoViewModel()
    {
        _webSocketService.OnMessageReceived += HandleWebSocketMessage;
    }

    public async Task StartListeningAsync()
    {
        await _webSocketService.ConnectAsync("ws://localhost:5000/ws/tasks");
    }

    private void HandleWebSocketMessage(string message)
    {
        // Example: parse JSON update and refresh task list
        Console.WriteLine("Received update: " + message);

        // Deserialize into TaskDto or use message type switching
        // Update ObservableCollection<TaskDto> accordingly
    }
}
