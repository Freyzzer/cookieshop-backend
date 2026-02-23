namespace CookieShop.Application.Services.Interfaces
{
    public interface IUnitOfWork
    {
        IProductRepository Products { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
