cmake_minimum_required(VERSION 3.10)
project(todo-server-drogon)

set(CMAKE_CXX_STANDARD 17)

find_package(Drogon CONFIG REQUIRED)

add_executable(todo-server-drogon
    src/main.cpp
    src/TodoController.h
    src/TodoController.cpp
    src/TodoWebSocket.h
    src/TodoWebSocket.cpp
    src/TaskStore.h
    src/TaskStore.cpp
    src/TaskStorePlugin.h
    src/TaskStorePlugin.cpp
)

target_link_libraries(todo-server-drogon PRIVATE Drogon::Drogon)

# --- Copy config.json to build output directory ---

set(CONFIG_FILE "${CMAKE_SOURCE_DIR}/config.json")
set(DEST_CONFIG_FILE "${CMAKE_BINARY_DIR}/config.json")

add_custom_target(copy_config ALL
    COMMAND ${CMAKE_COMMAND} -E copy_if_different ${CONFIG_FILE} ${DEST_CONFIG_FILE}
    COMMENT "Copying config.json to build directory"
    VERBATIM
)

add_dependencies(todo-server-drogon copy_config)
