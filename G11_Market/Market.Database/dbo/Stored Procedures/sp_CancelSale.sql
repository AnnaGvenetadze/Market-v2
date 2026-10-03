
CREATE   PROCEDURE dbo.sp_CancelSale
    @Id INT,
    @EmployeeId INT,
    @CancelReason NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Sales WHERE Id = @Id) THROW 50116, 'Sale not found.', 1;
    IF EXISTS (SELECT 1 FROM dbo.Sales WHERE Id = @Id AND Status = 1) THROW 50117, 'Completed sale cannot be cancelled.', 1;
    IF EXISTS (SELECT 1 FROM dbo.Sales WHERE Id = @Id AND Status = 2) THROW 50118, 'Sale is already cancelled.', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.Employees WHERE Id = @EmployeeId AND IsDeleted = 0) THROW 50119, 'Employee not found.', 1;
    IF @CancelReason IS NULL OR LEN(LTRIM(RTRIM(@CancelReason))) = 0 THROW 50120, 'Cancel reason is required.', 1;

    UPDATE dbo.Sales
    SET Status = 2,
        CancelledByEmployeeId = @EmployeeId,
        CancelledDate = GETUTCDATE(),
        CancelReason = @CancelReason
    WHERE Id = @Id AND Status = 0;
END;