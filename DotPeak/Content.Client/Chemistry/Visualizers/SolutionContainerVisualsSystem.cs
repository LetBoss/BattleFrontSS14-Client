// Decompiled with JetBrains decompiler
// Type: Content.Client.Chemistry.Visualizers.SolutionContainerVisualsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Items.Systems;
using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Clothing;
using Content.Shared.Clothing.Components;
using Content.Shared.Hands;
using Content.Shared.Item;
using Content.Shared.Rounding;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Client.Chemistry.Visualizers;

public sealed class SolutionContainerVisualsSystem : 
  VisualizerSystem<SolutionContainerVisualsComponent>
{
  [Dependency]
  private RMCReagentSystem _reagent;
  [Dependency]
  private ItemSystem _itemSystem;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<SolutionContainerVisualsComponent, MapInitEvent>(new ComponentEventHandler<SolutionContainerVisualsComponent, MapInitEvent>((object) this, __methodptr(OnMapInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<SolutionContainerVisualsComponent, GetInhandVisualsEvent>(new ComponentEventHandler<SolutionContainerVisualsComponent, GetInhandVisualsEvent>((object) this, __methodptr(OnGetHeldVisuals)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<SolutionContainerVisualsComponent, GetEquipmentVisualsEvent>(new EntityEventRefHandler<SolutionContainerVisualsComponent, GetEquipmentVisualsEvent>((object) this, __methodptr(OnGetClothingVisuals)), (Type[]) null, (Type[]) null);
  }

  private void OnMapInit(
    EntityUid uid,
    SolutionContainerVisualsComponent component,
    MapInitEvent args)
  {
    MetaDataComponent metaDataComponent = ((EntitySystem) this).MetaData(uid);
    component.InitialDescription = metaDataComponent.EntityDescription;
  }

  protected virtual void OnAppearanceChange(
    EntityUid uid,
    SolutionContainerVisualsComponent component,
    ref AppearanceChangeEvent args)
  {
    string str1;
    float actual;
    int num1;
    if (!string.IsNullOrEmpty(component.SolutionName) && ((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<string>(uid, (Enum) SolutionContainerVisuals.SolutionName, ref str1, args.Component) && str1 != component.SolutionName || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<float>(uid, (Enum) SolutionContainerVisuals.FillFraction, ref actual, args.Component) || args.Sprite == null || !this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) component.Layer, ref num1, false))
      return;
    int num2 = component.MaxFillLevels;
    string str2 = component.FillBaseName;
    bool flag1 = component.ChangeColor;
    SpriteSpecifier spriteSpecifier = component.MetamorphicDefaultSprite;
    if ((double) actual > 1.0)
    {
      ((EntitySystem) this).Log.Error($"Attempted to set solution container visuals volume ratio on {EntityStringRepresentation.op_Implicit(((EntitySystem) this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)))} to a value greater than 1. Volume should never be greater than max volume!");
      actual = 1f;
    }
    if (component.Metamorphic)
    {
      int num3;
      if (this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) component.BaseLayer, ref num3, false))
      {
        int num4;
        bool flag2 = this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) component.OverlayLayer, ref num4, false);
        string str3;
        if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<string>(uid, (Enum) SolutionContainerVisuals.BaseOverride, ref str3, args.Component))
        {
          Content.Shared._RMC14.Chemistry.Reagent.Reagent reagent;
          this._reagent.TryIndex(ProtoId<ReagentPrototype>.op_Implicit(str3), out reagent);
          SpriteSpecifier metamorphicSprite = reagent?.MetamorphicSprite;
          if (metamorphicSprite != null)
          {
            this.SpriteSystem.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num3, metamorphicSprite);
            if (reagent.MetamorphicMaxFillLevels > 0)
            {
              this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num1, true);
              num2 = reagent.MetamorphicMaxFillLevels;
              str2 = reagent.MetamorphicFillBaseName;
              flag1 = reagent.MetamorphicChangeColor;
              spriteSpecifier = metamorphicSprite;
            }
            else
              this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num1, false);
            if (flag2)
              this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num4, false);
          }
          else
          {
            this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num1, true);
            if (flag2)
              this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num4, true);
            if (component.MetamorphicDefaultSprite != null)
              this.SpriteSystem.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num3, component.MetamorphicDefaultSprite);
          }
        }
      }
    }
    else
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num1, true);
    int levels = ContentHelpers.RoundToLevels((double) actual, 1.0, num2 + 1);
    if (levels > 0)
    {
      if (str2 == null)
        return;
      string str4 = str2 + levels.ToString();
      if (spriteSpecifier != null)
        this.SpriteSystem.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num1, spriteSpecifier);
      this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num1, RSI.StateId.op_Implicit(str4));
      Color color;
      if (flag1 && ((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<Color>(uid, (Enum) SolutionContainerVisuals.Color, ref color, args.Component))
        this.SpriteSystem.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num1, color);
      else
        this.SpriteSystem.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num1, Color.White);
    }
    else if (component.EmptySpriteName == null)
    {
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num1, false);
    }
    else
    {
      this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num1, RSI.StateId.op_Implicit(component.EmptySpriteName));
      if (flag1)
        this.SpriteSystem.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num1, component.EmptySpriteColor);
      else
        this.SpriteSystem.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num1, Color.White);
    }
    this._itemSystem.VisualsChanged(uid);
  }

  private void OnGetHeldVisuals(
    EntityUid uid,
    SolutionContainerVisualsComponent component,
    GetInhandVisualsEvent args)
  {
    AppearanceComponent appearanceComponent;
    ItemComponent itemComponent;
    float actual;
    if (component.InHandsFillBaseName == null || !((EntitySystem) this).TryComp<AppearanceComponent>(uid, ref appearanceComponent) || !((EntitySystem) this).TryComp<ItemComponent>(uid, ref itemComponent) || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<float>(uid, (Enum) SolutionContainerVisuals.FillFraction, ref actual, appearanceComponent))
      return;
    int levels = ContentHelpers.RoundToLevels((double) actual, 1.0, component.InHandsMaxFillLevels + 1);
    if (levels <= 0)
      return;
    PrototypeLayerData prototypeLayerData = new PrototypeLayerData();
    string str = (itemComponent.HeldPrefix == null ? "inhand-" : itemComponent.HeldPrefix + "-inhand-") + args.Location.ToString().ToLowerInvariant() + component.InHandsFillBaseName + levels.ToString();
    prototypeLayerData.State = str;
    Color color;
    if (component.ChangeColor && ((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<Color>(uid, (Enum) SolutionContainerVisuals.Color, ref color, appearanceComponent))
      prototypeLayerData.Color = new Color?(color);
    args.Layers.Add((str, prototypeLayerData));
  }

  private void OnGetClothingVisuals(
    Entity<SolutionContainerVisualsComponent> ent,
    ref GetEquipmentVisualsEvent args)
  {
    AppearanceComponent appearanceComponent;
    ClothingComponent clothingComponent;
    float actual;
    if (ent.Comp.EquippedFillBaseName == null || !((EntitySystem) this).TryComp<AppearanceComponent>(Entity<SolutionContainerVisualsComponent>.op_Implicit(ent), ref appearanceComponent) || !((EntitySystem) this).TryComp<ClothingComponent>(Entity<SolutionContainerVisualsComponent>.op_Implicit(ent), ref clothingComponent) || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<float>(Entity<SolutionContainerVisualsComponent>.op_Implicit(ent), (Enum) SolutionContainerVisuals.FillFraction, ref actual, appearanceComponent))
      return;
    int levels = ContentHelpers.RoundToLevels((double) actual, 1.0, ent.Comp.EquippedMaxFillLevels + 1);
    if (levels <= 0)
      return;
    PrototypeLayerData prototypeLayerData = new PrototypeLayerData();
    string str = (clothingComponent.EquippedPrefix == null ? "equipped-" + args.Slot : $" {clothingComponent.EquippedPrefix}-equipped-{args.Slot}") + ent.Comp.EquippedFillBaseName + levels.ToString();
    SpriteComponent spriteComponent;
    RSI.State state;
    if (!((EntitySystem) this).TryComp<SpriteComponent>(Entity<SolutionContainerVisualsComponent>.op_Implicit(ent), ref spriteComponent) || spriteComponent.BaseRSI == null || !spriteComponent.BaseRSI.TryGetState(RSI.StateId.op_Implicit(str), ref state))
      return;
    prototypeLayerData.State = str;
    Color color;
    if (ent.Comp.ChangeColor && ((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<Color>(Entity<SolutionContainerVisualsComponent>.op_Implicit(ent), (Enum) SolutionContainerVisuals.Color, ref color, appearanceComponent))
      prototypeLayerData.Color = new Color?(color);
    args.Layers.Add((str, prototypeLayerData));
  }
}
