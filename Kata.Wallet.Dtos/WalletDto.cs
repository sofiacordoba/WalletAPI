using System.ComponentModel.DataAnnotations;
using Kata.Wallet.Domain;
using Kata.Wallet.Api.Validators;

namespace Kata.Wallet.Dtos;

public class WalletDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "The user document is required.")]
    [StringLength(50, ErrorMessage = "The user document cannot be longer than 50 characters.")]
    public string UserDocument { get; set; }

    [Required(ErrorMessage = "The user name is required.")]
    [StringLength(100, ErrorMessage = "The user name cannot be longer than 100 characters.")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "The currency is required.")]
    [CurrencyValidation]
    public string Currency { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "The balance must be greater than or equal to 0.")]
    public decimal Balance { get; set; }
}
