using System.ComponentModel.DataAnnotations;

namespace ContactManagerApp.ValidationAttributes
{
    public class BirthDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateOnly date)
            {
                var now = DateOnly.FromDateTime(DateTime.Now);

                if (date > now)
                {
                    return new ValidationResult("Date of birth cannot be in the future.");
                }

                if (date < now.AddYears(-120))
                {
                    return new ValidationResult("Date of birth is invalid (too old).");
                }
            }
            return ValidationResult.Success;
        }
    }
}
