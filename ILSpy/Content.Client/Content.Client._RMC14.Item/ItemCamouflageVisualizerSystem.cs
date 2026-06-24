using System;
using System.Collections.Generic;
using Content.Client._RMC14.Attachable.Components;
using Content.Client._RMC14.Attachable.Systems;
using Content.Client.Clothing;
using Content.Client.Items.Systems;
using Content.Shared._RMC14.Attachable.Components;
using Content.Shared._RMC14.Item;
using Content.Shared.Clothing;
using Content.Shared.Clothing.Components;
using Content.Shared.Hands;
using Content.Shared.Inventory;
using Content.Shared.Item;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.Item;

public sealed class ItemCamouflageVisualizerSystem : VisualizerSystem<ItemCamouflageComponent>
{
	[Dependency]
	private AttachableHolderVisualsSystem _attachableHolderVisuals;

	[Dependency]
	private ContainerSystem _container;

	[Dependency]
	private ItemSystem _item;

	[Dependency]
	private IResourceCache _resource;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ItemCamouflageComponent, GetInhandVisualsEvent>((ComponentEventHandler<ItemCamouflageComponent, GetInhandVisualsEvent>)OnGetInhandVisuals, (Type[])null, new Type[1] { typeof(ItemSystem) });
		((EntitySystem)this).SubscribeLocalEvent<ItemCamouflageComponent, GetEquipmentVisualsEvent>((ComponentEventHandler<ItemCamouflageComponent, GetEquipmentVisualsEvent>)OnGetClothingVisuals, (Type[])null, new Type[1] { typeof(ClientClothingSystem) });
	}

	private void OnGetInhandVisuals(EntityUid uid, ItemCamouflageComponent camoComp, GetInhandVisualsEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		AppearanceComponent val = default(AppearanceComponent);
		if (!((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref val))
		{
			return;
		}
		CamouflageType key = default(CamouflageType);
		((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<CamouflageType>(uid, (Enum)ItemCamouflageVisuals.Camo, ref key, val);
		if (camoComp.Colors == null)
		{
			return;
		}
		camoComp.Colors.TryGetValue(key, out var value);
		PrototypeLayerData val2 = new PrototypeLayerData();
		foreach (var layer in args.Layers)
		{
			string item = layer.Item1;
			PrototypeLayerData item2 = layer.Item2;
			val2.RsiPath = item2.RsiPath;
			val2.State = item + "-color";
			val2.MapKeys = new HashSet<string> { item + "-color" };
			val2.Color = value;
		}
		if (val2.State != null)
		{
			args.Layers.Add((val2.State, val2));
		}
	}

	private void OnGetClothingVisuals(EntityUid uid, ItemCamouflageComponent camoComp, GetEquipmentVisualsEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		InventoryComponent inventoryComponent = default(InventoryComponent);
		ClothingComponent clothingComponent = default(ClothingComponent);
		if (!((EntitySystem)this).TryComp<InventoryComponent>(args.Equipee, ref inventoryComponent) || !((EntitySystem)this).TryComp<ClothingComponent>(uid, ref clothingComponent))
		{
			return;
		}
		string speciesId = inventoryComponent.SpeciesId;
		AppearanceComponent val = default(AppearanceComponent);
		if (!((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref val))
		{
			return;
		}
		CamouflageType key = default(CamouflageType);
		((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<CamouflageType>(uid, (Enum)ItemCamouflageVisuals.Camo, ref key, val);
		if (camoComp.Colors == null)
		{
			return;
		}
		camoComp.Colors.TryGetValue(key, out var value);
		PrototypeLayerData val2 = new PrototypeLayerData();
		State val3 = default(State);
		foreach (var layer in args.Layers)
		{
			PrototypeLayerData item = layer.Item2;
			if (item.RsiPath != null)
			{
				RSI rSI = _resource.GetResource<RSIResource>(SpriteSpecifierSerializer.TextureRoot / item.RsiPath, true).RSI;
				string text = "equipped-" + args.Slot.ToUpper();
				string text2 = text + "-color";
				string text3 = text + "-" + speciesId + "-color";
				if (speciesId != null && rSI.TryGetState(StateId.op_Implicit(text3), ref val3))
				{
					text2 = text3;
				}
				val2.RsiPath = item.RsiPath;
				val2.State = text2;
				val2.MapKeys = new HashSet<string> { text2 };
				val2.Color = value;
			}
		}
		if (val2.State != null)
		{
			args.Layers.Add((val2.State, val2));
		}
	}

	protected unsafe override void OnAppearanceChange(EntityUid uid, ItemCamouflageComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Expected O, but got Unknown
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Expected O, but got Unknown
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Expected O, but got Unknown
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Expected O, but got Unknown
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		base.OnAppearanceChange(uid, component, ref args);
		CamouflageType key = default(CamouflageType);
		if (!((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<CamouflageType>(uid, (Enum)ItemCamouflageVisuals.Camo, ref key, args.Component))
		{
			return;
		}
		if (component.CamouflageVariations != null && component.CamouflageVariations.TryGetValue(key, out var value))
		{
			if (args.Sprite != null)
			{
				int num = default(int);
				RSIResource val = default(RSIResource);
				if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)ItemCamouflageLayers.Layer, ref num, false))
				{
					_sprite.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, value, (StateId?)null);
				}
				else if (args.Sprite.BaseRSI != null && _resource.TryGetResource<RSIResource>(SpriteSpecifierSerializer.TextureRoot / value, ref val))
				{
					_sprite.SetBaseRsi(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), val.RSI);
				}
			}
			ClothingComponent clothingComponent = default(ClothingComponent);
			if (((EntitySystem)this).TryComp<ClothingComponent>(uid, ref clothingComponent))
			{
				clothingComponent.RsiPath = ((object)(*(ResPath*)(&value))/*cast due to constrained. prefix*/).ToString();
			}
			ItemComponent itemComponent = default(ItemComponent);
			if (((EntitySystem)this).TryComp<ItemComponent>(uid, ref itemComponent))
			{
				itemComponent.RsiPath = ((object)(*(ResPath*)(&value))/*cast due to constrained. prefix*/).ToString();
			}
			AttachableToggleableComponent attachableToggleableComponent = default(AttachableToggleableComponent);
			if (((EntitySystem)this).TryComp<AttachableToggleableComponent>(uid, ref attachableToggleableComponent))
			{
				SpriteSpecifier icon = attachableToggleableComponent.Icon;
				Rsi val2 = (Rsi)(object)((icon is Rsi) ? icon : null);
				if (val2 != null)
				{
					attachableToggleableComponent.Icon = (SpriteSpecifier)new Rsi(value, val2.RsiState);
				}
				SpriteSpecifier? iconActive = attachableToggleableComponent.IconActive;
				Rsi val3 = (Rsi)(object)((iconActive is Rsi) ? iconActive : null);
				if (val3 != null)
				{
					attachableToggleableComponent.IconActive = (SpriteSpecifier?)new Rsi(value, val3.RsiState);
				}
			}
			AttachableVisualsComponent attachableVisualsComponent = default(AttachableVisualsComponent);
			if (((EntitySystem)this).TryComp<AttachableVisualsComponent>(uid, ref attachableVisualsComponent))
			{
				if (attachableVisualsComponent.Rsi.HasValue)
				{
					attachableVisualsComponent.Rsi = value;
				}
				BaseContainer val4 = default(BaseContainer);
				AttachableHolderVisualsComponent item = default(AttachableHolderVisualsComponent);
				if (attachableVisualsComponent.LastSlotId != null && attachableVisualsComponent.LastSuffix != null && ((SharedContainerSystem)_container).TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(uid, null)), ref val4) && ((EntitySystem)this).TryComp<AttachableHolderVisualsComponent>(val4.Owner, ref item))
				{
					_attachableHolderVisuals.RefreshVisuals(Entity<AttachableHolderVisualsComponent>.op_Implicit((val4.Owner, item)), Entity<AttachableVisualsComponent>.op_Implicit((uid, attachableVisualsComponent)), attachableVisualsComponent.LastSlotId, attachableVisualsComponent.LastSuffix);
				}
			}
		}
		if (component.States != null && component.States.TryGetValue(key, out string value2))
		{
			if (args.Sprite != null)
			{
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), 0, StateId.op_Implicit(value2));
			}
			AttachableToggleableComponent attachableToggleableComponent2 = default(AttachableToggleableComponent);
			if (((EntitySystem)this).TryComp<AttachableToggleableComponent>(uid, ref attachableToggleableComponent2))
			{
				SpriteSpecifier icon2 = attachableToggleableComponent2.Icon;
				Rsi val5 = (Rsi)(object)((icon2 is Rsi) ? icon2 : null);
				if (val5 != null)
				{
					attachableToggleableComponent2.Icon = (SpriteSpecifier)new Rsi(val5.RsiPath, value2);
				}
				SpriteSpecifier? iconActive2 = attachableToggleableComponent2.IconActive;
				Rsi val6 = (Rsi)(object)((iconActive2 is Rsi) ? iconActive2 : null);
				if (val6 != null)
				{
					attachableToggleableComponent2.IconActive = (SpriteSpecifier?)new Rsi(val6.RsiPath, value2);
				}
			}
		}
		if (component.Colors != null && component.Colors.TryGetValue(key, out var value3) && args.Sprite != null)
		{
			ItemCamouflageLayers[] values = Enum.GetValues<ItemCamouflageLayers>();
			int num2 = default(int);
			foreach (ItemCamouflageLayers itemCamouflageLayers in values)
			{
				if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)itemCamouflageLayers, ref num2, false))
				{
					_sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num2, value3);
				}
			}
		}
		if (component.Layers != null && args.Sprite != null)
		{
			int num3 = default(int);
			foreach (var (text2, dictionary2) in component.Layers)
			{
				if (dictionary2.TryGetValue(key, out var value4) && _sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), text2, ref num3, false))
				{
					_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num3, StateId.op_Implicit(value4));
				}
			}
		}
		_item.VisualsChanged(uid);
	}
}
