using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SQLCC.Core;
using SQLCC.Core.Helpers;
using SQLCC.Core.Objects;

namespace SQLCC
{
   public class CodeCoverageProcessor 
   {
      private readonly DbTraceCodeFormatter _dbCodeFormatter;
      private readonly HighlightCodeProvider _highlightCodeProvider;
      private readonly int _highlightMarkUpLength;
      private readonly DataScrubber _dataScrubber;

      public CodeCoverageProcessor(DbTraceCodeFormatter dbCodeFormatter, HighlightCodeProvider highlightCodeProvider)
      {
         _dbCodeFormatter = dbCodeFormatter;
         _highlightCodeProvider = highlightCodeProvider;
         _dataScrubber = new DataScrubber(dbCodeFormatter);
         _highlightMarkUpLength = _dbCodeFormatter.StartHighlightMarkUp.Length + _dbCodeFormatter.EndHighlightMarkUp.Length;
      }

      private List<DbCodeSegment> ProcessRawCodeSegments(MatchCollection rawCodeSegments)
      {
         var coveredSegments = new List<DbCodeSegment>();
          var countToSubtractFromPosition = 0;
         for (var i = 0; i < rawCodeSegments.Count; i++)
         {
            var codeSegment = rawCodeSegments[i];
            var dbCodeSegment = new DbCodeSegment();
            dbCodeSegment.LinesOfCode = codeSegment.Groups[1].Value.Split('\n').Length;
            dbCodeSegment.StartByte = codeSegment.Index - countToSubtractFromPosition;
            dbCodeSegment.EndByte = dbCodeSegment.StartByte + codeSegment.Length - _highlightMarkUpLength;
            coveredSegments.Add(dbCodeSegment);
            countToSubtractFromPosition += _highlightMarkUpLength;
         }
         return coveredSegments;
      }

      private MatchCollection GetRawHighlightedCodeSegments(string code)
      {
         return Regex.Matches(code, string.Format("{0}(.*?){1}", _dbCodeFormatter.StartHighlightMarkUp, _dbCodeFormatter.EndHighlightMarkUp), RegexOptions.Singleline);
      }

      private List<DbCodeSegment> ProcessHighlightedCode(string highlightedCode)
      {
         return ProcessRawCodeSegments(GetRawHighlightedCodeSegments(highlightedCode));
      }
      
      public DbObject ProcessObjectCoverage(DbObject dbObject)
      {
         var dbObjectClone = dbObject.Get();

         if (dbObject.Code == null)
            return dbObjectClone;

         if (dbObject.CoveredSegments.Count > 0)
         {
            var codeWithHighlights = _dbCodeFormatter.FormatCodeWithHighlights(dbObjectClone.Code,
                                                                               dbObjectClone.CoveredSegments);
            dbObjectClone.CoveredSegments = ProcessHighlightedCode(codeWithHighlights);

            var functionalHighlightedCode = _dataScrubber.Scrub(codeWithHighlights, "floc.scrub");
            var functionalSegments = ProcessHighlightedCode(functionalHighlightedCode);
            foreach (var functionalSegment in functionalSegments)
            {
               dbObjectClone.CoveredCharacters += functionalSegment.EndByte - functionalSegment.StartByte;
               dbObjectClone.CoveredLinesOfCode += functionalSegment.LinesOfCode;
            }
         }

         var functionalCode = _dataScrubber.Scrub(dbObject.Code, "floc.scrub");

         dbObjectClone.TotalLoc = dbObject.Code.Split('\n').Length;
         dbObjectClone.TotalCharacters = dbObject.Code.Length;

         dbObjectClone.TotalFloc = functionalCode.Split('\n').Length;
         dbObjectClone.TotalFunctionalCharacters = functionalCode.Length;

         dbObjectClone.CoveredPercent = (decimal)dbObjectClone.CoveredCharacters / dbObjectClone.TotalFunctionalCharacters;
         

         return dbObjectClone;
      }

      public void ProcessAllCoverage(DbCodeCoverage codeCover)
      {
         foreach (var obj in codeCover.TotalObjects)
         {
            obj.CoveredSegments = codeCover.TraceCodeSegments.Where(p => p.ObjectName.Equals(obj.Name)).ToList();
            obj.Set(ProcessObjectCoverage(obj));
            obj.CodeHighlighted = _highlightCodeProvider.HighlightCode(obj.Code, obj.CoveredSegments);

            codeCover.TotalLoc += obj.TotalLoc;
            codeCover.TotalFloc += obj.TotalFloc;
            codeCover.TotalCharacters += obj.TotalCharacters;
            codeCover.TotalFunctionalCharacters += obj.TotalFunctionalCharacters;
            codeCover.CoveredCharacters += (int)Math.Round(obj.TotalCharacters * obj.CoveredPercent);
            codeCover.CoveredLinesOfCode += (int)Math.Round(obj.TotalLoc * obj.CoveredPercent);
         }
         codeCover.CoveredPercent = ((decimal)codeCover.CoveredCharacters / codeCover.TotalFunctionalCharacters);
      }
   }
}
