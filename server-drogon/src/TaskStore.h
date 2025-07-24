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
    std::string title;
    bool completed;
};

class TaskStore {

private:

    std::unordered_map<int, Task> tasks_;
    std::mutex mutex_;
    int nextId_ = 1;
    std::function<void()> onChange_;

public:

    Task addTask(
        const std::string& title
    ) {
        std::lock_guard lock(mutex_);
        Task task{nextId_++, title, false};
        tasks_[task.id] = task;
        if (onChange_) onChange_();
        return task;
    }

    bool deleteTask(
        int id
    ) {
        std::lock_guard lock(mutex_);
        auto it = tasks_.find(id);
        if (it != tasks_.end()) {
            tasks_.erase(it);
            if (onChange_) onChange_();
            return true;
        }
        return false;
    }

    bool updateTask(
        int id,
        const std::string& title,
        bool completed
    ) {
        std::lock_guard lock(mutex_);
        auto it = tasks_.find(id);
        if (it != tasks_.end()) {
            it->second.title = title;
            it->second.completed = completed;
            if (onChange_) onChange_();
            return true;
        }
        return false;
    }

    std::vector<Task> getAllTasks() {
        std::lock_guard lock(mutex_);
        std::vector<Task> allTasks;
        allTasks.reserve(tasks_.size());
        for (const auto& [id, task] : tasks_) {
            allTasks.push_back(task);
        }
        return allTasks;
    }

    std::optional<Task> getTask(
        int id
    ) {
        std::lock_guard lock(mutex_);
        auto it = tasks_.find(id);
        if (it != tasks_.end()) {
            return it->second;
        }
        return std::nullopt;
    }

    void setOnChange(std::function<void()> callback) {
        onChange_ = std::move(callback);
    }

};
