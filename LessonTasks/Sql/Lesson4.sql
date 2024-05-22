select case
	when 2 > null then 1
	else 0
end as Col1
, null + 1 as Col2

create table TableWithNull (
	Col int null
)

select * from TableWithNull where Col IS NOT NULL AND Col > 0
insert into TableWithNull values (null)

select * from TableWithNull where Col = null -- не работает. результат - null
select * from TableWithNull where Col IS NULL -- только так

--bool ? trueVal : falseValue
--reference ?? throw new Exc

select 
go -- 1

use 
go -- 2

select 'blah not blah' as Col1
into #temp

insert into #temp (Col1)
output inserted.Id
values ('not he not t')
insert into #temp values ('notnot')
alter table #temp add Id int IDENTITY PRIMARY KEY
alter table #temp add Col2 int NULL
alter table #temp add Col3 int NULL

--select top 2 *
select top 2 id
from #temp t with (forceseek)
--from #temp t
--where Col1 like '%not%not%'
where Col1 like 'not%'
--where Col1 like 'not' -- Col1 = 'not'
--where Col1 like '%wont%find%'

create index IX_Naive on #temp (Col1)

create table DependencyTable (
  Id int identity primary key
)

create table DependingTable (
  Id int identity primary key
, DependencyTableId int foreign key references DependencyTable(Id) on delete cascade NOT NULL
)

create table ExtendingTable (
  DependencyTableId int primary key foreign key references DependencyTable(Id) on delete cascade NOT NULL
, Col4 int
)

select *
from DependencyTable dt
, ExtendingTable et
where dt.Id = et.DependencyTableId

select @@IDENTITY



alter table DependingTable add constraint FK_DependencyTable_ID foreign key (DependencyTableId) references DependencyTable(Id) on delete cascade

insert into DependencyTable default values

insert into DependingTable (DependencyTableId) values (1)
go 100

select * from DependingTable

delete from DependencyTable
where id = 1
