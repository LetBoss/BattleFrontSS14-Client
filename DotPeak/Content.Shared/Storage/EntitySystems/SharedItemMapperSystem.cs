// Decompiled with JetBrains decompiler
// Type: Content.Shared.Storage.EntitySystems.SharedItemMapperSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Storage.Components;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Storage.EntitySystems;

public abstract class SharedItemMapperSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private EntityWhitelistSystem _whitelistSystem;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ItemMapperComponent, ComponentInit>(new ComponentEventHandler<ItemMapperComponent, ComponentInit>(this.InitLayers));
    this.SubscribeLocalEvent<ItemMapperComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<ItemMapperComponent, EntInsertedIntoContainerMessage>(this.MapperEntityInserted));
    this.SubscribeLocalEvent<ItemMapperComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<ItemMapperComponent, EntRemovedFromContainerMessage>(this.MapperEntityRemoved));
  }

  private void InitLayers(EntityUid uid, ItemMapperComponent component, ComponentInit args)
  {
    foreach ((string key, SharedMapLayerData sharedMapLayerData) in component.MapLayers)
      sharedMapLayerData.Layer = key;
    AppearanceComponent comp;
    if (this.TryComp<AppearanceComponent>(uid, out comp))
    {
      List<string> other = new List<string>((IEnumerable<string>) component.MapLayers.Keys);
      this._appearance.SetData(uid, (Enum) StorageMapVisuals.InitLayers, (object) new ShowLayerData((IReadOnlyList<string>) other), comp);
    }
    this.UpdateAppearance(uid, component);
  }

  private void MapperEntityRemoved(
    EntityUid uid,
    ItemMapperComponent itemMapper,
    EntRemovedFromContainerMessage args)
  {
    if (itemMapper.ContainerWhitelist != null && !itemMapper.ContainerWhitelist.Contains(args.Container.ID))
      return;
    this.UpdateAppearance(uid, itemMapper);
  }

  private void MapperEntityInserted(
    EntityUid uid,
    ItemMapperComponent itemMapper,
    EntInsertedIntoContainerMessage args)
  {
    if (itemMapper.ContainerWhitelist != null && !itemMapper.ContainerWhitelist.Contains(args.Container.ID))
      return;
    this.UpdateAppearance(uid, itemMapper);
  }

  private void UpdateAppearance(EntityUid uid, ItemMapperComponent? itemMapper = null)
  {
    AppearanceComponent comp;
    List<string> showLayers;
    if (!this.Resolve<ItemMapperComponent>(uid, ref itemMapper) || !this.TryComp<AppearanceComponent>(uid, out comp) || !this.TryGetLayers(uid, itemMapper, out showLayers))
      return;
    this._appearance.SetData(uid, (Enum) StorageMapVisuals.LayerChanged, (object) new ShowLayerData((IReadOnlyList<string>) showLayers), comp);
  }

  private bool TryGetLayers(
    EntityUid uid,
    ItemMapperComponent itemMapper,
    out List<string> showLayers)
  {
    EntityUid[] array = this._container.GetAllContainers(uid).Where<BaseContainer>((Func<BaseContainer, bool>) (c =>
    {
      HashSet<string> containerWhitelist = itemMapper.ContainerWhitelist;
      // ISSUE: explicit non-virtual call
      return containerWhitelist == null || __nonvirtual (containerWhitelist.Contains(c.ID));
    })).SelectMany<BaseContainer, EntityUid>((Func<BaseContainer, IEnumerable<EntityUid>>) (cont => (IEnumerable<EntityUid>) cont.ContainedEntities)).ToArray<EntityUid>();
    List<string> stringList = new List<string>();
    foreach (SharedMapLayerData sharedMapLayerData in itemMapper.MapLayers.Values)
    {
      SharedMapLayerData mapLayerData = sharedMapLayerData;
      int num = ((IEnumerable<EntityUid>) array).Count<EntityUid>((Func<EntityUid, bool>) (ent => this._whitelistSystem.IsWhitelistPassOrNull(mapLayerData.Whitelist, ent)));
      if (num >= mapLayerData.MinCount && num <= mapLayerData.MaxCount)
        stringList.Add(mapLayerData.Layer);
    }
    showLayers = stringList;
    return true;
  }
}
