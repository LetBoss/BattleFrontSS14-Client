// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Repairable.RMCRepairableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Damage;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Tools;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Content.Shared.Tools.Components;
using Content.Shared.Tools.Systems;
using Content.Shared.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Repairable;

public sealed class RMCRepairableSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedRMCDamageableSystem _rmcDamageable;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SkillsSystem _skills;
  [Dependency]
  private SharedToolSystem _tool;
  [Dependency]
  private SharedSolutionContainerSystem _solution;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedStackSystem _stack;
  private const string SOLUTION_WELDER = "Welder";
  private const string REAGENT_WELDER = "WeldingFuel";

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCRepairableComponent, InteractUsingEvent>(new EntityEventRefHandler<RMCRepairableComponent, InteractUsingEvent>(this.OnRepairableInteractUsing));
    this.SubscribeLocalEvent<RMCRepairableComponent, RMCRepairableDoAfterEvent>(new EntityEventRefHandler<RMCRepairableComponent, RMCRepairableDoAfterEvent>(this.OnRepairableDoAfter));
    this.SubscribeLocalEvent<NailgunRepairableComponent, InteractUsingEvent>(new EntityEventRefHandler<NailgunRepairableComponent, InteractUsingEvent>(this.OnNailgunRepairableInteractUsing));
    this.SubscribeLocalEvent<NailgunRepairableComponent, RMCNailgunRepairableDoAfterEvent>(new EntityEventRefHandler<NailgunRepairableComponent, RMCNailgunRepairableDoAfterEvent>(this.OnNailgunRepairableDoAfter));
    this.SubscribeLocalEvent<ReagentTankComponent, InteractUsingEvent>(new EntityEventRefHandler<ReagentTankComponent, InteractUsingEvent>(this.OnWelderInteractUsing));
  }

  private void OnRepairableInteractUsing(
    Entity<RMCRepairableComponent> repairable,
    ref InteractUsingEvent args)
  {
    if (args.Handled)
      return;
    EntityUid used1 = args.Used;
    if (!this._tool.HasQuality(used1, (string) repairable.Comp.Quality))
      return;
    args.Handled = true;
    EntityUid user1 = args.User;
    if (!this.HasComp<BlowtorchComponent>(used1))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-repairable-need-blowtorch"), user1, new EntityUid?(user1), PopupType.SmallCaution);
    }
    else
    {
      DamageableComponent comp;
      if (!this.TryComp<DamageableComponent>((EntityUid) repairable, out comp))
        return;
      if (comp.TotalDamage <= FixedPoint2.Zero)
        this._popup.PopupClient(this.Loc.GetString("rmc-repairable-not-damaged", ("target", (object) repairable)), user1, new EntityUid?(user1), PopupType.SmallCaution);
      else if ((double) repairable.Comp.RepairableDamageLimit > 0.0 && comp.TotalDamage > (FixedPoint2) repairable.Comp.RepairableDamageLimit)
        this._popup.PopupClient(this.Loc.GetString("rmc-repairable-too-damaged", ("target", (object) repairable)), user1, new EntityUid?(user1), PopupType.SmallCaution);
      else if (repairable.Comp.SkillRequired > 0 && !this._skills.HasSkill((Entity<SkillsComponent>) user1, repairable.Comp.Skill, repairable.Comp.SkillRequired))
      {
        this._popup.PopupClient(this.Loc.GetString("rmc-repairable-not-trained", ("target", (object) repairable)), user1, new EntityUid?(user1), PopupType.SmallCaution);
      }
      else
      {
        if (!this.CanRepairPopup(user1, (EntityUid) repairable) || !this.UseFuel(args.Used, args.User, repairable.Comp.FuelUsed, true))
          return;
        RMCRepairableDoAfterEvent repairableDoAfterEvent = new RMCRepairableDoAfterEvent();
        EntityManager entityManager = this.EntityManager;
        EntityUid user2 = user1;
        TimeSpan delay = repairable.Comp.Delay;
        RMCRepairableDoAfterEvent @event = repairableDoAfterEvent;
        EntityUid? eventTarget = new EntityUid?((EntityUid) repairable);
        EntityUid? nullable = new EntityUid?(args.Used);
        EntityUid? target = new EntityUid?();
        EntityUid? used2 = nullable;
        DoAfterArgs args1 = new DoAfterArgs((IEntityManager) entityManager, user2, delay, (DoAfterEvent) @event, eventTarget, target, used2)
        {
          NeedHand = true,
          BreakOnMove = true,
          BlockDuplicate = true,
          DuplicateCondition = DuplicateConditions.SameEvent
        };
        RMCToolUseEvent args2 = new RMCToolUseEvent(user1, args1.Delay);
        this.RaiseLocalEvent<RMCToolUseEvent>(args.Used, ref args2);
        if (args2.Handled)
          args1.Delay = args2.Delay;
        if (!this._doAfter.TryStartDoAfter(args1))
          return;
        this._popup.PopupPredicted(this.Loc.GetString("rmc-repairable-start-self", ("target", (object) repairable)), this.Loc.GetString("rmc-repairable-start-others", ("user", (object) user1), ("target", (object) repairable)), user1, new EntityUid?(user1));
      }
    }
  }

  private void OnRepairableDoAfter(
    Entity<RMCRepairableComponent> repairable,
    ref RMCRepairableDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    args.Handled = true;
    if (!this.CanRepairPopup(args.User, (EntityUid) repairable))
      return;
    EntityUid? nullable = args.Used;
    if (!nullable.HasValue)
      return;
    nullable = args.Used;
    if (!this.UseFuel(nullable.Value, args.User, repairable.Comp.FuelUsed))
      return;
    DamageSpecifier damageSpecifier = -this._rmcDamageable.DistributeTypesTotal((Entity<DamageableComponent>) repairable.Owner, repairable.Comp.Heal);
    DamageableSystem damageable = this._damageable;
    EntityUid? uid = new EntityUid?((EntityUid) repairable);
    DamageSpecifier damage = damageSpecifier;
    nullable = new EntityUid?();
    EntityUid? origin = nullable;
    nullable = new EntityUid?();
    EntityUid? tool = nullable;
    damageable.TryChangeDamage(uid, damage, true, origin: origin, tool: tool);
    EntityUid user = args.User;
    this._popup.PopupPredicted(this.Loc.GetString("rmc-repairable-finish-self", ("target", (object) repairable)), this.Loc.GetString("rmc-repairable-finish-others", ("user", (object) user), ("target", (object) repairable)), user, new EntityUid?(user));
    this._audio.PlayPredicted(repairable.Comp.Sound, (EntityUid) repairable, new EntityUid?(user));
    DamageableComponent comp;
    if (!this.TryComp<DamageableComponent>((EntityUid) repairable, out comp) || !(comp.TotalDamage > FixedPoint2.Zero) || !(damageSpecifier.GetTotal() != FixedPoint2.Zero))
      return;
    args.Repeat = true;
  }

  public bool UseFuel(EntityUid tool, EntityUid user, FixedPoint2 fuelUsed, bool attempt = false)
  {
    SolutionContainerManagerComponent comp1;
    if (!this.TryComp<SolutionContainerManagerComponent>(tool, out comp1))
      return false;
    ItemToggleComponent comp2;
    if (!this.TryComp<ItemToggleComponent>(tool, out comp2) || !comp2.Activated)
    {
      this._popup.PopupClient(this.Loc.GetString("welder-component-welder-not-lit-message"), new EntityUid?(user), PopupType.SmallCaution);
      return false;
    }
    Entity<SolutionComponent>? entity;
    Solution solution;
    if (!this._solution.TryGetSolution((Entity<SolutionContainerManagerComponent>) (tool, comp1), "Welder", out entity, out solution))
      return false;
    if (solution.GetTotalPrototypeQuantity("WeldingFuel") == 0 || solution.GetTotalPrototypeQuantity("WeldingFuel") < fuelUsed)
    {
      this._popup.PopupClient(this.Loc.GetString("welder-component-no-fuel-message"), new EntityUid?(user), PopupType.SmallCaution);
      return false;
    }
    if (!attempt && this._net.IsServer)
    {
      this._solution.RemoveReagent(entity.Value, "WeldingFuel", fuelUsed);
      this.Dirty<SolutionComponent>(entity.Value);
    }
    return true;
  }

  private void OnNailgunRepairableInteractUsing(
    Entity<NailgunRepairableComponent> repairable,
    ref InteractUsingEvent args)
  {
    if (args.Handled)
      return;
    EntityUid used1 = args.Used;
    EntityUid user1 = args.User;
    NailgunComponent comp1;
    HandsComponent comp2;
    if (!this.TryComp<NailgunComponent>(used1, out comp1) || !this.TryComp<HandsComponent>(user1, out comp2))
      return;
    args.Handled = true;
    DamageableComponent comp3;
    if (!this.TryComp<DamageableComponent>((EntityUid) repairable, out comp3) || comp3.TotalDamage <= FixedPoint2.Zero)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-repairable-not-damaged", ("target", (object) repairable)), user1, new EntityUid?(user1), PopupType.SmallCaution);
    }
    else
    {
      GetAmmoCountEvent args1 = new GetAmmoCountEvent();
      this.RaiseLocalEvent<GetAmmoCountEvent>(used1, ref args1);
      if (args1.Count < 4)
      {
        this._popup.PopupClient(this.Loc.GetString("rmc-nailgun-no-nails-message"), new EntityUid?(user1), PopupType.SmallCaution);
      }
      else
      {
        EntityUid? heldStack;
        float repairValue = this.GetRepairValue(repairable, (Entity<HandsComponent>) (user1, comp2), comp1, out heldStack);
        if (!heldStack.HasValue || (FixedPoint2) repairValue <= FixedPoint2.Zero)
        {
          this._popup.PopupClient(this.Loc.GetString("rmc-nailgun-no-material-message", ("target", (object) repairable)), new EntityUid?(user1), PopupType.SmallCaution);
        }
        else
        {
          float nailingSpeed = comp1.NailingSpeed;
          RMCNailgunRepairableDoAfterEvent repairableDoAfterEvent = new RMCNailgunRepairableDoAfterEvent();
          EntityManager entityManager = this.EntityManager;
          EntityUid user2 = user1;
          double seconds = (double) nailingSpeed;
          RMCNailgunRepairableDoAfterEvent @event = repairableDoAfterEvent;
          EntityUid? eventTarget = new EntityUid?((EntityUid) repairable);
          EntityUid? nullable = new EntityUid?(args.Used);
          EntityUid? target = new EntityUid?();
          EntityUid? used2 = nullable;
          if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) entityManager, user2, (float) seconds, (DoAfterEvent) @event, eventTarget, target, used2)
          {
            NeedHand = true,
            BreakOnMove = true,
            BlockDuplicate = true,
            DuplicateCondition = DuplicateConditions.SameEvent
          }))
            return;
          this._popup.PopupPredicted(this.Loc.GetString("rmc-repairable-start-self", ("target", (object) repairable)), this.Loc.GetString("rmc-repairable-start-others", ("user", (object) user1), ("target", (object) repairable)), user1, new EntityUid?(user1));
        }
      }
    }
  }

  private void OnNailgunRepairableDoAfter(
    Entity<NailgunRepairableComponent> repairable,
    ref RMCNailgunRepairableDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    args.Handled = true;
    EntityUid? nullable = args.Used;
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault = nullable.GetValueOrDefault();
    EntityUid user = args.User;
    NailgunComponent comp1;
    if (!this.TryComp<NailgunComponent>(valueOrDefault, out comp1))
      return;
    GetAmmoCountEvent args1 = new GetAmmoCountEvent();
    this.RaiseLocalEvent<GetAmmoCountEvent>(valueOrDefault, ref args1);
    if (args1.Count < 4)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-nailgun-no-nails-message"), new EntityUid?(user), PopupType.SmallCaution);
    }
    else
    {
      HandsComponent comp2;
      if (!this.TryComp<HandsComponent>(user, out comp2))
        return;
      EntityUid? heldStack;
      float repairValue = this.GetRepairValue(repairable, (Entity<HandsComponent>) (user, comp2), comp1, out heldStack);
      if (!heldStack.HasValue || (FixedPoint2) repairValue <= FixedPoint2.Zero)
      {
        this._popup.PopupClient(this.Loc.GetString("rmc-nailgun-lost-stack"), new EntityUid?(user), PopupType.SmallCaution);
      }
      else
      {
        DamageSpecifier damageSpecifier = -this._rmcDamageable.DistributeTypesTotal((Entity<DamageableComponent>) repairable.Owner, (FixedPoint2) repairValue);
        DamageableSystem damageable = this._damageable;
        EntityUid? uid = new EntityUid?((EntityUid) repairable);
        DamageSpecifier damage = damageSpecifier;
        nullable = new EntityUid?();
        EntityUid? origin = nullable;
        nullable = new EntityUid?();
        EntityUid? tool = nullable;
        damageable.TryChangeDamage(uid, damage, true, origin: origin, tool: tool);
        StackComponent comp3;
        if (this.TryComp<StackComponent>(heldStack, out comp3))
          this._stack.SetCount(heldStack.Value, comp3.Count - comp1.MaterialPerRepair);
        TakeAmmoEvent args2 = new TakeAmmoEvent(4, new List<(EntityUid?, IShootable)>(), this.Transform(user).Coordinates, new EntityUid?(user));
        this.RaiseLocalEvent<TakeAmmoEvent>(valueOrDefault, args2);
        foreach ((EntityUid? Entity, IShootable Shootable) tuple in args2.Ammo)
          this.QueueDel(tuple.Entity);
        this._popup.PopupPredicted(this.Loc.GetString("rmc-nailgun-finish-self", ("material", (object) heldStack), ("target", (object) repairable)), this.Loc.GetString("rmc-repairable-finish-others", ("user", (object) user), ("material", (object) heldStack), ("target", (object) repairable)), user, new EntityUid?(user));
        this._audio.PlayPredicted(comp1.RepairSound, (EntityUid) repairable, new EntityUid?(user));
      }
    }
  }

  private float GetRepairValue(
    Entity<NailgunRepairableComponent> repairable,
    Entity<HandsComponent?> hands,
    NailgunComponent nailgunComponent,
    out EntityUid? heldStack)
  {
    float repairValue = 0.0f;
    heldStack = new EntityUid?();
    foreach (EntityUid uid in this._hands.EnumerateHeld(hands))
    {
      StackComponent comp;
      if (this.TryComp<StackComponent>(uid, out comp))
      {
        string stackTypeId = comp.StackTypeId;
        heldStack = new EntityUid?(uid);
        if (comp.Count >= nailgunComponent.MaterialPerRepair)
        {
          switch (stackTypeId)
          {
            case "CMSteel":
              repairValue = repairable.Comp.RepairMetal;
              goto label_12;
            case "CMPlasteel":
              repairValue = repairable.Comp.RepairPlasteel;
              goto label_12;
            case "RMCPlankWood":
              repairValue = repairable.Comp.RepairWood;
              goto label_12;
            default:
              continue;
          }
        }
      }
    }
label_12:
    return repairValue;
  }

  private void OnWelderInteractUsing(Entity<ReagentTankComponent> ent, ref InteractUsingEvent args)
  {
    if (args.Handled)
      return;
    EntityUid used = args.Used;
    EntityUid target = args.Target;
    WelderComponent comp;
    Entity<SolutionComponent>? soln;
    Solution solution1;
    Entity<SolutionComponent>? entity;
    Solution solution2;
    if (!this.TryComp<WelderComponent>(used, out comp) || ent.Comp.TankType != ReagentTankType.Fuel || !this._solution.TryGetDrainableSolution((Entity<DrainableSolutionComponent, SolutionContainerManagerComponent>) target, out soln, out solution1) || !this._solution.TryGetSolution((Entity<SolutionContainerManagerComponent>) used, comp.FuelSolutionName, out entity, out solution2))
      return;
    FixedPoint2 quantity = FixedPoint2.Min(solution2.AvailableVolume, solution1.Volume);
    if (comp.Enabled)
      this._popup.PopupClient(this.Loc.GetString("rmc-welder-component-danger"), used, new EntityUid?(args.User), PopupType.MediumCaution);
    else if (quantity > 0)
    {
      Solution toAdd = this._solution.Drain((Entity<DrainableSolutionComponent>) target, soln.Value, quantity);
      this._solution.TryAddSolution(entity.Value, toAdd);
      this._audio.PlayPredicted(comp.WelderRefill, used, new EntityUid?(args.User));
      this._popup.PopupClient(this.Loc.GetString("welder-component-after-interact-refueled-message"), used, new EntityUid?(args.User));
    }
    else if (solution2.AvailableVolume <= 0)
      this._popup.PopupClient(this.Loc.GetString("welder-component-already-full"), used, new EntityUid?(args.User));
    else
      this._popup.PopupClient(this.Loc.GetString("welder-component-no-fuel-in-tank", ("owner", (object) args.Target)), used, new EntityUid?(args.User));
    args.Handled = true;
  }

  private bool CanRepairPopup(EntityUid user, EntityUid target)
  {
    RMCRepairableTargetAttemptEvent args = new RMCRepairableTargetAttemptEvent(user, target);
    this.RaiseLocalEvent<RMCRepairableTargetAttemptEvent>(target, ref args);
    if (!args.Cancelled)
      return true;
    this._popup.PopupClient(args.Popup, user, new EntityUid?(user), PopupType.MediumCaution);
    return false;
  }
}
