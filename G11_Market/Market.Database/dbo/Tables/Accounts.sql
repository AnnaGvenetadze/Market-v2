CREATE TABLE [dbo].[Accounts] (
    [Id]                  INT            IDENTITY (1, 1) NOT NULL,
    [Username]            NVARCHAR (50)  NOT NULL,
    [PasswordHash]        NVARCHAR (255) NOT NULL,
    [IsDeleted]           BIT            DEFAULT ((0)) NOT NULL,
    [CreateDate]          DATETIME       DEFAULT (getdate()) NOT NULL,
    [UpdateDate]          DATETIME       NULL,
    [LastLoginAtUtc]      DATETIME2 (7)  NULL,
    [FailedLoginAttempts] INT            CONSTRAINT [DF_Accounts_FailedLoginAttempts] DEFAULT ((0)) NOT NULL,
    [LockoutEndUtc]       DATETIME2 (7)  NULL,
    [IsActive]            BIT            NOT NULL,
    [Token]          NVARCHAR (255) NULL,
    [TokenExpiration] DATETIME2 (7) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    UNIQUE NONCLUSTERED ([Username] ASC)
);

