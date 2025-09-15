using Microsoft.EntityFrameworkCore;
using MinimalApiBackend.Models;

namespace MinimalApiBackend.Data;

public class ProductContext : DbContext
{
    public ProductContext(DbContextOptions<ProductContext> options) : base(options) { }
    public DbSet<Product> Products => Set<Product>();
}