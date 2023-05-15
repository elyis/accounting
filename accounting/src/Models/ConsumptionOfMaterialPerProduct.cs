using accounting.src.Entity;

namespace accounting.src.Models
{
    public class ConsumptionOfMaterialPerProduct
    {
        public Guid ProductId { get; set; }
        public Guid MaterialId { get; set; }

        public Product Product { get; set; }
        public Material Material { get; set; }
        public float Amount { get; set; }

        public MaterialCosts ToMaterialCosts()
        {
            return new MaterialCosts
            {
                Amount = Amount,
                MaterialModel = Material.ToMaterialBody()
            };
        }
    }
}
