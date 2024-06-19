CREATE TYPE MatchingRecords AS TABLE (
  RecordId bigint PRIMARY KEY
, TotalRecords bigint NOT NULL
, RowNumber bigint NOT NULL
);
GO -- 👆 этот табличный тип используется для разделения запросов на фильтрацию(поиск и/или "keyset-пагинацию" - WHERE), сортировку (упорядочивание - ORDER BY) и "offset-пагинацию" (то еще зло OFFSET) от запросов на подгрузку данных

CREATE OR ALTER PROCEDURE SelectFromProcedures
  @Records MatchingRecords readonly -- требуется при передаче параметров табличных типов
, @SelectingProcedures nvarchar(max)
AS BEGIN
  SET XACT_ABORT ON; -- чтоб падало где надо. В триггерах ON по умолчанию
  SET NOCOUNT ON; -- чтоб не ело лишний траффик

  DECLARE @ProceduresCount int = (SELECT COUNT(*) FROM OPENJSON(@SelectingProcedures))
  DECLARE @ResultSetIndex int = 0;
  WHILE @ResultSetIndex < @ProceduresCount BEGIN
    DECLARE @Name varchar(128) = JSON_VALUE(@SelectingProcedures, CONCAT('strict $[', @ResultSetIndex, '].Name'));
    DECLARE @Options nvarchar(max) = JSON_QUERY(@SelectingProcedures, CONCAT('$lax [', @ResultSetIndex, '].Options')); -- lax можно не указывать, т.к. по умолчанию, т.е. не обязательно значение

    IF @Options IS NULL BEGIN
      EXEC @Name @Records = @Records; 
    END ELSE BEGIN
      EXEC @Name @Records = @Records, @Options = @Options;
    END
    
    SET @ResultSetIndex += 1;
  END
END
GO -- 👆 эта вспомагательная хранимка избавляет каждую читающую сущности хранимку от однотипной возни с вызовом подгружающих данные хранимок

CREATE TABLE GroupAnalytics (
  GroupId int PRIMARY KEY FOREIGN KEY REFERENCES Groups(Id) ON DELETE CASCADE
, PublishedHomeworksCount int NULL -- синхронизируется в результате процессе работы с домашними заданиями, в рамках другого агрегата. Механизмы обеспечения конечной согласованности для такой синхронизации сейчас не рассматриваем
)
GO -- 👆 расширим группы отдельной таблицей с регулярно обновляемой аналитикой, результаты которой нас не интересуют в большинстве транзакций над группами

CREATE TABLE Groups_TeachersCount (
  GroupId int PRIMARY KEY FOREIGN KEY REFERENCES Groups(Id) ON DELETE CASCADE
, TeachersCount int NOT NULL -- обновляем из триггера без опаски, т.к. таблицы находятся в пределах агрегата и GroupSubjectSchedule всегда модифицируется из транзакций над Groups
)
GO -- создаем в виде таблицы, т.к. одной материализованной вьюхой оптимизировать не позволяет SQL Server

CREATE VIEW dbo.Groups_TeachersCount_Name WITH SCHEMABINDING -- для материализации вьюх обязательно явное использование имени схемы
AS
  SELECT gtc.TeachersCount, g.Name, gtc.GroupId -- GroupId обязателен для оптимизации, т.к. собирается в результат
  FROM dbo.Groups g
  JOIN dbo.Groups_TeachersCount gtc ON gtc.GroupId = g.Id
GO -- непосредственно вьюха, объединяющая обращение к нескольким таблицам

CREATE UNIQUE CLUSTERED INDEX CIX_Groups_TeachersCount_Name
ON dbo.Groups_TeachersCount_Name(TeachersCount DESC, Name ASC);
GO -- оптимизируем фильтрации и сортировки с использованием TeachersCount и Name

CREATE OR ALTER TRIGGER Groups_OnInsert ON Groups AFTER INSERT
AS
  INSERT INTO Groups_TeachersCount (GroupId, TeachersCount)
  SELECT inserted.Id, 0
  FROM inserted;
GO -- количество известно всегда, когда существует запись группы

INSERT INTO Groups_TeachersCount (GroupId, TeachersCount)
SELECT g.Id, (SELECT COUNT(DISTINCT TeacherId) FROM GroupSubjectSchedule gss WHERE gss.GroupId = g.Id)
FROM Groups g;
GO -- опять-таки слегка проще, чем на самом деле, т.к. в триггере уже могут появляться записи и "забыли" заблокировать Groups

