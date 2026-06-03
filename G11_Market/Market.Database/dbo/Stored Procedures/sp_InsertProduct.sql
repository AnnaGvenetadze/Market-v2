create procedure dbo.sp_InsertProduct
    @CategoryId int,
    @ProductName nvarchar(100),
    @Price decimal(18, 2)
as
begin
    set nocount on;

    insert into dbo.Products (
        CategoryId,
        ProductName,
        Price
    )
    values (
        @CategoryId,
        @ProductName,
        @Price
    );

    select scope_identity() as ProductId;
end;
go