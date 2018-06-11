namespace EZTraceLog
{
	public interface IMsgSender
	{
		bool Send(string title, string message);
	}
}