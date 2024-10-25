using System.ComponentModel.DataAnnotations;
using MediatR;
using WeatherApp.Clients;
using WeatherApp.Data;
using WeatherApp.Validation;

namespace WeatherApp.Features.Weather.Commands;

public class CreateWeather
{
    public record CreateWeatherCommand(double Latitude, double Longitude) : IRequest<Unit>;

    public class CreateWeatherCommandHandler : IRequestHandler<CreateWeatherCommand, Unit>
    {
        private readonly WeatherAppDbContext _context;
        private readonly IWeatherClient _weatherClient;

        public CreateWeatherCommandHandler(WeatherAppDbContext context, IWeatherClient weatherClient)
        {
            _context = context;
            _weatherClient = weatherClient;
        }

        public async Task<Unit> Handle(CreateWeatherCommand command, CancellationToken cancellationToken)
        {
            var validationResult = CoordinateValidator.Validate(command.Latitude, command.Longitude);
            
            if (validationResult != ValidationResult.Success)
            {
                throw new ArgumentException(validationResult.ErrorMessage);
            }

            var weatherResponse = await _weatherClient.GetCurrentWeatherByCoordinates(command.Latitude, command.Longitude);

            if (weatherResponse is null)
            {
                throw new Exception("Failed to fetch weather data from the external API.");
            }

            var weather = new Data.Entity.Weather
            {
                Longitude = command.Longitude,
                Latitude = command.Latitude,
                Name = weatherResponse.Name,
                Description = weatherResponse.Weather[0].Description,
                Country = weatherResponse.Sys.Country,
                TempMin = weatherResponse.Main.Temp_Min,
                TempMax = weatherResponse.Main.Temp_Max,
                Temp = weatherResponse.Main.Temp,
                FeelsLike = weatherResponse.Main.Feels_Like,
                Pressure = weatherResponse.Main.Pressure,
                Humidity = weatherResponse.Main.Humidity,
                WindSpeed = weatherResponse.Wind.Speed,
                Sunrise = weatherResponse.Sys.Sunrise,
                Sunset = weatherResponse.Sys.Sunset
            };

            await _context.Weather.AddAsync(weather, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return default;
        }
    };
}