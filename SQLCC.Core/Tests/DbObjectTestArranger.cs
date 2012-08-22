namespace SQLCC.Core.Tests
{
   public abstract class DbObjectTestArranger : DbObjectTestActor
   {
      private readonly DbObjectTestActor _testActor;

      protected DbObjectTestArranger(DbObjectTestActor testActor)
      {
         _testActor = testActor;
      }

      protected abstract void SetUp();
      protected abstract void TearDown();

      public sealed override object Execute(params object[] parameters)
      {
         this.SetUp();
         var returnVal = _testActor.Execute(parameters);
         this.TearDown();
         return returnVal;
      }
   }
}
