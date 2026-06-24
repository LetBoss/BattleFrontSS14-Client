using System;
using Content.Shared.Administration.Logs;
using Content.Shared.Atmos.Piping.Binary.Components;
using Content.Shared.Atmos.Visuals;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Power;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Atmos.Piping.Binary.Systems;

public abstract class SharedGasVolumePumpSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedPowerReceiverSystem _receiver;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<GasVolumePumpComponent, ComponentInit>((EntityEventRefHandler<GasVolumePumpComponent, ComponentInit>)OnInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GasVolumePumpComponent, PowerChangedEvent>((EntityEventRefHandler<GasVolumePumpComponent, PowerChangedEvent>)OnPowerChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GasVolumePumpComponent, ExaminedEvent>((ComponentEventHandler<GasVolumePumpComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GasVolumePumpComponent, GasVolumePumpToggleStatusMessage>((ComponentEventHandler<GasVolumePumpComponent, GasVolumePumpToggleStatusMessage>)OnToggleStatusMessage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GasVolumePumpComponent, GasVolumePumpChangeTransferRateMessage>((ComponentEventHandler<GasVolumePumpComponent, GasVolumePumpChangeTransferRateMessage>)OnTransferRateChangeMessage, (Type[])null, (Type[])null);
	}

	private void OnInit(Entity<GasVolumePumpComponent> ent, ref ComponentInit args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		UpdateAppearance(ent.Owner, ent.Comp);
	}

	private void OnPowerChanged(Entity<GasVolumePumpComponent> ent, ref PowerChangedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		UpdateAppearance(ent.Owner, ent.Comp);
	}

	protected virtual void UpdateUi(Entity<GasVolumePumpComponent> entity)
	{
	}

	private void OnToggleStatusMessage(EntityUid uid, GasVolumePumpComponent pump, GasVolumePumpToggleStatusMessage args)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		pump.Enabled = args.Enabled;
		ISharedAdminLogManager adminLogger = _adminLogger;
		LogStringHandler handler = new LogStringHandler(22, 3);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "player", "ToPrettyString(args.Actor)");
		handler.AppendLiteral(" set the power on ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "device", "ToPrettyString(uid)");
		handler.AppendLiteral(" to ");
		handler.AppendFormatted(args.Enabled, "args.Enabled");
		adminLogger.Add(LogType.AtmosPowerChanged, LogImpact.Medium, ref handler);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)pump, (MetaDataComponent)null);
		UpdateUi(Entity<GasVolumePumpComponent>.op_Implicit((uid, pump)));
		UpdateAppearance(uid, pump);
	}

	private void OnTransferRateChangeMessage(EntityUid uid, GasVolumePumpComponent pump, GasVolumePumpChangeTransferRateMessage args)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		pump.TransferRate = Math.Clamp(args.TransferRate, 0f, pump.MaxTransferRate);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)pump, (MetaDataComponent)null);
		UpdateUi(Entity<GasVolumePumpComponent>.op_Implicit((uid, pump)));
		ISharedAdminLogManager adminLogger = _adminLogger;
		LogStringHandler handler = new LogStringHandler(30, 3);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "player", "ToPrettyString(args.Actor)");
		handler.AppendLiteral(" set the transfer rate on ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "device", "ToPrettyString(uid)");
		handler.AppendLiteral(" to ");
		handler.AppendFormatted(args.TransferRate, "args.TransferRate");
		adminLogger.Add(LogType.AtmosVolumeChanged, LogImpact.Medium, ref handler);
	}

	private void OnExamined(EntityUid uid, GasVolumePumpComponent pump, ExaminedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		string str = default(string);
		if (((EntitySystem)this).Transform(uid).Anchored && base.Loc.TryGetString("gas-volume-pump-system-examined", ref str, (ValueTuple<string, object>)("statusColor", "lightblue"), (ValueTuple<string, object>)("rate", pump.TransferRate)))
		{
			args.PushMarkup(str);
		}
	}

	protected void UpdateAppearance(EntityUid uid, GasVolumePumpComponent? pump = null, AppearanceComponent? appearance = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<GasVolumePumpComponent, AppearanceComponent>(uid, ref pump, ref appearance, false))
		{
			if (!pump.Enabled || !_receiver.IsPowered(Entity<SharedApcPowerReceiverComponent>.op_Implicit(uid)))
			{
				_appearance.SetData(uid, (Enum)GasVolumePumpVisuals.State, (object)GasVolumePumpState.Off, appearance);
			}
			else if (pump.Blocked)
			{
				_appearance.SetData(uid, (Enum)GasVolumePumpVisuals.State, (object)GasVolumePumpState.Blocked, appearance);
			}
			else
			{
				_appearance.SetData(uid, (Enum)GasVolumePumpVisuals.State, (object)GasVolumePumpState.On, appearance);
			}
		}
	}
}
