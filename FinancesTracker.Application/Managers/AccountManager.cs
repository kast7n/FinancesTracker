using AutoMapper;
using FinancesTracker.Application.DTOs;
using FinancesTracker.Domain.Entities;
using FinancesTracker.Domain.Repositories;

namespace FinancesTracker.Application.Managers
{
    public class AccountManager(IAccountRepository repository, IMapper mapper) : BaseManager<Account, AccountDto>(repository, mapper)
    {
        public async Task<IEnumerable<AccountDto>> GetAccountsFilteredAsync(AccountFilterDto filter)
        {
            var query = _repository.Query();
            if (!string.IsNullOrWhiteSpace(filter.Name))
                query = query.Where(a => a.Name.Contains(filter.Name));
            if (!string.IsNullOrWhiteSpace(filter.Currency))
                query = query.Where(a => a.Currency == filter.Currency);
            if (filter.MinBalance.HasValue)
                query = query.Where(a => a.Balance >= filter.MinBalance.Value);
            if (filter.MaxBalance.HasValue)
                query = query.Where(a => a.Balance <= filter.MaxBalance.Value);
            var result = await Task.FromResult(query.ToList());
            return _mapper.Map<IEnumerable<AccountDto>>(result);
        }
    }
}
