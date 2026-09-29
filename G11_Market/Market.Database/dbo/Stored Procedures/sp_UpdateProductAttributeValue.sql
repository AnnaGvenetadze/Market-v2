CREATE PROCEDURE dbo.sp_UpdateProductAttributeValue
    @ProductId INT,
    @AttributeId INT,
    @TextValue NVARCHAR(500) = NULL,
    @NumberValue DECIMAL(18,2) = NULL,
    @DateValue DATETIME = NULL,
    @BooleanValue BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Validate Active Product
    IF NOT EXISTS (
        SELECT 1 
        FROM dbo.Products 
        WHERE Id = @ProductId 
          AND IsDeleted = 0
    )
    BEGIN
        RAISERROR('Product not found or is deleted.', 16, 1);
        RETURN;
    END;

    -- 2. Validate Active Attribute
    IF NOT EXISTS (
        SELECT 1 
        FROM dbo.Attributes 
        WHERE Id = @AttributeId 
          AND IsDeleted = 0
    )
    BEGIN
        RAISERROR('Attribute not found or is deleted.', 16, 1);
        RETURN;
    END;

    -- 3. Perform Update
    UPDATE dbo.ProductAttributeValues
    SET
        TextValue = @TextValue,
        NumberValue = @NumberValue,
        DateValue = @DateValue,
        BooleanValue = @BooleanValue
    WHERE ProductId = @ProductId
      AND AttributeId = @AttributeId;

    -- 4. Check if record existed
    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('Attribute value does not exist for the specified Product and Attribute.', 16, 11);
        RETURN;
    END;
END;