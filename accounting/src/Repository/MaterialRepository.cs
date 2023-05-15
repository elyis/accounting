using accounting.src.Core.IRepository;
using accounting.src.Data;
using accounting.src.Entity;
using accounting.src.Entity.Request;
using accounting.src.Models;
using Microsoft.EntityFrameworkCore;

namespace accounting.src.Repository
{
    public class MaterialRepository : IMaterialRepository
    {
        private readonly AppDbContext _context;

        public MaterialRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Material> AddAsync(CreateMaterialBody body)
        {
            var material = new Material
            {
                Image = body.ImageUrl,
                Measure = Enum.GetName(typeof(Measurements), body.Measurement)!,
                Name = body.Name,
                MinimumQuantity = body.MinimumQuantity,
            };

            var result = await _context.Materials.AddAsync(material);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<Material?> UpdateAsync(CreateMaterialBody body, Guid materialId)
        {
            var material = await GetAsync(materialId);
            if (material == null)
                return null;

            material.Name = body.Name;
            material.Image = body.ImageUrl;
            material.Measure = Enum.GetName(typeof(Measurements), body.Measurement)!;
            material.MinimumQuantity = body.MinimumQuantity;
            await _context.SaveChangesAsync();

            return material;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var material = await GetAsync(id);
            if (material == null)
                return false;

            var result = _context.Materials.Remove(material);
            _context.SaveChanges();
            return result != null;
        }

        public IEnumerable<Material> GetAll(string? name)
        {
            var pattern = name ?? "";
            return _context.Materials
                    .Where(e => 
                        EF.Functions
                            .Like(e.Name.ToLower(), 
                                  $"%{pattern.ToLower()}%"));
        }

        public async Task<Material?> GetAsync(Guid id)
            => await _context.Materials.FindAsync(id);

        public async Task<Material?> Update(MaterialUpdateBody body)
        {
            var material = await GetAsync(body.Id);
            if (material == null)
                return null;

            material.Amount = body.Amount > 0 ? body.Amount : 0;

            _context.SaveChanges();
            return material;
        }

        public async Task<Material?> UpdateImage(Guid id,string image)
        {
            var material = await GetAsync(id);
            if (material == null)
                return null;

            material.Image = image;
            _context.SaveChanges();
            return material;
        }
    }
}
