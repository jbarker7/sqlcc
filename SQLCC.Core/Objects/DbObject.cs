using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SQLCC.Core.Objects
{
   [DataContract]
   public class DbObject
   {
      [DataMember]
      public string Schema { get; set; }

      [DataMember]
      public string Name { get; set; }

      [DataMember]
      public string Type { get; set; }

      [IgnoreDataMember]
      public string Code { get; set; }

      private string _codeHighlighted;

      [IgnoreDataMember] 
      public string CodeHighlighted 
      {
         get { return _codeHighlighted ?? (_codeHighlighted = this.Code); }
         set { _codeHighlighted = value; }
      }

      [IgnoreDataMember]
      public List<DbCodeSegment> CoveredSegments { get; set; }

      private int _functionalLinesOfCode;

      [DataMember]
      public int TotalFloc
      {
         get
         {
            if (_functionalLinesOfCode == 0)
            {
               _functionalLinesOfCode = _linesOfCode;
            }
            return _functionalLinesOfCode;
         }
         set { _functionalLinesOfCode = value; }
      }

      private int _linesOfCode;

      [DataMember]
      public int TotalLoc
      {
         get
         {
            if (_linesOfCode == 0 && this.Code != null)
            {
               _linesOfCode = this.Code.Split('\n').Length;
            }
            return _linesOfCode;
         }
         set
         {
            _linesOfCode = value;
         }
      }

      private int _numOfCharacters;

      [DataMember]
      public int TotalCharacters
      {
         get
         {
            if (_numOfCharacters == 0 && this.Code != null)
            {
               _numOfCharacters = this.Code.Length;
            }
            return _numOfCharacters;
         }
         set
         {
            _numOfCharacters = value;
         }
      }

      private int _totalFuncCharacters;

      [DataMember]
      public int TotalFunctionalCharacters
      {
         get
         {
            if (_totalFuncCharacters == 0 && this.Code != null)
            {
               _totalFuncCharacters = this.Code.Length;
            }
            return _totalFuncCharacters;
         }
         set
         {
            _totalFuncCharacters = value;
         }
      }

      [DataMember]
      public int CoveredCharacters { get; set; }

      [DataMember]
      public int CoveredLinesOfCode { get; set; }

      [DataMember]
      public decimal CoveredPercent { get; set; }

      public void Set(DbObject dbObject)
      {
         Code = dbObject.Code;
         CodeHighlighted = dbObject.CodeHighlighted;
         CoveredSegments = dbObject.CoveredSegments;
         CoveredCharacters = dbObject.CoveredCharacters;
         CoveredLinesOfCode = dbObject.CoveredLinesOfCode;
         CoveredPercent = dbObject.CoveredPercent;
         TotalLoc = dbObject.TotalLoc;
         TotalFloc = dbObject.TotalFloc;
         TotalFunctionalCharacters = dbObject.TotalFunctionalCharacters;
         Name = dbObject.Name;
         TotalCharacters = dbObject.TotalCharacters;
      }

      public DbObject Get()
      {
         return new DbObject()
         {
            Code = this.Code,
            CodeHighlighted = this.CodeHighlighted,
            CoveredSegments = this.CoveredSegments,
            CoveredCharacters = this.CoveredCharacters,
            CoveredLinesOfCode = this.CoveredLinesOfCode,
            CoveredPercent = this.CoveredPercent,
            TotalLoc = this.TotalLoc,
            TotalFloc = this.TotalFloc,
            Name = this.Name,
            TotalFunctionalCharacters = this.TotalFunctionalCharacters,
            TotalCharacters = this.TotalCharacters
         };
      }

   }
}