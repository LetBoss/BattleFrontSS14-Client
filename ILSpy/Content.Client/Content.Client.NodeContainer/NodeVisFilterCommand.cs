using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Client.NodeContainer;

public sealed class NodeVisFilterCommand : LocalizedEntityCommands
{
	[Dependency]
	private NodeGroupSystem _nodeSystem;

	public override string Command => "nodevisfilter";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		if (args.Length == 0)
		{
			foreach (string item2 in _nodeSystem.Filtered)
			{
				shell.WriteLine(item2);
			}
			return;
		}
		string item = args[0];
		if (!_nodeSystem.Filtered.Add(item))
		{
			_nodeSystem.Filtered.Remove(item);
		}
	}
}
