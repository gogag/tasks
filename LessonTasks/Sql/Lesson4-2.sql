create table TableBad(
  Id int identity primary key
, Name nvarchar(max) NOT NULL CONSTRAINT CK_TableBad_Name_NotEmpty CHECK(Name > '') -- определение nullability лучше ближе к типу, т.к. является частью определения непосредственно типа, не является объектом ограничения
, Rating int NOT NULL
, GroupName varchar(10) NOT NULL
)

insert into TableBad (Name, Rating, GroupName)
values (N'Марь Иванна', 10, 'C#') -- Id 1. Обратите внимание, исправил строковые константы на N'' - т.к. русские символы, работаем с UTF-16, тип nvarchar
, (N'Марь Иванна', 10, 'SQL') -- Id 2

select * from TableBad
go -- Создали денормализованную таблицу, содержащуюю две сущности: преподаватели и группы

select top 0 Id, Name, Rating
into Teachers -- сначала просто создали таблицу в "куче" для ускорения последующих вставок существующих сущностей
from TableBad

insert into Teachers (Name, Rating)
select distinct Name, Rating -- убираем дубликаты и создаем записи для существующих сущностей - учителей
from TableBad

insert into Teachers
values (N'Ирина Петровна', 5)

insert into Teachers (Name, Rating)
values (N'Зинаида Павловна', 7)

--alter table Teachers add constraint PK_Id primary key (Id) -- исправили, имена констрейнтов должны быть глобально уникальными в пределах БД, в том числе и среди имен объектов иных типов
alter table Teachers
add constraint PK_Teachers_Id primary key (Id) -- когда все добавили, одним махом перестраиваем новую таблицу в кластерный индекс (в сортированный вид)
, constraint CK_Teachers_Name_NotEmpty CHECK(Name > '') -- кстати обманул! ограничения не наследуются. А почему? Так решили разработчики, что объект Identity, особенный, копируется, а вот объекты ограничений и PK - ну их

select * from Teachers
select * from TableBad
select * from TableBad, Teachers -- 3 * 2 = 6 - всего комбинаций, результат декартого произведения (cortesian product). То же, что CROSS JOIN
go -- выделили нормализованную таблицу учителей

select tb.*
, t.Id as TeacherId
from TableBad tb
, Teachers t
where tb.Name = t.Name
and tb.Rating = t.Rating

alter table TableBad
add TeacherId int NULL foreign key references Teachers(Id) on delete cascade
go -- тут go обязателен, иначе следующий update в том же батче не сработает - не помню почему так

update TableBad
set TeacherId = t.Id
from TableBad tb
, Teachers t
--join Teachers t on tb.Name = t.Name and tb.Rating = t.Rating -- так вообще-то принято делать
where tb.Name = t.Name
and tb.Rating = t.Rating
go -- материализовали колонку TeacherId. Теперь можно и удалять старые колонки, т.к. уже связь не потеряем

alter table TableBad
drop constraint CK_TableBad_Name_NotEmpty

alter table TableBad
drop column Name, Rating

go -- прибили изначальные денормализованные колонки учителя

--exec sp_rename  -- зло

insert into TableBad (GroupName)
values ('Lost group') -- добавили чтоб продемонстрировать full [outer] join, когда у обеих сторон есть свои outer подмножества записей (не пересекающихся с другой таблицей)

select *
from TableBad tb
full join Teachers t on tb.TeacherId = t.Id -- тут мы разбирали разные варианты из "основных" жойнов - INNERT, OUTER-ы (множества записей двух таблиц в виде кругов частично пересекающихся друг с другом)
go -- смотрим на записи, где с одной стороны есть значение, а с другой все NULL (обращая внимание в первую очередь на сравниваемые колонки)

