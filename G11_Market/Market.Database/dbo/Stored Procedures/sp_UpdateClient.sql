create procedure dbo.sp_UpdateClient
    @Id int,
    @AccountId int,
    @ClientTypeId int,
    @FirstName nvarchar(100),
    @LastName nvarchar(100),
    @PhoneNumber varchar(20),
    @ContactEmail nvarchar(255)
as 
begin 
    set nocount on;
    set @FirstName = nullif(trim(@FirstName), '');
    set @LastName = nullif(trim(@LastName), '');
    set @PhoneNumber = nullif(trim(@PhoneNumber), '');
    set @ContactEmail = nullif(trim(@ContactEmail), '');

    if @Id is null throw 50033, 'Client ID is required for updates.', 1;
    if @AccountId is null throw 50013, 'AccountId is required.', 1;
    if @ClientTypeId is null throw 50012, 'ClientTypeId is required.', 1;
    if @FirstName is null throw 50010, 'FirstName cannot be empty.', 1;
    if @LastName is null throw 50011, 'LastName cannot be empty.', 1;
    if @ContactEmail is not null and @ContactEmail not like '%_@_%._%' throw 50015, 'Invalid email format.', 1;
    if @PhoneNumber is not null and @PhoneNumber like '%[^0-9+ -]%' throw 50020, 'PhoneNumber contains invalid characters.', 1;
    if not exists (select 1 from Clients where Id = @Id and IsDeleted = 0) throw 50034, 'Client not found or has been deleted.', 1;
    if not exists (select 1 from Accounts where Id = @AccountId and IsDeleted = 0) throw 50016, 'Invalid AccountId.', 1;
    if not exists (select 1 from ClientTypes where Id = @ClientTypeId) throw 50017, 'Invalid ClientTypeId.', 1;
    if @ContactEmail is not null and exists (select 1 from Clients where ContactEmail = @ContactEmail and Id <> @Id and IsDeleted = 0) throw 50021, 'Email already exists.', 1;
    if @PhoneNumber is not null and exists (select 1 from Clients where PhoneNumber = @PhoneNumber and Id <> @Id and IsDeleted = 0) throw 50022, 'Phone number already exists.', 1;

    update Clients
    set AccountId = @AccountId,
        ClientTypeId = @ClientTypeId,
        FirstName = @FirstName,
        LastName = @LastName,
        PhoneNumber = @PhoneNumber,
        ContactEmail = @ContactEmail,
        UpdateDate = getdate()
    where Id = @Id;
      
    return 0;
end