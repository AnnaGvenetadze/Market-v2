using Market.DTO;
using Market.DTO.Enums;

namespace Market.Tests.Helpers;

public static class SalesTestDataFactory
{
    public static SaleDTO CreateSale(
        int employeeId,
        SaleStatus status = SaleStatus.Draft,
        int? cancelledByEmployeeId = null,
        DateTime? cancelledDate = null,
        string? cancelReason = null)
    {
        return new SaleDTO
        {
            CreatedEmployeeId = employeeId,
            Status = status,
            CancelledByEmployeeId = cancelledByEmployeeId,
            CancelledDate = cancelledDate,
            CancelReason = cancelReason
        };
    }
}