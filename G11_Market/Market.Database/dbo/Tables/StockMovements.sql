CREATE TABLE [dbo].[StockMovements]
(
	Id                  int identity(1,1) primary key,
    ProductId           int not null,
    MovementType        tinyint not null check (MovementType in (0, 1, 2)), -- 0 = Sale, 1 = Refill, 2 = Adjustment
    QuantityChange      int not null check (QuantityChange <> 0),
    QuantityBefore      int not null check (QuantityBefore >= 0),
    SaleItemId          int null,
    ChangedByEmployeeId int not null,
    Reason              nvarchar(200) null check (Reason is null or len(ltrim(rtrim(Reason))) > 0),
    CreatedDate          datetime not null default getdate(),

    foreign key (ProductId) references dbo.Products(Id),
    foreign key (SaleItemId) references dbo.SaleItems(Id),
    foreign key (ChangedByEmployeeId) references dbo.Employees(Id),

    check (QuantityBefore + QuantityChange >= 0)
);
