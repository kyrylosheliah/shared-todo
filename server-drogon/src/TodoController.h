#pragma once

#include <drogon/HttpController.h>

using namespace drogon;

class TodoController : public HttpController<TodoController> {
public:
    METHOD_LIST_BEGIN
        ADD_METHOD_TO(TodoController::getTasks, "/api/tasks", Get);
        ADD_METHOD_TO(TodoController::addTask, "/api/task", Post);
        ADD_METHOD_TO(TodoController::updateTask, "/api/task", Put);
        ADD_METHOD_TO(TodoController::deleteTask, "/api/task/{1}", Delete);
    METHOD_LIST_END

    TodoController() = default;

protected:
    void getTasks(
        const HttpRequestPtr &req,
        std::function<void(const HttpResponsePtr &)> &&callback
    );

    void addTask(
        const HttpRequestPtr& req,
        std::function<void(const HttpResponsePtr &)> &&callback
    );

    void updateTask(
        const HttpRequestPtr& req,
        std::function<void(const HttpResponsePtr &)> &&callback
    );

    void deleteTask(
        const HttpRequestPtr& req,
        std::function<void(const HttpResponsePtr &)> &&callback,
        const std::string &idPathParam
    );
};