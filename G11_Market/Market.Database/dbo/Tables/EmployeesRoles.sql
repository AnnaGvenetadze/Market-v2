CREATE TABLE [dbo].[EmployeesRoles]
(
	EmployeeId int not null,
    RoleId int not null,
    primary key (EmployeeId, RoleId),
    foreign key (EmployeeId) references Employees(Id),
    foreign key (RoleId) references Roles(Id)
)
