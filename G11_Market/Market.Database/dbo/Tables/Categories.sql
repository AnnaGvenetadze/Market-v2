CREATE TABLE [dbo].[Categories] (
    [Id]           INT             IDENTITY (1, 1) NOT NULL,
    [ParentId]     INT             NULL,
    [CategoryName] NVARCHAR (100)  NOT NULL,
    [Description]  NVARCHAR (1000) NULL,
    [IsDeleted]    BIT             DEFAULT ((0)) NOT NULL,
    [CreatedDate]  DATETIME        DEFAULT (getdate()) NOT NULL,
    [UpdatedDate]  DATETIME        NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([ParentId]) REFERENCES [dbo].[Categories] ([Id]),
    UNIQUE NONCLUSTERED ([CategoryName] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_Categories_ParentId]
    ON [dbo].[Categories]([ParentId] ASC);

