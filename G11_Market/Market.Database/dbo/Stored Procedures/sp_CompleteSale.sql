create procedure dbo.sp_CompleteSale
    @SaleId int
as
begin
    set nocount on;

    if not exists (select 1 from Sales where Id = @SaleId) throw 50111, 'Sale not found.', 1;
    if exists (select 1 from Sales where Id = @SaleId and Status = 2) throw 50112, 'Cancelled sale cannot be completed.', 1;
    if exists (select 1 from Sales where Id = @SaleId and Status = 1) throw 50113, 'Sale is already completed.', 1;

    update Sales
    set Status = 1
    where Id = @SaleId;

    return 0;
end