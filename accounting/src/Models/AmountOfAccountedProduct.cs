namespace accounting.src.Models
{
    //Many to many - элемент списка продаж продукта

    public class AmountOfAccountedProduct
    {
        public Guid ProductAccountingId { get; set; }
        public Guid ProductId { get; set; }

        public ProductAccounting ProductAccounting { get; set; }
        public Product Product { get; set; }
        public int Amount { get; set; }
    }
}
