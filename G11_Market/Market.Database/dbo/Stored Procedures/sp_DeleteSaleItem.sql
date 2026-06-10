create procedure dbo.sp_DeleteSaleItem
    @Id int
as
begin
    set nocount on;

    delete from dbo.SaleItems
    where Id = @Id;

    return 0;
end;
go