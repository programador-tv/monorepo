using System.Globalization;
using Background;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Presentation.EndPoints;
using Queue;
using Sentry;
using Serilog;

namespace Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddSpecificTimeZone(this IServiceCollection services)
    {
        CultureInfo culture = new("pt-BR");
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        return services;
    }

    public static IServiceCollection AddWorkers(this IServiceCollection services)
    {
        services.AddHostedService<TimeSelectionWorker>();
        services.AddHostedService<JoinTimeWorker>();
        services.AddHostedService<LiveCloseWorker>();
        services.AddHostedService<LiveNotifyWorker>();

        return services;
    }

    public static IServiceCollection AddAppInsigths(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        string env =
            Environment.GetEnvironmentVariable("ENVIROMENT")
            ?? configuration["Settings:ENVIROMENT"]
            ?? "Local";

        Console.WriteLine("--------------------");

        if (env != "Production")
        {
            Log.Information("Sentry Application Inshgths in non-production enviroments");
            return services;
        }

        services.AddApplicationInsightsTelemetry();
        return services;
    }

    public static IServiceCollection AddSentry(
        this IServiceCollection _,
        IConfiguration configuration
    )
    {
        string env =
            Environment.GetEnvironmentVariable("ENVIROMENT")
            ?? configuration["Settings:ENVIROMENT"]
            ?? "Local";

        Console.WriteLine("--------------------");

        if (env != "Production")
        {
            Log.Information("Sentry skiped in non-production enviroments");
            return _;
        }

        string url =
            Environment.GetEnvironmentVariable("SENTRY_DSN")
            ?? configuration["Settings:SENTRY_DSN"]
            ?? throw new KeyNotFoundException("SENTRY_DSN");

        SentryOptions sentryOptions =
            new()
            {
                Dsn = url,
                SendDefaultPii = false,
                TracesSampleRate = 1.0,
                AutoSessionTracking = true,
                IsGlobalModeEnabled = true,
                EnableTracing = true,
            };

        // Configura o nível mínimo de detalhamento de trilhas (breadcrumbs)
        sentryOptions.SetBeforeBreadcrumb(
            (breadcrumb, _) =>
            {
                if (breadcrumb.Level < BreadcrumbLevel.Warning)
                {
                    return null;
                }
                return breadcrumb;
            }
        );

        SentrySdk.Init(sentryOptions);

        Log.Information("Sentry started");
        return _;
    }

    public static IServiceCollection AddSentry(
        this IServiceCollection _,
        IConfiguration configuration,
        ConfigureWebHostBuilder host
    )
    {
        string env =
            Environment.GetEnvironmentVariable("ENVIROMENT")
            ?? configuration["Settings:ENVIROMENT"]
            ?? "Local";

        Console.WriteLine("--------------------");

        if (env != "Production")
        {
            Log.Information("Sentry skiped in non-production enviroments");
            return _;
        }

        string url =
            Environment.GetEnvironmentVariable("SENTRY_DSN")
            ?? configuration["Settings:SENTRY_DSN"]
            ?? throw new KeyNotFoundException("SENTRY_DSN");

        host.UseSentry(options =>
        {
            options.Dsn = url;
            options.SendDefaultPii = false;
            options.MinimumBreadcrumbLevel = LogLevel.Warning;
            options.MinimumEventLevel = LogLevel.Error;
            options.TracesSampleRate = 1.0;
            options.AutoSessionTracking = true;
            options.IsGlobalModeEnabled = true;
            options.EnableTracing = true;

            options.SetBeforeSend(
                (@event, hint) =>
                {
                    @event.ServerName = null;
                    return @event;
                }
            );
        });

        Log.Information("Sentry started");
        return _;
    }

    public static IServiceCollection AddQueuing(
        this IServiceCollection services,
        IConfiguration configuration,
        bool consumers
    )
    {
        string url =
            Environment.GetEnvironmentVariable("RABBITMQ_URL")
            ?? configuration["RabbitMQ:RABBITMQ_URL"]
            ?? throw new KeyNotFoundException("RABBITMQ_URL");
        string user =
            Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_USER")
            ?? configuration["RabbitMQ:RABBITMQ_DEFAULT_USER"]
            ?? throw new KeyNotFoundException("RABBITMQ_DEFAULT_USER");
        string pass =
            Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_PASS")
            ?? configuration["RabbitMQ:RABBITMQ_DEFAULT_PASS"]
            ?? throw new KeyNotFoundException("RABBITMQ_DEFAULT_PASS");

        services.AddMassTransit(x =>
        {
            if (consumers)
            {
                x.AddConsumer<NotificationsQueue>();
                x.AddConsumer<CommentsQueue>();
                x.AddConsumer<ChatsQueue>();
                x.AddConsumer<EmailsQueue>();
                x.AddConsumer<LiveThumbnailQueue>();
            }

            x.UsingRabbitMq(
                (context, cfg) =>
                {
                    cfg.Host(
                        url,
                        user,
                        h =>
                        {
                            h.Username(user);
                            h.Password(pass);
                        }
                    );

                    cfg.ConfigureEndpoints(context);
                }
            );
        });

        services.AddScoped<IMessagePublisher, MassTransitMessagePublisher>();
        return services;
    }

    public static IServiceCollection AddStreamingQueuing(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        string url =
            Environment.GetEnvironmentVariable("RABBITMQ_URL")
            ?? configuration["RabbitMQ:RABBITMQ_URL"]
            ?? throw new KeyNotFoundException("RABBITMQ_URL");
        string user =
            Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_USER")
            ?? configuration["RabbitMQ:RABBITMQ_DEFAULT_USER"]
            ?? throw new KeyNotFoundException("RABBITMQ_DEFAULT_USER");
        string pass =
            Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_PASS")
            ?? configuration["RabbitMQ:RABBITMQ_DEFAULT_PASS"]
            ?? throw new KeyNotFoundException("RABBITMQ_DEFAULT_PASS");

        services.AddMassTransit(x =>
        {
            x.AddConsumer<LiveStreamingQueue>(cfg =>
            {
                cfg.ConcurrentMessageLimit = 5;
            });
            x.AddConsumer<LiveCloseQueue>();

            x.UsingRabbitMq(
                (context, cfg) =>
                {
                    cfg.Host(
                        url,
                        user,
                        h =>
                        {
                            h.Username(user);
                            h.Password(pass);
                        }
                    );

                    cfg.ConfigureEndpoints(context);
                }
            );
        });

        services.AddScoped<IMessagePublisher, MassTransitMessagePublisher>();
        return services;
    }

    public static IServiceCollection AddBrowserQueuing(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        string url =
            Environment.GetEnvironmentVariable("RABBITMQ_URL")
            ?? configuration["RabbitMQ:RABBITMQ_URL"]
            ?? throw new KeyNotFoundException("RABBITMQ_URL");
        string user =
            Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_USER")
            ?? configuration["RabbitMQ:RABBITMQ_DEFAULT_USER"]
            ?? throw new KeyNotFoundException("RABBITMQ_DEFAULT_USER");
        string pass =
            Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_PASS")
            ?? configuration["RabbitMQ:RABBITMQ_DEFAULT_PASS"]
            ?? throw new KeyNotFoundException("RABBITMQ_DEFAULT_PASS");

        services.AddMassTransit(x =>
        {
            x.AddConsumer<GenerateOpenGraphImageQueue>(cfg =>
            {
                cfg.ConcurrentMessageLimit = 5;
            });

            x.UsingRabbitMq(
                (context, cfg) =>
                {
                    cfg.Host(
                        url,
                        user,
                        h =>
                        {
                            h.Username(user);
                            h.Password(pass);
                        }
                    );

                    cfg.ConfigureEndpoints(context);
                }
            );
        });

        services.AddScoped<IMessagePublisher, MassTransitMessagePublisher>();
        return services;
    }

    public static IEndpointRouteBuilder AddEndPoints(this IEndpointRouteBuilder app)
    {
        app.AddPerfilsEndPoints();
        app.AddNotificationsEndPoints();
        app.AddLiveEndPoints();
        app.AddHelpBackstageEndPoint();
        app.AddTimeSelectionEndPoints();
        app.AddJoinTimeEndPoints();
        app.AddCommentEndPoints();
        app.AddChatEndPoints();
        app.AddFollowEndPoints();
        app.AddLikeEndPoints();

        return app;
    }
}
