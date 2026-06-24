using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Damage.ObstacleSlamming;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Components;
using Content.Shared.Stunnable;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Sweep;

public sealed class XenoTailSweepSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedColorFlashEffectSystem _colorFlash;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private RotateToFaceSystem _rotateTo;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private XenoPlasmaSystem _xenoPlasma;

	[Dependency]
	private RMCPullingSystem _rmcPulling;

	[Dependency]
	private SharedInteractionSystem _interact;

	[Dependency]
	private RMCSizeStunSystem _size;

	[Dependency]
	private RMCObstacleSlammingSystem _obstacleSlamming;

	private readonly HashSet<Entity<MobStateComponent>> _hit = new HashSet<Entity<MobStateComponent>>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoTailSweepComponent, XenoTailSweepActionEvent>((EntityEventRefHandler<XenoTailSweepComponent, XenoTailSweepActionEvent>)OnXenoTailSweepAction, (Type[])null, (Type[])null);
	}

	private void OnXenoTailSweepAction(Entity<XenoTailSweepComponent> xeno, ref XenoTailSweepActionEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent transform = default(TransformComponent);
		if (!((EntitySystem)this).TryComp(Entity<XenoTailSweepComponent>.op_Implicit(xeno), ref transform))
		{
			return;
		}
		XenoTailSweepAttemptEvent ev = default(XenoTailSweepAttemptEvent);
		((EntitySystem)this).RaiseLocalEvent<XenoTailSweepAttemptEvent>(Entity<XenoTailSweepComponent>.op_Implicit(xeno), ref ev, false);
		if (ev.Cancelled || !_xenoPlasma.TryRemovePlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xeno.Owner), xeno.Comp.PlasmaCost))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		_audio.PlayPredicted(xeno.Comp.Sound, Entity<XenoTailSweepComponent>.op_Implicit(xeno), (EntityUid?)Entity<XenoTailSweepComponent>.op_Implicit(xeno), (AudioParams?)null);
		((EntitySystem)this).EnsureComp<XenoSweepingComponent>(Entity<XenoTailSweepComponent>.op_Implicit(xeno));
		if (_net.IsClient)
		{
			return;
		}
		_hit.Clear();
		_entityLookup.GetEntitiesInRange<MobStateComponent>(transform.Coordinates, xeno.Comp.Range, _hit, (LookupFlags)110);
		MapCoordinates origin = _transform.GetMapCoordinates(Entity<XenoTailSweepComponent>.op_Implicit(xeno), (TransformComponent)null);
		foreach (Entity<MobStateComponent> mob in _hit)
		{
			if (_xeno.CanAbilityAttackTarget(Entity<XenoTailSweepComponent>.op_Implicit(xeno), Entity<MobStateComponent>.op_Implicit(mob)) && _interact.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(xeno.Owner), Entity<TransformComponent>.op_Implicit(mob.Owner), xeno.Comp.Range))
			{
				_rmcPulling.TryStopAllPullsFromAndOn(Entity<MobStateComponent>.op_Implicit(mob));
				DamageSpecifier damage = xeno.Comp.Damage;
				if (damage != null)
				{
					_damageable.TryChangeDamage(Entity<MobStateComponent>.op_Implicit(mob), _xeno.TryApplyXenoSlashDamageMultiplier(Entity<MobStateComponent>.op_Implicit(mob), damage), ignoreResistances: false, interruptsDoAfters: true, null, Entity<XenoTailSweepComponent>.op_Implicit(xeno), Entity<XenoTailSweepComponent>.op_Implicit(xeno));
				}
				Filter filter = Filter.Pvs(Entity<MobStateComponent>.op_Implicit(mob), 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null, (IConfigurationManager)null);
				_colorFlash.RaiseEffect(Color.Red, new List<EntityUid> { Entity<MobStateComponent>.op_Implicit(mob) }, filter);
				_obstacleSlamming.MakeImmune(Entity<MobStateComponent>.op_Implicit(mob));
				_size.KnockBack(Entity<MobStateComponent>.op_Implicit(mob), origin, xeno.Comp.KnockBackDistance, xeno.Comp.KnockBackDistance);
				if (!_size.TryGetSize(Entity<MobStateComponent>.op_Implicit(mob), out var size) || (int)size < 5)
				{
					_stun.TryParalyze(Entity<MobStateComponent>.op_Implicit(mob), _xeno.TryApplyXenoDebuffMultiplier(Entity<MobStateComponent>.op_Implicit(mob), xeno.Comp.ParalyzeTime), refresh: true);
				}
				_audio.PlayPvs(xeno.Comp.HitSound, Entity<MobStateComponent>.op_Implicit(mob), (AudioParams?)null);
				((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(xeno.Comp.HitEffect), mob.Owner.ToCoordinates(), (ComponentRegistry)null, default(Angle));
			}
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<XenoSweepingComponent, TransformComponent> query = ((EntitySystem)this).EntityQueryEnumerator<XenoSweepingComponent, TransformComponent>();
		EntityUid uid = default(EntityUid);
		XenoSweepingComponent sweeping = default(XenoSweepingComponent);
		TransformComponent xform = default(TransformComponent);
		while (query.MoveNext(ref uid, ref sweeping, ref xform))
		{
			if (sweeping.NextRotation > _timing.CurTime)
			{
				continue;
			}
			if (sweeping.TotalRotations >= sweeping.MaxRotations)
			{
				((EntitySystem)this).RemCompDeferred<XenoSweepingComponent>(uid);
				continue;
			}
			sweeping.TotalRotations++;
			sweeping.NextRotation = _timing.CurTime + sweeping.Delay;
			XenoSweepingComponent xenoSweepingComponent = sweeping;
			Direction valueOrDefault = xenoSweepingComponent.LastDirection.GetValueOrDefault();
			if (!xenoSweepingComponent.LastDirection.HasValue)
			{
				Angle worldRotation = _transform.GetWorldRotation(xform);
				valueOrDefault = ((Angle)(ref worldRotation)).GetDir();
				xenoSweepingComponent.LastDirection = valueOrDefault;
			}
			Angle nextAngle = DirectionExtensions.ToAngle(sweeping.LastDirection.Value) + Angle.FromDegrees(90.0);
			sweeping.LastDirection = ((Angle)(ref nextAngle)).GetDir();
			((EntitySystem)this).Dirty(uid, (IComponent)(object)sweeping, (MetaDataComponent)null);
			_rotateTo.TryFaceAngle(uid, nextAngle, xform);
		}
	}

	public override void FrameUpdate(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<XenoSweepingComponent, TransformComponent> query = ((EntitySystem)this).EntityQueryEnumerator<XenoSweepingComponent, TransformComponent>();
		EntityUid uid = default(EntityUid);
		XenoSweepingComponent sweeping = default(XenoSweepingComponent);
		TransformComponent xform = default(TransformComponent);
		while (query.MoveNext(ref uid, ref sweeping, ref xform))
		{
			Direction? lastDirection = sweeping.LastDirection;
			if (lastDirection.HasValue)
			{
				Direction direction = lastDirection.GetValueOrDefault();
				_rotateTo.TryFaceAngle(uid, DirectionExtensions.ToAngle(direction), xform);
			}
		}
	}
}
