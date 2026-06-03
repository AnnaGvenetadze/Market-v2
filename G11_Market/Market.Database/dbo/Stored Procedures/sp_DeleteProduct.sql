create procedure dbo.sp_DeleteProduct
    @ProductId int
as
begin
    set nocount on;

    update dbo.Products
    set
        IsDeleted = 0,
        UpdatedDate = getdate()
    where Id = @ProductId;

    if @@rowcount = 0
    begin
        raiserror('product not found.', 16, 1);
        return;
    end;
end;
go