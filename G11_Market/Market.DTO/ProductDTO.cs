using Market.Extensions.Attributes;

namespace Market.DTO
{
    public class ProductDTO
    {
        [IgnoreOnInsert]
        [IgnoreOnUpdate]
        public int Id { get; set; }

        public int CategoryId { get; set; }

        public string ProductName { get; set; } = null!;

        public decimal Price { get; set; }

        [IgnoreOnInsert]
        public bool IsDeleted { get; set; }

        [IgnoreOnInsert]
        [IgnoreOnUpdate]
        public DateTime CreatedDate { get; set; }

        [IgnoreOnInsert]
        public DateTime? UpdatedDate { get; set; }
    }
}