namespace accounting.src.Models
{
    public class AmountOfAccountedProduct
    {
        public Guid ProductAccountingId { get; set; }
        public Guid ProductId { get; set; }

        public ProductAccounting ProductAccounting { get; set; }
        public Product Product { get; set; }
        public int Amount { get; set; }
    }
}
