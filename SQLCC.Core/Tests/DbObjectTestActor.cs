namespace SQLCC.Core.Tests
{
   public abstract class DbObjectTestActor : IDbObjectTestActor
   {
      public abstract object Execute(params object[] parameters);

      public T Execute<T>(params object[] parameters)
      {
         return (T)this.Execute(parameters);
      }
   }

   public interface IDbObjectTestActor
   {
      object Execute(params object[] parameters);
   }

}
