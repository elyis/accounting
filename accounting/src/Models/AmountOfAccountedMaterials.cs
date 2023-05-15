namespace accounting.src.Models
{
    //Many to many - Элемент списка затраченных материалов
    public class AmountOfAccountedMaterials
    {
        public Guid MaterialAccountingId { get; set; }
        public Guid MaterialId { get; set; }

        public MaterialAccounting MaterialAccounting { get; set; }
        public Material Material { get; set; }
        public float Amount { get; set; }
    }
}
