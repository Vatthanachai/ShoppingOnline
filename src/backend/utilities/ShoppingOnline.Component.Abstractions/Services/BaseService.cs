using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using ShoppingOnline.Component.Data.Context;
using ShoppingOnline.Component.Data.UnitOfWork;

namespace ShoppingOnline.Component.Abstractions.Services;

public class BaseService<TEntity, TDbContext, TUnitOfWork>(
    TDbContext context,
    TUnitOfWork unitOfWork,
    ILogger logger,
    IHttpContextAccessor httpContextAccessor)
    : IBaseService<TEntity>
    where TEntity : class
    where TDbContext : IBaseDbContext
    where TUnitOfWork : IBaseUnitOfWork
{
    private bool _disposed;

    protected TDbContext DbContext { get; } = context;
    protected TUnitOfWork UnitOfWork { get; } = unitOfWork;
    protected DbSet<TEntity> DbSet { get; } = context.Set<TEntity>();
    protected IHttpContextAccessor HttpContextAccessor { get; } = httpContextAccessor;
    protected ILogger Logger { get; } = logger;


    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            DbContext.Dispose();
        }

        _disposed = true;
    }

    ~BaseService()
    {
        Dispose(false);
    }
}