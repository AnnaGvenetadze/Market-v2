create procedure dbo.sp_GetSalesByDateRange
    @DateFrom datetime,
    @DateTo   datetime
as
begin
    set nocount on;

    if @DateFrom > @DateTo
    begin
        raiserror(N'DateFrom cannot be greater than DateTo.', 16, 1);
        return;
    end;
    select
        Id,
        CreatedEmployeeId,
        CancelledByEmployeeId,
        Status,
        CreatedAt,
        CancelledAt,
        CancelReason
    from dbo.Sales
    where CreatedAt >= @DateFrom
      and CreatedAt <= @DateTo
    order by CreatedAt, Id;
end;
go