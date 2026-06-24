using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Water;
using Content.Shared.Gravity;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Slippery;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;

namespace Content.Shared.Movement.Systems;

public sealed class SpeedModifierContactsSystem : EntitySystem
{
	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedGravitySystem _gravity;

	[Dependency]
	private MovementSpeedModifierSystem _speedModifierSystem;

	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	[Dependency]
	private RMCWaterSystem _rmcWater;

	private readonly HashSet<EntityUid> _toUpdate = new HashSet<EntityUid>();

	private readonly HashSet<EntityUid> _toRemove = new HashSet<EntityUid>();

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SpeedModifierContactsComponent, StartCollideEvent>((ComponentEventRefHandler<SpeedModifierContactsComponent, StartCollideEvent>)OnEntityEnter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SpeedModifierContactsComponent, EndCollideEvent>((ComponentEventRefHandler<SpeedModifierContactsComponent, EndCollideEvent>)OnEntityExit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SpeedModifiedByContactComponent, RefreshMovementSpeedModifiersEvent>((ComponentEventHandler<SpeedModifiedByContactComponent, RefreshMovementSpeedModifiersEvent>)OnRefreshMovementSpeedModifiers, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SpeedModifierContactsComponent, ComponentShutdown>((ComponentEventHandler<SpeedModifierContactsComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
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
			_speedModifierSystem.RefreshMovementSpeedModifiers(ent);
		}
		foreach (EntityUid ent2 in _toRemove)
		{
			((EntitySystem)this).RemComp<SpeedModifiedByContactComponent>(ent2);
		}
		_toUpdate.Clear();
	}

	public void ChangeSpeedModifiers(EntityUid uid, float speed, SpeedModifierContactsComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		ChangeSpeedModifiers(uid, speed, speed, component);
	}

	public void ChangeSpeedModifiers(EntityUid uid, float walkSpeed, float sprintSpeed, SpeedModifierContactsComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<SpeedModifierContactsComponent>(uid, ref component, true))
		{
			component.WalkSpeedModifier = walkSpeed;
			component.SprintSpeedModifier = sprintSpeed;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			_toUpdate.UnionWith(_physics.GetContactingEntities(uid, (PhysicsComponent)null, false));
		}
	}

	private void OnShutdown(EntityUid uid, SpeedModifierContactsComponent component, ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		PhysicsComponent phys = default(PhysicsComponent);
		if (((EntitySystem)this).TryComp<PhysicsComponent>(uid, ref phys))
		{
			_toUpdate.UnionWith(_physics.GetContactingEntities(uid, phys, false));
		}
	}

	private void OnRefreshMovementSpeedModifiers(EntityUid uid, SpeedModifiedByContactComponent component, RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Invalid comparison between Unknown and I4
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		PhysicsComponent physicsComponent = default(PhysicsComponent);
		if (!((EntitySystem)this).TryComp<PhysicsComponent>(uid, ref physicsComponent))
		{
			return;
		}
		float walkSpeed = 0f;
		float sprintSpeed = 0f;
		bool isAirborne = (int)physicsComponent.BodyStatus == 1 || _gravity.IsWeightless(uid, physicsComponent);
		bool remove = true;
		int entries = 0;
		SpeedModifierContactsComponent slowContactsComponent = default(SpeedModifierContactsComponent);
		foreach (EntityUid ent in _physics.GetContactingEntities(uid, physicsComponent, false))
		{
			bool speedModified = false;
			if (((EntitySystem)this).TryComp<SpeedModifierContactsComponent>(ent, ref slowContactsComponent))
			{
				if (_whitelistSystem.IsWhitelistPass(slowContactsComponent.IgnoreWhitelist, uid) || (isAirborne && !slowContactsComponent.AffectAirborne))
				{
					continue;
				}
				walkSpeed += slowContactsComponent.WalkSpeedModifier;
				sprintSpeed += slowContactsComponent.SprintSpeedModifier;
				speedModified = true;
			}
			if (((EntitySystem)this).HasComp<SlipperyComponent>(ent) && !speedModified)
			{
				GetSlowedOverSlipperyModifierEvent evSlippery = new GetSlowedOverSlipperyModifierEvent();
				((EntitySystem)this).RaiseLocalEvent<GetSlowedOverSlipperyModifierEvent>(uid, ref evSlippery, false);
				if (!MathHelper.CloseTo(evSlippery.SlowdownModifier, 1f, 1E-07f))
				{
					walkSpeed += evSlippery.SlowdownModifier;
					sprintSpeed += evSlippery.SlowdownModifier;
					speedModified = true;
				}
			}
			if (speedModified)
			{
				remove = false;
				entries++;
			}
		}
		if (entries > 0 && (!MathHelper.CloseTo(walkSpeed, (float)entries, 1E-07f) || !MathHelper.CloseTo(sprintSpeed, (float)entries, 1E-07f)))
		{
			walkSpeed /= (float)entries;
			sprintSpeed /= (float)entries;
			GetSpeedModifierContactCapEvent evMax = new GetSpeedModifierContactCapEvent();
			((EntitySystem)this).RaiseLocalEvent<GetSpeedModifierContactCapEvent>(uid, ref evMax, false);
			walkSpeed = MathF.Max(walkSpeed, evMax.MaxWalkSlowdown);
			sprintSpeed = MathF.Max(sprintSpeed, evMax.MaxSprintSlowdown);
			args.ModifySpeed(walkSpeed, sprintSpeed);
		}
		if (remove)
		{
			_toRemove.Add(uid);
		}
	}

	private void OnEntityExit(EntityUid uid, SpeedModifierContactsComponent component, ref EndCollideEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		EntityUid otherUid = args.OtherEntity;
		_toUpdate.Add(otherUid);
	}

	private void OnEntityEnter(EntityUid uid, SpeedModifierContactsComponent component, ref StartCollideEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (_rmcWater.CanCollide(Entity<RMCWaterComponent>.op_Implicit(uid), args.OtherEntity))
		{
			AddModifiedEntity(args.OtherEntity);
		}
	}

	public void AddModifiedEntity(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<MovementSpeedModifierComponent>(uid))
		{
			((EntitySystem)this).EnsureComp<SpeedModifiedByContactComponent>(uid);
			_toUpdate.Add(uid);
		}
	}
}
