CREATE PROCEDURE dbo.sp_AdjustStock
    @ProductId INT,
    @QuantityDifference INT,
    @EmployeeId INT,
    @Reason NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @ProductId <= 0
        THROW 50211, 'Invalid product id.', 1;

    IF @QuantityDifference = 0
        THROW 50212, 'Quantity difference cannot be zero.', 1;

    IF @EmployeeId <= 0
        THROW 50213, 'Invalid employee id.', 1;

    IF @Reason IS NULL
       OR LEN(LTRIM(RTRIM(@Reason))) = 0
        THROW 50214, 'Adjustment reason is required.', 1;

    IF LEN(@Reason) > 200
        THROW 50215, 'Adjustment reason cannot exceed 200 characters.', 1;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Products
        WHERE Id = @ProductId
          AND IsDeleted = 0
    )
        THROW 50216, 'Product not found or inactive.', 1;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Employees
        WHERE Id = @EmployeeId
          AND IsDeleted = 0
    )
        THROW 50217, 'Employee not found or inactive.', 1;

    BEGIN TRY
        BEGIN TRANSACTION;

        SELECT Id
        FROM dbo.Products WITH (UPDLOCK, HOLDLOCK)
        WHERE Id = @ProductId;

        DECLARE @QuantityBefore INT;

        SELECT TOP (1)
            @QuantityBefore =
                QuantityBefore + QuantityChange
        FROM dbo.StockMovements
        WHERE ProductId = @ProductId
        ORDER BY Id DESC;

        SET @QuantityBefore = COALESCE(@QuantityBefore, 0);

        IF @QuantityBefore + @QuantityDifference < 0
            THROW 50218, 'Stock cannot become negative.', 1;

        INSERT INTO dbo.StockMovements
        (
            ProductId,
            MovementType,
            QuantityChange,
            QuantityBefore,
            SaleItemId,
            ChangedByEmployeeId,
            Reason
        )
        VALUES
        (
            @ProductId,
            2,                  -- 2 = Adjustment
            @QuantityDifference,
            @QuantityBefore,
            NULL,
            @EmployeeId,
            LTRIM(RTRIM(@Reason))
        );

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH;
END;