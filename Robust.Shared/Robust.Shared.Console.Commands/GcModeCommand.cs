using System;
using System.Runtime;

namespace Robust.Shared.Console.Commands;

internal sealed class GcModeCommand : LocalizedCommands
{
	public override string Command => "gc_mode";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		GCLatencyMode latencyMode = GCSettings.LatencyMode;
		GCLatencyMode result;
		if (args.Length == 0)
		{
			shell.WriteLine(base.Loc.GetString("cmd-gc_mode-current", ("prevMode", latencyMode.ToString())));
			shell.WriteLine(base.Loc.GetString("cmd-gc_mode-possible"));
			GCLatencyMode[] values = Enum.GetValues<GCLatencyMode>();
			for (int i = 0; i < values.Length; i++)
			{
				GCLatencyMode gCLatencyMode = values[i];
				shell.WriteLine(base.Loc.GetString("cmd-gc_mode-option", ("mode", gCLatencyMode.ToString())));
			}
		}
		else if (!Enum.TryParse<GCLatencyMode>(args[0], ignoreCase: true, out result))
		{
			shell.WriteLine(base.Loc.GetString("cmd-gc_mode-unknown", ("arg", args[0])));
		}
		else
		{
			shell.WriteLine(base.Loc.GetString("cmd-gc_mode-attempt", ("prevMode", latencyMode.ToString()), ("mode", result.ToString())));
			GCSettings.LatencyMode = result;
			shell.WriteLine(base.Loc.GetString("cmd-gc_mode-result", ("mode", GCSettings.LatencyMode.ToString())));
		}
	}

	public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
	{
		if (args.Length == 1)
		{
			return CompletionResult.FromHintOptions(Enum.GetNames<GCLatencyMode>(), base.Loc.GetString("cmd-gc_mode-arg-type"));
		}
		return CompletionResult.Empty;
	}
}
