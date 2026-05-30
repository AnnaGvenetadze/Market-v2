create procedure dbo.sp_GetSalesByStatus
    @Status tinyint
as
begin
    set nocount on;

    select
        Id,
        CreatedEmployeeId,
        CancelledByEmployeeId,
        Status,
        CreatedAt,
        CancelledAt,
        CancelReason
    from dbo.Sales
    where Status = @Status
    order by CreatedAt, Id;
end;
go