#pragma once

#include <vector>
#include <mutex>
#include <unordered_map>
#include <string>
#include <functional>
#include <algorithm>
#include "TaskStore.h"

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

    Task addTask(
        const std::string& description
    ) {
        std::lock_guard lock(_mutex);
        Task task{_nextId++, description, "Pending"};
        _tasks[task.id] = task;
        if (_onChange) _onChange();
        return task;
    }

    bool deleteTask(
        int id
    ) {
        std::lock_guard lock(_mutex);
        auto it = _tasks.find(id);
        if (it != _tasks.end()) {
            _tasks.erase(it);
            if (_onChange) _onChange();
            return true;
        }
        return false;
    }

    bool updateTask(
        int id,
        const std::string& description,
        const std::string& status
    ) {
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

    std::vector<Task> getAllTasks() {
        std::lock_guard lock(_mutex);
        std::vector<Task> allTasks;
        allTasks.reserve(_tasks.size());
        for (const auto& [id, task] : _tasks) {
            allTasks.push_back(task);
        }
        return allTasks;
    }

    std::optional<Task> getTask(
        int id
    ) {
        std::lock_guard lock(_mutex);
        auto it = _tasks.find(id);
        if (it != _tasks.end()) {
            return it->second;
        }
        return std::nullopt;
    }

    void setOnChange(std::function<void()> callback) {
        _onChange = std::move(callback);
    }

};
