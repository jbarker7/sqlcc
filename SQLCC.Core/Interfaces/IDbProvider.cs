using System.Collections.Generic;
using SQLCC.Core.Objects;

namespace SQLCC.Core.Interfaces
{
   internal interface IDbProvider : IExtension
   {
      void StartTrace(string traceName);

      void StopTrace(string traceName);

      bool IsTraceRunning(string traceName);

      List<DbCodeSegment> GetTraceCodeSegments(string traceName);

      List<DbObject> GetAllObjects();
   }
}
