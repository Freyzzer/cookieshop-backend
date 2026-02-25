using CookieShop.Domain.Const;
using System.ComponentModel.DataAnnotations;

namespace CookieShop.Domain.Models
{
    public class Product
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public int Stock { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool isNew { get; set; } = false;

        public Categories category { get; set; } = Categories.Clasico;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
