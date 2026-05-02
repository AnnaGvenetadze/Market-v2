CREATE TABLE [dbo].[InventoryManagerDetails]
(
	Id int not null,
    StockAdjustmentLimit decimal(18,2) not null,
    CanApproveStockCorrection bit not null default(0),
    CanApproveNegativeStock bit not null default(0),
    IsDeleted bit not null default(0),
    CreateDate datetime not null default(GetDate()),
    UpdateDate datetime null,
    primary key (Id),
    foreign key (Id) references Employees(Id)
)
