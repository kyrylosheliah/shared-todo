using System.Net.WebSockets;
using System.Text;

namespace TodoClientWpf.Services;

public class TaskWsService
{
    private ClientWebSocket _webSocket = new();
    private CancellationTokenSource _cts = new();

    public event Action<string> OnMessageReceived = s => { };

    public async Task ConnectAsync(string uri)
    {
        _webSocket = new ClientWebSocket();
        _cts = new CancellationTokenSource();

        try
        {
            await _webSocket.ConnectAsync(new Uri(uri), _cts.Token);
            _ = ReceiveLoopAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WebSocket connection failed: {ex.Message}");
        }
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
            OnMessageReceived?.Invoke(message.ToString());
        }
    }
}
