CREATE   PROCEDURE dbo.sp_RestoreAttribute
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Attributes
    SET IsDeleted = 0
    WHERE Id = @Id AND IsDeleted = 1;

    IF @@ROWCOUNT = 0
        RAISERROR('Attribute with Id %d was not found or is not deleted.', 16, 1, @Id);
END;
GO