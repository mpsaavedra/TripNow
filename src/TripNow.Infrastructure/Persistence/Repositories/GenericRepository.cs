using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TripNow.Domain.Interfaces;

namespace TripNow.Infrastructure.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly TripNowDbContext _context;
    protected readonly DbSet<T> _dbSet;
    protected readonly IUnitOfWork _unitOfWork;

    public GenericRepository(TripNowDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _dbSet = context.Set<T>();
        _unitOfWork = unitOfWork;
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellation = default)
    {
        return await _dbSet.FindAsync(id, cancellation);
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellation = default)
    {
        return await _dbSet.ToListAsync(cancellation);
    }

    public async Task AddAsync(T entity, CancellationToken cancellation = default)
    {
        await _unitOfWork.ExecuteAsync(async () =>
    {
            await _dbSet.AddAsync(entity, cancellation);
            return true;
        }, cancellation);
    }

    public async Task Update(T entity)
    {
        await _unitOfWork.ExecuteAsync(async () =>
        {
            await Task.Run(() => _dbSet.Update(entity));
            return true;
        });
    }

    public async Task Remove(T entity)
    {
        await _unitOfWork.ExecuteAsync(async () =>
        {
            _dbSet.Remove(entity);
            return true;
        });
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellation = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellation);
    }
}
