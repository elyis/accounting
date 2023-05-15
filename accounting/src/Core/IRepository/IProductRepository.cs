using accounting.src.Entity.Request;
using accounting.src.Models;

namespace accounting.src.Core.IRepository
{
    public interface IProductRepository
    {
        Task<Product?> GetAsync(Guid id);
        Task<Product?> AddAsync(CreateProductBody body);
        Task<Product?> UpdateImage(Guid id, string image);
        IEnumerable<Product> GetAll(string? pattern);
        Task<Product?> UpdateAsync(CreateProductBody body, Guid productId);
    }
}
