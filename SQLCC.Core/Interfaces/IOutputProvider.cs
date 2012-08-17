using SQLCC.Core.Objects;

namespace SQLCC.Core.Interfaces
{
   internal interface IOutputProvider : IExtension
   {
      bool SetUp(string traceName);

      bool SaveResults(DbCodeCoverage codeCoverage);

      DbCodeCoverage GetStartedTraceName(); 

      bool TearDown(string traceName);

   }
}
