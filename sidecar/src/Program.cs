using UtilitySidecar.Services;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

// Prepare and inject configuration
var kafkaConfig = GetKafkaConfig(builder.Configuration);
builder.Services.AddSingleton<KafkaConfig>(sp => kafkaConfig);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<LoggerService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();


static KafkaConfig GetKafkaConfig(IConfiguration configuration)
{
    /*
    var kafkaConfig = new KafkaConfig
    {
        BrokerList = configuration["EH_FQDN"],
        ConnectionString = configuration["EH_CONNECTION_STRING"],
        Topic = configuration["EH_NAME"],
        CaCertLocation = configuration["CA_CERT_LOCATION"]
    };
    */
    var kafkaConfig = new KafkaConfig
    {
        BrokerList = Environment.GetEnvironmentVariable("EH_FQDN") ?? "xxxxxxx.servicebus.windows.net:9093",
        ConnectionString = Environment.GetEnvironmentVariable("EH_CONNECTION_STRING") ?? "Endpoint=sb://xxxxxxx.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=xxxxxxx",
        Topic = Environment.GetEnvironmentVariable("EH_NAME") ?? "logger-events",
        CaCertLocation = Environment.GetEnvironmentVariable("CA_CERT_LOCATION") ?? "/app/certs/cacert.pem"
    };
    
    return kafkaConfig;
}

