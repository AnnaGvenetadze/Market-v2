CREATE PROCEDURE sp_DeleteCity
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    IF @Id IS NULL OR @Id <= 0
    BEGIN
        THROW 50000, 'Invalid City Id.', 1;
    END

    UPDATE Cities
    SET IsDeleted = 1,
        UpdateDate = GETDATE()
    WHERE Id = @Id AND IsDeleted = 0;

    IF @@ROWCOUNT = 0
    BEGIN
        THROW 50000, 'City not found or already deleted.', 1;
    END
END;