using System;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;

namespace Content.Shared.Weapons.Ranged.Systems;

public abstract class SharedFlyBySoundSystem : EntitySystem
{
	[Dependency]
	private FixtureSystem _fixtures;

	public const string FlyByFixture = "fly-by";

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<FlyBySoundComponent, ComponentStartup>((ComponentEventHandler<FlyBySoundComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FlyBySoundComponent, ComponentShutdown>((ComponentEventHandler<FlyBySoundComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
	}

	private void OnStartup(EntityUid uid, FlyBySoundComponent component, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		PhysicsComponent body = default(PhysicsComponent);
		if (((EntitySystem)this).TryComp<PhysicsComponent>(uid, ref body))
		{
			new PhysShapeCircle(component.Range);
		}
	}

	private void OnShutdown(EntityUid uid, FlyBySoundComponent component, ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Invalid comparison between Unknown and I4
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		PhysicsComponent body = default(PhysicsComponent);
		if (((EntitySystem)this).TryComp<PhysicsComponent>(uid, ref body) && (int)((EntitySystem)this).MetaData(uid).EntityLifeStage < 4)
		{
			_fixtures.DestroyFixture(uid, "fly-by", true, body, (FixturesComponent)null, (TransformComponent)null);
		}
	}
}
