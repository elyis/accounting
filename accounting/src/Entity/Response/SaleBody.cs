using accounting.src.Models;

namespace accounting.src.Entity.Response
{
    public class SaleBody
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public List<string> ImagesUrl { get; set; }
        public List<AmountOfProduct>  Products { get; set; }
        public string CreatorName { get; set; }
        public float CheckPrice { get; set; }
        public Currency Currency { get; set; }
        
    }
}
