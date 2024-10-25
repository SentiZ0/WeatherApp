using Microsoft.EntityFrameworkCore;
using WeatherApp.Data;
using WeatherApp.Data.Entity;
using WeatherApp.Features.Weather.Commands;
using WeatherApp.Features.Weather.Queries;

namespace WeatherApp.UnitTests.Queries;

public class GetAllPreviousLocationHandlerTests
{
    private readonly WeatherAppDbContext _context;
    private readonly GetAllPreviousLocation.GetAllPreviousLocationHandler _handler;

    public GetAllPreviousLocationHandlerTests()
    {
        var options = new DbContextOptionsBuilder<WeatherAppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new WeatherAppDbContext(options);
        _handler = new GetAllPreviousLocation.GetAllPreviousLocationHandler(_context);
    }

    [Fact]
    public async Task Handle_ReturnsAllPreviousLocations()
    {
        // Arrange
        var weather1 = new Weather { Latitude = 45.0, Longitude = 90.0, Country = "Poland", Description = "Cleary sky", 
            FeelsLike = 12.0, Humidity = 50, Name = "Sample Location", Pressure = 1010, Sunrise = 1627550400 };
        var weather2 = new Weather
        {
            Latitude = 30.0, Longitude = 60.0, Country = "Poland", Description = "Cleary sky", 
            FeelsLike = 12.0, Humidity = 50, Name = "Sample Location", Pressure = 1010, Sunrise = 1627550400
        };

        _context.Weather.AddRange(weather1, weather2);
        await _context.SaveChangesAsync();

        var query = new GetAllPreviousLocation.GetAllPreviousLocationQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.Latitude == 45.0 && r.Longitude == 90.0);
        Assert.Contains(result, r => r.Latitude == 30.0 && r.Longitude == 60.0);
    }

    [Fact]
    public async Task Handle_EmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetAllPreviousLocation.GetAllPreviousLocationQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}