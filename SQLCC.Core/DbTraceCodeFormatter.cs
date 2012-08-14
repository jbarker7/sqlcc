using System.Collections.Generic;
using SQLCC.Core.Interfaces;
using SQLCC.Core.Objects;

namespace SQLCC.Core
{
   public abstract class DbTraceCodeFormatter : IDbTraceCodeFormatter
   {
      private const string InsertStartTag = "<SQLCC>";
      private const string InsertEndTag = "</SQLCC>";

      public string StartHighlightMarkUp
      {
         get { return InsertStartTag; }
      }

      public string EndHighlightMarkUp
      {
         get { return InsertEndTag; }
      }

      public abstract string FormatCodeWithHighlights(string code, List<DbCodeSegment> codeSegments);

      public string ArgumentNamespace
      {
         get { return "tcf"; }
      }
   }
}
