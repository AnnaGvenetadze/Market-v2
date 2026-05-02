CREATE TABLE [dbo].[ClientTypes]
(
	Id int identity(1,1),
    Name nvarchar(100) not null unique,
    Description nvarchar(max) null,
    IsDeleted bit not null default(0),
    CreateDate datetime not null default(GetDate()),
    UpdateDate datetime null,
    primary key (Id)
)
