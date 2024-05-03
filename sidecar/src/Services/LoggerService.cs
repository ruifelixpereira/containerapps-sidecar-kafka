using Grpc.Core;
using Confluent.Kafka;
using UtilitySidecar;

namespace UtilitySidecar.Services;

public class LoggerService : Logger.LoggerBase
{
    private readonly ILogger<LoggerService> _logger;

    // Injected configuration
    private readonly KafkaConfig _config;

     private ProducerConfig _producerConfig;

    public LoggerService(ILogger<LoggerService> logger, KafkaConfig config)
    {
        _logger = logger;
        _config = config;

        _producerConfig = new ProducerConfig
        {
            BootstrapServers = config.BrokerList,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslMechanism = SaslMechanism.Plain,
            SaslUsername = "$ConnectionString",
            SaslPassword = config.ConnectionString,
            SslCaLocation = config.CaCertLocation,
            //Debug = "security,broker,protocol"        //Uncomment for librdkafka debugging information
        };
    }

    public override Task<LogEventReply> SendLogEvent(LogEventRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"SendLogEvent: {request.Message}");

        PublishMessage(request.Message);

        return Task.FromResult(new LogEventReply
        {
            Message = "Received " + request.Message
        });
    }

    private async Task PublishMessage(string message)
    {
        try
        {
            using (var producer = new ProducerBuilder<long, string>(_producerConfig).SetKeySerializer(Serializers.Int64).SetValueSerializer(Serializers.Utf8).Build())
            {
                _logger.LogInformation($"PublishMessage: {message}");
                var msg = string.Format("Message {0} sent at {1}", message, DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss.ffff"));
                var deliveryReport = await producer.ProduceAsync(_config.Topic, new Message<long, string> { Key = DateTime.UtcNow.Ticks, Value = msg });
                _logger.LogInformation(string.Format("Message {0} sent (value: '{1}')", msg, message));
            }
        }
        catch (Exception e)
        {
             _logger.LogError(string.Format("Exception Occurred - {0}", e.Message));
        }
    }
}

public class KafkaConfig
{
    public string BrokerList { get; set; }
    public string ConnectionString { get; set; }
    public string Topic { get; set; }
    public string CaCertLocation { get; set; }
}
