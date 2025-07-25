#include "TodoWebSocket.h"
#include <drogon/drogon.h>
#include <json/json.h>

using namespace drogon;

TodoWebSocket::TodoWebSocket() {}

void TodoWebSocket::broadcastVersion() {
    LOG_DEBUG << "[?] Broadcasting the version";

    Json::Value jsonVersionObject;
    jsonVersionObject["version"] = _version;
        //app().getPlugin<TaskStorePlugin>()->getStore()->getVersion();
    Json::StreamWriterBuilder writer;
    std::string message = Json::writeString(writer, jsonVersionObject);

    std::lock_guard<std::mutex> lock(_clientsMutex);
    for (const auto &conn : _clients) {
        LOG_DEBUG << " +  notification";
        if (conn->connected()) {
            conn->send(message);
        }
    }
}

void TodoWebSocket::bumpVersion() {
    LOG_DEBUG << "[?] Bumping the version";
    ++_version; // the overflow is intentional
    TodoWebSocket::broadcastVersion();
}

void TodoWebSocket::handleNewMessage(
    const WebSocketConnectionPtr &,
    std::string &&,
    const WebSocketMessageType &
) { }

void TodoWebSocket::handleNewConnection(
    const HttpRequestPtr &req,
    const WebSocketConnectionPtr &conn
) {
    LOG_DEBUG << "[?] New web socket connection";
    {
        std::lock_guard<std::mutex> lock(_clientsMutex);
        _clients.insert(conn);
    }
}

void TodoWebSocket::handleConnectionClosed(
    const WebSocketConnectionPtr &conn
) {
    LOG_DEBUG << "[?] A web socket connection has closed";
    std::lock_guard<std::mutex> lock(_clientsMutex);
    _clients.erase(conn);
}
