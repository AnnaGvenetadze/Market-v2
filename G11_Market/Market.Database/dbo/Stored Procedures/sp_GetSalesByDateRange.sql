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
        CreatedDate,
        CancelledDate,
        CancelReason
    from dbo.Sales
    where CreatedDate >= @DateFrom
      and CreatedDate <= @DateTo
    order by CreatedDate, Id;
end;
go