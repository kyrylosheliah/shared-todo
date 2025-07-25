#pragma once

#include <drogon/drogon.h>
#include <vector>
#include <mutex>
#include <unordered_map>
#include <string>
#include <functional>
#include <algorithm>
#include "TodoWebSocket.h"

struct Task {
    int id;
    std::string description;
    std::string status;
};

class TaskStore {

private:

    std::unordered_map<int, Task> _tasks;
    std::mutex _mutex;
    int _nextId = 1;
    std::function<void()> _onChange;

public:

    void setOnChange(std::function<void()> callback);

    Task addTask(const std::string& description);

    bool deleteTask(int id);

    bool updateTask(int id, const std::string& description, const std::string& status);

    std::vector<Task> getAllTasks();

    std::optional<Task> getTask(int id);

};
