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
         var result = prereq.Execute("all");

         // Assert
         Assert.AreEqual(result.Count, 3);
      }

      [TestMethod]
      public void SpTestSprocActorShowOnlyWhereTestNamePassedIn()
      {
         // Arrange
         var prereq = new SpTestSprocArranger(_objectTest);

         // Act
         var result = prereq.Execute("Test1");

         // Assert
         Assert.AreEqual(result.Count, 1);
         Assert.AreEqual(result[0].Name, "Test1");
      }

   }
   
}
