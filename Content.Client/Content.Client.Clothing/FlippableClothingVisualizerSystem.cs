using System;
using Content.Shared.Clothing;
using Content.Shared.Clothing.Components;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Foldable;
using Content.Shared.Item;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Clothing;

public sealed class FlippableClothingVisualizerSystem : VisualizerSystem<FlippableClothingVisualsComponent>
{
	[Dependency]
	private SharedItemSystem _itemSys;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<FlippableClothingVisualsComponent, GetEquipmentVisualsEvent>((EntityEventRefHandler<FlippableClothingVisualsComponent, GetEquipmentVisualsEvent>)OnGetVisuals, (Type[])null, new Type[1] { typeof(ClothingSystem) });
		((EntitySystem)this).SubscribeLocalEvent<FlippableClothingVisualsComponent, FoldedEvent>((EntityEventRefHandler<FlippableClothingVisualsComponent, FoldedEvent>)OnFolded, (Type[])null, (Type[])null);
	}

	private void OnFolded(Entity<FlippableClothingVisualsComponent> ent, ref FoldedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_itemSys.VisualsChanged(Entity<FlippableClothingVisualsComponent>.op_Implicit(ent));
	}

	private void OnGetVisuals(Entity<FlippableClothingVisualsComponent> ent, ref GetEquipmentVisualsEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		ClothingComponent clothingComponent = default(ClothingComponent);
		bool flag = default(bool);
		int num = default(int);
		if (!((EntitySystem)this).TryComp<SpriteComponent>(Entity<FlippableClothingVisualsComponent>.op_Implicit(ent), ref val) || !((EntitySystem)this).TryComp<ClothingComponent>(Entity<FlippableClothingVisualsComponent>.op_Implicit(ent), ref clothingComponent) || clothingComponent.MappedLayer == null || !((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(Entity<FlippableClothingVisualsComponent>.op_Implicit(ent), (Enum)FoldableSystem.FoldedVisuals.State, ref flag, (AppearanceComponent)null) || !base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((ent.Owner, val)), flag ? ent.Comp.FoldingLayer : ent.Comp.UnfoldingLayer, ref num, false))
		{
			return;
		}
		ISpriteLayer val2 = val[num];
		foreach (var layer in args.Layers)
		{
			if (!(layer.Item1 != clothingComponent.MappedLayer))
			{
				layer.Item2.Scale = val2.Scale;
			}
		}
	}
}
