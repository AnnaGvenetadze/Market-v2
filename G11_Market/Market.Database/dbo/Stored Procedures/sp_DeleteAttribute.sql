create procedure dbo.sp_DeleteAttribute
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    IF @Id IS NULL
    BEGIN
        RAISERROR('Attribute ID is required.', 16, 1);
        RETURN -1;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Attributes
        WHERE Id = @Id
          AND IsDeleted = 0
    )
    BEGIN
        RAISERROR('Attribute was not found or has already been deleted.', 16, 1);
        RETURN -2;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.CategoryAttributes
        WHERE AttributeId = @Id
    )
    BEGIN
        RAISERROR('Attribute is used in category attributes and cannot be deleted.', 16, 1);
        RETURN -3;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.ProductAttributeValues
        WHERE AttributeId = @Id
    )
    BEGIN
        RAISERROR('Attribute is used in product attribute values and cannot be deleted.', 16, 1);
        RETURN -4;
    END;

    UPDATE dbo.Attributes
    SET
        IsDeleted = 1,
        UpdatedDate = GETDATE()
    WHERE Id = @Id
      AND IsDeleted = 0;

    RETURN 0;
END;