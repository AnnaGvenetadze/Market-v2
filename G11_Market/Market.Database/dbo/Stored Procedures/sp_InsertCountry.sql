create procedure sp_InsertCountry
    @Name nvarchar(100), 
    @CountryCode varchar(3), 
    @Id int output
as 
begin 
    set nocount on;

    insert into Countries (Name, CountryCode) 
    values (@Name, @CountryCode);

    set @Id = scope_identity();

    return 0;
end