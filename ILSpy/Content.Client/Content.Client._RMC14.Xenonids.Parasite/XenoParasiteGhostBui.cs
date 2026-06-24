using System;
using Content.Shared._RMC14.Xenonids.Egg;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client._RMC14.Xenonids.Parasite;

public sealed class XenoParasiteGhostBui : BoundUserInterface
{
	[ViewVariables]
	private XenoParasiteGhostWindow? _window;

	public XenoParasiteGhostBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<XenoParasiteGhostWindow>((BoundUserInterface)(object)this);
		((BaseButton)_window.DenyButton).OnPressed += delegate
		{
			((BaseWindow)_window).Close();
		};
		((BaseButton)_window.ConfirmButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new XenoParasiteGhostBuiMsg());
		};
	}
}
