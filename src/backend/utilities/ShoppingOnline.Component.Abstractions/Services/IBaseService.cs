namespace ShoppingOnline.Component.Abstractions.Services;

public interface IBaseService<TEntity> : IDisposable
    where TEntity : class;