-- хотел оставить ссылку, но работало там неверно:
SELECT *
FROM (
  SELECT tb.Id, tb.GroupName, tb.TeacherId FROM TableBad tb
  UNION ALL
  SELECT Null, Null, null -- добавляем для подстановки, если нет групп. null-ов столько, сколько полей
) tb
CROSS JOIN ( -- то же, что запятая
  SELECT t.Id, t.Name, t.Rating FROM Teachers t
  UNION ALL
  SELECT Null, Null, null -- добавляем для подстановки, если нету учителя, null-ов столько, сколько полей
) t
WHERE (
  tb.TeacherId = t.Id -- это зона INNER
  OR ( -- это зона LEFT OUTER
	/* -- это невыполнимое условие, так как есть foreign key
    tb.TeacherId Is Not Null 
    AND t.Id IS NULL
    AND NOT EXISTS(SELECT 1 FROM Teachers t WHERE t.Id = tb.TeacherId)
    OR */
	tb.Id is not null
	AND tb.TeacherId is null AND t.Id IS NULL -- это проверка на комбинацию с добавленной записью с null-ами, при условии что слева запись есть (tb.Id is not null)
  )
  OR ( -- это зона RIGHT OUTER
    t.Id Is Not Null
    AND tb.Id IS NULL
    AND NOT EXISTS(SELECT 1 FROM TableBad tb WHERE tb.TeacherId = t.Id) -- это проверка выполняется, если для комбинации с null-ами, не существует других не-null комбинаций (из зоны INNER-ов)
  )
)
go -- тут мы видим во что "примерно" разворачивается сахар от FULL [OUTER] JOIN

--alter table TableBad drop column Subject
alter table TableBad
add Subject as case
	--when GroupName in ('Lost group') then null -- это же не предмет явно
	--else GroupName
	when GroupName in ('C#', 'SQL') then GroupName -- только предметы
	else null
end -- обратите внимание, что мы не пытаемся колонку материализовать, тем самым не приходится копировать тип и не нужно делать лишний update

select * from TableBad
go -- и тут мы поняли, что наши беды не кончились - у группы может быть несколько предметов (а до этого мы считали, что имя предмета = имя группы.. оказалось не так)

select top 0 Id, ISNULL(Subject, '') as Name -- ISNULL нужен, чтоб переделать тип под NOT NULL при наследовании из оригинальной колонки, которая NULL
into Subjects -- сначала просто создали таблицу в "куче" для ускорения последующих вставок существующих сущностей
from TableBad

insert into Subjects(Name)
select distinct Subject -- трюк тот же, только для предметов уже
from TableBad
where Subject is not null

alter table Subjects
add constraint PK_Subjects_Id primary key (Id) -- так же, в кластерный индекс
, check (Name > '') -- и так вот можно, на таблице
go -- мы все еще имеем связь 1-* между группой и учителем. Однако уже сейчас, наша плохая таблица обеспечивает связь *-* между учителем и предметом, хоть мы и упускаем некоторый контроль...

alter table Subjects
add constraint AK_Subjects_Name UNIQUE (Name)
go -- добавили какой-никакой недостающий контроль

alter table TableBad add SubjectId int NULL foreign key references Subjects(Id) on delete cascade
go -- идем известным путем - материализуем айдишники

update TableBad
set SubjectId = s.Id
from TableBad tb
join Subjects s on s.Name = tb.Subject

alter table TableBad drop column Subject
go -- есть, убрали предметы, но все таки с нашей TableBad что-то не то, она все еще помойка связей! Надо с этим разобраться

insert into TableBad (GroupName, TeacherId, SubjectId)
values ('C#', 2, 2) -- теперь мы можем добавить еще одну группу с тем же предметом

select * from Teachers
select * from TableBad
select * from Subjects
go -- обалдеть, одной из старых групп, которую мы все еще не переименовали, добавляют новый предмет - SQL, который будет теперь вести Ирина Петровна.. все еще могут быть аномалии - имя группы дублируется

select top 0 Id, GroupName as Name
into Groups
from TableBad

insert into Groups(Name)
select distinct GroupName
from TableBad

alter table Groups
add constraint PK_Groups_Id primary key (Id)
, constraint AK_Groups_Name unique (Name)
, check (Name > '') -- и так вот можно, на таблице

