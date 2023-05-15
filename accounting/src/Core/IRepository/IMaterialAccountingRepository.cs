using accounting.src.Entity.Request;
using accounting.src.Models;

namespace accounting.src.Core.IRepository
{
    public interface IMaterialAccountingRepository
    {
        Task<MaterialAccounting?> GetAsync(Guid id);
        Task<MaterialAccounting?> AddAsync(CreateMaterialAccounting body, User author, TypeOfGoodsAccounting type);
        Task UploadImages(IFormFileCollection files, Guid id);
        IEnumerable<MaterialAccounting> GetAll(TypeOfGoodsAccounting type);
    }
}
