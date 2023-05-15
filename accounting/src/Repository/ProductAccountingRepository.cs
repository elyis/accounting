using accounting.src.Core.IRepository;
using accounting.src.Data;
using accounting.src.Entity;
using accounting.src.Entity.Request;
using accounting.src.Entity.Response;
using accounting.src.Models;
using accounting.src.Utility;
using Microsoft.EntityFrameworkCore;

namespace accounting.src.Repository
{
    public class ProductAccountingRepository : IProductAccountingRepository
    {
        private readonly AppDbContext _context;

        private readonly List<(float, float)> _profitBasedPremiumCalculationTable = new ()
        {
            (0f, 0f),
            (1f, 500f ),
            (3000f, 600 ),
            (4000f, 800f ),

            (5000f, 1000f ),
            (6000f, 1200f ),
            (8000f, 1200f ),
            (10000f, 2000f ),

            (12000f,2500f ),
            (14000f, 3000f ),
            (16000f, 4000f ),
            (18000f, 4500f ),
            (20000f, 5000f ),

            (22000f,5500f ),
            (24000f, 6000f ),
            (26000f, 7000f ),
            (28000f, 7500f ),
            (30000f, 8000f ),

            (32000f, 9000f ),
            (34000f, 10000f ),
            (36000f, 11000f ),
            (38000f, 12000f ),
            (40000f, 13000f )
        };

        public ProductAccountingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductAccounting?> AddAsync(CreateSaleBody body, User author, TypeOfGoodsAccounting type)
        {
            var listOfAccountingProduct = new List<AmountOfAccountedProduct>();

            foreach(var amountOfProduct in body.Products)
            {
                var productId = Guid.Parse(amountOfProduct.Id);
                var product = await _context.Products
                    .Include(e => e.Materials)
                        .ThenInclude(e => e.Material)
                    .FirstAsync(e => e.Id == productId);

                foreach(var consumptionOfMaterialPerProduct in product.Materials)
                {
                    var material = consumptionOfMaterialPerProduct.Material;
                    var difference = material.Amount - amountOfProduct.Amount * consumptionOfMaterialPerProduct.Amount;

                    if(difference < 0)
                        return null;
                    else
                        material.Amount = difference;
                }

                var amountOfAccountedProduct = new AmountOfAccountedProduct
                {
                    Product = product,
                    Amount = (int)amountOfProduct.Amount
                };

                listOfAccountingProduct.Add(amountOfAccountedProduct);
            }

            var productAccounting = new ProductAccounting
            {
                AccountedProducts = listOfAccountingProduct,
                CreatedBy = author,
                CheckPrice = body.CheckPrice,
                Currency = Enum.GetName(typeof(Currency), body.Currency)!,
                Name = body.Name,
                Type = Enum.GetName(typeof(TypeOfGoodsAccounting), type)!,
            };

            var result = await _context.ProductAccountings.AddAsync(productAccounting);
            await _context.SaveChangesAsync();
            return result == null ? null : result.Entity;
        }

        public async Task<ProductAccounting?> UpdateAsync(CreateSaleBody body, Guid saleId)
        {
            var sale = await GetAsync(saleId);
            if (sale == null)
                return null;

            var listOfAccountingProduct = new List<AmountOfAccountedProduct>();
            foreach (var amountOfProduct in body.Products)
            {
                var productId = Guid.Parse(amountOfProduct.Id);
                var product = await _context.Products.FindAsync(productId);

                var amountOfAccountedProduct = new AmountOfAccountedProduct
                {
                    Product = product,
                    Amount = (int)amountOfProduct.Amount
                };

                listOfAccountingProduct.Add(amountOfAccountedProduct);
            }

            sale.Currency = Enum.GetName(typeof(Currency), body.Currency)!;
            sale.Name = body.Name;
            sale.CheckPrice = body.CheckPrice;
            sale.AccountedProducts = listOfAccountingProduct;
            await _context.SaveChangesAsync();
            return sale;
        }

        public IEnumerable<ProductAccounting> GetAll(Guid? userId)
        {
            if(userId != null)
                return _context.ProductAccountings
                    .Include(e => e.CreatedBy)
                    .Include(e => e.AccountedProducts)
                        .ThenInclude(e => e.Product)
                    .Where(e => e.CreatedBy.Id == userId);

            return _context.ProductAccountings
                    .Include(e => e.CreatedBy)
                    .Include(e => e.AccountedProducts)
                        .ThenInclude(e => e.Product);
        }

        private IEnumerable<ProductAccounting> GetAllByMonth(DateTime date, Guid? userId = null)
        {
            var currentMonth = date.Month;
            var currentYear = date.Year;
            if(userId != null)
                return _context.ProductAccountings
                .Include(e => e.CreatedBy)
                .Include(e => e.AccountedProducts).
                    ThenInclude(e => e.Product)
                .Where(e => e.CreatedAt.Month == currentMonth && 
                            e.CreatedAt.Year == currentYear && 
                            e.CreatedBy.Id == userId);


            return _context.ProductAccountings
                .Include(e => e.CreatedBy)
                .Include(e => e.AccountedProducts).
                    ThenInclude(e => e.Product)
                .Where(e => e.CreatedAt.Month == currentMonth && e.CreatedAt.Year == currentYear);
        }

