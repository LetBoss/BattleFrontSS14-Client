using Serilog.Events;

namespace Robust.Shared.Log;

public interface ILogHandler
{
	void Log(string sawmillName, LogEvent message);
}
