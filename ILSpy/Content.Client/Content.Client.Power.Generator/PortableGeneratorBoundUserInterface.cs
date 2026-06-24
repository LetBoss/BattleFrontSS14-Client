using System;
using Content.Shared.Power.Generator;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;

namespace Content.Client.Power.Generator;

public sealed class PortableGeneratorBoundUserInterface : BoundUserInterface
{
	private GeneratorWindow? _window;

	public PortableGeneratorBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindowCenteredLeft<GeneratorWindow>((BoundUserInterface)(object)this);
		_window.SetEntity(((BoundUserInterface)this).Owner);
		_window.OnState += delegate(bool args)
		{
			if (args)
			{
				Start();
			}
			else
			{
				Stop();
			}
		};
		_window.OnPower += SetTargetPower;
		_window.OnEjectFuel += EjectFuel;
		_window.OnSwitchOutput += SwitchOutput;
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		if (state is PortableGeneratorComponentBuiState state2)
		{
			_window?.Update(state2);
		}
	}

	public void SetTargetPower(int target)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new PortableGeneratorSetTargetPowerMessage(target));
	}

	public void Start()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new PortableGeneratorStartMessage());
	}

	public void Stop()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new PortableGeneratorStopMessage());
	}

	public void SwitchOutput()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new PortableGeneratorSwitchOutputMessage());
	}

	public void EjectFuel()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new PortableGeneratorEjectFuelMessage());
	}
}
