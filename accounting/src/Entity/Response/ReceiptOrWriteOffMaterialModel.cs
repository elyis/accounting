namespace accounting.src.Entity.Response
{
    public class ReceiptOrWriteOffMaterialModel
    {
        public string CreatorName { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public List<string>? ListImageUrl { get; set; }
        public List<AmountOfMaterial> ListAmountOfMaterial { get; set; }
    }
}
