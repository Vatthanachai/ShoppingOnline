using Microsoft.EntityFrameworkCore;

namespace ShoppingOnline.Component.Data.Context;

public class BaseDbContext(DbContextOptions options) : DbContext(options), IBaseDbContext
{
    public Task<int> SaveChangesAsync()
    {
        return base.SaveChangesAsync();
    }
}