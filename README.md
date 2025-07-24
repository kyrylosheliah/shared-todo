# Shared To-do list app

## Prerequisites

- Docker
- .NET SDK 9.0+

## Run

Build the docker image for the server
```
cd server-drogon
sudo docker build -t todo-server-drogon .
sudo docker run -d -p 5000:5000 todo-server-drogon
curl -H 'Content-Type: application/json' -d '{ "description": "task1" }' -X POST http://127.0.0.1:5000/api/tasks
curl http://127.0.0.1:5000/api/tasks
```

## Development

```
sudo docker run -it -p 5000:5000 -v <project_dir>/shared-todo/server-drogon:/app -w /app drogonframework/drogon bash
mkdir build && cd build
clear && rm -rf ./* && cmake ../ && make
```