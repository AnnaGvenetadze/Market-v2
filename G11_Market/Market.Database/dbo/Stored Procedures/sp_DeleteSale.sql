CREATE   PROCEDURE dbo.sp_DeleteSale
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.Sales WHERE Id = @Id;
END;