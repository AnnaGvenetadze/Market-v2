create procedure sp_GetClients
as
begin
    select
    *
    from Clients c
    where c.IsDeleted = 0
    order by c.Id;
end
go