// Decompiled with JetBrains decompiler
// Type: Content.Shared.Fluids.SharedAbsorbentSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.FixedPoint;
using Content.Shared.Fluids.Components;
using Content.Shared.Interaction;
using Content.Shared.Item;
using Content.Shared.Popups;
using Content.Shared.Timing;
using Content.Shared.Weapons.Melee;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared.Fluids;

public abstract class SharedAbsorbentSystem : EntitySystem
{
  [Dependency]
  private IPrototypeManager _proto;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedPopupSystem _popups;
  [Dependency]
  protected SharedPuddleSystem Puddle;
  [Dependency]
  private SharedMeleeWeaponSystem _melee;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  protected SharedSolutionContainerSystem SolutionContainer;
  [Dependency]
  private UseDelaySystem _useDelay;
  [Dependency]
  private SharedMapSystem _mapSystem;
  [Dependency]
  private SharedItemSystem _item;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<AbsorbentComponent, AfterInteractEvent>(new EntityEventRefHandler<AbsorbentComponent, AfterInteractEvent>(this.OnAfterInteract));
    this.SubscribeLocalEvent<AbsorbentComponent, UserActivateInWorldEvent>(new EntityEventRefHandler<AbsorbentComponent, UserActivateInWorldEvent>(this.OnActivateInWorld));
    this.SubscribeLocalEvent<AbsorbentComponent, SolutionContainerChangedEvent>(new EntityEventRefHandler<AbsorbentComponent, SolutionContainerChangedEvent>(this.OnAbsorbentSolutionChange));
  }

  private void OnActivateInWorld(Entity<AbsorbentComponent> ent, ref UserActivateInWorldEvent args)
  {
    if (args.Handled)
      return;
    this.Mop(ent, args.User, args.Target);
    args.Handled = true;
  }

  private void OnAfterInteract(Entity<AbsorbentComponent> ent, ref AfterInteractEvent args)
  {
    if (!args.CanReach || args.Handled)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    this.Mop(ent, args.User, valueOrDefault);
    args.Handled = true;
  }

  private void OnAbsorbentSolutionChange(
    Entity<AbsorbentComponent> ent,
    ref SolutionContainerChangedEvent args)
  {
    Solution solution;
    if (!this.SolutionContainer.TryGetSolution((Entity<SolutionContainerManagerComponent>) ent.Owner, ent.Comp.SolutionName, out Entity<SolutionComponent>? _, out solution))
      return;
    ent.Comp.Progress.Clear();
    string[] absorbentReagents = this.Puddle.GetAbsorbentReagents(solution);
    FixedPoint2 prototypeQuantity = solution.GetTotalPrototypeQuantity(absorbentReagents);
    if (prototypeQuantity > FixedPoint2.Zero)
      ent.Comp.Progress[solution.GetColorWithOnly(this._proto, absorbentReagents)] = prototypeQuantity.Float();
    Color colorWithout = solution.GetColorWithout(this._proto, absorbentReagents);
    FixedPoint2 fixedPoint2 = solution.Volume - prototypeQuantity;
    if (fixedPoint2 > FixedPoint2.Zero)
      ent.Comp.Progress[colorWithout] = fixedPoint2.Float();
    if (solution.AvailableVolume > FixedPoint2.Zero)
      ent.Comp.Progress[Color.DarkGray] = solution.AvailableVolume.Float();
    this.Dirty<AbsorbentComponent>(ent);
    this._item.VisualsChanged((EntityUid) ent);
  }

  [Obsolete("Use Entity<T> variant")]
  public void Mop(EntityUid user, EntityUid target, EntityUid used, AbsorbentComponent component)
  {
    this.Mop((Entity<AbsorbentComponent>) (used, component), user, target);
  }

  public void Mop(Entity<AbsorbentComponent> absorbEnt, EntityUid user, EntityUid target)
  {
    Entity<SolutionComponent>? entity;
    UseDelayComponent comp;
    if (!this.SolutionContainer.TryGetSolution((Entity<SolutionContainerManagerComponent>) absorbEnt.Owner, absorbEnt.Comp.SolutionName, out entity) || this.TryComp<UseDelayComponent>((EntityUid) absorbEnt, out comp) && this._useDelay.IsDelayed((Entity<UseDelayComponent>) (absorbEnt.Owner, comp)) || this.TryPuddleInteract((Entity<AbsorbentComponent, UseDelayComponent>) (absorbEnt.Owner, absorbEnt.Comp, comp), entity.Value, user, target) || !absorbEnt.Comp.UseAbsorberSolution)
      return;
    this.TryRefillableInteract((Entity<AbsorbentComponent, UseDelayComponent>) (absorbEnt.Owner, absorbEnt.Comp, comp), entity.Value, user, target);
  }

  private bool TryRefillableInteract(
    Entity<AbsorbentComponent, UseDelayComponent?> absorbEnt,
    Entity<SolutionComponent> absorbentSoln,
    EntityUid user,
    EntityUid target)
  {
    RefillableSolutionComponent comp;
    Entity<SolutionComponent>? soln;
    Solution solution;
    if (!this.TryComp<RefillableSolutionComponent>(target, out comp) || !this.SolutionContainer.TryGetRefillableSolution((Entity<RefillableSolutionComponent, SolutionContainerManagerComponent>) (target, comp, (SolutionContainerManagerComponent) null), out soln, out solution))
      return false;
    if (solution.Volume <= 0)
    {
      if (!this.TryTransferFromAbsorbentToRefillable((Entity<AbsorbentComponent>) absorbEnt, absorbentSoln, soln.Value, user, target))
        return false;
    }
    else if (!this.TryTwoWayAbsorbentRefillableTransfer((Entity<AbsorbentComponent>) absorbEnt, absorbentSoln, soln.Value, user, target))
      return false;
    (EntityUid owner, AbsorbentComponent comp1, UseDelayComponent comp2) = absorbEnt;
    this._audio.PlayPredicted(comp1.TransferSound, target, new EntityUid?(user));
    if (comp2 != null)
      this._useDelay.TryResetDelay((Entity<UseDelayComponent>) (owner, comp2));
    return true;
  }

  private bool TryTransferFromAbsorbentToRefillable(
    Entity<AbsorbentComponent> absorbEnt,
    Entity<SolutionComponent> absorbentSoln,
    Entity<SolutionComponent> refillableSoln,
    EntityUid user,
    EntityUid target)
  {
    if (absorbentSoln.Comp.Solution.Volume <= 0)
    {
      this._popups.PopupClient(this.Loc.GetString("mopping-system-target-container-empty", (nameof (target), (object) target)), user, new EntityUid?(user));
      return false;
    }
    Solution solution1 = refillableSoln.Comp.Solution;
    FixedPoint2 quantity = absorbEnt.Comp.PickupAmount < solution1.AvailableVolume ? absorbEnt.Comp.PickupAmount : solution1.AvailableVolume;
    if (quantity <= 0)
    {
      this._popups.PopupClient(this.Loc.GetString("mopping-system-full", ("used", (object) absorbEnt)), (EntityUid) absorbEnt, new EntityUid?(user));
      return false;
    }
    Solution solution2 = this.SolutionContainer.SplitSolutionWithout(absorbentSoln, quantity, this.Puddle.GetAbsorbentReagents(absorbentSoln.Comp.Solution));
    this.SolutionContainer.TryAddSolution(refillableSoln, solution2.Volume > 0 ? solution2 : this.SolutionContainer.SplitSolution(absorbentSoln, quantity));
    return true;
  }

  private bool TryTwoWayAbsorbentRefillableTransfer(
    Entity<AbsorbentComponent> absorbEnt,
    Entity<SolutionComponent> absorbentSoln,
    Entity<SolutionComponent> refillableSoln,
    EntityUid user,
    EntityUid target)
  {
    Solution toAdd1 = this.SolutionContainer.SplitSolutionWithout(absorbentSoln, absorbEnt.Comp.PickupAmount, this.Puddle.GetAbsorbentReagents(absorbentSoln.Comp.Solution));
    Solution solution1 = absorbentSoln.Comp.Solution;
    if (toAdd1.Volume == FixedPoint2.Zero && solution1.AvailableVolume == FixedPoint2.Zero)
    {
      this._popups.PopupClient(this.Loc.GetString("mopping-system-puddle-space", ("used", (object) absorbEnt)), user, new EntityUid?(user));
      return false;
    }
    FixedPoint2 toTake = absorbEnt.Comp.PickupAmount < solution1.AvailableVolume ? absorbEnt.Comp.PickupAmount : solution1.AvailableVolume;
    Solution solution2 = refillableSoln.Comp.Solution;
    Solution toAdd2 = solution2.SplitSolutionWithOnly(toTake, this.Puddle.GetAbsorbentReagents(refillableSoln.Comp.Solution));
    this.SolutionContainer.UpdateChemicals(refillableSoln);
    if (toAdd2.Volume == FixedPoint2.Zero && toAdd1.Volume == FixedPoint2.Zero)
    {
      this._popups.PopupClient(this.Loc.GetString("mopping-system-target-container-empty-water", (nameof (target), (object) target)), user, new EntityUid?(user));
      return false;
    }
    bool flag = false;
    if (toAdd2.Volume > FixedPoint2.Zero)
    {
      this.SolutionContainer.TryAddSolution(absorbentSoln, toAdd2);
      flag = true;
    }
    if (toAdd1.Volume <= 0)
      return flag;
    if (solution2.AvailableVolume <= 0)
    {
      this._popups.PopupClient(this.Loc.GetString("mopping-system-full", ("used", (object) target)), user, new EntityUid?(user));
    }
    else
    {
      Solution toAdd3 = toAdd1.SplitSolution(solution2.AvailableVolume);
      this.SolutionContainer.TryAddSolution(refillableSoln, toAdd3);
      flag = true;
    }
    this.SolutionContainer.TryAddSolution(absorbentSoln, toAdd1);
    return flag;
  }

  private bool TryPuddleInteract(
    Entity<AbsorbentComponent, UseDelayComponent?> absorbEnt,
    Entity<SolutionComponent> absorberSoln,
    EntityUid user,
    EntityUid target)
  {
    PuddleComponent comp1;
    Solution solution1;
    if (!this.TryComp<PuddleComponent>(target, out comp1) || !this.SolutionContainer.ResolveSolution((Entity<SolutionContainerManagerComponent>) target, comp1.SolutionName, ref comp1.Solution, out solution1) || solution1.Volume <= 0)
      return false;
    (EntityUid _, AbsorbentComponent comp1_1, UseDelayComponent comp2) = absorbEnt;
    bool flag = false;
    Solution toAdd;
    if (comp1_1.UseAbsorberSolution)
    {
      if (solution1.GetTotalPrototypeQuantity(this.Puddle.GetAbsorbentReagents(solution1)) == solution1.Volume)
      {
        this._popups.PopupClient(this.Loc.GetString("mopping-system-puddle-already-mopped", (nameof (target), (object) target)), target, new EntityUid?(user));
        return true;
      }
      Solution solution2 = absorberSoln.Comp.Solution;
      FixedPoint2 prototypeQuantity = solution2.GetTotalPrototypeQuantity(this.Puddle.GetAbsorbentReagents(solution2));
      if (prototypeQuantity == FixedPoint2.Zero)
      {
        this._popups.PopupClient(this.Loc.GetString("mopping-system-no-water", ("used", (object) absorbEnt)), (EntityUid) absorbEnt, new EntityUid?(user));
        return true;
      }
      FixedPoint2 pickupAmount = comp1_1.PickupAmount;
      FixedPoint2 toTake = prototypeQuantity > pickupAmount ? pickupAmount : prototypeQuantity;
      toAdd = solution1.SplitSolutionWithout(toTake, this.Puddle.GetAbsorbentReagents(solution1));
      Solution solution3 = solution2.SplitSolutionWithOnly(toAdd.Volume, this.Puddle.GetAbsorbentReagents(solution2));
      TransformComponent transformComponent = this.Transform(target);
      EntityUid? gridUid = transformComponent.GridUid;
      MapGridComponent comp3;
      if (this.TryComp<MapGridComponent>(gridUid, out comp3))
        this.Puddle.DoTileReactions(this._mapSystem.GetTileRef(gridUid.Value, comp3, transformComponent.Coordinates), solution3);
      this.SolutionContainer.AddSolution(comp1.Solution.Value, solution3);
    }
    else
    {
      toAdd = solution1.SplitSolutionWithout(comp1_1.PickupAmount, this.Puddle.GetAbsorbentReagents(solution1));
      if (solution1.Volume == FixedPoint2.Zero)
      {
        this.PredictedSpawnAttachedTo((string) comp1_1.MoppedEffect, this.Transform(target).Coordinates, rotation: new Angle());
        this.PredictedQueueDel(target);
        flag = true;
      }
    }
    this.SolutionContainer.AddSolution(absorberSoln, toAdd);
    this._audio.PlayPredicted(comp1_1.PickupSound, flag ? (EntityUid) absorbEnt : target, new EntityUid?(user));
    if (comp2 != null)
      this._useDelay.TryResetDelay((Entity<UseDelayComponent>) ((EntityUid) absorbEnt, comp2));
    TransformComponent component = this.Transform(user);
    Vector2 localPos = Vector2.Transform(this._transform.GetWorldPosition(target), this._transform.GetInvWorldMatrix(component));
    Angle localRotation = component.LocalRotation;
    localPos = ((Angle) ref localRotation).RotateVec(ref localPos);
    this._melee.DoLunge(user, (EntityUid) absorbEnt, Angle.Zero, localPos, (string) null);
    return true;
  }
}
