using System.Collections.Generic;
using SQLCC.Core.Objects;

namespace SQLCC.Core.Interfaces
{
   internal interface IDbProvider : IExtension
   {
      int StartTrace(string tracePath, object filterData);

      void StopTrace(int traceId);

      List<DbCodeSegment> GetTraceCodeSegments(string trace);

      List<DbObject> GetAllObjects();
   }
}
