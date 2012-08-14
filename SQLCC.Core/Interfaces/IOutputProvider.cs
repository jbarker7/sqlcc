using SQLCC.Core.Objects;

namespace SQLCC.Core.Interfaces
{
   internal interface IOutputProvider : IExtension
   {
      bool SetUp();

      bool SaveResults(DbCodeCoverage codeCoverage);

      bool TearDown();
   }
}
