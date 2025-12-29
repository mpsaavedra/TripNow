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
        await _dbSet.AddAsync(entity, cancellation);
    }

    public Task Update(T entity)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public Task Remove(T entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellation = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellation);
    }
}
