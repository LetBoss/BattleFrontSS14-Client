using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Dialog;
using Content.Shared._RMC14.Tracker.SquadLeader;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Watch;
using Content.Shared.Alert;
using Content.Shared.Mobs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Tracker.Xeno;

public sealed class HiveTrackerSystem : EntitySystem
{
	[Dependency]
	private AlertsSystem _alerts;

	[Dependency]
	private DialogSystem _dialog;

	[Dependency]
	private IComponentFactory _factory;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private TrackerSystem _tracker;

	[Dependency]
	private SquadLeaderTrackerSystem _squadLeaderTrackerSystem;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedXenoWatchSystem _xenoWatch;

	private const string HiveTrackerCategory = "HiveTracker";

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<HiveTrackerComponent, ComponentRemove>((EntityEventRefHandler<HiveTrackerComponent, ComponentRemove>)OnRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HiveTrackerComponent, HiveTrackerClickedAlertEvent>((EntityEventRefHandler<HiveTrackerComponent, HiveTrackerClickedAlertEvent>)OnClickedAlert, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HiveTrackerComponent, HiveTrackerAltClickedAlertEvent>((EntityEventRefHandler<HiveTrackerComponent, HiveTrackerAltClickedAlertEvent>)OnAltClickedAlert, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HiveTrackerComponent, HiveTrackerChangeModeEvent>((EntityEventRefHandler<HiveTrackerComponent, HiveTrackerChangeModeEvent>)OnHiveTrackerChangeMode, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HiveTrackerComponent, LeaderTrackerSelectTargetEvent>((EntityEventRefHandler<HiveTrackerComponent, LeaderTrackerSelectTargetEvent>)OnHiveTrackerSelectTarget, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCTrackableComponent, RequestTrackableNameEvent>((EntityEventRefHandler<RMCTrackableComponent, RequestTrackableNameEvent>)OnRequestTrackableName, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCTrackableComponent, MobStateChangedEvent>((EntityEventRefHandler<RMCTrackableComponent, MobStateChangedEvent>)OnMobStateChanged, (Type[])null, (Type[])null);
	}

	private void OnRemove(Entity<HiveTrackerComponent> ent, ref ComponentRemove args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		TrackerModePrototype trackerMode = default(TrackerModePrototype);
		_prototypeManager.TryIndex<TrackerModePrototype>(ent.Comp.Mode, ref trackerMode);
		if (trackerMode != null)
		{
			_alerts.ClearAlert(Entity<HiveTrackerComponent>.op_Implicit(ent), trackerMode.Alert);
		}
	}

	private void OnClickedAlert(Entity<HiveTrackerComponent> ent, ref HiveTrackerClickedAlertEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		Entity<HiveComponent>? hive = _hive.GetHive(Entity<HiveMemberComponent>.op_Implicit(ent.Owner));
		if (!hive.HasValue)
		{
			return;
		}
		Entity<HiveComponent> hive2 = hive.GetValueOrDefault();
		EntityUid? target = null;
		HiveMemberComponent targetHive = default(HiveMemberComponent);
		if (((EntitySystem)this).TryComp<HiveMemberComponent>(ent.Comp.Target, ref targetHive))
		{
			EntityUid? hive3 = targetHive.Hive;
			EntityUid owner = hive2.Owner;
			if (hive3.HasValue && hive3.GetValueOrDefault() == owner)
			{
				target = ent.Comp.Target.Value;
			}
		}
		if (!((EntitySystem)this).HasComp<XenoComponent>(target))
		{
			target = hive2.Comp.CurrentQueen;
		}
		if (target.HasValue)
		{
			((HandledEntityEventArgs)args).Handled = true;
			_xenoWatch.Watch(Entity<HiveMemberComponent, ActorComponent, EyeComponent>.op_Implicit(ent.Owner), Entity<HiveMemberComponent>.op_Implicit(target.Value));
		}
	}

	private void OnAltClickedAlert(Entity<HiveTrackerComponent> ent, ref HiveTrackerAltClickedAlertEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		List<DialogOption> options = new List<DialogOption>();
		foreach (ProtoId<TrackerModePrototype> mode in ent.Comp.TrackerModes)
		{
			options.Add(new DialogOption(base.Loc.GetString("rmc-xeno-tracker-target-" + ProtoId<TrackerModePrototype>.op_Implicit(mode)), new HiveTrackerChangeModeEvent(mode)));
		}
		_dialog.OpenOptions(Entity<HiveTrackerComponent>.op_Implicit(ent), base.Loc.GetString("rmc-squad-info-tracking-selection"), options, base.Loc.GetString("rmc-squad-info-tracking-choose"));
	}

	private void OnHiveTrackerChangeMode(Entity<HiveTrackerComponent> ent, ref HiveTrackerChangeModeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		HiveMemberComponent member = default(HiveMemberComponent);
		if (!_timing.IsFirstTimePredicted || !((EntitySystem)this).TryComp<HiveMemberComponent>(ent.Owner, ref member))
		{
			return;
		}
		_squadLeaderTrackerSystem.TryFindTargets(args.Mode, out List<DialogOption> options, out List<EntityUid> trackingOptions);
		int index = 0;
		HiveMemberComponent targetHiveMember = default(HiveMemberComponent);
		while (index < trackingOptions.Count)
		{
			if (((EntitySystem)this).TryComp<HiveMemberComponent>(trackingOptions[index], ref targetHiveMember))
			{
				EntityUid? hive = targetHiveMember.Hive;
				EntityUid? hive2 = member.Hive;
				if (hive.HasValue == hive2.HasValue && (!hive.HasValue || !(hive.GetValueOrDefault() != hive2.GetValueOrDefault())))
				{
					index++;
					continue;
				}
			}
			options.RemoveAt(index);
			trackingOptions.RemoveAt(index);
		}
		_dialog.OpenOptions(Entity<HiveTrackerComponent>.op_Implicit(ent), base.Loc.GetString("rmc-squad-info-tracking-selection"), options, base.Loc.GetString("rmc-squad-info-tracking-choose"));
	}

	private void OnHiveTrackerSelectTarget(Entity<HiveTrackerComponent> ent, ref LeaderTrackerSelectTargetEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		SetTarget(ent, ((EntitySystem)this).GetEntity(args.Target));
		SetMode(ent, args.Mode);
		((EntitySystem)this).Dirty<HiveTrackerComponent>(ent, (MetaDataComponent)null);
	}

	private void SetTarget(Entity<HiveTrackerComponent> ent, EntityUid? target)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Target = target;
		((EntitySystem)this).Dirty<HiveTrackerComponent>(ent, (MetaDataComponent)null);
	}

	private void SetMode(Entity<HiveTrackerComponent> ent, ProtoId<TrackerModePrototype> mode)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Mode = mode;
		((EntitySystem)this).Dirty<HiveTrackerComponent>(ent, (MetaDataComponent)null);
	}

	private void UpdateDirection(Entity<HiveTrackerComponent> ent, MapCoordinates? coordinates = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		_alerts.ClearAlertCategory(Entity<HiveTrackerComponent>.op_Implicit(ent), ProtoId<AlertCategoryPrototype>.op_Implicit("HiveTracker"));
		TrackerModePrototype trackerMode = default(TrackerModePrototype);
		_prototypeManager.TryIndex<TrackerModePrototype>(ent.Comp.Mode, ref trackerMode);
		if (trackerMode != null)
		{
			ProtoId<AlertPrototype> alert = trackerMode.Alert;
			short severity = TrackerSystem.CenterSeverity;
			if (coordinates.HasValue)
			{
				severity = _tracker.GetAlertSeverity(ent.Owner, coordinates.Value);
			}
			_alerts.ShowAlert(ent.Owner, alert, severity);
		}
	}

	private void OnRequestTrackableName(Entity<RMCTrackableComponent> ent, ref RequestTrackableNameEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (args.Handled)
		{
			return;
		}
		Entity<HiveComponent>? hive = _hive.GetHive(Entity<HiveMemberComponent>.op_Implicit(ent.Owner));
		if (!hive.HasValue)
		{
			return;
		}
		foreach (KeyValuePair<string, EntityUid> item in hive.Value.Comp.HiveTunnels)
		{
			if (!(item.Value != ent.Owner))
			{
				args.Name = item.Key;
				break;
			}
		}
		args.Handled = true;
	}

	private void OnMobStateChanged(Entity<RMCTrackableComponent> ent, ref MobStateChangedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoComponent>(Entity<RMCTrackableComponent>.op_Implicit(ent)) && args.NewMobState == MobState.Dead)
		{
			((EntitySystem)this).RemCompDeferred<RMCTrackableComponent>(Entity<RMCTrackableComponent>.op_Implicit(ent));
		}
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<HiveTrackerComponent> query = ((EntitySystem)this).EntityQueryEnumerator<HiveTrackerComponent>();
		EntityUid uid = default(EntityUid);
		HiveTrackerComponent tracker = default(HiveTrackerComponent);
		EntityUid trackableUid = default(EntityUid);
		RMCTrackableComponent rMCTrackableComponent = default(RMCTrackableComponent);
		HiveMemberComponent targetMember = default(HiveMemberComponent);
		HiveMemberComponent member = default(HiveMemberComponent);
		TrackerModePrototype trackerMode = default(TrackerModePrototype);
		IComponent val = default(IComponent);
		while (query.MoveNext(ref uid, ref tracker))
		{
			if (time < tracker.UpdateAt)
			{
				continue;
			}
			tracker.UpdateAt = time + tracker.UpdateEvery;
			if (tracker.Target.HasValue)
			{
				if (!((EntitySystem)this).HasComp<RMCTrackableComponent>(tracker.Target.Value))
				{
					SetTarget(Entity<HiveTrackerComponent>.op_Implicit((uid, tracker)), null);
				}
				else
				{
					UpdateDirection(Entity<HiveTrackerComponent>.op_Implicit((uid, tracker)), _transform.GetMapCoordinates(tracker.Target.Value, (TransformComponent)null));
				}
				continue;
			}
			EntityQueryEnumerator<RMCTrackableComponent, HiveMemberComponent> trackableQuery = ((EntitySystem)this).EntityQueryEnumerator<RMCTrackableComponent, HiveMemberComponent>();
			while (trackableQuery.MoveNext(ref trackableUid, ref rMCTrackableComponent, ref targetMember) && ((EntitySystem)this).TryComp<HiveMemberComponent>(uid, ref member))
			{
				_prototypeManager.TryIndex<TrackerModePrototype>(tracker.Mode, ref trackerMode);
				if (trackerMode == null || trackerMode.Component == null)
				{
					break;
				}
				EntityUid? hive = member.Hive;
				EntityUid? hive2 = targetMember.Hive;
				if (hive.HasValue != hive2.HasValue || (hive.HasValue && hive.GetValueOrDefault() != hive2.GetValueOrDefault()))
				{
					continue;
				}
				Type trackingComponent = ((object)_factory.GetComponent(trackerMode.Component, false)).GetType();
				if (base.EntityManager.TryGetComponent(trackableUid, trackingComponent, ref val))
				{
					SetTarget(Entity<HiveTrackerComponent>.op_Implicit((uid, tracker)), trackableUid);
					if (tracker.Target.HasValue)
					{
						UpdateDirection(Entity<HiveTrackerComponent>.op_Implicit((uid, tracker)), _transform.GetMapCoordinates(tracker.Target.Value, (TransformComponent)null));
					}
				}
				break;
			}
			UpdateDirection(Entity<HiveTrackerComponent>.op_Implicit((uid, tracker)));
		}
	}
}
