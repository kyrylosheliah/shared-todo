#pragma once

#include <drogon/HttpController.h>

using namespace drogon;

class TodoCrudController : public HttpController<TodoCrudController> {
public:
    METHOD_LIST_BEGIN
        ADD_METHOD_TO(TodoCrudController::getTasks, "/api/tasks", Get);
        ADD_METHOD_TO(TodoCrudController::addTask, "/api/task", Post);
        ADD_METHOD_TO(TodoCrudController::updateTask, "/api/task", Put);
        ADD_METHOD_TO(TodoCrudController::deleteTask, "/api/task/{1}", Delete);
    METHOD_LIST_END

    TodoCrudController() = default;

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