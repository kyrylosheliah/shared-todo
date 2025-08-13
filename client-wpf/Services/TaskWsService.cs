using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;

namespace TodoClientWpf.Services;

public class TaskWsService
{
    private ClientWebSocket _webSocket = new();
    private CancellationTokenSource _cts = new();
    static readonly string _baseUrl = "ws://localhost:5000/ws/tasks";

    public event Action<string> OnMessageReceived;

    public async Task ConnectAsync()
    {
        _webSocket = new ClientWebSocket();
        _cts = new CancellationTokenSource();

        await _webSocket.ConnectAsync(new Uri(_baseUrl), _cts.Token);
        Debug.WriteLine($"WebSocket start");
        await ReceiveLoopAsync();
        Debug.WriteLine($"WebSocket end");
    }

    public async Task DisconnectAsync()
    {
        if (_webSocket != null && _webSocket.State == WebSocketState.Open)
        {
            _cts.Cancel();
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnecting", CancellationToken.None);
            _webSocket.Dispose();
        }
    }

    private async Task ReceiveLoopAsync()
    {
        var buffer = new byte[4096];
        while (!_cts.IsCancellationRequested)
        {
            Debug.WriteLine("receiving socket message");
            WebSocketReceiveResult result;
            var message = new StringBuilder();
            do
            {
                result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", _cts.Token);
                    return;
                }
                var chunk = Encoding.UTF8.GetString(buffer, 0, result.Count);
                message.Append(chunk);
            } while (!result.EndOfMessage);
            var m = message.ToString();
            OnMessageReceived?.Invoke(m);
        }
    }
}
