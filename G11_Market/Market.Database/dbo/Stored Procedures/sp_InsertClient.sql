create procedure dbo.sp_InsertClient
	@AccountId int,
	@ClientTypeId int,
	@FirstName nvarchar(50),
	@LastName nvarchar(50),
	@PhoneNumber varchar(20) = null,
	@ContactEmail nvarchar(255) = null
as
begin
	set nocount on;
	insert into Clients (AccountId, ClientTypeId, FirstName, LastName, PhoneNumber, ContactEmail)
	values (@AccountId, @ClientTypeId, @FirstName, @LastName, @PhoneNumber, @ContactEmail);

	return 0;
end

