#pragma once

#include <drogon/HttpController.h>
#include <drogon/drogon.h>
#include <json/json.h>
#include "TaskStore.h"
#include <memory>

using namespace drogon;

class TodoController : public HttpController<TodoController> {

private:

    std::shared_ptr<TaskStore> _store;

public:

    METHOD_LIST_BEGIN
        ADD_METHOD_TO(TodoController::getTasks, "/api/tasks", Get);
        ADD_METHOD_TO(TodoController::addTask, "/api/tasks", Post);
        ADD_METHOD_TO(TodoController::updateTask, "/api/tasks/{1}", Put);
        ADD_METHOD_TO(TodoController::deleteTask, "/api/tasks/{1}", Delete);
    METHOD_LIST_END

    TodoController(): _store(std::make_shared<TaskStore>()) {};

    void setTaskStore(std::shared_ptr<TaskStore> store) {
        _store = store;
    }

protected:

    void getTasks(
        const HttpRequestPtr &req,
        std::function<void(const HttpResponsePtr &)> &&callback
    ) {
        auto tasks = _store->getAllTasks();
        Json::Value jsonResponse(Json::arrayValue);
        for (const auto& task : tasks) {
            Json::Value t;
            t["id"] = task.id;
            t["description"] = task.description;
            t["status"] = task.status;
            jsonResponse.append(t);
        }
        auto resp = HttpResponse::newHttpJsonResponse(jsonResponse);
        callback(resp);
    }

    void addTask(
        const HttpRequestPtr& req,
        std::function<void(const HttpResponsePtr &)> &&callback
    ) {
        auto json = req->getJsonObject();
        if (!json || !(*json).isMember("description") || !(*json)["description"].isString()) {
            auto resp = HttpResponse::newHttpResponse();
            resp->setStatusCode(k400BadRequest);
            resp->setBody("Missing or invalid 'description'");
            callback(resp);
            return;
        }
        std::string description = (*json)["description"].asString();
        Task newTask = _store->addTask(description);
        Json::Value jsonResponse;
        jsonResponse["id"] = newTask.id;
        jsonResponse["description"] = newTask.description;
        jsonResponse["status"] = newTask.status;
        auto resp = HttpResponse::newHttpJsonResponse(jsonResponse);
        callback(resp);
    }

    void updateTask(
        const HttpRequestPtr& req,
        std::function<void(const HttpResponsePtr &)> &&callback,
        int id
    ) {
        auto json = req->getJsonObject();
        if (!json || !(*json).isMember("description") || !(*json).isMember("status")) {
            auto resp = HttpResponse::newHttpResponse();
            resp->setStatusCode(k400BadRequest);
            resp->setBody("Missing 'description' or 'status'");
            callback(resp);
            return;
        }
        const auto& description = (*json)["description"];
        const auto& status = (*json)["status"];
        if (!description.isString() || !status.isString()) {
            auto resp = HttpResponse::newHttpResponse();
            resp->setStatusCode(k400BadRequest);
            resp->setBody("Invalid 'description' or 'status' types");
            callback(resp);
            return;
        }
        bool success = _store->updateTask(id, description.asString(), status.asString());
        if (!success) {
            auto resp = HttpResponse::newHttpResponse();
            resp->setStatusCode(k404NotFound);
            resp->setBody("Task not found");
            callback(resp);
            return;
        }
        auto updatedTask = _store->getTask(id);
        Json::Value jsonResponse;
        jsonResponse["id"] = updatedTask->id;
        jsonResponse["description"] = updatedTask->description;
        jsonResponse["status"] = updatedTask->status;
        auto resp = HttpResponse::newHttpJsonResponse(jsonResponse);
        callback(resp);
    }

    void deleteTask(
        const HttpRequestPtr& req,
        std::function<void(const HttpResponsePtr &)> &&callback,
        int id
    ) {
        bool success = _store->deleteTask(id);
        if (!success) {
            auto resp = HttpResponse::newHttpResponse();
            resp->setStatusCode(k404NotFound);
            resp->setBody("Task not found");
            callback(resp);
            return;
        }
        auto resp = HttpResponse::newHttpResponse();
        resp->setStatusCode(k204NoContent);
        callback(resp);
    }

};
