using System.Collections.Generic;
using System.Text;
using SQLCC.Core;
using SQLCC.Core.Objects;

namespace SQLCC.Impl.HtmlCodeHighlighter
{
   public class HtmlHighlightCodeProvider : HighlightCodeProvider
   {
      private const string InsertStartTag = "<span class=\"cc\">";
      private const string InsertEndTag = "</span>";

      public override string HighlightCode(string code, List<DbCodeSegment> codeSegments)
      {
         if (codeSegments.Count == 0)
            return code;
         var codeContents = new StringBuilder(code);
         var offset = 0;
         foreach (var lineOfCode in codeSegments)
         {
            codeContents.Insert(lineOfCode.StartByte + offset, InsertStartTag);
            offset += InsertStartTag.Length;
            codeContents.Insert(lineOfCode.EndByte + offset, InsertEndTag);
            offset += InsertEndTag.Length;
         }
         return codeContents.ToString();
      }
   }
}
