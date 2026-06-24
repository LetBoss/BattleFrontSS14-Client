using System;
using System.Collections.Generic;
using System.Linq;
using Content.Client.Projectiles;
using Content.Shared._RMC14.Weapons.Ranged.Prediction;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Client.GameObjects;
using Robust.Client.Physics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Client._RMC14.Weapons.Ranged.Prediction;

public sealed class GunPredictionSystem : SharedGunPredictionSystem
{
	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private ProjectileSystem _projectile;

	[Dependency]
	private SpriteSystem _sprite;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	private EntityQuery<IgnorePredictionHideComponent> _ignorePredictionHideQuery;

	private EntityQuery<IgnorePredictionHitComponent> _ignorePredictionHitQuery;

	private EntityQuery<SpriteComponent> _spriteQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize();
		_ignorePredictionHideQuery = ((EntitySystem)this).GetEntityQuery<IgnorePredictionHideComponent>();
		_ignorePredictionHitQuery = ((EntitySystem)this).GetEntityQuery<IgnorePredictionHitComponent>();
		_spriteQuery = ((EntitySystem)this).GetEntityQuery<SpriteComponent>();
		((EntitySystem)this).SubscribeLocalEvent<PhysicsUpdateBeforeSolveEvent>((EntityEventRefHandler<PhysicsUpdateBeforeSolveEvent>)OnBeforeSolve, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PhysicsUpdateAfterSolveEvent>((EntityEventRefHandler<PhysicsUpdateAfterSolveEvent>)OnAfterSolve, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RequestShootEvent>((EntitySessionEventHandler<RequestShootEvent>)OnShootRequest, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PredictedProjectileClientComponent, UpdateIsPredictedEvent>((EntityEventRefHandler<PredictedProjectileClientComponent, UpdateIsPredictedEvent>)OnClientProjectileUpdateIsPredicted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PredictedProjectileClientComponent, StartCollideEvent>((EntityEventRefHandler<PredictedProjectileClientComponent, StartCollideEvent>)OnClientProjectileStartCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PredictedProjectileServerComponent, ComponentStartup>((EntityEventRefHandler<PredictedProjectileServerComponent, ComponentStartup>)OnServerProjectileStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).UpdatesBefore.Add(typeof(TransformSystem));
	}

	private void OnBeforeSolve(ref PhysicsUpdateBeforeSolveEvent ev)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<PredictedProjectileClientComponent> val = ((EntitySystem)this).EntityQueryEnumerator<PredictedProjectileClientComponent>();
		EntityUid val2 = default(EntityUid);
		PredictedProjectileClientComponent predictedProjectileClientComponent = default(PredictedProjectileClientComponent);
		while (val.MoveNext(ref val2, ref predictedProjectileClientComponent))
		{
			predictedProjectileClientComponent.Coordinates = ((EntitySystem)this).Transform(val2).Coordinates;
		}
	}

	private void OnAfterSolve(ref PhysicsUpdateAfterSolveEvent ev)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (_timing.IsFirstTimePredicted)
		{
			return;
		}
		EntityQueryEnumerator<PredictedProjectileClientComponent> val = ((EntitySystem)this).EntityQueryEnumerator<PredictedProjectileClientComponent>();
		EntityUid val2 = default(EntityUid);
		PredictedProjectileClientComponent predictedProjectileClientComponent = default(PredictedProjectileClientComponent);
		while (val.MoveNext(ref val2, ref predictedProjectileClientComponent))
		{
			EntityCoordinates? coordinates = predictedProjectileClientComponent.Coordinates;
			if (coordinates.HasValue)
			{
				EntityCoordinates valueOrDefault = coordinates.GetValueOrDefault();
				_transform.SetCoordinates(val2, valueOrDefault);
			}
			predictedProjectileClientComponent.Coordinates = null;
		}
	}

	private void OnShootRequest(RequestShootEvent ev, EntitySessionEventArgs args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		ShootRequested(ev.Gun, ev.Coordinates, ev.Target, null, ((EntitySessionEventArgs)(ref args)).SenderSession);
	}

	private void OnClientProjectileUpdateIsPredicted(Entity<PredictedProjectileClientComponent> ent, ref UpdateIsPredictedEvent args)
	{
		args.IsPredicted = true;
	}

	private void OnClientProjectileStartCollide(Entity<PredictedProjectileClientComponent> ent, ref StartCollideEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		ProjectileComponent item = default(ProjectileComponent);
		PhysicsComponent item2 = default(PhysicsComponent);
		if (!ent.Comp.Hit && ((EntitySystem)this).TryComp<ProjectileComponent>(Entity<PredictedProjectileClientComponent>.op_Implicit(ent), ref item) && ((EntitySystem)this).TryComp<PhysicsComponent>(Entity<PredictedProjectileClientComponent>.op_Implicit(ent), ref item2) && !_ignorePredictionHitQuery.HasComp(args.OtherEntity))
		{
			NetEntity netEntity = ((EntitySystem)this).GetNetEntity(args.OtherEntity, (MetaDataComponent)null);
			MapCoordinates mapCoordinates = _transform.GetMapCoordinates(args.OtherEntity, (TransformComponent)null);
			HashSet<(NetEntity, MapCoordinates)> hit = new HashSet<(NetEntity, MapCoordinates)> { (netEntity, mapCoordinates) };
			PredictedProjectileHitEvent predictedProjectileHitEvent = new PredictedProjectileHitEvent(ent.Owner.Id, hit);
			ent.Comp.Hit = true;
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)predictedProjectileHitEvent);
			_projectile.ProjectileCollide(Entity<ProjectileComponent, PhysicsComponent>.op_Implicit((Entity<PredictedProjectileClientComponent>.op_Implicit(ent), item, item2)), args.OtherEntity);
		}
	}

