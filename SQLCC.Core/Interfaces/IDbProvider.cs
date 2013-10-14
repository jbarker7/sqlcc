using System.Collections.Generic;
using SQLCC.Core.Objects;

namespace SQLCC.Core.Interfaces
{
	public interface IDbProvider : IExtension
	{
		void StartTrace(string traceName);

		void StopTrace(string traceName);

		string GetLastTraceName();

		bool IsTraceRunning(string traceName);

		List<DbCodeSegment> GetTraceCodeSegments(string traceName);

		List<DbObject> GetAllObjects();
	}
}
