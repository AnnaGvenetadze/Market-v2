create table Countries
(
    Id int identity(1,1) primary key,
    Name nvarchar(100) not null
        check (trim(Name) = name and name <> ''),
    CountryCode varchar(3) not null unique
        check (trim(CountryCode) = CountryCode and len(CountryCode) >= 2),
    IsDeleted bit not null default(0),
    CreateDate datetime not null default(GetDate()),
    UpdateDate datetime null
        check (UpdateDate >= CreateDate)
);