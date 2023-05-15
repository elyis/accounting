using accounting.src.Entity.Request;
using accounting.src.Entity.Response;
using accounting.src.Models;

namespace accounting.src.Core.IRepository
{
    public interface IProductAccountingRepository
    {
        Task<ProductAccounting?> GetAsync(Guid id);
        Task<ProductAccounting?> AddAsync(CreateSaleBody body, User author, TypeOfGoodsAccounting type);
        Task UploadImages(IFormFileCollection files, Guid id);
        IEnumerable<ProductAccounting> GetAll(Guid? userId = null);
        IncomeForAllMonths GetProfitOfTheMonths(Guid userId);
        Task<ProductAccounting?> UpdateAsync(CreateSaleBody body, Guid saleId);
        ManagerAnalyticsBody GetManagerAnalytics(IEnumerable<User> sellers);
    }
}
