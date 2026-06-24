// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.EntitySystems.SolutionRegenerationSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.FixedPoint;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Chemistry.EntitySystems;

public sealed class SolutionRegenerationSystem : EntitySystem
{
  [Dependency]
  private SharedSolutionContainerSystem _solutionContainer;
  [Dependency]
  private IGameTiming _timing;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SolutionRegenerationComponent, MapInitEvent>(new EntityEventRefHandler<SolutionRegenerationComponent, MapInitEvent>((object) this, __methodptr(OnMapInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SolutionRegenerationComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<SolutionRegenerationComponent, EntRemovedFromContainerMessage>((object) this, __methodptr(OnEntRemoved)), (Type[]) null, (Type[]) null);
  }

  private void OnMapInit(Entity<SolutionRegenerationComponent> ent, ref MapInitEvent args)
  {
    ent.Comp.NextRegenTime = this._timing.CurTime + ent.Comp.Duration;
    this.Dirty<SolutionRegenerationComponent>(ent, (MetaDataComponent) null);
  }

  private void OnEntRemoved(
    Entity<SolutionRegenerationComponent> ent,
    ref EntRemovedFromContainerMessage args)
  {
    EntityUid entity = ((ContainerModifiedMessage) args).Entity;
    ref Entity<SolutionComponent>? local = ref ent.Comp.SolutionRef;
    EntityUid? nullable = local.HasValue ? new EntityUid?(local.GetValueOrDefault().Owner) : new EntityUid?();
    if ((nullable.HasValue ? (EntityUid.op_Equality(entity, nullable.GetValueOrDefault()) ? 1 : 0) : 0) == 0)
      return;
    ent.Comp.SolutionRef = new Entity<SolutionComponent>?();
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    EntityQueryEnumerator<SolutionRegenerationComponent, SolutionContainerManagerComponent> entityQueryEnumerator = this.EntityQueryEnumerator<SolutionRegenerationComponent, SolutionContainerManagerComponent>();
    EntityUid entityUid;
    SolutionRegenerationComponent regenerationComponent;
    SolutionContainerManagerComponent managerComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref regenerationComponent, ref managerComponent))
    {
      if (!(this._timing.CurTime < regenerationComponent.NextRegenTime))
      {
        regenerationComponent.NextRegenTime += regenerationComponent.Duration;
        this.Dirty(entityUid, (IComponent) regenerationComponent, (MetaDataComponent) null);
        Solution solution;
        if (this._solutionContainer.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit((entityUid, managerComponent)), regenerationComponent.SolutionName, ref regenerationComponent.SolutionRef, out solution))
        {
          FixedPoint2 toTake = FixedPoint2.Min(solution.AvailableVolume, regenerationComponent.Generated.Volume);
          if (!(toTake <= FixedPoint2.Zero))
          {
            Solution toAdd = toTake == regenerationComponent.Generated.Volume ? regenerationComponent.Generated : regenerationComponent.Generated.Clone().SplitSolution(toTake);
            this._solutionContainer.TryAddSolution(regenerationComponent.SolutionRef.Value, toAdd);
          }
        }
      }
    }
  }
}