--delete from Groups where Id > 3
DECLARE @groupsToGenereate int = 100;
WHILE @groupsToGenereate > 0 BEGIN
  INSERT INTO Groups (Name) VALUES (CONCAT('gen-', @groupsToGenereate));
  SET @groupsToGenereate -= 1;

  --UPDATE Groups_TeachersCount
  --SET TeachersCount = RAND() * 100 + 1
  --WHERE GroupId = @@identity
END;
GO -- добавим групп, чтобы проверить оптимизацию на плане

CREATE OR ALTER TRIGGER GroupSubjectSchedule_OnTeacherChange ON GroupSubjectSchedule AFTER INSERT, UPDATE, DELETE
AS
  WITH affected(GroupId) AS (
    SELECT DISTINCT ISNULL(inserted.GroupId, deleted.GroupId)
    FROM inserted
    FULL JOIN deleted ON deleted.GroupId = inserted.GroupId AND deleted.SubjectId = inserted.SubjectId
    WHERE ISNULL(inserted.TeacherId, -1) != ISNULL(deleted.TeacherId, -1) -- вот именно тут можно словить прикол с NULL, если пытаться предикатами отделаться вместо ISNULL с -1
  )
  UPDATE Groups_TeachersCount
  SET TeachersCount = (SELECT COUNT(DISTINCT TeacherId) FROM GroupSubjectSchedule gss WHERE gss.GroupId = affected.GroupId)
  FROM affected
  JOIN Groups_TeachersCount gtc ON gtc.GroupId = affected.GroupId;
GO -- обновляем количество 

CREATE OR ALTER PROCEDURE ReadGroups
  @PageSize int
