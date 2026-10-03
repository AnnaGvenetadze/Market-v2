CREATE   PROCEDURE dbo.sp_GetProductAttributeValueById
    @ProductId INT = NULL,
    @AttributeId INT = NULL,
    @Id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ProductId, AttributeId, TextValue, NumberValue, DateValue, BooleanValue
    FROM dbo.ProductAttributeValues
    WHERE (@ProductId IS NULL OR ProductId = @ProductId)
      AND (@AttributeId IS NULL OR AttributeId = @AttributeId);
END;