CREATE   PROCEDURE dbo.sp_LoginAccount
    @Username NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SET @Username = LTRIM(RTRIM(@Username));
    IF @Username IS NULL OR @Username = '' THROW 50050, 'Username is required.', 1;
    SELECT * FROM dbo.Accounts WHERE Username = @Username AND IsDeleted = 0;
END;