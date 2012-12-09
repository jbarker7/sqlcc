using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLCC.Core;

namespace SQLCC.Commands
{
   public class StopCommand
   {
      private DbProvider _dbProvider;
      private OutputProvider _outputProvider;
      private string _traceName;

      public StopCommand(DbProvider dbProvider, OutputProvider outputProvider, string traceName)
      {
         this._dbProvider = dbProvider;
         this._outputProvider = outputProvider;
         this._traceName = traceName;
      }

      public void Execute()
      {
         if (!_dbProvider.IsTraceRunning(_traceName))
         {
            throw new ApplicationException(
               "You must first start a trace. The last trace found was already completed.");
         }

         _dbProvider.StopTrace(_traceName); // TODO: Do not hard code "2"
      }
   }
}
