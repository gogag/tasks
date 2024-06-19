DECLARE @OrderBy nvarchar(max) = '[]';
DECLARE @ContinueAfter nvarchar(max);
DECLARE @TotalRecords int = 1;
DECLARE @TeachersCountLessThan int = 10;

EXEC ReadGroups
  @PageSize = 50
, @PageNumber = 1
, @OrderBy = @OrderBy OUTPUT
, @ContinueAfter = @ContinueAfter -- TODO переложить на хранимку
, @TeachersCountLessThan = 10
, @CountTotalRecords = 1
, @TotalRecords = @TotalRecords OUTPUT

SELECT @OrderBy, @ContinueAfter, @TotalRecords

EXEC ReadGroups
  @PageSize = 50
, @PageNumber = 1
, @OrderBy = @OrderBy
--, @OrderBy = '[{
--    "Direction": "D",
--    "ExpName": "GroupSubjectSchedule.TeachersCount"
--  }, {
--    "Direction": "A",
--    "ExpName": "Name"
--  }]'
, @ContinueAfter = @ContinueAfter
--, @ContinueAfter = '{
--    "Name": "C#",
--    "GroupSubjectSchedule.TeachersCount": 2
--  }'
--, @ContinueAfter nvarchar(max) = NULL
--, @SelectingProcedures = '[{ "Name": "SelectGroupAnalytics" }]'
-- filters
, @TeachersCountLessThan = @TeachersCountLessThan
-- GroupAnalytics
--, @PublishedHomeworksCountMoreThanOrEqual int = 0


