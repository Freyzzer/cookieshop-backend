using CookieShop.Application.Services.Interfaces;
using CookieShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace CookieShop.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        public IProductRepository Products { get; }

        public UnitOfWork(AppDbContext context, IProductRepository products)
        {
            _context = context;
            Products = products;
        }

        public async Task BeginTransactionAsync() =>
            _transaction = await _context.Database.BeginTransactionAsync();

        public async Task CommitAsync() => await _transaction!.CommitAsync();
        public async Task RollbackAsync() => await _transaction!.RollbackAsync();
        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();
    }
}
