using accounting.src.Core.IRepository;
using accounting.src.Data;
using accounting.src.Entity.Request;
using accounting.src.Models;
using accounting.src.Utility;
using Microsoft.EntityFrameworkCore;

namespace accounting.src.Repository
{
    public class MaterialAccountingRepository : IMaterialAccountingRepository
    {
        private readonly AppDbContext _context;

        public MaterialAccountingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MaterialAccounting?> AddAsync(CreateMaterialAccounting body, User author, TypeOfGoodsAccounting type)
        {
            var amountOfAccountingMaterials = new List<AmountOfAccountedMaterials>();


            foreach(var amountOfMaterial in body.ListAmountOfMaterial)
            {
                var materialId = Guid.Parse(amountOfMaterial.Id);
                var material = await _context.Materials.FindAsync(materialId);

                var materialMeasure = (Measurements)Enum.Parse(typeof(Measurements), material.Measure);
                if (materialMeasure == Measurements.Other || materialMeasure == Measurements.Piece)
                    amountOfMaterial.Amount = (int)amountOfMaterial.Amount;
                var amountOfAccountedMaterials = new AmountOfAccountedMaterials
                {
                    Material = material,
                    Amount = amountOfMaterial.Amount,
                };
                float difference;
                if (type == TypeOfGoodsAccounting.Write_off)
                    difference = material.Amount - amountOfAccountedMaterials.Amount;
                else
                    difference = material.Amount + amountOfAccountedMaterials.Amount;

                if(difference < 0)
                {
                    amountOfMaterial.Amount = material.Amount;
                    difference = 0;
                }
                amountOfAccountedMaterials.Amount = amountOfMaterial.Amount;
                material.Amount = difference;
                amountOfAccountingMaterials.Add(amountOfAccountedMaterials);
            }

            var materialAccounting = new MaterialAccounting
            {
                AccountedMaterials = amountOfAccountingMaterials,
                CreatedBy = author,
                Name = body.Name,
                Type = Enum.GetName(typeof(TypeOfGoodsAccounting), type)!
            };

            var result = await _context.MaterialAccountings.AddAsync(materialAccounting);
            await _context.SaveChangesAsync();
            return result?.Entity;
        }

        public IEnumerable<MaterialAccounting> GetAll(TypeOfGoodsAccounting type)
        {
            var typeName = Enum.GetName(typeof(TypeOfGoodsAccounting), type);
            return _context.MaterialAccountings
                    .Include(e => e.CreatedBy)
                    .Include(e => e.AccountedMaterials)
                        .ThenInclude(e => e.Material)
                    .Where(e => e.Type == typeName)
                        .OrderByDescending(e => e.CreatedAt);

        }

        public async Task<MaterialAccounting?> GetAsync(Guid id)
            => await _context.MaterialAccountings
                    .Include(e => e.CreatedBy)
                    .Include(e => e.AccountedMaterials)
                        .ThenInclude(e => e.Material)
                    .FirstOrDefaultAsync(e => e.Id == id);

        public async Task UploadImages(IFormFileCollection files, Guid id)
        {
            var filenames = new List<string>();
            foreach(var file in files)
            {
                var stream = file.OpenReadStream();
                var filename = await FileUploader.UploadImage(Constants.localPathToMaterialAccounting, stream);
                filenames.Add(filename);
            }

            var materialAccounting = await _context.MaterialAccountings.FindAsync(id);
            materialAccounting.Images = filenames.Count > 0 ? filenames : null;
            await _context.SaveChangesAsync();
        }
    }
}
