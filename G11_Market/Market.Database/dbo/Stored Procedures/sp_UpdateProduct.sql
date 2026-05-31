create procedure dbo.sp_UpdateProduct
    @ProductID int,
    @CategoryID int,
    @ProductName nvarchar(100),
    @Price decimal(18, 2)
as
begin
    set nocount on;

    update dbo.Products
    set
        CategoryID = @CategoryID,
        ProductName = @ProductName,
        Price = @Price,
        UpdatedDate = getdate()
    where ID = @ProductID;

    select @ProductID as ProductID;
end;
go