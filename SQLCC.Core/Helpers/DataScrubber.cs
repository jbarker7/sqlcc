using System.Text.RegularExpressions;

namespace SQLCC.Core.Helpers
{
   public class DataScrubber
   {
      private readonly DbTraceCodeFormatter _traceCodeFormatter;

      public DataScrubber(DbTraceCodeFormatter traceCodeFormatter)
      {
         _traceCodeFormatter = traceCodeFormatter;
      }

      public string Scrub(string code, string[] scrubArray)
      {
         if (code == null)
            return null;
         var finalCode = code;
         if (scrubArray.Length > 1)
         {
            scrubArray[1] = scrubArray[1].Replace("\\n", "\n");
            finalCode = Regex.Replace(finalCode, string.Format(scrubArray[0], _traceCodeFormatter.StartHighlightMarkUp, _traceCodeFormatter.EndHighlightMarkUp), string.Format(scrubArray[1], _traceCodeFormatter.StartHighlightMarkUp, _traceCodeFormatter.EndHighlightMarkUp), RegexOptions.IgnoreCase);
         }
         return finalCode;
      }

      public string Scrub(string code, string scrubFile)
      {
         if (code == null)
            return null;
         var finalCode = code;
         var scrubLines = System.IO.File.ReadAllLines(scrubFile);
         foreach (var scrubLine in scrubLines)
         {
            var scrubText = scrubLine.Split('\t');
            finalCode = Scrub(finalCode, scrubText);
         }
         return finalCode;
      }
   }
}
