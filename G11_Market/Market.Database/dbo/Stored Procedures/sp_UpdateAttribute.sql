create procedure dbo.sp_UpdateAttribute
    @Id INT,
    @AttributeName NVARCHAR(100),
    @AttributeType TINYINT
AS
BEGIN
    SET NOCOUNT ON;

    SET @AttributeName = NULLIF(LTRIM(RTRIM(@AttributeName)), '');

    IF @Id IS NULL
    BEGIN
        RAISERROR('Attribute ID is required.', 16, 1);
        RETURN -1;
    END;

    IF @AttributeName IS NULL
    BEGIN
        RAISERROR('Attribute name is required.', 16, 1);
        RETURN -2;
    END;

    IF @AttributeType NOT IN (1, 2, 3, 4)
    BEGIN
        RAISERROR('Attribute type is invalid.', 16, 1);
        RETURN -3;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Attributes
        WHERE Id = @Id
          AND IsDeleted = 0
    )
    BEGIN
        RAISERROR('Attribute was not found or has been deleted.', 16, 1);
        RETURN -4;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.Attributes
        WHERE AttributeName = @AttributeName
          AND Id <> @Id
          AND IsDeleted = 0
    )
    BEGIN
        RAISERROR('Attribute with the same name already exists.', 16, 1);
        RETURN -5;
    END;

    UPDATE dbo.Attributes
    SET
        AttributeName = @AttributeName,
        AttributeType = @AttributeType,
        UpdatedDate = GETDATE()
    WHERE Id = @Id
      AND IsDeleted = 0;

    RETURN 0;
END;