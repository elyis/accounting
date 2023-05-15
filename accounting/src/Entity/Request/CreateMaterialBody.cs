using accounting.src.Models;

namespace accounting.src.Entity.Request
{
    public class CreateMaterialBody
    {
        public string Name { get; set; }
        public Measurements Measurement { get; set; }
        public float MinimumQuantity { get; set; }
        public string? ImageUrl { get; set; }
    }
}
