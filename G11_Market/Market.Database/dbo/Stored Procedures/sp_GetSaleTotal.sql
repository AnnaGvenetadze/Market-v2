create procedure dbo.sp_GetSaleTotal
    @Id int
as
begin
    set nocount on;

    select
        s.Id,
        sum(si.Quantity * si.UnitPrice - si.DiscountAmount) as Total

        from dbo.Sales s
        inner join dbo.SaleItems si on s.Id = si.SaleId
        where s.Id = @Id
        group by s.Id;
end;
go