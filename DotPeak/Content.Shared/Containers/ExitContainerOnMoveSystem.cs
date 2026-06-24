// Decompiled with JetBrains decompiler
// Type: Content.Shared.Containers.ExitContainerOnMoveSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Climbing.Systems;
using Content.Shared.Movement.Events;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Shared.Containers;

public sealed class ExitContainerOnMoveSystem : EntitySystem
{
  [Dependency]
  private ClimbSystem _climb;
  [Dependency]
  private SharedContainerSystem _container;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ExitContainerOnMoveComponent, ContainerRelayMovementEntityEvent>(new EntityEventRefHandler<ExitContainerOnMoveComponent, ContainerRelayMovementEntityEvent>((object) this, __methodptr(OnContainerRelay)), (Type[]) null, (Type[]) null);
  }

  private void OnContainerRelay(
    Entity<ExitContainerOnMoveComponent> ent,
    ref ContainerRelayMovementEntityEvent args)
  {
    EntityUid entityUid;
    ExitContainerOnMoveComponent containerOnMoveComponent1;
    ent.Deconstruct(ref entityUid, ref containerOnMoveComponent1);
    ExitContainerOnMoveComponent containerOnMoveComponent2 = containerOnMoveComponent1;
    ContainerManagerComponent managerComponent;
    BaseContainer baseContainer;
    if (!this.TryComp<ContainerManagerComponent>(Entity<ExitContainerOnMoveComponent>.op_Implicit(ent), ref managerComponent) || !this._container.TryGetContainer(Entity<ExitContainerOnMoveComponent>.op_Implicit(ent), containerOnMoveComponent2.ContainerId, ref baseContainer, managerComponent) || !baseContainer.Contains(args.Entity))
      return;
    this._climb.ForciblySetClimbing(args.Entity, Entity<ExitContainerOnMoveComponent>.op_Implicit(ent));
    this._container.RemoveEntity(Entity<ExitContainerOnMoveComponent>.op_Implicit(ent), args.Entity, managerComponent, (TransformComponent) null, (MetaDataComponent) null, true, false, new EntityCoordinates?(), new Angle?());
  }
}
