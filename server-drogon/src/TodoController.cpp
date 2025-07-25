#include "TodoController.h"
#include "TaskStore.h"
#include "TaskStorePlugin.h"
#include <json/json.h>
#include <sstream>

void TodoController::getTasks(
    const HttpRequestPtr &req,
    std::function<void(const HttpResponsePtr &)> &&callback
) {
    auto tasks = app().getPlugin<TaskStorePlugin>()->getStore()->getAllTasks();
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

void TodoController::addTask(
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
    Task newTask = app().getPlugin<TaskStorePlugin>()->getStore()->addTask(description);
    Json::Value jsonResponse;
    jsonResponse["id"] = newTask.id;
    jsonResponse["description"] = newTask.description;
    jsonResponse["status"] = newTask.status;
    auto resp = HttpResponse::newHttpJsonResponse(jsonResponse);
    callback(resp);
}

void TodoController::updateTask(
    const HttpRequestPtr& req,
    std::function<void(const HttpResponsePtr &)> &&callback
) {
    auto json = req->getJsonObject();
    if (!json || !(*json).isMember("id")) {
        auto resp = HttpResponse::newHttpResponse();
        resp->setStatusCode(k400BadRequest);
        resp->setBody("Missing 'id'");
        callback(resp);
        return;
    }

    const auto& idField = (*json)["id"];
    if (!idField.isNumeric()) {
        auto resp = HttpResponse::newHttpResponse();
        resp->setStatusCode(k400BadRequest);
        resp->setBody("Invalid 'id' type");
        callback(resp);
        return;
    }

    const int id = idField.asInt();
    auto taskRecord = app().getPlugin<TaskStorePlugin>()->getStore()->getTask(id);
    if (!taskRecord) {
        auto resp = HttpResponse::newHttpResponse();
        resp->setStatusCode(k404NotFound);
        resp->setBody("Entity with such 'id' not found");
        callback(resp);
        return;
    }

    auto task = taskRecord.value();
    auto& descriptionField = (*json)["description"];
    auto& statusField = (*json)["status"];

    const bool eitherStringsOrNull = 
        (!descriptionField || descriptionField.isString()) &&
        (!statusField || statusField.isString());

    if (!eitherStringsOrNull) {
        auto resp = HttpResponse::newHttpResponse();
        resp->setStatusCode(k400BadRequest);
        resp->setBody("Invalid 'description' or 'status' types");
        callback(resp);
        return;
    }

    std::string description = descriptionField ? descriptionField.asString() : task.description;
    std::string status = statusField ? statusField.asString() : task.status;

    bool success = app().getPlugin<TaskStorePlugin>()->getStore()->updateTask(id, description, status);
    if (!success) {
        auto resp = HttpResponse::newHttpResponse();
        resp->setStatusCode(k404NotFound);
        resp->setBody("Task not found");
        callback(resp);
        return;
    }

    Json::Value jsonResponse;
    jsonResponse["id"] = id;
    jsonResponse["description"] = description;
    jsonResponse["status"] = status;
    auto resp = HttpResponse::newHttpJsonResponse(jsonResponse);
    callback(resp);
}

void TodoController::deleteTask(
    const HttpRequestPtr& req,
    std::function<void(const HttpResponsePtr &)> &&callback,
    const std::string &idPathParam
) {
    std::istringstream iss(idPathParam);
    int id;
    if (!(iss >> id)) {
        auto resp = HttpResponse::newHttpResponse();
        resp->setStatusCode(k400BadRequest);
        resp->setBody("Invalid 'id' type");
        callback(resp);
        return;
    }

    bool success = app().getPlugin<TaskStorePlugin>()->getStore()->deleteTask(id);
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
