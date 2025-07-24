#pragma once

#include <set>
#include <json/json.h>
#include <drogon/drogon.h>
#include <drogon/WebSocketController.h>
#include "TaskStore.h"

using namespace drogon;

class TodoWebSocket : public WebSocketController<TodoWebSocket> {

private:

    static inline std::set<WebSocketConnectionPtr> _clients;
    static inline std::shared_ptr<TaskStore> _store = nullptr;
    static inline std::mutex _clientsMutex;
    static inline int _version = 1;

public:

    WS_PATH_LIST_BEGIN
        WS_PATH_ADD("/ws/tasks");
    WS_PATH_LIST_END

    static void setTaskStore(
        std::shared_ptr<TaskStore> store
    ) {
        _store = store;
        if (_store){
            _store->setOnChange([] {
                TodoWebSocket::broadcastVersion();
            });
        }
    }

    static void broadcastVersion() {
        if (!_store) return;
        Json::Value jsonVersionObject;
        jsonVersionObject["version"] = _version;
        Json::StreamWriterBuilder writer;
        std::string message = Json::writeString(writer, jsonVersionObject);
        std::lock_guard<std::mutex> lock(_clientsMutex);
        for (const auto& conn : _clients) {
            if (conn->connected()) {
                conn->send(message);
            }
        }
    }

    static void bumpVersion() {
        {
            std::lock_guard<std::mutex> lock(_clientsMutex);
            ++_version; // the overflow is intentional
        }
        TodoWebSocket::broadcastVersion();
    }

    void handleNewMessage(
        const WebSocketConnectionPtr&,
        std::string&&,
        const WebSocketMessageType&
    ) override {}

    void handleNewConnection(
        const HttpRequestPtr& req,
        const WebSocketConnectionPtr& conn
    ) {
        {
            std::lock_guard<std::mutex> lock(_clientsMutex);
            _clients.insert(conn);
        }
        TodoWebSocket::broadcastVersion();
    }

    void handleConnectionClosed(
        const WebSocketConnectionPtr& conn
    ) {
        std::lock_guard<std::mutex> lock(_clientsMutex);
        _clients.erase(conn);
    }

};
