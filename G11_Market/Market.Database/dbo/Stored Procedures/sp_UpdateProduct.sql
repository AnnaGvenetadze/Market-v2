create procedure dbo.sp_UpdateProduct
    @ProductId int,
    @CategoryId int,
    @ProductName nvarchar(100),
    @Price decimal(18, 2)
as
begin
    set nocount on;

    update dbo.Products
    set
        CategoryId = @CategoryId,
        ProductName = @ProductName,
        Price = @Price,
        UpdatedDate = getdate()
    where Id = @ProductId;

    select @ProductId as ProductId;
end;
go