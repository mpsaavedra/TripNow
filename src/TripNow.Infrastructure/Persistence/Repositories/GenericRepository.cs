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

    public GenericRepository(TripNowDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
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
        try
        {
            await _context.Database.BeginTransactionAsync(cancellation);
            await _dbSet.AddAsync(entity, cancellation);
            await _context.Database.CommitTransactionAsync(cancellation);
        }
        catch
        {
            await _context.Database.RollbackTransactionAsync(cancellation);
        }
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellation = default)
    {
        try
        { 
            await _context.Database.BeginTransactionAsync(cancellation);
            _dbSet.Update(entity);
            await _context.Database.CommitTransactionAsync(cancellation);
        }
        catch
        {
            await _context.Database.RollbackTransactionAsync(cancellation);
        }

    }

    public async Task RemoveAsync(T entity, CancellationToken cancellation = default)
    {
        try
        {
            await _context.Database.BeginTransactionAsync(cancellation);
            _dbSet.Remove(entity);
            await _context.Database.CommitTransactionAsync(cancellation);
        }
        catch
        {
            await _context.Database.RollbackTransactionAsync(cancellation);
        }
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellation = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellation);
    }
}
