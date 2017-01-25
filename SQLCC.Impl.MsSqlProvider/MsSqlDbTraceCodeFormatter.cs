using System.Collections.Generic;
using System.Text;
using SQLCC.Core;
using SQLCC.Core.Helpers;
using SQLCC.Core.Objects;

namespace SQLCC.Impl.MsSqlProvider
{
   public class MsSqlDbTraceCodeFormatter : DbTraceCodeFormatter
   {
      private readonly DataScrubber _dataScrubber;

      public MsSqlDbTraceCodeFormatter()
      {
         _dataScrubber = new DataScrubber(this);
      }

      public override string FormatCodeWithHighlights(string code, List<DbCodeSegment> codeSegments)
      {
         if (codeSegments.Count == 0)
            return code;
         var codeContents = new StringBuilder(code);
         var offset = 0;
         foreach (var lineOfCode in codeSegments)
         {
            codeContents.Insert((lineOfCode.StartByte / 2) + offset, this.StartHighlightMarkUp);
            offset += this.StartHighlightMarkUp.Length;
            if (lineOfCode.EndByte == -1)
            {
                codeContents.Append(this.EndHighlightMarkUp);
            }
            else
            {
                codeContents.Insert((lineOfCode.EndByte / 2) + offset, this.EndHighlightMarkUp);
                offset += this.EndHighlightMarkUp.Length;
            }
         }
         return _dataScrubber.Scrub(codeContents.ToString(), "format.scrub");
      }
   }
}
