CREATE TABLE [dbo].[Accounts](
    [Id]                  INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Username]            NVARCHAR(50)  NOT NULL UNIQUE,
    [PasswordHash]        NVARCHAR(255) NOT NULL,
    [Email]               NVARCHAR(255) NOT NULL UNIQUE,
    [AccountType]         TINYINT       NOT NULL,
    [FirstName]           NVARCHAR(50)  NOT NULL,
    [LastName]            NVARCHAR(50)  NOT NULL,
    [Hwid]                VARCHAR(64)   NOT NULL DEFAULT '',
    [FailedLoginAttempts] INT           NOT NULL DEFAULT 0,
    [LockoutTime]         DATETIME      NULL,
    [IsDeleted]           BIT           NOT NULL DEFAULT 0,
    [CreateDate]          DATETIME      NOT NULL DEFAULT GETDATE(),
    [UpdateDate]          DATETIME      NULL
);