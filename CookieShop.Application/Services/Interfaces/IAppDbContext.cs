using CookieShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CookieShop.Application.Services.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<Order> Orders { get; }
        DbSet<Product> Products { get; }
        DbSet<OrderItem> OrderItems { get; }
        DbSet<Payment> Payments { get; }
        DbSet<WebhookEvent> WebhookEvents { get; }
        
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
