create procedure dbo.sp_InsertClient
    @AccountId int,
    @ClientTypeId int,
    @FirstName nvarchar(100),
    @LastName nvarchar(100),
    @PhoneNumber varchar(20),
    @ContactEmail nvarchar(255),
    @Id int output
as
begin
    set nocount on;
    set @FirstName = nullif(trim(@FirstName), '');
    set @LastName = nullif(trim(@LastName), '');
    set @ContactEmail = nullif(trim(@ContactEmail), '');
    set @PhoneNumber = nullif(trim(@PhoneNumber), '');

    if @FirstName is null throw 50010, 'FirstName cannot be empty.', 1;
    if @LastName is null throw 50011, 'LastName cannot be empty.', 1;
    if @ClientTypeId is null throw 50012, 'ClientTypeId is required.', 1;
    if @AccountId is null throw 50013, 'AccountId is required.', 1;
    if @ContactEmail is null throw 50014, 'Email cannot be empty.', 1;
    if @ContactEmail not like '%_@_%._%' throw 50015, 'Invalid email format.', 1;
    if @PhoneNumber is not null and @PhoneNumber like '%[^0-9+ -]%' throw 50020, 'PhoneNumber contains invalid characters.', 1;    
    if exists (select 1 from Clients where ContactEmail = @ContactEmail) throw 50021, 'Email already exists.', 1;
    if exists (select 1 from Clients where AccountId = @AccountId) throw 50006, 'Client for this AccountId already exists.', 1;
    if not exists (select 1 from Accounts where Id = @AccountId) throw 50016, 'Invalid AccountId.', 1;
    if not exists (select 1 from ClientTypes where Id = @ClientTypeId) throw 50017, 'Invalid ClientTypeId.', 1;

    insert into Clients (AccountId, ClientTypeId, FirstName, LastName, PhoneNumber, ContactEmail)
    values (@AccountId, @ClientTypeId, @FirstName, @LastName, @PhoneNumber, @ContactEmail);

    set @Id = scope_identity();

    return 0;
end