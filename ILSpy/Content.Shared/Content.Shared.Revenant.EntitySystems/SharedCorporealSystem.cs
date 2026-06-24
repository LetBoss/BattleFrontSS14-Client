using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Movement.Systems;
using Content.Shared.Revenant.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;

namespace Content.Shared.Revenant.EntitySystems;

public abstract class SharedCorporealSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private MovementSpeedModifierSystem _movement;

	[Dependency]
	private SharedPhysicsSystem _physics;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<CorporealComponent, ComponentStartup>((ComponentEventHandler<CorporealComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CorporealComponent, ComponentShutdown>((ComponentEventHandler<CorporealComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CorporealComponent, RefreshMovementSpeedModifiersEvent>((ComponentEventHandler<CorporealComponent, RefreshMovementSpeedModifiersEvent>)OnRefresh, (Type[])null, (Type[])null);
	}

	private void OnRefresh(EntityUid uid, CorporealComponent component, RefreshMovementSpeedModifiersEvent args)
	{
		args.ModifySpeed(component.MovementSpeedDebuff, component.MovementSpeedDebuff);
	}

	public virtual void OnStartup(EntityUid uid, CorporealComponent component, ComponentStartup args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		_appearance.SetData(uid, (Enum)RevenantVisuals.Corporeal, (object)true, (AppearanceComponent)null);
		FixturesComponent fixtures = default(FixturesComponent);
		if (((EntitySystem)this).TryComp<FixturesComponent>(uid, ref fixtures) && fixtures.FixtureCount >= 1)
		{
			KeyValuePair<string, Fixture> fixture = fixtures.Fixtures.First();
			_physics.SetCollisionMask(uid, fixture.Key, fixture.Value, 50, fixtures, (PhysicsComponent)null);
			_physics.SetCollisionLayer(uid, fixture.Key, fixture.Value, 65, fixtures, (PhysicsComponent)null);
		}
		_movement.RefreshMovementSpeedModifiers(uid);
	}

	public virtual void OnShutdown(EntityUid uid, CorporealComponent component, ComponentShutdown args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		_appearance.SetData(uid, (Enum)RevenantVisuals.Corporeal, (object)false, (AppearanceComponent)null);
		FixturesComponent fixtures = default(FixturesComponent);
		if (((EntitySystem)this).TryComp<FixturesComponent>(uid, ref fixtures) && fixtures.FixtureCount >= 1)
		{
			KeyValuePair<string, Fixture> fixture = fixtures.Fixtures.First();
			_physics.SetCollisionMask(uid, fixture.Key, fixture.Value, 32, fixtures, (PhysicsComponent)null);
			_physics.SetCollisionLayer(uid, fixture.Key, fixture.Value, 0, fixtures, (PhysicsComponent)null);
		}
		component.MovementSpeedDebuff = 1f;
		_movement.RefreshMovementSpeedModifiers(uid);
	}
}
