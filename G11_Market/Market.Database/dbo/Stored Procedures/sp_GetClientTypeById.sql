CREATE PROCEDURE sp_GetClientTypeById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id, Name, Description, IsDeleted, CreateDate, UpdateDate
    FROM ClientTypes
    WHERE Id = @Id AND IsDeleted = 0;
END;