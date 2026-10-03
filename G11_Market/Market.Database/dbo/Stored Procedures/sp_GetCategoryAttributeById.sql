CREATE   PROCEDURE dbo.sp_GetCategoryAttributeById
    @CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF @CategoryId IS NULL
    BEGIN
        RAISERROR('CategoryId is required.', 16, 1);
        RETURN -1;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM Categories
        WHERE Id = @CategoryId
          AND IsDeleted = 0
    )
    BEGIN
        RAISERROR('Category was not found.', 16, 1);
        RETURN -2;
    END;

    SELECT
        CategoryId,
        AttributeId,
        OrderPosition
    FROM CategoryAttributes
    WHERE CategoryId = @CategoryId
    ORDER BY OrderPosition, AttributeId;

    RETURN 0;
END;