#include <drogon/drogon.h>
#include "TodoController.h"
#include "TodoWebSocket.h"
#include "TaskStore.h"

int main() {
    std::shared_ptr<TaskStore> store = std::make_shared<TaskStore>();

    TodoWebSocket::setTaskStore(store);
    store->setOnChange([] {
        TodoWebSocket::bumpVersion();
    });

    auto controller = std::make_shared<TodoController>();
    controller->setTaskStore(store);

    drogon::app()
        .addListener("0.0.0.0", 5000)
        .run();
}
