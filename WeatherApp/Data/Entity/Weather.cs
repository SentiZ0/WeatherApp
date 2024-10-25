namespace WeatherApp.Data.Entity;

public class Weather
{
    public double Longitude { get; set; }
    
    public double Latitude { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public string Country { get; set; }
    
    public double TempMin { get; set; }
    
    public double TempMax { get; set; }
    
    public double Temp { get; set; }
    
    public double FeelsLike { get; set; }
    
    public int Pressure { get; set; }
    
    public int Humidity { get; set; }
    
    public double WindSpeed { get; set; }
    
    public long Sunrise { get; set; }
    
    public long Sunset { get; set; }
}