CREATE   PROCEDURE dbo.sp_RestoreAccount
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Accounts
    SET IsDeleted = 0,
        UpdateDate = GETUTCDATE()
    WHERE Id = @Id AND IsDeleted = 1;

    IF @@ROWCOUNT = 0
        RAISERROR('Account with Id %d was not found or is not deleted.', 16, 1, @Id);
END;