create procedure dbo.sp_GetSalesByStatus
    @Status tinyint
as
begin
    set nocount on;

    if @Status not in (0, 1, 2)

    begin
        raiserror('Invalid status value. Status must be 0 (Draft), 1 (Completed), or 2 (Cancelled).', 16, 1);
        return;
    end

    select
        Id,
        CreatedEmployeeId,
        CancelledByEmployeeId,
        Status,
        CreatedDate,
        CancelledDate,
        CancelReason

        from dbo.Sales
        where Status = @status
        order by CreatedDate, Id;
end;
go