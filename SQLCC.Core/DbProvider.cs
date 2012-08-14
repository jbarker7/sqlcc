using System.Collections.Generic;
using SQLCC.Core.Interfaces;
using SQLCC.Core.Objects;

namespace SQLCC.Core
{
   public abstract class DbProvider : IDbProvider
   {
      public string ArgumentNamespace
      {
         get { return "dbp"; }
      }

      public abstract int StartTrace(string tracePath, object filterData);

      public abstract void StopTrace(int traceId);

      public abstract List<DbCodeSegment> GetTraceCodeSegments(string trace);

      public abstract List<DbObject> GetAllObjects();
   }
}
