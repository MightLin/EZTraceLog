using Microsoft.VisualStudio.TestTools.UnitTesting;
using EZTraceLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZTraceLog.Tests
{
	[TestClass()]
	public class TraceLogTests
	{
		[TestMethod()]
		public void InfoWriteTest()
		{

			TraceLog.InfoWrite("main", "Start Process");

			TraceLog.DebugWrite("methodName", "This is debug log");

			TraceLog.WarnWrite("className", "This is warn log");

			TraceLog.ErrorWrite("canTypeAnything", "This is error log");

			TraceLog.InfoWrite("main", "End Process");
		}
	}
}