#pragma once

#include <drogon/drogon.h>
#include <memory>

class TaskStore;

class TaskStorePlugin : public drogon::Plugin<TaskStorePlugin> {
private:
    std::shared_ptr<TaskStore> _store;

public:
    TaskStorePlugin();

    void initAndStart(const Json::Value &) override;

    void shutdown() override;

    std::shared_ptr<TaskStore> getStore();
};
