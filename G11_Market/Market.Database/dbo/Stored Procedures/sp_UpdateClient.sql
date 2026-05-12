create procedure dbo.sp_UpdateClient
    @Id            int,
    @AccountId     int,
    @ClientTypeId  int,
    @FirstName     nvarchar(100),
    @LastName      nvarchar(100),
    @PhoneNumber   varchar(20) = null,
    @ContactEmail  nvarchar(255) = null

as
begin
    set nocount on;

    if not exists 
       (select 1 from dbo.Accounts where Id = @AccountId and IsDeleted = 0)
        
       begin
        ;throw 50001, N'The specified account does not exist or has been deleted.', 1;
       end
    
    if not exists
       (select 1 from dbo.ClientTypes where Id = @ClientTypeId and IsDeleted = 0)
       
       begin
        ;throw 50002, N'The specified client type does not exist or has been deleted.', 1;
       end

    if exists 
       (select 1 from dbo.Clients where AccountId = @AccountId and Id != @Id and IsDeleted = 0)
       
       begin
        ;throw 50003, N'This account is already assigned to another client.', 1;
       end
       

    update dbo.Clients
    set
        AccountId    = @AccountId,
        ClientTypeId = @ClientTypeId,
        FirstName    = @FirstName,
        LastName     = @LastName,
        PhoneNumber  = @PhoneNumber,
        ContactEmail = @ContactEmail,
        UpdateDate   = getdate()
    where Id = @Id
      and IsDeleted = 0;

end
go