using CookieShop.Application.Services.Interfaces;
using CookieShop.Domain.Models;

namespace CookieShop.Application.Services
{
    public class ProductServices: IProductService
    {
        private readonly IProductRepository _repository;

       public ProductServices(IProductRepository repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }
        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }
        public async Task<Product> AddAsync(Product product)
        {
            if(product.Price < 0)
            {
                throw new ArgumentException("Price cannot be negative");
            }

            await _repository.AddAsync(product);
            return product;
        }
        public async Task<bool> UpdateAsync(Guid id,Product product)
        {
            var existingProduct = await _repository.GetByIdAsync(id);
            if (existingProduct == null)
            {
                return false;
                throw new KeyNotFoundException("Product not found");
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;

            await _repository.UpdateAsync(existingProduct);
            return true;
        }
        public async Task<bool> DeleteAsync(Guid id)
        {
            var existingProduct = await _repository.GetByIdAsync(id);
            if (existingProduct == null)
            {
                return false;
                throw new KeyNotFoundException("Product not found");
            }

            await _repository.DeleteAsync(id);
            return true;
        }
    }
}
