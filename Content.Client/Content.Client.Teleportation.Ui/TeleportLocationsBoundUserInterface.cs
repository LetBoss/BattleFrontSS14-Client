using System;
using Content.Shared.Teleportation;
using Content.Shared.Teleportation.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.ViewVariables;

namespace Content.Client.Teleportation.Ui;

public sealed class TeleportLocationsBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private TeleportMenu? _menu;

	public TeleportLocationsBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<TeleportMenu>((BoundUserInterface)(object)this);
		TeleportLocationsComponent teleportLocationsComponent = default(TeleportLocationsComponent);
		if (base.EntMan.TryGetComponent<TeleportLocationsComponent>(((BoundUserInterface)this).Owner, ref teleportLocationsComponent))
		{
			((DefaultWindow)_menu).Title = Loc.GetString(LocId.op_Implicit(teleportLocationsComponent.Name));
			_menu.Warps = teleportLocationsComponent.AvailableWarps;
			_menu.AddTeleportButtons();
			_menu.TeleportClicked += delegate(NetEntity netEnt, string pointName)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new TeleportLocationDestinationMessage(netEnt, pointName));
			};
		}
	}
}
