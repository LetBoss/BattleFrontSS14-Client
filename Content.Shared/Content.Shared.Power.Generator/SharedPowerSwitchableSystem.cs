using System;
using Content.Shared.Examine;
using Robust.Shared.GameObjects;

namespace Content.Shared.Power.Generator;

public abstract class SharedPowerSwitchableSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<PowerSwitchableComponent, ExaminedEvent>((ComponentEventHandler<PowerSwitchableComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
	}

	private void OnExamined(EntityUid uid, PowerSwitchableComponent comp, ExaminedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		string voltage = VoltageColor(GetVoltage(uid, comp));
		args.PushMarkup(base.Loc.GetString(comp.ExamineText, (ValueTuple<string, object>)("voltage", voltage)));
	}

	public string VoltageColor(SwitchableVoltage voltage)
	{
		return base.Loc.GetString("power-switchable-voltage", (ValueTuple<string, object>)("voltage", VoltageString(voltage)));
	}

	public string VoltageString(SwitchableVoltage voltage)
	{
		return voltage.ToString().ToUpper();
	}

	public int NextIndex(EntityUid uid, PowerSwitchableComponent? comp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PowerSwitchableComponent>(uid, ref comp, true))
		{
			return 0;
		}
		return (comp.ActiveIndex + 1) % comp.Cables.Count;
	}

	public SwitchableVoltage GetVoltage(EntityUid uid, PowerSwitchableComponent? comp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PowerSwitchableComponent>(uid, ref comp, true))
		{
			return SwitchableVoltage.HV;
		}
		return comp.Cables[comp.ActiveIndex].Voltage;
	}

	public SwitchableVoltage GetNextVoltage(EntityUid uid, PowerSwitchableComponent? comp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PowerSwitchableComponent>(uid, ref comp, true))
		{
			return SwitchableVoltage.HV;
		}
		return comp.Cables[NextIndex(uid, comp)].Voltage;
	}
}
