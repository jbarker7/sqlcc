using System.Collections.Generic;
using SQLCC.Core.Interfaces;
using SQLCC.Core.Objects;

namespace SQLCC.Core
{
   public abstract class HighlightCodeProvider : IHighlightCodeProvider
   {
      public string ArgumentNamespace { get { return "hcp"; } }
      public abstract string HighlightCode(string code, List<DbCodeSegment> codeSegments);
   }
}
