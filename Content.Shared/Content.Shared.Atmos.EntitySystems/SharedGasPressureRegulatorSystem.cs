using System;
using Content.Shared.Administration.Logs;
using Content.Shared.Atmos.Piping.Binary.Components;
using Content.Shared.Database;
using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Atmos.EntitySystems;

public abstract class SharedGasPressureRegulatorSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	protected SharedUserInterfaceSystem UserInterfaceSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<GasPressureRegulatorComponent, ExaminedEvent>((EntityEventRefHandler<GasPressureRegulatorComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GasPressureRegulatorComponent, GasPressureRegulatorChangeThresholdMessage>((EntityEventRefHandler<GasPressureRegulatorComponent, GasPressureRegulatorChangeThresholdMessage>)OnThresholdChangeMessage, (Type[])null, (Type[])null);
	}

	private void OnExamined(Entity<GasPressureRegulatorComponent> ent, ref ExaminedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Transform(Entity<GasPressureRegulatorComponent>.op_Implicit(ent)).Anchored || !args.IsInDetailsRange)
		{
			return;
		}
		using (args.PushGroup("GasPressureRegulatorComponent"))
		{
			args.PushMarkup(base.Loc.GetString("gas-pressure-regulator-system-examined", (ValueTuple<string, object>)("statusColor", ent.Comp.Enabled ? "green" : "red"), (ValueTuple<string, object>)("open", ent.Comp.Enabled)));
			args.PushMarkup(base.Loc.GetString("gas-pressure-regulator-examined-threshold-pressure", (ValueTuple<string, object>)("threshold", $"{ent.Comp.Threshold:0.#}")));
			args.PushMarkup(base.Loc.GetString("gas-pressure-regulator-examined-flow-rate", (ValueTuple<string, object>)("flowRate", $"{ent.Comp.FlowRate:0.#}")));
		}
	}

	private void OnThresholdChangeMessage(Entity<GasPressureRegulatorComponent> ent, ref GasPressureRegulatorChangeThresholdMessage args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Threshold = Math.Max(0f, args.ThresholdPressure);
		ISharedAdminLogManager adminLogger = _adminLogger;
		LogStringHandler handler = new LogStringHandler(35, 3);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "player", "ToPrettyString(args.Actor)");
		handler.AppendLiteral(" set the pressure threshold on ");
		handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<GasPressureRegulatorComponent>.op_Implicit(ent), (MetaDataComponent)null), "device", "ToPrettyString(ent)");
		handler.AppendLiteral(" to ");
		handler.AppendFormatted(ent.Comp.Threshold, "ent.Comp.Threshold");
		adminLogger.Add(LogType.AtmosVolumeChanged, LogImpact.Medium, ref handler);
		((EntitySystem)this).Dirty<GasPressureRegulatorComponent>(ent, (MetaDataComponent)null);
		UpdateUi(ent);
	}

	protected virtual void UpdateUi(Entity<GasPressureRegulatorComponent> ent)
	{
	}
}
