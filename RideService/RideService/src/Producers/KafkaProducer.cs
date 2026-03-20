using System.Text.Json;
using Confluent.Kafka;
using RideService.DTOs;

namespace RideService.Producers;

public class KafkaProducer : IKafkaProducer, IDisposable
{
    private readonly IProducer<string, string> producer;
    private readonly ILogger<KafkaProducer> logger;
    private readonly string RideTopic;
    private readonly string RideCreatedEvent;

    public KafkaProducer(IConfiguration configuration, ILogger<KafkaProducer> logger)
    {
        this.logger = logger;
        var kafkaSection = configuration.GetSection("Kafka");
        RideTopic = kafkaSection["RideTopic"]!;
        RideCreatedEvent = kafkaSection["RideCreatedEvent"]!;
        var config = new ProducerConfig
        {
            BootstrapServers = kafkaSection["BootstrapServers"],
            MessageTimeoutMs = int.Parse(kafkaSection["MessageTimeoutMs"] ?? "5000"),
            RequestTimeoutMs = int.Parse(kafkaSection["RequestTimeoutMs"] ?? "5000"),
        };

        producer = new ProducerBuilder<string, string>(config)
            .SetKeySerializer(Serializers.Utf8)
            .SetValueSerializer(Serializers.Utf8)
            .SetErrorHandler((_, e) => logger.LogError($"Kafka Error: {e.Reason}"))
            .Build();
        logger.LogInformation("Kafka Producer initialized.");
    }

    public async Task<Result> SendRideCreatedEventAsync(RideDto dto)
    {
        var kafkaEvent = new KafkaEvent<RideDto>(RideCreatedEvent, dto);
        var json = JsonSerializer.Serialize(kafkaEvent);
        try
        {
            await SendMessageAsync(RideTopic, dto.Id.ToString(), json);
        }
        catch (ProduceException<string, string> e)
        {
            logger.LogError(e, $"Kafka Produce Error: {e.Error.Reason}");
            return Result.Failure(500, e.Error.Reason);
        }

        return Result.Success();
    }

    private async Task SendMessageAsync(string topic, string key, string message)
    {
        var kafkaMessage = new Message<string, string>()
        {
            Key = key,
            Value = message
        };
        var delivery = await producer.ProduceAsync(topic, kafkaMessage);
        logger.LogInformation($"Kafka Message: {delivery.Message.Value} for topic: {delivery.Topic}");
    }

    public void Dispose()
    {
        logger.LogInformation("Disposing Kafka Producer.");
        producer.Flush(TimeSpan.FromSeconds(10));
        producer.Dispose();
    }
}