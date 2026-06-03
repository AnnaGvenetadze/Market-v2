CREATE PROCEDURE sp_ValidateAttribute
    @AttributeId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM Attributes
        WHERE Id = @AttributeId
    )
    BEGIN
        RAISERROR('Attribute was not found.', 16, 1);
        RETURN -1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM Attributes
        WHERE Id = @AttributeId
          AND IsDeleted = 1
    )
    BEGIN
        RAISERROR('Attribute is inactive.', 16, 1);
        RETURN -2;
    END;

    RETURN 0;
END;