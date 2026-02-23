using CookieShop.Domain.Models;

namespace CookieShop.Application.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(Guid id);
        Task<Product> AddAsync(Product product);
        Task<bool> UpdateAsync(Guid id, Product product);
        Task<bool> DeleteAsync(Guid id);
    }
}
