using System.Collections.Generic;
using SQLCC.Core.Interfaces;
using SQLCC.Core.Objects;

namespace SQLCC.Core
{
   public abstract class HighlightCodeProvider : IHighlightCodeProvider
   {
      public abstract string HighlightCode(string code, List<DbCodeSegment> codeSegments);
   }
}
