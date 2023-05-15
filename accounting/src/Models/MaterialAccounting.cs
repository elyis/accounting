using accounting.src.Entity;
using accounting.src.Entity.Response;

namespace accounting.src.Models
{
    /*
        Учет материалов (списания и начисления)
        Name - Наименование (Завоз за январь)
        CreatedAt - дата списания/начисления
        Type - списание или начисление
        CreatedBy - тот кто добавил запись
        AccountedMaterials - какие именно материалы начислены/списаны 
        Images - список фото при начислении/списании
    */

    public class MaterialAccounting
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Type { get; set; }

        public User CreatedBy { get; set; }

        public List<AmountOfAccountedMaterials> AccountedMaterials { get; set; } = new();
        public List<string>? Images { get; set; }

        public ReceiptOrWriteOffMaterialModel ToReceiptOrWriteOffMaterialModel()
        {
            var materials = AccountedMaterials.Select(e => e.Material);
            var listOfMaterialBody = materials.Select(e => e.ToMaterialBody()).ToList();
            var listOfAmountMaterial = new List<AmountOfMaterial>();

            for(int i = 0; i < materials.Count(); i++)
            {
                var amountOfMaterial = new AmountOfMaterial
                {
                    Amount = AccountedMaterials[i].Amount,
                    MaterialModel = listOfMaterialBody[i]
                };
                listOfAmountMaterial.Add(amountOfMaterial);
            }

            var urls = Images.Select(image => image == null ? 
                null : 
                $"{Constants.webPathToMaterialAccountingIcons}{image}")
                .ToList();

            return new ReceiptOrWriteOffMaterialModel
            {
                CreatorName = CreatedBy.FirstName + " " + CreatedBy.LastName + " " + CreatedBy.Patronymic,
                Date = CreatedAt.ToString("u").Replace(" ", "T"),
                Name = Name,
                ListAmountOfMaterial = listOfAmountMaterial,
                ListImageUrl = urls,
            };
        }
    }

    public enum TypeOfGoodsAccounting
    {
        Write_off,
        Receipt
    }
}
