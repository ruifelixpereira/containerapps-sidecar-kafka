using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using albumapi_csharp;

var builder = WebApplication.CreateBuilder();

// The port number must match the port of the gRPC server.
//var sidecarUri = builder.Configuration["SIDECAR_URI"] ?? "https://localhost:5002";
var sidecarUri = Environment.GetEnvironmentVariable("SIDECAR_URI") ?? "https://localhost:5001";
using var channel = CreateChannel(sidecarUri);
var client = new Logger.LoggerClient(channel);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin();
        builder.AllowAnyHeader();
        builder.AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.MapGet("/", async context =>
{
    await context.Response.WriteAsync("Hit the /albums endpoint to retrieve a list of albums!");
});

app.MapGet("/albums", async () =>
{
    var result = Album.GetAll(); 

    // Send log event to sidecar
    var reply = await Task.FromResult( client.SendLogEvent( new LogEventRequest { Message = "loooooooooooooooooooooog" } ) );
    Console.WriteLine("Return from sidecar: " + reply.Message);

    return result;
})
.WithName("GetAlbums");

app.Run();


static GrpcChannel CreateChannel(string grpcServerUri)
{
    // Retry policy for gRPC client
    var methodConfig = new MethodConfig
    {
        Names = { MethodName.Default },
        RetryPolicy = new RetryPolicy
        {
            MaxAttempts = 5,
            InitialBackoff = TimeSpan.FromSeconds(0.5),
            MaxBackoff = TimeSpan.FromSeconds(0.5),
            BackoffMultiplier = 1,
            RetryableStatusCodes = { StatusCode.Unavailable }
        }
    };

    // Return "true" to allow certificates that are untrusted/invalid
    var httpHandler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };

    return GrpcChannel.ForAddress(grpcServerUri, new GrpcChannelOptions
    {
        ServiceConfig = new ServiceConfig { MethodConfigs = { methodConfig } },
        HttpHandler = httpHandler
    });
}

record Album(int Id, string Title, string Artist, double Price, string Image_url)
{
    public static List<Album> GetAll()
    {
        var albums = new List<Album>(){
            new Album(1, "You, Me and an App Id", "Daprize", 10.99, "https://aka.ms/albums-daprlogo"),
            new Album(2, "Seven Revision Army", "The Blue-Green Stripes", 13.99, "https://aka.ms/albums-containerappslogo"),
            new Album(3, "Scale It Up", "KEDA Club", 13.99, "https://aka.ms/albums-kedalogo"),
            new Album(4, "Lost in Translation", "MegaDNS", 12.99,"https://aka.ms/albums-envoylogo"),
            new Album(5, "Lock Down Your Love", "V is for VNET", 12.99, "https://aka.ms/albums-vnetlogo"),
            new Album(6, "Sweet Container O' Mine", "Guns N Probeses", 14.99, "https://aka.ms/albums-containerappslogo")
         };

        return albums;
    }
}


