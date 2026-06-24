// Decompiled with JetBrains decompiler
// Type: Content.Shared.Animals.WoolySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Systems;
using Content.Shared.Nutrition;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Animals;

public sealed class WoolySystem : EntitySystem
{
  [Dependency]
  private HungerSystem _hunger;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private SharedSolutionContainerSystem _solutionContainer;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<WoolyComponent, BeforeFullyEatenEvent>(new EntityEventRefHandler<WoolyComponent, BeforeFullyEatenEvent>((object) this, __methodptr(OnBeforeFullyEaten)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<WoolyComponent, MapInitEvent>(new ComponentEventHandler<WoolyComponent, MapInitEvent>((object) this, __methodptr(OnMapInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<WoolyComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<WoolyComponent, EntRemovedFromContainerMessage>((object) this, __methodptr(OnEntRemoved)), (Type[]) null, (Type[]) null);
  }

  private void OnMapInit(EntityUid uid, WoolyComponent component, MapInitEvent args)
  {
    component.NextGrowth = this._timing.CurTime + component.GrowthDelay;
  }

  private void OnEntRemoved(Entity<WoolyComponent> entity, ref EntRemovedFromContainerMessage args)
  {
    if (!entity.Comp.Solution.HasValue || EntityUid.op_Inequality(((ContainerModifiedMessage) args).Entity, entity.Comp.Solution.Value.Owner))
      return;
    entity.Comp.Solution = new Entity<SolutionComponent>?();
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    EntityQueryEnumerator<WoolyComponent> entityQueryEnumerator = this.EntityQueryEnumerator<WoolyComponent>();
    EntityUid entityUid;
    WoolyComponent woolyComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref woolyComponent))
    {
      if (!(this._timing.CurTime < woolyComponent.NextGrowth))
      {
        woolyComponent.NextGrowth += woolyComponent.GrowthDelay;
        Solution solution;
        if (!this._mobState.IsDead(entityUid) && this._solutionContainer.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(entityUid), woolyComponent.SolutionName, ref woolyComponent.Solution, out solution) && !(solution.AvailableVolume == 0))
        {
          HungerComponent component;
          if (this.TryComp<HungerComponent>(entityUid, ref component))
          {
            if (this._hunger.GetHungerThreshold(component) >= HungerThreshold.Okay)
              this._hunger.ModifyHunger(entityUid, -woolyComponent.HungerUsage, component);
            else
              continue;
          }
          this._solutionContainer.TryAddReagent(woolyComponent.Solution.Value, ProtoId<ReagentPrototype>.op_Implicit(woolyComponent.ReagentId), woolyComponent.Quantity, out FixedPoint2 _);
        }
      }
    }
  }

  private void OnBeforeFullyEaten(Entity<WoolyComponent> ent, ref BeforeFullyEatenEvent args)
  {
    args.Cancel();
  }
}
