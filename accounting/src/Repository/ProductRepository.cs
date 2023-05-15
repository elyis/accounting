using accounting.src.Core.IRepository;
using accounting.src.Data;
using accounting.src.Entity.Request;
using accounting.src.Models;
using Microsoft.EntityFrameworkCore;

namespace accounting.src.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Product?> AddAsync(CreateProductBody body)
        {
            var materials = new List<ConsumptionOfMaterialPerProduct>();
            foreach(var mat in body.Materials)
            {
                var materialId = Guid.Parse(mat.Id);
                var material = await _context.Materials.FindAsync(materialId);
                if(material == null)
                    return null;

                var materialPerProduct = new ConsumptionOfMaterialPerProduct 
                { 
                    Material = material, 
                    Amount = mat.Amount,
                };
                materials.Add(materialPerProduct);
            }

            var product = new Product
            {
                Currency = Enum.GetName(typeof(Currency), body.Currency)!,
                Name = body.Name,
                Price = body.Price,
                Materials = materials
            };

            var result = await _context.Products.AddAsync(product);
            _context.SaveChanges();
            return result.Entity;
        }

        public async Task<Product?> UpdateAsync(CreateProductBody body, Guid productId)
        {
            var product = await GetAsync(productId);
            if (product == null)
                return null;

            var materials = new List<ConsumptionOfMaterialPerProduct>();
            foreach (var mat in body.Materials)
            {
                var materialId = Guid.Parse(mat.Id);
                var material = await _context.Materials.FindAsync(materialId);
                if (material == null)
                    return null;

                var materialPerProduct = new ConsumptionOfMaterialPerProduct
                {
                    Material = material,
                    Amount = mat.Amount,
                };
                materials.Add(materialPerProduct);
            }

            product.Currency = Enum.GetName(typeof(Currency), body.Currency)!;
            product.Price = body.Price;
            product.Name = body.Name;
            product.Materials = materials;
            await _context.SaveChangesAsync();
            return product;
        }

        public IEnumerable<Product> GetAll(string? name)
        {
            var pattern = name ?? "";
            return _context.Products
                .Include(e => e.Materials)
                .ThenInclude(e => e.Material)
                    .Where(e =>
                        EF.Functions
                            .Like(e.Name.ToLower(),
                                  $"%{pattern.ToLower()}%"));
        }

        public async Task<Product?> GetAsync(Guid id)
            => await _context.Products
                    .Include(e => e.Materials)
                        .ThenInclude(e => e.Material)
                     .FirstOrDefaultAsync(product => 
                                                product.Id == id);

        public async Task<Product?> UpdateImage(Guid id, string image)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return null;

            product.Image = image;
            _context.SaveChanges();
            return product;
        }
    }
}
