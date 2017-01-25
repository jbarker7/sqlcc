using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using NDesk.Options;
using SQLCC.Core;
using SQLCC.Core.Helpers;
using SQLCC.Core.Objects;
using SQLCC.Commands;

namespace SQLCC
{
   class Program
   {
      static void Main(string[] args)
      {
         var arguments = ParseCommandLine(args);
         RequiredAttributes(arguments, 
             "databaseProvider", 
             "codeFormatProvider", 
             "codeHighlightProvider", 
             "outputProvider", 
             "action"
             );

         var loader = new AssemblyLoader();
         var dbProvider = loader.CreateTypeFromAssembly<DbProvider>(arguments["databaseProvider"], arguments);
         var dbCodeFormatter = loader.CreateTypeFromAssembly<DbTraceCodeFormatter>(arguments["codeFormatProvider"], arguments);
         var codeHighlighter = loader.CreateTypeFromAssembly<HighlightCodeProvider>(arguments["codeHighlightProvider"], arguments);
         var outputProvider = loader.CreateTypeFromAssembly<OutputProvider>(arguments["outputProvider"], arguments);

         var command = arguments["action"].ToLower().Trim();

         var traceName = arguments.ContainsKey("traceFileName") ? arguments["traceFileName"] : null;
         switch (command)
         {
             case "generate":
             {
                 RequiredAttributes(arguments,
                  "traceFileName"
                 );

                 var generateCommand = new GenerateOutputCommand(dbProvider, dbCodeFormatter, codeHighlighter, outputProvider, traceName);
                 generateCommand.Execute();
                 break;
             }

             case "start":
             {
                 traceName = traceName ?? DateTime.Now.ToString("yyyyMMddHHmmss");
                 var startCommand = new StartCommand(outputProvider, dbProvider, traceName);
                 startCommand.Execute();

                 break;
             }

             case "stop":
             {
                 traceName = traceName ?? DateTime.Now.ToString("yyyyMMddHHmmss");
                 var stopCommand = new StopCommand(dbProvider, outputProvider, traceName);
                 stopCommand.Execute();

                 break;
             }

             case "execute":
             {
                 RequiredAttributes(arguments,
                  "target"
                 );

                 traceName = traceName ?? DateTime.Now.ToString("yyyyMMddHHmmss");
                 var startCommand = new StartCommand(outputProvider, dbProvider, traceName);
                 startCommand.Execute();

                 var executeCommand = new ExecuteCommand(arguments["target"], arguments.ContainsKey("targetArgs") ? arguments["targetArgs"] : string.Empty);
                 executeCommand.Execute();

                 var stopCommand = new StopCommand(dbProvider, outputProvider, traceName);
                 stopCommand.Execute();

                 break;
             }
         }

      }

       public static void RequiredAttributes(Dictionary<string, string> args, params string[] requiredArgs)
       {
           foreach (var arg in requiredArgs)
           {
               if (!args.ContainsKey(arg))
                   throw new ApplicationException("Required argument " + arg + " not found!");
           }
       }

      public static Dictionary<string, string> ParseCommandLine(string[] args)
      {
          var arguments = new Dictionary<string, string>();

          // App.Config Settings
          var appSettingKeys = ConfigurationManager.AppSettings.Keys;
          for (var i = 0; i < appSettingKeys.Count; i++)
          {
              var key = appSettingKeys[i];
              arguments.AddOrUpdate(key, ConfigurationManager.AppSettings[key]);
          }

          // Manual override through CLI.
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
          return arguments;
      }

   }


}
