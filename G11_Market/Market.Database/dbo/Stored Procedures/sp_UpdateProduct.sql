CREATE   PROCEDURE dbo.sp_UpdateProduct
    @Id INT,
    @CategoryId INT,
    @ProductName NVARCHAR(100),
    @Price DECIMAL(18, 2)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Price IS NULL OR @Price < 0 THROW 50000, 'Price must be nonnegative.', 1;
    IF NULLIF(LTRIM(RTRIM(@ProductName)), '') IS NULL THROW 50000, 'ProductName is required.', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE Id = @CategoryId AND IsDeleted = 0) THROW 50000, 'Active category not found.', 1;
    IF EXISTS (SELECT 1 FROM dbo.ProductAttributeValues v WHERE v.ProductId = @Id
        AND NOT EXISTS (SELECT 1 FROM dbo.CategoryAttributes ca WHERE ca.CategoryId = @CategoryId AND ca.AttributeId = v.AttributeId))
        THROW 50000, 'Existing product attributes are incompatible with the new category.', 1;

    UPDATE dbo.Products
    SET CategoryId = @CategoryId,
        ProductName = @ProductName,
        Price = @Price,
        UpdatedDate = GETUTCDATE()
    WHERE Id = @Id AND IsDeleted = 0;

    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('Product with Id %d was not found or has been deleted.', 16, 1, @Id);
    END;
END;
GO