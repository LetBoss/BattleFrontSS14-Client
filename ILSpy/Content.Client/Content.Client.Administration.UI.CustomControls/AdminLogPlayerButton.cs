using System;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.Administration.UI.CustomControls;

public sealed class AdminLogPlayerButton : Button
{
	public Guid Id { get; }

	public AdminLogPlayerButton(Guid id)
	{
		Id = id;
		((Button)this).ClipText = true;
		((BaseButton)this).ToggleMode = true;
	}
}
