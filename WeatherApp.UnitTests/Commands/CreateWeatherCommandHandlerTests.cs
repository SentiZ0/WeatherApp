using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using WeatherApp.Clients;
using WeatherApp.Data;
using WeatherApp.Features.Weather.Commands;
using WeatherApp.Models;

namespace WeatherApp.UnitTests;

public class CreateWeatherCommandHandlerTests
{
    private readonly WeatherAppDbContext _context;
    private readonly Mock<IWeatherClient> _mockWeatherClient;
    private readonly CreateWeather.CreateWeatherCommandHandler _handler;

    public CreateWeatherCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<WeatherAppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new WeatherAppDbContext(options);
        _mockWeatherClient = new Mock<IWeatherClient>();
        _handler = new CreateWeather.CreateWeatherCommandHandler(_context, _mockWeatherClient.Object);
    }

    [Fact]
    public async Task Handle_ValidCoordinates_CreatesWeatherRecord()
    {
        // Arrange
        var command = new CreateWeather.CreateWeatherCommand(45.0, 90.0);
        var mockWeatherResponse = new WeatherResponse
        {
            Name = "Sample Location",
            Weather = new List<Weather> { new Weather { Description = "Clear Sky" } },
            Main = new Main
            {
                Temp_Min = 10.0,
                Temp_Max = 20.0,
                Temp = 15.0,
                Feels_Like = 12.0,
                Pressure = 1010,
                Humidity = 50
            },
            Wind = new Wind { Speed = 5.0 },
            Sys = new Sys
            {
                Country = "Poland",
                Sunrise = 1627550400,
                Sunset = 1627593600
            }
        };

        _mockWeatherClient.Setup(client => client.GetCurrentWeatherByCoordinates(command.Latitude, command.Longitude))
            .ReturnsAsync(mockWeatherResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        var weather = await _context.Weather
            .Where(x => x.Latitude == command.Latitude && x.Longitude == command.Longitude)
            .FirstOrDefaultAsync();
            
        Assert.NotNull(weather);
        Assert.Equal(Unit.Value, result);
    }

    [Fact]
    public async Task Handle_InvalidCoordinates_ThrowsArgumentException()
    {
        // Arrange
        var command = new CreateWeather.CreateWeatherCommand(-100.0, 200.0); // Invalid coordinates

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Contains("Latitude must be between -90 and 90", exception.Message);
    }

    [Fact]
    public async Task Handle_WeatherClientReturnsNull_ThrowsException()
    {
        // Arrange
        var command = new CreateWeather.CreateWeatherCommand(45.0, 90.0);
        _mockWeatherClient.Setup(client => client.GetCurrentWeatherByCoordinates(command.Latitude, command.Longitude))
            .ReturnsAsync((WeatherResponse)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal("Failed to fetch weather data from the external API.", exception.Message);
    }
}