using Microsoft.EntityFrameworkCore;
using WeatherApp.Data;
using WeatherApp.Data.Entity;
using WeatherApp.Features.Weather.Queries;

namespace WeatherApp.UnitTests.Queries;

public class GetWeatherByCoordinatesHandlerTests
{
    private readonly WeatherAppDbContext _context;
    private readonly GetWeatherByCoordinates.GetWeatherByCoordinatesHandler _handler;

    public GetWeatherByCoordinatesHandlerTests()
    {
        var options = new DbContextOptionsBuilder<WeatherAppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new WeatherAppDbContext(options);
        _handler = new GetWeatherByCoordinates.GetWeatherByCoordinatesHandler(_context);
    }

    [Fact]
    public async Task Handle_ValidCoordinates_ReturnsWeatherLocationsDto()
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

        var query = new GetWeatherByCoordinates.GetWeatherByCoordinatesQuery(45.0, 90.0);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(weather.Latitude, result.Latitude);
        Assert.Equal(weather.Longitude, result.Longitude);
        Assert.Equal(weather.Name, result.Name);
        Assert.Equal(weather.Description, result.Description);
        Assert.Equal(weather.Country, result.Country);
        Assert.Equal(weather.TempMin, result.TempMin);
        Assert.Equal(weather.TempMax, result.TempMax);
        Assert.Equal(weather.Temp, result.Temp);
        Assert.Equal(weather.FeelsLike, result.FeelsLike);
        Assert.Equal(weather.Pressure, result.Pressure);
        Assert.Equal(weather.Humidity, result.Humidity);
        Assert.Equal(weather.WindSpeed, result.WindSpeed);
        Assert.Equal(weather.Sunrise, result.Sunrise);
        Assert.Equal(weather.Sunset, result.Sunset);
    }

    [Fact]
    public async Task Handle_InvalidCoordinates_ThrowsArgumentException()
    {
        // Arrange
        var query = new GetWeatherByCoordinates.GetWeatherByCoordinatesQuery(-100.0, 200.0); // Invalid coordinates

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(query, CancellationToken.None));
        Assert.Contains("Latitude must be between -90 and 90", exception.Message);
    }

    [Fact]
    public async Task Handle_NonExistentWeatherRecord_ThrowsKeyNotFoundException()
    {
        // Arrange
        var query = new GetWeatherByCoordinates.GetWeatherByCoordinatesQuery(30.0, 40.0); // Non-existent record

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(query, CancellationToken.None));
        Assert.Contains("Location not found for coordinates", exception.Message);
    }
}