using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Ionic.Zip;
using SQLCC.Core;
using SQLCC.Core.Objects;
using SQLCC.Impl.HtmlCodeHighlighter.Helpers;

namespace SQLCC.Impl.HtmlCodeHighlighter
{
   public class HtmlOutputProvider : OutputProvider
   {
      private readonly string _outputDir;

      public HtmlOutputProvider(string outputDirectory)
      {
          _outputDir = outputDirectory;
      }

      public override bool SetUp(string traceName)
      {
         var assembly = Assembly.GetExecutingAssembly();
         var zipStream = assembly.GetManifestResourceStream("SQLCC.Impl.HtmlCodeHighlighter.sqlcc_output.zip");

         using (var zip = ZipFile.Read(zipStream))
         {
            zip.ExtractAll(_outputDir, ExtractExistingFileAction.DoNotOverwrite);
         }

         WriteEmptyRunToCoverageFile(traceName);

         return true;
      }

      private List<DbCodeCoverage> GetCodeCoverageFile(string filePath)
      {
         var codeCoverages = new List<DbCodeCoverage>();
         if (File.Exists(filePath))
         {
            var currentJsFile = File.ReadAllText(filePath).Replace("var sqlcc=", "");
            codeCoverages = JsonHelper.JsonDeserialize<List<DbCodeCoverage>>(currentJsFile);
         }
         return codeCoverages;
      }

      private void SaveCodeCoverageFile(List<DbCodeCoverage> codeCoverages, string filePath)
      {
         var codeCoverJs = "var sqlcc=" + JsonHelper.JsonSerializer(codeCoverages);
         File.WriteAllText(filePath, codeCoverJs);
      }

      private void WriteAllDbObjectFiles(List<DbObject> objects, string baseLocation, string codeCoverageLocation)
      {
         var template = File.ReadAllText(Path.Combine(baseLocation, "template.html"));
         
         foreach (var obj in objects)
         {
            File.WriteAllText(Path.Combine(codeCoverageLocation, obj.Name + ".html"), template.Replace("[CODE]", obj.CodeHighlighted));
         }
      }

      private void WriteEmptyRunToCoverageFile(string traceName)
      {
         var codeCoverageFilePath = Path.Combine(_outputDir, "sqlcc.js");
         var codeCoverages = GetCodeCoverageFile(codeCoverageFilePath);
         codeCoverages.Add(new DbCodeCoverage() { Name = traceName, StartDate = DateTime.Now });
         SaveCodeCoverageFile(codeCoverages, codeCoverageFilePath);
      }

      private void WriteCodeCoverageMetaData(DbCodeCoverage codeCoverage, string location)
      {
         var json = "sqlcc_objects['" + codeCoverage.Name + "']=" + JsonHelper.JsonSerializer(codeCoverage.TotalObjects);
         File.WriteAllText(Path.Combine(location, "sqlcc.js"), json);
      }

      public void RewriteLastRunWithCoverageData(DbCodeCoverage codeCoverage)
      {
         var codeCoverageFilePath = Path.Combine(_outputDir, "sqlcc.js");
         var codeCoverages = GetCodeCoverageFile(codeCoverageFilePath);

         var lastCodeCoverage = codeCoverages.LastOrDefault();

         codeCoverage.Name = lastCodeCoverage.Name;
         codeCoverage.StartDate = lastCodeCoverage.StartDate;

         codeCoverages.RemoveAt(codeCoverages.Count - 1);
         codeCoverages.Add(codeCoverage);
         SaveCodeCoverageFile(codeCoverages, codeCoverageFilePath);
      }

      public override bool SaveResults(DbCodeCoverage codeCoverage)
      {
         codeCoverage.EndDate = DateTime.Now;

         var baseLocation = _outputDir;

         var codeCoverageLocation = Path.Combine(baseLocation, codeCoverage.Name);

         Directory.CreateDirectory(codeCoverageLocation);
         WriteCodeCoverageMetaData(codeCoverage, codeCoverageLocation);
         WriteAllDbObjectFiles(codeCoverage.TotalObjects, baseLocation, codeCoverageLocation);
         RewriteLastRunWithCoverageData(codeCoverage);

         return true;
      }
      
      public override DbCodeCoverage GetStartedTraceName()
      {
         var codeCoverageFilePath = Path.Combine(_outputDir, "sqlcc.js");
         var codeCoverages = GetCodeCoverageFile(codeCoverageFilePath);
         return codeCoverages.LastOrDefault();
      }

      public override bool TearDown(string traceName)
      {
         throw new System.NotImplementedException();
      }
   }
}
