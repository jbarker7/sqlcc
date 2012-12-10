using System;
using System.Collections.Generic;
using System.IO;
using SQLCC.Core;
using SQLCC.Core.Objects;

namespace SQLCC.Impl.MsSqlProvider
{
   public class MsSqlProvider : DbProvider
   {
      private readonly PetaPoco.Database _db;
      private readonly string _applicationName;
      private readonly string _traceDir;

      public MsSqlProvider(string connString, string traceDir, string applicationName)
      {
         _db = new PetaPoco.Database(connString, "System.Data.SqlClient");
         _applicationName = applicationName;
         _traceDir = traceDir;
      }

      public override void StartTrace(string traceName)
      {
         var trace = Path.Combine(_traceDir, traceName);

         var sqlString = 
               @"declare @@rc int
declare @@TraceID int
declare @@maxfilesize bigint
declare @@DateTime datetime
set @@DateTime = DateAdd(hour, 12, GetDate()) -- auto trace stop
set @@maxfilesize = 1000
exec @@rc = sp_trace_create @@TraceID output, 0, N'" +
               trace +
               @"', @@maxfilesize, @@Datetime
if (@@rc != 0) goto error

declare @@on bit;set @@on = 1;
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

declare @@i int, @@max int;
select @@i = min(RowID), @@max = max(RowID) from @@TraceInfo;
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
         sqlString += @"execute sp_trace_setfilter @@TraceID, 8, 0, 0, N'" + Environment.MachineName + @"';";
         sqlString += @"execute sp_trace_setfilter @@TraceID, 10, 0, 0, N'" + _applicationName.Replace("'", "") + @"';";

sqlString += @"
execute sp_trace_setstatus @@TraceID, 1 -- start
error:
select @@TraceID";

         _db.ExecuteScalar<int>(sqlString);
      }

      public override bool IsTraceRunning(string traceName)
      {
         var trace = Path.Combine(_traceDir, traceName + ".trc");
         var traceCount = _db.ExecuteScalar<int>(@"SELECT TraceCount = count(1) FROM sys.traces Where [path] = '" + trace + @"'");

         bool isTraceRunning = (traceCount > 0);

         return isTraceRunning;
      }


      public override void StopTrace(string traceName)
      {
         var trace = Path.Combine(_traceDir, traceName + ".trc");
         _db.Execute(@"
DECLARE @@TraceID INT;
SELECT @@TraceID = id FROM sys.traces Where [path] = '" + trace + @"'
exec sp_trace_setstatus @@TraceID, 0 -- stop
exec sp_trace_setstatus @@TraceID, 2 -- delete");
      }

      public override List<DbCodeSegment> GetTraceCodeSegments(string traceName)
      {
         var trace = Path.Combine(_traceDir, traceName + ".trc");
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
