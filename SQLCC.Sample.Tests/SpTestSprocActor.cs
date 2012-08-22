using System.Collections.Generic;
using PetaPoco;
using SQLCC.Core.Tests;

namespace SQLCC.Sample.Tests
{
   public class SpTestSprocReturn
   {
      public int TestId { get; set; }

      public string Name { get; set; }
   }

   public class SpTestSprocActor : DbObjectTestActor
   {
      private readonly PetaPoco.Database _db;

      public SpTestSprocActor()
      {
         _db = new Database("Data Source=.;Initial Catalog=TestDb;Trusted_Connection=True;Application Name=SQLCC;", "System.Data.SqlClient");
      }

      public override object Execute(params object[] parameters)
      {
         return _db.Fetch<SpTestSprocReturn>("EXECUTE spTestSproc @@TestName = @0", parameters);
      }
   }

   public class SpTestSprocArranger : DbObjectTestArranger
   {
      private readonly PetaPoco.Database _db;

      public SpTestSprocArranger(DbObjectTestActor objectTest)
         : base(objectTest)
      {
         _db = new Database("Data Source=.;Initial Catalog=TestDb;Trusted_Connection=True", "System.Data.SqlClient");
      }

      protected override void SetUp()
      {
         _db.Execute(@"
INSERT INTO dbo.TestTable (Name)
SELECT 'Test1' UNION ALL
SELECT 'Test2' UNION ALL
SELECT 'Test3';
");
      }

      protected override void TearDown()
      {
         _db.Execute(@"TRUNCATE TABLE dbo.TestTable;");
      }
   }

}
