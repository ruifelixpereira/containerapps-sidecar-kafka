﻿using System.Threading.Tasks;
using Grpc.Net.Client;
using GrpcGreeterClient;

// Return "true" to allow certificates that are untrusted/invalid
var httpHandler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback =
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
};

// The port number must match the port of the gRPC server.
//using var channel = GrpcChannel.ForAddress("https://localhost:7106", new GrpcChannelOptions { HttpHandler = httpHandler });
using var channel = GrpcChannel.ForAddress("https://greeter-service:7106", new GrpcChannelOptions { HttpHandler = httpHandler });
var client = new Greeter.GreeterClient(channel);
var reply = await client.SayHelloAsync(
                  new HelloRequest { Name = "GreeterClient" });
Console.WriteLine("Greeting: " + reply.Message);
Console.WriteLine("Press any key to exit...");
Console.ReadKey();