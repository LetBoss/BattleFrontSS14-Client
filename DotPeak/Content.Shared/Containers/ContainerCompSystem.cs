// Decompiled with JetBrains decompiler
// Type: Content.Shared.Containers.ContainerCompSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Containers;

public sealed class ContainerCompSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IPrototypeManager _proto;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ContainerCompComponent, EntInsertedIntoContainerMessage>(new EntityEventRefHandler<ContainerCompComponent, EntInsertedIntoContainerMessage>((object) this, __methodptr(OnConInsert)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ContainerCompComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<ContainerCompComponent, EntRemovedFromContainerMessage>((object) this, __methodptr(OnConRemove)), (Type[]) null, (Type[]) null);
  }

  private void OnConRemove(
    Entity<ContainerCompComponent> ent,
    ref EntRemovedFromContainerMessage args)
  {
    EntityPrototype entityPrototype;
    if (((ContainerModifiedMessage) args).Container.ID != ent.Comp.Container || this._timing.ApplyingState || !this._proto.TryIndex(ent.Comp.Proto, ref entityPrototype))
      return;
    this.EntityManager.RemoveComponents(((ContainerModifiedMessage) args).Entity, entityPrototype.Components);
  }

  private void OnConInsert(
    Entity<ContainerCompComponent> ent,
    ref EntInsertedIntoContainerMessage args)
  {
    EntityPrototype entityPrototype;
    if (((ContainerModifiedMessage) args).Container.ID != ent.Comp.Container || this._timing.ApplyingState || !this._proto.TryIndex(ent.Comp.Proto, ref entityPrototype))
      return;
    this.EntityManager.AddComponents(((ContainerModifiedMessage) args).Entity, entityPrototype.Components, true);
  }
}
