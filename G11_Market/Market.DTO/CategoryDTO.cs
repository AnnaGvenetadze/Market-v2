using Market.Extensions.Attributes;

namespace Market.DTO;

public sealed class CategoryDTO
{
    [IgnoreOnInsert]
    public int Id { get; set; }

    public int? ParentId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public string? Description { get; set; }

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