using System;
using Content.Shared.Station.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

namespace Content.Shared.Station;

public abstract class SharedStationSystem : EntitySystem
{
	[Dependency]
	private MetaDataSystem _meta;

	private EntityQuery<TransformComponent> _xformQuery;

	private EntityQuery<StationMemberComponent> _stationMemberQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_xformQuery = ((EntitySystem)this).GetEntityQuery<TransformComponent>();
		_stationMemberQuery = ((EntitySystem)this).GetEntityQuery<StationMemberComponent>();
		((EntitySystem)this).SubscribeLocalEvent<StationTrackerComponent, MapInitEvent>((EntityEventRefHandler<StationTrackerComponent, MapInitEvent>)OnTrackerMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StationTrackerComponent, ComponentRemove>((EntityEventRefHandler<StationTrackerComponent, ComponentRemove>)OnTrackerRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StationTrackerComponent, GridUidChangedEvent>((EntityEventRefHandler<StationTrackerComponent, GridUidChangedEvent>)OnTrackerGridChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StationTrackerComponent, MetaFlagRemoveAttemptEvent>((EntityEventRefHandler<StationTrackerComponent, MetaFlagRemoveAttemptEvent>)OnMetaFlagRemoveAttempt, (Type[])null, (Type[])null);
	}

	private void OnTrackerMapInit(Entity<StationTrackerComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		_meta.AddFlag(Entity<StationTrackerComponent>.op_Implicit(ent), (MetaDataFlags)32, (MetaDataComponent)null);
		UpdateStationTracker(Entity<StationTrackerComponent, TransformComponent>.op_Implicit(ent.AsNullable()));
	}

	private void OnTrackerRemove(Entity<StationTrackerComponent> ent, ref ComponentRemove args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_meta.RemoveFlag(Entity<StationTrackerComponent>.op_Implicit(ent), (MetaDataFlags)32, (MetaDataComponent)null);
	}

	private void OnTrackerGridChanged(Entity<StationTrackerComponent> ent, ref GridUidChangedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		UpdateStationTracker(Entity<StationTrackerComponent, TransformComponent>.op_Implicit((Entity<StationTrackerComponent>.op_Implicit(ent), ent.Comp, ((GridUidChangedEvent)(ref args)).Transform)));
	}

	private void OnMetaFlagRemoveAttempt(Entity<StationTrackerComponent> ent, ref MetaFlagRemoveAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Invalid comparison between Unknown and I4
		if ((args.ToRemove & 0x20) != 0 && (int)((Component)ent.Comp).LifeStage <= 6)
		{
			ref MetaDataFlags toRemove = ref args.ToRemove;
			toRemove = (MetaDataFlags)((uint)toRemove & 0xDFu);
		}
	}

	public void UpdateStationTracker(Entity<StationTrackerComponent?, TransformComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StationTrackerComponent>(Entity<StationTrackerComponent, TransformComponent>.op_Implicit(ent), ref ent.Comp1, true))
		{
			return;
		}
		TransformComponent xform = ent.Comp2;
		if (_xformQuery.Resolve(Entity<StationTrackerComponent, TransformComponent>.op_Implicit(ent), ref xform, true))
		{
			StationMemberComponent stationMember = default(StationMemberComponent);
			if (xform.MapID == MapId.Nullspace || !xform.GridUid.HasValue)
			{
				SetStation(Entity<StationTrackerComponent, TransformComponent>.op_Implicit(ent), null);
			}
			else if (!_stationMemberQuery.TryGetComponent(xform.GridUid.Value, ref stationMember))
			{
				SetStation(Entity<StationTrackerComponent, TransformComponent>.op_Implicit(ent), null);
			}
			else
			{
				SetStation(Entity<StationTrackerComponent, TransformComponent>.op_Implicit(ent), stationMember.Station);
			}
		}
	}

	public void SetStation(Entity<StationTrackerComponent?> ent, EntityUid? station)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<StationTrackerComponent>(Entity<StationTrackerComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			EntityUid? station2 = ent.Comp.Station;
			EntityUid? val = station;
			if (station2.HasValue != val.HasValue || (station2.HasValue && !(station2.GetValueOrDefault() == val.GetValueOrDefault())))
			{
				ent.Comp.Station = station;
				((EntitySystem)this).Dirty<StationTrackerComponent>(ent, (MetaDataComponent)null);
			}
		}
	}

	public EntityUid? GetCurrentStation(Entity<StationTrackerComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StationTrackerComponent>(Entity<StationTrackerComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return null;
		}
		return ent.Comp.Station;
	}
}
