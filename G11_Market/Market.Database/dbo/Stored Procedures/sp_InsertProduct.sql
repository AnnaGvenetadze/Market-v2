create or alter procedure dbo.sp_InsertProduct
    @CategoryID int,
    @ProductName nvarchar(100),
    @Price decimal(18, 2)
as
begin
    set nocount on;

    insert into dbo.Products (
        CategoryID,
        ProductName,
        Price
    )
    values (
        @CategoryID,
        @ProductName,
        @Price
    );

    select scope_identity() as ProductID;
end;
go