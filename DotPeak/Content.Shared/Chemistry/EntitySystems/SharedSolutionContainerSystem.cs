// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.EntitySystems.SharedSolutionContainerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Containers;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Kitchen.Components;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

#nullable enable
namespace Content.Shared.Chemistry.EntitySystems;

public abstract class SharedSolutionContainerSystem : EntitySystem
{
  [Dependency]
  protected IPrototypeManager PrototypeManager;
  [Dependency]
  protected ChemicalReactionSystem ChemicalReactionSystem;
  [Dependency]
  protected ExamineSystemShared ExamineSystem;
  [Dependency]
  protected SharedAppearanceSystem AppearanceSystem;
  [Dependency]
  protected SharedHandsSystem Hands;
  [Dependency]
  protected SharedContainerSystem ContainerSystem;
  [Dependency]
  protected MetaDataSystem MetaDataSys;
  [Dependency]
  protected INetManager NetManager;
  [Dependency]
  private RMCReagentSystem _rmcReagents;

  public bool TryGetRefillableSolution(
    Entity<RefillableSolutionComponent?, SolutionContainerManagerComponent?> entity,
    [NotNullWhen(true)] out Entity<SolutionComponent>? soln,
    [NotNullWhen(true)] out Solution? solution)
  {
    if (this.Resolve<RefillableSolutionComponent>(Entity<RefillableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(entity), ref entity.Comp1, false))
      return this.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit((entity.Owner, entity.Comp2)), entity.Comp1.Solution, out soln, out solution);
    soln = new Entity<SolutionComponent>?();
    solution = (Solution) null;
    return false;
  }

  public bool TryGetDrainableSolution(
    Entity<DrainableSolutionComponent?, SolutionContainerManagerComponent?> entity,
    [NotNullWhen(true)] out Entity<SolutionComponent>? soln,
    [NotNullWhen(true)] out Solution? solution)
  {
    if (this.Resolve<DrainableSolutionComponent>(Entity<DrainableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(entity), ref entity.Comp1, false))
      return this.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit((entity.Owner, entity.Comp2)), entity.Comp1.Solution, out soln, out solution);
    soln = new Entity<SolutionComponent>?();
    solution = (Solution) null;
    return false;
  }

  public bool TryGetExtractableSolution(
    Entity<ExtractableComponent?, SolutionContainerManagerComponent?> entity,
    [NotNullWhen(true)] out Entity<SolutionComponent>? soln,
    [NotNullWhen(true)] out Solution? solution)
  {
    if (this.Resolve<ExtractableComponent>(Entity<ExtractableComponent, SolutionContainerManagerComponent>.op_Implicit(entity), ref entity.Comp1, false))
      return this.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit((entity.Owner, entity.Comp2)), entity.Comp1.GrindableSolution, out soln, out solution);
    soln = new Entity<SolutionComponent>?();
    solution = (Solution) null;
    return false;
  }

  public bool TryGetDumpableSolution(
    Entity<DumpableSolutionComponent?, SolutionContainerManagerComponent?> entity,
    [NotNullWhen(true)] out Entity<SolutionComponent>? soln,
    [NotNullWhen(true)] out Solution? solution)
  {
    if (this.Resolve<DumpableSolutionComponent>(Entity<DumpableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(entity), ref entity.Comp1, false))
      return this.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit((entity.Owner, entity.Comp2)), entity.Comp1.Solution, out soln, out solution);
    soln = new Entity<SolutionComponent>?();
    solution = (Solution) null;
    return false;
  }

  public bool TryGetDrawableSolution(
    Entity<DrawableSolutionComponent?, SolutionContainerManagerComponent?> entity,
    [NotNullWhen(true)] out Entity<SolutionComponent>? soln,
    [NotNullWhen(true)] out Solution? solution)
  {
    if (this.Resolve<DrawableSolutionComponent>(Entity<DrawableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(entity), ref entity.Comp1, false))
      return this.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit((entity.Owner, entity.Comp2)), entity.Comp1.Solution, out soln, out solution);
    soln = new Entity<SolutionComponent>?();
    solution = (Solution) null;
    return false;
  }

  public bool TryGetInjectableSolution(
    Entity<InjectableSolutionComponent?, SolutionContainerManagerComponent?> entity,
    [NotNullWhen(true)] out Entity<SolutionComponent>? soln,
    [NotNullWhen(true)] out Solution? solution)
  {
    if (this.Resolve<InjectableSolutionComponent>(Entity<InjectableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(entity), ref entity.Comp1, false))
      return this.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit((entity.Owner, entity.Comp2)), entity.Comp1.Solution, out soln, out solution);
    soln = new Entity<SolutionComponent>?();
    solution = (Solution) null;
    return false;
  }

  public bool TryGetFitsInDispenser(
    Entity<FitsInDispenserComponent?, SolutionContainerManagerComponent?> entity,
    [NotNullWhen(true)] out Entity<SolutionComponent>? soln,
    [NotNullWhen(true)] out Solution? solution)
  {
    if (this.Resolve<FitsInDispenserComponent>(Entity<FitsInDispenserComponent, SolutionContainerManagerComponent>.op_Implicit(entity), ref entity.Comp1, false))
      return this.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit((entity.Owner, entity.Comp2)), entity.Comp1.Solution, out soln, out solution);
    soln = new Entity<SolutionComponent>?();
    solution = (Solution) null;
    return false;
  }

  public bool TryGetMixableSolution(
    Entity<MixableSolutionComponent?, SolutionContainerManagerComponent?> entity,
    [NotNullWhen(true)] out Entity<SolutionComponent>? soln,
    [NotNullWhen(true)] out Solution? solution)
  {
    if (this.Resolve<MixableSolutionComponent>(Entity<MixableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(entity), ref entity.Comp1, false))
      return this.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit((entity.Owner, entity.Comp2)), entity.Comp1.Solution, out soln, out solution);
    soln = new Entity<SolutionComponent>?();
    solution = (Solution) null;
    return false;
  }

  public void Refill(
    Entity<RefillableSolutionComponent?> entity,
    Entity<SolutionComponent> soln,
    Solution refill)
  {
    if (!this.Resolve<RefillableSolutionComponent>(Entity<RefillableSolutionComponent>.op_Implicit(entity), ref entity.Comp, false))
      return;
    this.AddSolution(soln, refill);
  }

  public void Inject(
    Entity<InjectableSolutionComponent?> entity,
    Entity<SolutionComponent> soln,
    Solution inject)
  {
    if (!this.Resolve<InjectableSolutionComponent>(Entity<InjectableSolutionComponent>.op_Implicit(entity), ref entity.Comp, false))
      return;
    this.AddSolution(soln, inject);
  }

  public Solution Drain(
    Entity<DrainableSolutionComponent?> entity,
    Entity<SolutionComponent> soln,
    FixedPoint2 quantity)
  {
    return !this.Resolve<DrainableSolutionComponent>(Entity<DrainableSolutionComponent>.op_Implicit(entity), ref entity.Comp, false) ? new Solution() : this.SplitSolution(soln, quantity);
  }

  public Solution Draw(
    Entity<DrawableSolutionComponent?> entity,
    Entity<SolutionComponent> soln,
    FixedPoint2 quantity)
  {
    return !this.Resolve<DrawableSolutionComponent>(Entity<DrawableSolutionComponent>.op_Implicit(entity), ref entity.Comp, false) ? new Solution() : this.SplitSolution(soln, quantity);
  }

  public float PercentFull(EntityUid uid)
  {
    Solution solution;
    return !this.TryGetDrainableSolution(Entity<DrainableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(uid), out Entity<SolutionComponent>? _, out solution) || solution.MaxVolume.Equals(FixedPoint2.Zero) ? 0.0f : solution.FillFraction * 100f;
  }

  public static string ToPrettyString(Solution solution)
  {
    StringBuilder stringBuilder1 = new StringBuilder();
    if (solution.Name == null)
    {
      stringBuilder1.Append("[");
    }
    else
    {
      StringBuilder stringBuilder2 = stringBuilder1;
      StringBuilder stringBuilder3 = stringBuilder2;
      StringBuilder.AppendInterpolatedStringHandler interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(2, 1, stringBuilder2);
      interpolatedStringHandler.AppendFormatted(solution.Name);
      interpolatedStringHandler.AppendLiteral(":[");
      ref StringBuilder.AppendInterpolatedStringHandler local = ref interpolatedStringHandler;
      stringBuilder3.Append(ref local);
    }
    bool flag = true;
    foreach ((ReagentId id, FixedPoint2 quantity) in solution.Contents)
    {
      if (flag)
        flag = false;
      else
        stringBuilder1.Append(", ");
      stringBuilder1.AppendFormat("{0}: {1}u", (object) id, (object) quantity);
    }
    stringBuilder1.Append(']');
    return stringBuilder1.ToString();
  }

  public virtual void Initialize()
  {
    base.Initialize();
    this.InitializeRelays();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SolutionComponent, ComponentInit>(new EntityEventRefHandler<SolutionComponent, ComponentInit>((object) this, __methodptr(OnComponentInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SolutionComponent, ComponentStartup>(new EntityEventRefHandler<SolutionComponent, ComponentStartup>((object) this, __methodptr(OnSolutionStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SolutionComponent, ComponentShutdown>(new EntityEventRefHandler<SolutionComponent, ComponentShutdown>((object) this, __methodptr(OnSolutionShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SolutionContainerManagerComponent, ComponentInit>(new EntityEventRefHandler<SolutionContainerManagerComponent, ComponentInit>((object) this, __methodptr(OnContainerManagerInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ExaminableSolutionComponent, ExaminedEvent>(new EntityEventRefHandler<ExaminableSolutionComponent, ExaminedEvent>((object) this, __methodptr(OnExamineSolution)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ExaminableSolutionComponent, GetVerbsEvent<ExamineVerb>>(new EntityEventRefHandler<ExaminableSolutionComponent, GetVerbsEvent<ExamineVerb>>((object) this, __methodptr(OnSolutionExaminableVerb)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SolutionContainerManagerComponent, MapInitEvent>(new EntityEventRefHandler<SolutionContainerManagerComponent, MapInitEvent>((object) this, __methodptr(OnMapInit)), (Type[]) null, (Type[]) null);
    if (!this.NetManager.IsServer)
      return;
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SolutionContainerManagerComponent, ComponentShutdown>(new EntityEventRefHandler<SolutionContainerManagerComponent, ComponentShutdown>((object) this, __methodptr(OnContainerManagerShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ContainedSolutionComponent, ComponentShutdown>(new EntityEventRefHandler<ContainedSolutionComponent, ComponentShutdown>((object) this, __methodptr(OnContainedSolutionShutdown)), (Type[]) null, (Type[]) null);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool ResolveSolution(
    Entity<SolutionContainerManagerComponent?> container,
    string? name,
    [NotNullWhen(true)] ref Entity<SolutionComponent>? entity,
    [NotNullWhen(true)] out Solution? solution)
  {
    if (!this.ResolveSolution(container, name, ref entity))
    {
      solution = (Solution) null;
      return false;
    }
    solution = entity.Value.Comp.Solution;
    return true;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool ResolveSolution(
    Entity<SolutionContainerManagerComponent?> container,
    string? name,
    [NotNullWhen(true)] ref Entity<SolutionComponent>? entity)
  {
    return entity.HasValue || this.TryGetSolution(container, name, out entity);
  }

  public bool TryGetSolution(
    Entity<SolutionContainerManagerComponent?> container,
    string? name,
    [NotNullWhen(true)] out Entity<SolutionComponent>? entity,
    [NotNullWhen(true)] out Solution? solution,
    bool errorOnMissing = false)
  {
    if (!this.TryGetSolution(container, name, out entity, errorOnMissing))
    {
      solution = (Solution) null;
      return false;
    }
    solution = entity.Value.Comp.Solution;
    return true;
  }

  public bool TryGetSolution(
    Entity<SolutionContainerManagerComponent?> container,
    string? name,
    [NotNullWhen(true)] out Entity<SolutionComponent>? entity,
    bool errorOnMissing = false)
  {
    GetConnectedContainerEvent connectedContainerEvent = new GetConnectedContainerEvent();
    this.RaiseLocalEvent<GetConnectedContainerEvent>(Entity<SolutionContainerManagerComponent>.op_Implicit(container), ref connectedContainerEvent, false);
    if (connectedContainerEvent.ContainerEntity.HasValue)
      container = Entity<SolutionContainerManagerComponent>.op_Implicit(connectedContainerEvent.ContainerEntity.Value);
    EntityUid entityUid;
    if (name == null)
    {
      entityUid = Entity<SolutionContainerManagerComponent>.op_Implicit(container);
    }
    else
    {
      BaseContainer baseContainer;
      if (this.ContainerSystem.TryGetContainer(Entity<SolutionContainerManagerComponent>.op_Implicit(container), "solution@" + name, ref baseContainer, (ContainerManagerComponent) null) && baseContainer is ContainerSlot containerSlot)
      {
        EntityUid? containedEntity = containerSlot.ContainedEntity;
        if (containedEntity.HasValue)
        {
          EntityUid valueOrDefault = containedEntity.GetValueOrDefault();
          SolutionAccessAttemptEvent accessAttemptEvent = new SolutionAccessAttemptEvent(name);
          this.RaiseLocalEvent<SolutionAccessAttemptEvent>(Entity<SolutionContainerManagerComponent>.op_Implicit(container), ref accessAttemptEvent, false);
          if (accessAttemptEvent.Cancelled)
          {
            entity = new Entity<SolutionComponent>?();
            return false;
          }
          entityUid = valueOrDefault;
          goto label_12;
        }
      }
      entity = new Entity<SolutionComponent>?();
      if (!errorOnMissing)
        return false;
      this.Log.Error($"{this.ToPrettyString(new EntityUid?(Entity<SolutionContainerManagerComponent>.op_Implicit(container)), (MetaDataComponent) null)} does not have a solution with ID: {name}");
      return false;
    }
label_12:
    SolutionComponent solutionComponent;
    if (!this.TryComp<SolutionComponent>(entityUid, ref solutionComponent))
    {
      entity = new Entity<SolutionComponent>?();
      if (!errorOnMissing)
        return false;
      this.Log.Error($"{this.ToPrettyString(new EntityUid?(Entity<SolutionContainerManagerComponent>.op_Implicit(container)), (MetaDataComponent) null)} does not have a solution with ID: {name}");
      return false;
    }
    entity = new Entity<SolutionComponent>?(Entity<SolutionComponent>.op_Implicit((entityUid, solutionComponent)));
    return true;
  }

  public bool TryGetSolution(
    SolutionContainerManagerComponent container,
    string name,
    [NotNullWhen(true)] out Solution? solution,
    bool errorOnMissing = false)
  {
    solution = (Solution) null;
    if (container.Solutions != null)
      return container.Solutions.TryGetValue(name, out solution);
    if (!errorOnMissing)
      return false;
    this.Log.Error($"{container} does not have a solution with ID: {name}");
    return false;
  }

  public IEnumerable<(string? Name, Entity<SolutionComponent> Solution)> EnumerateSolutions(
    Entity<SolutionContainerManagerComponent?> container,
    bool includeSelf = true)
  {
    SharedSolutionContainerSystem solutionContainerSystem = this;
    SolutionComponent solutionComponent;
    if (includeSelf && solutionContainerSystem.TryComp<SolutionComponent>(Entity<SolutionContainerManagerComponent>.op_Implicit(container), ref solutionComponent))
      yield return ((string) null, Entity<SolutionComponent>.op_Implicit((container.Owner, solutionComponent)));
    if (solutionContainerSystem.Resolve<SolutionContainerManagerComponent>(Entity<SolutionContainerManagerComponent>.op_Implicit(container), ref container.Comp, false))
    {
      foreach (string container1 in container.Comp.Containers)
      {
        SolutionAccessAttemptEvent accessAttemptEvent = new SolutionAccessAttemptEvent(container1);
        solutionContainerSystem.RaiseLocalEvent<SolutionAccessAttemptEvent>(Entity<SolutionContainerManagerComponent>.op_Implicit(container), ref accessAttemptEvent, false);
        if (!accessAttemptEvent.Cancelled && solutionContainerSystem.ContainerSystem.GetContainer(Entity<SolutionContainerManagerComponent>.op_Implicit(container), "solution@" + container1, (ContainerManagerComponent) null) is ContainerSlot container2)
        {
          EntityUid? containedEntity = container2.ContainedEntity;
          if (containedEntity.HasValue)
          {
            EntityUid valueOrDefault = containedEntity.GetValueOrDefault();
            yield return (container1, Entity<SolutionComponent>.op_Implicit((valueOrDefault, solutionContainerSystem.Comp<SolutionComponent>(valueOrDefault))));
          }
        }
      }
    }
  }

  public IEnumerable<(string Name, Solution Solution)> EnumerateSolutions(
    SolutionContainerManagerComponent container)
  {
    Dictionary<string, Solution> solutions = container.Solutions;
    if (solutions != null && solutions.Count > 0)
    {
      foreach ((string key, Solution solution) in solutions)
        yield return (key, solution);
    }
  }

  protected void UpdateAppearance(
    Entity<AppearanceComponent?> container,
    Entity<SolutionComponent, ContainedSolutionComponent> soln)
  {
    EntityUid entityUid1;
    AppearanceComponent appearanceComponent1;
    container.Deconstruct(ref entityUid1, ref appearanceComponent1);
    EntityUid entityUid2 = entityUid1;
    AppearanceComponent appearanceComponent2 = appearanceComponent1;
    if (!this.HasComp<SolutionContainerVisualsComponent>(entityUid2) || !this.Resolve<AppearanceComponent>(entityUid2, ref appearanceComponent2, false))
      return;
    SolutionComponent solutionComponent1;
    ContainedSolutionComponent solutionComponent2;
    soln.Deconstruct(ref entityUid1, ref solutionComponent1, ref solutionComponent2);
    SolutionComponent solutionComponent3 = solutionComponent1;
    ContainedSolutionComponent solutionComponent4 = solutionComponent2;
    Solution solution = solutionComponent3.Solution;
    this.AppearanceSystem.SetData(entityUid2, (Enum) SolutionContainerVisuals.FillFraction, (object) solution.FillFraction, appearanceComponent2);
    this.AppearanceSystem.SetData(entityUid2, (Enum) SolutionContainerVisuals.Color, (object) solution.GetColor(this.PrototypeManager), appearanceComponent2);
    this.AppearanceSystem.SetData(entityUid2, (Enum) SolutionContainerVisuals.SolutionName, (object) solutionComponent4.ContainerName, appearanceComponent2);
    ReagentId? primaryReagentId = solution.GetPrimaryReagentId();
    if (!primaryReagentId.HasValue)
      return;
    ReagentId valueOrDefault = primaryReagentId.GetValueOrDefault();
    this.AppearanceSystem.SetData(entityUid2, (Enum) SolutionContainerVisuals.BaseOverride, (object) valueOrDefault.ToString(), appearanceComponent2);
  }

  public FixedPoint2 GetTotalPrototypeQuantity(EntityUid owner, string reagentId)
  {
    FixedPoint2 prototypeQuantity = FixedPoint2.New(0);
    SolutionContainerManagerComponent managerComponent;
    if (this.Exists(owner) && this.TryComp<SolutionContainerManagerComponent>(owner, ref managerComponent))
    {
      foreach ((string Name, Entity<SolutionComponent> Solution) enumerateSolution in this.EnumerateSolutions(Entity<SolutionContainerManagerComponent>.op_Implicit((owner, managerComponent))))
      {
        Solution solution = enumerateSolution.Solution.Comp.Solution;
        prototypeQuantity += solution.GetTotalPrototypeQuantity(reagentId);
      }
    }
    return prototypeQuantity;
  }

  public void UpdateChemicals(
    Entity<SolutionComponent> soln,
    bool needsReactionsProcessing = true,
    ReactionMixerComponent? mixerComponent = null)
  {
    this.Dirty<SolutionComponent>(soln, (MetaDataComponent) null);
    EntityUid entityUid1;
    SolutionComponent solutionComponent1;
    soln.Deconstruct(ref entityUid1, ref solutionComponent1);
    EntityUid entityUid2 = entityUid1;
    SolutionComponent solutionComponent2 = solutionComponent1;
    Solution solution = solutionComponent2.Solution;
    if (needsReactionsProcessing && solution.CanReact)
      this.ChemicalReactionSystem.FullyReactSolution(soln, mixerComponent);
    FixedPoint2 Overflow = solution.Volume - solution.MaxVolume;
    if (Overflow > FixedPoint2.Zero)
    {
      SolutionOverflowEvent solutionOverflowEvent = new SolutionOverflowEvent(soln, Overflow);
      this.RaiseLocalEvent<SolutionOverflowEvent>(entityUid2, ref solutionOverflowEvent, false);
    }
    this.UpdateAppearance(Entity<SolutionComponent, AppearanceComponent>.op_Implicit((entityUid2, solutionComponent2, (AppearanceComponent) null)));
    SolutionChangedEvent solutionChangedEvent = new SolutionChangedEvent(soln);
    this.RaiseLocalEvent<SolutionChangedEvent>(entityUid2, ref solutionChangedEvent, false);
  }

  public void UpdateAppearance(
    Entity<SolutionComponent, AppearanceComponent?> soln)
  {
    EntityUid entityUid1;
    SolutionComponent solutionComponent1;
    AppearanceComponent appearanceComponent1;
    soln.Deconstruct(ref entityUid1, ref solutionComponent1, ref appearanceComponent1);
    EntityUid entityUid2 = entityUid1;
    SolutionComponent solutionComponent2 = solutionComponent1;
    AppearanceComponent appearanceComponent2 = appearanceComponent1;
    Solution solution = solutionComponent2.Solution;
    if (!this.Exists(entityUid2) || !this.Resolve<AppearanceComponent>(entityUid2, ref appearanceComponent2, false))
      return;
    this.AppearanceSystem.SetData(entityUid2, (Enum) SolutionContainerVisuals.FillFraction, (object) solution.FillFraction, appearanceComponent2);
    this.AppearanceSystem.SetData(entityUid2, (Enum) SolutionContainerVisuals.Color, (object) solution.GetColor(this.PrototypeManager), appearanceComponent2);
    ReagentId? primaryReagentId = solution.GetPrimaryReagentId();
    if (!primaryReagentId.HasValue)
      return;
    ReagentId valueOrDefault = primaryReagentId.GetValueOrDefault();
    this.AppearanceSystem.SetData(entityUid2, (Enum) SolutionContainerVisuals.BaseOverride, (object) valueOrDefault.ToString(), appearanceComponent2);
  }

  public Solution SplitSolution(Entity<SolutionComponent> soln, FixedPoint2 quantity)
  {
    EntityUid entityUid;
    SolutionComponent solutionComponent;
    soln.Deconstruct(ref entityUid, ref solutionComponent);
    Solution solution = solutionComponent.Solution.SplitSolution(quantity);
    this.UpdateChemicals(soln);
    return solution;
  }

  public Solution SplitStackSolution(
    Entity<SolutionComponent> soln,
    FixedPoint2 quantity,
    int stackCount)
  {
    EntityUid entityUid;
    SolutionComponent solutionComponent;
    soln.Deconstruct(ref entityUid, ref solutionComponent);
    Solution solution1 = solutionComponent.Solution;
    Solution solution2 = solution1.SplitSolution(quantity / (float) stackCount);
    solution1.SplitSolution(quantity - solution2.Volume);
    this.UpdateChemicals(soln);
    return solution2;
  }

  public Solution SplitSolutionWithout(
    Entity<SolutionComponent> soln,
    FixedPoint2 quantity,
    params string[] reagents)
  {
    EntityUid entityUid;
    SolutionComponent solutionComponent;
    soln.Deconstruct(ref entityUid, ref solutionComponent);
    Solution solution = solutionComponent.Solution.SplitSolutionWithout(quantity, reagents);
    this.UpdateChemicals(soln);
    return solution;
  }

  public void RemoveAllSolution(Entity<SolutionComponent> soln)
  {
    EntityUid entityUid;
    SolutionComponent solutionComponent;
    soln.Deconstruct(ref entityUid, ref solutionComponent);
    Solution solution = solutionComponent.Solution;
    if (solution.Volume == 0)
      return;
    solution.RemoveAllSolution();
    this.UpdateChemicals(soln);
  }

  public void SetCapacity(Entity<SolutionComponent> soln, FixedPoint2 capacity)
  {
    EntityUid entityUid;
    SolutionComponent solutionComponent;
    soln.Deconstruct(ref entityUid, ref solutionComponent);
    Solution solution = solutionComponent.Solution;
    if (solution.MaxVolume == capacity)
      return;
    solution.MaxVolume = capacity;
    this.UpdateChemicals(soln);
  }

  public bool TryAddReagent(
    Entity<SolutionComponent> soln,
    ReagentQuantity reagentQuantity,
    out FixedPoint2 acceptedQuantity,
    float? temperature = null)
  {
    EntityUid entityUid;
    SolutionComponent solutionComponent;
    soln.Deconstruct(ref entityUid, ref solutionComponent);
    Solution solution = solutionComponent.Solution;
    acceptedQuantity = solution.AvailableVolume > reagentQuantity.Quantity ? reagentQuantity.Quantity : solution.AvailableVolume;
    if (acceptedQuantity <= 0)
      return reagentQuantity.Quantity == 0;
    if (!temperature.HasValue)
    {
      solution.AddReagent(reagentQuantity.Reagent, acceptedQuantity);
    }
    else
    {
      Content.Shared._RMC14.Chemistry.Reagent.Reagent proto = this._rmcReagents.Index(ProtoId<ReagentPrototype>.op_Implicit(reagentQuantity.Reagent.Prototype));
      solution.AddReagent((ReagentPrototype) proto, acceptedQuantity, temperature.Value, this.PrototypeManager);
    }
    this.UpdateChemicals(soln);
    return acceptedQuantity == reagentQuantity.Quantity;
  }

  public bool TryAddReagent(
    Entity<SolutionComponent> soln,
    string prototype,
    FixedPoint2 quantity,
    float? temperature = null,
    List<ReagentData>? data = null)
  {
    return this.TryAddReagent(soln, new ReagentQuantity(prototype, quantity, data), out FixedPoint2 _, temperature);
  }

  public bool TryAddReagent(
    Entity<SolutionComponent> soln,
    string prototype,
    FixedPoint2 quantity,
    out FixedPoint2 acceptedQuantity,
    float? temperature = null,
    List<ReagentData>? data = null)
  {
    ReagentQuantity reagentQuantity = new ReagentQuantity(prototype, quantity, data);
    return this.TryAddReagent(soln, reagentQuantity, out acceptedQuantity, temperature);
  }

  public bool TryAddReagent(
    Entity<SolutionComponent> soln,
    ReagentId reagentId,
    FixedPoint2 quantity,
    out FixedPoint2 acceptedQuantity,
    float? temperature = null)
  {
    ReagentQuantity reagentQuantity = new ReagentQuantity(reagentId, quantity);
    return this.TryAddReagent(soln, reagentQuantity, out acceptedQuantity, temperature);
  }

  public bool RemoveReagent(Entity<SolutionComponent> soln, ReagentQuantity reagentQuantity)
  {
    EntityUid entityUid;
    SolutionComponent solutionComponent;
    soln.Deconstruct(ref entityUid, ref solutionComponent);
    if (solutionComponent.Solution.RemoveReagent(reagentQuantity) <= FixedPoint2.Zero)
      return false;
    this.UpdateChemicals(soln);
    return true;
  }

  public bool RemoveReagent(
    Entity<SolutionComponent> soln,
    string prototype,
    FixedPoint2 quantity,
    List<ReagentData>? data = null)
  {
    return this.RemoveReagent(soln, new ReagentQuantity(prototype, quantity, data));
  }

  public bool RemoveReagent(
    Entity<SolutionComponent> soln,
    ReagentId reagentId,
    FixedPoint2 quantity)
  {
    return this.RemoveReagent(soln, new ReagentQuantity(reagentId, quantity));
  }

  public bool TryTransferSolution(
    Entity<SolutionComponent> soln,
    Solution source,
    FixedPoint2 quantity)
  {
    EntityUid entityUid;
    SolutionComponent solutionComponent;
    soln.Deconstruct(ref entityUid, ref solutionComponent);
    Solution solution = solutionComponent.Solution;
    if (quantity < 0)
      throw new InvalidOperationException("Quantity must be positive");
    quantity = FixedPoint2.Min(quantity, solution.AvailableVolume, source.Volume);
    if (quantity == 0)
      return false;
    solution.AddSolution(source.SplitSolution(quantity), this.PrototypeManager);
    this.UpdateChemicals(soln);
    return true;
  }

  public bool TryAddSolution(Entity<SolutionComponent> soln, Solution toAdd)
  {
    EntityUid entityUid;
    SolutionComponent solutionComponent;
    soln.Deconstruct(ref entityUid, ref solutionComponent);
    Solution solution = solutionComponent.Solution;
    if (toAdd.Volume == FixedPoint2.Zero)
      return true;
    if (toAdd.Volume > solution.AvailableVolume)
      return false;
    this.ForceAddSolution(soln, toAdd);
    return true;
  }

  public FixedPoint2 AddSolution(Entity<SolutionComponent> soln, Solution toAdd)
  {
    EntityUid entityUid;
    SolutionComponent solutionComponent;
    soln.Deconstruct(ref entityUid, ref solutionComponent);
    Solution solution = solutionComponent.Solution;
    if (toAdd.Volume == FixedPoint2.Zero)
      return FixedPoint2.Zero;
    FixedPoint2 quantity = FixedPoint2.Max(FixedPoint2.Zero, FixedPoint2.Min(toAdd.Volume, solution.AvailableVolume));
    if (quantity < toAdd.Volume)
      this.TryTransferSolution(soln, toAdd, quantity);
    else
      this.ForceAddSolution(soln, toAdd);
    return quantity;
  }

  public bool ForceAddSolution(Entity<SolutionComponent> soln, Solution toAdd)
  {
    EntityUid entityUid;
    SolutionComponent solutionComponent;
    soln.Deconstruct(ref entityUid, ref solutionComponent);
    Solution solution = solutionComponent.Solution;
    if (toAdd.Volume == FixedPoint2.Zero)
      return false;
    solution.AddSolution(toAdd, this.PrototypeManager);
    this.UpdateChemicals(soln);
    return true;
  }

  public bool TryMixAndOverflow(
    Entity<SolutionComponent> soln,
    Solution toAdd,
    FixedPoint2 overflowThreshold,
    [MaybeNullWhen(false)] out Solution overflowingSolution)
  {
    EntityUid entityUid;
    SolutionComponent solutionComponent;
    soln.Deconstruct(ref entityUid, ref solutionComponent);
    Solution solution = solutionComponent.Solution;
    if (toAdd.Volume == 0 || overflowThreshold > solution.MaxVolume)
    {
      overflowingSolution = (Solution) null;
      return false;
    }
    solution.AddSolution(toAdd, this.PrototypeManager);
    overflowingSolution = solution.SplitSolution(FixedPoint2.Max(FixedPoint2.Zero, solution.Volume - overflowThreshold));
    this.UpdateChemicals(soln);
    return true;
  }

  public Solution RemoveEachReagent(Entity<SolutionComponent> soln, FixedPoint2 quantity)
  {
    EntityUid entityUid;
    SolutionComponent solutionComponent;
    soln.Deconstruct(ref entityUid, ref solutionComponent);
    Solution solution1 = solutionComponent.Solution;
    if (quantity <= 0)
      return new Solution();
    Solution solution2 = new Solution();
    for (int index = solution1.Contents.Count - 1; index >= 0; --index)
    {
      (ReagentId reagentId, FixedPoint2 _) = solution1.Contents[index];
      FixedPoint2 quantity1 = solution1.RemoveReagent(reagentId, quantity);
      solution2.AddReagent(reagentId, quantity1);
    }
    this.UpdateChemicals(soln);
    return solution2;
  }

  public void SetTemperature(Entity<SolutionComponent> soln, float temperature)
  {
    EntityUid entityUid;
    SolutionComponent solutionComponent;
    soln.Deconstruct(ref entityUid, ref solutionComponent);
    Solution solution = solutionComponent.Solution;
    if ((double) temperature == (double) solution.Temperature)
      return;
    solution.Temperature = temperature;
    this.UpdateChemicals(soln);
  }

  public void SetThermalEnergy(Entity<SolutionComponent> soln, float thermalEnergy)
  {
    EntityUid entityUid;
    SolutionComponent solutionComponent;
    soln.Deconstruct(ref entityUid, ref solutionComponent);
    Solution solution = solutionComponent.Solution;
    float heatCapacity = solution.GetHeatCapacity(this.PrototypeManager);
    solution.Temperature = (double) heatCapacity == 0.0 ? 0.0f : thermalEnergy / heatCapacity;
    this.UpdateChemicals(soln);
  }

  public void AddThermalEnergy(Entity<SolutionComponent> soln, float thermalEnergy)
  {
    EntityUid entityUid;
    SolutionComponent solutionComponent;
    soln.Deconstruct(ref entityUid, ref solutionComponent);
    Solution solution = solutionComponent.Solution;
    if ((double) thermalEnergy == 0.0)
      return;
    float heatCapacity = solution.GetHeatCapacity(this.PrototypeManager);
    solution.Temperature += (double) heatCapacity == 0.0 ? 0.0f : thermalEnergy / heatCapacity;
    this.UpdateChemicals(soln);
  }

  private void OnComponentInit(Entity<SolutionComponent> entity, ref ComponentInit args)
  {
    entity.Comp.Solution.ValidateSolution();
  }

  private void OnSolutionStartup(Entity<SolutionComponent> entity, ref ComponentStartup args)
  {
    this.UpdateChemicals(entity);
  }

  private void OnSolutionShutdown(Entity<SolutionComponent> entity, ref ComponentShutdown args)
  {
    this.RemoveAllSolution(entity);
  }

  private void OnContainerManagerInit(
    Entity<SolutionContainerManagerComponent> entity,
    ref ComponentInit args)
  {
    HashSet<string> containers = entity.Comp.Containers;
    if (containers == null || containers.Count <= 0)
      return;
    ContainerManagerComponent managerComponent = this.EnsureComp<ContainerManagerComponent>(Entity<SolutionContainerManagerComponent>.op_Implicit(entity));
    foreach (string str in containers)
      this.ContainerSystem.EnsureContainer<ContainerSlot>(entity.Owner, "solution@" + str, managerComponent);
  }

  private void OnExamineSolution(Entity<ExaminableSolutionComponent> entity, ref ExaminedEvent args)
  {
    Solution solution;
    if (!this.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(entity.Owner), entity.Comp.Solution, out Entity<SolutionComponent>? _, out solution) || !this.CanSeeHiddenSolution(entity, args.Examiner))
      return;
    ReagentId? primaryReagentId = solution.GetPrimaryReagentId();
    ReagentId valueOrDefault;
    string str1;
    if (!primaryReagentId.HasValue)
    {
      str1 = (string) null;
    }
    else
    {
      valueOrDefault = primaryReagentId.GetValueOrDefault();
      str1 = valueOrDefault.Prototype;
    }
    if (string.IsNullOrEmpty(str1))
    {
      args.PushText(this.Loc.GetString("shared-solution-container-component-on-examine-empty-container"));
    }
    else
    {
      RMCReagentSystem rmcReagents = this._rmcReagents;
      valueOrDefault = primaryReagentId.Value;
      ProtoId<ReagentPrototype> id = ProtoId<ReagentPrototype>.op_Implicit(valueOrDefault.Prototype);
      Content.Shared._RMC14.Chemistry.Reagent.Reagent reagent;
      ref Content.Shared._RMC14.Chemistry.Reagent.Reagent local = ref reagent;
      if (!rmcReagents.TryIndex(id, out local))
      {
        this.Log.Error($"{"Solution"} could not find the prototype associated with {primaryReagentId}.");
      }
      else
      {
        Color color = solution.GetColor(this.PrototypeManager);
        string hexNoAlpha = ((Color) ref color).ToHexNoAlpha();
        string str2 = "shared-solution-container-component-on-examine-main-text";
        using (args.PushGroup("ExaminableSolutionComponent"))
        {
          args.PushMarkup(this.Loc.GetString(str2, new (string, object)[3]
          {
            ("color", (object) hexNoAlpha),
            ("wordedAmount", (object) this.Loc.GetString(solution.Contents.Count == 1 ? "shared-solution-container-component-on-examine-worded-amount-one-reagent" : "shared-solution-container-component-on-examine-worded-amount-multiple-reagents")),
            ("desc", (object) reagent.LocalizedPhysicalDescription)
          }));
          IOrderedEnumerable<KeyValuePair<ReagentPrototype, FixedPoint2>> orderedEnumerable = solution.GetReagentPrototypes(this.PrototypeManager).OrderByDescending<KeyValuePair<ReagentPrototype, FixedPoint2>, int>((Func<KeyValuePair<ReagentPrototype, FixedPoint2>, int>) (pair => pair.Value.Value)).ThenBy<KeyValuePair<ReagentPrototype, FixedPoint2>, string>((Func<KeyValuePair<ReagentPrototype, FixedPoint2>, string>) (pair => pair.Key.LocalizedName));
          List<ReagentPrototype> reagentPrototypeList1 = new List<ReagentPrototype>();
          foreach (KeyValuePair<ReagentPrototype, FixedPoint2> keyValuePair in (IEnumerable<KeyValuePair<ReagentPrototype, FixedPoint2>>) orderedEnumerable)
          {
            ReagentPrototype key = keyValuePair.Key;
            if (key.Recognizable)
              reagentPrototypeList1.Add(key);
          }
          if (reagentPrototypeList1.Count == 0)
            return;
          StringBuilder stringBuilder1 = new StringBuilder();
          foreach (ReagentPrototype reagentPrototype1 in reagentPrototypeList1)
          {
            string str3;
            if (reagentPrototype1 == reagentPrototypeList1[0])
            {
              str3 = "examinable-solution-recognized-first";
            }
            else
            {
              ReagentPrototype reagentPrototype2 = reagentPrototype1;
              List<ReagentPrototype> reagentPrototypeList2 = reagentPrototypeList1;
              ReagentPrototype reagentPrototype3 = reagentPrototypeList2[reagentPrototypeList2.Count - 1];
              if (reagentPrototype2 == reagentPrototype3)
              {
                stringBuilder1.Append(' ');
                str3 = "examinable-solution-recognized-last";
              }
              else
                str3 = "examinable-solution-recognized-next";
            }
            StringBuilder stringBuilder2 = stringBuilder1;
            ILocalizationManager loc = this.Loc;
            string str4 = str3;
            Color substanceColor = reagentPrototype1.SubstanceColor;
            (string, object) valueTuple1 = ("color", (object) ((Color) ref substanceColor).ToHexNoAlpha());
            (string, object) valueTuple2 = ("chemical", (object) reagentPrototype1.LocalizedName);
            string str5 = loc.GetString(str4, valueTuple1, valueTuple2);
            stringBuilder2.Append(str5);
          }
          args.PushMarkup(this.Loc.GetString("examinable-solution-has-recognizable-chemicals", ("recognizedString", (object) stringBuilder1.ToString())));
        }
      }
    }
  }

  private void OnSolutionExaminableVerb(
    Entity<ExaminableSolutionComponent> entity,
    ref GetVerbsEvent<ExamineVerb> args)
  {
    if (!args.CanInteract || !args.CanAccess)
      return;
    SolutionScanEvent solutionScanEvent = new SolutionScanEvent();
    this.RaiseLocalEvent<SolutionScanEvent>(args.User, solutionScanEvent, false);
    Solution solutionHolder;
    if (!solutionScanEvent.CanScan || !this.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(args.Target), entity.Comp.Solution, out Entity<SolutionComponent>? _, out solutionHolder) || !this.CanSeeHiddenSolution(entity, args.User))
      return;
    EntityUid target = args.Target;
    EntityUid user = args.User;
    ExamineVerb examineVerb1 = new ExamineVerb();
    examineVerb1.Act = (Action) (() => this.ExamineSystem.SendExamineTooltip(user, target, this.GetSolutionExamine(solutionHolder), false, false));
    examineVerb1.Text = this.Loc.GetString("scannable-solution-verb-text");
    examineVerb1.Message = this.Loc.GetString("scannable-solution-verb-message");
    examineVerb1.Category = VerbCategory.Examine;
    examineVerb1.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/drink.svg.192dpi.png"));
    ExamineVerb examineVerb2 = examineVerb1;
    args.Verbs.Add(examineVerb2);
  }

  private FormattedMessage GetSolutionExamine(Solution solution)
  {
    FormattedMessage solutionExamine = new FormattedMessage();
    if (solution.Volume == 0)
    {
      solutionExamine.AddMarkupOrThrow(this.Loc.GetString("scannable-solution-empty-container"));
      return solutionExamine;
    }
    solutionExamine.AddMarkupOrThrow(this.Loc.GetString("scannable-solution-main-text"));
    foreach ((ReagentPrototype key, FixedPoint2 fixedPoint2) in (IEnumerable<KeyValuePair<ReagentPrototype, FixedPoint2>>) solution.GetReagentPrototypes(this.PrototypeManager).OrderByDescending<KeyValuePair<ReagentPrototype, FixedPoint2>, int>((Func<KeyValuePair<ReagentPrototype, FixedPoint2>, int>) (pair => pair.Value.Value)).ThenBy<KeyValuePair<ReagentPrototype, FixedPoint2>, string>((Func<KeyValuePair<ReagentPrototype, FixedPoint2>, string>) (pair => pair.Key.LocalizedName)))
    {
      solutionExamine.PushNewline();
      FormattedMessage formattedMessage = solutionExamine;
      ILocalizationManager loc = this.Loc;
      (string, object)[] valueTupleArray = new (string, object)[3];
      valueTupleArray[0] = ("type", (object) key.LocalizedName);
      Color substanceColor = key.SubstanceColor;
      valueTupleArray[1] = ("color", (object) ((Color) ref substanceColor).ToHexNoAlpha());
      valueTupleArray[2] = ("amount", (object) fixedPoint2);
      string str = loc.GetString("scannable-solution-chemical", valueTupleArray);
      formattedMessage.AddMarkupOrThrow(str);
    }
    solutionExamine.PushNewline();
    solutionExamine.AddMarkupOrThrow(this.Loc.GetString("scannable-solution-temperature", ("temperature", (object) Math.Round((double) solution.Temperature))));
    return solutionExamine;
  }

  private bool CanSeeHiddenSolution(Entity<ExaminableSolutionComponent> entity, EntityUid examiner)
  {
    return !entity.Comp.HeldOnly || this.Hands.IsHolding(Entity<HandsComponent>.op_Implicit(examiner), new EntityUid?(Entity<ExaminableSolutionComponent>.op_Implicit(entity)), out string _);
  }

  private void OnMapInit(Entity<SolutionContainerManagerComponent> entity, ref MapInitEvent args)
  {
    this.EnsureAllSolutions(entity);
  }

  private void OnContainerManagerShutdown(
    Entity<SolutionContainerManagerComponent> entity,
    ref ComponentShutdown args)
  {
    foreach (string container in entity.Comp.Containers)
    {
      BaseContainer baseContainer;
      if (this.ContainerSystem.TryGetContainer(Entity<SolutionContainerManagerComponent>.op_Implicit(entity), "solution@" + container, ref baseContainer, (ContainerManagerComponent) null))
        this.ContainerSystem.ShutdownContainer(baseContainer);
    }
    entity.Comp.Containers.Clear();
  }

  private void OnContainedSolutionShutdown(
    Entity<ContainedSolutionComponent> entity,
    ref ComponentShutdown args)
  {
    SolutionContainerManagerComponent managerComponent;
    if (this.TryComp<SolutionContainerManagerComponent>(entity.Comp.Container, ref managerComponent))
    {
      managerComponent.Containers.Remove(entity.Comp.ContainerName);
      this.Dirty(entity.Comp.Container, (IComponent) managerComponent, (MetaDataComponent) null);
    }
    BaseContainer baseContainer;
    if (!this.ContainerSystem.TryGetContainer(Entity<ContainedSolutionComponent>.op_Implicit(entity), "solution@" + entity.Comp.ContainerName, ref baseContainer, (ContainerManagerComponent) null))
      return;
    this.ContainerSystem.ShutdownContainer(baseContainer);
  }

  public bool EnsureSolution(
    Entity<MetaDataComponent?> entity,
    string name,
    [NotNullWhen(true)] out Solution? solution,
    FixedPoint2 maxVol = default (FixedPoint2))
  {
    return this.EnsureSolution(entity, name, maxVol, (Solution) null, out bool _, out solution);
  }

  public bool EnsureSolution(
    Entity<MetaDataComponent?> entity,
    string name,
    out bool existed,
    [NotNullWhen(true)] out Solution? solution,
    FixedPoint2 maxVol = default (FixedPoint2))
  {
    return this.EnsureSolution(entity, name, maxVol, (Solution) null, out existed, out solution);
  }

  public bool EnsureSolution(
    Entity<MetaDataComponent?> entity,
    string name,
    FixedPoint2 maxVol,
    Solution? prototype,
    out bool existed,
    [NotNullWhen(true)] out Solution? solution)
  {
    solution = (Solution) null;
    existed = false;
    EntityUid entityUid1;
    MetaDataComponent metaDataComponent1;
    entity.Deconstruct(ref entityUid1, ref metaDataComponent1);
    EntityUid entityUid2 = entityUid1;
    MetaDataComponent metaDataComponent2 = metaDataComponent1;
    SolutionContainerManagerComponent managerComponent = this.Resolve(entityUid2, ref metaDataComponent2, true) ? this.EnsureComp<SolutionContainerManagerComponent>(entityUid2) : throw new InvalidOperationException("Attempted to ensure solution on invalid entity.");
    if (metaDataComponent2.EntityLifeStage >= 3)
    {
      Entity<SolutionComponent>? solutionEntity;
      this.EnsureSolutionEntity(Entity<SolutionContainerManagerComponent>.op_Implicit((entityUid2, managerComponent)), name, out existed, out solutionEntity, maxVol, prototype);
      solution = solutionEntity.Value.Comp.Solution;
      return true;
    }
    solution = this.EnsureSolutionPrototype(Entity<SolutionContainerManagerComponent>.op_Implicit((entityUid2, managerComponent)), name, maxVol, prototype, out existed);
    return true;
  }

  public void EnsureAllSolutions(Entity<SolutionContainerManagerComponent> entity)
  {
    if (this.NetManager.IsClient)
      return;
    Dictionary<string, Solution> solutions = entity.Comp.Solutions;
    if (solutions == null)
      return;
    foreach ((string str, Solution prototype) in solutions)
      this.EnsureSolutionEntity(Entity<SolutionContainerManagerComponent>.op_Implicit((entity.Owner, entity.Comp)), str, out bool _, out Entity<SolutionComponent>? _, prototype.MaxVolume, prototype);
    entity.Comp.Solutions = (Dictionary<string, Solution>) null;
    this.Dirty<SolutionContainerManagerComponent>(entity, (MetaDataComponent) null);
  }

  public bool EnsureSolutionEntity(
    Entity<SolutionContainerManagerComponent?> entity,
    string name,
    [NotNullWhen(true)] out Entity<SolutionComponent>? solutionEntity,
    FixedPoint2 maxVol = default (FixedPoint2))
  {
    return this.EnsureSolutionEntity(entity, name, out bool _, out solutionEntity, maxVol);
  }

  public bool EnsureSolutionEntity(
    Entity<SolutionContainerManagerComponent?> entity,
    string name,
    out bool existed,
    [NotNullWhen(true)] out Entity<SolutionComponent>? solutionEntity,
    FixedPoint2 maxVol = default (FixedPoint2),
    Solution? prototype = null)
  {
    existed = true;
    solutionEntity = new Entity<SolutionComponent>?();
    EntityUid entityUid1;
    SolutionContainerManagerComponent managerComponent1;
    entity.Deconstruct(ref entityUid1, ref managerComponent1);
    EntityUid entityUid2 = entityUid1;
    SolutionContainerManagerComponent managerComponent2 = managerComponent1;
    ContainerSlot container = this.ContainerSystem.EnsureContainer<ContainerSlot>(entityUid2, "solution@" + name, ref existed, (ContainerManagerComponent) null);
    if (!this.Resolve<SolutionContainerManagerComponent>(entityUid2, ref managerComponent2, false))
    {
      existed = false;
      managerComponent2 = this.AddComp<SolutionContainerManagerComponent>(entityUid2);
      managerComponent2.Containers.Add(name);
      if (this.NetManager.IsClient)
        return false;
    }
    else if (!existed)
    {
      managerComponent2.Containers.Add(name);
      this.Dirty(entityUid2, (IComponent) managerComponent2, (MetaDataComponent) null);
    }
    bool flag = false;
    EntityUid? containedEntity = container.ContainedEntity;
    EntityUid entityUid3;
    SolutionComponent solutionComponent1;
    if (containedEntity.HasValue)
    {
      entityUid3 = containedEntity.GetValueOrDefault();
      solutionComponent1 = this.Comp<SolutionComponent>(entityUid3);
      Solution solution = solutionComponent1.Solution;
      solution.MaxVolume = FixedPoint2.Max(solution.MaxVolume, maxVol);
      if (prototype != null && prototype.Volume.Value > 0)
        solution.AddSolution(prototype, this.PrototypeManager);
      this.Dirty(entityUid3, (IComponent) solutionComponent1, (MetaDataComponent) null);
    }
    else
    {
      if (this.NetManager.IsClient)
        return false;
      if (prototype == null)
        prototype = new Solution() { MaxVolume = maxVol };
      prototype.Name = name;
      SolutionComponent solutionComponent2;
      ContainedSolutionComponent solutionComponent3;
      this.SpawnSolutionUninitialized(container, name, maxVol, prototype).Deconstruct(ref entityUid1, ref solutionComponent2, ref solutionComponent3);
      entityUid3 = entityUid1;
      solutionComponent1 = solutionComponent2;
      existed = false;
      flag = true;
      this.Dirty(entityUid2, (IComponent) managerComponent2, (MetaDataComponent) null);
    }
    if (flag)
      this.EntityManager.InitializeAndStartEntity(entityUid3, new MapId?(this.Transform(entityUid3).MapID));
    solutionEntity = new Entity<SolutionComponent>?(Entity<SolutionComponent>.op_Implicit((entityUid3, solutionComponent1)));
    return true;
  }

  private Solution EnsureSolutionPrototype(
    Entity<SolutionContainerManagerComponent?> entity,
    string name,
    FixedPoint2 maxVol,
    Solution? prototype,
    out bool existed)
  {
    existed = true;
    EntityUid entityUid1;
    SolutionContainerManagerComponent managerComponent1;
    entity.Deconstruct(ref entityUid1, ref managerComponent1);
    EntityUid entityUid2 = entityUid1;
    SolutionContainerManagerComponent managerComponent2 = managerComponent1;
    if (!this.Resolve<SolutionContainerManagerComponent>(entityUid2, ref managerComponent2, false))
    {
      managerComponent2 = this.AddComp<SolutionContainerManagerComponent>(entityUid2);
      existed = false;
    }
    if (managerComponent2.Solutions == null)
      managerComponent2.Solutions = new Dictionary<string, Solution>(2);
    Solution solution1;
    if (!managerComponent2.Solutions.TryGetValue(name, out solution1))
    {
      Solution solution2 = prototype;
      if (solution2 == null)
        solution2 = new Solution()
        {
          Name = name,
          MaxVolume = maxVol
        };
      solution1 = solution2;
      managerComponent2.Solutions.Add(name, solution1);
      existed = false;
    }
    else
      solution1.MaxVolume = FixedPoint2.Max(solution1.MaxVolume, maxVol);
    this.Dirty(entityUid2, (IComponent) managerComponent2, (MetaDataComponent) null);
    return solution1;
  }

  private Entity<SolutionComponent, ContainedSolutionComponent> SpawnSolutionUninitialized(
    ContainerSlot container,
    string name,
    FixedPoint2 maxVol,
    Solution prototype)
  {
    EntityCoordinates entityCoordinates;
    // ISSUE: explicit constructor call
    ((EntityCoordinates) ref entityCoordinates).\u002Ector(((BaseContainer) container).Owner, Vector2.Zero);
    EntityUid entityUninitialized = this.EntityManager.CreateEntityUninitialized((string) null, entityCoordinates, (ComponentRegistry) null, new Angle());
    SolutionComponent solutionComponent1 = new SolutionComponent()
    {
      Solution = prototype
    };
    this.AddComp<SolutionComponent>(entityUninitialized, solutionComponent1, false);
    ContainedSolutionComponent solutionComponent2 = new ContainedSolutionComponent()
    {
      Container = ((BaseContainer) container).Owner,
      ContainerName = name
    };
    this.AddComp<ContainedSolutionComponent>(entityUninitialized, solutionComponent2, false);
    this.MetaDataSys.SetEntityName(entityUninitialized, "solution - " + name, (MetaDataComponent) null, true);
    this.ContainerSystem.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(entityUninitialized), (BaseContainer) container, (TransformComponent) null, true);
    return Entity<SolutionComponent, ContainedSolutionComponent>.op_Implicit((entityUninitialized, solutionComponent1, solutionComponent2));
  }

  public void AdjustDissolvedReagent(
    Entity<SolutionComponent> dissolvedSolution,
    FixedPoint2 volume,
    ReagentId reagent,
    float concentrationChange)
  {
    if ((double) concentrationChange == 0.0)
      return;
    Solution solution = dissolvedSolution.Comp.Solution;
    FixedPoint2 fromConcentration = this.GetReagentQuantityFromConcentration(dissolvedSolution, volume, MathF.Abs(concentrationChange));
    if ((double) concentrationChange > 0.0)
      solution.AddReagent(reagent, fromConcentration);
    else
      solution.RemoveReagent(reagent, fromConcentration);
    this.UpdateChemicals(dissolvedSolution);
  }

  public FixedPoint2 GetReagentQuantityFromConcentration(
    Entity<SolutionComponent> dissolvedSolution,
    FixedPoint2 volume,
    float concentration)
  {
    Solution solution = dissolvedSolution.Comp.Solution;
    return volume == 0 || solution.Volume == 0 ? (FixedPoint2) 0 : (FixedPoint2) concentration * volume;
  }

  public float GetReagentConcentration(
    Entity<SolutionComponent> dissolvedSolution,
    FixedPoint2 volume,
    ReagentId dissolvedReagent)
  {
    Solution solution = dissolvedSolution.Comp.Solution;
    FixedPoint2 volume1;
    return volume == 0 || solution.Volume == 0 || !solution.TryGetReagentQuantity(dissolvedReagent, out volume1) ? 0.0f : (float) volume1 / volume.Float();
  }

  public FixedPoint2 ClampReagentAmountByConcentration(
    Entity<SolutionComponent> dissolvedSolution,
    FixedPoint2 volume,
    ReagentId dissolvedReagent,
    FixedPoint2 dissolvedReagentAmount,
    float maxConcentration = 1f)
  {
    Solution solution = dissolvedSolution.Comp.Solution;
    FixedPoint2 volume1;
    if (volume == 0 || solution.Volume == 0 || !solution.TryGetReagentQuantity(dissolvedReagent, out volume1))
      return (FixedPoint2) 0;
    volume *= maxConcentration;
    FixedPoint2 fixedPoint2_1 = volume1 + dissolvedReagentAmount;
    FixedPoint2 fixedPoint2_2 = volume - fixedPoint2_1;
    if (fixedPoint2_2 < 0)
      dissolvedReagentAmount += fixedPoint2_2;
    return dissolvedReagentAmount;
  }

  protected void InitializeRelays()
  {
    SharedSolutionContainerSystem solutionContainerSystem1 = this;
    // ISSUE: virtual method pointer
    this.SubscribeLocalEvent<ContainedSolutionComponent, SolutionChangedEvent>(new EntityEventRefHandler<ContainedSolutionComponent, SolutionChangedEvent>((object) solutionContainerSystem1, __vmethodptr(solutionContainerSystem1, OnSolutionChanged)), (Type[]) null, (Type[]) null);
    SharedSolutionContainerSystem solutionContainerSystem2 = this;
    // ISSUE: virtual method pointer
    this.SubscribeLocalEvent<ContainedSolutionComponent, SolutionOverflowEvent>(new EntityEventRefHandler<ContainedSolutionComponent, SolutionOverflowEvent>((object) solutionContainerSystem2, __vmethodptr(solutionContainerSystem2, OnSolutionOverflow)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ContainedSolutionComponent, ReactionAttemptEvent>(new EntityEventRefHandler<ContainedSolutionComponent, ReactionAttemptEvent>((object) this, __methodptr(RelaySolutionRefEvent<ReactionAttemptEvent>)), (Type[]) null, (Type[]) null);
  }

  protected virtual void OnSolutionChanged(
    Entity<ContainedSolutionComponent> entity,
    ref SolutionChangedEvent args)
  {
    EntityUid entityUid1;
    SolutionComponent solutionComponent1;
    args.Solution.Deconstruct(ref entityUid1, ref solutionComponent1);
    EntityUid entityUid2 = entityUid1;
    SolutionComponent solutionComponent2 = solutionComponent1;
    Solution solution = solutionComponent2.Solution;
    this.UpdateAppearance(Entity<AppearanceComponent>.op_Implicit(entity.Comp.Container), Entity<SolutionComponent, ContainedSolutionComponent>.op_Implicit((entityUid2, solutionComponent2, entity.Comp)));
    SolutionContainerChangedEvent containerChangedEvent = new SolutionContainerChangedEvent(solution, entity.Comp.ContainerName);
    this.RaiseLocalEvent<SolutionContainerChangedEvent>(entity.Comp.Container, ref containerChangedEvent, false);
  }

  protected virtual void OnSolutionOverflow(
    Entity<ContainedSolutionComponent> entity,
    ref SolutionOverflowEvent args)
  {
    Solution solution = args.Solution.Comp.Solution;
    Solution Overflow = solution.SplitSolution(args.Overflow);
    SolutionContainerOverflowEvent containerOverflowEvent = new SolutionContainerOverflowEvent(entity.Owner, solution, Overflow)
    {
      Handled = args.Handled
    };
    this.RaiseLocalEvent<SolutionContainerOverflowEvent>(entity.Comp.Container, ref containerOverflowEvent, false);
    args.Handled = containerOverflowEvent.Handled;
  }

  private void RelaySolutionValEvent<TEvent>(
    EntityUid uid,
    ContainedSolutionComponent comp,
    TEvent @event)
  {
    SolutionRelayEvent<TEvent> solutionRelayEvent = new SolutionRelayEvent<TEvent>(@event, uid, comp.ContainerName);
    this.RaiseLocalEvent<SolutionRelayEvent<TEvent>>(comp.Container, ref solutionRelayEvent, false);
  }

  private void RelaySolutionRefEvent<TEvent>(
    Entity<ContainedSolutionComponent> entity,
    ref TEvent @event)
  {
    SolutionRelayEvent<TEvent> solutionRelayEvent = new SolutionRelayEvent<TEvent>(@event, entity.Owner, entity.Comp.ContainerName);
    this.RaiseLocalEvent<SolutionRelayEvent<TEvent>>(entity.Comp.Container, ref solutionRelayEvent, false);
    @event = solutionRelayEvent.Event;
  }

  private void RelaySolutionContainerEvent<TEvent>(
    EntityUid uid,
    SolutionContainerManagerComponent comp,
    TEvent @event)
  {
    foreach ((string Name, Entity<SolutionComponent> entity) in this.EnumerateSolutions(Entity<SolutionContainerManagerComponent>.op_Implicit((uid, comp))))
    {
      SolutionContainerRelayEvent<TEvent> containerRelayEvent = new SolutionContainerRelayEvent<TEvent>(@event, entity, Name);
      this.RaiseLocalEvent<SolutionContainerRelayEvent<TEvent>>(Entity<SolutionComponent>.op_Implicit(entity), ref containerRelayEvent, false);
    }
  }

  private void RelaySolutionContainerEvent<TEvent>(
    Entity<SolutionContainerManagerComponent> entity,
    ref TEvent @event)
  {
    foreach ((string Name, Entity<SolutionComponent> entity1) in this.EnumerateSolutions(Entity<SolutionContainerManagerComponent>.op_Implicit((entity.Owner, entity.Comp))))
    {
      SolutionContainerRelayEvent<TEvent> containerRelayEvent = new SolutionContainerRelayEvent<TEvent>(@event, entity1, Name);
      this.RaiseLocalEvent<SolutionContainerRelayEvent<TEvent>>(Entity<SolutionComponent>.op_Implicit(entity1), ref containerRelayEvent, false);
      @event = containerRelayEvent.Event;
    }
  }
}
