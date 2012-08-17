using SQLCC.Core.Interfaces;
using SQLCC.Core.Objects;

namespace SQLCC.Core
{
   public abstract class OutputProvider : IOutputProvider
   {
      public string ArgumentNamespace { get { return "out"; } }
      public abstract bool SetUp(string traceName);
      public abstract bool SaveResults(DbCodeCoverage codeCoverage);
      public abstract string GetStartedTraceName();
      public abstract bool TearDown(string traceName);
   }
}
