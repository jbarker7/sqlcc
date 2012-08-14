using System;
using System.Collections.Generic;
using SQLCC.Core;
using SQLCC.Core.Objects;

namespace SQLCC.Impl.MsSqlProvider
{
   public class MsSqlProvider : DbProvider
   {
      private readonly PetaPoco.Database _db;

      public MsSqlProvider(string connString, string traceDir, string traceFileName, string applicationName)
      {
         _db = new PetaPoco.Database(connString, "System.Data.SqlClient");
      }

      public override int StartTrace(string tracePath, object filterData)
      {
         if (!(filterData is string || filterData is int))
            throw new ArgumentException("Trace must be either of type integer (PID) or string (Application Name) for MSSQL provider.");

         var sqlString = 
               @"declare @@rc int
declare @@TraceID int
declare @@maxfilesize bigint
declare @@DateTime datetime

set @@DateTime = DateAdd(hour, 12, GetDate()) -- trace stop
set @@maxfilesize = 1000

-- Please replace the text InsertFileNameHere, with an appropriate
-- filename prefixed by a path, e.g., c:\MyFolder\MyTrace. The .trc extension
-- will be appended to the filename automatically. If you are writing from
-- remote server to local drive, please use UNC path and make sure server has
-- write access to your network share

exec @@rc = sp_trace_create @@TraceID output, 0, N'" +
               tracePath +
               @"', @@maxfilesize, @@Datetime
if (@@rc != 0) goto error

-- Client side File and Table cannot be scripted

-- Set the events
declare @@on bit
set @@on = 1

DECLARE @@TraceInfo TABLE(RowID INT identity(1,1) primary key, TraceColumn INT, TraceEvent INT);

INSERT INTO @@TraceInfo
SELECT c.TraceColumn, e.TraceEvent
FROM (
-- Trace columns
SELECT 1 AS TraceColumn UNION ALL
SELECT 5 UNION ALL
SELECT 13 UNION ALL
SELECT 14 UNION ALL
SELECT 15 UNION ALL
SELECT 22 UNION ALL
SELECT 28 UNION ALL
SELECT 29 UNION ALL
SELECT 31 UNION ALL
SELECT 34 UNION ALL
SELECT 55 UNION ALL
SELECT 61 ) as c

CROSS APPLY

-- Events
(SELECT 40 AS TraceEvent UNION ALL
SELECT 41 UNION ALL
SELECT 42 UNION ALL
SELECT 43 UNION ALL
SELECT 44 ) as e

declare @@i int, @@max int
select @@i = min(RowID), @@max = max(RowID) from @@TraceInfo

declare @@TraceColumn int, @@TraceEvent int;

while @@i <= @@max begin

	SELECT
		@@TraceColumn = TraceColumn,
		@@TraceEvent = TraceEvent
	FROM @@TraceInfo
		WHERE RowID = @@i;
	
	execute sp_trace_setevent @@TraceID, @@TraceEvent, @@TraceColumn, @@on
	set @@i = @@i + 1
end
";

         if (filterData is int)
            sqlString += @"execute sp_trace_setfilter @@TraceID, 9, 0, 0, " + ((int) filterData) + @";";
         else
            sqlString += @"execute sp_trace_setfilter @@TraceID, 10, 0, 0, N'" + ((string)filterData).Replace("'", "") + @"';";

sqlString += @"
execute sp_trace_setstatus @@TraceID, 1 -- start
error:
select @@TraceID";

         var traceId = _db.ExecuteScalar<int>(sqlString);

         return traceId;
      }

      public override void StopTrace(int traceId)
      {
         _db.Execute(@"exec sp_trace_setstatus " + traceId + @", 0 -- stop
exec sp_trace_setstatus " + traceId +  @", 2 -- delete");
      }

      public override List<DbCodeSegment> GetTraceCodeSegments(string trace)
      {
         trace = trace + ".trc";
         var codeTrace = _db.Fetch<DbCodeSegment>(@"SELECT DISTINCT LineNumber, Offset as StartByte, IntegerData2 as EndByte, ObjectName
FROM ::fn_trace_gettable('" + trace + @"', default) 
WHERE EventClass IN (40,41,42,43,44) AND Offset IS NOT NULL AND ObjectName IS NOT NULL
ORDER BY ObjectName, LineNumber ASC, StartByte ASC, IntegerData2 ASC;");
         return codeTrace;
      }

      public override List<DbObject> GetAllObjects()
      {
         var objects = _db.Fetch<DbObject>(@"SELECT
	SCHEMA_NAME(sys.objects.schema_id) AS [Schema],
	sys.objects.name as [Name],
	CASE sys.objects.type
		WHEN 'P' THEN 'PROCEDURE'
		WHEN 'TF' THEN 'FUNCTION/TABLE-VALUED'
		WHEN 'TR' THEN 'TRIGGER'
		WHEN 'FN' THEN 'FUNCTION/SCALAR'
		WHEN 'IF' THEN 'FUNCTION/INLINE TABLE-VALUED'
	END as [Type],
	OBJECT_DEFINITION(sys.objects.object_id) as [Code]
from sys.objects
where sys.objects.type in ('tr','p','fn','if','tf')");
         return objects;
      }
   }
}
