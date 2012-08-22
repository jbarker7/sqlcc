using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SQLCC.Sample.Tests
{
   [TestClass]
   public class SpFooTestSprocTests
   {
      private SpFooTestSprocActor _objectTest;

      [TestInitialize]
      public void SetUp()
      {
         _objectTest = new SpFooTestSprocActor();
      }

      [TestMethod]
      public void SpTestSprocActorWhereFooTableIsEmpty()
      {
         var result = _objectTest.Execute<List<SpFooTestSprocReturn>>("foo", "foo");

         Assert.AreEqual(0, result.Count);
      }

   }
}
