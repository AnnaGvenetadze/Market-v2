create table Users(
    Id int identity(1,1) primary key,
    Username nvarchar(50) not null unique,
    PasswordHash nvarchar(255) not null,
    Email nvarchar(255) not null unique,
    FirstName nvarchar(50) not null,
    LastName nvarchar(50) not null,
    IsDeleted bit not null default 0,
    CreateDate datetime not null default getdate(),
    UpdateDate datetime null
);