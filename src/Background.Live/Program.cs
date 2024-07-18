using Application;
using Domain.Contracts;
using Infrastructure;
using Presentation;

var builder = Host.CreateApplicationBuilder(args);

builder
    .Services.AddWebServices(builder.Configuration)
    .AddStreamingUtils()
    .AddStreamingQueuing(builder.Configuration)
    .AddWebServices(builder.Configuration)
    .AddMemoryCache();

// .AddSentry(builder.Configuration)
// .AddAppInsigths(builder.Configuration);

// .AddSentry(builder.Configuration, builder.);

var host = builder.Build();
host.Run();
