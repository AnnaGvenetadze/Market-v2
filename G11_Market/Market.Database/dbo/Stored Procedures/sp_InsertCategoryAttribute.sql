
CREATE   PROCEDURE dbo.sp_InsertCategoryAttribute
    @CategoryId INT,
    @AttributeId INT,
    @OrderPosition INT
AS
BEGIN
    SET NOCOUNT ON;

    IF @CategoryId IS NULL
    BEGIN
        RAISERROR('CategoryId is required.', 16, 1);
        RETURN -1;
    END;

    IF @AttributeId IS NULL
    BEGIN
        RAISERROR('AttributeId is required.', 16, 1);
        RETURN -2;
    END;

    IF @OrderPosition IS NULL
    BEGIN
        SET @OrderPosition = 0;
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
        RETURN -3;
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.Attributes WHERE Id = @AttributeId AND IsDeleted = 0)
        THROW 50000, 'Attribute not found or deleted.', 1;

    IF EXISTS
    (
        SELECT 1
        FROM CategoryAttributes
        WHERE CategoryId = @CategoryId
          AND AttributeId = @AttributeId
    )
    BEGIN
        RAISERROR('Category attribute already exists.', 16, 1);
        RETURN -4;
    END;

    INSERT INTO CategoryAttributes
    (
        CategoryId,
        AttributeId,
        OrderPosition
    )
    VALUES
    (
        @CategoryId,
        @AttributeId,
        @OrderPosition
    );

    RETURN 0;
END;