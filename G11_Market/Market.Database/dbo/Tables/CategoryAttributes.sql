CREATE TABLE [dbo].[CategoryAttributes] (
    [CategoryId]    INT NOT NULL,
    [AttributeId]   INT NOT NULL,
    [OrderPosition] INT DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([CategoryId] ASC, [AttributeId] ASC),
    FOREIGN KEY ([AttributeId]) REFERENCES [dbo].[Attributes] ([Id]),
    FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_CategoryAttributes_AttributeId]
    ON [dbo].[CategoryAttributes]([AttributeId] ASC);

