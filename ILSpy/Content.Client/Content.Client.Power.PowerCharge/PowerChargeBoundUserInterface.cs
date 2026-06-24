using System;
using Content.Shared.Power;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.ViewVariables;

namespace Content.Client.Power.PowerCharge;

public sealed class PowerChargeBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private PowerChargeWindow? _window;

	public PowerChargeBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	public void SetPowerSwitch(bool on)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new SwitchChargingMachineMessage(on));
	}

	protected override void Open()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		PowerChargeComponent powerChargeComponent = default(PowerChargeComponent);
		if (base.EntMan.TryGetComponent<PowerChargeComponent>(((BoundUserInterface)this).Owner, ref powerChargeComponent))
		{
			_window = BoundUserInterfaceExt.CreateWindow<PowerChargeWindow>((BoundUserInterface)(object)this);
			_window.UpdateWindow(this, Loc.GetString(LocId.op_Implicit(powerChargeComponent.WindowTitle)));
		}
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is PowerChargeState state2)
		{
			_window?.UpdateState(state2);
		}
	}
}
