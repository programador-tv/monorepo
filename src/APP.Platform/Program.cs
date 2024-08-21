using System.Globalization;
using APP.Platform;
using APP.Platform.Data;
using APP.Platform.Services;
using Infrastructure;
using Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Platform.Services;
using Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var connectionString =
    Environment.GetEnvironmentVariable("APP_CONNECTIONSTRING")
    ?? builder.Configuration["ConnectionStrings:APP_CONNECTIONSTRING"]
    ?? string.Empty;

var identityConnection =
    Environment.GetEnvironmentVariable("IDENTITY_CONNECTIONSTRING")
    ?? builder.Configuration["ConnectionStrings:IDENTITY_CONNECTIONSTRING"]
    ?? string.Empty;

var PerfilConnectionString =
    Environment.GetEnvironmentVariable("PERFIL_CONNECTIONSTRING")
    ?? builder.Configuration["ConnectionStrings:PERFIL_CONNECTIONSTRING"]
    ?? string.Empty;

var ProducaoConnectionString =
    Environment.GetEnvironmentVariable("PRODUCAO_CONNECTIONSTRING")
    ?? builder.Configuration["ConnectionStrings:PRODUCAO_CONNECTIONSTRING"]
    ?? string.Empty;

var meet_url =
    Environment.GetEnvironmentVariable("MEET_URL")
    ?? builder.Configuration["Settings:MEET_URL"]
    ?? string.Empty;

var liveapi_url =
    Environment.GetEnvironmentVariable("LIVEAPI_URL")
    ?? builder.Configuration["Settings:LIVEAPI_URL"]
    ?? string.Empty;

var livews_url =
    Environment.GetEnvironmentVariable("LIVESOCKET_URL")
    ?? builder.Configuration["Settings:LIVESOCKET_URL"]
    ?? string.Empty;

var mediaserver_url =
    Environment.GetEnvironmentVariable("MEDIASERVER_URL")
    ?? builder.Configuration["Settings:MEDIASERVER_URL"]
    ?? string.Empty;

var environment =
    Environment.GetEnvironmentVariable("ENVIROMENT")
    ?? builder.Configuration["Settings:ENVIROMENT"]
    ?? string.Empty;

var chatSocketUrl =
    Environment.GetEnvironmentVariable("CHATSOCKET_URL")
    ?? builder.Configuration["Settings:CHATSOCKET_URL"]
    ?? string.Empty;

var settings = new Settings()
{
    MEET_URL = meet_url,
    LIVEAPI_URL = liveapi_url,
    ENVIROMENT = environment,
    LIVESOCKET_URL = livews_url,
    MEDIASERVER_URL = mediaserver_url,
    CHATSOCKET_URL = chatSocketUrl
};

builder.Services.Configure<Settings>(builder.Configuration.GetSection("Settings"));
builder.Services.AddSingleton(settings);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(ProducaoConnectionString)
);

builder.Services.AddDbContext<PerfilDbContext>(options =>
    options.UseSqlServer(ProducaoConnectionString)
);

builder.Services.AddDbContext<IdentityApplicationDbContext>(options =>
    options.UseSqlServer(ProducaoConnectionString)
);

builder
    .Services.AddWebServices(builder.Configuration)
    .AddQueuing(builder.Configuration, consumers: false)
    .AddSpecificTimeZone()
    // .AddRedis(builder.Configuration)
    .AddSentry(builder.Configuration, builder.WebHost)
    .AddAppInsigths(builder.Configuration);

// builder.Services.AddSignalR();

builder.Services.AddSingleton<RateLimit>();
builder.Services.AddScoped<OpenAiService>();
builder.Services.AddScoped<IEnsinarService, EnsinarService>();
builder.Services.AddScoped<ILiveService, LiveService>();
builder.Services.AddScoped<IAprenderService, AprenderService>();
builder.Services.AddScoped<IAliasService, AliasService>();

builder
    .Services.AddDefaultIdentity<IdentityUser>(options =>
        options.SignIn.RequireConfirmedAccount = false
    )
    .AddEntityFrameworkStores<IdentityApplicationDbContext>();

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Perfil");
    // options.Conventions.AuthorizeFolder("/Ensinar");
    // options.Conventions.AuthorizeFolder("/Aprender");
    options.Conventions.AuthorizeFolder("/Feedback");
    options.Conventions.AuthorizeFolder("/End");
    options.Conventions.AuthorizeFolder("/Notificacoes");
    options.Conventions.AuthorizeFolder("/Canal/Editor");
    options.Conventions.AuthorizeFolder("/Studio");
    options.Conventions.AuthorizeFolder("/Sala");
    // options.Conventions.AuthorizeFolder("/Canal");
    // options.Conventions.AuthorizeFolder("/Busca");
    // options.Conventions.AuthorizeFolder("/Watch");
    options.Conventions.AddPageRoute("/Watch", "/Watch/{key}");
});

var GoogleClientId =
    Environment.GetEnvironmentVariable("Google_ClientId")
    ?? builder.Configuration["Google:ClientId"]
    ?? string.Empty;

var GoogleSecret =
    Environment.GetEnvironmentVariable("Google_ClientSecret")
    ?? builder.Configuration["Google:ClientSecret"]
    ?? string.Empty;

var LinkedinClientId =
    Environment.GetEnvironmentVariable("LinkedIn_ClientId")
    ?? builder.Configuration["LinkedIn:ClientId"]
    ?? string.Empty;

var LinkedinSecret =
    Environment.GetEnvironmentVariable("LinkedIn_ClientSecret")
    ?? builder.Configuration["LinkedIn:ClientSecret"]
    ?? string.Empty;

var GitHubClientId =
    Environment.GetEnvironmentVariable("GitHub_ClientId")
    ?? builder.Configuration["GitHub:ClientId"]
    ?? string.Empty;

var GitHubSecret =
    Environment.GetEnvironmentVariable("GitHub_ClientSecret")
    ?? builder.Configuration["GitHub:ClientSecret"]
    ?? string.Empty;

builder
    .Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = GoogleClientId;
        googleOptions.ClientSecret = GoogleSecret;
    })
    .AddLinkedIn(options =>
    {
        options.ClientId = LinkedinClientId;
        options.ClientSecret = LinkedinSecret;
        options.Scope.Add("r_liteprofile");
        options.Scope.Add("r_emailaddress");
        options.SaveTokens = true;
    })
    .AddGitHub(options =>
    {
        options.ClientId = GitHubClientId;
        options.ClientSecret = GitHubSecret;
        options.Scope.Add("user:email");
        options.SaveTokens = true;
    });

var app = builder.Build();

//ISSO TEM A VER COM O FATO DE SE USAR DOCKER+PROXY NGINX E PRECISAR RECEBER O IP DO CLIENTE
var forwardedHeadersOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
    RequireHeaderSymmetry = false
};
forwardedHeadersOptions.KnownNetworks.Clear();
forwardedHeadersOptions.KnownProxies.Clear();

app.UseForwardedHeaders(forwardedHeadersOptions);

app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days.
    // You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// app.MapHub<ChatHub>("/chat");

app.Run();
