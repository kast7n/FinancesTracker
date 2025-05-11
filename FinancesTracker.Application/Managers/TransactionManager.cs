using AutoMapper;
using FinancesTracker.Application.DTOs;
using FinancesTracker.Domain.Entities;
using FinancesTracker.Domain.Repositories;

namespace FinancesTracker.Application.Managers
{
    public class TransactionManager : BaseManager<Transaction, TransactionDto>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;

        public TransactionManager(ITransactionRepository transactionRepository, IAccountRepository accountRepository, IMapper mapper)
            : base(transactionRepository, mapper)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }

        public override async Task AddAsync(TransactionDto dto)
        {
            var entity = _mapper.Map<Transaction>(dto);
            var account = await _accountRepository.GetByIdAsync(entity.AccountID);
            if (account == null)
                throw new Exception("Account not found");
            if (entity.Type == "Expense")
                account.Balance -= entity.Amount;
            else if (entity.Type == "Income")
                account.Balance += entity.Amount;
            else
                throw new Exception("Invalid transaction type");
            await _transactionRepository.AddAsync(entity);
            _accountRepository.Update(account);
            await _transactionRepository.SaveChangesAsync();
            await _accountRepository.SaveChangesAsync();
        }

        public override async Task UpdateAsync(TransactionDto dto)
        {
            // Fetch the tracked entity from the database
            var oldEntity = await _transactionRepository.GetByIdAsync(dto.TransactionID);
            if (oldEntity == null)
                throw new Exception("Transaction not found");
            var oldAccount = await _accountRepository.GetByIdAsync(oldEntity.AccountID);
            var newAccount = await _accountRepository.GetByIdAsync(dto.AccountID);
            if (oldAccount == null || newAccount == null)
                throw new Exception("Account not found");
            // Revert old transaction effect
            if (oldEntity.Type == "Expense")
                oldAccount.Balance += oldEntity.Amount;
            else if (oldEntity.Type == "Income")
                oldAccount.Balance -= oldEntity.Amount;
            // Apply new transaction effect
            if (dto.Type == "Expense")
                newAccount.Balance -= dto.Amount;
            else if (dto.Type == "Income")
                newAccount.Balance += dto.Amount;
            // Update the properties of the tracked entity
            oldEntity.Date = dto.Date;
            oldEntity.Type = dto.Type;
            oldEntity.Category = dto.Category;
            oldEntity.Amount = dto.Amount;
            oldEntity.AccountID = dto.AccountID;
            oldEntity.Notes = dto.Notes;
            _transactionRepository.Update(oldEntity);
            _accountRepository.Update(oldAccount);
            if (oldAccount.AccountID != newAccount.AccountID)
                _accountRepository.Update(newAccount);
            await _transactionRepository.SaveChangesAsync();
            await _accountRepository.SaveChangesAsync();
        }

    }
}