alter table TableBad add GroupId int NULL foreign key references Groups(Id) on delete cascade
go -- идем известным путем - материализуем айдишники

update TableBad
set GroupId = g.Id
from TableBad tb
join Groups g on g.Name = tb.GroupName

alter table TableBad alter column GroupId int NOT NULL
alter table TableBad drop column GroupName

select * from TableBad
select * from Groups
select * from Teachers
select * from Subjects
go -- ну в общем, проделали все то же для групп, только как видим, в TableBad все еще может быть бред:
   -- 1. Группа, оказалось, может быть с предметом, но без учителя, или наоборот.
   -- 2. Всего у всех много, непонятно как, а память в целом отъедает (каждый NULL кушает столько же, сколько сам int - и мы рады этому).
   -- 3. Плюс, можно сделать бессмысленную копию всей строки и получить те же связи объявленные дважды.. - опять будет нарушена 1НФ

select distinct ISNULL(TeacherId, -1) as TeacherId, ISNULL(SubjectId, -1) as SubjectId
into TeacherSubjects
from TableBad
where TeacherId IS NOT NULL
AND SubjectId IS NOT NULL

alter table TeacherSubjects
add constraint PK_TeacherSubjects primary key (TeacherId, SubjectId) -- и такой порядок не спроста, т.к. добавлять предметы будем в транзакциях над учителем, чтоб связанные записи хранились ближе в памяти
, foreign key (TeacherId) references Teachers(Id) on delete cascade
, foreign key (SubjectId) references Subjects(Id) on delete cascade
go -- так мы говорим, какие предметы может вести преподаватель

select top 0 GroupId, ISNULL(SubjectId, -1) as SubjectId, TeacherId -- конфигурация времени, состояние черновика, и прочее вытекающие из процессов редактирования расписания были опущены
into GroupSubjectSchedule
from TableBad

insert into GroupSubjectSchedule
select distinct GroupId, SubjectId, TeacherId
from TableBad
where SubjectId IS NOT NULL
go -- тут надо прервать, чтобы объявить функцию

---- Если ошибся в функции, использованной в Constraint-ах (будь то check или default)
---- то вот такая вот боль:
--alter table GroupSubjectSchedule
--drop constraint CK__GroupSubjectSche__59FA5E80

---- затем alter функции (а на реальной системе сначала создать вторую функций, второй check, а потом только удалять)

--alter table GroupSubjectSchedule
--add constraint CK_GroupSubjectSchedule_Teaches
--check (dbo.FN_CheckTeaches(TeacherId, SubjectId) = 'true')
--go -- это упрощенный пример боли от sql server . . .

create function FN_CheckTeaches (@TeacherId int, @SubjectId int)
--alter function FN_CheckTeaches (@TeacherId int, @SubjectId int) -- . . . так просто не прокатитывает
returns bit as
begin
  return case when EXISTS(
    select top 1 1
    from TeacherSubjects
    where @TeacherId is null -- чтобы не мешать снимать преподавателя с расписания
	OR (
		TeacherId = @TeacherId
		AND SubjectId = @SubjectId
	)
) then 1 else 0 end;
end;
go

alter table GroupSubjectSchedule
add constraint PK_GroupSubjectSchedule primary key (GroupId, SubjectId) -- будь у нас конфигурация времени - номер временного окна был бы частью этого ключа, плюс возможно с отфильтровкой утративших силу записей
, foreign key (GroupId) references Groups(Id) on delete cascade
, foreign key (SubjectId) references Subjects(Id) on delete cascade
, check (dbo.FN_CheckTeaches(TeacherId, SubjectId) = 'true') -- эта дичь очень далеко от правды.. Сейчас - просто ради демонстрации укуренных возможностей SQL Server прошлых лет. Во первых мы бы проверяли это в шарпе, во вторых реальная реализация может быть гораздо сложнее, смотря как редактируются преподаваемые предметы
go -- все, разрулили. Теперь если в добавленное расписание группы пытаются поставить преподавателя - ограничение будет проверять, преподает ли он данный предмет

