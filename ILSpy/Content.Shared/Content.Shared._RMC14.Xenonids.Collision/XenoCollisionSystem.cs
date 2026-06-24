using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Xenonids.Egg;
using Content.Shared._RMC14.Xenonids.Fortify;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Rest;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Standing;
using Content.Shared.StatusEffect;
using Content.Shared.Stunnable;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Collision;

public sealed class XenoCollisionSystem : EntitySystem
{
	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private StandingStateSystem _standingState;

	[Dependency]
	private StatusEffectsSystem _statusEffects;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private XenoRestSystem _xenoRest;

	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private DamageableSystem _damage;

	[Dependency]
	private SharedColorFlashEffectSystem _colorFlash;

	private EntityQuery<MobCollisionComponent> _mobCollisionQuery;

	private EntityQuery<StunFriendlyXenoOnStepComponent> _stunFriendlyXenoOnStepQuery;

	private EntityQuery<StunHostilesOnStepComponent> _stunHostileOnStepQuery;

	private EntityQuery<XenoFortifyComponent> _xenoFortifyQuery;

	private readonly HashSet<EntityUid> _contacts = new HashSet<EntityUid>();

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
		_mobCollisionQuery = ((EntitySystem)this).GetEntityQuery<MobCollisionComponent>();
		_stunFriendlyXenoOnStepQuery = ((EntitySystem)this).GetEntityQuery<StunFriendlyXenoOnStepComponent>();
		_stunHostileOnStepQuery = ((EntitySystem)this).GetEntityQuery<StunHostilesOnStepComponent>();
		_xenoFortifyQuery = ((EntitySystem)this).GetEntityQuery<XenoFortifyComponent>();
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, AttemptMobTargetCollideEvent>((EntityEventRefHandler<XenoComponent, AttemptMobTargetCollideEvent>)OnXenoAttemptMobTargetCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StunFriendlyXenoOnStepComponent, MobStateChangedEvent>((EntityEventRefHandler<StunFriendlyXenoOnStepComponent, MobStateChangedEvent>)OnStunUpdated, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StunFriendlyXenoOnStepComponent, XenoRestEvent>((EntityEventRefHandler<StunFriendlyXenoOnStepComponent, XenoRestEvent>)OnStunUpdated, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StunFriendlyXenoOnStepComponent, StunnedEvent>((EntityEventRefHandler<StunFriendlyXenoOnStepComponent, StunnedEvent>)OnStunUpdated, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StunFriendlyXenoOnStepComponent, StatusEffectEndedEvent>((EntityEventRefHandler<StunFriendlyXenoOnStepComponent, StatusEffectEndedEvent>)OnStunUpdated, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StunFriendlyXenoOnStepComponent, XenoOvipositorChangedEvent>((EntityEventRefHandler<StunFriendlyXenoOnStepComponent, XenoOvipositorChangedEvent>)OnStunUpdated, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StunFriendlyXenoOnStepComponent, PreventCollideEvent>((EntityEventRefHandler<StunFriendlyXenoOnStepComponent, PreventCollideEvent>)OnPreventCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StunHostilesOnStepComponent, MobStateChangedEvent>((EntityEventRefHandler<StunHostilesOnStepComponent, MobStateChangedEvent>)OnStunUpdatedHostile, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StunHostilesOnStepComponent, XenoRestEvent>((EntityEventRefHandler<StunHostilesOnStepComponent, XenoRestEvent>)OnStunUpdatedHostile, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StunHostilesOnStepComponent, StunnedEvent>((EntityEventRefHandler<StunHostilesOnStepComponent, StunnedEvent>)OnStunUpdatedHostile, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StunHostilesOnStepComponent, StatusEffectEndedEvent>((EntityEventRefHandler<StunHostilesOnStepComponent, StatusEffectEndedEvent>)OnStunUpdatedHostile, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StunHostilesOnStepComponent, XenoOvipositorChangedEvent>((EntityEventRefHandler<StunHostilesOnStepComponent, XenoOvipositorChangedEvent>)OnStunUpdatedHostile, (Type[])null, (Type[])null);
	}

	private void OnXenoAttemptMobTargetCollide(Entity<XenoComponent> ent, ref AttemptMobTargetCollideEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && _stunFriendlyXenoOnStepQuery.HasComp(args.Entity))
		{
			args.Cancelled = true;
		}
	}

	private void OnStunUpdated<T>(Entity<StunFriendlyXenoOnStepComponent> ent, ref T args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		StunFriendlyXenoOnStepComponent comp = ent.Comp;
		int enabled;
		if (_mobState.IsAlive(Entity<StunFriendlyXenoOnStepComponent>.op_Implicit(ent)) && !((EntitySystem)this).HasComp<XenoRestingComponent>(Entity<StunFriendlyXenoOnStepComponent>.op_Implicit(ent)) && !_statusEffects.HasStatusEffect(Entity<StunFriendlyXenoOnStepComponent>.op_Implicit(ent), ProtoId<StatusEffectPrototype>.op_Implicit(ent.Comp.DisableStatus)))
		{
			XenoAttachedOvipositorComponent xenoAttachedOvipositorComponent = ((EntitySystem)this).CompOrNull<XenoAttachedOvipositorComponent>(Entity<StunFriendlyXenoOnStepComponent>.op_Implicit(ent));
			enabled = ((xenoAttachedOvipositorComponent == null || !((Component)xenoAttachedOvipositorComponent).Running) ? 1 : 0);
		}
		else
		{
			enabled = 0;
		}
		comp.Enabled = (byte)enabled != 0;
		((EntitySystem)this).Dirty<StunFriendlyXenoOnStepComponent>(ent, (MetaDataComponent)null);
	}

	private void OnStunUpdatedHostile<T>(Entity<StunHostilesOnStepComponent> ent, ref T args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		StunHostilesOnStepComponent comp = ent.Comp;
		int enabled;
		if (_mobState.IsAlive(Entity<StunHostilesOnStepComponent>.op_Implicit(ent)) && !((EntitySystem)this).HasComp<XenoRestingComponent>(Entity<StunHostilesOnStepComponent>.op_Implicit(ent)) && !_statusEffects.HasStatusEffect(Entity<StunHostilesOnStepComponent>.op_Implicit(ent), ProtoId<StatusEffectPrototype>.op_Implicit(ent.Comp.DisableStatus)))
		{
			XenoAttachedOvipositorComponent xenoAttachedOvipositorComponent = ((EntitySystem)this).CompOrNull<XenoAttachedOvipositorComponent>(Entity<StunHostilesOnStepComponent>.op_Implicit(ent));
			enabled = ((xenoAttachedOvipositorComponent == null || !((Component)xenoAttachedOvipositorComponent).Running) ? 1 : 0);
		}
		else
		{
			enabled = 0;
		}
		comp.Enabled = (byte)enabled != 0;
		((EntitySystem)this).Dirty<StunHostilesOnStepComponent>(ent, (MetaDataComponent)null);
	}

	private void OnPreventCollide(Entity<StunFriendlyXenoOnStepComponent> ent, ref PreventCollideEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		XenoFortifyComponent fortify = default(XenoFortifyComponent);
		if (_xenoFortifyQuery.TryComp(args.OtherEntity, ref fortify) && fortify.Fortified)
		{
			args.Cancelled = true;
		}
	}

	public override void Update(float frameTime)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<StunFriendlyXenoOnStepComponent, TransformComponent> query = ((EntitySystem)this).EntityQueryEnumerator<StunFriendlyXenoOnStepComponent, TransformComponent>();
		EntityUid uid = default(EntityUid);
		StunFriendlyXenoOnStepComponent comp = default(StunFriendlyXenoOnStepComponent);
		TransformComponent xform = default(TransformComponent);
		while (query.MoveNext(ref uid, ref comp, ref xform))
		{
			if (!comp.Enabled)
			{
				continue;
			}
			_contacts.Clear();
			_physics.GetContactingEntities(Entity<PhysicsComponent>.op_Implicit(uid), _contacts, false);
			AttemptMobCollideEvent evOne = default(AttemptMobCollideEvent);
			((EntitySystem)this).RaiseLocalEvent<AttemptMobCollideEvent>(uid, ref evOne, false);
			if (evOne.Cancelled)
			{
				continue;
			}
			foreach (EntityUid other in _contacts)
			{
				if (_mobState.IsDead(other) || _xenoRest.IsResting(Entity<XenoRestingComponent>.op_Implicit(other)) || _standingState.IsDown(other) || !_mobCollisionQuery.HasComp(other))
				{
					continue;
				}
				TransformComponent otherTransform = ((EntitySystem)this).Transform(other);
				Box2 ourAabb = _entityLookup.GetAABBNoContainer(uid, xform.LocalPosition, xform.LocalRotation);
				Box2 otherAabb = _entityLookup.GetAABBNoContainer(other, otherTransform.LocalPosition, otherTransform.LocalRotation);
				if (!((Box2)(ref ourAabb)).Intersects(ref otherAabb))
				{
					continue;
				}
				Box2 val = ((Box2)(ref otherAabb)).Intersect(ref ourAabb);
				float intersect = Box2.Area(ref val);
				if (Math.Max(intersect / Box2.Area(ref otherAabb), intersect / Box2.Area(ref ourAabb)) < comp.Ratio)
				{
					continue;
				}
				AttemptMobTargetCollideEvent evTwo = default(AttemptMobTargetCollideEvent);
				((EntitySystem)this).RaiseLocalEvent<AttemptMobTargetCollideEvent>(other, ref evTwo, false);
				if (!evTwo.Cancelled && _hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(uid), Entity<HiveMemberComponent>.op_Implicit(other)))
				{
					RecentlyStunnedByFriendlyXenoComponent recently = ((EntitySystem)this).EnsureComp<RecentlyStunnedByFriendlyXenoComponent>(other);
					if (!(time < recently.At + comp.Cooldown))
					{
						recently.At = time;
						((EntitySystem)this).Dirty(other, (IComponent)(object)recently, (MetaDataComponent)null);
						_stun.TryParalyze(other, comp.Duration, refresh: true, null, force: true);
					}
				}
			}
		}
		EntityQueryEnumerator<StunHostilesOnStepComponent, TransformComponent> queryTwo = ((EntitySystem)this).EntityQueryEnumerator<StunHostilesOnStepComponent, TransformComponent>();
		EntityUid uid2 = default(EntityUid);
		StunHostilesOnStepComponent comp2 = default(StunHostilesOnStepComponent);
		TransformComponent xform2 = default(TransformComponent);
		while (true)
		{
			if (!queryTwo.MoveNext(ref uid2, ref comp2, ref xform2))
			{
				break;
			}
			if (!comp2.Enabled)
			{
				continue;
			}
			_contacts.Clear();
			_physics.GetContactingEntities(Entity<PhysicsComponent>.op_Implicit(uid2), _contacts, false);
			AttemptMobCollideEvent evOne2 = default(AttemptMobCollideEvent);
			((EntitySystem)this).RaiseLocalEvent<AttemptMobCollideEvent>(uid2, ref evOne2, false);
			if (evOne2.Cancelled)
			{
				continue;
			}
			foreach (EntityUid other2 in _contacts)
			{
				if (_mobState.IsDead(other2) || _xenoRest.IsResting(Entity<XenoRestingComponent>.op_Implicit(other2)) || _standingState.IsDown(other2) || !_mobCollisionQuery.HasComp(other2))
				{
					continue;
				}
				TransformComponent otherTransform2 = ((EntitySystem)this).Transform(other2);
				Box2 ourAabb2 = _entityLookup.GetAABBNoContainer(uid2, xform2.LocalPosition, xform2.LocalRotation);
				Box2 otherAabb2 = _entityLookup.GetAABBNoContainer(other2, otherTransform2.LocalPosition, otherTransform2.LocalRotation);
				if (!((Box2)(ref ourAabb2)).Intersects(ref otherAabb2))
				{
					continue;
				}
				Box2 val = ((Box2)(ref otherAabb2)).Intersect(ref ourAabb2);
				float intersect2 = Box2.Area(ref val);
				if (Math.Max(intersect2 / Box2.Area(ref otherAabb2), intersect2 / Box2.Area(ref ourAabb2)) < comp2.Ratio)
				{
					continue;
				}
				AttemptMobTargetCollideEvent evTwo2 = default(AttemptMobTargetCollideEvent);
				((EntitySystem)this).RaiseLocalEvent<AttemptMobTargetCollideEvent>(other2, ref evTwo2, false);
				if (evTwo2.Cancelled || !_xeno.CanAbilityAttackTarget(uid2, other2))
				{
					continue;
				}
				RecentlyStunnedByHostileXenoComponent recently2 = ((EntitySystem)this).EnsureComp<RecentlyStunnedByHostileXenoComponent>(other2);
				if (time < recently2.At + comp2.Cooldown)
				{
					continue;
				}
				recently2.At = time;
				((EntitySystem)this).Dirty(other2, (IComponent)(object)recently2, (MetaDataComponent)null);
				_stun.TryParalyze(other2, comp2.Duration, refresh: true, null, force: true);
				if (!((EntitySystem)this).HasComp<XenoComponent>(other2) && _damage.TryChangeDamage(other2, comp2.Damage, ignoreResistances: false, interruptsDoAfters: true, null, uid2, uid2)?.GetTotal() > FixedPoint2.Zero)
				{
					Filter filter = Filter.Pvs(other2, 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null, (IConfigurationManager)null).RemoveWhereAttachedEntity((Predicate<EntityUid>)((EntityUid o) => o == uid2));
					_colorFlash.RaiseEffect(Color.Red, new List<EntityUid> { other2 }, filter);
				}
			}
		}
	}
}
