using SQLCC.Core.Interfaces;
using SQLCC.Core.Objects;

namespace SQLCC.Core
{
   public abstract class OutputProvider : IOutputProvider
   {
      public string ArgumentNamespace { get { return "out"; } }
      public abstract bool SetUp();
      public abstract bool SaveResults(DbCodeCoverage codeCoverage);
      public abstract bool TearDown();
   }
}
