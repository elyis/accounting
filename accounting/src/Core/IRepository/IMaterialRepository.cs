using accounting.src.Entity.Request;
using accounting.src.Models;

namespace accounting.src.Core.IRepository
{
    public interface IMaterialRepository
    {
        Task<Material?> GetAsync(Guid id);
        IEnumerable<Material> GetAll(string? name);
        Task<Material?> UpdateImage(Guid id, string image);
        Task<Material> AddAsync(CreateMaterialBody body);
        Task<Material?> Update(MaterialUpdateBody body);
        Task<bool> DeleteAsync(Guid id);
        Task<Material?> UpdateAsync(CreateMaterialBody body, Guid materialId);
    }
}
