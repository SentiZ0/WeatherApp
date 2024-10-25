using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WeatherApp.Clients;
using WeatherApp.Data;
using WeatherApp.Extensions;
using WeatherApp.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IWeatherClient, OpenWeatherClient>();
builder.Services.AddDbContext<WeatherAppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WeatherApp")));

builder.Services.Configure<OpenWeatherApiOptions>(
    builder.Configuration.GetSection("OpenWeatherApi"));

builder.Services.AddHttpClient("weatherapi", (serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<OpenWeatherApiOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
});

builder.Services.AddMediatR(
    cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();
app.MapControllers();
app.UseHttpsRedirection();

app.Run();
