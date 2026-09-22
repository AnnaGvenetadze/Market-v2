CREATE PROCEDURE sp_InsertClientType
    @Name NVARCHAR(100),
    @Description NVARCHAR(MAX) = NULL,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF @Name IS NULL OR TRIM(@Name) = ''
        THROW 50000, 'ClientType Name cannot be empty.', 1;

    IF EXISTS (SELECT 1 FROM ClientTypes WHERE Name = TRIM(@Name) AND IsDeleted = 0)
        THROW 50000, 'A ClientType with this name already exists.', 1;

    INSERT INTO ClientTypes (Name, Description, IsDeleted, CreateDate)
    VALUES (TRIM(@Name), TRIM(@Description), 0, GETDATE());

    SET @Id = SCOPE_IDENTITY();

    SELECT @Id AS Id;
END;