CREATE PROCEDURE dbo.sp_GetProductById
    @Id INT
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
    WHERE Id = @Id
      AND IsDeleted = 0;
END