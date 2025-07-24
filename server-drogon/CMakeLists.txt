cmake_minimum_required(VERSION 3.10)
project(todo-server-drogon)

set(CMAKE_CXX_STANDARD 17)

find_package(Drogon CONFIG REQUIRED)

add_executable(todo-server-drogon
    src/main.cc
    src/TodoController.h
    src/TodoWebSocket.h
    src/TaskStore.h
)

target_link_libraries(todo-server-drogon PRIVATE Drogon::Drogon)
