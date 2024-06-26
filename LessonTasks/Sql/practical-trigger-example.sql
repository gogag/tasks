create table Something (
  Id int primary key identity
, SomeColumn int NOT NULL
, SomeColumn2 int NOT NULL
)
go

create TABLE Outbox (
  Id bigint PRIMARY KEY IDENTITY
, EventType varchar(500) NOT NULL
, Message nvarchar(max) NOT NULL
)
go

create or alter trigger Something_OnInsert on Something after insert
as
	insert into Outbox (EventType, Message)
	select 'SomethingInserted', (
		select i.*
		for json path, without_array_wrapper
	)
	from inserted i
	--cross apply (
	--	select i.*
	--	for json path
	--) serialized(JSON)
go

insert into Something
values (1, 2)

select * from Outbox