using Newtonsoft.Json;

namespace HomeCook.DTO.Product
{
    public class AddUserProductDto
    {
        public string ProductId { get; set; }
        [JsonIgnore]
        public long ProductInternalId { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string? Amount { get; set; }
        public bool IsOnShoppingList { get; set; }
    }
}
