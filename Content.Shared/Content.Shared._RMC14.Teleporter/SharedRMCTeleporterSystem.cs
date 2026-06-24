using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Dropship;
using Content.Shared._RMC14.Marines;
using Content.Shared.Damage;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Pulling.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Player;

namespace Content.Shared._RMC14.Teleporter;

public abstract class SharedRMCTeleporterSystem : EntitySystem
{
	[Dependency]
	private PullingSystem _pulling;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private DamageableSystem _damageableSystem;

	private EntityQuery<ActorComponent> _actorQuery;

	private EntityQuery<AlmayerComponent> _almayerQuery;

	private EntityQuery<DropshipComponent> _dropshipQuery;

	private EntityQuery<MapGridComponent> _mapGridQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		_actorQuery = ((EntitySystem)this).GetEntityQuery<ActorComponent>();
		_almayerQuery = ((EntitySystem)this).GetEntityQuery<AlmayerComponent>();
		_dropshipQuery = ((EntitySystem)this).GetEntityQuery<DropshipComponent>();
		_mapGridQuery = ((EntitySystem)this).GetEntityQuery<MapGridComponent>();
		((EntitySystem)this).SubscribeLocalEvent<RMCTeleporterComponent, StartCollideEvent>((EntityEventRefHandler<RMCTeleporterComponent, StartCollideEvent>)OnTeleportStartCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCTeleporterViewerComponent, StartCollideEvent>((EntityEventRefHandler<RMCTeleporterViewerComponent, StartCollideEvent>)OnViewerStartCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCTeleporterViewerComponent, EndCollideEvent>((EntityEventRefHandler<RMCTeleporterViewerComponent, EndCollideEvent>)OnViewerEndCollide, (Type[])null, (Type[])null);
	}

	private void OnTeleportStartCollide(Entity<RMCTeleporterComponent> ent, ref StartCollideEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		EntityUid other = args.OtherEntity;
		if (_almayerQuery.HasComp(other) || _dropshipQuery.HasComp(other) || _mapGridQuery.HasComp(other))
		{
			return;
		}
		MapCoordinates otherCoords = _transform.GetMapCoordinates(other, (TransformComponent)null);
		MapCoordinates teleporter = _transform.GetMapCoordinates(Entity<RMCTeleporterComponent>.op_Implicit(ent), (TransformComponent)null);
		if (otherCoords.MapId != teleporter.MapId)
		{
			return;
		}
		Vector2 diff = otherCoords.Position - teleporter.Position;
		if (!(diff.Length() > 10f))
		{
			teleporter = ((MapCoordinates)(ref teleporter)).Offset(diff);
			teleporter = ((MapCoordinates)(ref teleporter)).Offset(ent.Comp.Adjust);
			HandlePulling(other, teleporter);
			if (ent.Comp.TeleportDamage != null)
			{
				_damageableSystem.TryChangeDamage(args.OtherEntity, ent.Comp.TeleportDamage, ignoreResistances: false, interruptsDoAfters: true, null, Entity<RMCTeleporterComponent>.op_Implicit(ent));
			}
		}
	}

	private void OnViewerStartCollide(Entity<RMCTeleporterViewerComponent> ent, ref StartCollideEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		ActorComponent actor = default(ActorComponent);
		if (!_actorQuery.TryComp(args.OtherEntity, ref actor))
		{
			return;
		}
		EntityQueryEnumerator<RMCTeleporterViewerComponent> query = ((EntitySystem)this).EntityQueryEnumerator<RMCTeleporterViewerComponent>();
		EntityUid uid = default(EntityUid);
		RMCTeleporterViewerComponent viewer = default(RMCTeleporterViewerComponent);
		while (query.MoveNext(ref uid, ref viewer))
		{
			if (!(uid == ent.Owner) && !(viewer.Id != ent.Comp.Id))
			{
				AddViewer(Entity<RMCTeleporterViewerComponent>.op_Implicit((uid, viewer)), actor.PlayerSession);
			}
		}
	}

	private void OnViewerEndCollide(Entity<RMCTeleporterViewerComponent> ent, ref EndCollideEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		ActorComponent actor = default(ActorComponent);
		if (!_actorQuery.TryComp(args.OtherEntity, ref actor))
		{
			return;
		}
		EntityQueryEnumerator<RMCTeleporterViewerComponent> query = ((EntitySystem)this).EntityQueryEnumerator<RMCTeleporterViewerComponent>();
		EntityUid uid = default(EntityUid);
		RMCTeleporterViewerComponent viewer = default(RMCTeleporterViewerComponent);
		while (query.MoveNext(ref uid, ref viewer))
		{
			if (!(uid == ent.Owner) && !(viewer.Id != ent.Comp.Id))
			{
				RemoveViewer(Entity<RMCTeleporterViewerComponent>.op_Implicit((uid, viewer)), actor.PlayerSession);
			}
		}
	}

	protected virtual void AddViewer(Entity<RMCTeleporterViewerComponent> viewer, ICommonSession player)
	{
	}

	protected virtual void RemoveViewer(Entity<RMCTeleporterViewerComponent> viewer, ICommonSession player)
	{
	}

	public IEnumerable<Entity<RMCTeleporterViewerComponent>> GetMatchingTeleporterViewers(Entity<RMCTeleporterViewerComponent> viewer)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<RMCTeleporterViewerComponent> viewers = ((EntitySystem)this).EntityQueryEnumerator<RMCTeleporterViewerComponent>();
		EntityUid otherUid = default(EntityUid);
		RMCTeleporterViewerComponent otherViewer = default(RMCTeleporterViewerComponent);
		while (viewers.MoveNext(ref otherUid, ref otherViewer))
		{
			if (viewer.Owner != otherUid && viewer.Comp.Id == otherViewer.Id)
			{
				yield return Entity<RMCTeleporterViewerComponent>.op_Implicit((otherUid, otherViewer));
			}
		}
	}

	public void HandlePulling(EntityUid user, MapCoordinates teleport)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		PullableComponent otherPullable = default(PullableComponent);
		if (((EntitySystem)this).TryComp<PullableComponent>(user, ref otherPullable) && otherPullable.Puller.HasValue)
		{
			_pulling.TryStopPull(user, otherPullable, otherPullable.Puller.Value);
		}
		PullerComponent puller = default(PullerComponent);
		PullableComponent pullable = default(PullableComponent);
		if (((EntitySystem)this).TryComp<PullerComponent>(user, ref puller) && ((EntitySystem)this).TryComp<PullableComponent>(puller.Pulling, ref pullable))
		{
			PullerComponent otherPullingPuller = default(PullerComponent);
			PullableComponent otherPullingPullable = default(PullableComponent);
			if (((EntitySystem)this).TryComp<PullerComponent>(puller.Pulling, ref otherPullingPuller) && ((EntitySystem)this).TryComp<PullableComponent>(otherPullingPuller.Pulling, ref otherPullingPullable))
			{
				_pulling.TryStopPull(otherPullingPuller.Pulling.Value, otherPullingPullable, puller.Pulling);
			}
			EntityUid pulling = puller.Pulling.Value;
			_pulling.TryStopPull(pulling, pullable, user);
			_transform.SetMapCoordinates(user, teleport);
			_transform.SetMapCoordinates(pulling, teleport);
			_pulling.TryStartPull(user, pulling);
		}
		else
		{
			_transform.SetMapCoordinates(user, teleport);
		}
	}
}
