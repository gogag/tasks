select case
	when 2 > null then 1
	else 0
end as Col1
, null + 1 as Col2

create table TableWithNull (
	Col int null
,	Col2 int null
)

select * from TableWithNull where Col IS NOT NULL AND Col > 0
insert into TableWithNull values (null)
insert into TableWithNull values (100500)
insert into TableWithNull (Col2) values (22)

-- тут про особое отношение AND и OR к NULL операндам, что позволяет комбинировать IS NULL проверки с обычными предикатами над null:
-- https://learn.microsoft.com/ru-ru/sql/t-sql/language-elements/null-and-unknown-transact-sql?view=sql-server-ver16

select * from TableWithNull where Col = null -- не работает. результат - null
select * from TableWithNull where Col != null -- не работает. результат - null
select * from TableWithNull where Col != 2 -- упс, запись с null тоже не подтянется в результат, тогда как в шарпе null != 2 это истина
select * from TableWithNull where not (Col = 2) -- не поможет, запись с null все равно не подтянется в результат
select * from TableWithNull where Col not in (2) -- упс, запись с null тоже не подтянется в результат, тогда как в шарпе null != 2 это истина
select Col, count(*) from TableWithNull group by Col -- а вот значением группы null может оставаться
declare @someVar varchar;
select 'asdf' + @someVar + 'asdf' -- null. лечится CONCAT-ом
select concat('asdf', @someVar, 'asdf') -- asdfasdf
select count(*) from TableWithNull -- 2
select count(Col) from TableWithNull -- 1 -- потому что null значения исключаются из агреггирующей функции
select * from TableWithNull where Col IS NULL -- только так
select * from TableWithNull where Col IS NOT NULL -- только так
select * from TableWithNull where Col IS NULL OR Col != 2 -- OR решает проблему
select * from TableWithNull where Col = 22 OR Col2 = 22 -- и не только с Col IS NULL, но в таком варианте могут быть проблемы с оптимизацией запросов
select * from TableWithNull where ISNULL(Col, Col2) = 22 -- еще как альтернатива записи, но в оптимизации не поможет

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
