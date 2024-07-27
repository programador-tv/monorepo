using Domain.Repositories;
using Domain.WebServices;
using Infrastructure.Browser;
using Infrastructure.Contexts;
using Infrastructure.FileHandling;
// using Infrastructure.Redis;
using Infrastructure.Repositories;
using Infrastructure.WebServices;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using StackExchange.Redis;

namespace Infrastructure;

public static class DependencyInjection
{
    // public static IServiceCollection AddRedisCache(
    //     this IServiceCollection services,
    //     IConfiguration configuration
    // )
    // {
    //     var hostAndPort =
    //         Environment.GetEnvironmentVariable("REDIS_HOST")
    //         ?? configuration["Settings:REDIS_HOST"]
    //         ?? throw new NullReferenceException("REDIS_HOST");
    //     var pass =
    //         Environment.GetEnvironmentVariable("REDIS_PASS")
    //         ?? configuration["Settings:REDIS_PASS"]
    //         ?? throw new NullReferenceException("REDIS_PASS");
    //     var user =
    //         Environment.GetEnvironmentVariable("REDIS_USER")
    //         ?? configuration["Settings:REDIS_USER"]
    //         ?? throw new NullReferenceException("REDIS_USER");
    //     var host = hostAndPort.Split(":")[0];
    //     var port = int.Parse(hostAndPort.Split(":")[1]);
    //     var options = new ConfigurationOptions
    //     {
    //         EndPoints = { { host, port } },
    //         User = user,
    //         Password = pass,
    //         Ssl = true,
    //         SslProtocols = System.Security.Authentication.SslProtocols.Tls12
    //     };
    //     services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(options));

    //     return services;
    // }

    public static IServiceCollection AddLogs(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        string connectionString =
            Environment.GetEnvironmentVariable("APP_CONNECTIONSTRING")
            ?? configuration.GetConnectionString("APP_CONNECTIONSTRING")
            ?? throw new KeyNotFoundException("APP_CONNECTIONSTRING");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .WriteTo.MSSqlServer(
                connectionString: connectionString,
                sinkOptions: new MSSqlServerSinkOptions { TableName = "Logs" }
            )
            .WriteTo.Console()
            .CreateLogger();
        return services;
    }

    public static IServiceCollection AddRedis(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        string connectionString =
            Environment.GetEnvironmentVariable("REDIS")
            ?? configuration.GetConnectionString("REDIS")
            ?? throw new KeyNotFoundException("REDIS");

        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(connectionString)
        );
        // services.AddSingleton<IRedisContext, RedisContext>();
        // services.AddSingleton<IChatRedisRepository, ChatRedisRepository>();

        return services;
    }

    public static IServiceCollection AddPublicWebServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var webappUrl =
            Environment.GetEnvironmentVariable("WEBAPP_URL")
            ?? configuration["Settings:WEBAPP_URL"]
            ?? throw new KeyNotFoundException("WEBAPP_URL");

        services.AddHttpClient(
            "PublicWebApp",
            httpClient =>
            {
                httpClient.BaseAddress = new Uri(webappUrl);
            }
        );

        services.AddScoped<WebAppClient>();
        services.AddScoped<IPublicWebService, PublicWebService>();

        return services;
    }

    public static IServiceCollection AddOpenaiWebServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var openai_url =
            Environment.GetEnvironmentVariable("API_URL_OPENAI")
            ?? configuration["OpenAIConfig:API_URL_OPENAI"]
            ?? throw new KeyNotFoundException("API_URL_OPENAI");

        var openaiBearer =
            Environment.GetEnvironmentVariable("API_KEY_OPENAI")
            ?? configuration["OpenAIConfig:API_KEY_OPENAI"]
            ?? throw new KeyNotFoundException("API_KEY_OPENAI");

        services.AddHttpClient(
            "OpenAiAPI",
            httpClient =>
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation(
                    "Authorization",
                    "Bearer " + openaiBearer
                );
                httpClient.BaseAddress = new Uri(openai_url);
            }
        );

        services.AddScoped<OpenaiClient>();

        services.AddScoped<IOpenaiWebService, OpenaiWebService>();

        return services;
    }

    public static IServiceCollection AddBocaSujaWebServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var bsapi_url =
            Environment.GetEnvironmentVariable("BOCASUJA_URL")
            ?? configuration["Settings:BOCASUJA_URL"]
            ?? throw new KeyNotFoundException("BOCASUJA_URL");

        services.AddHttpClient(
            "BocaSujaAPI",
            httpClient =>
            {
                httpClient.BaseAddress = new Uri(bsapi_url);
            }
        );

        services.AddScoped<CoreClient>();

        services.AddScoped<BocaSujaClient>();
        services.AddScoped<IBocaSujaWebService, BocaSujanWebService>();
        ;
        return services;
    }

    public static IServiceCollection AddWebServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var coreapi_url =
            Environment.GetEnvironmentVariable("COREAPI_URL")
            ?? configuration["Settings:COREAPI_URL"]
            ?? throw new KeyNotFoundException("COREAPI_URL");

        services
            .AddHttpClient(
                "CoreAPI",
                httpClient =>
                {
                    httpClient.BaseAddress = new Uri(coreapi_url);
                }
            )
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (
                    sender,
                    cert,
                    chain,
                    sslPolicyErrors
                ) => true;
                return handler;
            });

        services.AddScoped<CoreClient>();

        services.AddScoped<IPerfilWebService, PerfilWebService>();
        services.AddScoped<INotificationWebService, NotificationWebService>();
        services.AddScoped<ITimeSelectionWebService, TimeSelectionWebService>();
        services.AddScoped<ILiveWebService, LiveWebService>();
        services.AddScoped<IChatWebService, ChatWebService>();
        services.AddScoped<ICommentWebService, CommentWebService>();
        
        return services;
    }

    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        string connectionString =
            Environment.GetEnvironmentVariable("APP_CONNECTIONSTRING")
            ?? configuration.GetConnectionString("APP_CONNECTIONSTRING")
            ?? throw new KeyNotFoundException("APP_CONNECTIONSTRING");

        services.AddDbContext<ApplicationDbContext>(options =>
            options
                .UseSqlServer(connectionString, b => b.MigrationsAssembly("Infrastructure"))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
        );

        return services;
    }

    public static IServiceCollection AddHelpers(this IServiceCollection services)
    {
        services.AddScoped<ISaveFile, SaveFile>();
        services.AddScoped<IEmailHandling, EmailHandling>();
        return services;
    }

    public static IServiceCollection AddStreamingUtils(this IServiceCollection services)
    {
        services.AddSingleton<IVideoHandling, VideoHandlingSingleton>();
        return services;
    }

    public static IServiceCollection AddBrowserUtils(this IServiceCollection services)
    {
        services.AddScoped<IBrowserHandler, BrowserHandler>();
        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IPerfilRepository, PerfilRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<ILiveRepository, LiveRepository>();
        services.AddScoped<ITimeSelectionRepository, TimeSelectionRepository>();
        services.AddScoped<IJoinTimeRepository, JoinTimeRepository>();
        services.AddScoped<IHelpBackstageRepository, HelpBackstageRepository>();
        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<ISaveFile, SaveFile>();
        services.AddScoped<IFollowRepository, FollowRepository>();
        services.AddScoped<ILikeRepository, LikeRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        return services;
    }
}
