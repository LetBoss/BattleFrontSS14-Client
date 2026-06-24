// Decompiled with JetBrains decompiler
// Type: Content.Shared.Containers.DeleteOnContainerEmptySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Containers;

public sealed class DeleteOnContainerEmptySystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _container;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DeleteOnContainerEmptyComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<DeleteOnContainerEmptyComponent, EntRemovedFromContainerMessage>((object) this, __methodptr(OnEntRemoved)), (Type[]) null, (Type[]) null);
  }

  private void OnEntRemoved(
    Entity<DeleteOnContainerEmptyComponent> ent,
    ref EntRemovedFromContainerMessage args)
  {
    BaseContainer baseContainer;
    if (((ContainerModifiedMessage) args).Container.ID != ent.Comp.ContainerId || !this._container.TryGetContainer(ent.Owner, ent.Comp.ContainerId, ref baseContainer, (ContainerManagerComponent) null) || baseContainer.ContainedEntities.Count != 0)
      return;
    this.PredictedQueueDel(ent.Owner);
  }
}
