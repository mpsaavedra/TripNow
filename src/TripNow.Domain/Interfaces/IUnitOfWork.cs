using System;
using System.Collections.Generic;
using System.Text;

namespace TripNow.Domain.Interfaces;

public interface IUnitOfWork
{
    Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> operation, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
