using System.Collections.Generic;
using PetaPoco;
using SQLCC.Core.Tests;

namespace SQLCC.Sample.Tests
{
   public class SpFooTestSprocReturn
   {
      public int FooTableId { get; set; }

      public string Description { get; set; }
   }

   public class SpFooTestSprocActor : DbObjectTestActor
   {

      private readonly PetaPoco.Database _db;

      public SpFooTestSprocActor()
      {
         _db = new Database("Data Source=.;Initial Catalog=TestDb;Trusted_Connection=True;Application Name=SQLCC;", "System.Data.SqlClient");
      }

      public override object Execute(params object[] parameters)
      {
         return _db.Fetch<SpFooTestSprocReturn>("EXECUTE spFooTestSproc @@TestName = @0, @@FooDescription = @1", parameters);
      }
   }

   public class SpFooTestSprocArranger : DbObjectTestArranger
   {
      private readonly PetaPoco.Database _db;

      public SpFooTestSprocArranger(DbObjectTestActor objectTest)
         : base(objectTest)
      {
         _db = new Database("Data Source=.;Initial Catalog=TestDb;Trusted_Connection=True;", "System.Data.SqlClient");
      }

      protected override void SetUp()
      {
         _db.Execute(@"
INSERT INTO dbo.FooTable ([Description])
SELECT 'Foo1' UNION ALL
SELECT 'Foo2' UNION ALL
SELECT 'Foo3' UNION ALL
SELECT 'StaticVar'
");
      }

      protected override void TearDown()
      {
         _db.Execute(@"TRUNCATE TABLE dbo.FooTable;");
      }
   }
}
