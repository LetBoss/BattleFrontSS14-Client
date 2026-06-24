using System;
using System.Collections.Generic;
using Content.Shared.Rounding;
using Content.Shared.Stacks;
using Content.Shared.Storage.Components;
using Content.Shared.Storage.EntitySystems;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Storage.Systems;

public sealed class ItemCounterSystem : SharedItemCounterSystem
{
	[Dependency]
	private AppearanceSystem _appearanceSystem;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ItemCounterComponent, AppearanceChangeEvent>((ComponentEventRefHandler<ItemCounterComponent, AppearanceChangeEvent>)OnAppearanceChange, (Type[])null, (Type[])null);
	}

	private void OnAppearanceChange(EntityUid uid, ItemCounterComponent comp, ref AppearanceChangeEvent args)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		int count = default(int);
		if (args.Sprite != null && comp.LayerStates.Count >= 1 && ((SharedAppearanceSystem)_appearanceSystem).TryGetData<int>(uid, (Enum)StackVisuals.Actual, ref count, args.Component))
		{
			int count2 = default(int);
			if (!((SharedAppearanceSystem)_appearanceSystem).TryGetData<int>(uid, (Enum)StackVisuals.MaxCount, ref count2, args.Component))
			{
				count2 = comp.LayerStates.Count;
			}
			bool hide = default(bool);
			if (!((SharedAppearanceSystem)_appearanceSystem).TryGetData<bool>(uid, (Enum)StackVisuals.Hide, ref hide, args.Component))
			{
				hide = false;
			}
			if (comp.IsComposite)
			{
				ProcessCompositeSprite(uid, count, count2, comp.LayerStates, hide, args.Sprite);
			}
			else
			{
				ProcessOpaqueSprite(uid, comp.BaseLayer, count, count2, comp.LayerStates, hide, args.Sprite);
			}
		}
	}

	public void ProcessOpaqueSprite(EntityUid uid, string layer, int count, int maxCount, List<string> states, bool hide = false, SpriteComponent? sprite = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		int num = default(int);
		if (((EntitySystem)this).Resolve<SpriteComponent>(uid, ref sprite, true) && _sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), layer, ref num, true))
		{
			int index = ContentHelpers.RoundToEqualLevels(count, maxCount, states.Count);
			_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, StateId.op_Implicit(states[index]));
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, !hide);
		}
	}

	public void ProcessCompositeSprite(EntityUid uid, int count, int maxCount, List<string> layers, bool hide = false, SpriteComponent? sprite = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<SpriteComponent>(uid, ref sprite, true))
		{
			int num = ContentHelpers.RoundToNearestLevels(count, maxCount, layers.Count);
			for (int i = 0; i < layers.Count; i++)
			{
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), layers[i], !hide && i < num);
			}
		}
	}

	protected override int? GetCount(ContainerModifiedMessage msg, ItemCounterComponent itemCounter)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		int value = default(int);
		if (((SharedAppearanceSystem)_appearanceSystem).TryGetData<int>(msg.Container.Owner, (Enum)StackVisuals.Actual, ref value, (AppearanceComponent)null))
		{
			return value;
		}
		return null;
	}
}
