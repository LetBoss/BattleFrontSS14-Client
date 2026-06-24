// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Medical.Refill.CMRefillableSolutionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Chemistry;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Rules;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared._RMC14.Medical.Refill;

public sealed class CMRefillableSolutionSystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private RMCMapSystem _rmcMap;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedSolutionContainerSystem _solution;
  [Dependency]
  private SolutionTransferSystem _solutionTransfer;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private EntityWhitelistSystem _whitelist;
  [Dependency]
  private RMCPlanetSystem _rmcPlanet;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private IPrototypeManager _proto;
  [Dependency]
  private SharedDoAfterSystem _doafter;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<CMRefillableSolutionComponent, ExaminedEvent>(new EntityEventRefHandler<CMRefillableSolutionComponent, ExaminedEvent>(this.OnRefillableSolutionExamined));
    this.SubscribeLocalEvent<CMSolutionRefillerComponent, MapInitEvent>(new EntityEventRefHandler<CMSolutionRefillerComponent, MapInitEvent>(this.OnRefillerMapInit));
    this.SubscribeLocalEvent<CMSolutionRefillerComponent, InteractUsingEvent>(new EntityEventRefHandler<CMSolutionRefillerComponent, InteractUsingEvent>(this.OnRefillerInteractUsing));
    this.SubscribeLocalEvent<RMCRefillSolutionOnStoreComponent, EntInsertedIntoContainerMessage>(new EntityEventRefHandler<RMCRefillSolutionOnStoreComponent, EntInsertedIntoContainerMessage>(this.OnRefillSolutionOnStoreInserted));
    this.SubscribeLocalEvent<RMCRefillSolutionFromContainerOnStoreComponent, EntInsertedIntoContainerMessage>(new EntityEventRefHandler<RMCRefillSolutionFromContainerOnStoreComponent, EntInsertedIntoContainerMessage>(this.OnRefillSolutionFromContainerOnStoreInserted));
    this.SubscribeLocalEvent<RMCRefillSolutionFromContainerOnStoreComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<RMCRefillSolutionFromContainerOnStoreComponent, GetVerbsEvent<AlternativeVerb>>(this.OnRefillSolutionFromContainerOnStoreGetVerbs));
    this.SubscribeLocalEvent<RMCRefillSolutionFromContainerOnStoreComponent, ContainerFlushDoAfterEvent>(new EntityEventRefHandler<RMCRefillSolutionFromContainerOnStoreComponent, ContainerFlushDoAfterEvent>(this.OnRefillSolutionFromContainerOnStoreFlush));
    this.SubscribeLocalEvent<RMCFlushableSolutionComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<RMCFlushableSolutionComponent, GetVerbsEvent<AlternativeVerb>>(this.OnFlushableSolutionGetVerbs));
    this.SubscribeLocalEvent<RMCFlushableSolutionComponent, ContainerFlushDoAfterEvent>(new EntityEventRefHandler<RMCFlushableSolutionComponent, ContainerFlushDoAfterEvent>(this.OnFlushableSolutionFlush));
    this.SubscribeLocalEvent<RMCPressurizedSolutionComponent, AfterInteractEvent>(new EntityEventRefHandler<RMCPressurizedSolutionComponent, AfterInteractEvent>(this.OnPressurizedRefillAttempt));
  }

  private void OnRefillableSolutionExamined(
    Entity<CMRefillableSolutionComponent> ent,
    ref ExaminedEvent args)
  {
    using (args.PushGroup("CMRefillableSolutionComponent"))
      args.PushMarkup("[color=cyan]This can be refilled by clicking on a medical vendor with it![/color]");
  }

  private void OnRefillerMapInit(Entity<CMSolutionRefillerComponent> ent, ref MapInitEvent args)
  {
    TransformComponent xform = this.Transform(ent.Owner);
    if (!ent.Comp.RandomizeReagentsPlanetside || !this._rmcPlanet.IsOnPlanet(xform))
      return;
    double num = this._random.NextDouble(0.0, ent.Comp.Max.Double() * 0.04) * 25.0;
    ent.Comp.Current = FixedPoint2.New(num);
    this.Dirty<CMSolutionRefillerComponent>(ent);
  }

  private void OnRefillerInteractUsing(
    Entity<CMSolutionRefillerComponent> ent,
    ref InteractUsingEvent args)
  {
    EntityUid entityUid = args.Used;
    RMCHyposprayComponent comp1;
    BaseContainer container;
    if (this.TryComp<RMCHyposprayComponent>(args.Used, out comp1) && this._container.TryGetContainer(args.Used, comp1.SlotId, out container) && container.ContainedEntities.Count != 0)
      entityUid = container.ContainedEntities[0];
    CMRefillableSolutionComponent comp2;
    if (!this.TryComp<CMRefillableSolutionComponent>(entityUid, out comp2))
      return;
    args.Handled = true;
    if (!this._whitelist.IsValid(ent.Comp.Whitelist, entityUid))
    {
      this._popup.PopupClient(this.Loc.GetString("cm-refillable-solution-cannot-refill", ("user", (object) ent.Owner), ("target", (object) entityUid)), args.User, new EntityUid?(args.User), PopupType.SmallCaution);
    }
    else
    {
      Entity<SolutionComponent>? entity;
      if (!this._solution.TryGetSolution((Entity<SolutionContainerManagerComponent>) entityUid, comp2.Solution, out entity))
        return;
      Solution solution = entity.Value.Comp.Solution;
      if (solution.AvailableVolume == FixedPoint2.Zero)
      {
        this._popup.PopupClient(this.Loc.GetString("cm-refillable-solution-full", ("target", (object) entityUid)), args.User, new EntityUid?(args.User));
      }
      else
      {
        bool flag = false;
        foreach ((ProtoId<ReagentPrototype> protoId, FixedPoint2 b) in comp2.Reagents)
        {
          if (ent.Comp.Reagents.Contains(protoId))
          {
            FixedPoint2 quantity = FixedPoint2.Min(FixedPoint2.Min(ent.Comp.Current, b), solution.AvailableVolume);
            if (!(quantity == FixedPoint2.Zero))
            {
              ent.Comp.Current -= quantity;
              this._solution.TryAddReagent(entity.Value, (string) protoId, quantity);
              flag = true;
            }
            else
              break;
          }
        }
        if (flag)
        {
          this.Dirty<CMSolutionRefillerComponent>(ent);
          RefilledSolutionEvent args1 = new RefilledSolutionEvent();
          this.RaiseLocalEvent<RefilledSolutionEvent>(args.Used, ref args1);
          this._popup.PopupClient(this.Loc.GetString("cm-refillable-solution-whirring-noise", ("user", (object) ent.Owner), ("target", (object) entityUid)), args.User, new EntityUid?(args.User));
        }
        else
          this._popup.PopupClient(this.Loc.GetString("cm-refillable-solution-cannot-refill", ("user", (object) ent.Owner), ("target", (object) entityUid)), args.User, new EntityUid?(args.User), PopupType.SmallCaution);
      }
    }
  }

  private void OnRefillSolutionOnStoreInserted(
    Entity<RMCRefillSolutionOnStoreComponent> ent,
    ref EntInsertedIntoContainerMessage args)
  {
    Entity<SolutionComponent>? entity;
    Entity<SolutionComponent>? soln;
    if (!this._solution.TryGetSolution((Entity<SolutionContainerManagerComponent>) ent.Owner, ent.Comp.SolutionId, out entity) || !this.TryGetStorageFillableSolution((Entity<SolutionStorageFillableComponent, SolutionContainerManagerComponent>) args.Entity, out soln, out Solution _))
      return;
    FixedPoint2 availableVolume = soln.Value.Comp.Solution.AvailableVolume;
    this._solutionTransfer.Transfer(new EntityUid?(), (EntityUid) ent, entity.Value, args.Entity, soln.Value, availableVolume);
  }

  private void OnRefillSolutionFromContainerOnStoreInserted(
    Entity<RMCRefillSolutionFromContainerOnStoreComponent> ent,
    ref EntInsertedIntoContainerMessage args)
  {
    BaseContainer container;
    EntityUid? element;
    Entity<SolutionComponent>? soln1;
    Solution solution;
    if (!this._container.TryGetContainer((EntityUid) ent, ent.Comp.ContainerId, out container) || !container.ContainedEntities.TryFirstOrNull<EntityUid>(out element) || !this._solution.TryGetDrainableSolution((Entity<DrainableSolutionComponent, SolutionContainerManagerComponent>) element.Value, out soln1, out solution) && !this.TryGetPressurizedSolution((Entity<RMCPressurizedSolutionComponent, SolutionContainerManagerComponent>) element.Value, out soln1, out solution))
      return;
    if (solution != null)
      this._appearance.SetData((EntityUid) ent, (Enum) SolutionContainerStoreVisuals.Color, (object) solution.GetColor(this._proto));
    Entity<SolutionComponent>? soln2;
    if (!this.TryGetStorageFillableSolution((Entity<SolutionStorageFillableComponent, SolutionContainerManagerComponent>) args.Entity, out soln2, out Solution _))
      return;
    FixedPoint2 availableVolume = soln2.Value.Comp.Solution.AvailableVolume;
    Solution toAdd = this._solution.SplitSolution(soln1.Value, availableVolume);
    this._solution.AddSolution(soln2.Value, toAdd);
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<CMSolutionRefillerComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<CMSolutionRefillerComponent, TransformComponent>();
    EntityUid uid1;
    CMSolutionRefillerComponent comp1;
    TransformComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid1, out comp1, out comp2))
    {
      if (!(curTime < comp1.RechargeAt))
      {
        comp1.RechargeAt = curTime + comp1.RechargeCooldown;
        this.Dirty(uid1, (IComponent) comp1);
        if (comp2.Anchored)
        {
          RMCAnchoredEntitiesEnumerator entitiesEnumerator = this._rmcMap.GetAnchoredEntitiesEnumerator(uid1, facing: (DirectionFlag) 0);
          bool flag = false;
          EntityUid uid2;
          while (entitiesEnumerator.MoveNext(out uid2))
          {
            if (this.HasComp<CMMedicalSupplyLinkComponent>(uid2))
            {
              flag = true;
              break;
            }
          }
          if (flag)
            comp1.Current = FixedPoint2.Min(comp1.Max, comp1.Current + comp1.Recharge);
        }
      }
    }
  }

  public bool TryGetStorageFillableSolution(
    Entity<SolutionStorageFillableComponent?, SolutionContainerManagerComponent?> entity,
    [NotNullWhen(true)] out Entity<SolutionComponent>? soln,
    [NotNullWhen(true)] out Solution? solution)
  {
    if (this.Resolve<SolutionStorageFillableComponent>((EntityUid) entity, ref entity.Comp1, false))
      return this._solution.TryGetSolution((Entity<SolutionContainerManagerComponent>) (entity.Owner, entity.Comp2), entity.Comp1.Solution, out soln, out solution);
    soln = new Entity<SolutionComponent>?();
    solution = (Solution) null;
    return false;
  }

  public bool TryGetPressurizedSolution(
    Entity<RMCPressurizedSolutionComponent?, SolutionContainerManagerComponent?> entity,
    [NotNullWhen(true)] out Entity<SolutionComponent>? soln,
    [NotNullWhen(true)] out Solution? solution)
  {
    if (this.Resolve<RMCPressurizedSolutionComponent>((EntityUid) entity, ref entity.Comp1, false))
      return this._solution.TryGetSolution((Entity<SolutionContainerManagerComponent>) (entity.Owner, entity.Comp2), entity.Comp1.Solution, out soln, out solution);
    soln = new Entity<SolutionComponent>?();
    solution = (Solution) null;
    return false;
  }

  private void OnPressurizedRefillAttempt(
    Entity<RMCPressurizedSolutionComponent> beaker,
    ref AfterInteractEvent args)
  {
    if (!args.CanReach)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault1 = target.GetValueOrDefault();
    (EntityUid entityUid, RMCPressurizedSolutionComponent _) = beaker;
    SolutionTransferComponent comp1;
    if (!this.HasComp<ReagentTankComponent>(valueOrDefault1) || !this.TryComp<SolutionTransferComponent>((EntityUid) beaker, out comp1))
      return;
    Entity<SolutionComponent>? soln1;
    Solution solution1;
    Entity<SolutionComponent>? soln2;
    Solution solution2;
    if (!this.HasComp<RefillableSolutionComponent>(valueOrDefault1) && this._solution.TryGetDrainableSolution((Entity<DrainableSolutionComponent, SolutionContainerManagerComponent>) valueOrDefault1, out soln1, out solution1) && this.TryGetPressurizedSolution((Entity<RMCPressurizedSolutionComponent, SolutionContainerManagerComponent>) (entityUid, (RMCPressurizedSolutionComponent) null, (SolutionContainerManagerComponent) null), out soln2, out solution2))
    {
      FixedPoint2 transferAmount = comp1.TransferAmount;
      FixedPoint2 fixedPoint2 = this._solutionTransfer.Transfer(new EntityUid?(args.User), valueOrDefault1, soln1.Value, entityUid, soln2.Value, transferAmount);
      args.Handled = true;
      if (fixedPoint2 > 0)
      {
        this._popup.PopupPredicted(this.Loc.GetString(solution2.AvailableVolume == 0 ? "comp-solution-transfer-fill-fully" : "comp-solution-transfer-fill-normal", ("owner", (object) args.Target), ("amount", (object) fixedPoint2), ("target", (object) entityUid)), entityUid, new EntityUid?(args.User));
        return;
      }
    }
    RefillableSolutionComponent comp2;
    if (!this.TryComp<RefillableSolutionComponent>(valueOrDefault1, out comp2) || !this._solution.TryGetRefillableSolution((Entity<RefillableSolutionComponent, SolutionContainerManagerComponent>) (valueOrDefault1, comp2, (SolutionContainerManagerComponent) null), out soln1, out solution1) || !this.TryGetPressurizedSolution((Entity<RMCPressurizedSolutionComponent, SolutionContainerManagerComponent>) entityUid, out soln2, out solution1))
      return;
    FixedPoint2 fixedPoint2_1 = comp1.TransferAmount;
    FixedPoint2? maxRefill = (FixedPoint2?) comp2?.MaxRefill;
    if (maxRefill.HasValue)
    {
      FixedPoint2 valueOrDefault2 = maxRefill.GetValueOrDefault();
      fixedPoint2_1 = FixedPoint2.Min(fixedPoint2_1, valueOrDefault2);
    }
    FixedPoint2 fixedPoint2_2 = this._solutionTransfer.Transfer(new EntityUid?(args.User), entityUid, soln2.Value, valueOrDefault1, soln1.Value, fixedPoint2_1);
    args.Handled = true;
    if (!(fixedPoint2_2 > 0))
      return;
    this._popup.PopupEntity(this.Loc.GetString("comp-solution-transfer-transfer-solution", ("amount", (object) fixedPoint2_2), ("target", (object) valueOrDefault1)), entityUid, args.User);
  }

  private void OnRefillSolutionFromContainerOnStoreGetVerbs(
    Entity<RMCRefillSolutionFromContainerOnStoreComponent> ent,
    ref GetVerbsEvent<AlternativeVerb> args)
  {
    BaseContainer container;
    if (!args.CanAccess || !args.CanInteract || !ent.Comp.CanFlush || !this._container.TryGetContainer((EntityUid) ent, ent.Comp.ContainerId, out container) || !container.ContainedEntities.TryFirstOrNull<EntityUid>(out EntityUid? _))
      return;
    EntityUid user = args.User;
    SortedSet<AlternativeVerb> verbs = args.Verbs;
    AlternativeVerb alternativeVerb = new AlternativeVerb();
    alternativeVerb.Text = this.Loc.GetString("rmc-refillsolution-flush");
    alternativeVerb.Act = (Action) (() => this.TryFlushContainer(ent, user));
    verbs.Add(alternativeVerb);
  }

  private void TryFlushContainer(
    Entity<RMCRefillSolutionFromContainerOnStoreComponent> ent,
    EntityUid user)
  {
    BaseContainer container;
    EntityUid? element;
    Entity<SolutionComponent>? soln;
    Solution solution;
    if (!ent.Comp.CanFlush || !this._container.TryGetContainer((EntityUid) ent, ent.Comp.ContainerId, out container) || !container.ContainedEntities.TryFirstOrNull<EntityUid>(out element) || !this._solution.TryGetDrainableSolution((Entity<DrainableSolutionComponent, SolutionContainerManagerComponent>) element.Value, out soln, out solution) && !this.TryGetPressurizedSolution((Entity<RMCPressurizedSolutionComponent, SolutionContainerManagerComponent>) element.Value, out soln, out solution))
      return;
    this._popup.PopupClient(this.Loc.GetString("rmc-refillsolution-flush-start", ("time", (object) ent.Comp.FlushTime.TotalSeconds)), user, new EntityUid?(user), PopupType.SmallCaution);
    this._doafter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, ent.Comp.FlushTime, (DoAfterEvent) new ContainerFlushDoAfterEvent(), new EntityUid?((EntityUid) ent), new EntityUid?((EntityUid) ent))
    {
      BreakOnMove = true,
      DuplicateCondition = DuplicateConditions.SameTarget
    });
  }

  private void OnRefillSolutionFromContainerOnStoreFlush(
    Entity<RMCRefillSolutionFromContainerOnStoreComponent> ent,
    ref ContainerFlushDoAfterEvent args)
  {
    if (!ent.Comp.CanFlush || args.Cancelled || args.Handled)
      return;
    args.Handled = true;
    BaseContainer container;
    EntityUid? element;
    Entity<SolutionComponent>? soln;
    Solution solution;
    if (!this._container.TryGetContainer((EntityUid) ent, ent.Comp.ContainerId, out container) || !container.ContainedEntities.TryFirstOrNull<EntityUid>(out element) || !this._solution.TryGetDrainableSolution((Entity<DrainableSolutionComponent, SolutionContainerManagerComponent>) element.Value, out soln, out solution) && !this.TryGetPressurizedSolution((Entity<RMCPressurizedSolutionComponent, SolutionContainerManagerComponent>) element.Value, out soln, out solution))
      return;
    this._solution.RemoveAllSolution(soln.Value);
    AppearanceComponent comp;
    if (!this.TryComp<AppearanceComponent>((EntityUid) ent, out comp))
      return;
    this._appearance.QueueUpdate((EntityUid) ent, comp);
  }

  private void OnFlushableSolutionGetVerbs(
    Entity<RMCFlushableSolutionComponent> ent,
    ref GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract)
      return;
    EntityUid user = args.User;
    SortedSet<AlternativeVerb> verbs = args.Verbs;
    AlternativeVerb alternativeVerb = new AlternativeVerb();
    alternativeVerb.Text = this.Loc.GetString("rmc-refillsolution-flush");
    alternativeVerb.Act = (Action) (() => this.TryFlushSolution(ent, user));
    verbs.Add(alternativeVerb);
  }

  private void TryFlushSolution(Entity<RMCFlushableSolutionComponent> ent, EntityUid user)
  {
    this._popup.PopupClient(this.Loc.GetString("rmc-refillsolution-flush-start", ("time", (object) ent.Comp.FlushTime.TotalSeconds)), user, new EntityUid?(user), PopupType.SmallCaution);
    this._doafter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, ent.Comp.FlushTime, (DoAfterEvent) new ContainerFlushDoAfterEvent(), new EntityUid?((EntityUid) ent), new EntityUid?((EntityUid) ent))
    {
      BreakOnMove = true,
      DuplicateCondition = DuplicateConditions.SameTarget
    });
  }

  private void OnFlushableSolutionFlush(
    Entity<RMCFlushableSolutionComponent> ent,
    ref ContainerFlushDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    args.Handled = true;
    Entity<SolutionComponent>? entity;
    if (!this._solution.TryGetSolution((Entity<SolutionContainerManagerComponent>) ent.Owner, ent.Comp.Solution, out entity, out Solution _))
      return;
    this._solution.RemoveAllSolution(entity.Value);
    AppearanceComponent comp;
    if (!this.TryComp<AppearanceComponent>((EntityUid) ent, out comp))
      return;
    this._appearance.QueueUpdate((EntityUid) ent, comp);
  }
}
