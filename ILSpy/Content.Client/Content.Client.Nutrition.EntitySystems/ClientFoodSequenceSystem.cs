using System;
using System.Numerics;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Robust.Client.GameObjects;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Nutrition.EntitySystems;

public sealed class ClientFoodSequenceSystem : SharedFoodSequenceSystem
{
	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<FoodSequenceStartPointComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<FoodSequenceStartPointComponent, AfterAutoHandleStateEvent>)OnHandleState, (Type[])null, (Type[])null);
	}

	private void OnHandleState(Entity<FoodSequenceStartPointComponent> start, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent sprite = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(Entity<FoodSequenceStartPointComponent>.op_Implicit(start), ref sprite))
		{
			UpdateFoodVisuals(start, sprite);
		}
	}

	private void UpdateFoodVisuals(Entity<FoodSequenceStartPointComponent> start, SpriteComponent? sprite = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<SpriteComponent>(Entity<FoodSequenceStartPointComponent>.op_Implicit(start), ref sprite, false))
		{
			return;
		}
		foreach (string revealedLayer in start.Comp.RevealedLayers)
		{
			_sprite.RemoveLayer(Entity<SpriteComponent>.op_Implicit((start.Owner, sprite)), revealedLayer, true);
		}
		start.Comp.RevealedLayers.Clear();
		int num = 0;
		int num2 = default(int);
		foreach (FoodSequenceVisualLayer foodLayer in start.Comp.FoodLayers)
		{
			if (foodLayer.Sprite != null)
			{
				string text = $"food-layer-{num}";
				start.Comp.RevealedLayers.Add(text);
				_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((start.Owner, sprite)), start.Comp.TargetLayerMap, ref num2, false);
				if (start.Comp.InverseLayers)
				{
					num2++;
				}
				_sprite.AddBlankLayer(Entity<SpriteComponent>.op_Implicit((start.Owner, sprite)), (int?)num2);
				_sprite.LayerMapSet(Entity<SpriteComponent>.op_Implicit((start.Owner, sprite)), text, num2);
				_sprite.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((start.Owner, sprite)), num2, foodLayer.Sprite);
				_sprite.LayerSetScale(Entity<SpriteComponent>.op_Implicit((start.Owner, sprite)), num2, foodLayer.Scale);
				Vector2 startPosition = start.Comp.StartPosition;
				startPosition += start.Comp.Offset * num + foodLayer.LocalOffset;
				_sprite.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((start.Owner, sprite)), num2, startPosition);
				num++;
			}
		}
	}
}
