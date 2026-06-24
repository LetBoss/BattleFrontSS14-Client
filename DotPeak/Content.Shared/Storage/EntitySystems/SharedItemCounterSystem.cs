// Decompiled with JetBrains decompiler
// Type: Content.Shared.Storage.EntitySystems.SharedItemCounterSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Stacks;
using Content.Shared.Storage.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Storage.EntitySystems;

public abstract class SharedItemCounterSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ItemCounterComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<ItemCounterComponent, EntInsertedIntoContainerMessage>(this.CounterEntityInserted));
    this.SubscribeLocalEvent<ItemCounterComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<ItemCounterComponent, EntRemovedFromContainerMessage>(this.CounterEntityRemoved));
  }

  private void CounterEntityInserted(
    EntityUid uid,
    ItemCounterComponent itemCounter,
    EntInsertedIntoContainerMessage args)
  {
    AppearanceComponent comp;
    if (!this.TryComp<AppearanceComponent>(uid, out comp))
      return;
    int? count = this.GetCount((ContainerModifiedMessage) args, itemCounter);
    if (!count.HasValue)
      return;
    this._appearance.SetData(uid, (Enum) StackVisuals.Actual, (object) count, comp);
    if (!itemCounter.MaxAmount.HasValue)
      return;
    this._appearance.SetData(uid, (Enum) StackVisuals.MaxCount, (object) itemCounter.MaxAmount, comp);
  }

  private void CounterEntityRemoved(
    EntityUid uid,
    ItemCounterComponent itemCounter,
    EntRemovedFromContainerMessage args)
  {
    AppearanceComponent comp;
    if (!this.TryComp<AppearanceComponent>(uid, out comp))
      return;
    int? count = this.GetCount((ContainerModifiedMessage) args, itemCounter);
    if (!count.HasValue)
      return;
    this._appearance.SetData(uid, (Enum) StackVisuals.Actual, (object) count, comp);
    if (!itemCounter.MaxAmount.HasValue)
      return;
    this._appearance.SetData(uid, (Enum) StackVisuals.MaxCount, (object) itemCounter.MaxAmount, comp);
  }

  protected abstract int? GetCount(ContainerModifiedMessage msg, ItemCounterComponent itemCounter);
}
