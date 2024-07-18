using Application;
using Domain.Contracts;
using Infrastructure;
using Presentation;

var builder = Host.CreateApplicationBuilder(args);

builder
    .Services.AddBrowserUtils()
    .AddWebServices(builder.Configuration)
    .AddPublicWebServices(builder.Configuration)
    .AddBrowserQueuing(builder.Configuration);

// .AddSentry(builder.Configuration)
// .AddAppInsigths(builder.Configuration);

var host = builder.Build();
host.Run();
