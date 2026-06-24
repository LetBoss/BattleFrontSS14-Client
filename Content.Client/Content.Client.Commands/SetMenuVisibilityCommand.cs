using System;
using Content.Client.Verbs;
using Content.Shared.Verbs;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Commands;

internal sealed class SetMenuVisibilityCommand : LocalizedCommands
{
	[Dependency]
	private IEntitySystemManager _entitySystemManager;

	public override string Command => "menuvis";

	public override string Help => base.LocalizationManager.GetString("cmd-" + ((LocalizedCommands)this).Command + "-help", (ValueTuple<string, object>)("command", ((LocalizedCommands)this).Command));

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		if (TryParseArguments(shell, args, out var visibility))
		{
			_entitySystemManager.GetEntitySystem<VerbSystem>().Visibility = visibility;
		}
	}

	private bool TryParseArguments(IConsoleShell shell, string[] args, out MenuVisibility visibility)
	{
		visibility = MenuVisibility.Default;
		foreach (string text in args)
		{
			switch (text.ToLower())
			{
			case "nofov":
				visibility |= MenuVisibility.NoFov;
				break;
			case "incontainer":
				visibility |= MenuVisibility.InContainer;
				break;
			case "invisible":
				visibility |= MenuVisibility.Invisible;
				break;
			case "all":
				visibility |= MenuVisibility.All;
				break;
			default:
				shell.WriteError(base.LocalizationManager.GetString("cmd-" + ((LocalizedCommands)this).Command + "-error", (ValueTuple<string, object>)("arg", text)));
				return false;
			}
		}
		return true;
	}
}
