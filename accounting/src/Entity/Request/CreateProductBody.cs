using accounting.src.Models;

namespace accounting.src.Entity.Request
{
    public class CreateProductBody
    {
        public string Name { get; set; }
        public float Price { get; set; }
        public Currency Currency { get; set; }
        public List<AmountOfIdMaterial> Materials { get; set; }
    }
}
