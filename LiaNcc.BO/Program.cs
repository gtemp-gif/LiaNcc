using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http.Headers;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.Cookie.Name = "LiaNcc.BO.Auth";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddHttpContextAccessor();

var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];

if (string.IsNullOrEmpty(apiBaseUrl))
{
    throw new InvalidOperationException("ApiSettings:BaseUrl is not configured.");
}

builder.Services.AddHttpClient<LiaNcc.BO.Services.Interfaces.IAuthApiClient, LiaNcc.BO.Services.Implementations.AuthApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<LiaNcc.BO.Services.Interfaces.IToursApiClient, LiaNcc.BO.Services.Implementations.ToursApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<LiaNcc.BO.Services.Interfaces.IVehiclesApiClient, LiaNcc.BO.Services.Implementations.VehiclesApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<LiaNcc.BO.Services.Interfaces.IServicesApiClient, LiaNcc.BO.Services.Implementations.ServicesApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<LiaNcc.BO.Services.Interfaces.IMediaAssetsApiClient, LiaNcc.BO.Services.Implementations.MediaAssetsApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<LiaNcc.BO.Services.Interfaces.ISitePagesApiClient, LiaNcc.BO.Services.Implementations.SitePagesApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});
builder.Services.AddHttpClient<LiaNcc.BO.Services.Interfaces.ICompanyApiClient, LiaNcc.BO.Services.Implementations.CompanyApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});
builder.Services.AddHttpClient<LiaNcc.BO.Services.Interfaces.IPartnersApiClient, LiaNcc.BO.Services.Implementations.PartnersApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});
builder.Services.AddHttpClient<LiaNcc.BO.Services.Interfaces.IUsersApiClient, LiaNcc.BO.Services.Implementations.UsersApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});
builder.Services.AddHttpClient<LiaNcc.BO.Services.Interfaces.IBookingsApiClient, LiaNcc.BO.Services.Implementations.BookingsApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});
builder.Services.AddHttpClient<LiaNcc.BO.Services.Interfaces.IContactMessagesApiClient, LiaNcc.BO.Services.Implementations.ContactMessagesApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});
builder.Services.AddHttpClient<LiaNcc.BO.Services.Interfaces.ILanguagesApiClient, LiaNcc.BO.Services.Implementations.LanguagesApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
