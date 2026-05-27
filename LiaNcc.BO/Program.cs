using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/lia-bo-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllersWithViews();

var pathBase = builder.Configuration["AppSettings:PathBase"] ?? "";

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = $"{pathBase}/Auth/Login";
        options.LogoutPath = $"{pathBase}/Auth/Logout";
        options.AccessDeniedPath = $"{pathBase}/Auth/AccessDenied";
        options.Cookie.Name = "LiaNcc.BO.Auth";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<LiaNcc.BO.Helpers.CorrelationIdHandler>();

var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];

if (string.IsNullOrEmpty(apiBaseUrl))
{
    throw new InvalidOperationException("ApiSettings:BaseUrl is not configured.");
}

Action<IServiceProvider, HttpClient> configureClient = (sp, client) =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
};

builder.Services.AddHttpClient<LiaNcc.BO.Services.Interfaces.IAuthApiClient, LiaNcc.BO.Services.Implementations.AuthApiClient>(configureClient)
    .AddHttpMessageHandler<LiaNcc.BO.Helpers.CorrelationIdHandler>();

builder.Services.AddHttpClient<LiaNcc.BO.Services.Interfaces.ILocalizedContentsApiClient, LiaNcc.BO.Services.Implementations.LocalizedContentsApiClient>(configureClient)
    .AddHttpMessageHandler<LiaNcc.BO.Helpers.CorrelationIdHandler>();

builder.Services.AddHttpClient<LiaNcc.BO.Services.Interfaces.IDashboardApiClient, LiaNcc.BO.Services.Implementations.DashboardApiClient>(configureClient)
    .AddHttpMessageHandler<LiaNcc.BO.Helpers.CorrelationIdHandler>();

builder.Services.AddHttpClient<LiaNcc.BO.Services.Interfaces.IFilesApiClient, LiaNcc.BO.Services.Implementations.FilesApiClient>(configureClient)
    .AddHttpMessageHandler<LiaNcc.BO.Helpers.CorrelationIdHandler>();

builder.Services.AddHttpClient<LiaNcc.BO.Services.Interfaces.IToursApiClient, LiaNcc.BO.Services.Implementations.ToursApiClient>(configureClient)
    .AddHttpMessageHandler<LiaNcc.BO.Helpers.CorrelationIdHandler>();

builder.Services.AddHttpClient<LiaNcc.BO.Services.Interfaces.IVehiclesApiClient, LiaNcc.BO.Services.Implementations.VehiclesApiClient>(configureClient)
    .AddHttpMessageHandler<LiaNcc.BO.Helpers.CorrelationIdHandler>();

builder.Services.AddHttpClient<LiaNcc.BO.Services.Interfaces.IServicesApiClient, LiaNcc.BO.Services.Implementations.ServicesApiClient>(configureClient)
    .AddHttpMessageHandler<LiaNcc.BO.Helpers.CorrelationIdHandler>();

builder.Services.AddHttpClient<LiaNcc.BO.Services.Interfaces.IMediaAssetsApiClient, LiaNcc.BO.Services.Implementations.MediaAssetsApiClient>(configureClient)
    .AddHttpMessageHandler<LiaNcc.BO.Helpers.CorrelationIdHandler>();

builder.Services.AddHttpClient<LiaNcc.BO.Services.Interfaces.ISitePagesApiClient, LiaNcc.BO.Services.Implementations.SitePagesApiClient>(configureClient)
    .AddHttpMessageHandler<LiaNcc.BO.Helpers.CorrelationIdHandler>();

builder.Services.AddHttpClient<LiaNcc.BO.Services.Interfaces.ICompanyApiClient, LiaNcc.BO.Services.Implementations.CompanyApiClient>(configureClient)
    .AddHttpMessageHandler<LiaNcc.BO.Helpers.CorrelationIdHandler>();

builder.Services.AddHttpClient<LiaNcc.BO.Services.Interfaces.IPartnersApiClient, LiaNcc.BO.Services.Implementations.PartnersApiClient>(configureClient)
    .AddHttpMessageHandler<LiaNcc.BO.Helpers.CorrelationIdHandler>();

builder.Services.AddHttpClient<LiaNcc.BO.Services.Interfaces.IUsersApiClient, LiaNcc.BO.Services.Implementations.UsersApiClient>(configureClient)
    .AddHttpMessageHandler<LiaNcc.BO.Helpers.CorrelationIdHandler>();

builder.Services.AddHttpClient<LiaNcc.BO.Services.Interfaces.IBookingsApiClient, LiaNcc.BO.Services.Implementations.BookingsApiClient>(configureClient)
    .AddHttpMessageHandler<LiaNcc.BO.Helpers.CorrelationIdHandler>();

builder.Services.AddHttpClient<LiaNcc.BO.Services.Interfaces.IContactMessagesApiClient, LiaNcc.BO.Services.Implementations.ContactMessagesApiClient>(configureClient)
    .AddHttpMessageHandler<LiaNcc.BO.Helpers.CorrelationIdHandler>();

builder.Services.AddHttpClient<LiaNcc.BO.Services.Interfaces.ILanguagesApiClient, LiaNcc.BO.Services.Implementations.LanguagesApiClient>(configureClient)
    .AddHttpMessageHandler<LiaNcc.BO.Helpers.CorrelationIdHandler>();

builder.Services.AddHttpClient<LiaNcc.BO.Services.Interfaces.ILogsApiClient, LiaNcc.BO.Services.Implementations.LogsApiClient>(configureClient)
    .AddHttpMessageHandler<LiaNcc.BO.Helpers.CorrelationIdHandler>();

builder.Services.AddScoped<LiaNcc.BO.Services.Interfaces.IApplicationLoggerService, LiaNcc.BO.Services.Implementations.ApplicationLoggerService>();

var app = builder.Build();

if (!string.IsNullOrWhiteSpace(pathBase))
{
    app.UsePathBase(pathBase);
}

// Initialize MediaUrlHelper
LiaNcc.BO.Helpers.MediaUrlHelper.Initialize(app.Configuration);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler($"{pathBase}/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseMiddleware<LiaNcc.BO.Middleware.CorrelationIdMiddleware>();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
