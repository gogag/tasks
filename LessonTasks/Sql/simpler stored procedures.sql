create type SomeTableType as table (
	Id int primary key
)
go

create or alter procedure spDoSomething
--  @param int
--, @paramOutput int output
--, @paramTable SomeTableType readonly
  @param int = NULL
, @paramOutput int = -1 output
, @para varchar(10) = '1234'
as begin
	--declare @additionalVar int;
	declare @additionalVarTable table (
		Id int primary key
	);

	set @paramOutput = 100500;

	if @param > 2 begin
		print 'ha ha'
	end

	--if exists (select top 1 1 from @paramTable)

	declare @asdf int = min(1)

	--while 1=1 begin
	--	min(1) -- в каждом цикле свое состояние, не сохраняется между итерациями
	--	print 'never end'
	--end

    print 'asdf' -- инфа
	select 1 -- результирующий набор с данными
	return
	return 0
	return 1
end
go

declare @paramOutput int;
exec spDoSomething @param = 2, @paramOutput = @paramOutput output
select @paramOutput
go

create table Faculties (
  Id int primary key identity
, Name nvarchar(50) unique not null check(Name > '')
, Funding int NOT NULL check(Funding > 0)
)

create table Departments (
  FacultyId int not null foreign key references Faculties(Id)
, Name nvarchar(50) not null check(Name > '')
, Funding int NOT NULL check(Funding >= 0)
, constraint PK_Departments primary key (FacultyId, Name)
)
go

--alter table Departments
--add constraint PK_Departments1 primary key (FacultyId, Name)

create or alter procedure CreateOrUpdateFaculties
  @records nvarchar(max)
as begin

  SET XACT_ABORT ON; -- чтоб падало где надо. В триггерах ON по умолчанию
  SET NOCOUNT ON; -- чтоб не ело лишний траффик

  declare @recordsTable table (
    Id int NOT NULL
  , Name nvarchar(50)
  , Funding int
  )
  insert into @recordsTable
  select * from openjson(@records) with (
    Id int 'strict $.Id'
  , Name nvarchar(max) '$.Name'
  , Funding int '$.Funding'
  )

  insert into Faculties (Name, Funding)
  select Name, Funding from @recordsTable
  where Id < 1;

  update Faculties
  set Name = case when r.Name is not null then r.Name else prev.Name end
  , Funding = case when r.Funding is not null then r.Funding else prev.Funding end
  --output -- важно, но не сейчас
  from Faculties prev
  join @recordsTable r on r.Id = prev.Id
  where r.Id > 0
end

select * from openjson('[{
  "Id": 1,
  "Funding": 100
},{
  "Id": 0,
  "Name": "Faculty2",
  "Funding": 10
}]') with (
  Id int '$.Id'
, Name nvarchar(max) '$.Name'
, Funding int '$.Funding'
)

exec CreateOrUpdateFaculties @records = '[{
  "Id": 1,
  "Name": "Faculty2",
  "Funding": 100
},{
  "Id": 0,
  "Name": "Faculty3",
  "Funding": 100600
}]'

select * from Faculties

create type MatchingRecordsShort as table ( -- укороченная версия
  RecordId int
)
go

create or alter procedure ReadFaculties
  @PageSize int
, @PageNumber int
, @FundingMoreThan int = NULL
as begin
  SET XACT_ABORT ON; -- чтоб падало где надо. В триггерах ON по умолчанию
  SET NOCOUNT ON; -- чтоб не ело лишний траффик

  DECLARE @Records MatchingRecordsShort;

  INSERT INTO @records (RecordId)
  SELECT record.ID
  FROM Faculties record
  WHERE 1=1 -- все фильтры должны быть необязательными
  AND (@FundingMoreThan IS NULL OR record.Funding > @FundingMoreThan)
  ORDER BY Id -- пропустили конфигурируемость сортировки
  OFFSET (@PageNumber - 1) * @PageSize
  ROWS FETCH NEXT @PageSize ROWS ONLY
  OPTION (RECOMPILE);
  
  SELECT prev.*
  FROM @records r
  JOIN Faculties prev ON prev.Id = r.RecordId
  ORDER BY r.RecordId; -- r.RowNumber; -- смотрите в полный пример
end

exec ReadFaculties @PageSize = 1, @PageNumber = 1, @FundingMoreThan = 100
go

create or alter function dbo.FindMax()
returns int
as begin
	return 0
end
go

select dbo.FindMax() as asdf

