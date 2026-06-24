using Content.Shared.Database;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.Administration.UI.CustomControls;

public sealed class AdminLogTypeButton : Button
{
	public LogType Type { get; }

	public AdminLogTypeButton(LogType type)
	{
		Type = type;
		((Button)this).ClipText = true;
		((BaseButton)this).ToggleMode = true;
	}
}
