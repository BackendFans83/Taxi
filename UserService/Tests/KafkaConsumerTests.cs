using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using UserService.Consumers;
using UserService.DTOs;
using UserService.Services;

namespace Tests;

public class KafkaConsumerTests
{
    private readonly Mock<ILogger<KafkaConsumer>> mockLogger;
    private readonly Mock<IUserService> mockService;
    private readonly IServiceProvider serviceProvider;
    private readonly IConfiguration configuration;
    private readonly KafkaConsumer consumer;

    public KafkaConsumerTests()
    {
        mockLogger = new Mock<ILogger<KafkaConsumer>>();
        mockService = new Mock<IUserService>();
        serviceProvider = CreateServiceProvider(mockService);
        configuration = CreateConfiguration();
        consumer = new KafkaConsumer(mockLogger.Object, serviceProvider, configuration);
    }

    private IServiceProvider CreateServiceProvider(Mock<IUserService> mockService)
    {
        var scopeMock = new Mock<IServiceScope>();
        scopeMock.Setup(s => s.ServiceProvider).Returns(new ServiceCollection()
            .AddSingleton(mockService.Object)
            .BuildServiceProvider());

        var scopeFactoryMock = new Mock<IServiceScopeFactory>();
        scopeFactoryMock.Setup(f => f.CreateScope()).Returns(scopeMock.Object);

        var providerMock = new Mock<IServiceProvider>();
        providerMock.Setup(p => p.GetService(typeof(IServiceScopeFactory)))
            .Returns(scopeFactoryMock.Object);

        return providerMock.Object;
    }

    private IConfiguration CreateConfiguration()
    {
        var inMemoryConfig = new Dictionary<string, string?>
        {
            { "Kafka:BootstrapServers", "localhost:9092" },
            { "Kafka:Topic", "test-topic" },
            { "Kafka:GroupId", "test-group" }
        };
        return new ConfigurationBuilder()
            .AddInMemoryCollection(inMemoryConfig)
            .Build();
    }

    #region ProcessMessage Tests

    [Fact]
    public async Task ProcessMessage_RegisteredEvent_CallsCreateUser()
    {
        mockService.Setup(s => s.CreateUser(It.IsAny<CreateUserDto>()))
            .ReturnsAsync(Result.Success());

        var jsonData = new
        {
            Event = "registered",
            Data = new { Id = 1, Name = "Test User", Role = "Passenger" }
        };
        var jsonDocument = JsonDocument.Parse(JsonSerializer.Serialize(jsonData));

        var methodInfo = typeof(KafkaConsumer).GetMethod("ProcessMessage",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        Assert.NotNull(methodInfo);
        var task = methodInfo.Invoke(consumer, new object[] { jsonDocument }) as Task;
        Assert.NotNull(task);
        await task;

        mockService.Verify(s => s.CreateUser(It.Is<CreateUserDto>(u =>
            u.Id == 1 && u.Name == "Test User" && u.Role == "Passenger")), Times.Once);
    }

    [Fact]
    public async Task ProcessMessage_UnknownEvent_LogsWarning()
    {
        var jsonData = new { Event = "unknown_event", Data = new { } };
        var jsonDocument = JsonDocument.Parse(JsonSerializer.Serialize(jsonData));

        var methodInfo = typeof(KafkaConsumer).GetMethod("ProcessMessage",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        Assert.NotNull(methodInfo);
        var task = methodInfo.Invoke(consumer, new object[] { jsonDocument }) as Task;
        Assert.NotNull(task);
        await task;

        mockService.Verify(s => s.CreateUser(It.IsAny<CreateUserDto>()), Times.Never);
    }

    [Fact]
    public async Task ProcessMessage_RegisteredEventWithInvalidRole_ThrowsException()
    {
        mockService.Setup(s => s.CreateUser(It.IsAny<CreateUserDto>()))
            .ReturnsAsync(Result.Failure(400, "Invalid role"));

        var jsonData = new
        {
            Event = "registered",
            Data = new { Id = 1, Name = "Test User", Role = "InvalidRole" }
        };
        var jsonDocument = JsonDocument.Parse(JsonSerializer.Serialize(jsonData));

        var methodInfo = typeof(KafkaConsumer).GetMethod("ProcessMessage",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        Assert.NotNull(methodInfo);
        var task = methodInfo.Invoke(consumer, new object[] { jsonDocument }) as Task;
        Assert.NotNull(task);
        await Assert.ThrowsAsync<ArgumentException>(async () => await task);
    }

    [Fact]
    public async Task ProcessMessage_RegisteredEvent_ProfileAlreadyExists_ThrowsException()
    {
        mockService.Setup(s => s.CreateUser(It.IsAny<CreateUserDto>()))
            .ReturnsAsync(Result.Failure(409));

        var jsonData = new
        {
            Event = "registered",
            Data = new { Id = 1, Name = "Test User", Role = "Passenger" }
        };
        var jsonDocument = JsonDocument.Parse(JsonSerializer.Serialize(jsonData));

        var methodInfo = typeof(KafkaConsumer).GetMethod("ProcessMessage",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        Assert.NotNull(methodInfo);
        var task = methodInfo.Invoke(consumer, new object[] { jsonDocument }) as Task;
        Assert.NotNull(task);
        await Assert.ThrowsAsync<ArgumentException>(async () => await task);
    }

    #endregion
}
