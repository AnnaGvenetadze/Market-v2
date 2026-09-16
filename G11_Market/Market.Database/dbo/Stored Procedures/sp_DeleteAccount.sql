CREATE PROCEDURE sp_DeleteAccount
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Accounts
    SET IsDeleted = 1,
        UpdateDate = GETDATE()
    WHERE Id = @Id AND IsDeleted = 0;

    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('Account with Id %d was not found or is already deleted.', 16, 1, @Id);
    END;
END;
GO