CREATE TABLE [dbo].[StockMovements] (
    [Id]                  INT            IDENTITY (1, 1) NOT NULL,
    [ProductId]           INT            NOT NULL,
    [MovementType]        TINYINT        NOT NULL,
    [QuantityChange]      INT            NOT NULL,
    [QuantityBefore]      INT            NOT NULL,
    [SaleItemId]          INT            NULL,
    [ChangedByEmployeeId] INT            NOT NULL,
    [Reason]              NVARCHAR (200) NULL,
    [CreatedDate]         DATETIME       DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CHECK (([QuantityBefore]+[QuantityChange])>=(0)),
    CHECK ([MovementType]=(2) OR [MovementType]=(1) OR [MovementType]=(0)),
    CHECK ([QuantityBefore]>=(0)),
    CHECK ([QuantityChange]<>(0)),
    CHECK ([Reason] IS NULL OR len(ltrim(rtrim([Reason])))>(0)),
    FOREIGN KEY ([ChangedByEmployeeId]) REFERENCES [dbo].[Employees] ([Id]),
    FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products] ([Id]),
    FOREIGN KEY ([SaleItemId]) REFERENCES [dbo].[SaleItems] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_StockMovements_SaleItemId]
    ON [dbo].[StockMovements]([SaleItemId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_StockMovements_ProductId]
    ON [dbo].[StockMovements]([ProductId] ASC, [Id] ASC)
    INCLUDE([QuantityChange]);


GO
CREATE NONCLUSTERED INDEX [IX_StockMovements_ChangedByEmployeeId]
    ON [dbo].[StockMovements]([ChangedByEmployeeId] ASC);

