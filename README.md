# Shared To-do list app

## Run

Build the docker image for the server
```
cd server-drogon
sudo docker build -t todo-drogon-server .
sudo docker run -d -p 5000:5000 todo-drogon-server
curl -H 'Content-Type: application/json' -d '{ "title": "task1" }' -X POST http://127.0.0.1:5000/api/tasks
curl -H 'Content-Type: application/json' -d '{ "title": "task2" }' -X POST http://127.0.0.1:5000/api/tasks
curl -H 'Content-Type: application/json' -d '{ "title": "task3" }' -X POST http://127.0.0.1:5000/api/tasks
curl http://127.0.0.1:5000/api/tasks
```

## Development

```
sudo docker run -it -v <project_dir>/shared-todo/server-drogon:/app -w /app drogonframework/drogon bash
mkdir build && cd build
clear && rm -rf ./* && cmake ../ && make
```