        public IEnumerable<ProductAccounting> GetAllByDay(DateTime date, Guid? userId = null)
        {
            var dateonly = DateOnly.FromDateTime(date);

            if(userId != null)
                return _context.ProductAccountings
                .Include(e => e.CreatedBy)
                .Include(e => e.AccountedProducts).
                    ThenInclude(e => e.Product)
                .Where(e => DateOnly.FromDateTime(e.CreatedAt) == dateonly &&
                            e.CreatedBy.Id == userId);

            return _context.ProductAccountings
                .Include(e => e.CreatedBy)
                .Include(e => e.AccountedProducts).
                    ThenInclude(e => e.Product)
                .Where(e => DateOnly.FromDateTime(e.CreatedAt) == dateonly);
        }



        public async Task<ProductAccounting?> GetAsync(Guid id)
            => await _context.ProductAccountings
                    .Include(e => e.CreatedBy)
                    .Include(e => e.AccountedProducts)
                        .ThenInclude(e => e.Product)
                    .FirstOrDefaultAsync(e => e.Id == id);

        private float CalculateSalaryForIncome(float income)
        {
            var topBorder = _profitBasedPremiumCalculationTable.First(e => e.Item1 > income);
            var indexOfTopBorder = _profitBasedPremiumCalculationTable.IndexOf(topBorder);
            if (indexOfTopBorder == -1)
                return _profitBasedPremiumCalculationTable.First().Item2;

            var salary = _profitBasedPremiumCalculationTable[indexOfTopBorder - 1].Item2;
            return salary;
        }

        public IncomeForAllMonths? GetProfitOfTheMonths(Guid userId)
        {
            var listOfProfitByMonths = new List<IncomePerDate>();
            var listOfProfitByCurrentMonth = new List<IncomePerDate>();

            var currentDate = DateTime.UtcNow;
            var firstSale = _context.ProductAccountings
                    .Include(e => e.CreatedBy)
                    .OrderBy(e => e.CreatedAt)
                    .FirstOrDefault(e => e.CreatedBy.Id == userId);
            if (firstSale == null)
                return null;

            
            var salesByCurrentMonth = GetAllByMonth(currentDate, userId).GroupBy(e => e.CreatedAt.Day).OrderBy(e => e.Key).ToList();

            for(int currentDay = 1, currentSalesByDay = 0; currentDay <= DateTime.UtcNow.Day; currentDay++)
            {

                var incomePerDate = new IncomePerDate
                {
                    Date = new DateTime(currentDate.Year, currentDate.Month, currentDay).ToString("u").Replace(" ", "T"),
                };
                if (currentSalesByDay < salesByCurrentMonth.Count && salesByCurrentMonth[currentSalesByDay].Key == currentDay)
                {
                    incomePerDate.Income = CalculateSalaryForIncome(salesByCurrentMonth[currentSalesByDay].Sum(e => e.CheckPrice));
                    currentSalesByDay++;    
                }
                else
                    incomePerDate.Income = 0;

                listOfProfitByCurrentMonth.Add(incomePerDate);
            }

            for (var currentMonth = 1;
                currentMonth <= DateTime.UtcNow.Month;
                currentMonth++)
            {
                var temp = new DateTime(currentDate.Year, currentMonth, 1);
                var sales = GetAllByMonth(new DateTime(currentDate.Year, currentMonth, 1), userId)
                    .GroupBy(e => e.CreatedAt.Day)
                    .OrderBy(e => e.Key)
                    .ToList();

                var salaryByMonth = 0f;
                foreach(var sale in sales)
                    salaryByMonth += CalculateSalaryForIncome(sale.Sum(e => e.CheckPrice));
                

                var profitByMonth = new IncomePerDate
                {
                    Date = temp.ToString("u").Replace(" ", "T"),
                    Income = salaryByMonth,
                };
                listOfProfitByMonths.Add(profitByMonth);
            }

            var incomeByDay = GetAllByDay(currentDate, userId);
            var soldToday = incomeByDay.Sum(e => e.CheckPrice);

            var incomeForAllMonths = new IncomeForAllMonths
            {
                EarningThisMonth = listOfProfitByCurrentMonth,
                MonthlyEarnings = listOfProfitByMonths,
                Currency = Currency.Rub,
                SoldToday = soldToday,
                EarningForToday = CalculateSalaryForIncome(soldToday)
            };

            return incomeForAllMonths;
        }

