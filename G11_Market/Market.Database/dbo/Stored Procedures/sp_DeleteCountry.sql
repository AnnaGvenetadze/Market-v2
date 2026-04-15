create procedure sp_DeleteCountry
    @Id int
as
begin
    set nocount on;

    update Countries
    set IsDeleted = 1, 
        UpdateDate = GetDate() 
    where Id = @Id and IsDeleted = 0;

    return 0;
end