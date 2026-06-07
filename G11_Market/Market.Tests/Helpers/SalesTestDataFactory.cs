using Market.DTO;

namespace Market.Tests.Helpers;

public static class SalesTestDataFactory
{
    public static SaleDTO CreateSale(
        int employeeId,
        byte status = 0,
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
            CancelReason = cancelReason ?? "default cancel reason"
        };
    }
}