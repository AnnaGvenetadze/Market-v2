CREATE TABLE [dbo].[Sales] (
    [Id]                    INT            IDENTITY (1, 1) NOT NULL,
    [CreatedEmployeeId]     INT            NOT NULL,
    [CancelledByEmployeeId] INT            NULL,
    [Status]                TINYINT        NOT NULL,
    [CreatedDate]           DATETIME       DEFAULT (getdate()) NOT NULL,
    [CancelledDate]         DATETIME       NULL,
    [CancelReason]          NVARCHAR (200) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CHECK ([CancelReason] IS NULL OR len(ltrim(rtrim([CancelReason])))>(0)),
    CHECK ([Status]=(2) OR [Status]=(1) OR [Status]=(0)),
    CONSTRAINT [CK_Sales_CancelledDate] CHECK ([CancelledDate] IS NULL OR [CancelledDate]>=[CreatedDate]),
    FOREIGN KEY ([CancelledByEmployeeId]) REFERENCES [dbo].[Employees] ([Id]),
    FOREIGN KEY ([CreatedEmployeeId]) REFERENCES [dbo].[Employees] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Sales_CreatedEmployeeId]
    ON [dbo].[Sales]([CreatedEmployeeId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Sales_CancelledByEmployeeId]
    ON [dbo].[Sales]([CancelledByEmployeeId] ASC);

