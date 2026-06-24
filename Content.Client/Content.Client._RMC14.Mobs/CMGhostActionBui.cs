using System;
using Content.Client.Message;
using Content.Shared._RMC14.Mobs;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.ViewVariables;

namespace Content.Client._RMC14.Mobs;

public sealed class CMGhostActionBui : BoundUserInterface
{
	[ViewVariables]
	private CMGhostActionWindow? _window;

	public CMGhostActionBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		if (_window == null)
		{
			_window = BoundUserInterfaceExt.CreateWindow<CMGhostActionWindow>((BoundUserInterface)(object)this);
			_window.Text.SetMarkupPermissive(Loc.GetString("cm-ghost-window-text"));
			((BaseButton)_window.Stay).OnPressed += delegate
			{
				((BoundUserInterface)this).Close();
			};
			((BaseButton)_window.Ghost).OnPressed += delegate
			{
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new CMGhostActionBuiMsg());
			};
		}
	}
}
