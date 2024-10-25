using MediatR;
using Microsoft.AspNetCore.Mvc;
using WeatherApp.Features.Weather.Commands;
using WeatherApp.Features.Weather.Queries;

namespace WeatherApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WeatherController : ControllerBase
{
    private readonly IMediator _mediator;

    public WeatherController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{latitude}/{longitude}")]
    public async Task<IActionResult> GetWeather(double latitude, double longitude)
    {
        var weather = await _mediator.Send(new GetWeatherByCoordinates.GetWeatherByCoordinatesQuery(latitude, longitude));

        return Ok(weather);
    }

    [HttpGet("locations")]
    public async Task<IActionResult> GetLocations()
    {
        var weather = await _mediator.Send(new GetAllPreviousLocation.GetAllPreviousLocationQuery());

        return Ok(weather);
    }

    [HttpPost]
    public async Task<IActionResult> AddLocation([FromBody] CreateWeather.CreateWeatherCommand command)
    {
        var weather = await _mediator.Send(command);

        return Ok(weather);
    }

    [HttpDelete("{latitude}/{longitude}")]
    public async Task<IActionResult> DeleteLocation(double latitude, double longitude)
    {
        var command = new DeleteWeather.DeleteWeatherCommand(latitude, longitude);
        var weather = await _mediator.Send(new DeleteWeather.DeleteWeatherCommand(latitude, longitude));

        return Ok(weather);
    }
}