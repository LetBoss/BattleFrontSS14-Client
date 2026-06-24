using System;
using Content.Shared.Administration.Logs;
using Content.Shared.Atmos.Piping.Unary.Components;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Power.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Atmos.Piping.Unary.Systems;

public abstract class SharedGasThermoMachineSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private SharedPowerReceiverSystem _receiver;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<GasThermoMachineComponent, ExaminedEvent>((ComponentEventHandler<GasThermoMachineComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GasThermoMachineComponent, GasThermomachineToggleMessage>((ComponentEventHandler<GasThermoMachineComponent, GasThermomachineToggleMessage>)OnToggleMessage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GasThermoMachineComponent, GasThermomachineChangeTemperatureMessage>((ComponentEventHandler<GasThermoMachineComponent, GasThermomachineChangeTemperatureMessage>)OnChangeTemperature, (Type[])null, (Type[])null);
	}

	private void OnExamined(EntityUid uid, GasThermoMachineComponent thermoMachine, ExaminedEvent args)
	{
		string str = default(string);
		if (base.Loc.TryGetString("gas-thermomachine-system-examined", ref str, new(string, object)[3]
		{
			("machineName", (!IsHeater(thermoMachine)) ? "freezer" : "heater"),
			("tempColor", (!IsHeater(thermoMachine)) ? "deepskyblue" : "red"),
			("temp", Math.Round(thermoMachine.TargetTemperature, 2))
		}))
		{
			args.PushMarkup(str);
		}
	}

	public bool IsHeater(GasThermoMachineComponent comp)
	{
		return comp.Cp >= 0f;
	}

	private void OnToggleMessage(EntityUid uid, GasThermoMachineComponent thermoMachine, GasThermomachineToggleMessage args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		bool powerState = _receiver.TogglePower(uid, playSwitchSound: true, null, ((BaseBoundUserInterfaceEvent)args).Actor);
		ISharedAdminLogManager adminLogger = _adminLogger;
		LogStringHandler handler = new LogStringHandler(9, 3);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "ToPrettyString(args.Actor)");
		handler.AppendLiteral(" turned ");
		handler.AppendFormatted(powerState ? "On" : "Off");
		handler.AppendLiteral(" ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "ToPrettyString(uid)");
		adminLogger.Add(LogType.AtmosPowerChanged, ref handler);
		DirtyUI(uid, thermoMachine);
	}

	private void OnChangeTemperature(EntityUid uid, GasThermoMachineComponent thermoMachine, GasThermomachineChangeTemperatureMessage args)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		if (IsHeater(thermoMachine))
		{
			thermoMachine.TargetTemperature = MathF.Min(args.Temperature, thermoMachine.MaxTemperature);
		}
		else
		{
			thermoMachine.TargetTemperature = MathF.Max(args.Temperature, thermoMachine.MinTemperature);
		}
		thermoMachine.TargetTemperature = MathF.Max(thermoMachine.TargetTemperature, 2.7f);
		ISharedAdminLogManager adminLogger = _adminLogger;
		LogStringHandler handler = new LogStringHandler(24, 3);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "ToPrettyString(args.Actor)");
		handler.AppendLiteral(" set temperature on ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "ToPrettyString(uid)");
		handler.AppendLiteral(" to ");
		handler.AppendFormatted(thermoMachine.TargetTemperature, "thermoMachine.TargetTemperature");
		adminLogger.Add(LogType.AtmosTemperatureChanged, ref handler);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)thermoMachine, (MetaDataComponent)null);
		DirtyUI(uid, thermoMachine);
	}

	protected virtual void DirtyUI(EntityUid uid, GasThermoMachineComponent? thermoMachine, UserInterfaceComponent? ui = null)
	{
	}
}
