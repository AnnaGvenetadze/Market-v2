create procedure dbo.sp_GetSaleItemsBySaleId
    @SaleId int
as
begin
    set nocount on;

    select
        Id,
        SaleId,
        ProductId,
        Quantity,
        UnitPrice,
        TotalPrice
    from dbo.SaleItems
    where SaleId = @SaleId
    order by Id;
end;
go  