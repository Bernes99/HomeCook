﻿namespace HomeCook.DTO.Product
{
    public class UserProductDto 
    {
        public string Id { get; set; }
        public ProductResponseDto Product { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string? Amount { get; set; }
        public bool IsOnShoppingList { get; set; }
    }
}
