using accounting.src.Models;

namespace accounting.src.Entity
{
    public class MaterialBody
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Measurements Measurement { get; set; }
        public float MinimumQuantity { get; set; }
        public string? ImageUrl { get; set; }
    }
}
