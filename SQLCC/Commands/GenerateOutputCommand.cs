using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLCC.Core;
using SQLCC.Core.Interfaces;
using SQLCC.Core.Objects;

namespace SQLCC.Commands
{
   public class GenerateOutputCommand : ICommand
   {
      private DbProvider _dbProvider;
      private DbTraceCodeFormatter _dbCodeFormatter;
      private HighlightCodeProvider _codeHighlighter;
      private OutputProvider _outputProvider;
      private string _traceName;

      public GenerateOutputCommand(DbProvider dbProvider, DbTraceCodeFormatter dbCodeFormatter,
         HighlightCodeProvider codeHighlighter, OutputProvider outputProvider, string traceName)
      {
         _dbProvider = dbProvider;
         _dbCodeFormatter = dbCodeFormatter;
         _codeHighlighter = codeHighlighter;
         _outputProvider = outputProvider;
         _traceName = traceName;
      }

      public void Execute()
      {
         var objects = _dbProvider.GetAllObjects();
         foreach (var o in objects)
         {
             o.Name = $"{o.Schema}.{o.Name}";
         }
         var codeCoverageProcessor = new CodeCoverageProcessor(_dbCodeFormatter, _codeHighlighter);
         var codeCover = new DbCodeCoverage();

         codeCover.Name = _traceName;
         codeCover.TotalObjects = objects;
         codeCover.TraceCodeSegments = _dbProvider.GetTraceCodeSegments(_traceName);

         codeCoverageProcessor.ProcessAllCoverage(codeCover);

         _outputProvider.SetUp(_traceName);
         _outputProvider.SaveResults(codeCover);
      }
   }
}
