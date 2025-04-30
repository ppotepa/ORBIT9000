using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Orbit9000.EngineTerminal
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RegexValidationAttribute : ValidationAttribute
    {
        private readonly Regex _regex;

        public RegexValidationAttribute(string pattern)
        {
            _regex = new Regex(pattern);
        }

        public RegexValidationAttribute(int max)
        {
            _regex = new Regex($@"^.{{0,{max}}}$");
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var s = value.ToString();
            if (!_regex.IsMatch(s))
                return new ValidationResult(
                    ErrorMessage ?? $"{validationContext.DisplayName} is not valid.");

            return ValidationResult.Success;
        }
    }
}