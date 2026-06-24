// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.EntitySystems.ReactiveContainerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.Reaction;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Chemistry.EntitySystems;

public sealed class ReactiveContainerSystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _containerSystem;
  [Dependency]
  private ReactiveSystem _reactiveSystem;
  [Dependency]
  private SharedSolutionContainerSystem _solutionContainerSystem;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ReactiveContainerComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<ReactiveContainerComponent, EntInsertedIntoContainerMessage>((object) this, __methodptr(OnInserted)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ReactiveContainerComponent, SolutionContainerChangedEvent>(new ComponentEventHandler<ReactiveContainerComponent, SolutionContainerChangedEvent>((object) this, __methodptr(OnSolutionChange)), (Type[]) null, (Type[]) null);
  }

  private void OnInserted(
    EntityUid uid,
    ReactiveContainerComponent comp,
    EntInsertedIntoContainerMessage args)
  {
    Solution solution;
    if (!this.HasComp<ReactiveComponent>(((ContainerModifiedMessage) args).Entity) || !this._solutionContainerSystem.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(uid), comp.Solution, out Entity<SolutionComponent>? _, out solution) || solution.Volume == 0)
      return;
    this._reactiveSystem.DoEntityReaction(((ContainerModifiedMessage) args).Entity, solution, ReactionMethod.Touch);
  }

  private void OnSolutionChange(
    EntityUid uid,
    ReactiveContainerComponent comp,
    SolutionContainerChangedEvent args)
  {
    Solution solution;
    ContainerManagerComponent managerComponent;
    BaseContainer baseContainer;
    if (!this._solutionContainerSystem.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(uid), comp.Solution, out Entity<SolutionComponent>? _, out solution) || solution.Volume == 0 || !this.TryComp<ContainerManagerComponent>(uid, ref managerComponent) || !this._containerSystem.TryGetContainer(uid, comp.Container, ref baseContainer, (ContainerManagerComponent) null))
      return;
    foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) baseContainer.ContainedEntities)
    {
      if (this.HasComp<ReactiveComponent>(containedEntity))
        this._reactiveSystem.DoEntityReaction(containedEntity, solution, ReactionMethod.Touch);
    }
  }
}
