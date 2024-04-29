## Grpc hello world client

Reference: https://learn.microsoft.com/en-us/aspnet/core/tutorials/grpc/grpc-start?view=aspnetcore-8.0&tabs=visual-studio-code#create-the-grpc-client-in-a-net-console-app


## Create project

```bash
dotnet new grpc -o GrpcGreeterClient
cd GrpcGreeterClient

dotnet add GrpcGreeterClient.csproj package Grpc.Net.Client
dotnet add GrpcGreeterClient.csproj package Google.Protobuf
dotnet add GrpcGreeterClient.csproj package Grpc.Tools
```

## Run locally

```bash
# Test service
dotnet run
```

## Run with Docker

```bash
docker run --name greetercli greeterclient:v1 --network=host
```

## Test with docker composer

```bash
cd ..
docker-compose up
```
