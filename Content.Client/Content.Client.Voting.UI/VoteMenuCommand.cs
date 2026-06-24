using Content.Shared.Administration;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Console;

namespace Content.Client.Voting.UI;

[AnyCommand]
public sealed class VoteMenuCommand : LocalizedCommands
{
	public override string Command => "votemenu";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		((BaseWindow)new VoteCallMenu()).OpenCentered();
	}
}
