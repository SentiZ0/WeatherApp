using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WeatherApp.Data;
using WeatherApp.Validation;

namespace WeatherApp.Features.Weather.Commands;

public class DeleteWeather
{
    public record DeleteWeatherCommand(double Latitude, double Longitude) : IRequest<Unit>;
    
    public class DeleteWeatherCommandHandler : IRequestHandler<DeleteWeatherCommand, Unit>
    {
        private readonly WeatherAppDbContext _context;

        public DeleteWeatherCommandHandler(WeatherAppDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteWeatherCommand command, CancellationToken cancellationToken)
        {
            var validationResult = CoordinateValidator.Validate(command.Latitude, command.Longitude);
            
            if (validationResult != ValidationResult.Success)
            {
                throw new ArgumentException(validationResult.ErrorMessage);
            }
            
            var weather = await _context.Weather
                .Where(x => x.Longitude == command.Longitude && x.Latitude == command.Latitude)
                .FirstOrDefaultAsync(cancellationToken);

            if (weather is null)
            {
                throw new KeyNotFoundException($"Location not found for coordinates Latitude {command.Latitude}, Longitude {command.Longitude}.");
            }

            _context.Weather.Remove(weather);
            await _context.SaveChangesAsync(cancellationToken);
            
            return default;
        }
    }
}