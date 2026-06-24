using System;

namespace Robust.Shared.Console.Commands;

internal sealed class MemCommand : LocalizedCommands
{
	public override string Command => "mem";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		long heapSizeBytes = GC.GetGCMemoryInfo().HeapSizeBytes;
		long totalMemory = GC.GetTotalMemory(forceFullCollection: false);
		shell.WriteLine(base.Loc.GetString("cmd-mem-report", ("heapSize", heapSizeBytes), ("totalAllocated", totalMemory)));
	}
}
