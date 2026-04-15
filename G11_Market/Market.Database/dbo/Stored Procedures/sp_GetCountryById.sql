create procedure sp_GetCountryById
    @Id int
as
begin
    set nocount on;

    if not exists (select 1 from Countries where Id = @Id and IsDeleted = 0)
    begin
        raiserror('Country with Id %d not found.', 16, 1, @Id);
        return -1;
    end

    select *
    from Countries 
    where Id = @Id and IsDeleted = 0;

    return 0;
end