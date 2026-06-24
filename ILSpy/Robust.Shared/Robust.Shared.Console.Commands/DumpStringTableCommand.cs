using System.Collections.Generic;
using System.Linq;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Robust.Shared.Console.Commands;

internal sealed class DumpStringTableCommand : IConsoleCommand
{
	[Dependency]
	private readonly INetManager _netManager;

	public string Command => "net_dumpstringtable";

	public string Description => "";

	public string Help => "";

	public void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		foreach (var (value, value2) in ((NetManager)_netManager).StringTable.Strings.OrderBy<KeyValuePair<int, string>, int>((KeyValuePair<int, string> x) => x.Key))
		{
			shell.WriteLine($"{value}: {value2}");
		}
	}
}
