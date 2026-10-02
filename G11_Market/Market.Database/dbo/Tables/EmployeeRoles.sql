CREATE TABLE [dbo].[EmployeeRoles] (
    [EmployeeId] INT NOT NULL,
    [RoleId]     INT NOT NULL,
    PRIMARY KEY CLUSTERED ([EmployeeId] ASC, [RoleId] ASC),
    FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([Id]),
    FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_EmployeeRoles_RoleId]
    ON [dbo].[EmployeeRoles]([RoleId] ASC);

