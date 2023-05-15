using accounting.src.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace accounting.src.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public string Currency { get; set; }
        public string? Image { get; set; }

        public List<ConsumptionOfMaterialPerProduct> Materials { get; set; } = new();
        public List<AmountOfAccountedProduct> AccountedProducts { get; set; } = new();

        public ProductBody ToProductBody()
        {
            return new ProductBody
            {
                Id = Id.ToString(),
                Name = Name,
                Currency = (Currency)Enum.Parse(typeof(Currency), Currency),
                ImageUrl = Image == null ? null : $"{Constants.webPathToProductIcons}{Image}",
                Price = Price,
                Materials = Materials.Select(e => e.ToMaterialCosts()).ToList()
            };
        }
    }



    [JsonConverter(typeof(StringEnumConverter))]
    public enum Currency
    {
        Rub,
        //Dollar,
        //Euro,
    }
}
