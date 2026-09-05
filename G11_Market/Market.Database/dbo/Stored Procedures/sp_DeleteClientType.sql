CREATE PROCEDURE dbo.sp_DeleteClientType
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.ClientTypes
    SET
        IsDeleted = 1,
        UpdateDate = GETDATE()
    WHERE Id = @Id
      AND IsDeleted = 0;

    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('Client type with Id %d was not found or has already been deleted.', 16, 1, @Id);
    END;
END;
GO
