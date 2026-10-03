CREATE   PROCEDURE dbo.sp_DeleteCategoryAttribute
    @CategoryId INT = NULL,
    @AttributeId INT = NULL,
    @Id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @CategoryId IS NOT NULL AND @AttributeId IS NOT NULL
    BEGIN
        IF EXISTS (SELECT 1 FROM dbo.ProductAttributeValues v JOIN dbo.Products p ON p.Id = v.ProductId
            WHERE p.CategoryId = @CategoryId AND v.AttributeId = @AttributeId)
            THROW 50000, 'Remove associated product attribute values before unassigning.', 1;
    END;

    DELETE FROM dbo.CategoryAttributes
    WHERE (@CategoryId IS NULL OR CategoryId = @CategoryId)
      AND (@AttributeId IS NULL OR AttributeId = @AttributeId);
END;