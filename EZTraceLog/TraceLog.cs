using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace EZTraceLog
{
	public class TraceLog
	{
		/// <summary>
		///     LogFilePath
		/// </summary>
		private static string _logFilePath = "";
		private static string _apName = "";
		private static Mutex _mut;
		private static DateTime _errorDateTime = DateTime.Now.AddDays(-1);

		/// <summary>
		///		設定或取得錯誤發生時要寄送通知的方式
		/// </summary>
		public static IMsgSender ErrMsgSender { get; set; }

		/// <summary>
		///     設定 Log 檔名與路徑
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="apName"></param>
		public static void SetLogFile(string filePath, string apName)
		{
			try
			{
				_apName = apName;
				_mut = new Mutex(false, apName);
				_logFilePath = Path.Combine(filePath, apName);
			}
			catch (Exception ee)
			{
				var none = ee.Source;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="className">類別名稱</param>
		/// <param name="message">內文</param>
		[Conditional("DEBUG")]
		public static void DebugWrite(string className, string message)
		{
			// ***Release Mode 時不顯示
			// [DEBUG] 進出function: 以方便了解程式執行流程
			// [DEBUG] function parameters和return value
			// [DEBUG] 程式流程重點: 如重要判斷 
			var strError = DateTime.Now.ToString("HH:mm:ss.fff") + " [" + className.PadRight(10) + "][DEBUG] " + message;
			Console.WriteLine(strError);
			if (_mut == null) return;
			try
			{
				_mut.WaitOne(5 * 1000, true);

				// 設定 Track
				SetTrackFile();

				Debug.WriteLine(strError);

				// 清除 Track
				ClearTrack();
			}
			catch (Exception ee)
			{
				WriteEventLog(ee);
			}
			finally
			{
				try
				{
					_mut.ReleaseMutex();
				}
				catch
				{
				}
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="className"></param>
		/// <param name="message"></param>
		public static void InfoWrite(string className, string message)
		{
			// [INFO] IO access 前後: 包括FTP, HTTP, SMTP, socket, MSMQ, 讀寫檔, 下SQL指令...等各種io, 以紀錄access所花時間
			// [INFO]  IO access的資料: 如所下的sql指令, ftp/http/smtp/socket/msmq/讀寫檔...等, 從哪裡到哪裡(source & destination)
			// [INFO] IO access的結果: 如成功/失敗, SQL query 回幾筆
			WriteLog("INFO", className, message);
		}

		/// <summary>
		/// </summary>
		/// <param name="className"></param>
		/// <param name="message"></param>
		public static void WarnWrite(string className, string message)
		{
			// [WARN] 特殊事件, 但還是屬於正常執行範圍內
			WriteLog("WARN", className, message);
		}

		private static void WriteLog(string type, string className, string message)
		{
			var strError = DateTime.Now.ToString("HH:mm:ss.fff") + " [" + className.PadRight(10) + "][" + type.PadRight(5) +
							   "] " + message;
			Console.WriteLine(strError);

			if (_mut == null) return;
			_mut.WaitOne(5 * 1000, true);
			try
			{
				// 設定 Track
				SetTrackFile();
				Trace.WriteLine(strError);
				// 清除 Track
				ClearTrack();
			}
			catch (Exception ee)
			{
				WriteEventLog(ee);
			}
			finally
			{
				try
				{
					_mut.ReleaseMutex();
				}
				catch
				{
				}
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="className"></param>
		/// <param name="message"></param>
		public static void ErrorWrite(string className, string message)
		{
			// [ERROR] 系統執行異常狀況
			WriteLog("ERROR", className, message);
			if (ErrMsgSender != null)
				ErrMsgSender.Send(_apName, "[" + className + "]" + message);
		}

		public static void WriteEventLog(Exception ee)
		{
			try
			{
				if (_errorDateTime.AddMinutes(30) < DateTime.Now)
				{
					_errorDateTime = DateTime.Now;
					EventLog.WriteEntry("TraceLog", "來源應用程式:" + AppDomain.CurrentDomain.FriendlyName + " " + ee, EventLogEntryType.Error);
				}
			}
			catch
			{
			}
		}

		/// <summary>
		/// </summary>
		private static void SetTrackFile()
		{
			try
			{
				//**************************************************
				// 設定 Trace 增加 File 輸出
				//************************************************** 
				var path = _logFilePath + "." + DateTime.Now.ToString("yyyy-MM-dd") + ".log";

				var write = new StreamWriter(path, true, Encoding.GetEncoding("utf-8"));
				Trace.Listeners.Add(new TextWriterTraceListener(write, "log"));

				//**************************************************
				// 設定 Trace 增加 Console 輸出
				//************************************************** 
				//System.Diagnostics.Trace.Listeners.Add(
				//	new System.Diagnostics.TextWriterTraceListener(Console.Out,"console"));

				Trace.AutoFlush = true;
				Debug.AutoFlush = true;
			}
			catch (Exception ee)
			{
				var none = ee.Source;
				//Console.WriteLine(ee.ToString());
				//using(System.IO.StreamWriter write = new System.IO.StreamWriter(@"c:\commom.log",true,System.Text.Encoding.GetEncoding("utf-8")))
				//{
				//	write.WriteLine("SetTrackFile:" + ee.ToString());
				//	write.Close();
				//}
			}
		}

		/// <summary>
		/// </summary>
		public static void ClearTrack()
		{
			try
			{
				//**************************************************
				// 清除 Trace 
				//************************************************** 
				Trace.Listeners["log"].Flush();
				Trace.Listeners["log"].Close();
				Trace.Listeners.Remove("log");
			}
			catch (Exception ee)
			{
				var none = ee.Source;
			}
		}
	}
}