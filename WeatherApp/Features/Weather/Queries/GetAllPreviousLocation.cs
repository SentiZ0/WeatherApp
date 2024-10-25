using MediatR;
using Microsoft.EntityFrameworkCore;
using WeatherApp.Data;

namespace WeatherApp.Features.Weather.Queries;

public class GetAllPreviousLocation
{
    public record GetAllPreviousLocationQuery() : IRequest<List<WeatherLocationsDto>>;
    
    public class GetAllPreviousLocationHandler : IRequestHandler<GetAllPreviousLocationQuery, List<WeatherLocationsDto>>
    {
        private readonly WeatherAppDbContext _context;

        public GetAllPreviousLocationHandler(WeatherAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<WeatherLocationsDto>> Handle(GetAllPreviousLocationQuery request, CancellationToken cancellationToken)
        {
            var locations = await _context.Weather
                .Select(x => new WeatherLocationsDto(x.Latitude, x.Longitude))
                .ToListAsync(cancellationToken);
            
            return locations;
        }
    }

    public record WeatherLocationsDto(double Latitude, double Longitude);
}

