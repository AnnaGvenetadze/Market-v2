CREATE PROCEDURE dbo.sp_DeleteSale
    @Id INT,
    @EmployeeId INT,
    @CancelReason NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Sales
    SET
        Status = 2,
        CancelledByEmployeeId = @EmployeeId,
        CancelledDate = GETDATE(),
        CancelReason = @CancelReason
    WHERE Id = @Id
      AND Status <> 2;

    IF @@ROWCOUNT = 0
    throw 50116, 'Sale not found or already cancelled.', 1;

END
GO