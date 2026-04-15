create table Cities
(
    Id int identity(1,1) primary key,
    Name nvarchar(100) not null,
    CountryId int not null,
    IsDeleted bit not null default(0),
    CreateDate datetime not null default(GetDate()),
    UpdateDate datetime null,
    foreign key (CountryId) references Countries(Id)
);