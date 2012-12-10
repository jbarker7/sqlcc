using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLCC.Core;

namespace SQLCC.Commands
{
   public class StartCommand
   {
      private OutputProvider _outputProvider;
      private DbProvider _dbProvider;
      private string _traceName;

      public StartCommand(OutputProvider outputProvider, DbProvider dbProvider, string traceName)
      {
         _outputProvider = outputProvider;
         _dbProvider = dbProvider;
         _traceName = traceName;
      }

      public void Execute()
      {
         if (_dbProvider.IsTraceRunning(_traceName))
         {
            throw new ApplicationException("You cannot start more than one trace at a time!");
         }

         _outputProvider.SetUp(_traceName);
         _dbProvider.StartTrace(_traceName);
      }
   }
}
