using System;
using Content.Shared.Atmos.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Atmos.EntitySystems;

public sealed class ExtinguishableSetCollisionWakeSystem : EntitySystem
{
	[Dependency]
	private CollisionWakeSystem _collisionWake;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ExtinguishableSetCollisionWakeComponent, ExtinguishedEvent>((EntityEventRefHandler<ExtinguishableSetCollisionWakeComponent, ExtinguishedEvent>)HandleExtinguished, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ExtinguishableSetCollisionWakeComponent, IgnitedEvent>((EntityEventRefHandler<ExtinguishableSetCollisionWakeComponent, IgnitedEvent>)HandleIgnited, (Type[])null, (Type[])null);
	}

	private void HandleExtinguished(Entity<ExtinguishableSetCollisionWakeComponent> ent, ref ExtinguishedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_collisionWake.SetEnabled(Entity<ExtinguishableSetCollisionWakeComponent>.op_Implicit(ent), true, (CollisionWakeComponent)null);
	}

	private void HandleIgnited(Entity<ExtinguishableSetCollisionWakeComponent> ent, ref IgnitedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_collisionWake.SetEnabled(Entity<ExtinguishableSetCollisionWakeComponent>.op_Implicit(ent), false, (CollisionWakeComponent)null);
	}
}
