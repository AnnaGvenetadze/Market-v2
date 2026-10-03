using Market.Extensions.Attributes;

namespace Market.DTO;

public sealed class AttributeDTO
{
    [IgnoreOnInsert]
    public int Id { get; set; }

    public string AttributeName { get; set; } = null!;
    public byte AttributeType { get; set; }

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