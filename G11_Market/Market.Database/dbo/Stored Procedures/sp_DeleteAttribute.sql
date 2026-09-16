CREATE PROCEDURE sp_DeleteAttribute
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Attributes
    SET IsDeleted = 1,
        UpdatedDate = GETDATE()
    WHERE Id = @Id AND IsDeleted = 0;

    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('Attribute with Id %d was not found or is already deleted.', 16, 1, @Id);
    END
END;