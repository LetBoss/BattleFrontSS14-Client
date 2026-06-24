using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stun;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Effects;
using Content.Shared.IdentityManagement;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Damage.ObstacleSlamming;

public sealed class RMCObstacleSlammingSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedColorFlashEffectSystem _color;

	[Dependency]
	private RMCSizeStunSystem _size;

	[Dependency]
	private RMCSlowSystem _slow;

	private static readonly ProtoId<DamageTypePrototype> SlamDamageType = ProtoId<DamageTypePrototype>.op_Implicit("Blunt");

	private readonly HashSet<EntityUid> _queuedImmuneEntities = new HashSet<EntityUid>();

	private readonly HashSet<EntityUid> _queuedBonusEntities = new HashSet<EntityUid>();

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RMCObstacleSlammingComponent, ThrowDoHitEvent>((EntityEventRefHandler<RMCObstacleSlammingComponent, ThrowDoHitEvent>)HandleCollide, (Type[])null, (Type[])null);
	}

	private void HandleCollide(Entity<RMCObstacleSlammingComponent> ent, ref ThrowDoHitEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.Thrown;
		EntityUid obstacle = args.Target;
		PhysicsComponent body = default(PhysicsComponent);
		PhysicsComponent bodyObstacle = default(PhysicsComponent);
		if (((HandledEntityEventArgs)args).Handled || user != ent.Owner || ((EntitySystem)this).HasComp<RMCObstacleSlamImmuneComponent>(user) || ((EntitySystem)this).HasComp<RMCObstacleSlamCauserImmuneComponent>(obstacle) || !((EntitySystem)this).TryComp<PhysicsComponent>(user, ref body) || !((EntitySystem)this).TryComp<PhysicsComponent>(obstacle, ref bodyObstacle) || !body.Hard || !bodyObstacle.Hard || !_size.TryGetSize(user, out var size) || !((EntitySystem)this).HasComp<DamageableComponent>(user))
		{
			return;
		}
		float speed = body.LinearVelocity.Length();
		if (ent.Comp.LastHit.HasValue && _timing.CurTime - ent.Comp.LastHit.Value < ent.Comp.DamageCooldown)
		{
			return;
		}
		ent.Comp.LastHit = _timing.CurTime;
		float damageAmount = (1f + ent.Comp.MobSizeCoefficient / (float)(int)(size + 1)) * ent.Comp.ThrowSpeedCoefficient * speed;
		DamageSpecifier damage = new DamageSpecifier
		{
			DamageDict = { [ProtoId<DamageTypePrototype>.op_Implicit(SlamDamageType)] = damageAmount }
		};
		_damageable.TryChangeDamage(user, damage);
		_color.RaiseEffect(Color.Red, new List<EntityUid> { user }, Filter.Pvs(user, 2f, (IEntityManager)null, (ISharedPlayerManager)null, (IConfigurationManager)null));
		RMCObstacleSlamBonusEffectsComponent bonus = default(RMCObstacleSlamBonusEffectsComponent);
		if (((EntitySystem)this).TryComp<RMCObstacleSlamBonusEffectsComponent>(user, ref bonus))
		{
			_slow.TrySlowdown(user, bonus.Slow, refresh: false);
			_stun.TryParalyze(user, bonus.Stun, refresh: false);
		}
		_physics.SetLinearVelocity(Entity<RMCObstacleSlammingComponent>.op_Implicit(ent), Vector2.Zero, true, true, (FixturesComponent)null, (PhysicsComponent)null);
		_physics.SetAngularVelocity(Entity<RMCObstacleSlammingComponent>.op_Implicit(ent), 0f, true, (FixturesComponent)null, (PhysicsComponent)null);
		if ((_transform.GetMoverCoordinates(user).Position - _transform.GetMoverCoordinates(obstacle).Position).Length() != 0f)
		{
			_size.KnockBack(user, _transform.GetMapCoordinates(obstacle, (TransformComponent)null), ent.Comp.KnockbackPower, ent.Comp.KnockbackPower, ent.Comp.KnockBackSpeed);
		}
		if (_timing.IsFirstTimePredicted)
		{
			_audio.PlayPvs(ent.Comp.SoundHit, obstacle, (AudioParams?)null);
		}
		if (_net.IsServer)
		{
			EntProtoId? hitEffect = ent.Comp.HitEffect;
			((EntitySystem)this).SpawnAttachedTo(hitEffect.HasValue ? EntProtoId.op_Implicit(hitEffect.GetValueOrDefault()) : null, user.ToCoordinates(), (ComponentRegistry)null, default(Angle));
		}
		string selfMessage = base.Loc.GetString("rmc-obstacle-slam-self", (ValueTuple<string, object>)("ent", user), (ValueTuple<string, object>)("object", Identity.Name(obstacle, (IEntityManager)(object)base.EntityManager, user)));
		_popup.PopupClient(selfMessage, user, user, PopupType.MediumCaution);
		foreach (ICommonSession recipient in Filter.PvsExcept(user, 2f, (IEntityManager)null).Recipients)
		{
			EntityUid? attachedEntity = recipient.AttachedEntity;
			if (attachedEntity.HasValue)
			{
				EntityUid otherEnt = attachedEntity.GetValueOrDefault();
				string otherMessage = base.Loc.GetString("rmc-obstacle-slam-others", (ValueTuple<string, object>)("ent", user), (ValueTuple<string, object>)("object", Identity.Name(obstacle, (IEntityManager)(object)base.EntityManager, otherEnt)));
				_popup.PopupEntity(otherMessage, user, otherEnt, PopupType.MediumCaution);
			}
		}
		((HandledEntityEventArgs)args).Handled = true;
	}

	public void MakeImmune(EntityUid uid, float immuneDuration = 2f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		RMCObstacleSlamImmuneComponent comp = ((EntitySystem)this).EnsureComp<RMCObstacleSlamImmuneComponent>(uid);
		comp.ExpireIn = TimeSpan.FromSeconds(immuneDuration);
		comp.ExpireAt = _timing.CurTime + comp.ExpireIn;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
	}

	public void ApplyBonuses(EntityUid uid, TimeSpan stun, TimeSpan slow)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		RMCObstacleSlamBonusEffectsComponent comp = ((EntitySystem)this).EnsureComp<RMCObstacleSlamBonusEffectsComponent>(uid);
		comp.ExpireAt = _timing.CurTime + comp.ExpireIn;
		comp.Stun = stun;
		comp.Slow = slow;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
	}

	public override void Update(float frameTime)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		_queuedImmuneEntities.Clear();
		EntityQueryEnumerator<RMCObstacleSlamImmuneComponent> immuneQuery = ((EntitySystem)this).EntityQueryEnumerator<RMCObstacleSlamImmuneComponent>();
		EntityUid uid = default(EntityUid);
		RMCObstacleSlamImmuneComponent comp = default(RMCObstacleSlamImmuneComponent);
		while (immuneQuery.MoveNext(ref uid, ref comp))
		{
			if (!comp.ExpireAt.HasValue || !(comp.ExpireAt.Value > _timing.CurTime))
			{
				_queuedImmuneEntities.Add(uid);
			}
		}
		foreach (EntityUid queued in _queuedImmuneEntities)
		{
			((EntitySystem)this).RemComp<RMCObstacleSlamImmuneComponent>(queued);
		}
		_queuedBonusEntities.Clear();
		EntityQueryEnumerator<RMCObstacleSlamBonusEffectsComponent> bonusQuery = ((EntitySystem)this).EntityQueryEnumerator<RMCObstacleSlamBonusEffectsComponent>();
		EntityUid uid2 = default(EntityUid);
		RMCObstacleSlamBonusEffectsComponent comp2 = default(RMCObstacleSlamBonusEffectsComponent);
		while (bonusQuery.MoveNext(ref uid2, ref comp2))
		{
			if (!comp2.ExpireAt.HasValue || !(comp2.ExpireAt.Value > _timing.CurTime))
			{
				_queuedBonusEntities.Add(uid2);
			}
		}
		foreach (EntityUid queued2 in _queuedBonusEntities)
		{
			((EntitySystem)this).RemComp<RMCObstacleSlamBonusEffectsComponent>(queued2);
		}
	}
}
