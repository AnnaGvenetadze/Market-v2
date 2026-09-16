CREATE PROCEDURE sp_UpdateAttribute
    @Id INT,
    @AttributeName NVARCHAR(100),
    @AttributeType TINYINT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Attributes WHERE Id = @Id AND IsDeleted = 0)
    BEGIN
        RAISERROR('Attribute with Id %d was not found or is deleted.', 16, 1, @Id);
        RETURN;
    END

    IF @AttributeName IS NULL OR LTRIM(RTRIM(@AttributeName)) = ''
    BEGIN
        RAISERROR('AttributeName cannot be empty.', 16, 1);
        RETURN;
    END

    IF @AttributeType NOT IN (1, 2, 3, 4)
    BEGIN
        RAISERROR('Invalid AttributeType. Must be 1, 2, 3, or 4.', 16, 1);
        RETURN;
    END

    IF EXISTS (
        SELECT 1 
        FROM Attributes 
        WHERE AttributeName = @AttributeName 
          AND Id <> @Id 
          AND IsDeleted = 0
    )
    BEGIN
        RAISERROR('Another active attribute with this name already exists.', 16, 1);
        RETURN;
    END

    UPDATE Attributes
    SET AttributeName = @AttributeName,
        AttributeType = @AttributeType,
        UpdatedDate = GETDATE()
    WHERE Id = @Id AND IsDeleted = 0;
END;
