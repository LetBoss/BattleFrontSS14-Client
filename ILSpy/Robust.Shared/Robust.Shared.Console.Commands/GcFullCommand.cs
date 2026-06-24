using System;
using System.Runtime;

namespace Robust.Shared.Console.Commands;

internal sealed class GcFullCommand : LocalizedCommands
{
	public override string Command => "gcf";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
		GC.Collect(2, GCCollectionMode.Forced, blocking: true, compacting: true);
	}
}
