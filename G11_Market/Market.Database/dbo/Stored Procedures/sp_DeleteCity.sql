CREATE PROCEDURE sp_DeleteCity
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Cities
    SET
        IsDeleted = 1,
        UpdateDate = GETDATE()
    WHERE Id = @Id
      AND IsDeleted = 0;
END