create procedure sp_UpdateCountry
    @Id int, 
    @Name nvarchar(100), 
    @CountryCode varchar(3)
as
begin 
    set nocount on;

    update Countries
    set Name = @Name, 
        CountryCode = @CountryCode, 
        UpdateDate = GetDate() 
    where Id = @Id and IsDeleted = 0;

    return 0;
end