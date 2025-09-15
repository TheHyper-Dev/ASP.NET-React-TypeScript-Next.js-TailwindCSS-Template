using Microsoft.EntityFrameworkCore;
using MinimalApiBackend.Data;
using MinimalApiBackend.Models;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<ProductContext>(opt =>
    opt.UseInMemoryDatabase("ProductDb"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Minimal API", Version = "v1" });
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

var app = builder.Build(); // ✅ Only once

// Use middleware
app.UseCors("AllowFrontend");

// Seed initial data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProductContext>();
    db.Products.AddRange(new List<Product>
    {
        new Product { Id = 1, Name = "Laptop", Price = 1000 },
        new Product { Id = 2, Name = "Mouse", Price = 20 }
    });
    db.SaveChanges();
}

// Configure endpoints
app.MapGet("/products", async (ProductContext db) =>
    await db.Products.ToListAsync());

app.MapGet("/products/{id}", async (int id, ProductContext db) =>
    await db.Products.FindAsync(id) is Product product
        ? Results.Ok(product)
        : Results.NotFound());

app.MapPost("/products", async (Product product, ProductContext db) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/products/{product.Id}", product);
});

app.MapPut("/products/{id}", async (int id, Product updatedProduct, ProductContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product is null) return Results.NotFound();
    product.Name = updatedProduct.Name;
    product.Price = updatedProduct.Price;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/products/{id}", async (int id, ProductContext db) =>
{
    if (await db.Products.FindAsync(id) is Product product)
    {
        db.Products.Remove(product);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    return Results.NotFound();
});

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minimal API V1");
});


app.Run();
