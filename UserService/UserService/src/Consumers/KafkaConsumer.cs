using System.Text.Json;
using Confluent.Kafka;
using UserService.DTOs;
using UserService.Services;

namespace UserService.Consumers;

public class KafkaConsumer : BackgroundService
{
    private readonly ILogger<KafkaConsumer> logger;
    private readonly IServiceProvider serviceProvider;

    private readonly string bootstrapServers;
    private readonly string topic;
    private readonly string groupId;

    public KafkaConsumer(ILogger<KafkaConsumer> logger, IServiceProvider serviceProvider, IConfiguration configuration)
    {
        this.logger = logger;
        this.serviceProvider = serviceProvider;
        var kafkaConfig = configuration.GetSection("Kafka");
        bootstrapServers = kafkaConfig["BootstrapServers"]!;
        topic = kafkaConfig["Topic"]!;
        groupId = kafkaConfig["GroupId"]!;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };
        using var consumer = new ConsumerBuilder<Ignore, string>(config)
            .SetErrorHandler((_, e) => logger.LogError(e.Reason))
            .Build();
        consumer.Subscribe(topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ConsumeMessage(consumer);
                await Task.Yield();
            }
        }
        finally
        {
            consumer.Close();
            logger.LogInformation("Consumer Stopped");
        }
    }

    private async Task ConsumeMessage(IConsumer<Ignore, string> consumer)
    {
        ConsumeResult<Ignore, string>? consumeResult = null;
        try
        {
            consumeResult = consumer.Consume(TimeSpan.FromMilliseconds(100));
            if (consumeResult == null)
                return;
            logger.LogInformation($"Received message: {consumeResult.Message.Value}");
            using var json = JsonDocument.Parse(consumeResult.Message.Value);

            await ProcessMessage(json);
            consumer.Commit(consumeResult);
        }
        catch (ConsumeException e)
        {
            logger.LogError(e, $"Consume error: {e.Error.Reason} [{e.Error.Code}]");
            Task.Delay(1000).Wait();
        }
        catch (Exception ex)
        {
            if (consumeResult != null)
                consumer.Commit(consumeResult);
            logger.LogError(ex, $"Error occured while consuming message: {ex.Message}");
        }
    }

    private async Task ProcessMessage(JsonDocument json)
    {
        var eventType = json.RootElement.GetProperty("Event").GetString() ?? "";

        using var scope = serviceProvider.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

        if (eventType == "registered")
        {
            var userDto = json.RootElement.GetProperty("Data").Deserialize<CreateUserDto>();
            if (userDto == null)
                throw new JsonException("Deserialize object is null");
            await userService.CreateUser(userDto);
        }
        else
            logger.LogWarning($"Received unknown event: {eventType}");
    }
}