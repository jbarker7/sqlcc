using System.Data;

namespace SQLCC.Core.Tests
{
   public abstract class DbObjectTestActor : IDbObjectTest
   {
      public abstract DataSet Execute(params object[] parameters);
   }
   
   public abstract class DbObjectTestActor<T> : IDbObjectTest<T>
   {
      public abstract T Execute(params object[] parameters);
   }
   
   public interface IDbObjectTest : IDbObjectTest<DataSet>
   {
   }

   public interface IDbObjectTest<T>
   {
      T Execute(params object[] parameters);
   }
}
