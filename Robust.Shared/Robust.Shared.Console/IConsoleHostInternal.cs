namespace Robust.Shared.Console;

internal interface IConsoleHostInternal : IConsoleHost
{
	bool IsCmdServer(IConsoleCommand cmd);
}
