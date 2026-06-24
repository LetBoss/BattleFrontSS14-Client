using System;
using System.Collections.Generic;
using System.Linq;
using Content.Client.NPC;
using Content.Shared.NPC;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Commands;

public sealed class DebugPathfindingCommand : LocalizedCommands
{
	[Dependency]
	private IEntitySystemManager _entitySystemManager;

	public override string Command => "pathfinder";

	public override string Help => base.LocalizationManager.GetString("cmd-" + ((LocalizedCommands)this).Command + "-help", (ValueTuple<string, object>)("command", ((LocalizedCommands)this).Command));

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		PathfindingSystem entitySystem = _entitySystemManager.GetEntitySystem<PathfindingSystem>();
		if (args.Length == 0)
		{
			entitySystem.Modes = PathfindingDebugMode.None;
			return;
		}
		foreach (string text in args)
		{
			if (!Enum.TryParse<PathfindingDebugMode>(text, out var result))
			{
				shell.WriteError(base.LocalizationManager.GetString("cmd-" + ((LocalizedCommands)this).Command + "-error", (ValueTuple<string, object>)("arg", text)));
				continue;
			}
			entitySystem.Modes ^= result;
			shell.WriteLine(base.LocalizationManager.GetString("cmd-" + ((LocalizedCommands)this).Command + "-notify", (ValueTuple<string, object>)("arg", text), (ValueTuple<string, object>)("newMode", (entitySystem.Modes & result) != 0)));
		}
	}

	public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (args.Length > 1)
		{
			return CompletionResult.Empty;
		}
		List<PathfindingDebugMode> list = Enum.GetValues<PathfindingDebugMode>().ToList();
		List<CompletionOption> list2 = new List<CompletionOption>();
		foreach (PathfindingDebugMode item in list)
		{
			if (item != PathfindingDebugMode.None)
			{
				list2.Add(new CompletionOption(item.ToString(), (string)null, (CompletionOptionFlags)0));
			}
		}
		return CompletionResult.FromOptions((IEnumerable<CompletionOption>)list2);
	}
}