, @PageNumber int
, @OrderBy nvarchar(max) = NULL OUTPUT
, @ContinueAfter nvarchar(max) = NULL -- OUTPUT -- TODO надо попробовать
, @ForUpdate bit = 0
, @CountTotalRecords bit = 0
, @TotalRecords int = 0 OUTPUT
, @RecordKeys nvarchar(max) = NULL
, @RecordKeyType varchar(10) = NULL
, @SelectingProcedures nvarchar(max) = NULL
-- filters
, @TeachersCountLessThan int = null
-- GroupAnalytics
, @PublishedHomeworksCountMoreThanOrEqual int = null
AS
BEGIN
  SET XACT_ABORT, NOCOUNT ON;

  IF @OrderBy IS NULL AND @ContinueAfter IS NOT NULL THROW 60000, '@OrderBy IS NULL AND @ContinueAfter IS NOT NULL', 1;

  IF @OrderBy = '[]' BEGIN
    IF @TeachersCountLessThan IS NOT NULL BEGIN
      SET @OrderBy = CONCAT(N'['
      , JSON_MODIFY(JSON_MODIFY('{}'
      , '$.ExpName', 'GroupSubjectSchedule.TeachersCount')
      , '$.Direction', 'D')
      , N','
      , JSON_MODIFY(JSON_MODIFY('{}'
      , '$.ExpName', 'Name')
      , '$.Direction', 'A')
      , N']'); -- TODO автовыбор сортировки исходя из используемых фильтров
      SELECT @OrderBy
    --END ELSE IF BEGIN ...
    --  SET @OrderBy = '[...]';
    END ELSE
      THROW 60000, 'Unable to determine optimal order', 1;
  END
  
  DECLARE @TotalOrderExpressions tinyint = 0;
  DECLARE @DistinctOrderExpressions tinyint = 0;

    -- находим порядковые номера выражений сортировки начиная с 1
  DECLARE @IdOrderExpNum tinyint = 0; -- помогаем себе, облегчая копи-пасту
  DECLARE @NameOrderExpNum tinyint = 0;
  DECLARE @TeachersCountOrderExpNum tinyint = 0; -- в том числе и помогаем SQL Server-у, т.к. OPTION(RECOMPILE) не может отсекать части выражения по предикатам со вложенными запросами

  -- находим направление сортировки для выражений
  DECLARE @IdOrderExpDirection varchar(1) = null;
  DECLARE @NameOrderExpDirection varchar(1) = null;
  DECLARE @TeachersCountOrderExpDirection varchar(1) = null;

  SELECT @TotalOrderExpressions = COUNT(*)
  , @DistinctOrderExpressions = COUNT(DISTINCT e.Name)
  , @IdOrderExpNum = ISNULL(CAST(STRING_AGG(CASE e.Name WHEN 'Id' THEN e.Num END, '') as int), 0)
  , @IdOrderExpDirection = STRING_AGG(CASE e.Name WHEN 'Id' THEN e.Direction END, '')
  , @NameOrderExpNum = ISNULL(CAST(STRING_AGG(CASE e.Name WHEN 'Name' THEN e.Num END, '') as int), 0)
  , @NameOrderExpDirection = STRING_AGG(CASE e.Name WHEN 'Name' THEN e.Direction END, '')
  , @TeachersCountOrderExpNum = ISNULL(CAST(STRING_AGG(CASE e.Name WHEN 'GroupSubjectSchedule.TeachersCount' THEN e.Num END, '') as int), 0)
  , @TeachersCountOrderExpDirection = STRING_AGG(CASE e.Name WHEN 'GroupSubjectSchedule.TeachersCount' THEN e.Direction END, '')
  FROM OPENJSON(@OrderBy)
  CROSS APPLY (
    SELECT [key] + 1
    , JSON_VALUE(value, 'strict $.ExpName')
    , JSON_VALUE(value, 'strict $.Direction')
  ) e(Num, Name, Direction);

  SELECT @TotalOrderExpressions
  , @DistinctOrderExpressions
  , @IdOrderExpNum
  , @IdOrderExpDirection
  , @NameOrderExpNum
  , @NameOrderExpDirection
  , @TeachersCountOrderExpNum
  , @TeachersCountOrderExpDirection

  IF @TotalOrderExpressions > 2 THROW 60000, '@TotalOrderExpressions > 3', 1;
  IF @TotalOrderExpressions != @DistinctOrderExpressions THROW 60000, '@TotalOrderExpressions != @DistinctOrderExpressions', 1;

  DECLARE @ContinueAfterId int = JSON_VALUE(@ContinueAfter, '$.Id');
  DECLARE @ContinueAfterName varchar(max) = JSON_VALUE(@ContinueAfter, '$.Name');
  DECLARE @ContinueAfterTeachersCount int = JSON_VALUE(@ContinueAfter, '$."GroupSubjectSchedule.TeachersCount"');

  DECLARE @Records MatchingRecords;

  INSERT INTO @records (RecordId, TotalRecords, RowNumber)
  SELECT record.ID, CASE WHEN @CountTotalRecords = 1 THEN COUNT(*) OVER() ELSE 0 END, ROW_NUMBER() OVER (ORDER BY
  -- первое выражение
    CASE WHEN 1 = @IdOrderExpNum AND @IdOrderExpDirection = 'A' THEN record.Id END ASC
  , CASE WHEN 1 = @IdOrderExpNum AND @IdOrderExpDirection = 'D' THEN record.Id END DESC
  , CASE WHEN 1 = @NameOrderExpNum AND @NameOrderExpDirection = 'A' THEN record.Name END ASC
  , CASE WHEN 1 = @NameOrderExpNum AND @NameOrderExpDirection = 'D' THEN record.Name END DESC
  , CASE WHEN 1 = @TeachersCountOrderExpNum AND @TeachersCountOrderExpDirection = 'A' THEN gtc.TeachersCount END ASC
  , CASE WHEN 1 = @TeachersCountOrderExpNum AND @TeachersCountOrderExpDirection = 'D' THEN gtc.TeachersCount END DESC
  -- второе выражение
  , CASE WHEN 2 = @IdOrderExpNum AND @IdOrderExpDirection = 'A' THEN record.Id END ASC
  , CASE WHEN 2 = @IdOrderExpNum AND @IdOrderExpDirection = 'D' THEN record.Id END DESC
  , CASE WHEN 2 = @NameOrderExpNum AND @NameOrderExpDirection = 'A' THEN record.Name END ASC
  , CASE WHEN 2 = @NameOrderExpNum AND @NameOrderExpDirection = 'D' THEN record.Name END DESC
  , CASE WHEN 2 = @TeachersCountOrderExpNum AND @TeachersCountOrderExpDirection = 'A' THEN gtc.TeachersCount END ASC
  , CASE WHEN 2 = @TeachersCountOrderExpNum AND @TeachersCountOrderExpDirection = 'D' THEN gtc.TeachersCount END DESC
  -- третье выражение
  , CASE WHEN 3 = @IdOrderExpNum AND @IdOrderExpDirection = 'A' THEN record.Id END ASC
  , CASE WHEN 3 = @IdOrderExpNum AND @IdOrderExpDirection = 'D' THEN record.Id END DESC
  , CASE WHEN 3 = @NameOrderExpNum AND @NameOrderExpDirection = 'A' THEN record.Name END ASC
  , CASE WHEN 3 = @NameOrderExpNum AND @NameOrderExpDirection = 'D' THEN record.Name END DESC
  , CASE WHEN 3 = @TeachersCountOrderExpNum AND @TeachersCountOrderExpDirection = 'A' THEN gtc.TeachersCount END ASC
  , CASE WHEN 3 = @TeachersCountOrderExpNum AND @TeachersCountOrderExpDirection = 'D' THEN gtc.TeachersCount END DESC
  ) as RowNumber
  -- ... а дальше столько копи-пасты, сколько будем позволять комбинаций. Это к сожалению предел возможностей SQL Server без сторонней помощи (динамической генерации скриптов)
  FROM Groups record
  LEFT JOIN GroupAnalytics ga ON ga.GroupId = record.Id
  LEFT JOIN Groups_TeachersCount gtc ON gtc.GroupId = record.Id
  WHERE 1=1 -- все фильтры должны быть необязательными
  AND (@RecordKeys IS NULL OR ISNULL(@RecordKeyType, 'Id') != 'Id' OR record.Id IN (SELECT CAST([key] as int) FROM OPENJSON(@RecordKeys)))
  AND (@ContinueAfter IS NULL -- первая страница в текущей сортировке
    OR @IdOrderExpNum = 0 -- если в сортировке не участвует
    OR @IdOrderExpNum >= 2 AND NOT (1 = @IdOrderExpNum AND record.Id = @ContinueAfterId OR 1 = @NameOrderExpNum AND record.Name = @ContinueAfterName OR 1 = @TeachersCountOrderExpNum AND gtc.TeachersCount = @ContinueAfterTeachersCount)
    OR @IdOrderExpNum >= 3 AND NOT (2 = @IdOrderExpNum AND record.Id = @ContinueAfterId OR 2 = @NameOrderExpNum AND record.Name = @ContinueAfterName OR 2 = @TeachersCountOrderExpNum AND gtc.TeachersCount = @ContinueAfterTeachersCount)
    OR (
      @IdOrderExpDirection = 'A' AND (record.Id > @ContinueAfterId OR @IdOrderExpNum != @TotalOrderExpressions AND record.Id = @ContinueAfterId)
      OR @IdOrderExpDirection = 'D' AND (record.Id < @ContinueAfterId OR @IdOrderExpNum != @TotalOrderExpressions AND record.Id = @ContinueAfterId)
    )
  )
  AND (@RecordKeys IS NULL OR ISNULL(@RecordKeyType, 'Name') != 'Name' OR record.Name IN (SELECT [key] COLLATE SQL_Latin1_General_CP1_CI_AS FROM OPENJSON(@RecordKeys)))
  AND (@ContinueAfter IS NULL -- первая страница в текущей сортировке
    OR @NameOrderExpNum = 0 -- если в сортировке не участвует
    OR @NameOrderExpNum >= 2 AND NOT (1 = @IdOrderExpNum AND record.Id = @ContinueAfterId OR 1 = @NameOrderExpNum AND record.Name = @ContinueAfterName OR 1 = @TeachersCountOrderExpNum AND gtc.TeachersCount = @ContinueAfterTeachersCount)
    OR @NameOrderExpNum >= 3 AND NOT (2 = @IdOrderExpNum AND record.Id = @ContinueAfterId OR 2 = @NameOrderExpNum AND record.Name = @ContinueAfterName OR 2 = @TeachersCountOrderExpNum AND gtc.TeachersCount = @ContinueAfterTeachersCount)
    OR (
      @NameOrderExpDirection = 'A' AND (record.Name > @ContinueAfterName OR @NameOrderExpNum != @TotalOrderExpressions AND record.Name = @ContinueAfterName)
      OR @NameOrderExpDirection = 'D' AND (record.Name < @ContinueAfterName OR @NameOrderExpNum != @TotalOrderExpressions AND record.Name = @ContinueAfterName)
    )
  )
  -- GroupAnalytics
  AND (@PublishedHomeworksCountMoreThanOrEqual IS NULL OR ga.PublishedHomeworksCount >= @PublishedHomeworksCountMoreThanOrEqual)
  -- Groups_TeachersCount
  AND (@TeachersCountOrderExpNum = 0 OR gtc.GroupId IS NOT NULL) -- полдня вспоминал.. !Обязательно! для оптимизации на вьюхе, чтобы внести для SQL Server понимание, что LEFT JOIN на самом деле INNER JOIN
  AND (@TeachersCountLessThan IS NULL OR gtc.GroupId IS NOT NULL AND gtc.TeachersCount < @TeachersCountLessThan)
  AND (@ContinueAfter IS NULL -- первая страница в текущей сортировке
    OR @TeachersCountOrderExpNum = 0 -- если в сортировке не участвует
    OR @TeachersCountOrderExpNum >= 2 AND NOT (1 = @IdOrderExpNum AND record.Id = @ContinueAfterId OR 1 = @NameOrderExpNum AND record.Name = @ContinueAfterName OR 1 = @TeachersCountOrderExpNum AND gtc.TeachersCount = @ContinueAfterTeachersCount)
    OR @TeachersCountOrderExpNum >= 3 AND NOT (2 = @IdOrderExpNum AND record.Id = @ContinueAfterId OR 2 = @NameOrderExpNum AND record.Name = @ContinueAfterName OR 2 = @TeachersCountOrderExpNum AND gtc.TeachersCount = @ContinueAfterTeachersCount)
    OR (
      @TeachersCountOrderExpDirection = 'A' AND (gtc.TeachersCount > @ContinueAfterTeachersCount OR @TeachersCountOrderExpNum != @TotalOrderExpressions AND gtc.TeachersCount = @ContinueAfterTeachersCount)
      OR @TeachersCountOrderExpDirection = 'D' AND (gtc.TeachersCount < @ContinueAfterTeachersCount OR @TeachersCountOrderExpNum != @TotalOrderExpressions AND gtc.TeachersCount = @ContinueAfterTeachersCount)
    )
  ) 
  ORDER BY RowNumber OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY
  OPTION (RECOMPILE);
  
  IF @ForUpdate = 1 AND (SELECT COUNT(*) FROM @records) > 0 BEGIN
    DECLARE @dev0 int;
    SELECT @dev0 = COUNT(*)
    FROM @records r
    JOIN Groups g WITH (UPDLOCK) ON g.Id = r.RecordId;
  END

  SELECT g.*
  , ISNULL(schedule.Records, '[]') as Schedule
  FROM @records r
  JOIN Groups g WITH (FORCESEEK) ON g.Id = r.RecordId -- очень аккуратно, строго поиском по индексу
  CROSS APPLY (
    SELECT gss.SubjectId
    , gss.TeacherId
    FROM GroupSubjectSchedule gss WITH (FORCESEEK)
    WHERE gss.GroupId = g.Id
    FOR JSON PATH
  ) schedule(Records)
  ORDER BY r.RowNumber;

  -- TODO можно ли аггрегациями на основном поисковом запросе замкнуть последние значения, чтобы обеспечить перебор страниц целиком хранимкой
  --IF @OrderBy IS NOT NULL BEGIN
  --  SET @ContinueAfter = (
  --    SELECT *
  --  );
  --END

  SET @TotalRecords = (SELECT TOP 1 TotalRecords FROM @records);
  IF @TotalRecords IS NULL BEGIN
    SET @TotalRecords = 0;
    RETURN;
  END

  EXEC SelectFromProcedures @records, @SelectingProcedures;
END
GO

CREATE OR ALTER PROCEDURE SelectGroupAnalytics @Records MatchingRecords readonly
AS
BEGIN
  SET XACT_ABORT, NOCOUNT ON;

  SELECT prev.*
  FROM GroupAnalytics prev
  JOIN @Records r on r.RecordId = prev.GroupId
  ORDER BY r.RecordId;
END
GO