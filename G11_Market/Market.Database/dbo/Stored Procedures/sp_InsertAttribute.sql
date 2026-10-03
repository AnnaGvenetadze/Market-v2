CREATE   PROCEDURE dbo.sp_InsertAttribute
    @AttributeName NVARCHAR(100),
    @AttributeType TINYINT,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @AttributeName = NULLIF(LTRIM(RTRIM(@AttributeName)), '');

    IF @AttributeName IS NULL
    BEGIN
        RAISERROR('Attribute name is required.', 16, 1);
        RETURN -1;
    END;

    IF @AttributeType NOT IN (1, 2, 3, 4)
    BEGIN
        RAISERROR('Attribute type is invalid.', 16, 1);
        RETURN -2;
    END;

    IF EXISTS (SELECT 1 FROM dbo.Attributes WHERE AttributeName = @AttributeName AND IsDeleted = 0)
    BEGIN
        RAISERROR('Attribute with the same name already exists.', 16, 1);
        RETURN -3;
    END;

    INSERT INTO dbo.Attributes (AttributeName, AttributeType, IsDeleted, CreatedDate)
    VALUES (@AttributeName, @AttributeType, 0, GETUTCDATE());

    SET @Id = CONVERT(INT, SCOPE_IDENTITY());
END;