using System;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Robust.Shared.GameObjects;

namespace Content.Shared.Movement.Systems;

public abstract class SharedSpriteMovementSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SpriteMovementComponent, SpriteMoveEvent>((EntityEventRefHandler<SpriteMovementComponent, SpriteMoveEvent>)OnSpriteMoveInput, (Type[])null, (Type[])null);
	}

	private void OnSpriteMoveInput(Entity<SpriteMovementComponent> ent, ref SpriteMoveEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.IsMoving != args.IsMoving)
		{
			ent.Comp.IsMoving = args.IsMoving;
			((EntitySystem)this).Dirty<SpriteMovementComponent>(ent, (MetaDataComponent)null);
		}
	}
}
