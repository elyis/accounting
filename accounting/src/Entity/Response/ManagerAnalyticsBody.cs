using accounting.src.Models;

namespace accounting.src.Entity.Response
{
    public class ManagerAnalyticsBody
    {
        public float SoldToday { get; set; }
        public Currency Currency { get; set; }

        public List<SellerIncomeBody> SalesStatisticsForToday { get; set; }
        public List<SellerIncomeBody> SalesStatisticsThisMonth { get; set; }
        
        public AmountOfProduct SellingProductToday { get; set; }

        public List<AmountOfProduct> ProductSalesForToday { get; set; }
        public List<AmountOfProduct> ProductSalesThisMonth { get; set; }
    }
}
