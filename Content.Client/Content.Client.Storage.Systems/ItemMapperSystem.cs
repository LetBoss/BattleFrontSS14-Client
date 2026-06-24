using System;
using System.Linq;
using Content.Shared.Storage.Components;
using Content.Shared.Storage.EntitySystems;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

namespace Content.Client.Storage.Systems;

public sealed class ItemMapperSystem : SharedItemMapperSystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ItemMapperComponent, ComponentStartup>((ComponentEventHandler<ItemMapperComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemMapperComponent, AppearanceChangeEvent>((ComponentEventRefHandler<ItemMapperComponent, AppearanceChangeEvent>)OnAppearance, (Type[])null, (Type[])null);
	}

	private void OnStartup(EntityUid uid, ItemMapperComponent component, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref val))
		{
			ResPath valueOrDefault = component.RSIPath.GetValueOrDefault();
			if (!component.RSIPath.HasValue)
			{
				valueOrDefault = val.BaseRSI.Path;
				component.RSIPath = valueOrDefault;
			}
		}
	}

	private void OnAppearance(EntityUid uid, ItemMapperComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item))
		{
			if (component.SpriteLayers.Count == 0)
			{
				InitLayers(Entity<ItemMapperComponent, SpriteComponent, AppearanceComponent>.op_Implicit((uid, component, item, args.Component)));
			}
			EnableLayers(Entity<ItemMapperComponent, SpriteComponent, AppearanceComponent>.op_Implicit((uid, component, item, args.Component)));
		}
	}

	private void InitLayers(Entity<ItemMapperComponent, SpriteComponent, AppearanceComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected O, but got Unknown
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		Entity<ItemMapperComponent, SpriteComponent, AppearanceComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		ItemMapperComponent itemMapperComponent = default(ItemMapperComponent);
		SpriteComponent val3 = default(SpriteComponent);
		AppearanceComponent val4 = default(AppearanceComponent);
		val.Deconstruct(ref val2, ref itemMapperComponent, ref val3, ref val4);
		EntityUid val5 = val2;
		ItemMapperComponent itemMapperComponent2 = itemMapperComponent;
		SpriteComponent item = val3;
		AppearanceComponent val6 = val4;
		ShowLayerData showLayerData = default(ShowLayerData);
		if (!_appearance.TryGetData<ShowLayerData>(val5, (Enum)StorageMapVisuals.InitLayers, ref showLayerData, val6))
		{
			return;
		}
		itemMapperComponent2.SpriteLayers.AddRange(showLayerData.QueuedEntities);
		foreach (string spriteLayer in itemMapperComponent2.SpriteLayers)
		{
			_sprite.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((val5, item)), spriteLayer);
			_sprite.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((val5, item)), spriteLayer, (SpriteSpecifier)new Rsi(itemMapperComponent2.RSIPath.Value, spriteLayer));
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((val5, item)), spriteLayer, false);
		}
	}

	private void EnableLayers(Entity<ItemMapperComponent, SpriteComponent, AppearanceComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		Entity<ItemMapperComponent, SpriteComponent, AppearanceComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		ItemMapperComponent itemMapperComponent = default(ItemMapperComponent);
		SpriteComponent val3 = default(SpriteComponent);
		AppearanceComponent val4 = default(AppearanceComponent);
		val.Deconstruct(ref val2, ref itemMapperComponent, ref val3, ref val4);
		EntityUid val5 = val2;
		ItemMapperComponent itemMapperComponent2 = itemMapperComponent;
		SpriteComponent item = val3;
		AppearanceComponent val6 = val4;
		ShowLayerData showLayerData = default(ShowLayerData);
		if (!_appearance.TryGetData<ShowLayerData>(val5, (Enum)StorageMapVisuals.LayerChanged, ref showLayerData, val6))
		{
			return;
		}
		foreach (string spriteLayer in itemMapperComponent2.SpriteLayers)
		{
			bool flag = showLayerData.QueuedEntities.Contains(spriteLayer);
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((val5, item)), spriteLayer, flag);
		}
	}
}
