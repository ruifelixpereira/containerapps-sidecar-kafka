## Grpc hello world server

Reference: https://learn.microsoft.com/en-us/aspnet/core/tutorials/grpc/grpc-start?view=aspnetcore-8.0&tabs=visual-studio-code#create-a-grpc-service


## Create project

```bash
dotnet new grpc -o GrpcGreeter
cd GrpcGreeter

# Generate developer vertificates to be used in HTTPS
dotnet dev-certs https --trust
```

## Generate certiticates

Grpc uses HTTPS and requires TLS. The following commands generate self-signed certificates required for the application.

```bash
../scripts/generate-certs.sh
```

## Run locally

```bash
# Test service
dotnet run

# Test service with client
dotnet run -p GrpcGreeterClient
```

## Run with Docker

```bash
docker run -d --name greeter -p 7106:7106 greeterservice:v1 --network=host
```

## Test with docker composer

```bash
cd ..
docker-compose up
```
