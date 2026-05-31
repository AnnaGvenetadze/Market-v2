create procedure dbo.sp_GetSaleItemsBySaleId
    @Id int
as
begin
    set nocount on;

    select
        sd.Id,
        sd.SaleId,
        sd.ProductId,
        p.ProductName,
        sd.Quantity,
        sd.UnitPrice,
        sd.Quantity * sd.UnitPrice as TotalPrice

        from dbo.SaleItems sd
        inner join dbo.Products p on sd.ProductId = p.Id
        where sd.SaleId = @Id;
end;
go  