using System.ComponentModel.DataAnnotations;

namespace Kata.Wallet.Api.Validators
{
    public class CurrencyValidationAttribute : ValidationAttribute
    {
        private readonly string[] _validCurrencies = new[] { "USD", "EUR", "ARS" };
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var currencyValue = value?.ToString()?.ToUpper();

            if (currencyValue == null || !_validCurrencies.Contains(currencyValue))
            {
                return new ValidationResult($"The currency must be one of the following: {string.Join(", ", _validCurrencies)}.");
            }

            return ValidationResult.Success;
        }
    }
}
