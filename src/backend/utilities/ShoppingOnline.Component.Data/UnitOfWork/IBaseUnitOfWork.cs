namespace ShoppingOnline.Component.Data.UnitOfWork;

public interface IBaseUnitOfWork : IDisposable
{
    bool Commit();
    Task<bool> CommitAsync();
}