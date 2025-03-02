using ApiAfterDotnetCourse.Bll.Dtos;
using ApiAfterDotnetCourse.Bll.Interfaces;
using ApiAfterDotnetCourse.Data;
using ApiAfterDotnetCourse.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiAfterDotnetCourse.Bll.Services;

public class ProductService : IProductService
{
    private readonly ApiAfterDotnetCourseDBContext _context;

    public ProductService(ApiAfterDotnetCourseDBContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<Product> CreateProductAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Price = dto.Price,
            Description = dto.Description
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return product;
    }
}
