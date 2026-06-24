// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Rotting.SharedRottingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Rejuvenate;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Atmos.Rotting;

public abstract class SharedRottingSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private MobStateSystem _mobState;
  public const int MaxStages = 3;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PerishableComponent, MapInitEvent>(new ComponentEventHandler<PerishableComponent, MapInitEvent>((object) this, __methodptr(OnPerishableMapInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PerishableComponent, MobStateChangedEvent>(new ComponentEventHandler<PerishableComponent, MobStateChangedEvent>((object) this, __methodptr(OnMobStateChanged)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PerishableComponent, ExaminedEvent>(new EntityEventRefHandler<PerishableComponent, ExaminedEvent>((object) this, __methodptr(OnPerishableExamined)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RottingComponent, ComponentShutdown>(new ComponentEventHandler<RottingComponent, ComponentShutdown>((object) this, __methodptr(OnShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RottingComponent, MobStateChangedEvent>(new ComponentEventHandler<RottingComponent, MobStateChangedEvent>((object) this, __methodptr(OnRottingMobStateChanged)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RottingComponent, RejuvenateEvent>(new ComponentEventHandler<RottingComponent, RejuvenateEvent>((object) this, __methodptr(OnRejuvenate)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RottingComponent, ExaminedEvent>(new ComponentEventHandler<RottingComponent, ExaminedEvent>((object) this, __methodptr(OnExamined)), (Type[]) null, (Type[]) null);
  }

  private void OnPerishableMapInit(EntityUid uid, PerishableComponent component, MapInitEvent args)
  {
    component.RotNextUpdate = this._timing.CurTime + component.PerishUpdateRate;
  }

  private void OnMobStateChanged(
    EntityUid uid,
    PerishableComponent component,
    MobStateChangedEvent args)
  {
    if (args.NewMobState != MobState.Dead && args.OldMobState != MobState.Dead || this.HasComp<RottingComponent>(uid))
      return;
    component.RotAccumulator = TimeSpan.Zero;
    component.RotNextUpdate = this._timing.CurTime + component.PerishUpdateRate;
  }

  private void OnPerishableExamined(Entity<PerishableComponent> perishable, ref ExaminedEvent args)
  {
    int num = this.PerishStage(perishable, 3);
    if (num < 1 || num > 3)
      return;
    bool flag = this.HasComp<MobStateComponent>(Entity<PerishableComponent>.op_Implicit(perishable));
    string str = $"perishable-{num.ToString()}{(!flag ? "-nonmob" : string.Empty)}";
    args.PushMarkup(this.Loc.GetString(str, ("target", (object) Identity.Entity(Entity<PerishableComponent>.op_Implicit(perishable), (IEntityManager) this.EntityManager))));
  }

  private void OnShutdown(EntityUid uid, RottingComponent component, ComponentShutdown args)
  {
    PerishableComponent perishableComponent;
    if (!this.TryComp<PerishableComponent>(uid, ref perishableComponent))
      return;
    perishableComponent.RotNextUpdate = TimeSpan.Zero;
  }

  private void OnRottingMobStateChanged(
    EntityUid uid,
    RottingComponent component,
    MobStateChangedEvent args)
  {
    if (args.NewMobState == MobState.Dead)
      return;
    this.RemCompDeferred(uid, (IComponent) component);
  }

  private void OnRejuvenate(EntityUid uid, RottingComponent component, RejuvenateEvent args)
  {
    this.RemCompDeferred<RottingComponent>(uid);
  }

  private void OnExamined(EntityUid uid, RottingComponent component, ExaminedEvent args)
  {
    int num = this.RotStage(uid, component);
    string str = num >= 2 ? "rotting-extremely-bloated" : (num >= 1 ? "rotting-bloated" : "rotting-rotting");
    if (!this.HasComp<MobStateComponent>(uid))
      str += "-nonmob";
    args.PushMarkup(this.Loc.GetString(str, ("target", (object) Identity.Entity(uid, (IEntityManager) this.EntityManager))));
  }

  public int PerishStage(Entity<PerishableComponent> perishable, int maxStages)
  {
    return perishable.Comp.RotAfter.TotalSeconds == 0.0 || perishable.Comp.RotAccumulator.TotalSeconds == 0.0 ? 0 : (int) (1.0 + (double) maxStages * perishable.Comp.RotAccumulator.TotalSeconds / perishable.Comp.RotAfter.TotalSeconds);
  }

  public bool IsRotProgressing(EntityUid uid, PerishableComponent? perishable)
  {
    if (!this.Resolve<PerishableComponent>(uid, ref perishable, false))
      return false;
    if (perishable.ForceRotProgression)
      return true;
    MobStateComponent component;
    BaseContainer baseContainer;
    if (this.TryComp<MobStateComponent>(uid, ref component) && !this._mobState.IsDead(uid, component) || this._container.TryGetOuterContainer(uid, this.Transform(uid), ref baseContainer) && this.HasComp<AntiRottingContainerComponent>(baseContainer.Owner))
      return false;
    IsRottingEvent isRottingEvent = new IsRottingEvent();
    this.RaiseLocalEvent<IsRottingEvent>(uid, ref isRottingEvent, false);
    return !isRottingEvent.Handled;
  }

  public bool IsRotten(EntityUid uid, RottingComponent? rotting = null)
  {
    return this.Resolve<RottingComponent>(uid, ref rotting, false);
  }

  public void ReduceAccumulator(EntityUid uid, TimeSpan time)
  {
    PerishableComponent perishableComponent;
    if (!this.TryComp<PerishableComponent>(uid, ref perishableComponent))
      return;
    RottingComponent rottingComponent;
    if (!this.TryComp<RottingComponent>(uid, ref rottingComponent))
    {
      perishableComponent.RotAccumulator -= time;
    }
    else
    {
      TimeSpan timeSpan = rottingComponent.TotalRotTime + perishableComponent.RotAccumulator - time;
      if (timeSpan < perishableComponent.RotAfter)
      {
        this.RemCompDeferred(uid, (IComponent) rottingComponent);
        perishableComponent.RotAccumulator = timeSpan;
      }
      else
        rottingComponent.TotalRotTime = timeSpan - perishableComponent.RotAfter;
    }
  }

  public int RotStage(EntityUid uid, RottingComponent? comp = null, PerishableComponent? perishable = null)
  {
    return !this.Resolve<RottingComponent, PerishableComponent>(uid, ref comp, ref perishable, true) ? 0 : (int) (comp.TotalRotTime.TotalSeconds / perishable.RotAfter.TotalSeconds);
  }
}
