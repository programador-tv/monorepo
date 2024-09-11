using Domain.Interfaces;
using Domain.Repositories;
using Infrastructure;
using Infrastructure.Contexts;
using Infrastructure.FileHandling;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Presentation;
using Queue;

namespace tests;

public class DependencyInjectionTest
{
    private readonly IConfiguration _configuration;

    public DependencyInjectionTest()
    {
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["RabbitMQ:RABBITMQ_URL"] = "your_rabbitmq_url",
                    ["RabbitMQ:RABBITMQ_DEFAULT_USER"] = "your_rabbitmq_user",
                    ["RabbitMQ:RABBITMQ_DEFAULT_PASS"] = "your_rabbitmq_pass",
                    ["ConnectionStrings:APP_CONNECTIONSTRING"] = "your_connection_string",
                    ["Settings:COREAPI_URL"] = "http://a.com",
                }
            )
            .Build();
    }

    [Fact]
    public void AddWebServices_ShouldAddHttpClient()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddWebServices(_configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var httpClient = serviceProvider
            .GetRequiredService<IHttpClientFactory>()
            .CreateClient("CoreAPI");
        Assert.NotNull(httpClient);
        Assert.Equal(new Uri("http://a.com"), httpClient.BaseAddress);
    }

    [Fact]
    public void AddQueuing_ShouldAddMassTransitServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddQueuing(_configuration, consumers: false);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var messagePublisher = serviceProvider.GetRequiredService<IMessagePublisher>();
        Assert.NotNull(messagePublisher);
        // Add more assertions as needed
    }

    [Fact]
    public void AddDatabase_ShouldAddDbContext()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDatabase(_configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
        Assert.NotNull(dbContext);
        // Add more assertions as needed
    }

    [Fact]
    public void AddRepositories_ShouldAddRepositories()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDatabase(_configuration);
        services.AddRepositories();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        Assert.NotNull(serviceProvider.GetRequiredService<IPerfilRepository>());
        Assert.NotNull(serviceProvider.GetRequiredService<INotificationRepository>());
        Assert.NotNull(serviceProvider.GetRequiredService<ILiveRepository>());
        Assert.NotNull(serviceProvider.GetRequiredService<ITimeSelectionRepository>());
        Assert.NotNull(serviceProvider.GetRequiredService<IJoinTimeRepository>());
        Assert.NotNull(serviceProvider.GetRequiredService<IHelpBackstageRepository>());
        Assert.NotNull(serviceProvider.GetRequiredService<ISaveFile>());
        Assert.NotNull(serviceProvider.GetRequiredService<IFollowRepository>());
        Assert.NotNull(serviceProvider.GetRequiredService<ILikeRepository>());
        Assert.NotNull(serviceProvider.GetRequiredService<IFeedbackJoinTimeRepository>());
        // Add more assertions as needed
    }
}
