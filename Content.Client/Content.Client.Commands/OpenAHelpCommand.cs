using System;
using Content.Client.UserInterface.Systems.Bwoink;
using Content.Shared.Administration;
using Robust.Client.UserInterface;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Client.Commands;

[AnyCommand]
public sealed class OpenAHelpCommand : LocalizedCommands
{
	[Dependency]
	private IUserInterfaceManager _userInterfaceManager;

	public override string Command => "openahelp";

	public override string Help => base.LocalizationManager.GetString("cmd-" + ((LocalizedCommands)this).Command + "-help", (ValueTuple<string, object>)("command", ((LocalizedCommands)this).Command));

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		Guid result;
		if (args.Length >= 2)
		{
			shell.WriteLine(((LocalizedCommands)this).Help);
		}
		else if (args.Length == 0)
		{
			_userInterfaceManager.GetUIController<AHelpUIController>().Open();
		}
		else if (Guid.TryParse(args[0], out result))
		{
			NetUserId userId = default(NetUserId);
			((NetUserId)(ref userId))._002Ector(result);
			_userInterfaceManager.GetUIController<AHelpUIController>().Open(userId);
		}
		else
		{
			shell.WriteError(base.LocalizationManager.GetString("cmd-" + ((LocalizedCommands)this).Command + "-error"));
		}
	}
}
