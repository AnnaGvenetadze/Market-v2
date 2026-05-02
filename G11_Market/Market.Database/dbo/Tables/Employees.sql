CREATE TABLE [dbo].[Employees]
(
    Id int identity(1,1),
    AccountId int not null,
    ManagerEmployeeId int null,
    FirstName nvarchar(100) not null,
    LastName nvarchar(100) not null,
    PhoneNumber varchar(20) null unique,
    ContactEmail nvarchar(255) null unique,
    EmployeeCode nvarchar(50) not null unique,
    HireDate date not null,
    IsDeleted bit not null default(0),
    CreateDate datetime not null default(GetDate()),
    UpdateDate datetime null,
    primary key (Id),
    foreign key (AccountId) references Accounts(Id),
    foreign key (ManagerEmployeeId) references Employees(Id)
)
