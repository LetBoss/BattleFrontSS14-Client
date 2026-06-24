using System;
using Content.Shared._RMC14.Xenonids.Construction.Tunnel;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client._RMC14.Xenonids.Construction.Tunnel;

public sealed class NameTunnelBui : BoundUserInterface
{
	[ViewVariables]
	private NameTunnelWindow? _window;

	public NameTunnelBui(EntityUid owner, Enum key)
		: base(owner, key)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		NameTunnelWindow window = _window;
		if (window != null && ((BaseWindow)window).IsOpen)
		{
			return;
		}
		_window = BoundUserInterfaceExt.CreateWindow<NameTunnelWindow>((BoundUserInterface)(object)this);
		LineEdit tunnelInput = _window.TunnelName;
		((BaseButton)_window.SubmitButton).OnPressed += delegate
		{
			string text = tunnelInput.Text.Trim();
			if (text.Length != 0)
			{
				((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new NameTunnelMessage(text));
			}
		};
	}
}
