// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.EntitySystems.SolutionPurgeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Linq;

#nullable enable
namespace Content.Shared.Chemistry.EntitySystems;

public sealed class SolutionPurgeSystem : EntitySystem
{
  [Dependency]
  private SharedSolutionContainerSystem _solutionContainer;
  [Dependency]
  private IGameTiming _timing;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SolutionPurgeComponent, MapInitEvent>(new EntityEventRefHandler<SolutionPurgeComponent, MapInitEvent>((object) this, __methodptr(OnMapInit)), (Type[]) null, (Type[]) null);
  }

  private void OnMapInit(Entity<SolutionPurgeComponent> ent, ref MapInitEvent args)
  {
    ent.Comp.NextPurgeTime = this._timing.CurTime + ent.Comp.Duration;
    this.Dirty<SolutionPurgeComponent>(ent, (MetaDataComponent) null);
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    EntityQueryEnumerator<SolutionPurgeComponent, SolutionContainerManagerComponent> entityQueryEnumerator = this.EntityQueryEnumerator<SolutionPurgeComponent, SolutionContainerManagerComponent>();
    EntityUid entityUid;
    SolutionPurgeComponent solutionPurgeComponent;
    SolutionContainerManagerComponent managerComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref solutionPurgeComponent, ref managerComponent))
    {
      if (!(this._timing.CurTime < solutionPurgeComponent.NextPurgeTime))
      {
        solutionPurgeComponent.NextPurgeTime += solutionPurgeComponent.Duration;
        this.Dirty(entityUid, (IComponent) solutionPurgeComponent, (MetaDataComponent) null);
        Entity<SolutionComponent>? entity;
        if (this._solutionContainer.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit((entityUid, managerComponent)), solutionPurgeComponent.Solution, out entity))
          this._solutionContainer.SplitSolutionWithout(entity.Value, solutionPurgeComponent.Quantity, solutionPurgeComponent.Preserve.Select<ProtoId<ReagentPrototype>, string>((Func<ProtoId<ReagentPrototype>, string>) (proto => proto.Id)).ToArray<string>());
      }
    }
  }
}
