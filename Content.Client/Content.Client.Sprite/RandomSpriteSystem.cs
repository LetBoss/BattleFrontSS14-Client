using System;
using System.Collections.Generic;
using Content.Client.Clothing;
using Content.Shared.Clothing.Components;
using Content.Shared.Sprite;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Reflection;

namespace Content.Client.Sprite;

public sealed class RandomSpriteSystem : SharedRandomSpriteSystem
{
	[Dependency]
	private IReflectionManager _reflection;

	[Dependency]
	private ClientClothingSystem _clothing;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RandomSpriteComponent, ComponentHandleState>((ComponentEventRefHandler<RandomSpriteComponent, ComponentHandleState>)OnHandleState, (Type[])null, (Type[])null);
	}

	private void OnHandleState(EntityUid uid, RandomSpriteComponent component, ref ComponentHandleState args)
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		if (!(((ComponentHandleState)(ref args)).Current is RandomSpriteColorComponentState randomSpriteColorComponentState) || randomSpriteColorComponentState.Selected.Equals(component.Selected))
		{
			return;
		}
		component.Selected.Clear();
		component.Selected.EnsureCapacity(randomSpriteColorComponentState.Selected.Count);
		foreach (KeyValuePair<string, (string, Color?)> item in randomSpriteColorComponentState.Selected)
		{
			component.Selected.Add(item.Key, item.Value);
		}
		UpdateSpriteComponentAppearance(uid, component);
		UpdateClothingComponentAppearance(uid, component);
	}

	private void UpdateClothingComponentAppearance(EntityUid uid, RandomSpriteComponent component, ClothingComponent? clothing = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ClothingComponent>(uid, ref clothing, false))
		{
			return;
		}
		foreach (KeyValuePair<string, List<PrototypeLayerData>> clothingVisual in clothing.ClothingVisuals)
		{
			foreach (KeyValuePair<string, (string, Color?)> item in component.Selected)
			{
				_clothing.SetLayerColor(clothing, clothingVisual.Key, item.Key, item.Value.Item2);
				_clothing.SetLayerState(clothing, clothingVisual.Key, item.Key, item.Value.Item1);
			}
		}
	}

	private void UpdateSpriteComponentAppearance(EntityUid uid, RandomSpriteComponent component, SpriteComponent? sprite = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<SpriteComponent>(uid, ref sprite, false))
		{
			return;
		}
		Enum obj = default(Enum);
		int result = default(int);
		foreach (KeyValuePair<string, (string, Color?)> item in component.Selected)
		{
			if (_reflection.TryParseEnumReference(item.Key, ref obj, true))
			{
				if (!_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), obj, ref result, true))
				{
					continue;
				}
			}
			else if (!_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), item.Key, ref result, false))
			{
				string key = item.Key;
				if (key == null || !int.TryParse(key, out result))
				{
					((EntitySystem)this).Log.Error($"Invalid key `{item.Key}` for entity with random sprite {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))}");
					continue;
				}
			}
			_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), result, StateId.op_Implicit(item.Value.Item1));
			_sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, sprite)), result, (Color)(((_003F?)item.Value.Item2) ?? Color.White));
		}
	}
}
