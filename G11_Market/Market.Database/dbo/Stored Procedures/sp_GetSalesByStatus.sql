create procedure dbo.sp_GetSaleTotal
    @SaleId int
as
begin
    set nocount on;

    select
        SaleId = @SaleId,
        TotalAmount = isnull(SUM(TotalPrice), 0)
    from dbo.SaleItems
    where SaleId = @SaleId;
end;
go