using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using WeatherApp.Clients;
using WeatherApp.Data;
using WeatherApp.Data.Entity;
using WeatherApp.Features.Weather.Commands;

namespace WeatherApp.UnitTests;

public class DeleteWeatherCommandHandlerTests
{
    private readonly WeatherAppDbContext _context;
    private readonly DeleteWeather.DeleteWeatherCommandHandler _handler;

    public DeleteWeatherCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<WeatherAppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new WeatherAppDbContext(options);
        _handler = new DeleteWeather.DeleteWeatherCommandHandler(_context);
    }
    
    [Fact]
    public async Task Handle_ValidCoordinates_DeletesWeatherRecord()
    {
        // Arrange
        var weather = new Weather
        {
            Latitude = 45.0,
            Longitude = 90.0,
            Name = "Sample Location",
            Description = "Clear Sky",
            Country = "Sample Country",
            TempMin = 10.0,
            TempMax = 20.0,
            Temp = 15.0,
            FeelsLike = 12.0,
            Pressure = 1010,
            Humidity = 50,
            WindSpeed = 5.0,
            Sunrise = 1627550400,
            Sunset = 1627593600
        };

        _context.Weather.Add(weather);
        await _context.SaveChangesAsync();

        var command = new DeleteWeather.DeleteWeatherCommand(45.0, 90.0);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedWeather = await _context.Weather.FindAsync(45.0, 90.0);
        Assert.Null(deletedWeather);
        Assert.Equal(Unit.Value, result);
    }
    
    
    [Fact]
    public async Task Handle_InvalidCoordinates_ThrowsArgumentException()
    {
        // Arrange
        var command = new DeleteWeather.DeleteWeatherCommand(-100.0, 200.0); // Invalid coordinates

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Contains("Latitude must be between -90 and 90", exception.Message);
    }

    [Fact]
    public async Task Handle_NonExistentWeatherRecord_ThrowsKeyNotFoundException()
    {
        // Arrange
        var command = new DeleteWeather.DeleteWeatherCommand(30.0, 40.0);
        // Non-existent record

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Contains($"Location not found for coordinates Latitude {command.Latitude}, Longitude {command.Longitude}.", exception.Message);
    }
}