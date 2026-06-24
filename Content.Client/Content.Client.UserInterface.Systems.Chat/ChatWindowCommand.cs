using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Console;

namespace Content.Client.UserInterface.Systems.Chat;

public sealed class ChatWindowCommand : LocalizedCommands
{
	public override string Command => "chatwindow";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		((BaseWindow)new ChatWindow()).OpenCentered();
	}
}
