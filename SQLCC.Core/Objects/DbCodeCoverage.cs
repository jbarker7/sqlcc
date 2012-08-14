using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SQLCC.Core.Objects
{
   [DataContract]
   public class DbCodeCoverage
   {
      [DataMember]
      public string Name { get; set; }

      [DataMember]
      public int TotalLoc { get; set; }

      private int _totalObjectCount;

      [DataMember]
      public int TotalObjectCount
      {
         get 
         { 
            if (_totalObjectCount == 0 && this.TotalObjects != null)
            {
               _totalObjectCount = this.TotalObjects.Count;
            }
            return _totalObjectCount;
         }
         set { _totalObjectCount = value; }
      }

      [IgnoreDataMember]
      public List<DbObject> TotalObjects { get; set; }

      [DataMember]
      public int TotalCharacters { get; set; }

      [DataMember]
      public int TotalFunctionalCharacters { get; set; }

      [DataMember]
      public int TotalFloc { get; set; }

      [DataMember]
      public int CoveredLinesOfCode { get; set; }

      [DataMember]
      public int CoveredCharacters { get; set; }

      [DataMember]
      public decimal CoveredPercent { get; set; }

      [IgnoreDataMember]
      public List<DbCodeSegment> TraceCodeSegments { get; set; }
   }
}
