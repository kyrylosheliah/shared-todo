#include "TaskStorePlugin.h"
#include "../services/TaskStoreService.h"
#include "../controllers/TodoWebSocketController.h"
#include <drogon/drogon.h>

TaskStorePlugin::TaskStorePlugin() {
    _store = std::make_shared<TaskStoreService>();
}

void TaskStorePlugin::initAndStart(const Json::Value &) {
    _store->setOnChange([this] {
        LOG_DEBUG << "[?] store changed";
        TodoWebSocketController::bumpVersion();
    });
}

void TaskStorePlugin::shutdown() {}

std::shared_ptr<TaskStoreService> TaskStorePlugin::getStore() {
    return _store;
}
