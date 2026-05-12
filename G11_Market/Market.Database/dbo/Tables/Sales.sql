CREATE TABLE [dbo].[Sales]
(
	Id                      int identity(1,1) primary key,
    CreatedEmployeeId       int not null,
    CancelledByEmployeeId   int null,
    Status                  tinyint not null check (Status in (0, 1, 2)), -- 0 = Draft, 1 = Completed, 2 = Cancelled
    CreatedDate               datetime not null default getdate(),
    CancelledDate             datetime null,
    CancelReason            nvarchar(200) null check (CancelReason is null or len(ltrim(rtrim(CancelReason))) > 0),

    foreign key (CreatedEmployeeId) references dbo.Employees(Id),
    foreign key (CancelledByEmployeeId) references dbo.Employees(Id)
)
