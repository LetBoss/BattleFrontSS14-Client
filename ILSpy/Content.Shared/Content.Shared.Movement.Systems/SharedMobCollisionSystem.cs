using System;
using System.Numerics;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Movement;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.CCVar;
using Content.Shared.Movement.Components;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics.Contacts;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Random;
using Robust.Shared.Serialization;

namespace Content.Shared.Movement.Systems;

public abstract class SharedMobCollisionSystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	protected sealed class MobCollisionMessage : EntityEventArgs
	{
		public Vector2 Direction;

		public float SpeedModifier;
	}

	[Dependency]
	protected IConfigurationManager CfgManager;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private MovementSpeedModifierSystem _moveMod;

	[Dependency]
	protected SharedPhysicsSystem Physics;

	[Dependency]
	private SharedTransformSystem _xformSystem;

	protected EntityQuery<MobCollisionComponent> MobQuery;

	protected EntityQuery<PhysicsComponent> PhysicsQuery;

	private float _pushingCap;

	private float _pushingDotProduct;

	private float _minimumPushSquared = 0.01f;

	private float _penCap;

	public const float BufferTime = 0.2f;

	private float _massDiffCap;

	[Dependency]
	private RMCSizeStunSystem _rmcSizeStun;

	private EntityQuery<RMCMobCollisionMassComponent> _rmcMobCollisionMassQuery;

	private EntityQuery<XenoComponent> _xenoQuery;

	private float _penCapSubtract;

	private bool _bigXenosCancelMovement;

	public override void Initialize()
	{
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		UpdatePushCap();
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, CfgManager, CVars.NetTickrate, (Action<int>)delegate
		{
			UpdatePushCap();
		}, false);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, CfgManager, CCVars.MovementMinimumPush, (Action<float>)delegate(float val)
		{
			_minimumPushSquared = val * val;
		}, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, CfgManager, CCVars.MovementPenetrationCap, (Action<float>)delegate(float val)
		{
			_penCap = val;
		}, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, CfgManager, CCVars.MovementPushingCap, (Action<float>)delegate
		{
			UpdatePushCap();
		}, false);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, CfgManager, CCVars.MovementPushingVelocityProduct, (Action<float>)delegate(float value)
		{
			_pushingDotProduct = value;
		}, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, CfgManager, CCVars.MovementPushMassCap, (Action<float>)delegate(float val)
		{
			_massDiffCap = val;
		}, true);
		MobQuery = ((EntitySystem)this).GetEntityQuery<MobCollisionComponent>();
		PhysicsQuery = ((EntitySystem)this).GetEntityQuery<PhysicsComponent>();
		((EntitySystem)this).SubscribeAllEvent<MobCollisionMessage>((EntitySessionEventHandler<MobCollisionMessage>)OnCollision, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobCollisionComponent, RefreshMovementSpeedModifiersEvent>((EntityEventRefHandler<MobCollisionComponent, RefreshMovementSpeedModifiersEvent>)OnMoveModifier, (Type[])null, (Type[])null);
		((EntitySystem)this).UpdatesBefore.Add(typeof(SharedPhysicsSystem));
		_rmcMobCollisionMassQuery = ((EntitySystem)this).GetEntityQuery<RMCMobCollisionMassComponent>();
		_xenoQuery = ((EntitySystem)this).GetEntityQuery<XenoComponent>();
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, CfgManager, RMCCVars.RMCMovementPenCapSubtract, (Action<float>)delegate(float v)
		{
			_penCapSubtract = v;
		}, true);
		EntitySystemSubscriptionExt.CVar<bool>(((EntitySystem)this).Subs, CfgManager, RMCCVars.RMCMovementBigXenosCancelMovement, (Action<bool>)delegate(bool v)
		{
			_bigXenosCancelMovement = v;
		}, true);
	}

	private void UpdatePushCap()
	{
		_pushingCap = 1f / (float)CfgManager.GetCVar<int>(CVars.NetTickrate) * CfgManager.GetCVar<float>(CCVars.MovementPushingCap);
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		AllEntityQueryEnumerator<MobCollisionComponent> query = ((EntitySystem)this).AllEntityQuery<MobCollisionComponent>();
		EntityUid uid = default(EntityUid);
		MobCollisionComponent comp = default(MobCollisionComponent);
		PhysicsComponent physics = default(PhysicsComponent);
		while (query.MoveNext(ref uid, ref comp))
		{
			if (!comp.Colliding)
			{
				continue;
			}
			comp.BufferAccumulator -= frameTime;
			((EntitySystem)this).DirtyField<MobCollisionComponent>(uid, comp, "BufferAccumulator", (MetaDataComponent)null);
			Vector2 direction = comp.Direction;
			if (comp.BufferAccumulator <= 0f)
			{
				SetColliding(Entity<MobCollisionComponent>.op_Implicit((uid, comp)), value: false, 1f);
			}
			else if (direction != Vector2.Zero && PhysicsQuery.TryComp(uid, ref physics))
			{
				if (direction.Length() > _pushingCap)
				{
					direction = Vector2Helpers.Normalized(direction) * _pushingCap;
				}
				Physics.ApplyLinearImpulse(uid, direction * physics.Mass, (FixturesComponent)null, physics);
				comp.Direction = Vector2.Zero;
				((EntitySystem)this).DirtyField<MobCollisionComponent>(uid, comp, "Direction", (MetaDataComponent)null);
			}
		}
	}

	private void OnMoveModifier(Entity<MobCollisionComponent> ent, ref RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Colliding)
		{
			args.ModifySpeed(ent.Comp.SpeedModifier);
		}
	}

	private void SetColliding(Entity<MobCollisionComponent> entity, bool value, float speedMod)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		if (value)
		{
			entity.Comp.BufferAccumulator = 0.2f;
			((EntitySystem)this).DirtyField<MobCollisionComponent>(entity.Owner, entity.Comp, "BufferAccumulator", (MetaDataComponent)null);
		}
		if (entity.Comp.Colliding != value)
		{
			entity.Comp.Colliding = value;
			((EntitySystem)this).DirtyField<MobCollisionComponent>(entity.Owner, entity.Comp, "Colliding", (MetaDataComponent)null);
		}
		if (!entity.Comp.SpeedModifier.Equals(speedMod))
		{
			entity.Comp.SpeedModifier = speedMod;
			_moveMod.RefreshMovementSpeedModifiers(entity.Owner);
			((EntitySystem)this).DirtyField<MobCollisionComponent>(entity.Owner, entity.Comp, "SpeedModifier", (MetaDataComponent)null);
		}
	}

	private void OnCollision(MobCollisionMessage msg, EntitySessionEventArgs args)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? player = ((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity;
		MobCollisionComponent comp = default(MobCollisionComponent);
		if (!MobQuery.TryComp(player, ref comp))
		{
			return;
		}
		TransformComponent xform = ((EntitySystem)this).Transform(player.Value);
		EntityUid parentUid = xform.ParentUid;
		EntityUid? gridUid = xform.GridUid;
		if (!gridUid.HasValue || parentUid != gridUid.GetValueOrDefault())
		{
			parentUid = xform.ParentUid;
			gridUid = xform.MapUid;
			if (!gridUid.HasValue || parentUid != gridUid.GetValueOrDefault())
			{
				return;
			}
		}
		Vector2 direction = msg.Direction;
		MoveMob(Entity<MobCollisionComponent, TransformComponent>.op_Implicit((player.Value, comp, xform)), direction, msg.SpeedModifier);
	}

	protected void MoveMob(Entity<MobCollisionComponent, TransformComponent> entity, Vector2 direction, float speedMod)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		bool pushing = true;
		if (direction.LengthSquared() < _minimumPushSquared)
		{
			pushing = false;
			direction = Vector2.Zero;
			speedMod = 1f;
		}
		else if (float.IsNaN(direction.X) || float.IsNaN(direction.Y))
		{
			direction = Vector2.Zero;
		}
		speedMod = Math.Clamp(speedMod, 0f, 1f);
		SetColliding(Entity<MobCollisionComponent, TransformComponent>.op_Implicit(entity), pushing, speedMod);
		if (!(direction == entity.Comp1.Direction))
		{
			entity.Comp1.Direction = direction;
			((EntitySystem)this).DirtyField<MobCollisionComponent>(entity.Owner, entity.Comp1, "Direction", (MetaDataComponent)null);
		}
	}

	protected bool HandleCollisions(Entity<MobCollisionComponent, PhysicsComponent> entity, float frameTime)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		PhysicsComponent physics = entity.Comp2;
		if (physics.ContactCount == 0)
		{
			return false;
		}
		Vector2 ourVelocity = entity.Comp2.LinearVelocity;
		if (ourVelocity == Vector2.Zero && !CfgManager.GetCVar<bool>(CCVars.MovementPushingStatic))
		{
			return false;
		}
		TransformComponent xform = ((EntitySystem)this).Transform(entity.Owner);
		EntityUid parentUid = xform.ParentUid;
		EntityUid? gridUid = xform.GridUid;
		if (!gridUid.HasValue || parentUid != gridUid.GetValueOrDefault())
		{
			parentUid = xform.ParentUid;
			gridUid = xform.MapUid;
			if (!gridUid.HasValue || parentUid != gridUid.GetValueOrDefault())
			{
				return false;
			}
		}
		AttemptMobCollideEvent ev = default(AttemptMobCollideEvent);
		((EntitySystem)this).RaiseLocalEvent<AttemptMobCollideEvent>(entity.Owner, ref ev, false);
		if (ev.Cancelled)
		{
			return false;
		}
		var (worldPos, worldRot) = _xformSystem.GetWorldPositionRotation(xform);
		Transform ourTransform = default(Transform);
		((Transform)(ref ourTransform))._002Ector(worldPos, worldRot);
		ContactEnumerator contacts = Physics.GetContacts(Entity<FixturesComponent>.op_Implicit(entity.Owner), false);
		Vector2 direction = Vector2.Zero;
		int contactCount = 0;
		float ourMass = physics.FixturesMass;
		float speedMod = 1f;
		Vector2 cancellableDirection = Vector2.Zero;
		bool userIsXeno = _xenoQuery.HasComp(Entity<MobCollisionComponent, PhysicsComponent>.op_Implicit(entity));
		RMCSizes userSize = RMCSizes.Small;
		if (userIsXeno)
		{
			_rmcSizeStun.TryGetSize(Entity<MobCollisionComponent, PhysicsComponent>.op_Implicit(entity), out userSize);
		}
		Contact contact = default(Contact);
		MobCollisionComponent otherComp = default(MobCollisionComponent);
		PhysicsComponent otherPhysics = default(PhysicsComponent);
		RMCMobCollisionMassComponent otherCollision = default(RMCMobCollisionMassComponent);
		while (((ContactEnumerator)(ref contacts)).MoveNext(ref contact))
		{
			if (!contact.IsTouching || contact.OurFixture(entity.Owner).Item1 != entity.Comp1.FixtureId)
			{
				continue;
			}
			EntityUid other = contact.OtherEnt(entity.Owner);
			if (!MobQuery.TryComp(other, ref otherComp) || !PhysicsQuery.TryComp(other, ref otherPhysics) || Vector2.Dot(ourVelocity, otherPhysics.LinearVelocity) < _pushingDotProduct)
			{
				continue;
			}
			AttemptMobTargetCollideEvent targetEv = new AttemptMobTargetCollideEvent(Entity<MobCollisionComponent, PhysicsComponent>.op_Implicit(entity));
			((EntitySystem)this).RaiseLocalEvent<AttemptMobTargetCollideEvent>(other, ref targetEv, false);
			if (targetEv.Cancelled)
			{
				continue;
			}
			Transform otherTransform = Physics.GetPhysicsTransform(other, (TransformComponent)null);
			Vector2 diff = ourTransform.Position - otherTransform.Position;
			if (diff == Vector2.Zero)
			{
				diff = _random.NextVector2(0.01f);
			}
			float penDepth = Math.Clamp(_penCapSubtract - diff.Length(), 0f, _penCap);
			Vector2 mobMovement = penDepth * Vector2Helpers.Normalized(diff) * (entity.Comp1.Strength + otherComp.Strength);
			if (_massDiffCap > 0f)
			{
				float mass = otherPhysics.FixturesMass;
				if (_rmcMobCollisionMassQuery.TryComp(other, ref otherCollision))
				{
					mass = otherCollision.Mass;
				}
				float modifier = Math.Clamp(mass / ourMass, 1f / _massDiffCap, _massDiffCap);
				mobMovement *= modifier;
				float speedReduction = 1f - entity.Comp1.MinimumSpeedModifier;
				speedReduction /= _penCap / penDepth;
				speedMod = MathF.Min(Math.Clamp(1f - speedReduction * modifier, entity.Comp1.MinimumSpeedModifier, 1f), 1f);
			}
			direction += mobMovement;
			contactCount++;
			if (_bigXenosCancelMovement && userIsXeno && (int)userSize >= 5 && _xenoQuery.HasComp(other) && _rmcSizeStun.TryGetSize(other, out var otherSize) && (int)otherSize < 5)
			{
				cancellableDirection += mobMovement;
			}
		}
		if (direction == Vector2.Zero)
		{
			return contactCount > 0;
		}
		if (cancellableDirection != Vector2.Zero)
		{
			direction -= cancellableDirection;
		}
		direction *= frameTime;
		RaiseCollisionEvent(entity.Owner, direction, speedMod);
		return true;
	}

	protected abstract void RaiseCollisionEvent(EntityUid uid, Vector2 direction, float speedmodifier);
}
