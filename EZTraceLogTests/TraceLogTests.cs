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
			EZTraceLog.TraceLog.InfoWrite("test", "test message");
		}
	}
}