CREATE   PROCEDURE dbo.sp_DeleteProductAttributeValue
    @ProductId INT = NULL,
    @AttributeId INT = NULL,
    @Id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @ProductId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Id = @ProductId)
    BEGIN
        RAISERROR('Product was not found.', 16, 1);
        RETURN -1;
    END;

    IF @AttributeId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Attributes WHERE Id = @AttributeId)
    BEGIN
        RAISERROR('Attribute was not found.', 16, 1);
        RETURN -2;
    END;

    DELETE FROM dbo.ProductAttributeValues
    WHERE (@ProductId IS NULL OR ProductId = @ProductId)
      AND (@AttributeId IS NULL OR AttributeId = @AttributeId);
END;