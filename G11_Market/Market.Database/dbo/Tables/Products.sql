create table Products (
    Id          int identity(1, 1) not null,
    Name        nvarchar(255) not null unique,
    Description nvarchar(max) null,
    Price       decimal(18, 2) not null,
    CategoryId  int not null,
    primary key (Id),
    foreign key (CategoryId) references Categories(Id)
);