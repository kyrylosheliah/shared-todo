FROM drogonframework/drogon

WORKDIR /app

COPY . .

RUN rm -rf build
WORKDIR /app/build

RUN cmake ../ && make

EXPOSE 5000 5000

ENTRYPOINT [ "./todo-server" ]