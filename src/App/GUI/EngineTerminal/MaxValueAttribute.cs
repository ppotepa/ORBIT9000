using System.ComponentModel.DataAnnotations;

namespace Orbit9000.EngineTerminal
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MaxValueAttribute : ValidationAttribute
    {
        public int MaxValue { get; private set; }

        public MaxValueAttribute(int maxValue)
        {
            MaxValue = maxValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext), "ValidationContext cannot be null.");
            }

            if (value is int intValue && intValue > MaxValue)
            {
                return new ValidationResult(
                    ErrorMessage ?? $"{validationContext.DisplayName} must be less than or equal to {MaxValue}.");
            }

            return ValidationResult.Success;
        }
    }
}