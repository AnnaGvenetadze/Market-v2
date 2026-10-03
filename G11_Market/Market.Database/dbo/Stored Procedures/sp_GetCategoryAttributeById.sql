CREATE   PROCEDURE dbo.sp_GetCategoryAttributeById
    @CategoryId INT = NULL,
    @AttributeId INT = NULL,
    @Id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CategoryId, AttributeId, OrderPosition
    FROM dbo.CategoryAttributes
    WHERE (@CategoryId IS NULL OR CategoryId = @CategoryId)
      AND (@AttributeId IS NULL OR AttributeId = @AttributeId);
END;