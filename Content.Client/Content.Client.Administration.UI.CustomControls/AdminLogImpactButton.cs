using Content.Shared.Database;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.Administration.UI.CustomControls;

public sealed class AdminLogImpactButton : Button
{
	public LogImpact Impact { get; }

	public AdminLogImpactButton(LogImpact impact)
	{
		Impact = impact;
		((BaseButton)this).ToggleMode = true;
		((BaseButton)this).Pressed = true;
	}
}
