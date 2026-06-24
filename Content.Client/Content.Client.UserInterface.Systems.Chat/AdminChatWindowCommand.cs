using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Console;

namespace Content.Client.UserInterface.Systems.Chat;

public sealed class AdminChatWindowCommand : LocalizedCommands
{
	public override string Command => "achatwindow";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		ChatWindow chatWindow = new ChatWindow();
		chatWindow.ConfigureForAdminChat();
		((BaseWindow)chatWindow).OpenCentered();
	}
}
