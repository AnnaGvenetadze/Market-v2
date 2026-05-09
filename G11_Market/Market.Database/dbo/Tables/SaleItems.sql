CREATE TABLE [dbo].[SaleItems]
(
	Id              int identity(1,1) primary key,
    SaleId          int not null,
    ProductId       int not null,
    Quantity        int not null check (Quantity > 0),
    UnitPrice       money not null check (UnitPrice >= 0),
    DiscountAmount  money not null default 0 check (DiscountAmount >= 0), -- we need to create a separate table for discounts

    foreign key (SaleId) references dbo.Sales(Id),
    foreign key (ProductId) references dbo.Products(Id),

    unique (SaleId, ProductId)
);
