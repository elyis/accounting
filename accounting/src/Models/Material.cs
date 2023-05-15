using accounting.src.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace accounting.src.Models
{
    public class Material
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Measure { get; set; }
        public string? Image { get; set; }
        public float Amount { get; set; } = 0;
        public float MinimumQuantity { get; set; }

        public List<ConsumptionOfMaterialPerProduct> Products { get; set; } = new();
        public List<AmountOfAccountedMaterials> AccountedMaterials { get; set; } = new();

        public MaterialBody ToMaterialBody()
        {
            return new MaterialBody
            {
                Id = Id.ToString(),
                ImageUrl = Image == null ? null : $"{Constants.webPathToMaterialIcons}{Image}",
                Name = Name,
                Measurement = (Measurements)Enum.Parse(typeof(Measurements), Measure), 
                MinimumQuantity = MinimumQuantity
            };
        }

        public AmountOfMaterial ToAmountOfMaterial()
        {
            return new AmountOfMaterial
            {
                Amount = Amount,
                MaterialModel = ToMaterialBody(),
            };
        }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Measurements
    {
        Piece,
        Kilogram,
        Gram,
        Liter,
        Milliliter,
        Meter,
        Centimeter,
        Other,
    }
}
