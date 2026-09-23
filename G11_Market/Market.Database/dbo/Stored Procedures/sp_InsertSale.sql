CREATE PROCEDURE dbo.sp_InsertSale
    @CreatedEmployeeId INT,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Employees
        WHERE Id = @CreatedEmployeeId
          AND IsDeleted = 0
    )
        THROW 50101, 'Employee not found or inactive.', 1;

    INSERT INTO dbo.Sales
    (
        CreatedEmployeeId,
        Status
    )
    VALUES
    (
        @CreatedEmployeeId,
        0 -- Draft
    );

    SET @Id = CONVERT(INT, SCOPE_IDENTITY());
END;
GO