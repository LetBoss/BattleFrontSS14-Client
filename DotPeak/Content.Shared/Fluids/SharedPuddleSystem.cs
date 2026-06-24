// Decompiled with JetBrains decompiler
// Type: Content.Shared.Fluids.SharedPuddleSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Fluids.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Nutrition.EntitySystems;
using Content.Shared.Spillable;
using Content.Shared.StepTrigger.Components;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Melee;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Fluids;

public abstract class SharedPuddleSystem : EntitySystem
{
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedSolutionContainerSystem _solutionContainerSystem;
  [Dependency]
  private SharedDoAfterSystem _doAfterSystem;
  [Dependency]
  private RMCReagentSystem _rmcReagents;
  private static readonly ProtoId<ReagentPrototype> Blood = (ProtoId<ReagentPrototype>) nameof (Blood);
  private static readonly ProtoId<ReagentPrototype> Slime = (ProtoId<ReagentPrototype>) nameof (Slime);
  private static readonly ProtoId<ReagentPrototype> CopperBlood = (ProtoId<ReagentPrototype>) nameof (CopperBlood);
  private static readonly string[] StandoutReagents = new string[3]
  {
    (string) SharedPuddleSystem.Blood,
    (string) SharedPuddleSystem.Slime,
    (string) SharedPuddleSystem.CopperBlood
  };
  public const float LowThreshold = 0.3f;
  public const float MediumThreshold = 0.6f;
  [Dependency]
  protected OpenableSystem Openable;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RefillableSolutionComponent, CanDragEvent>(new EntityEventRefHandler<RefillableSolutionComponent, CanDragEvent>(this.OnRefillableCanDrag));
    this.SubscribeLocalEvent<DumpableSolutionComponent, CanDropTargetEvent>(new EntityEventRefHandler<DumpableSolutionComponent, CanDropTargetEvent>(this.OnDumpCanDropTarget));
    this.SubscribeLocalEvent<DrainableSolutionComponent, CanDropTargetEvent>(new EntityEventRefHandler<DrainableSolutionComponent, CanDropTargetEvent>(this.OnDrainCanDropTarget));
    this.SubscribeLocalEvent<RefillableSolutionComponent, CanDropDraggedEvent>(new EntityEventRefHandler<RefillableSolutionComponent, CanDropDraggedEvent>(this.OnRefillableCanDropDragged));
    this.SubscribeLocalEvent<PuddleComponent, SolutionContainerChangedEvent>(new EntityEventRefHandler<PuddleComponent, SolutionContainerChangedEvent>(this.OnSolutionUpdate));
    this.SubscribeLocalEvent<PuddleComponent, GetFootstepSoundEvent>(new EntityEventRefHandler<PuddleComponent, GetFootstepSoundEvent>(this.OnGetFootstepSound));
    this.SubscribeLocalEvent<PuddleComponent, ExaminedEvent>(new EntityEventRefHandler<PuddleComponent, ExaminedEvent>(this.HandlePuddleExamined));
    this.SubscribeLocalEvent<PuddleComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<PuddleComponent, EntRemovedFromContainerMessage>(this.OnEntRemoved));
    this.InitializeSpillable();
  }

  protected virtual void OnSolutionUpdate(
    Entity<PuddleComponent> entity,
    ref SolutionContainerChangedEvent args)
  {
    if (args.SolutionId != entity.Comp.SolutionName)
      return;
    this.UpdateAppearance((Entity<PuddleComponent, AppearanceComponent>) ((EntityUid) entity, entity.Comp));
  }

  private void OnRefillableCanDrag(
    Entity<RefillableSolutionComponent> entity,
    ref CanDragEvent args)
  {
    args.Handled = true;
  }

  private void OnDumpCanDropTarget(
    Entity<DumpableSolutionComponent> entity,
    ref CanDropTargetEvent args)
  {
    if (!this.HasComp<DrainableSolutionComponent>(args.Dragged))
      return;
    args.CanDrop = true;
    args.Handled = true;
  }

  private void OnDrainCanDropTarget(
    Entity<DrainableSolutionComponent> entity,
    ref CanDropTargetEvent args)
  {
    if (!this.HasComp<RefillableSolutionComponent>(args.Dragged))
      return;
    args.CanDrop = true;
    args.Handled = true;
  }

  private void OnRefillableCanDropDragged(
    Entity<RefillableSolutionComponent> entity,
    ref CanDropDraggedEvent args)
  {
    if (!this.HasComp<DrainableSolutionComponent>(args.Target) && !this.HasComp<DumpableSolutionComponent>(args.Target))
      return;
    args.CanDrop = true;
    args.Handled = true;
  }

  private void OnGetFootstepSound(Entity<PuddleComponent> entity, ref GetFootstepSoundEvent args)
  {
    Solution solution;
    if (!this._solutionContainerSystem.ResolveSolution((Entity<SolutionContainerManagerComponent>) entity.Owner, entity.Comp.SolutionName, ref entity.Comp.Solution, out solution))
      return;
    ReagentId? primaryReagentId = solution.GetPrimaryReagentId();
    ReagentId valueOrDefault;
    string str;
    if (!primaryReagentId.HasValue)
    {
      str = (string) null;
    }
    else
    {
      valueOrDefault = primaryReagentId.GetValueOrDefault();
      str = valueOrDefault.Prototype;
    }
    if (string.IsNullOrWhiteSpace(str))
      return;
    RMCReagentSystem rmcReagents = this._rmcReagents;
    valueOrDefault = primaryReagentId.Value;
    ProtoId<ReagentPrototype> prototype = (ProtoId<ReagentPrototype>) valueOrDefault.Prototype;
    Content.Shared._RMC14.Chemistry.Reagent.Reagent reagent;
    ref Content.Shared._RMC14.Chemistry.Reagent.Reagent local = ref reagent;
    if (!rmcReagents.TryIndex(prototype, out local))
      return;
    args.Sound = reagent.FootstepSound;
  }

  private void HandlePuddleExamined(Entity<PuddleComponent> entity, ref ExaminedEvent args)
  {
    using (args.PushGroup("PuddleComponent"))
    {
      StepTriggerComponent comp;
      if (this.TryComp<StepTriggerComponent>((EntityUid) entity, out comp) && comp.Active)
        args.PushMarkup(this.Loc.GetString("puddle-component-examine-is-slippery-text"));
      Solution solution;
      if (this.HasComp<EvaporationComponent>((EntityUid) entity) && this._solutionContainerSystem.ResolveSolution((Entity<SolutionContainerManagerComponent>) entity.Owner, entity.Comp.SolutionName, ref entity.Comp.Solution, out solution))
      {
        if (this.CanFullyEvaporate(solution))
          args.PushMarkup(this.Loc.GetString("puddle-component-examine-evaporating"));
        else if (solution.GetTotalPrototypeQuantity(this.GetEvaporatingReagents(solution)) > FixedPoint2.Zero)
          args.PushMarkup(this.Loc.GetString("puddle-component-examine-evaporating-partial"));
        else
          args.PushMarkup(this.Loc.GetString("puddle-component-examine-evaporating-no"));
      }
      else
        args.PushMarkup(this.Loc.GetString("puddle-component-examine-evaporating-no"));
    }
  }

  private void OnEntRemoved(Entity<PuddleComponent> ent, ref EntRemovedFromContainerMessage args)
  {
    EntityUid entity = args.Entity;
    ref Entity<SolutionComponent>? local = ref ent.Comp.Solution;
    EntityUid? nullable = local.HasValue ? new EntityUid?(local.GetValueOrDefault().Owner) : new EntityUid?();
    if ((nullable.HasValue ? (entity == nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0)
      return;
    ent.Comp.Solution = new Entity<SolutionComponent>?();
  }

  private void UpdateAppearance(Entity<PuddleComponent?, AppearanceComponent?> ent)
  {
    (EntityUid entityUid, PuddleComponent comp1, AppearanceComponent comp2) = ent;
    if (!this.Resolve<PuddleComponent, AppearanceComponent>((EntityUid) ent, ref comp1, ref comp2))
      return;
    FixedPoint2 fixedPoint2 = FixedPoint2.Zero;
    Color color = Color.White;
    Solution solution;
    if (this._solutionContainerSystem.ResolveSolution((Entity<SolutionContainerManagerComponent>) entityUid, comp1.SolutionName, ref comp1.Solution, out solution))
    {
      fixedPoint2 = solution.Volume / comp1.OverflowVolume;
      color = solution.GetColorWithout(this._prototypeManager, SharedPuddleSystem.StandoutReagents);
      color = ((Color) ref color).WithAlpha(0.7f);
      foreach (string standoutReagent in SharedPuddleSystem.StandoutReagents)
      {
        FixedPoint2 prototypeQuantity = solution.GetTotalPrototypeQuantity(standoutReagent);
        if (!(prototypeQuantity <= FixedPoint2.Zero))
        {
          float num = prototypeQuantity.Float() / solution.Volume.Float();
          color = Color.InterpolateBetween(color, this._rmcReagents.Index((ProtoId<ReagentPrototype>) standoutReagent).SubstanceColor, num);
        }
      }
    }
    this._appearance.SetData((EntityUid) ent, (Enum) PuddleVisuals.CurrentVolume, (object) fixedPoint2.Float(), comp2);
    this._appearance.SetData((EntityUid) ent, (Enum) PuddleVisuals.SolutionColor, (object) color, comp2);
  }

  public void DoTileReactions(TileRef tileRef, Solution solution)
  {
    for (int index = solution.Contents.Count - 1; index >= 0; --index)
    {
      (ReagentId reagentId, FixedPoint2 fixedPoint2) = solution.Contents[index];
      FixedPoint2 quantity = this._rmcReagents.Index((ProtoId<ReagentPrototype>) reagentId.Prototype).ReactionTile(tileRef, fixedPoint2, (IEntityManager) this.EntityManager, reagentId.Data);
      if (!(quantity <= FixedPoint2.Zero))
        solution.RemoveReagent(reagentId, quantity);
    }
  }

  public abstract bool TrySplashSpillAt(
    EntityUid uid,
    EntityCoordinates coordinates,
    Solution solution,
    out EntityUid puddleUid,
    bool sound = true,
    EntityUid? user = null);

  public abstract bool TrySpillAt(
    EntityCoordinates coordinates,
    Solution solution,
    out EntityUid puddleUid,
    bool sound = true);

  public abstract bool TrySpillAt(
    EntityUid uid,
    Solution solution,
    out EntityUid puddleUid,
    bool sound = true,
    TransformComponent? transformComponent = null);

  public abstract bool TrySpillAt(
    TileRef tileRef,
    Solution solution,
    out EntityUid puddleUid,
    bool sound = true,
    bool tileReact = true);

  public string[] GetEvaporatingReagents(Solution solution)
  {
    List<string> stringList = new List<string>();
    foreach (ReagentPrototype key in solution.GetReagentPrototypes(this._prototypeManager).Keys)
    {
      if (key.EvaporationSpeed > FixedPoint2.Zero)
        stringList.Add(key.ID);
    }
    return stringList.ToArray();
  }

  public string[] GetAbsorbentReagents(Solution solution)
  {
    List<string> stringList = new List<string>();
    foreach (ReagentPrototype key in solution.GetReagentPrototypes(this._prototypeManager).Keys)
    {
      if (key.Absorbent)
        stringList.Add(key.ID);
    }
    return stringList.ToArray();
  }

  public bool CanFullyEvaporate(Solution solution)
  {
    return solution.GetTotalPrototypeQuantity(this.GetEvaporatingReagents(solution)) == solution.Volume;
  }

  public Dictionary<string, FixedPoint2> GetEvaporationSpeeds(Solution solution)
  {
    Dictionary<string, FixedPoint2> evaporationSpeeds = new Dictionary<string, FixedPoint2>();
    foreach (ReagentPrototype key in solution.GetReagentPrototypes(this._prototypeManager).Keys)
    {
      if (key.EvaporationSpeed > FixedPoint2.Zero)
        evaporationSpeeds.Add(key.ID, key.EvaporationSpeed);
    }
    return evaporationSpeeds;
  }

  protected virtual void InitializeSpillable()
  {
    this.SubscribeLocalEvent<SpillableComponent, ExaminedEvent>(new EntityEventRefHandler<SpillableComponent, ExaminedEvent>(this.OnExamined));
    this.SubscribeLocalEvent<SpillableComponent, GetVerbsEvent<Verb>>(new EntityEventRefHandler<SpillableComponent, GetVerbsEvent<Verb>>(this.AddSpillVerb));
  }

  private void OnExamined(Entity<SpillableComponent> entity, ref ExaminedEvent args)
  {
    using (args.PushGroup("SpillableComponent"))
    {
      args.PushMarkup(this.Loc.GetString("spill-examine-is-spillable"));
      if (!this.HasComp<MeleeWeaponComponent>((EntityUid) entity))
        return;
      args.PushMarkup(this.Loc.GetString("spill-examine-spillable-weapon"));
    }
  }

  private void AddSpillVerb(Entity<SpillableComponent> entity, ref GetVerbsEvent<Verb> args)
  {
    Solution solution;
    if (!args.CanAccess || !args.CanInteract || args.Hands == null || !this._solutionContainerSystem.TryGetSolution((Entity<SolutionContainerManagerComponent>) args.Target, entity.Comp.SolutionName, out Entity<SolutionComponent>? _, out solution) || this.Openable.IsClosed(args.Target) || solution.Volume == FixedPoint2.Zero)
      return;
    Verb verb = new Verb()
    {
      Text = this.Loc.GetString("spill-target-verb-get-data-text")
    };
    if (!entity.Comp.SpillDelay.HasValue)
    {
      EntityUid target = args.Target;
      verb.Act = (Action) (() =>
      {
        Solution solution1 = this._solutionContainerSystem.SplitSolution(soln.Value, solution.Volume);
        this.TrySpillAt(this.Transform(target).Coordinates, solution1, out EntityUid _);
        InjectorComponent comp;
        if (!this.TryComp<InjectorComponent>((EntityUid) entity, out comp))
          return;
        comp.ToggleState = InjectorToggleMode.Draw;
        this.Dirty((EntityUid) entity, (IComponent) comp);
      });
    }
    else
    {
      EntityUid user = args.User;
      verb.Act = (Action) (() => this._doAfterSystem.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, entity.Comp.SpillDelay.GetValueOrDefault(), (DoAfterEvent) new SpillDoAfterEvent(), new EntityUid?(entity.Owner), new EntityUid?(entity.Owner))
      {
        BreakOnDamage = true,
        BreakOnMove = true,
        NeedHand = true
      }));
    }
    verb.Impact = LogImpact.Medium;
    verb.DoContactInteraction = new bool?(true);
    args.Verbs.Add(verb);
  }
}
