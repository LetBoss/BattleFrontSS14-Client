// Decompiled with JetBrains decompiler
// Type: Content.Client.Storage.Systems.ItemMapperSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Storage.Components;
using Content.Shared.Storage.EntitySystems;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
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
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ItemMapperComponent, ComponentStartup>(new ComponentEventHandler<ItemMapperComponent, ComponentStartup>((object) this, __methodptr(OnStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ItemMapperComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<ItemMapperComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearance)), (Type[]) null, (Type[]) null);
  }

  private void OnStartup(EntityUid uid, ItemMapperComponent component, ComponentStartup args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    ItemMapperComponent itemMapperComponent = component;
    itemMapperComponent.RSIPath.GetValueOrDefault();
    if (itemMapperComponent.RSIPath.HasValue)
      return;
    ResPath path = spriteComponent.BaseRSI.Path;
    itemMapperComponent.RSIPath = new ResPath?(path);
  }

  private void OnAppearance(
    EntityUid uid,
    ItemMapperComponent component,
    ref AppearanceChangeEvent args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    if (component.SpriteLayers.Count == 0)
      this.InitLayers(Entity<ItemMapperComponent, SpriteComponent, AppearanceComponent>.op_Implicit((uid, component, spriteComponent, args.Component)));
    this.EnableLayers(Entity<ItemMapperComponent, SpriteComponent, AppearanceComponent>.op_Implicit((uid, component, spriteComponent, args.Component)));
  }

  private void InitLayers(
    Entity<ItemMapperComponent, SpriteComponent, AppearanceComponent> ent)
  {
    EntityUid entityUid1;
    ItemMapperComponent itemMapperComponent1;
    SpriteComponent spriteComponent1;
    AppearanceComponent appearanceComponent1;
    ent.Deconstruct(ref entityUid1, ref itemMapperComponent1, ref spriteComponent1, ref appearanceComponent1);
    EntityUid entityUid2 = entityUid1;
    ItemMapperComponent itemMapperComponent2 = itemMapperComponent1;
    SpriteComponent spriteComponent2 = spriteComponent1;
    AppearanceComponent appearanceComponent2 = appearanceComponent1;
    ShowLayerData showLayerData;
    if (!this._appearance.TryGetData<ShowLayerData>(entityUid2, (Enum) StorageMapVisuals.InitLayers, ref showLayerData, appearanceComponent2))
      return;
    itemMapperComponent2.SpriteLayers.AddRange((IEnumerable<string>) showLayerData.QueuedEntities);
    foreach (string spriteLayer in itemMapperComponent2.SpriteLayers)
    {
      this._sprite.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((entityUid2, spriteComponent2)), spriteLayer);
      this._sprite.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((entityUid2, spriteComponent2)), spriteLayer, (SpriteSpecifier) new SpriteSpecifier.Rsi(itemMapperComponent2.RSIPath.Value, spriteLayer));
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((entityUid2, spriteComponent2)), spriteLayer, false);
    }
  }

  private void EnableLayers(
    Entity<ItemMapperComponent, SpriteComponent, AppearanceComponent> ent)
  {
    EntityUid entityUid1;
    ItemMapperComponent itemMapperComponent1;
    SpriteComponent spriteComponent1;
    AppearanceComponent appearanceComponent1;
    ent.Deconstruct(ref entityUid1, ref itemMapperComponent1, ref spriteComponent1, ref appearanceComponent1);
    EntityUid entityUid2 = entityUid1;
    ItemMapperComponent itemMapperComponent2 = itemMapperComponent1;
    SpriteComponent spriteComponent2 = spriteComponent1;
    AppearanceComponent appearanceComponent2 = appearanceComponent1;
    ShowLayerData showLayerData;
    if (!this._appearance.TryGetData<ShowLayerData>(entityUid2, (Enum) StorageMapVisuals.LayerChanged, ref showLayerData, appearanceComponent2))
      return;
    foreach (string spriteLayer in itemMapperComponent2.SpriteLayers)
    {
      bool flag = showLayerData.QueuedEntities.Contains<string>(spriteLayer);
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((entityUid2, spriteComponent2)), spriteLayer, flag);
    }
  }
}
