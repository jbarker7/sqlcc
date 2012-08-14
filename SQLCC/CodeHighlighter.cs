//using System.Collections.Generic;
//using System.Text;
//using SQLCC.Core;
//using SQLCC.Core.Objects;

//namespace SQLCC
//{
//   public class CodeHighlighter
//   {
//      private const string InsertStartTag = "<span class=\"cc\">";
//      private const string InsertEndTag = "</span>";

//      private readonly IDbCodeFormatter _codeFormatter;

//      public CodeHighlighter(IDbCodeFormatter codeFormatter)
//      {
//         _codeFormatter = codeFormatter;
//      }

//      public StringBuilder Highlight(StringBuilder codeContents, List<DbCodeSegment> codeTrace)
//      {
//         var offset = 0;
//         foreach (var lineOfCode in codeTrace)
//         {
//            codeContents.Insert((lineOfCode.StartByte / 2) + offset, InsertStartTag);
//            offset += InsertStartTag.Length;

//            codeContents.Insert((lineOfCode.EndByte / 2) + offset, InsertEndTag);
//            offset += InsertEndTag.Length;
//         }

//         return _codeFormatter.FormatCode(codeContents.ToString(), "format.scrub");
//      }
//   }
//}
