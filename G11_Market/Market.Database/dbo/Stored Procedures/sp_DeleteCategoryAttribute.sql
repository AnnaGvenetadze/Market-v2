
CREATE   PROCEDURE dbo.sp_DeleteCategoryAttribute
    @CategoryId INT,
    @AttributeId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF @CategoryId IS NULL
    BEGIN
        RAISERROR('CategoryId is required.', 16, 1);
        RETURN -1;
    END;

    IF @AttributeId IS NULL
    BEGIN
        RAISERROR('AttributeId is required.', 16, 1);
        RETURN -2;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM CategoryAttributes
        WHERE CategoryId = @CategoryId
          AND AttributeId = @AttributeId
    )
    BEGIN
        RAISERROR('Category attribute was not found.', 16, 1);
        RETURN -3;
    END;

    IF EXISTS (SELECT 1 FROM dbo.ProductAttributeValues v JOIN dbo.Products p ON p.Id = v.ProductId
        WHERE p.CategoryId = @CategoryId AND v.AttributeId = @AttributeId)
        THROW 50000, 'Remove associated product attribute values before unassigning.', 1;
    DELETE FROM CategoryAttributes
    WHERE CategoryId = @CategoryId
      AND AttributeId = @AttributeId;

    RETURN 0;
END;