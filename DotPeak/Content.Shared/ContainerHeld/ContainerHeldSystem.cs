// Decompiled with JetBrains decompiler
// Type: Content.Shared.ContainerHeld.ContainerHeldSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Item;
using Content.Shared.Storage;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Toggleable;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.ContainerHeld;

public sealed class ContainerHeldSystem : EntitySystem
{
  [Dependency]
  private SharedItemSystem _item;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedStorageSystem _storage;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ContainerHeldComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<ContainerHeldComponent, EntInsertedIntoContainerMessage>((object) this, __methodptr(OnContainerModified)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ContainerHeldComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<ContainerHeldComponent, EntRemovedFromContainerMessage>((object) this, __methodptr(OnContainerModified)), (Type[]) null, (Type[]) null);
  }

  private void OnContainerModified(
    EntityUid uid,
    ContainerHeldComponent comp,
    ContainerModifiedMessage args)
  {
    AppearanceComponent appearanceComponent;
    ItemComponent component;
    if (!this.HasComp<StorageComponent>(uid) || !this.TryComp<AppearanceComponent>(uid, ref appearanceComponent) || !this.TryComp<ItemComponent>(uid, ref component))
      return;
    if (this._storage.GetCumulativeItemAreas(Entity<StorageComponent>.op_Implicit(uid)) >= comp.Threshold)
    {
      this._item.SetHeldPrefix(uid, "full", component: component);
      this._appearance.SetData(uid, (Enum) ToggleableVisuals.Enabled, (object) true, appearanceComponent);
    }
    else
    {
      this._item.SetHeldPrefix(uid, "empty", component: component);
      this._appearance.SetData(uid, (Enum) ToggleableVisuals.Enabled, (object) false, appearanceComponent);
    }
  }
}
