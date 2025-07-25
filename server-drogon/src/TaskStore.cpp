#include "TaskStore.h"
#include "TodoWebSocket.h"
#include <drogon/drogon.h>

void TaskStore::setOnChange(std::function<void()> callback) {
    _onChange = std::move(callback);
}

Task TaskStore::addTask(const std::string& description) {
    std::lock_guard lock(_mutex);
    Task task{_nextId++, description, "Pending"};
    _tasks[task.id] = task;
    if (_onChange) _onChange();
    return task;
}

bool TaskStore::deleteTask(int id) {
    std::lock_guard lock(_mutex);
    auto it = _tasks.find(id);
    if (it != _tasks.end()) {
        _tasks.erase(it);
        if (_onChange) _onChange();
        return true;
    }
    return false;
}

bool TaskStore::updateTask(int id, const std::string& description, const std::string& status) {
    std::lock_guard lock(_mutex);
    auto it = _tasks.find(id);
    if (it != _tasks.end()) {
        it->second.description = description;
        it->second.status = status;
        if (_onChange) _onChange();
        return true;
    }
    return false;
}

std::vector<Task> TaskStore::getAllTasks() {
    std::lock_guard lock(_mutex);
    std::vector<Task> allTasks;
    allTasks.reserve(_tasks.size());
    for (const auto& [id, task] : _tasks) {
        allTasks.push_back(task);
    }
    return allTasks;
}

std::optional<Task> TaskStore::getTask(int id) {
    std::lock_guard lock(_mutex);
    auto it = _tasks.find(id);
    if (it != _tasks.end()) {
        return it->second;
    }
    return std::nullopt;
}
