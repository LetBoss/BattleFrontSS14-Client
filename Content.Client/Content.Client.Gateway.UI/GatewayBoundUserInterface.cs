using System;
using Content.Shared.Gateway;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;

namespace Content.Client.Gateway.UI;

public sealed class GatewayBoundUserInterface : BoundUserInterface
{
	private GatewayWindow? _window;

	public GatewayBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<GatewayWindow>((BoundUserInterface)(object)this);
		_window.SetEntity(base.EntMan.GetNetEntity(((BoundUserInterface)this).Owner, (MetaDataComponent)null));
		_window.OpenPortal += delegate(NetEntity destination)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new GatewayOpenPortalMessage(destination));
		};
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is GatewayBoundUserInterfaceState state2)
		{
			_window?.UpdateState(state2);
		}
	}
}
