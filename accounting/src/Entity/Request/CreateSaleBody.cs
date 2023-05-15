using accounting.src.Models;

namespace accounting.src.Entity.Request
{
    public class CreateSaleBody
    {
        public string Name { get; set; }
        public List<AmountOfIdProduct> Products { get; set; }
        public float CheckPrice { get; set; }
        public Currency Currency { get; set; }
    }
}
