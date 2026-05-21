var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7001/api/";

builder.Services.AddHttpClient("LiaNccApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).ConfigurePrimaryHttpMessageHandler((sp) =>
{
    var handler = new HttpClientHandler();
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    if (env.IsDevelopment())
    {
        handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    }
    return handler;
});

builder.Services.AddScoped<LiaNcc.FE.Helpers.IMediaUrlBuilder, LiaNcc.FE.Helpers.MediaUrlBuilder>();

builder.Services.AddScoped<LiaNcc.FE.Services.Interfaces.IServicesApiClient, LiaNcc.FE.Services.Implementations.ServicesApiClient>(sp =>
    new LiaNcc.FE.Services.Implementations.ServicesApiClient(sp.GetRequiredService<IHttpClientFactory>().CreateClient("LiaNccApi")));

builder.Services.AddScoped<LiaNcc.FE.Services.Interfaces.IVehiclesApiClient, LiaNcc.FE.Services.Implementations.VehiclesApiClient>(sp =>
    new LiaNcc.FE.Services.Implementations.VehiclesApiClient(sp.GetRequiredService<IHttpClientFactory>().CreateClient("LiaNccApi")));

builder.Services.AddScoped<LiaNcc.FE.Services.Interfaces.IToursApiClient, LiaNcc.FE.Services.Implementations.ToursApiClient>(sp =>
    new LiaNcc.FE.Services.Implementations.ToursApiClient(sp.GetRequiredService<IHttpClientFactory>().CreateClient("LiaNccApi")));

builder.Services.AddScoped<LiaNcc.FE.Services.Interfaces.IPartnersApiClient, LiaNcc.FE.Services.Implementations.PartnersApiClient>(sp =>
    new LiaNcc.FE.Services.Implementations.PartnersApiClient(sp.GetRequiredService<IHttpClientFactory>().CreateClient("LiaNccApi")));

builder.Services.AddScoped<LiaNcc.FE.Services.Interfaces.ICompanyApiClient, LiaNcc.FE.Services.Implementations.CompanyApiClient>(sp =>
    new LiaNcc.FE.Services.Implementations.CompanyApiClient(sp.GetRequiredService<IHttpClientFactory>().CreateClient("LiaNccApi")));

builder.Services.AddScoped<LiaNcc.FE.Services.Interfaces.ISitePagesApiClient, LiaNcc.FE.Services.Implementations.SitePagesApiClient>(sp =>
    new LiaNcc.FE.Services.Implementations.SitePagesApiClient(sp.GetRequiredService<IHttpClientFactory>().CreateClient("LiaNccApi")));

builder.Services.AddScoped<LiaNcc.FE.Services.Interfaces.IContactMessagesApiClient, LiaNcc.FE.Services.Implementations.ContactMessagesApiClient>(sp =>
    new LiaNcc.FE.Services.Implementations.ContactMessagesApiClient(sp.GetRequiredService<IHttpClientFactory>().CreateClient("LiaNccApi")));

builder.Services.AddScoped<LiaNcc.FE.Services.Interfaces.IBookingsApiClient, LiaNcc.FE.Services.Implementations.BookingsApiClient>(sp =>
    new LiaNcc.FE.Services.Implementations.BookingsApiClient(sp.GetRequiredService<IHttpClientFactory>().CreateClient("LiaNccApi")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

var supportedCultures = new[] { "it", "en" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
