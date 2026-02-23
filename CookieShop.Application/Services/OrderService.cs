using CookieShop.Application.DTOs;
using CookieShop.Application.Services.Interfaces;
using CookieShop.Domain.Const;
using CookieShop.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CookieShop.Application.Services
{
    public class OrderService: IOrderService
    {
        private readonly IAppDbContext _context;

        public OrderService(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto)
        {
            if (!dto.Items.Any())
                throw new Exception("La orden debe tener al menos un producto");

            using var transaction = await _context.BeginTransactionAsync();

            var order = new Order
            {
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                CustomerName = dto.CustomerName,
                CustomerEmail = dto.CustomerEmail,
                CustomerPhone = dto.CustomerPhone,
                ShippingAddress = dto.ShippingAddress,
                OrderItems = new List<OrderItem>()
            };

            decimal total = 0;

            foreach (var item in dto.Items)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == item.ProductId);

                if (product == null)
                    throw new Exception($"Producto {item.ProductId} no existe");

                if (product.Stock < item.Quantity)
                    throw new Exception($"Stock insuficiente para {product.Name}. Disponible: {product.Stock}");

                var subtotal = product.Price * item.Quantity;
                total += subtotal;

                product.Stock -= item.Quantity;

                order.OrderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price,
                    Subtotal = subtotal
                });
            }

            order.TotalAmount = total;

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return new OrderResponseDto
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                CustomerEmail = order.CustomerEmail,
                CustomerPhone = order.CustomerPhone,
                ShippingAddress = order.ShippingAddress,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Items = order.OrderItems.Select(i => new OrderItemResponseDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.UnitPrice
                }).ToList()
            };
        }
    }
}
