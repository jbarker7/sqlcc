using SQLCC.Core.Interfaces;
using SQLCC.Core.Objects;

namespace SQLCC.Core
{
   public abstract class OutputProvider : IOutputProvider
   {
      public abstract bool SetUp(string traceName);
      public abstract bool SaveResults(DbCodeCoverage codeCoverage);
      public abstract DbCodeCoverage GetStartedTraceName();
      public abstract bool TearDown(string traceName);
   }
}
