CREATE   PROCEDURE dbo.sp_GetAllCategoryAttributes
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CategoryId, AttributeId, OrderPosition
    FROM dbo.CategoryAttributes;
END;