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

      public abstract void StartTrace(string traceName);

      public abstract void StopTrace(string traceName);

      public abstract string GetLastTraceName();

      public abstract bool IsTraceRunning(string traceName);

      public abstract List<DbCodeSegment> GetTraceCodeSegments(string traceName);

      public abstract List<DbObject> GetAllObjects();
   }
}
