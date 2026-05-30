create or alter procedure dbo.sp_DeactivateProduct
    @ProductID int
as
begin
    set nocount on;

    update dbo.Products
    set
        IsActive = 0,
        UpdatedDate = getdate()
    where ID = @ProductID;

    if @@rowcount = 0
    begin
        raiserror('product not found.', 16, 1);
        return;
    end;
end;
go