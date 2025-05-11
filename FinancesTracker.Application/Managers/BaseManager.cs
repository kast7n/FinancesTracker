using AutoMapper;
using FinancesTracker.Domain.Repositories;

namespace FinancesTracker.Application.Managers
{
    public class BaseManager<TEntity, TDto>(IRepository<TEntity> repository, IMapper mapper)
        where TEntity : class
        where TDto : class
    {
        protected readonly IRepository<TEntity> _repository = repository;
        protected readonly IMapper _mapper = mapper;

        public virtual async Task<IEnumerable<TDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<TDto>>(entities);
        }

        public virtual async Task<TDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<TDto>(entity);
        }

        public virtual async Task AddAsync(TDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(TDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            _repository.Update(entity);
            await _repository.SaveChangesAsync();
        }

        public virtual async Task RemoveAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                _repository.Remove(entity);
                await _repository.SaveChangesAsync();
            }
        }
    }
}