        public async Task UploadImages(IFormFileCollection files, Guid id)
        {
            var filenames = new List<string>();
            foreach (var file in files)
            {
                var stream = file.OpenReadStream();
                var filename = await FileUploader.UploadImage(Constants.localPathToProductAccounting, stream);
                filenames.Add(filename);
            }

            var materialAccounting = await _context.ProductAccountings.FindAsync(id);
            materialAccounting.Images = filenames.Count > 0 ? filenames : null;
            await _context.SaveChangesAsync();
        }

        public ManagerAnalyticsBody GetManagerAnalytics(IEnumerable<User> sellers)
        {
            var listOfSellerSalesForToday = new List<SellerIncomeBody>();
            var listOfSellerSalesForCurrentMonth = new List<SellerIncomeBody>();

            var listOfSalesForToday = new List<AmountOfProduct>();
            var listOfSalesForCurrentMonth = new List<AmountOfProduct>();

            var currentTime = DateTime.UtcNow;
            var salesToday = GetAllByDay(currentTime);
            var salesMonth = GetAllByMonth(currentTime).GroupBy(e => e.CreatedBy).ToList();

            var soldToday = salesToday.Sum(e => e.CheckPrice);
            var sellersSales = salesToday.GroupBy(e => e.CreatedBy).ToList();
            foreach (var seller in sellers)
            {
                var sales = sellersSales.FirstOrDefault(e => e.Key.Id == seller.Id);
                var income = sales?.Sum(e => e.CheckPrice);

                var sellerIncomeBody = new SellerIncomeBody
                {
                    SellerName = seller.FirstName + " " + seller.LastName + " " + seller.Patronymic,
                    Income = (float)(income == null ? 0 : income)
                };
                listOfSellerSalesForToday.Add(sellerIncomeBody);
            }


            foreach (var seller in sellers)
            {
                var sales = salesMonth.FirstOrDefault(e => e.Key.Id == seller.Id);
                var income = sales?.Sum(e => e.CheckPrice);

                var sellerIncomeBody = new SellerIncomeBody
                {
                    SellerName = seller.FirstName + " " + seller.LastName + " " + seller.Patronymic,
                    Income = (float)(income == null ? 0 : income)
                };

                listOfSellerSalesForCurrentMonth.Add(sellerIncomeBody);
            }
            

            var amountOfProducts = new Dictionary<Guid, int>();
            foreach(var sale in salesToday)
            {
                var products = sale.AccountedProducts.GroupBy(e => e.ProductId);
                foreach (var product in products)
                {
                    if (amountOfProducts.ContainsKey(product.Key))
                        amountOfProducts[product.Key] += product.Sum(e => e.Amount);
                    else
                        amountOfProducts.Add(product.Key, product.Sum(e => e.Amount));
                }
            }

            foreach(var amountOfProduct in amountOfProducts)
            {
                var productModel = _context.Products
                    .Include(e => e.Materials)
                        .ThenInclude(e => e.Material)
                    .First(e => e.Id == amountOfProduct.Key);

                var temp = new AmountOfProduct
                {
                    Product = productModel.ToProductBody(),
                    Amount = amountOfProduct.Value,
                };
                listOfSalesForToday.Add(temp);
            }

            var amountOfProductsByMonth = new Dictionary<Guid, int>();
            var salesByCurrentMonth = GetAllByMonth(currentTime);
            foreach (var sale in salesByCurrentMonth)
            {
                var products = sale.AccountedProducts.GroupBy(e => e.ProductId);
                foreach (var product in products)
                {
                    if(amountOfProductsByMonth.ContainsKey(product.Key))
                        amountOfProductsByMonth[product.Key] += product.Sum(e => e.Amount);
                    else
                        amountOfProductsByMonth.Add(product.Key, product.Sum(e => e.Amount));
                }
            }

            foreach (var amountOfProduct in amountOfProductsByMonth)
            {
                var productModel = _context.Products
                    .Include(e => e.Materials)
                        .ThenInclude(e => e.Material)
                    .First(e => e.Id == amountOfProduct.Key);

                var temp = new AmountOfProduct
                {
                    Product = productModel.ToProductBody(),
                    Amount = amountOfProduct.Value,
                };
                listOfSalesForCurrentMonth.Add(temp);
            }


            return new ManagerAnalyticsBody
            {
                Currency = Currency.Rub,
                SoldToday = soldToday,
                SalesStatisticsForToday = listOfSellerSalesForToday,
                SalesStatisticsThisMonth = listOfSellerSalesForCurrentMonth,
                ProductSalesForToday = listOfSalesForToday,
                ProductSalesThisMonth = listOfSalesForCurrentMonth,
                SellingProductToday = listOfSalesForToday.MaxBy(e => e.Amount) ?? new AmountOfProduct
                {
                    Amount = 0,
                    Product = new ProductBody
                    {
                        Id = "",
                        Name = "",
                        Price = 0,
                        Currency = Currency.Rub,
                        Materials = new(),
                        ImageUrl = ""
                    }
                },
            };
        }
    }
}
