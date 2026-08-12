using System;
using System.Collections.Generic;
using System.Text;

namespace Elysium.Domain.Interfaces;

public interface IUnitOfWork
{
    Task StartTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
