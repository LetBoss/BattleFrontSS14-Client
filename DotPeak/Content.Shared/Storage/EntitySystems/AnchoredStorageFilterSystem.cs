// Decompiled with JetBrains decompiler
// Type: Content.Shared.Storage.EntitySystems.AnchoredStorageFilterSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Storage.Components;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Storage.EntitySystems;

public sealed class AnchoredStorageFilterSystem : EntitySystem
{
  [Dependency]
  private EntityWhitelistSystem _whitelist;
  [Dependency]
  private SharedContainerSystem _container;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<AnchoredStorageFilterComponent, AnchorStateChangedEvent>(new EntityEventRefHandler<AnchoredStorageFilterComponent, AnchorStateChangedEvent>(this.OnAnchorStateChanged));
    this.SubscribeLocalEvent<AnchoredStorageFilterComponent, ContainerIsInsertingAttemptEvent>(new EntityEventRefHandler<AnchoredStorageFilterComponent, ContainerIsInsertingAttemptEvent>(this.OnInsertAttempt));
  }

  private void OnAnchorStateChanged(
    Entity<AnchoredStorageFilterComponent> ent,
    ref AnchorStateChangedEvent args)
  {
    StorageComponent comp;
    if (!args.Anchored || !this.TryComp<StorageComponent>((EntityUid) ent, out comp))
      return;
    foreach (EntityUid key in comp.StoredItems.Keys)
    {
      if (!this._whitelist.CheckBoth(new EntityUid?(key), ent.Comp.Blacklist, ent.Comp.Whitelist))
        this._container.RemoveEntity((EntityUid) ent, key);
    }
  }

  private void OnInsertAttempt(
    Entity<AnchoredStorageFilterComponent> ent,
    ref ContainerIsInsertingAttemptEvent args)
  {
    if (args.Cancelled || !this.Transform((EntityUid) ent).Anchored || this._whitelist.CheckBoth(new EntityUid?(args.EntityUid), ent.Comp.Blacklist, ent.Comp.Whitelist))
      return;
    args.Cancel();
  }
}
