
select GroupId
, count(*) as Count
, count(TeacherId) as AssignedCount
, count(case when TeacherId is null then 1 end) as NotAssignedCount
, count(*) over () as TotalCount -- бонус, общее число записей в таблице, будет у всех записей в результате одинковым
, ROW_NUMBER() over (order by GroupId) as GroupNumber -- бонус порядковый номер группы
--, ROW_NUMBER() over (partition by GroupId order by GroupId) as NumberWithinGroup -- бонус, будет выводить порядковый номер предмета в группе
from GroupSubjectSchedule
group by GroupId
having count(*) > 1

select *
, each.NumberWithinGroup
from GroupSubjectSchedule s
--having ROW_NUMBER() over (partition by GroupId order by GroupId) -- а так не могет
cross apply ( -- логически соответсвует inner join-у
	select ROW_NUMBER() over (partition by GroupId order by GroupId)
	, 1
) as each(NumberWithinGroup, SomethingCol) -- бонус, будет выводить порядковый номер предмета в группе, если бы мы группировали по GroupId
where each.NumberWithinGroup = 1

select GroupId, COUNT(*)
from GroupSubjectSchedule s
group by GroupId
having

--outer apply -- left join-у соответсвенно

update GroupSubjectSchedule
set TeacherId = 1 -- count(TeacherId) покажет столько же сколько count(*)
--set TeacherId = null -- count(TeacherId) покажет только с заданным TeacherId
where GroupId = 1 and SubjectId = 1

select * from GroupSubjectSchedule

select COUNT(*)
, COUNT(case when SubjectId = 2 then 1 end)
from GroupSubjectSchedule

create table Wards (
  Id int primary key identity
, Name nvarchar(20) NOT NULL
, Places int NOT NULL check (Places > 1)
)

insert into Wards
values ('Ward 1', 5)
, ('Ward 2', 30)
, ('Ward 3', 60)

select COUNT(*) as TotalBigWards
, COUNT(case when Places = 30 then 1 end) as TotalWardsWith30Places
--select *
--, case when Places = 30 then 1 end
from Wards
where Places > 10

create table Departments (
  Id int primary key identity
, Building int not null check (Building between 1 and 5)
, Name nvarchar(100) not null
)

alter table Wards add DepartmentId int foreign key references Departments(Id)

insert into Departments
--values (1, 'Department 1')
values (1, 'Department 2')
go 100

update Wards
set DepartmentId = 1

alter table Wards alter column DepartmentId int not null

insert into Wards (Name, Places, DepartmentId)
values ('Ward 4', 5, 2)
, ('Ward 5', 30, 2)

select * from Wards

select d.Name
, count(*)
from Departments d
join Wards w on w.DepartmentId = d.Id
group by d.Name

select (select d.Name from Departments d where d.Id = w.DepartmentId)
, count(*)
from Wards w
group by w.DepartmentId

select d.Name
, count(*)
from Departments d
join Wards w on w.DepartmentId = d.Id
group by d.Name
having count(*) > (select count(*) from Departments)

delete from Departments
where id > 2

select *
, count(*) over (partition by GroupId order by GroupId)
from GroupSubjectSchedule s

select *
, (select count(*) from GroupSubjectSchedule s1 where s1.GroupId = s.GroupId)
from GroupSubjectSchedule s

go -- теперь пойдем в EXISTS

create table Doctors (
  Id int primary key identity
, Name nvarchar(max)
)

insert into Doctors 
values ('Joshua Bell')

create table DoctorsExaminations (
  Id int primary key identity
, DoctorId int not null foreign key references Doctors(Id)
, WardId int not null foreign key references Wards(Id)
)

insert into DoctorsExaminations
values (1, 1)

insert into DoctorsExaminations
values (1, 2)
go 30


--select top 0 *
--from Wards w with(nolock) -- обезопасьте себя на боевых таблицах

select *
from Wards w with(forceseek)
--where exists (select 1) -- true
--where 1 = 1 -- тоже true. с хранимками коснемся
--where not (1 != 1) -- тоже true. чисто бред для демонстрации
where exists ( -- да мы всегда делали так
	select top 1 1
	from DoctorsExaminations de
	join Doctors d on d.Id = de.DoctorId
	where de.WardId = w.Id
	and d.Name = 'Joshua Bell'
)

select *
from Wards w
where id in ( -- тоже использовали, но реже
	select top 1 de.WardId
	from DoctorsExaminations de
	join Doctors d on d.Id = de.DoctorId
	where d.Name = 'Joshua Bell'
)

select *
from Wards w
where id = any ( -- тоже использовали, но реже
	select top 1 de.WardId
	from DoctorsExaminations de
	join Doctors d on d.Id = de.DoctorId
	where d.Name = 'Joshua Bell'
)

;with DrJoshuaExaminationWards as (
	select top 1 de.WardId
	from DoctorsExaminations de
	join Doctors d on d.Id = de.DoctorId
	where d.Name = 'Joshua Bell'
)
select *
from Wards w
where id in (select * from DrJoshuaExaminationWards)

