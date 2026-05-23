create procedure sp_DeleteAccountById
	@AccountID int
as
begin
	set nocount on;
	update Accounts
	set
		IsDeleted = 1,
		UpdateDate = getdate() 
	where Id = @AccountID
		and IsDeleted = 0;
	if @@ROWCOUNT = 0
	begin
        ;throw 50001, N'Account does not exist or has already been deleted', 1;
    end

end
go
