create table Countries
(
    Id int identity(1,1) primary key,
    Name nvarchar(100) not null,
    CountryCode varchar(3) not null unique,
    IsDeleted bit not null default(0),
    CreateDate datetime not null default(GetDate()),
    UpdateDate datetime null
);