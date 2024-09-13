using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kata.Wallet.Dtos
{
    public class TransferRequestDto
    {
        public int SourceWalletId { get; set; }  
        public int TargetWalletId { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "The transfer amount must be greater than 0.")]
        public decimal Amount { get; set; }      
    }
}
