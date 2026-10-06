CREATE PROCEDURE dbo.sp_CompleteSale
    @SaleId INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @SaleId <= 0
        THROW 50301, 'Invalid sale id.', 1;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @EmployeeId INT;
        DECLARE @Status TINYINT;

        -- Sale-ის დაბლოკვა, რომ პარალელურად ვერ გაუქმდეს
        -- ან მეორედ ვერ დასრულდეს.
        SELECT
            @EmployeeId = CreatedEmployeeId,
            @Status = Status
        FROM dbo.Sales WITH (UPDLOCK, HOLDLOCK)
        WHERE Id = @SaleId;

        IF @EmployeeId IS NULL
            THROW 50302, 'Sale not found.', 1;

        IF @Status <> 0
            THROW 50303, 'Only a draft sale can be completed.', 1;

        -- ცარიელი Sale ვერ დასრულდება.
        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.SaleItems
            WHERE SaleId = @SaleId
        )
            THROW 50304, 'Sale must contain at least one item.', 1;


        -- Sale-ში არსებული პროდუქტების დაბლოკვა.
        -- Refill / Adjust-იც Product row-ს კეტავს,
        -- ამიტომ stock-ის ცვლილებები ერთმანეთს დაელოდება.
        SELECT p.Id
        FROM dbo.Products AS p WITH (UPDLOCK, HOLDLOCK)
        INNER JOIN dbo.SaleItems AS si
            ON si.ProductId = p.Id
        WHERE si.SaleId = @SaleId;


        -- ვამოწმებთ, საკმარისია თუ არა მიმდინარე stock.
        IF EXISTS
        (
            SELECT 1
            FROM dbo.SaleItems AS si

            OUTER APPLY
            (
                SELECT TOP (1)
                    sm.QuantityBefore + sm.QuantityChange AS CurrentQuantity
                FROM dbo.StockMovements AS sm
                WHERE sm.ProductId = si.ProductId
                ORDER BY sm.Id DESC
            ) AS stock

            WHERE si.SaleId = @SaleId
              AND COALESCE(stock.CurrentQuantity, 0) < si.Quantity
        )
            THROW 50305, 'Insufficient stock for one or more products.', 1;


        -- ყოველი გაყიდული პროდუქტისათვის StockMovement.
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
        SELECT
            si.ProductId,
            0,                          -- 0 = Sale
            -si.Quantity,
            COALESCE(stock.CurrentQuantity, 0),
            si.Id,
            @EmployeeId,
            NULL
        FROM dbo.SaleItems AS si

        OUTER APPLY
        (
            SELECT TOP (1)
                sm.QuantityBefore + sm.QuantityChange AS CurrentQuantity
            FROM dbo.StockMovements AS sm
            WHERE sm.ProductId = si.ProductId
            ORDER BY sm.Id DESC
        ) AS stock

        WHERE si.SaleId = @SaleId;


        -- Sale დასრულებულია.
        UPDATE dbo.Sales
        SET Status = 1                  -- 1 = Completed
        WHERE Id = @SaleId;


        COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH;
END;