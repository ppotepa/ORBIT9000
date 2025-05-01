using System.ComponentModel.DataAnnotations;

namespace Orbit9000.EngineTerminal
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MaxValueAttribute : ValidationAttribute
    {
        private readonly int _maxValue;

        public MaxValueAttribute(int maxValue)
        {
            _maxValue = maxValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (validationContext == null)
            {
               throw new ArgumentNullException(nameof(validationContext), "ValidationContext cannot be null.");
            }

            if (value is int intValue && intValue > _maxValue)
            {
                return new ValidationResult(
                    ErrorMessage ?? $"{validationContext.DisplayName} must be less than or equal to {_maxValue}.");
            }

            return ValidationResult.Success;
        }
    }
}