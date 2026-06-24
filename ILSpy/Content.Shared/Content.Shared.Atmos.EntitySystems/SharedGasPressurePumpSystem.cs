using System;
using Content.Shared.Administration.Logs;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.Piping;
using Content.Shared.Atmos.Piping.Binary.Components;
using Content.Shared.Atmos.Piping.Components;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Power;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Atmos.EntitySystems;

public abstract class SharedGasPressurePumpSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedPowerReceiverSystem _receiver;

	[Dependency]
	protected SharedUserInterfaceSystem UserInterfaceSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<GasPressurePumpComponent, ComponentInit>((EntityEventRefHandler<GasPressurePumpComponent, ComponentInit>)OnInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GasPressurePumpComponent, PowerChangedEvent>((EntityEventRefHandler<GasPressurePumpComponent, PowerChangedEvent>)OnPowerChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GasPressurePumpComponent, GasPressurePumpChangeOutputPressureMessage>((EntityEventRefHandler<GasPressurePumpComponent, GasPressurePumpChangeOutputPressureMessage>)OnOutputPressureChangeMessage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GasPressurePumpComponent, GasPressurePumpToggleStatusMessage>((EntityEventRefHandler<GasPressurePumpComponent, GasPressurePumpToggleStatusMessage>)OnToggleStatusMessage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GasPressurePumpComponent, AtmosDeviceDisabledEvent>((EntityEventRefHandler<GasPressurePumpComponent, AtmosDeviceDisabledEvent>)OnPumpLeaveAtmosphere, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GasPressurePumpComponent, ExaminedEvent>((EntityEventRefHandler<GasPressurePumpComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
	}

	private void OnExamined(Entity<GasPressurePumpComponent> ent, ref ExaminedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		string str = default(string);
		if (((EntitySystem)this).Transform(Entity<GasPressurePumpComponent>.op_Implicit(ent)).Anchored && base.Loc.TryGetString("gas-pressure-pump-system-examined", ref str, (ValueTuple<string, object>)("statusColor", "lightblue"), (ValueTuple<string, object>)("pressure", ent.Comp.TargetPressure)))
		{
			args.PushMarkup(str);
		}
	}

	private void OnInit(Entity<GasPressurePumpComponent> ent, ref ComponentInit args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		UpdateAppearance(Entity<GasPressurePumpComponent, AppearanceComponent>.op_Implicit(ent));
	}

	private void OnPowerChanged(Entity<GasPressurePumpComponent> ent, ref PowerChangedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		UpdateAppearance(Entity<GasPressurePumpComponent, AppearanceComponent>.op_Implicit(ent));
	}

	private void UpdateAppearance(Entity<GasPressurePumpComponent, AppearanceComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<AppearanceComponent>(Entity<GasPressurePumpComponent, AppearanceComponent>.op_Implicit(ent), ref ent.Comp2, false))
		{
			bool pumpOn = ent.Comp1.Enabled && _receiver.IsPowered(Entity<SharedApcPowerReceiverComponent>.op_Implicit(ent.Owner));
			_appearance.SetData(Entity<GasPressurePumpComponent, AppearanceComponent>.op_Implicit(ent), (Enum)PumpVisuals.Enabled, (object)pumpOn, ent.Comp2);
		}
	}

	private void OnToggleStatusMessage(Entity<GasPressurePumpComponent> ent, ref GasPressurePumpToggleStatusMessage args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Enabled = args.Enabled;
		ISharedAdminLogManager adminLogger = _adminLogger;
		LogStringHandler handler = new LogStringHandler(22, 3);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "player", "ToPrettyString(args.Actor)");
		handler.AppendLiteral(" set the power on ");
		handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<GasPressurePumpComponent>.op_Implicit(ent), (MetaDataComponent)null), "device", "ToPrettyString(ent)");
		handler.AppendLiteral(" to ");
		handler.AppendFormatted(args.Enabled, "args.Enabled");
		adminLogger.Add(LogType.AtmosPowerChanged, LogImpact.Medium, ref handler);
		((EntitySystem)this).Dirty<GasPressurePumpComponent>(ent, (MetaDataComponent)null);
		UpdateAppearance(Entity<GasPressurePumpComponent, AppearanceComponent>.op_Implicit(ent));
		UpdateUi(ent);
	}

	private void OnOutputPressureChangeMessage(Entity<GasPressurePumpComponent> ent, ref GasPressurePumpChangeOutputPressureMessage args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.TargetPressure = Math.Clamp(args.Pressure, 0f, 4500f);
		ISharedAdminLogManager adminLogger = _adminLogger;
		LogStringHandler handler = new LogStringHandler(28, 3);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "player", "ToPrettyString(args.Actor)");
		handler.AppendLiteral(" set the pressure on ");
		handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<GasPressurePumpComponent>.op_Implicit(ent), (MetaDataComponent)null), "device", "ToPrettyString(ent)");
		handler.AppendLiteral(" to ");
		handler.AppendFormatted(args.Pressure, "args.Pressure");
		handler.AppendLiteral("kPa");
		adminLogger.Add(LogType.AtmosPressureChanged, LogImpact.Medium, ref handler);
		((EntitySystem)this).Dirty<GasPressurePumpComponent>(ent, (MetaDataComponent)null);
		UpdateUi(ent);
	}

	private void OnPumpLeaveAtmosphere(Entity<GasPressurePumpComponent> ent, ref AtmosDeviceDisabledEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Enabled = false;
		((EntitySystem)this).Dirty<GasPressurePumpComponent>(ent, (MetaDataComponent)null);
		UpdateAppearance(Entity<GasPressurePumpComponent, AppearanceComponent>.op_Implicit(ent));
		UserInterfaceSystem.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)GasPressurePumpUiKey.Key);
	}

	protected virtual void UpdateUi(Entity<GasPressurePumpComponent> ent)
	{
	}
}
