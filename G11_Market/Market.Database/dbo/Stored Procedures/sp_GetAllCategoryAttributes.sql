CREATE PROCEDURE dbo.sp_GetCategoryAttributes
    @CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Categories
        WHERE Id = @CategoryId
          AND IsDeleted = 0
    )
    BEGIN
        RAISERROR('Category was not found or has been deleted.', 16, 1);
        RETURN -1;
    END;

    SELECT
        ca.OrderPosition,
        a.AttributeName
    FROM dbo.CategoryAttributes AS ca
    INNER JOIN dbo.Attributes AS a
        ON ca.AttributeId = a.Id
    WHERE ca.CategoryId = @CategoryId
      AND a.IsDeleted = 0
    ORDER BY ca.OrderPosition;

    RETURN 0;
END;