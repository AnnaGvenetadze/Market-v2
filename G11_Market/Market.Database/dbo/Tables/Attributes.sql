CREATE TABLE [dbo].[Attributes] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [AttributeName] NVARCHAR (100) NOT NULL,
    [AttributeType] TINYINT        NOT NULL,
    [IsDeleted]     BIT            DEFAULT ((0)) NOT NULL,
    [CreatedDate]   DATETIME       DEFAULT (getdate()) NOT NULL,
    [UpdatedDate]   DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CHECK ([AttributeType]=(4) OR [AttributeType]=(3) OR [AttributeType]=(2) OR [AttributeType]=(1)),
    UNIQUE NONCLUSTERED ([AttributeName] ASC)
);

