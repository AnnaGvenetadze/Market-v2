CREATE PROCEDURE dbo.sp_UnassignCategoryAttribute
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
        FROM dbo.CategoryAttributes
        WHERE CategoryId = @CategoryId
          AND AttributeId = @AttributeId
    )
    BEGIN
        RAISERROR('Category attribute was not found.', 16, 1);
        RETURN -3;
    END;

    DELETE FROM dbo.CategoryAttributes
    WHERE CategoryId = @CategoryId
      AND AttributeId = @AttributeId;

    RETURN 0;
END;