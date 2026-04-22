create table Categories (
    Id          int identity(1, 1) not null,
    Name        nvarchar(255) not null unique,
    Description nvarchar(max) null,
    ParentId    int null,
    primary key (Id),
    foreign key (ParentId) references Categories(Id)
);