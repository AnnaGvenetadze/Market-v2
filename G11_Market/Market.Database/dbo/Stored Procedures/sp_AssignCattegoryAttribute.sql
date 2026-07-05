CREATE PROCEDURE dbo.sp_AssignCategoryAttribute
    @CategoryId INT,
    @AttributeId INT,
    @OrderPosition INT = 0
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
        FROM dbo.Categories
        WHERE Id = @CategoryId
          AND IsDeleted = 0
    )
    BEGIN
        RAISERROR('Category was not found.', 16, 1);
        RETURN -3;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Attributes
        WHERE Id = @AttributeId
          AND IsDeleted = 0
    )
    BEGIN
        RAISERROR('Attribute was not found.', 16, 1);
        RETURN -4;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.CategoryAttributes
        WHERE CategoryId = @CategoryId
          AND AttributeId = @AttributeId
    )
    BEGIN
        RAISERROR('Category attribute already exists.', 16, 1);
        RETURN -5;
    END;

    INSERT INTO dbo.CategoryAttributes
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