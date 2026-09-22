CREATE PROCEDURE sp_UpdateClientType
    @Id INT,
    @Name NVARCHAR(100),
    @Description NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Id IS NULL OR @Id <= 0
        THROW 50000, 'Invalid ClientType Id.', 1;

    IF @Name IS NULL OR TRIM(@Name) = ''
        THROW 50000, 'ClientType Name cannot be empty.', 1;

    IF EXISTS (SELECT 1 FROM ClientTypes WHERE Name = TRIM(@Name) AND Id <> @Id AND IsDeleted = 0)
        THROW 50000, 'A ClientType with this name already exists.', 1;

    UPDATE ClientTypes
    SET Name = TRIM(@Name),
        Description = TRIM(@Description),
        UpdateDate = GETDATE()
    WHERE Id = @Id AND IsDeleted = 0;

    IF @@ROWCOUNT = 0
        THROW 50000, 'ClientType not found or already deleted.', 1;
END;