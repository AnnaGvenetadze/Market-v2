CREATE   PROCEDURE dbo.sp_DeleteAttribute
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Attributes
    SET IsDeleted = 1,
        UpdatedDate = GETUTCDATE()
    WHERE Id = @Id AND IsDeleted = 0;

    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('Attribute with Id %d was not found or is already deleted.', 16, 1, @Id);
    END;
END;