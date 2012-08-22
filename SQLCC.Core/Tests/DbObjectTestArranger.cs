using System.Data;

namespace SQLCC.Core.Tests
{
   public abstract class DbObjectTestArranger<T> : DbObjectTestActor<T>
   {
      private readonly DbObjectTestActor<T> _objectTest;

      protected DbObjectTestArranger(DbObjectTestActor<T> objectTest)
      {
         _objectTest = objectTest;
      }

      public abstract void SetUp();
      public abstract void TearDown();

      public sealed override T Execute(params object[] parameters)
      {
         this.SetUp();
         var returnVal = _objectTest.Execute(parameters);
         this.TearDown();
         return returnVal;
      }
   }

   public abstract class DbObjectTestArranger : DbObjectTestActor
   {
      private readonly DbObjectTestActor _objectTest;

      protected DbObjectTestArranger(DbObjectTestActor objectTest)
      {
         _objectTest = objectTest;
      }

      public abstract void SetUp();
      public abstract void TearDown();

      public sealed override DataSet Execute(params object[] parameters)
      {
         this.SetUp();
         var returnVal = _objectTest.Execute(parameters);
         this.TearDown();
         return returnVal;
      }
   }
}
