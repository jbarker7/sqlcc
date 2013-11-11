using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SQLCC.Core.Interfaces;
using SQLCC.Core.Objects;

namespace SQLCC.Tests
{
	[TestClass]
	public class TestArgumentProvider
	{
		[TestMethod]
		public void GetTraceName_GivenEmptyTraceNameArgAndStartCommand_ShouldReturnDateTimetracename()
		{
			var startTime = DateTime.Now.ToString("yyyyMMddHHmmss");
			
			var traceName = ArgumentProvider.GetTraceName(new Dictionary<string, string>(), "start", null);
			
			var endTime = DateTime.Now.ToString("yyyyMMddHHmmss");

			Assert.IsTrue(String.CompareOrdinal(startTime, traceName) <= 0);
			Assert.IsTrue(String.CompareOrdinal(endTime, traceName) >= 0);
		}

		[TestMethod]
		public void GetTraceName_GivenTraceNameArg_ShouldReturnTracename()
		{
			const string expectedTraceName = "TestTrace1";
			var arguments = new Dictionary<string, string> { { "app.traceName", expectedTraceName } };

			var traceName = ArgumentProvider.GetTraceName(arguments, "start", null);

			Assert.AreEqual(expectedTraceName, traceName);
		}

		[TestMethod]
		public void GetTraceName_GivenEmptyTraceNameArgAndNotStartCommand_WithSQLCCPrefix_ShouldStripPrefixFromLastTraceName()
		{
			const string lastTraceName = "SQLCC_TestTrace1";
			const string expectedTraceName = "TestTrace1";
			var dbProvider = Substitute.For<IDbProvider>();
			dbProvider.GetLastTraceName().Returns(lastTraceName);
			var arguments = new Dictionary<string, string>();

			var traceName = ArgumentProvider.GetTraceName(arguments, "notStart", dbProvider);

			Assert.AreEqual(expectedTraceName, traceName);
		}
	}
	
}
