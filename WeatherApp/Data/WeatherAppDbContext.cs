using Microsoft.EntityFrameworkCore;
using WeatherApp.Data.Entity;

namespace WeatherApp.Data;

public class WeatherAppDbContext : DbContext
{
    public WeatherAppDbContext(DbContextOptions options) : base(options)
    {
    }
    
    public DbSet<Weather> Weather { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Weather>()
            .HasKey(w => new { w.Longitude, w.Latitude });
    }
}
