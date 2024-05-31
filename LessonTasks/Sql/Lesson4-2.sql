create table TableBad(
  Id int identity primary key
, Name nvarchar(max) CHECK(Name > '') NOT NULL
, Rating int NOT NULL
, GroupName varchar(10) NOT NULL
)
go

insert into TableBad (Name, Rating, GroupName)
values ('Марь Иванна', 10, 'C#') -- Id 1
, ('Марь Иванна', 10, 'SQL') -- Id 2

select * from TableBad

select top 0 Id, Name, Rating
into Teachers
from TableBad

insert into Teachers (Name, Rating)
select distinct Name, Rating
from TableBad

insert into Teachers
values ('Ирина Петровна', 5)

insert into Teachers (Name, Rating)
values ('Ирина Петровна Возвращается', 7)

alter table Teachers add constraint PK_Id primary key (Id)

select * from Teachers
select * from TableBad
select * from TableBad, Teachers
go -- на полпути

select tb.*
, t.Id as TeacherId
from TableBad tb
, Teachers t
where tb.Name = t.Name
and tb.Rating = t.Rating

alter table TableBad
add TeacherId int foreign key references Teachers(Id) NULL

update TableBad
set TeacherId = t.Id
from TableBad tb
, Teachers t
--join Teachers t on tb.Name = t.Name and tb.Rating = t.Rating -- так вообще-то принято делать
where tb.Name = t.Name
and tb.Rating = t.Rating

--update TableBad
--set TeacherId = null

alter table TableBad
drop constraint CK__TableBad__Name__24927208

alter table TableBad
drop column Name, Rating

--exec sp_rename  -- зло

insert into TableBad (GroupName)
values ('Lost group')

select *
from TableBad tb
full join Teachers t on tb.TeacherId = t.Id

