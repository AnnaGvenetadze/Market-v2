CREATE TABLE [dbo].[ProductAttributeValues] (
    [ProductId]    INT             NOT NULL,
    [AttributeId]  INT             NOT NULL,
    [TextValue]    NVARCHAR (500)  NULL,
    [NumberValue]  DECIMAL (18, 2) NULL,
    [DateValue]    DATETIME        NULL,
    [BooleanValue] BIT             NULL,
    PRIMARY KEY CLUSTERED ([ProductId] ASC, [AttributeId] ASC),
    CONSTRAINT [CK_ProductAttributeValues_ExactlyOneValue] CHECK ((((case when [TextValue] IS NOT NULL then (1) else (0) end+case when [NumberValue] IS NOT NULL then (1) else (0) end)+case when [DateValue] IS NOT NULL then (1) else (0) end)+case when [BooleanValue] IS NOT NULL then (1) else (0) end)=(1)),
    CONSTRAINT [CK_ProductAttributeValues_TextNotEmpty] CHECK ([TextValue] IS NULL OR len(ltrim(rtrim([TextValue])))>(0)),
    FOREIGN KEY ([AttributeId]) REFERENCES [dbo].[Attributes] ([Id]),
    FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_ProductAttributeValues_AttributeId]
    ON [dbo].[ProductAttributeValues]([AttributeId] ASC);

