using System.ComponentModel.DataAnnotations;

namespace ORBIT9000.Core.Attributes.UI
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MaxValueAttribute(int maxValue) : ValidationAttribute
    {
        public int MaxValue { get; } = maxValue;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext), "ValidationContext cannot be null.");
            }

            if (value is int intValue && intValue > this.MaxValue)
            {
                return new ValidationResult(
                    this.ErrorMessage ?? $"{validationContext.DisplayName} must be less than or equal to {this.MaxValue}.");
            }

            return ValidationResult.Success;
        }
    }
}