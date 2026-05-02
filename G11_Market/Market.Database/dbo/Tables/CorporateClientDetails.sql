CREATE TABLE [dbo].[CorporateClientDetails]
(
	Id int not null,
    CompanyName nvarchar(255) not null,
    TaxNumber nvarchar(100) not null unique,
    LegalAddress nvarchar(max) null,
    ContactPersonName nvarchar(255) null,
    IsDeleted bit not null default(0),
    CreateDate datetime not null default(GetDate()),
    UpdateDate datetime null,
    primary key (Id),
    foreign key (Id) references Accounts(Id)
)
