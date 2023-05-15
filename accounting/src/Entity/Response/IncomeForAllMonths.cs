using accounting.src.Models;

namespace accounting.src.Entity.Response
{
    public class IncomeForAllMonths
    {
        public List<IncomePerDate> EarningThisMonth { get; set; }
        public List<IncomePerDate> MonthlyEarnings { get; set; }
        public float SoldToday { get; set; }
        public float EarningForToday { get; set; } 
        public Currency Currency { get; set; }
    }
}
