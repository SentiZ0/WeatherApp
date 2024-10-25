using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WeatherApp.Data;
using WeatherApp.Models;
using WeatherApp.Validation;

namespace WeatherApp.Features.Weather.Queries;

public class GetWeatherByCoordinates
{
    public record GetWeatherByCoordinatesQuery(double Latitude, double Longitude) : IRequest<WeatherLocationsDto>;
    
    public class GetWeatherByCoordinatesHandler : IRequestHandler<GetWeatherByCoordinatesQuery, WeatherLocationsDto>
    {
        private readonly WeatherAppDbContext _context;

        public GetWeatherByCoordinatesHandler(WeatherAppDbContext context)
        {
            _context = context;
        }

        public async Task<WeatherLocationsDto> Handle(GetWeatherByCoordinatesQuery query, CancellationToken cancellationToken)
        {
            var validationResult = CoordinateValidator.Validate(query.Latitude, query.Longitude);
            
            if (validationResult != ValidationResult.Success)
            {
                throw new ArgumentException(validationResult.ErrorMessage);
            }
            
            var weather = await _context.Weather
                .Where(x => x.Longitude == query.Longitude && x.Latitude == query.Latitude)
                .Select(x => new WeatherLocationsDto(
                    x.Longitude,
                    x.Latitude,
                    x.Name,
                    x.Description,
                    x.Country,
                    x.TempMin,
                    x.TempMax,
                    x.Temp,
                    x.FeelsLike,
                    x.Pressure,
                    x.Humidity,
                    x.WindSpeed,
                    x.Sunrise,
                    x.Sunset))
                .SingleOrDefaultAsync();

            if (weather is null)
            {
                throw new KeyNotFoundException($"Location not found for coordinates Latitude {query.Latitude}, Longitude {query.Longitude}.");
            }

            return weather;
        }
    }
    
    public record WeatherLocationsDto(
        double Longitude,
        double Latitude,
        string Name,
        string Description,
        string Country,
        double TempMin,
        double TempMax,
        double Temp,
        double FeelsLike,
        int Pressure,
        int Humidity,
        double WindSpeed,
        long Sunrise,
        long Sunset);
}