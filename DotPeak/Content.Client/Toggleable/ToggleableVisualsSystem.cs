// Decompiled with JetBrains decompiler
// Type: Content.Client.Toggleable.ToggleableVisualsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client.Toggleable;

public sealed class ToggleableVisualsSystem : VisualizerSystem<ToggleableVisualsComponent>
{
  [Dependency]
  private SharedItemSystem _item;
  [Dependency]
  private SharedPointLightSystem _pointLight;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<ToggleableVisualsComponent, GetInhandVisualsEvent>(new ComponentEventHandler<ToggleableVisualsComponent, GetInhandVisualsEvent>((object) this, __methodptr(OnGetHeldVisuals)), (Type[]) null, new Type[1]
    {
      typeof (ItemSystem)
    });
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<ToggleableVisualsComponent, GetEquipmentVisualsEvent>(new ComponentEventHandler<ToggleableVisualsComponent, GetEquipmentVisualsEvent>((object) this, __methodptr(OnGetEquipmentVisuals)), (Type[]) null, new Type[1]
    {
      typeof (ClientClothingSystem)
    });
  }

  protected virtual void OnAppearanceChange(
    EntityUid uid,
    ToggleableVisualsComponent component,
    ref AppearanceChangeEvent args)
  {
    bool flag;
    if (!((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) ToggleableVisuals.Enabled, ref flag, args.Component))
      return;
    Color color;
    bool data = ((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<Color>(uid, (Enum) ToggleableVisuals.Color, ref color, args.Component);
    int num;
    if (args.Sprite != null && component.SpriteLayer != null && this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), component.SpriteLayer, ref num, false))
    {
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, flag);
      if (data)
        this.SpriteSystem.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), component.SpriteLayer, color);
    }
    ItemTogglePointLightComponent pointLightComponent1;
    PointLightComponent pointLightComponent2;
    if (((EntitySystem) this).TryComp<ItemTogglePointLightComponent>(uid, ref pointLightComponent1) && ((EntitySystem) this).TryComp<PointLightComponent>(uid, ref pointLightComponent2))
    {
      this._pointLight.SetEnabled(uid, flag, (SharedPointLightComponent) pointLightComponent2, (MetaDataComponent) null);
      if (data && pointLightComponent1.ToggleableVisualsColorModulatesLights)
        this._pointLight.SetColor(uid, color, (SharedPointLightComponent) pointLightComponent2);
    }
    this._item.VisualsChanged(uid);
  }

  private void OnGetEquipmentVisuals(
    EntityUid uid,
    ToggleableVisualsComponent component,
    GetEquipmentVisualsEvent args)
  {
    AppearanceComponent appearanceComponent;
    bool flag;
    InventoryComponent inventoryComponent;
    if (!((EntitySystem) this).TryComp<AppearanceComponent>(uid, ref appearanceComponent) || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) ToggleableVisuals.Enabled, ref flag, appearanceComponent) || !flag || !((EntitySystem) this).TryComp<InventoryComponent>(args.Equipee, ref inventoryComponent))
      return;
    List<PrototypeLayerData> prototypeLayerDataList = (List<PrototypeLayerData>) null;
    if (inventoryComponent.SpeciesId != null)
      component.ClothingVisuals.TryGetValue($"{args.Slot}-{inventoryComponent.SpeciesId}", out prototypeLayerDataList);
    if (prototypeLayerDataList == null && !component.ClothingVisuals.TryGetValue(args.Slot, out prototypeLayerDataList))
      return;
    Color color;
    bool data = ((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<Color>(uid, (Enum) ToggleableVisuals.Color, ref color, appearanceComponent);
    int num = 0;
    foreach (PrototypeLayerData prototypeLayerData in prototypeLayerDataList)
    {
      HashSet<string> mapKeys = prototypeLayerData.MapKeys;
      string str1 = mapKeys != null ? mapKeys.FirstOrDefault<string>() : (string) null;
      if (str1 == null)
      {
        string str2;
        if (num != 0)
          str2 = $"{args.Slot}-toggle-{num}";
        else
          str2 = args.Slot + "-toggle";
        str1 = str2;
        ++num;
      }
      if (data)
        prototypeLayerData.Color = new Color?(color);
      args.Layers.Add((str1, prototypeLayerData));
    }
  }

  private void OnGetHeldVisuals(
    EntityUid uid,
    ToggleableVisualsComponent component,
    GetInhandVisualsEvent args)
  {
    AppearanceComponent appearanceComponent;
    bool flag;
    List<PrototypeLayerData> prototypeLayerDataList;
    if (!((EntitySystem) this).TryComp<AppearanceComponent>(uid, ref appearanceComponent) || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) ToggleableVisuals.Enabled, ref flag, appearanceComponent) || !flag || !component.InhandVisuals.TryGetValue(args.Location, out prototypeLayerDataList))
      return;
    Color color;
    bool data = ((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<Color>(uid, (Enum) ToggleableVisuals.Color, ref color, appearanceComponent);
    int num = 0;
    string str1 = $"inhand-{args.Location.ToString().ToLowerInvariant()}-toggle";
    foreach (PrototypeLayerData prototypeLayerData in prototypeLayerDataList)
    {
      HashSet<string> mapKeys = prototypeLayerData.MapKeys;
      string str2 = mapKeys != null ? mapKeys.FirstOrDefault<string>() : (string) null;
      if (str2 == null)
      {
        string str3;
        if (num != 0)
          str3 = $"{str1}-{num}";
        else
          str3 = str1;
        str2 = str3;
        ++num;
      }
      if (data)
        prototypeLayerData.Color = new Color?(color);
      args.Layers.Add((str2, prototypeLayerData));
    }
  }
}
