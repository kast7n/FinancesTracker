using AutoMapper;
using FinancesTracker.Application.DTOs;
using FinancesTracker.Domain.Entities;

namespace FinancesTracker.Application.Managers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Account, AccountDto>().ReverseMap();
            CreateMap<Transaction, TransactionDto>().ReverseMap();

        }
    }
}
