using System;
using System.Collections.Generic;
using SQLCC.Core.Interfaces;

static public class ArgumentProvider
{
	public static string GetTraceName(IDictionary<string, string> arguments, string command, IDbProvider dbProvider)
	{
		// Get trace name from provided, last trace, or generate one.
		string traceName;
		arguments.TryGetValue("app.traceName", out traceName);
		if (traceName == null && command != "start")
		{
			traceName = dbProvider.GetLastTraceName().Substring(6);
		}
		else if (traceName == null && command == "start")
		{
			traceName = DateTime.Now.ToString("yyyyMMddHHmmss");
		}
		return traceName;
	}
}