namespace accounting.src.Entity.Request
{
    public class CreateMaterialAccounting
    {
        public string Name { get; set; }
        public List<AmountOfIdMaterial> ListAmountOfMaterial { get; set; }
    }
}
