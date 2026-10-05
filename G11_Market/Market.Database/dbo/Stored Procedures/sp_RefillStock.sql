CREATE PROCEDURE dbo.sp_RefillStock
    @ProductId INT,
    @Quantity INT,
    @EmployeeId INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @ProductId <= 0
        THROW 50201, 'Invalid product id.', 1;

    IF @Quantity <= 0
        THROW 50202, 'Refill quantity must be greater than zero.', 1;

    IF @EmployeeId <= 0
        THROW 50203, 'Invalid employee id.', 1;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Products
        WHERE Id = @ProductId
          AND IsDeleted = 0
    )
        THROW 50204, 'Product not found or inactive.', 1;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Employees
        WHERE Id = @EmployeeId
          AND IsDeleted = 0
    )
        THROW 50205, 'Employee not found or inactive.', 1;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Product row-ს ვკეტავთ, რომ ერთსა და იმავე პროდუქტზე
        -- პარალელური stock ცვლილებები ერთმანეთს არ დაეჯახოს.
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
            1,              -- 1 = Refill
            @Quantity,
            @QuantityBefore,
            NULL,
            @EmployeeId,
            NULL
        );

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH;
END;