	private void OnServerProjectileStartup(Entity<PredictedProjectileServerComponent> ent, ref ComponentStartup args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		if (base.GunPrediction)
		{
			EntityUid? clientEnt = ent.Comp.ClientEnt;
			EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
			SpriteComponent item = default(SpriteComponent);
			if (clientEnt.HasValue == localEntity.HasValue && (!clientEnt.HasValue || !(clientEnt.GetValueOrDefault() != localEntity.GetValueOrDefault())) && !_ignorePredictionHideQuery.HasComp(Entity<PredictedProjectileServerComponent>.op_Implicit(ent)) && _spriteQuery.TryComp(Entity<PredictedProjectileServerComponent>.op_Implicit(ent), ref item))
			{
				_sprite.SetVisible(Entity<SpriteComponent>.op_Implicit((Entity<PredictedProjectileServerComponent>.op_Implicit(ent), item)), false);
			}
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		if (!_timing.IsFirstTimePredicted)
		{
			return;
		}
		EntityQueryEnumerator<PredictedProjectileClientComponent, ProjectileComponent, PhysicsComponent> val = ((EntitySystem)this).EntityQueryEnumerator<PredictedProjectileClientComponent, ProjectileComponent, PhysicsComponent>();
		EntityUid val2 = default(EntityUid);
		PredictedProjectileClientComponent predictedProjectileClientComponent = default(PredictedProjectileClientComponent);
		ProjectileComponent item = default(ProjectileComponent);
		PhysicsComponent val3 = default(PhysicsComponent);
		while (val.MoveNext(ref val2, ref predictedProjectileClientComponent, ref item, ref val3))
		{
			if (predictedProjectileClientComponent.Hit)
			{
				continue;
			}
			HashSet<EntityUid> contactingEntities = _physics.GetContactingEntities(val2, val3, true);
			if (contactingEntities.Count == 0)
			{
				continue;
			}
			HashSet<(NetEntity, MapCoordinates)> hashSet = new HashSet<(NetEntity, MapCoordinates)>();
			foreach (EntityUid item4 in contactingEntities)
			{
				if (!_ignorePredictionHitQuery.HasComp(item4))
				{
					NetEntity netEntity = ((EntitySystem)this).GetNetEntity(item4, (MetaDataComponent)null);
					MapCoordinates mapCoordinates = _transform.GetMapCoordinates(item4, (TransformComponent)null);
					hashSet.Add((netEntity, mapCoordinates));
				}
			}
			if (hashSet.Count != 0)
			{
				PredictedProjectileHitEvent predictedProjectileHitEvent = new PredictedProjectileHitEvent(val2.Id, hashSet);
				predictedProjectileClientComponent.Hit = true;
				((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)predictedProjectileHitEvent);
				_projectile.ProjectileCollide(Entity<ProjectileComponent, PhysicsComponent>.op_Implicit((val2, item, val3)), contactingEntities.First());
			}
		}
		EntityQueryEnumerator<PredictedProjectileHitComponent, SpriteComponent, TransformComponent> val4 = ((EntitySystem)this).EntityQueryEnumerator<PredictedProjectileHitComponent, SpriteComponent, TransformComponent>();
		EntityUid item2 = default(EntityUid);
		PredictedProjectileHitComponent predictedProjectileHitComponent = default(PredictedProjectileHitComponent);
		SpriteComponent item3 = default(SpriteComponent);
		TransformComponent val5 = default(TransformComponent);
		float num = default(float);
		while (val4.MoveNext(ref item2, ref predictedProjectileHitComponent, ref item3, ref val5))
		{
			EntityCoordinates origin = predictedProjectileHitComponent.Origin;
			EntityCoordinates coordinates = val5.Coordinates;
			if (!((EntityCoordinates)(ref origin)).TryDistance((IEntityManager)(object)((EntitySystem)this).EntityManager, _transform, coordinates, ref num) || num >= predictedProjectileHitComponent.Distance)
			{
				_sprite.SetVisible(Entity<SpriteComponent>.op_Implicit((item2, item3)), false);
			}
		}
	}

	public override void FrameUpdate(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).FrameUpdate(frameTime);
		EntityQueryEnumerator<PredictedProjectileClientComponent, TransformComponent> val = ((EntitySystem)this).EntityQueryEnumerator<PredictedProjectileClientComponent, TransformComponent>();
		PredictedProjectileClientComponent predictedProjectileClientComponent = default(PredictedProjectileClientComponent);
		TransformComponent val2 = default(TransformComponent);
		while (val.MoveNext(ref predictedProjectileClientComponent, ref val2))
		{
			val2.ActivelyLerping = false;
		}
	}
}
