// Decompiled with JetBrains decompiler
// Type: Content.Client.Clothing.FlippableClothingVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Clothing;
using Content.Shared.Clothing.Components;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Foldable;
using Content.Shared.Item;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Clothing;

public sealed class FlippableClothingVisualizerSystem : 
  VisualizerSystem<FlippableClothingVisualsComponent>
{
  [Dependency]
  private SharedItemSystem _itemSys;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<FlippableClothingVisualsComponent, GetEquipmentVisualsEvent>(new EntityEventRefHandler<FlippableClothingVisualsComponent, GetEquipmentVisualsEvent>((object) this, __methodptr(OnGetVisuals)), (Type[]) null, new Type[1]
    {
      typeof (ClothingSystem)
    });
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<FlippableClothingVisualsComponent, FoldedEvent>(new EntityEventRefHandler<FlippableClothingVisualsComponent, FoldedEvent>((object) this, __methodptr(OnFolded)), (Type[]) null, (Type[]) null);
  }

  private void OnFolded(Entity<FlippableClothingVisualsComponent> ent, ref FoldedEvent args)
  {
    this._itemSys.VisualsChanged(Entity<FlippableClothingVisualsComponent>.op_Implicit(ent));
  }

  private void OnGetVisuals(
    Entity<FlippableClothingVisualsComponent> ent,
    ref GetEquipmentVisualsEvent args)
  {
    SpriteComponent spriteComponent;
    ClothingComponent clothingComponent;
    bool flag;
    int num;
    if (!((EntitySystem) this).TryComp<SpriteComponent>(Entity<FlippableClothingVisualsComponent>.op_Implicit(ent), ref spriteComponent) || !((EntitySystem) this).TryComp<ClothingComponent>(Entity<FlippableClothingVisualsComponent>.op_Implicit(ent), ref clothingComponent) || clothingComponent.MappedLayer == null || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(Entity<FlippableClothingVisualsComponent>.op_Implicit(ent), (Enum) FoldableSystem.FoldedVisuals.State, ref flag, (AppearanceComponent) null) || !this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), flag ? ent.Comp.FoldingLayer : ent.Comp.UnfoldingLayer, ref num, false))
      return;
    ISpriteLayer ispriteLayer = spriteComponent[num];
    foreach ((string, PrototypeLayerData) layer in args.Layers)
    {
      if (!(layer.Item1 != clothingComponent.MappedLayer))
        layer.Item2.Scale = new Vector2?(ispriteLayer.Scale);
    }
  }
}
