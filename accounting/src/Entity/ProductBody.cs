using accounting.src.Models;

namespace accounting.src.Entity
{
    public class ProductBody
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public Currency Currency { get; set; }
        public string? ImageUrl { get; set; }

        public List<MaterialCosts> Materials { get; set; }
    }
}
