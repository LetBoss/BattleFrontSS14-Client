using System;
using Content.Shared.Chemistry.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Chemistry.EntitySystems;

public sealed class PillSystem : EntitySystem
{
	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PillComponent, AfterAutoHandleStateEvent>((ComponentEventRefHandler<PillComponent, AfterAutoHandleStateEvent>)OnHandleState, (Type[])null, (Type[])null);
	}

	private void OnHandleState(EntityUid uid, PillComponent component, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		Layer val = default(Layer);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item) && _sprite.TryGetLayer(Entity<SpriteComponent>.op_Implicit((uid, item)), 0, ref val, false))
		{
			_sprite.LayerSetRsiState(val, StateId.op_Implicit($"pill{component.PillType + 1}"), false);
		}
	}
}
