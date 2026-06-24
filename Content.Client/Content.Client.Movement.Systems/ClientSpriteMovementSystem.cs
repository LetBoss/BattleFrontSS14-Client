using System;
using System.Collections.Generic;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Robust.Client.GameObjects;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Movement.Systems;

public sealed class ClientSpriteMovementSystem : SharedSpriteMovementSystem
{
	[Dependency]
	private SpriteSystem _sprite;

	private EntityQuery<SpriteComponent> _spriteQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize();
		_spriteQuery = ((EntitySystem)this).GetEntityQuery<SpriteComponent>();
		((EntitySystem)this).SubscribeLocalEvent<SpriteMovementComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<SpriteMovementComponent, AfterAutoHandleStateEvent>)OnAfterAutoHandleState, (Type[])null, (Type[])null);
	}

	private void OnAfterAutoHandleState(Entity<SpriteMovementComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (!_spriteQuery.TryGetComponent(Entity<SpriteMovementComponent>.op_Implicit(ent), ref item))
		{
			return;
		}
		int num = default(int);
		foreach (var (text2, val2) in ent.Comp.IsMoving ? ent.Comp.MovementLayers : ent.Comp.NoMovementLayers)
		{
			if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((ent.Owner, item)), text2, ref num, false))
			{
				_sprite.LayerSetData(Entity<SpriteComponent>.op_Implicit((ent.Owner, item)), text2, val2);
			}
		}
	}
}
