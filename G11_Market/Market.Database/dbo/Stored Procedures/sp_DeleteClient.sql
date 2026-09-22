CREATE PROCEDURE sp_DeleteClient
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    IF @Id IS NULL OR @Id <= 0
        THROW 50000, 'Invalid Client Id.', 1;

    UPDATE Clients
    SET IsDeleted = 1,
        UpdateDate = GETDATE()
    WHERE Id = @Id AND IsDeleted = 0;

    IF @@ROWCOUNT = 0
        THROW 50000, 'Client not found or has been deleted.', 1;
END;