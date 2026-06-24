// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Item.ItemCamouflageVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using Robust.Shared.Maths;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
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

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<ItemCamouflageComponent, GetInhandVisualsEvent>(new ComponentEventHandler<ItemCamouflageComponent, GetInhandVisualsEvent>((object) this, __methodptr(OnGetInhandVisuals)), (Type[]) null, new Type[1]
    {
      typeof (ItemSystem)
    });
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<ItemCamouflageComponent, GetEquipmentVisualsEvent>(new ComponentEventHandler<ItemCamouflageComponent, GetEquipmentVisualsEvent>((object) this, __methodptr(OnGetClothingVisuals)), (Type[]) null, new Type[1]
    {
      typeof (ClientClothingSystem)
    });
  }

  private void OnGetInhandVisuals(
    EntityUid uid,
    ItemCamouflageComponent camoComp,
    GetInhandVisualsEvent args)
  {
    AppearanceComponent appearanceComponent;
    if (!((EntitySystem) this).TryComp<AppearanceComponent>(uid, ref appearanceComponent))
      return;
    CamouflageType key;
    ((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<CamouflageType>(uid, (Enum) ItemCamouflageVisuals.Camo, ref key, appearanceComponent);
    if (camoComp.Colors == null)
      return;
    Color color;
    camoComp.Colors.TryGetValue(key, out color);
    PrototypeLayerData prototypeLayerData1 = new PrototypeLayerData();
    foreach ((string str, PrototypeLayerData prototypeLayerData2) in args.Layers)
    {
      prototypeLayerData1.RsiPath = prototypeLayerData2.RsiPath;
      prototypeLayerData1.State = str + "-color";
      prototypeLayerData1.MapKeys = new HashSet<string>()
      {
        str + "-color"
      };
      prototypeLayerData1.Color = new Color?(color);
    }
    if (prototypeLayerData1.State == null)
      return;
    args.Layers.Add((prototypeLayerData1.State, prototypeLayerData1));
  }

  private void OnGetClothingVisuals(
    EntityUid uid,
    ItemCamouflageComponent camoComp,
    GetEquipmentVisualsEvent args)
  {
    InventoryComponent inventoryComponent;
    ClothingComponent clothingComponent;
    if (!((EntitySystem) this).TryComp<InventoryComponent>(args.Equipee, ref inventoryComponent) || !((EntitySystem) this).TryComp<ClothingComponent>(uid, ref clothingComponent))
      return;
    string speciesId = inventoryComponent.SpeciesId;
    AppearanceComponent appearanceComponent;
    if (!((EntitySystem) this).TryComp<AppearanceComponent>(uid, ref appearanceComponent))
      return;
    CamouflageType key;
    ((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<CamouflageType>(uid, (Enum) ItemCamouflageVisuals.Camo, ref key, appearanceComponent);
    if (camoComp.Colors == null)
      return;
    Color color;
    camoComp.Colors.TryGetValue(key, out color);
    PrototypeLayerData prototypeLayerData1 = new PrototypeLayerData();
    foreach ((string _, PrototypeLayerData prototypeLayerData2) in args.Layers)
    {
      if (prototypeLayerData2.RsiPath != null)
      {
        RSI rsi = this._resource.GetResource<RSIResource>(ResPath.op_Division(SpriteSpecifierSerializer.TextureRoot, prototypeLayerData2.RsiPath), true).RSI;
        string str1 = "equipped-" + args.Slot.ToUpper();
        string str2 = str1 + "-color";
        string str3 = $"{str1}-{speciesId}-color";
        RSI.State state;
        if (speciesId != null && rsi.TryGetState(RSI.StateId.op_Implicit(str3), ref state))
          str2 = str3;
        prototypeLayerData1.RsiPath = prototypeLayerData2.RsiPath;
        prototypeLayerData1.State = str2;
        prototypeLayerData1.MapKeys = new HashSet<string>()
        {
          str2
        };
        prototypeLayerData1.Color = new Color?(color);
      }
    }
    if (prototypeLayerData1.State == null)
      return;
    args.Layers.Add((prototypeLayerData1.State, prototypeLayerData1));
  }

  protected virtual void OnAppearanceChange(
    EntityUid uid,
    ItemCamouflageComponent component,
    ref AppearanceChangeEvent args)
  {
    base.OnAppearanceChange(uid, component, ref args);
    CamouflageType key1;
    if (!((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<CamouflageType>(uid, (Enum) ItemCamouflageVisuals.Camo, ref key1, args.Component))
      return;
    ResPath resPath;
    if (component.CamouflageVariations != null && component.CamouflageVariations.TryGetValue(key1, out resPath))
    {
      if (args.Sprite != null)
      {
        int num;
        if (this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) ItemCamouflageLayers.Layer, ref num, false))
        {
          this._sprite.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, resPath, new RSI.StateId?());
        }
        else
        {
          RSIResource rsiResource;
          if (args.Sprite.BaseRSI != null && this._resource.TryGetResource<RSIResource>(ResPath.op_Division(SpriteSpecifierSerializer.TextureRoot, resPath), ref rsiResource))
            this._sprite.SetBaseRsi(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), rsiResource.RSI);
        }
      }
      ClothingComponent clothingComponent;
      if (((EntitySystem) this).TryComp<ClothingComponent>(uid, ref clothingComponent))
        clothingComponent.RsiPath = resPath.ToString();
      ItemComponent itemComponent;
      if (((EntitySystem) this).TryComp<ItemComponent>(uid, ref itemComponent))
        itemComponent.RsiPath = resPath.ToString();
      AttachableToggleableComponent toggleableComponent;
      if (((EntitySystem) this).TryComp<AttachableToggleableComponent>(uid, ref toggleableComponent))
      {
        if (toggleableComponent.Icon is SpriteSpecifier.Rsi icon)
          toggleableComponent.Icon = (SpriteSpecifier) new SpriteSpecifier.Rsi(resPath, icon.RsiState);
        if (toggleableComponent.IconActive is SpriteSpecifier.Rsi iconActive)
          toggleableComponent.IconActive = (SpriteSpecifier) new SpriteSpecifier.Rsi(resPath, iconActive.RsiState);
      }
      AttachableVisualsComponent visualsComponent1;
      if (((EntitySystem) this).TryComp<AttachableVisualsComponent>(uid, ref visualsComponent1))
      {
        if (visualsComponent1.Rsi.HasValue)
          visualsComponent1.Rsi = new ResPath?(resPath);
        BaseContainer baseContainer;
        AttachableHolderVisualsComponent visualsComponent2;
        if (visualsComponent1.LastSlotId != null && visualsComponent1.LastSuffix != null && ((SharedContainerSystem) this._container).TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((uid, (TransformComponent) null)), ref baseContainer) && ((EntitySystem) this).TryComp<AttachableHolderVisualsComponent>(baseContainer.Owner, ref visualsComponent2))
          this._attachableHolderVisuals.RefreshVisuals(Entity<AttachableHolderVisualsComponent>.op_Implicit((baseContainer.Owner, visualsComponent2)), Entity<AttachableVisualsComponent>.op_Implicit((uid, visualsComponent1)), visualsComponent1.LastSlotId, visualsComponent1.LastSuffix);
      }
    }
    string str1;
    if (component.States != null && component.States.TryGetValue(key1, out str1))
    {
      if (args.Sprite != null)
        this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), 0, RSI.StateId.op_Implicit(str1));
      AttachableToggleableComponent toggleableComponent;
      if (((EntitySystem) this).TryComp<AttachableToggleableComponent>(uid, ref toggleableComponent))
      {
        if (toggleableComponent.Icon is SpriteSpecifier.Rsi icon)
          toggleableComponent.Icon = (SpriteSpecifier) new SpriteSpecifier.Rsi(icon.RsiPath, str1);
        if (toggleableComponent.IconActive is SpriteSpecifier.Rsi iconActive)
          toggleableComponent.IconActive = (SpriteSpecifier) new SpriteSpecifier.Rsi(iconActive.RsiPath, str1);
      }
    }
    Color color;
    if (component.Colors != null && component.Colors.TryGetValue(key1, out color) && args.Sprite != null)
    {
      foreach (ItemCamouflageLayers camouflageLayers in Enum.GetValues<ItemCamouflageLayers>())
      {
        int num;
        if (this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) camouflageLayers, ref num, false))
          this._sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, color);
      }
    }
    if (component.Layers != null && args.Sprite != null)
    {
      foreach ((string key2, Dictionary<CamouflageType, string> dictionary) in component.Layers)
      {
        string str2;
        int num;
        if (dictionary.TryGetValue(key1, out str2) && this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), key2, ref num, false))
          this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, RSI.StateId.op_Implicit(str2));
      }
    }
    this._item.VisualsChanged(uid);
  }
}
