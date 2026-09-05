CREATE PROCEDURE dbo.sp_GetAllProducts
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        CategoryId,
        ProductName,
        Price,
        IsDeleted,
        CreatedDate,
        UpdatedDate
    FROM dbo.Products
    WHERE IsDeleted = 0;
END;
GO
