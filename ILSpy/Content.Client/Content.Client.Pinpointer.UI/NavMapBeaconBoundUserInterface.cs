using System;
using Content.Shared.Pinpointer;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.ViewVariables;

namespace Content.Client.Pinpointer.UI;

public sealed class NavMapBeaconBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private NavMapBeaconWindow? _window;

	public NavMapBeaconBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<NavMapBeaconWindow>((BoundUserInterface)(object)this);
		NavMapBeaconComponent navMap = default(NavMapBeaconComponent);
		if (base.EntMan.TryGetComponent<NavMapBeaconComponent>(((BoundUserInterface)this).Owner, ref navMap))
		{
			_window.SetEntity(((BoundUserInterface)this).Owner, navMap);
		}
		_window.OnApplyButtonPressed += delegate(string? label, bool enabled, Color color)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new NavMapBeaconConfigureBuiMessage(label, enabled, color));
		};
	}
}
