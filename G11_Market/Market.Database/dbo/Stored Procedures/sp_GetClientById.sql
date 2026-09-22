create procedure dbo.sp_GetClientById
    @Id int
as
begin
    set nocount on;

    select *
    from Clients
    where Id = @Id and IsDeleted = 0;

    return 0;
end