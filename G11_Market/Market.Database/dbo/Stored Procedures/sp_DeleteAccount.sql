
CREATE   PROCEDURE dbo.sp_DeleteAccount
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Accounts
    SET IsDeleted = 1,
        UpdateDate = GETUTCDATE()
    WHERE Id = @Id AND IsDeleted = 0;

    IF @@ROWCOUNT = 0
        RAISERROR('Account with Id %d was not found or is already deleted.', 16, 1, @Id);
END;
GO