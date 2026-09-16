using Market.Extensions.Attributes;

namespace Market.DTO
{
    public sealed class ProductDTO
    {
        [IgnoreOnInsert]
        public int Id { get; set; }

        public int CategoryId { get; set; }

        public string ProductName { get; set; } = null!;

        public decimal Price { get; set; }

        [IgnoreOnInsert]
        [IgnoreOnUpdate]
        public bool IsDeleted { get; set; }

        [IgnoreOnInsert]
        [IgnoreOnUpdate]
        public DateTime CreatedDate { get; set; }

        [IgnoreOnInsert]
        [IgnoreOnUpdate]
        public DateTime? UpdatedDate { get; set; }
    }
}