select * from Groups
select * from GroupSubjectSchedule
select * from Subjects
select * from TeacherSubjects
select * from Teachers

select * from TableBad

--delete from TeacherSubjects -- наш волшебный check не способен это предотвратить
--where TeacherId = 1 and SubjectId = 1

--insert into TeacherSubjects
--values (1, 1)

drop table TableBad
go -- грохнули наш изначальный мусор, ужасаемся количеству мучений, необходимых чтобы исправить подобного рода ошибку - и никогда ее не допускаем, нормализуем в уме, затем записываем в БД (стараемся и не перебарщиваем с атомарностью, когда это не требуется)

select *
from Subjects

select t.*
, CONCAT('[', subject.Ids, ']') as Teaches -- человек физически не может вести бесконечное число дисциплин. В агрегате мы бы проверяли, что максимальное число выбранных предметов не было превышено
from Teachers t
cross apply ( -- очень аккуратно, строго поиском по индексу
	select STRING_AGG(ts.SubjectId, ',') -- пример аггрегирующей функции
	from TeacherSubjects ts with (FORCESEEK)
	where ts.TeacherId = t.Id
) subject(Ids)

select g.*
, ISNULL(schedule.Records, '[]') as Schedule
from Groups g
cross apply ( -- очень аккуратно, строго поиском по индексу
	select gss.SubjectId
	, gss.TeacherId
	from GroupSubjectSchedule gss with (FORCESEEK)
	where gss.GroupId = g.Id
	for json path
) schedule(Records)
go -- это бонус: как бы выглядило состояние со стороны агрегатов доменной модели (терминология из DDD, если грубо - в классах шарпа). Сервер оперировал бы именно этими тремя сущностями, не смешивая их в единые транзакции и не ковыряясь в деталях, как вложенные объекты хранятся физически в БД

insert into GroupSubjectSchedule (GroupId, SubjectId, TeacherId)
select Id
, (select s.Id from Subjects s where s.Name = 'C#')
, (select t.Id from Teachers t where t.Name = N'Ирина Петровна') from Groups
where Name = 'Lost group'
go -- этот батч падает ожидаемо, потому что срабатывает ограничение: Ирина Петровна не ведет C#

update GroupSubjectSchedule
set TeacherId = (select t.Id from Teachers t where t.Name = N'Ирина Петровна')
from GroupSubjectSchedule gss
join Subjects s on s.Id = gss.SubjectId
where s.Name = 'C#'
go -- этот батч падает ожидаемо, потому что срабатывает ограничение: Ирина Петровна не ведет C#

delete from TeacherSubjects
where TeacherId = (select t.Id from Teachers t where t.Name = N'Марь Иванна')
and SubjectId = (select s.Id from Subjects s where s.Name = 'C#')
go -- а вот этот не реагирует - ограчению откуда знать, что там в функции написано и на что реагировать. Это уже триггеры бы пригодились, но мы то знаем, что так удалять точно не будем, верно?

select * from Groups
select * from GroupSubjectSchedule
select * from Subjects
select * from TeacherSubjects
select * from Teachers
go -- что у нас есть в "сыром виде", оперативные таблицы (OLTP). Агрегации и групировки - все OLAP

select distinct GroupId
from GroupSubjectSchedule

select GroupId
from GroupSubjectSchedule
group by GroupId
go -- distinct эквивалентен такой группировке

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
, ROW_NUMBER() over (partition by GroupId order by GroupId) as NumberWithinGroup -- бонус, будет выводить порядковый номер предмета в группе, если бы мы группировали по GroupId
from GroupSubjectSchedule

update GroupSubjectSchedule
set TeacherId = 1 -- count(TeacherId) покажет столько же сколько count(*)
--set TeacherId = null -- count(TeacherId) покажет только с заданным TeacherId
where GroupId = 1 and SubjectId = 1
