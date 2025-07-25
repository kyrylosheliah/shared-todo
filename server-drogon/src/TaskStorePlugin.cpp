#include "TaskStorePlugin.h"
#include "TaskStore.h"
#include "TodoWebSocket.h"
#include <drogon/drogon.h>

TaskStorePlugin::TaskStorePlugin() {
    _store = std::make_shared<TaskStore>();
}

void TaskStorePlugin::initAndStart(const Json::Value &) {
    _store->setOnChange([this] {
        LOG_DEBUG << "[?] store changed";
        TodoWebSocket::bumpVersion();
    });
}

void TaskStorePlugin::shutdown() {}

std::shared_ptr<TaskStore> TaskStorePlugin::getStore() {
    return _store;
}
