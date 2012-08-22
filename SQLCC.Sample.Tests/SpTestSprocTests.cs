using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SQLCC.Sample.Tests
{
   [TestClass]
   public class SpTestSprocTests
   {
      private SpTestSprocActor _objectTest;

      [TestInitialize]
      public void SetUp()
      {
         _objectTest = new SpTestSprocActor();
      }

      [TestMethod]
      public void SpTestSprocActorShowAllWhenAllIsTestName()
      {
         // Arrange
         var prereq = new SpTestSprocArranger(_objectTest);

         // Act
         var result = prereq.Execute<List<SpTestSprocReturn>>("all");

         // Assert
         Assert.AreEqual(3, result.Count);
      }

      [TestMethod]
      public void SpTestSprocActorShowOnlyWhereTestNamePassedIn()
      {
         // Arrange
         var prereq = new SpTestSprocArranger(_objectTest);

         // Act
         var result = prereq.Execute<List<SpTestSprocReturn>>("Test1");

         // Assert
         Assert.AreEqual(1, result.Count);
         Assert.AreEqual("Test1", result[0].Name);
      }

      [TestMethod]
      public void SpTestSprocActorWhereIsFoo()
      {
         // Arrange
         var arranger = new SpTestSprocArranger(new SpFooTestSprocArranger(_objectTest));

         // Act
         var result = arranger.Execute<List<SpTestSprocReturn>>("foo");

         // Assert
         Assert.AreEqual(4, result.Count);
         Assert.AreEqual("Foo1", result[0].Name);
      }

      [TestMethod]
      public void SpTestSprocActorWhereStartsWithFoo()
      {
         // Arrange
         var arranger = new SpTestSprocArranger(new SpFooTestSprocArranger(_objectTest));

         // Act
         var result = arranger.Execute<List<SpTestSprocReturn>>("foofoo");

         // Assert
         Assert.AreEqual(1, result.Count);
         Assert.AreEqual("StaticVar", result[0].Name);
      }

   }
   
}
