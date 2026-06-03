CREATE PROCEDURE sp_UpdateProductAttributeValue
    @ProductId INT,
    @AttributeId INT,
    @TextValue NVARCHAR(500) = NULL,
    @NumberValue DECIMAL(18,2) = NULL,
    @DateValue DATETIME = NULL,
    @BooleanValue BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    EXEC sp_ValidateAttributeValue
        @ProductId,
        @AttributeId,
        @TextValue,
        @NumberValue,
        @DateValue,
        @BooleanValue;

    UPDATE ProductAttributeValues
    SET
        TextValue = @TextValue,
        NumberValue = @NumberValue,
        DateValue = @DateValue,
        BooleanValue = @BooleanValue
    WHERE ProductId = @ProductId
    AND AttributeId = @AttributeId;

    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('Attribute value does not exist', 16, 11);
    END;
END;
GO
