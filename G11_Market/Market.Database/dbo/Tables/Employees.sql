CREATE TABLE [dbo].[Employees] (
    [Id]                INT           IDENTITY (1, 1) NOT NULL,
    [AccountId]         INT           NOT NULL,
    [ManagerEmployeeId] INT           NULL,
    [EmployeeCode]      NVARCHAR (50) NOT NULL,
    [HireDate]          DATE          NOT NULL,
    [IsDeleted]         BIT           DEFAULT ((0)) NOT NULL,
    [CreateDate]        DATETIME      DEFAULT (getdate()) NOT NULL,
    [UpdateDate]        DATETIME      NULL,
    [FirstName]         NVARCHAR (50) NOT NULL,
    [LastName]          NVARCHAR (50) NOT NULL,
    [Email]             NVARCHAR (50) NOT NULL,
    [PhoneNumber]       NVARCHAR (20) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([AccountId]) REFERENCES [dbo].[Accounts] ([Id]),
    FOREIGN KEY ([ManagerEmployeeId]) REFERENCES [dbo].[Employees] ([Id]),
    UNIQUE NONCLUSTERED ([EmployeeCode] ASC),
    CONSTRAINT [UQ_Employees_AccountId] UNIQUE NONCLUSTERED ([AccountId] ASC)
);

