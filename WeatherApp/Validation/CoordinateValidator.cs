using System.ComponentModel.DataAnnotations;

namespace WeatherApp.Validation;

public class CoordinateValidator
{
    public static ValidationResult Validate(double latitude, double longitude)
    {
        var errors = new List<string>();
        
        if (latitude < -90 || latitude > 90)
        {
            errors.Add("Latitude must be between -90 and 90");
        }

        if (longitude < -180 || longitude > 180)
        {
            errors.Add("Longitude must be between -180 and 180");
        }

        return errors.Count == 0 ? ValidationResult.Success! : new ValidationResult(string.Join(", ", errors));
    }
}