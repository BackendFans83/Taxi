using System.Reflection;
using System.Text.Json;
using Confluent.Kafka;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using RideService.DTOs;
using RideService.Models;
using RideService.Producers;

namespace Tests;

public class KafkaProducerTests
{
    private readonly Mock<ILogger<KafkaProducer>> _loggerMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IProducer<string, string>> _producerMock;
    private const string RideTopic = "ride";
    private const string RideCreatedEvent = "ride";
    
    public KafkaProducerTests()
    {
        _loggerMock = new Mock<ILogger<KafkaProducer>>();
        _configurationMock = new Mock<IConfiguration>();
        _producerMock = new Mock<IProducer<string, string>>();
    }

    private KafkaProducer CreateProducer()
    {
        var kafkaSectionMock = new Mock<IConfigurationSection>();
        kafkaSectionMock.Setup(s => s["BootstrapServers"]).Returns("localhost:9092");
        kafkaSectionMock.Setup(s => s["MessageTimeoutMs"]).Returns("5000");
        kafkaSectionMock.Setup(s => s["RequestTimeoutMs"]).Returns("5000");
        kafkaSectionMock.Setup(s => s["RideTopic"]).Returns(RideTopic);
        kafkaSectionMock.Setup(s => s["RideCreatedEvent"]).Returns(RideCreatedEvent);

        _configurationMock.Setup(c => c.GetSection("Kafka")).Returns(kafkaSectionMock.Object);

        var producer = new KafkaProducer(_configurationMock.Object, _loggerMock.Object);
        ReplaceProducer(producer);
        return producer;
    }

    private void ReplaceProducer(KafkaProducer kafkaProducer)
    {
        var field = typeof(KafkaProducer).GetField("producer", BindingFlags.NonPublic | BindingFlags.Instance);
        field!.SetValue(kafkaProducer, _producerMock.Object);
    }

    [Fact]
    public async Task SendRideCreatedEventAsync_SendsMessageToKafka()
    {
        var producer = CreateProducer();
        var rideDto = new RideDto(new Ride
        {
            Id = 1,
            PassengerId = 123,
            PickupLatitude = 83.97,
            PickupLongitude = 83.97,
            DropOffLatitude = 83.83,
            DropOffLongitude = 83.83,
        });

        var deliveryResult = new DeliveryResult<string, string>
        {
            Topic = RideTopic,
            Message = new Message<string, string> { Key = rideDto.PassengerId.ToString(), Value = "" }
        };

        _producerMock.Setup(p =>
                p.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<string, string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(deliveryResult);

        var result = await producer.SendRideCreatedEventAsync(rideDto);

        Assert.True(result.IsSuccess);
        _producerMock.Verify(
            p => p.ProduceAsync(RideTopic, It.IsAny<Message<string, string>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SendRideCreatedEventAsync_ProduceException_ReturnsFailure()
    {
        var producer = CreateProducer();
        var rideDto = new RideDto(new Ride
        {
            Id = 1,
            PassengerId = 2,
            PickupLatitude = 83.97,
            PickupLongitude = 83.97,
            DropOffLatitude = 83.83,
            DropOffLongitude = 83.83,
        });

        _producerMock.Setup(p =>
                p.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<string, string>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ProduceException<string, string>(new Error(ErrorCode.Unknown, "Kafka error"),
                new DeliveryResult<string, string>()));

        var result = await producer.SendRideCreatedEventAsync(rideDto);

        Assert.False(result.IsSuccess);
        Assert.Equal(500, result.StatusCode);
        Assert.Contains("Kafka error", result.ErrorMessage);
    }

    [Fact]
    public async Task SendMessageAsync_SetsCorrectMessageKeyAndValue()
    {
        var producer = CreateProducer();
        var rideDto = new RideDto(new Ride
        {
            Id = 1,
            PassengerId = 2,
            PickupLatitude = 83.97,
            PickupLongitude = 83.97,
            DropOffLatitude = 83.83,
            DropOffLongitude = 83.83,
        });
        var kafkaEvent = new KafkaEvent<RideDto>(RideCreatedEvent, rideDto);
        var json = JsonSerializer.Serialize(kafkaEvent);

        var deliveryResult = new DeliveryResult<string, string>
        {
            Topic = RideTopic,
            Message = new Message<string, string> { Key = rideDto.PassengerId.ToString(), Value = json }
        };

        Message<string, string>? capturedMessage = null;
        _producerMock.Setup(p =>
                p.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<string, string>>(), It.IsAny<CancellationToken>()))
            .Callback<string, Message<string, string>, CancellationToken>((_, msg, _) => capturedMessage = msg)
            .ReturnsAsync(deliveryResult);

        var sendMessageMethod =
            typeof(KafkaProducer).GetMethod("SendMessageAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        await sendMessageMethod!.InvokeAsync(producer, RideTopic, rideDto.PassengerId.ToString(), json);

        Assert.NotNull(capturedMessage);
        Assert.Equal(rideDto.PassengerId.ToString(), capturedMessage.Key);
        Assert.Equal(json, capturedMessage.Value);
    }

    [Fact]
    public void Dispose_FlushesAndDisposesProducer()
    {
        var producer = CreateProducer();
        producer.Dispose();

        _producerMock.Verify(p => p.Flush(It.IsAny<TimeSpan>()), Times.Once);
    }
}

public static class ReflectionHelper
{
    public static Task InvokeAsync(this MethodInfo method, object obj, params object[] parameters)
    {
        var task = method.Invoke(obj, parameters) as Task;
        return task!;
    }
}