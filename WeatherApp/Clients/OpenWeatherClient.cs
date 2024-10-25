using System.Text.Json;
using Microsoft.Extensions.Options;
using WeatherApp.Models;
using WeatherApp.Options;

namespace WeatherApp.Clients;

public interface IWeatherClient
{
    Task<WeatherResponse?> GetCurrentWeatherByCoordinates(double longitude, double latitude);
}

public class OpenWeatherClient : IWeatherClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    
    private IOptions<OpenWeatherApiOptions> _options;

    public OpenWeatherClient(IHttpClientFactory httpClientFactory, IOptions<OpenWeatherApiOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _options = options;
    }

    public async Task<WeatherResponse?> GetCurrentWeatherByCoordinates(double longitude, double latitude)
    {
        var client = _httpClientFactory.CreateClient("weatherapi");
        
        return 
            await client.GetFromJsonAsync<WeatherResponse>(
                $"weather?lat={latitude}&lon={longitude}&appid={_options.Value.ApiKey}&units=metric");
    }
}


