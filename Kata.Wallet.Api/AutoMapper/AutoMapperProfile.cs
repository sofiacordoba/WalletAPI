using AutoMapper;
using Kata.Wallet.Domain;
using Kata.Wallet.Dtos;

namespace Kata.Wallet.Api.AutoMapper;

// AutoMapper profile configuration to map between domain models and DTOs
public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Mapping configuration for Transaction <-> TransactionDto
        CreateMap<Domain.Transaction, TransactionDto>();
        CreateMap<TransactionDto, Domain.Transaction>();

        // Mapping configuration for Wallet <-> WalletDto
        CreateMap<Domain.Wallet, WalletDto>();
        CreateMap<WalletDto, Domain.Wallet>();

        // Reverse mappings for Transaction and Wallet
        CreateMap<Transaction, TransactionDto>().ReverseMap();
        CreateMap<Kata.Wallet.Domain.Wallet, WalletDto>().ReverseMap();
    }
}
