using System;
using Content.Client.UserInterface;
using Content.Shared.Power;
using Robust.Client.Timing;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.ViewVariables;

namespace Content.Client.Power.Battery;

public sealed class BatteryBoundUserInterface : BoundUserInterface, IBuiPreTickUpdate
{
	[Dependency]
	private IClientGameTiming _gameTiming;

	[ViewVariables]
	private BatteryMenu? _menu;

	private BuiPredictionState? _pred;

	private InputCoalescer<float> _chargeRateCoalescer;

	private InputCoalescer<float> _dischargeRateCoalescer;

	public BatteryBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<BatteryBoundUserInterface>(this);
	}

	protected override void Open()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_pred = new BuiPredictionState((BoundUserInterface)(object)this, _gameTiming);
		_menu = BoundUserInterfaceExt.CreateWindow<BatteryMenu>((BoundUserInterface)(object)this);
		_menu.SetEntity(((BoundUserInterface)this).Owner);
		_menu.OnInBreaker += delegate(bool val)
		{
			_pred.SendMessage((BoundUserInterfaceMessage)(object)new BatterySetInputBreakerMessage(val));
		};
		_menu.OnOutBreaker += delegate(bool val)
		{
			_pred.SendMessage((BoundUserInterfaceMessage)(object)new BatterySetOutputBreakerMessage(val));
		};
		_menu.OnChargeRate += delegate(float val)
		{
			_chargeRateCoalescer.Set(val);
		};
		_menu.OnDischargeRate += delegate(float val)
		{
			_dischargeRateCoalescer.Set(val);
		};
	}

	void IBuiPreTickUpdate.PreTickUpdate()
	{
		if (_chargeRateCoalescer.CheckIsModified(out var value))
		{
			_pred.SendMessage((BoundUserInterfaceMessage)(object)new BatterySetChargeRateMessage(value));
		}
		if (_dischargeRateCoalescer.CheckIsModified(out var value2))
		{
			_pred.SendMessage((BoundUserInterfaceMessage)(object)new BatterySetDischargeRateMessage(value2));
		}
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		if (!(state is BatteryBuiState batteryBuiState))
		{
			return;
		}
		foreach (BoundUserInterfaceMessage item in _pred.MessagesToReplay())
		{
			if (!(item is BatterySetInputBreakerMessage batterySetInputBreakerMessage))
			{
				if (!(item is BatterySetOutputBreakerMessage batterySetOutputBreakerMessage))
				{
					if (!(item is BatterySetChargeRateMessage batterySetChargeRateMessage))
					{
						if (item is BatterySetDischargeRateMessage batterySetDischargeRateMessage)
						{
							batteryBuiState.MaxSupply = batterySetDischargeRateMessage.Rate;
						}
					}
					else
					{
						batteryBuiState.MaxChargeRate = batterySetChargeRateMessage.Rate;
					}
				}
				else
				{
					batteryBuiState.CanDischarge = batterySetOutputBreakerMessage.On;
				}
			}
			else
			{
				batteryBuiState.CanCharge = batterySetInputBreakerMessage.On;
			}
		}
		_menu?.Update(batteryBuiState);
	}
}
