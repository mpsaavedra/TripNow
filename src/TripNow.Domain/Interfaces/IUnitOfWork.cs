using System;
using System.Collections.Generic;
using System.Text;

namespace TripNow.Domain.Interfaces;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
