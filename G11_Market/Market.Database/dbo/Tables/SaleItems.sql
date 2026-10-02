CREATE TABLE [dbo].[SaleItems] (
    [Id]             INT   IDENTITY (1, 1) NOT NULL,
    [SaleId]         INT   NOT NULL,
    [ProductId]      INT   NOT NULL,
    [Quantity]       INT   NOT NULL,
    [UnitPrice]      MONEY NOT NULL,
    [DiscountAmount] MONEY DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CHECK ([DiscountAmount]>=(0)),
    CHECK ([Quantity]>(0)),
    CHECK ([UnitPrice]>=(0)),
    CONSTRAINT [CK_SaleItems_DiscountAmount_Limit] CHECK ([DiscountAmount]<=[UnitPrice]),
    FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products] ([Id]),
    FOREIGN KEY ([SaleId]) REFERENCES [dbo].[Sales] ([Id]),
    UNIQUE NONCLUSTERED ([SaleId] ASC, [ProductId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_SaleItems_ProductId]
    ON [dbo].[SaleItems]([ProductId] ASC);

