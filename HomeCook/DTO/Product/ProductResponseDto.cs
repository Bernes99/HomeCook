using HomeCook.Data.Enums;

namespace HomeCook.DTO.Product
{
    public class ProductResponseDto
    {
        public string? Id { get; set; }
        public string Name { get; set; }
        public long? Calories { get; set; }
        public CategoryDto Category { get; set; }
        public UnitType UnitType { get; set; }
    }
}
