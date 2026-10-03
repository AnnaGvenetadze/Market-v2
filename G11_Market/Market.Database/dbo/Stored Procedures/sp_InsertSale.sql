CREATE   PROCEDURE dbo.sp_InsertSale
    @CreatedEmployeeId INT,
    @Status TINYINT = 0,
    @CancelledByEmployeeId INT = NULL,
    @CancelledDate DATETIME = NULL,
    @CancelReason NVARCHAR(200) = NULL,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Employees WHERE Id = @CreatedEmployeeId AND IsDeleted = 0)
        THROW 50101, 'Employee not found or inactive.', 1;

    INSERT INTO dbo.Sales (CreatedEmployeeId, Status, CancelledByEmployeeId, CancelledDate, CancelReason, CreatedDate)
    VALUES (@CreatedEmployeeId, COALESCE(@Status, 0), @CancelledByEmployeeId, @CancelledDate, @CancelReason, GETUTCDATE());

    SET @Id = CONVERT(INT, SCOPE_IDENTITY());
END;