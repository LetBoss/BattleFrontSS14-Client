using System;
using Content.Shared._RMC14.Xenonids.Fruit.Components;
using Content.Shared._RMC14.Xenonids.Fruit.Events;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Xenonids.Fruit;

public sealed class XenoFruitVisualizerSystem : EntitySystem
{
	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<XenoFruitComponent, ComponentStartup>((EntityEventRefHandler<XenoFruitComponent, ComponentStartup>)SetVisuals, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFruitComponent, XenoFruitStateChangedEvent>((EntityEventRefHandler<XenoFruitComponent, XenoFruitStateChangedEvent>)SetVisuals, (Type[])null, (Type[])null);
	}

	private void SetVisuals<T>(Entity<XenoFruitComponent> ent, ref T args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(Entity<XenoFruitComponent>.op_Implicit(ent), ref item))
		{
			string text = ent.Comp.State switch
			{
				XenoFruitState.Item => ent.Comp.ItemState, 
				XenoFruitState.Growing => ent.Comp.GrowingState, 
				XenoFruitState.Grown => ent.Comp.GrownState, 
				XenoFruitState.Eaten => ent.Comp.EatenState, 
				_ => null, 
			};
			if (!string.IsNullOrWhiteSpace(text))
			{
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((ent.Owner, item)), (Enum)XenoFruitLayers.Base, StateId.op_Implicit(text));
			}
		}
	}
}
