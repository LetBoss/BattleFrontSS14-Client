using System;
using Content.Shared.Dice;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Dice;

public sealed class DiceSystem : SharedDiceSystem
{
	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DiceComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<DiceComponent, AfterAutoHandleStateEvent>)OnDiceAfterHandleState, (Type[])null, (Type[])null);
	}

	private void OnDiceAfterHandleState(Entity<DiceComponent> entity, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(Entity<DiceComponent>.op_Implicit(entity), ref item))
		{
			string name = _sprite.LayerGetRsiState(Entity<SpriteComponent>.op_Implicit((entity.Owner, item)), 0).Name;
			if (name != null)
			{
				string value = name.Substring(0, name.IndexOf('_'));
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entity.Owner, item)), 0, StateId.op_Implicit($"{value}_{entity.Comp.CurrentValue}"));
			}
		}
	}
}
