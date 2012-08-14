using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Ionic.Zip;
using SQLCC.Core;
using SQLCC.Core.Objects;
using SQLCC.Impl.HtmlCodeHighlighter.Helpers;

namespace SQLCC.Impl.HtmlCodeHighlighter
{
   public class HtmlOutputProvider : OutputProvider
   {
      private readonly int _deleteOlderThan;
      private readonly string _outputDir;

      public HtmlOutputProvider(string deleteOlderThan, string outputDir)
      {
         _deleteOlderThan = int.Parse(deleteOlderThan);
         _outputDir = outputDir;
      }

      public override bool SetUp()
      {
         var assembly = Assembly.GetExecutingAssembly();
         var zipStream = assembly.GetManifestResourceStream("SQLCC.Impl.HtmlCodeHighlighter.sqlcc_output.zip");

         using (var zip = ZipFile.Read(zipStream))
         {
            zip.ExtractAll(_outputDir, ExtractExistingFileAction.DoNotOverwrite);
         }

         return true;
      }

      public override bool SaveResults(DbCodeCoverage codeCoverage)
      {
         var location = _outputDir;
         var newDirectory = Path.Combine(location, codeCoverage.Name);

         var template = File.ReadAllText(Path.Combine(location, "template.html"));

         Directory.CreateDirectory(newDirectory);

         foreach (var obj in codeCoverage.TotalObjects)
         {
            File.WriteAllText(Path.Combine(newDirectory, obj.Name + ".html"), template.Replace("[CODE]", obj.CodeHighlighted));
         }

         var json = "sqlcc_objects['" + codeCoverage.Name + "']=" + JsonHelper.JsonSerializer(codeCoverage.TotalObjects);

         File.WriteAllText(Path.Combine(newDirectory, "sqlcc.js"), json);

         // Create master
         var codeCoverages = new List<DbCodeCoverage>();
         if (File.Exists(Path.Combine(location, "sqlcc.js")))
         {
            var currentJsFile = File.ReadAllText(Path.Combine(location, "sqlcc.js")).Replace("var sqlcc=", "");
            codeCoverages = JsonHelper.JsonDeserialize<List<DbCodeCoverage>>(currentJsFile);
         }

         codeCoverages.Add(codeCoverage);

         var codeCoverJs = "var sqlcc=" + JsonHelper.JsonSerializer(codeCoverages);

         File.WriteAllText(Path.Combine(location, "sqlcc.js"), codeCoverJs);

         return true;
      }

      public override bool TearDown()
      {
         throw new System.NotImplementedException();
      }
   }
}
