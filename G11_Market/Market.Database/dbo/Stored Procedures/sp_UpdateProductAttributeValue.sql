CREATE   PROCEDURE dbo.sp_UpdateProductAttributeValue
    @ProductId INT,
    @AttributeId INT,
    @TextValue NVARCHAR(500) = NULL,
    @NumberValue DECIMAL(18,2) = NULL,
    @DateValue DATETIME = NULL,
    @BooleanValue BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Type TINYINT;
    SELECT @Type = a.AttributeType FROM dbo.Products p
    JOIN dbo.Categories c ON c.Id = p.CategoryId AND c.IsDeleted = 0
    JOIN dbo.CategoryAttributes ca ON ca.CategoryId = p.CategoryId AND ca.AttributeId = @AttributeId
    JOIN dbo.Attributes a ON a.Id = ca.AttributeId AND a.IsDeleted = 0
    WHERE p.Id = @ProductId AND p.IsDeleted = 0;
    IF @Type IS NULL THROW 50000, 'Active product/category attribute assignment not found.', 1;
    IF (CASE WHEN @TextValue IS NOT NULL THEN 1 ELSE 0 END + CASE WHEN @NumberValue IS NOT NULL THEN 1 ELSE 0 END
        + CASE WHEN @DateValue IS NOT NULL THEN 1 ELSE 0 END + CASE WHEN @BooleanValue IS NOT NULL THEN 1 ELSE 0 END) <> 1
        THROW 50000, 'Exactly one attribute value is required.', 1;
    IF (@Type = 1 AND (@TextValue IS NULL OR LEN(LTRIM(RTRIM(@TextValue))) = 0))
       OR (@Type = 2 AND @NumberValue IS NULL) OR (@Type = 3 AND @DateValue IS NULL)
       OR (@Type = 4 AND @BooleanValue IS NULL) THROW 50000, 'Value does not match attribute type.', 1;
    UPDATE dbo.ProductAttributeValues SET TextValue = @TextValue, NumberValue = @NumberValue, DateValue = @DateValue, BooleanValue = @BooleanValue
    WHERE ProductId = @ProductId AND AttributeId = @AttributeId;
    IF @@ROWCOUNT = 0 THROW 50000, 'Attribute value not found.', 1;
END;