using System;
using Content.Shared.Singularity.Components;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.ParticleAccelerator.UI;

public sealed class ParticleAcceleratorBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private ParticleAcceleratorControlMenu? _menu;

	public ParticleAcceleratorBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<ParticleAcceleratorControlMenu>((BoundUserInterface)(object)this);
		_menu.SetEntity(((BoundUserInterface)this).Owner);
		_menu.OnOverallState += SendEnableMessage;
		_menu.OnPowerState += SendPowerStateMessage;
		_menu.OnScan += SendScanPartsMessage;
	}

	public void SendEnableMessage(bool enable)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ParticleAcceleratorSetEnableMessage(enable));
	}

	public void SendPowerStateMessage(ParticleAcceleratorPowerState state)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ParticleAcceleratorSetPowerStateMessage(state));
	}

	public void SendScanPartsMessage()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ParticleAcceleratorRescanPartsMessage());
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		_menu?.DataUpdate((ParticleAcceleratorUIState)(object)state);
	}
}
