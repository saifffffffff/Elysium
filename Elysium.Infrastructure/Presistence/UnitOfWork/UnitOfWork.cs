using Elysium.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;
namespace Elysium.Infrastructure.Presistence.UnitOfWork;

public class UnitOfWork(AppDbContext context) : IUnitOfWork , IDisposable
{

    private IDbContextTransaction? _transaction;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if ( _transaction != null )
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        context.Dispose();
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null )
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task StartTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await context.Database.BeginTransactionAsync(cancellationToken);
    }
}
