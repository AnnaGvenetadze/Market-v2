CREATE   PROCEDURE dbo.sp_GetAllProductAttributeValues
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ProductId, AttributeId, TextValue, NumberValue, DateValue, BooleanValue
    FROM dbo.ProductAttributeValues;
END;