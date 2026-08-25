using ShoppingOnline.Component.Data.UnitOfWork;
using ShoppingOnline.Database.Context;

namespace ShoppingOnline.Database.UnitOfWork;

public interface IShoppingUnitOfWork : IBaseUnitOfWork
{
}

public class ShoppingUnitOfWork(IShoppingDbContext context, ILogger logger)
    : BaseUnitOfWork<IShoppingDbContext>(context, logger), IShoppingUnitOfWork
{
}