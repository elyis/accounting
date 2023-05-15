using accounting.src.Entity;
using accounting.src.Entity.Response;
using System.Globalization;

namespace accounting.src.Models
{
    /*
        Учет продаж продуктов
        Name - наименование продажи
        CreatedAt - дата продажи
        CheckPrice - конечная сумма продажи
        Currency - валюта
        CreatedBy - тот кто продал
        AccountedProducts - список проданных товаров
    */

    public class ProductAccounting
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Type { get; set; }
        public float CheckPrice { get; set; }
        public string Currency { get; set; }

        public User CreatedBy { get; set; }
        public List<AmountOfAccountedProduct> AccountedProducts { get; set; } = new();
        public List<string>? Images { get; set; }

        public SaleBody ToSaleBody()
        {
            var imageUrls = Images.Select(image => image == null ? 
                null : 
                $"{Constants.webPathToProductAccountingIcons}{image}")
                .ToList();

            var listOfAmountProduct = new List<AmountOfProduct>();
            var listOfProductBody = AccountedProducts.Select(e => e.Product.ToProductBody()).ToList();

            for(int i = 0; i < listOfProductBody.Count(); i++)
            {
                var amountOfProduct = new AmountOfProduct
                {
                    Amount = AccountedProducts[i].Amount,
                    Product = listOfProductBody[i]
                };
                listOfAmountProduct.Add(amountOfProduct);
            }

            return new SaleBody
            {
                Id = Id.ToString(),
                Name = Name,
                Date = CreatedAt.ToString("u").Replace(" ", "T"),
                CreatorName = CreatedBy.FirstName + " " + CreatedBy.LastName + " " + CreatedBy.Patronymic,
                ImagesUrl =  imageUrls,
                Products = listOfAmountProduct,
                Currency = (Currency)Enum.Parse(typeof(Currency), Currency),
                CheckPrice = CheckPrice
            };
        }

        public IncomePerDate ToIncomePerDate()
        {
            return new IncomePerDate
            {
                Date = CreatedAt.ToString("u", CultureInfo.InvariantCulture),
                Income = CheckPrice
            };
        }
    }
}
