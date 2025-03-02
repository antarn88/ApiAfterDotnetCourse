using ApiAfterDotnetCourse.Bll.Dtos;
using ApiAfterDotnetCourse.Data.Entities;

namespace ApiAfterDotnetCourse.Bll.Interfaces;

public interface IProductService
{
    Task<List<Product>> GetAllProductsAsync();
    Task<Product> CreateProductAsync(CreateProductDto dto);
}
