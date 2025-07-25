#pragma once

#include <set>
#include <mutex>
#include <drogon/WebSocketController.h>

using namespace drogon;

class TodoWebSocketController : public WebSocketController<TodoWebSocketController> {
private:
    static inline std::set<WebSocketConnectionPtr> _clients;
    static inline std::mutex _clientsMutex;
    static inline int _version = 1;

public:
    WS_PATH_LIST_BEGIN
        WS_PATH_ADD("/ws/tasks");
    WS_PATH_LIST_END

    TodoWebSocketController();

    static void broadcastVersion();

    static void bumpVersion();

    void handleNewMessage(
        const WebSocketConnectionPtr &,
        std::string &&,
        const WebSocketMessageType &
    ) override;

    void handleNewConnection(
        const HttpRequestPtr &req,
        const WebSocketConnectionPtr &conn
    ) override;

    void handleConnectionClosed(
        const WebSocketConnectionPtr &conn
    ) override;
};
