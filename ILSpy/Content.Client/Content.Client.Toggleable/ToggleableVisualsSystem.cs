using System;
using System.Collections.Generic;
using System.Linq;
using Content.Client.Clothing;
using Content.Client.Items.Systems;
using Content.Shared.Clothing;
using Content.Shared.Hands;
using Content.Shared.Inventory;
using Content.Shared.Item;
using Content.Shared.Light.Components;
using Content.Shared.Toggleable;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.Toggleable;

public sealed class ToggleableVisualsSystem : VisualizerSystem<ToggleableVisualsComponent>
{
	[Dependency]
	private SharedItemSystem _item;

	[Dependency]
	private SharedPointLightSystem _pointLight;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ToggleableVisualsComponent, GetInhandVisualsEvent>((ComponentEventHandler<ToggleableVisualsComponent, GetInhandVisualsEvent>)OnGetHeldVisuals, (Type[])null, new Type[1] { typeof(ItemSystem) });
		((EntitySystem)this).SubscribeLocalEvent<ToggleableVisualsComponent, GetEquipmentVisualsEvent>((ComponentEventHandler<ToggleableVisualsComponent, GetEquipmentVisualsEvent>)OnGetEquipmentVisuals, (Type[])null, new Type[1] { typeof(ClientClothingSystem) });
	}

	protected override void OnAppearanceChange(EntityUid uid, ToggleableVisualsComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		bool flag = default(bool);
		if (!((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)ToggleableVisuals.Enabled, ref flag, args.Component))
		{
			return;
		}
		Color val = default(Color);
		bool flag2 = ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<Color>(uid, (Enum)ToggleableVisuals.Color, ref val, args.Component);
		int num = default(int);
		if (args.Sprite != null && component.SpriteLayer != null && base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), component.SpriteLayer, ref num, false))
		{
			base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, flag);
			if (flag2)
			{
				base.SpriteSystem.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), component.SpriteLayer, val);
			}
		}
		ItemTogglePointLightComponent itemTogglePointLightComponent = default(ItemTogglePointLightComponent);
		PointLightComponent val2 = default(PointLightComponent);
		if (((EntitySystem)this).TryComp<ItemTogglePointLightComponent>(uid, ref itemTogglePointLightComponent) && ((EntitySystem)this).TryComp<PointLightComponent>(uid, ref val2))
		{
			_pointLight.SetEnabled(uid, flag, (SharedPointLightComponent)(object)val2, (MetaDataComponent)null);
			if (flag2 && itemTogglePointLightComponent.ToggleableVisualsColorModulatesLights)
			{
				_pointLight.SetColor(uid, val, (SharedPointLightComponent)(object)val2);
			}
		}
		_item.VisualsChanged(uid);
	}

	private void OnGetEquipmentVisuals(EntityUid uid, ToggleableVisualsComponent component, GetEquipmentVisualsEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		AppearanceComponent val = default(AppearanceComponent);
		bool flag = default(bool);
		InventoryComponent inventoryComponent = default(InventoryComponent);
		if (!((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref val) || !((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)ToggleableVisuals.Enabled, ref flag, val) || !flag || !((EntitySystem)this).TryComp<InventoryComponent>(args.Equipee, ref inventoryComponent))
		{
			return;
		}
		List<PrototypeLayerData> value = null;
		if (inventoryComponent.SpeciesId != null)
		{
			component.ClothingVisuals.TryGetValue(args.Slot + "-" + inventoryComponent.SpeciesId, out value);
		}
		if (value == null && !component.ClothingVisuals.TryGetValue(args.Slot, out value))
		{
			return;
		}
		Color value2 = default(Color);
		bool flag2 = ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<Color>(uid, (Enum)ToggleableVisuals.Color, ref value2, val);
		int num = 0;
		foreach (PrototypeLayerData item in value)
		{
			string text = item.MapKeys?.FirstOrDefault();
			if (text == null)
			{
				text = ((num == 0) ? (args.Slot + "-toggle") : $"{args.Slot}-toggle-{num}");
				num++;
			}
			if (flag2)
			{
				item.Color = value2;
			}
			args.Layers.Add((text, item));
		}
	}

	private void OnGetHeldVisuals(EntityUid uid, ToggleableVisualsComponent component, GetInhandVisualsEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		AppearanceComponent val = default(AppearanceComponent);
		bool flag = default(bool);
		if (!((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref val) || !((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)ToggleableVisuals.Enabled, ref flag, val) || !flag || !component.InhandVisuals.TryGetValue(args.Location, out List<PrototypeLayerData> value))
		{
			return;
		}
		Color value2 = default(Color);
		bool flag2 = ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<Color>(uid, (Enum)ToggleableVisuals.Color, ref value2, val);
		int num = 0;
		string text = "inhand-" + args.Location.ToString().ToLowerInvariant() + "-toggle";
		foreach (PrototypeLayerData item in value)
		{
			string text2 = item.MapKeys?.FirstOrDefault();
			if (text2 == null)
			{
				text2 = ((num == 0) ? text : $"{text}-{num}");
				num++;
			}
			if (flag2)
			{
				item.Color = value2;
			}
			args.Layers.Add((text2, item));
		}
	}
}
