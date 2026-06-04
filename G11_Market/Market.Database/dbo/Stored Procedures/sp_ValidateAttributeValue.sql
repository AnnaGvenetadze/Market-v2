CREATE PROCEDURE sp_ValidateAttributeValue
    @ProductId INT,
    @AttributeId INT,
    @TextValue NVARCHAR(500) = NULL,
    @NumberValue DECIMAL(18,2) = NULL,
    @DateValue DATETIME = NULL,
    @BooleanValue BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @AttributeType TINYINT;

    SELECT @AttributeType = AttributeType
    FROM Attributes
    WHERE AttributeId = @AttributeId;

    IF @AttributeType IS NULL
    BEGIN
        RAISERROR('Invalid AttributeID', 16, 1);
        RETURN;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM Products p
        JOIN CategoryAttributes ca
            ON ca.CategoryId = p.CategoryId
        WHERE p.Id = @ProductId
        AND ca.AttributeID = @AttributeId
    )
    BEGIN
        RAISERROR('Attribute does not belong to product category', 16, 2);
        RETURN;
    END;

    IF @AttributeType = 1 AND @TextValue IS NULL
    BEGIN
        RAISERROR('Text value required', 16, 3);
        RETURN;
    END;

    IF @AttributeType = 2 AND @NumberValue IS NULL
    BEGIN
        RAISERROR('Number value required', 16, 4);
        RETURN;
    END;

    IF @AttributeType = 3 AND @DateValue IS NULL
    BEGIN
        RAISERROR('Date value required', 16, 5);
        RETURN;
    END;

    IF @AttributeType = 4 AND @BooleanValue IS NULL
    BEGIN
        RAISERROR('Boolean value required', 16, 6);
        RETURN;
    END;

    IF @AttributeType <> 1 AND @TextValue IS NOT NULL
    BEGIN
        RAISERROR('TextValue not allowed', 16, 7);
        RETURN;
    END;

    IF @AttributeType <> 2 AND @NumberValue IS NOT NULL
    BEGIN
        RAISERROR('NumberValue not allowed', 16, 8);
        RETURN;
    END;

    IF @AttributeType <> 3 AND @DateValue IS NOT NULL
    BEGIN
        RAISERROR('DateValue not allowed', 16, 9);
        RETURN;
    END;

    IF @AttributeType <> 4 AND @BooleanValue IS NOT NULL
    BEGIN
        RAISERROR('BooleanValue not allowed', 16, 10);
        RETURN;
    END;

    RETURN 0;
END;
GO