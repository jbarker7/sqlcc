using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Diagnostics;
using NDesk.Options;
using SQLCC.Core;
using SQLCC.Core.Helpers;
using SQLCC.Core.Objects;

namespace SQLCC
{
   class Program
   {
      static void Main(string[] args)
      {
         var arguments = new Dictionary<string, string>();
         var appSettingKeys = ConfigurationManager.AppSettings.Keys;
         for (var i = 0; i < appSettingKeys.Count; i++)
         {
            var key = appSettingKeys[i];
            arguments.AddOrUpdate(key, ConfigurationManager.AppSettings[key]);
         }

         var p = new OptionSet()
                    {
                       {
                          "<>", v =>
                                   {
                                      if (!v.StartsWith("--"))
                                         return;
                                      var split = v.Split(new[] { '=' }, 2);
                                      if (split.Length != 2)
                                         return;
                                      arguments.AddOrUpdate(split[0].TrimStart('-'), split[1]);
                                   }
                          }
                    };

         p.Parse(args);

         var fileName = Path.Combine(arguments["dbp.traceDir"], arguments["dbp.traceFileName"]);

         var loader = new AssemblyLoader();
         var dbProvider = loader.CreateTypeFromAssembly<DbProvider>(arguments["dbp.provider"], arguments);
         var dbCodeFormatter = loader.CreateTypeFromAssembly<DbTraceCodeFormatter>(arguments["tcf.provider"], arguments);
         var codeHighlighter = loader.CreateTypeFromAssembly<HighlightCodeProvider>(arguments["hcp.provider"], arguments);
         var outputProvider = loader.CreateTypeFromAssembly<OutputProvider>(arguments["out.provider"], arguments);

         // Start
         if (arguments["app.mode"].ToLower().Trim() == "start")
         {
            dbProvider.StartTrace(fileName, arguments["dbp.applicationName"]);
         }
         else if (arguments["app.mode"].ToLower().Trim() == "stop")
         {
            // Stop
            dbProvider.StopTrace(2); // TODO: Do not hard code "2"

            var codeCoverageProcessor = new CodeCoverageProcessor(dbCodeFormatter, codeHighlighter);

            var codeCover = new DbCodeCoverage();
            var codeCoverName = Path.GetFileName(fileName);
            codeCover.Name = codeCoverName; //DateTime.Now.ToString("yyyyMMddHHmmssFFF");
            codeCover.TotalObjects = dbProvider.GetAllObjects();
            codeCover.TraceCodeSegments = dbProvider.GetTraceCodeSegments(fileName);

            codeCoverageProcessor.ProcessAllCoverage(codeCover);

            if (outputProvider.SetUp())
               outputProvider.SaveResults(codeCover);
         }


         //var deleteOlderThan = 10;
         //Mode mode;
         //string localProfileApp = null, 
         //   localOutputDir = null, 
         //   serverTraceDir = null, 
         //   serverConnString = null;

         //// each module might need their own options.
         //// highlightProvider=
         //// dbProvider=

         //var p = new OptionSet()
         //   .Add("deleteOlderThan=", delegate(int v) { deleteOlderThan = v; })
         //   .Add("mode=", v => Enum.TryParse(v, out mode))
         //   .Add("localApp=", delegate (string v) { localProfileApp = v; })
         //   .Add("localOutputDir=", delegate (string v) { localOutputDir = v; })
         //   .Add("serverTraceDir=", delegate (string v) { serverTraceDir = v; })
         //   .Add("serverConn=", delegate (string v) { serverConnString = v; });

         //p.Parse(args);

         //if (localProfileApp == null || localOutputDir == null || serverTraceDir == null || serverConnString == null)
         //{
         //   p.WriteOptionDescriptions(Console.Out);
         //   return;
         //}

         //var provider = new MsSqlProvider(serverConnString);

         //var traceId = -1;
         //string fileName = null;

         //Console.WriteLine("Press the ESCAPE key to stop");
         //do
         //{
         //   while (!Console.KeyAvailable)
         //   {
         //      Thread.Sleep(250);
         //      var pid = GetPidOfExecutable(localProfileApp);
         //      //"C:\Code\ConsoleApplication1\ConsoleApplication1\bin\Debug\ConsoleApplication1.exe"

         //      // found, start trace.  if started, wait until stops to stop trace.
         //      bool traceStarted = traceId > -1;
         //      if (pid > -1)
         //      {
         //         if (!traceStarted)
         //         {
         //            Console.WriteLine("Starting Trace...");
         //            fileName = Path.Combine(serverTraceDir, DateTime.Now.ToString("yyyyMMddHHmmssFFF")); // @"C:\Code\"
         //            traceId = provider.StartTrace(fileName, pid);
         //         }
         //      }
         //      else
         //      {
         //         if (traceStarted)
         //         {
         //            Console.WriteLine("Stopping Trace..."); 
         //            provider.StopTrace(traceId);

         //            // generate code coverage
         //            GenerateCodeCoverageResults(fileName, localProfileApp, localOutputDir, serverTraceDir, serverConnString);

         //            traceId = -1;
         //            fileName = null;
         //         }
         //      }
         //   }
         //} while (Console.ReadKey(true).Key != ConsoleKey.Escape);
      }
      
      private static int GetPidOfExecutable(string executable)
      {
         var pid = -1;
         var processlist = Process.GetProcesses();
         foreach (Process theprocess in processlist)
         {
            try
            {
               if (theprocess.MainModule.FileName == executable)
               {
                  pid = theprocess.Id;
                  break;
               }
            }
            catch
            {

            }
         }
         return pid;
      }
   }


}
