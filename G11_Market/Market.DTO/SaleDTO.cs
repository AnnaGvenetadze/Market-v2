using Market.Extensions.Attributes;

namespace Market.DTO;

public sealed class SaleDTO
{
    [IgnoreOnInsert]
    public int Id { get; set; }
    [IgnoreOnUpdate]
    public int CreatedEmployeeId { get; set; }
    public int? CancelledByEmployeeId { get; set; }
    public byte Status { get; set; }
    [IgnoreOnInsert]
    [IgnoreOnUpdate]
    public DateTime CreatedDate { get; set; }
    public DateTime? CancelledDate { get; set; }
    public string CancelReason { get; set; }
}