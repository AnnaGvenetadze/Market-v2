create procedure sp_GetClients
as
begin
    select
        c.Id,
        c.AccountId,
        c.ClientTypeId,
        c.FirstName,
        c.LastName,
        c.PhoneNumber,
        c.ContactEmail,
        c.IsDeleted,
        c.CreateDate,
        c.UpdateDate
    from Clients c
    where c.IsDeleted = 0
    order by c.Id;
end
go