# Shared To-do list app

The data mechanism provides the REST endpoints for the frontend to query.
Making the status field of a string type allows for a potential frontend
functionality enrichment with an assumption of some values marking a completed
task.

The update mechanism provides the to-do list version number updates via a web
socket broadcast.
Using a web socket enables the clients to be notified when a change is made
in the list. This way, they are able to perform reactive updates, but are
still free to implement the interval logic for them on their own.

The client frontend WPF app leverages async execution, observables and
property signals to automatically update the task list and show the visual
countdown for tasks deletion with a background progress bar.

## Prerequisites

- Docker
- .NET SDK 9.0

## Run

Run the WPF form app
```
cd client-wpf
dotnet restore
dotnet run
```

Run the docker image for the server
```
cd server-drogon
sudo docker build -t todo-server-drogon .
sudo docker run -d -p 5000:5000 todo-server-drogon
```

Use the API via multiple WFP form apps and/or in CLI
```
curl -H 'Content-Type: application/json' -d '{ "description": "task1" }' -X POST http://127.0.0.1:5000/api/tasks
curl http://127.0.0.1:5000/api/tasks
```

## Development

Server
```
sudo docker run -it -p 5000:5000 -v <project_dir>/shared-todo/server-drogon:/app -w /app drogonframework/drogon bash
mkdir build && cd build
clear && rm -rf ./* && cmake ../ && make
```