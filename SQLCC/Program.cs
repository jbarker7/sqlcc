using System;
using System.Collections.Generic;
using System.Configuration;
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

         var loader = new AssemblyLoader();
         var dbProvider = loader.CreateTypeFromAssembly<DbProvider>(arguments["dbp.provider"], arguments);
         var dbCodeFormatter = loader.CreateTypeFromAssembly<DbTraceCodeFormatter>(arguments["tcf.provider"], arguments);
         var codeHighlighter = loader.CreateTypeFromAssembly<HighlightCodeProvider>(arguments["hcp.provider"], arguments);
         var outputProvider = loader.CreateTypeFromAssembly<OutputProvider>(arguments["out.provider"], arguments);

         switch (arguments["app.mode"].ToLower().Trim())
         {
            case "generate":
               var generateCommand = new GenerateOutputCommand(dbProvider, dbCodeFormatter, codeHighlighter, outputProvider, arguments["app.traceName"]);
               generateCommand.Execute();
               break;

            case "start":
               var startCommand = new StartCommand(outputProvider, dbProvider, arguments["app.traceName"]);
               startCommand.Execute();
               break;

            case "stop":
               {
                  var stopCommand = new StopCommand(dbProvider, outputProvider, arguments["app.traceName"]);
                  stopCommand.Execute();
               }
               break;
         }

      }

   }


}
