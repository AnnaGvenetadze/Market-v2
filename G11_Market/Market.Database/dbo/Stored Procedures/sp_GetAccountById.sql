
CREATE   PROCEDURE dbo.sp_GetAccountById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM Accounts
    WHERE Id = @Id
      AND IsDeleted = 0;
END;