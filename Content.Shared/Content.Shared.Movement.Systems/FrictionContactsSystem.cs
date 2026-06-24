using System;
using System.Collections.Generic;
using Content.Shared.Gravity;
using Content.Shared.Movement.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;

namespace Content.Shared.Movement.Systems;

public sealed class FrictionContactsSystem : EntitySystem
{
	[Dependency]
	private SharedGravitySystem _gravity;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private MovementSpeedModifierSystem _speedModifierSystem;

	private readonly HashSet<EntityUid> _toUpdate = new HashSet<EntityUid>();

	private readonly HashSet<EntityUid> _toRemove = new HashSet<EntityUid>();

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<FrictionContactsComponent, StartCollideEvent>((ComponentEventRefHandler<FrictionContactsComponent, StartCollideEvent>)OnEntityEnter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FrictionContactsComponent, EndCollideEvent>((ComponentEventRefHandler<FrictionContactsComponent, EndCollideEvent>)OnEntityExit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FrictionModifiedByContactComponent, RefreshFrictionModifiersEvent>((EntityEventRefHandler<FrictionModifiedByContactComponent, RefreshFrictionModifiersEvent>)OnRefreshFrictionModifiers, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FrictionContactsComponent, ComponentShutdown>((ComponentEventHandler<FrictionContactsComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).UpdatesAfter.Add(typeof(SharedPhysicsSystem));
	}

	public override void Update(float frameTime)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		_toRemove.Clear();
		foreach (EntityUid ent in _toUpdate)
		{
			_speedModifierSystem.RefreshFrictionModifiers(ent);
		}
		foreach (EntityUid ent2 in _toRemove)
		{
			((EntitySystem)this).RemComp<FrictionModifiedByContactComponent>(ent2);
		}
		_toUpdate.Clear();
	}

	public void ChangeFrictionModifiers(EntityUid uid, float friction, FrictionContactsComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		ChangeFrictionModifiers(uid, friction, null, null, component);
	}

	public void ChangeFrictionModifiers(EntityUid uid, float mobFriction, float? mobFrictionNoInput, float? acceleration, FrictionContactsComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<FrictionContactsComponent>(uid, ref component, true))
		{
			component.MobFriction = mobFriction;
			component.MobFrictionNoInput = mobFrictionNoInput;
			if (acceleration.HasValue)
			{
				component.MobAcceleration = acceleration.Value;
			}
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			_toUpdate.UnionWith(_physics.GetContactingEntities(uid, (PhysicsComponent)null, false));
		}
	}

	private void OnShutdown(EntityUid uid, FrictionContactsComponent component, ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		PhysicsComponent phys = default(PhysicsComponent);
		if (((EntitySystem)this).TryComp<PhysicsComponent>(uid, ref phys))
		{
			_toUpdate.UnionWith(_physics.GetContactingEntities(uid, phys, false));
		}
	}

	private void OnRefreshFrictionModifiers(Entity<FrictionModifiedByContactComponent> entity, ref RefreshFrictionModifiersEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Invalid comparison between Unknown and I4
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		PhysicsComponent physicsComponent = default(PhysicsComponent);
		if (!((EntitySystem)this).TryComp<PhysicsComponent>(Entity<FrictionModifiedByContactComponent>.op_Implicit(entity), ref physicsComponent))
		{
			return;
		}
		float friction = 0f;
		float frictionNoInput = 0f;
		float acceleration = 0f;
		bool isAirborne = (int)physicsComponent.BodyStatus == 1 || _gravity.IsWeightless(Entity<FrictionModifiedByContactComponent>.op_Implicit(entity), physicsComponent);
		bool remove = true;
		int entries = 0;
		FrictionContactsComponent contacts = default(FrictionContactsComponent);
		foreach (EntityUid ent in _physics.GetContactingEntities(Entity<FrictionModifiedByContactComponent>.op_Implicit(entity), physicsComponent, false))
		{
			if (((EntitySystem)this).TryComp<FrictionContactsComponent>(ent, ref contacts) && (!isAirborne || contacts.AffectAirborne))
			{
				friction += contacts.MobFriction;
				frictionNoInput += contacts.MobFrictionNoInput ?? contacts.MobFriction;
				acceleration += contacts.MobAcceleration;
				remove = false;
				entries++;
			}
		}
		if (entries > 0)
		{
			if (!MathHelper.CloseTo(friction, (float)entries, 1E-07f) || !MathHelper.CloseTo(frictionNoInput, (float)entries, 1E-07f))
			{
				friction /= (float)entries;
				frictionNoInput /= (float)entries;
				args.ModifyFriction(friction, frictionNoInput);
			}
			if (!MathHelper.CloseTo(acceleration, (float)entries, 1E-07f))
			{
				acceleration /= (float)entries;
				args.ModifyAcceleration(acceleration);
			}
		}
		if (remove)
		{
			_toRemove.Add(Entity<FrictionModifiedByContactComponent>.op_Implicit(entity));
		}
	}

	private void OnEntityExit(EntityUid uid, FrictionContactsComponent component, ref EndCollideEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		EntityUid otherUid = args.OtherEntity;
		_toUpdate.Add(otherUid);
	}

	private void OnEntityEnter(EntityUid uid, FrictionContactsComponent component, ref StartCollideEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		AddModifiedEntity(args.OtherEntity);
	}

	public void AddModifiedEntity(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<MovementSpeedModifierComponent>(uid))
		{
			((EntitySystem)this).EnsureComp<FrictionModifiedByContactComponent>(uid);
			_toUpdate.Add(uid);
		}
	}
}
