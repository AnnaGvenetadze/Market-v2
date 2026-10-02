CREATE TABLE [dbo].[Products] (
    [Id]          INT             IDENTITY (1, 1) NOT NULL,
    [CategoryId]  INT             NOT NULL,
    [ProductName] NVARCHAR (100)  NOT NULL,
    [Price]       DECIMAL (18, 2) NOT NULL,
    [IsDeleted]   BIT             DEFAULT ((0)) NOT NULL,
    [CreatedDate] DATETIME        DEFAULT (getdate()) NOT NULL,
    [UpdatedDate] DATETIME        NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [CK_Products_Price_NonNegative] CHECK ([Price]>=(0)),
    FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories] ([Id]),
    UNIQUE NONCLUSTERED ([ProductName] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_Products_CategoryId]
    ON [dbo].[Products]([CategoryId] ASC);

