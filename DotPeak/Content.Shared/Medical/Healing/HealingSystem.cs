// Decompiled with JetBrains decompiler
// Type: Content.Shared.Medical.Healing.HealingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Body.Components;
using Content.Shared.Body.Systems;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Medical.Healing;

public sealed class HealingSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private SharedBloodstreamSystem _bloodstreamSystem;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedStackSystem _stacks;
  [Dependency]
  private SharedInteractionSystem _interactionSystem;
  [Dependency]
  private MobThresholdSystem _mobThresholdSystem;
  [Dependency]
  private SharedPopupSystem _popupSystem;
  [Dependency]
  private SharedSolutionContainerSystem _solutionContainerSystem;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<HealingComponent, UseInHandEvent>(new EntityEventRefHandler<HealingComponent, UseInHandEvent>(this.OnHealingUse));
    this.SubscribeLocalEvent<HealingComponent, AfterInteractEvent>(new EntityEventRefHandler<HealingComponent, AfterInteractEvent>(this.OnHealingAfterInteract));
    this.SubscribeLocalEvent<DamageableComponent, HealingDoAfterEvent>(new EntityEventRefHandler<DamageableComponent, HealingDoAfterEvent>(this.OnDoAfter));
  }

  private void OnDoAfter(Entity<DamageableComponent> target, ref HealingDoAfterEvent args)
  {
    HealingComponent comp1;
    if (args.Handled || args.Cancelled || !this.TryComp<HealingComponent>(args.Used, out comp1) || comp1.DamageContainers != null && target.Comp.DamageContainerID.HasValue && !comp1.DamageContainers.Contains(target.Comp.DamageContainerID.Value))
      return;
    BloodstreamComponent comp2;
    this.TryComp<BloodstreamComponent>((EntityUid) target, out comp2);
    if ((double) comp1.BloodlossModifier != 0.0 && comp2 != null)
    {
      int num1 = (double) comp2.BleedAmount > 0.0 ? 1 : 0;
      this._bloodstreamSystem.TryModifyBleedAmount((Entity<BloodstreamComponent>) (target.Owner, comp2), comp1.BloodlossModifier);
      int num2 = (double) comp2.BleedAmount > 0.0 ? 1 : 0;
      if (num1 != num2)
        this._popupSystem.PopupClient(args.User == target.Owner ? this.Loc.GetString("medical-item-stop-bleeding-self") : this.Loc.GetString("medical-item-stop-bleeding", (nameof (target), (object) Identity.Entity(target.Owner, (IEntityManager) this.EntityManager))), (EntityUid) target, new EntityUid?(args.User));
    }
    if ((double) comp1.ModifyBloodLevel != 0.0 && comp2 != null)
      this._bloodstreamSystem.TryModifyBloodLevel((Entity<BloodstreamComponent>) (target.Owner, comp2), (FixedPoint2) comp1.ModifyBloodLevel);
    DamageSpecifier damageSpecifier = this._damageable.TryChangeDamage(new EntityUid?(target.Owner), comp1.Damage * this._damageable.UniversalTopicalsHealModifier, true, origin: new EntityUid?(args.Args.User));
    if (damageSpecifier == null && (double) comp1.BloodlossModifier != 0.0)
      return;
    FixedPoint2 fixedPoint2 = damageSpecifier != null ? damageSpecifier.GetTotal() : FixedPoint2.Zero;
    bool flag = false;
    EntityUid? used = args.Used;
    StackComponent comp3;
    if (this.TryComp<StackComponent>(used.Value, out comp3))
    {
      SharedStackSystem stacks1 = this._stacks;
      used = args.Used;
      EntityUid uid1 = used.Value;
      StackComponent stack = comp3;
      stacks1.Use(uid1, 1, stack);
      SharedStackSystem stacks2 = this._stacks;
      used = args.Used;
      EntityUid uid2 = used.Value;
      StackComponent component = comp3;
      if (stacks2.GetCount(uid2, component) <= 0)
        flag = true;
    }
    else
    {
      used = args.Used;
      this.PredictedQueueDel(used.Value);
    }
    if (target.Owner != args.User)
    {
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(20, 3);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.User), "user", "ToPrettyString(args.User)");
      logStringHandler.AppendLiteral(" healed ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) target.Owner), nameof (target), "ToPrettyString(target.Owner)");
      logStringHandler.AppendLiteral(" for ");
      logStringHandler.AppendFormatted<FixedPoint2>(fixedPoint2, "damage", "total");
      logStringHandler.AppendLiteral(" damage");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.Healed, ref local);
    }
    else
    {
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(30, 2);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.User), "user", "ToPrettyString(args.User)");
      logStringHandler.AppendLiteral(" healed themselves for ");
      logStringHandler.AppendFormatted<FixedPoint2>(fixedPoint2, "damage", "total");
      logStringHandler.AppendLiteral(" damage");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.Healed, ref local);
    }
    this._audio.PlayPredicted(comp1.HealingEndSound, target.Owner, new EntityUid?(args.User));
    HealingDoAfterEvent healingDoAfterEvent = args;
    used = args.Used;
    int num = !this.HasDamage((Entity<HealingComponent>) (used.Value, comp1), target) ? 0 : (!flag ? 1 : 0);
    healingDoAfterEvent.Repeat = num != 0;
    if (!args.Repeat && !flag)
      this._popupSystem.PopupClient(this.Loc.GetString("medical-item-finished-using", ("item", (object) args.Used)), target.Owner, new EntityUid?(args.User));
    args.Handled = true;
  }

  private bool HasDamage(Entity<HealingComponent> healing, Entity<DamageableComponent> target)
  {
    Dictionary<string, FixedPoint2> damageDict = target.Comp.Damage.DamageDict;
    foreach (KeyValuePair<string, FixedPoint2> keyValuePair in healing.Comp.Damage.DamageDict)
    {
      if (damageDict[keyValuePair.Key].Value > 0)
        return true;
    }
    BloodstreamComponent comp;
    Solution solution;
    return this.TryComp<BloodstreamComponent>((EntityUid) target, out comp) && ((double) healing.Comp.ModifyBloodLevel > 0.0 && this._solutionContainerSystem.ResolveSolution((Entity<SolutionContainerManagerComponent>) target.Owner, comp.BloodSolutionName, ref comp.BloodSolution, out solution) && solution.Volume < solution.MaxVolume || (double) healing.Comp.BloodlossModifier < 0.0 && (double) comp.BleedAmount > 0.0);
  }

  private void OnHealingUse(Entity<HealingComponent> healing, ref UseInHandEvent args)
  {
    if (args.Handled || !this.TryHeal(healing, (Entity<DamageableComponent>) args.User, args.User))
      return;
    args.Handled = true;
  }

  private void OnHealingAfterInteract(Entity<HealingComponent> healing, ref AfterInteractEvent args)
  {
    if (args.Handled || !args.CanReach)
      return;
    EntityUid? target1 = args.Target;
    if (!target1.HasValue)
      return;
    Entity<HealingComponent> healing1 = healing;
    target1 = args.Target;
    Entity<DamageableComponent> target2 = (Entity<DamageableComponent>) target1.Value;
    EntityUid user = args.User;
    if (!this.TryHeal(healing1, target2, user))
      return;
    args.Handled = true;
  }

  private bool TryHeal(
    Entity<HealingComponent> healing,
    Entity<DamageableComponent?> target,
    EntityUid user)
  {
    StackComponent comp;
    if (!this.Resolve<DamageableComponent>((EntityUid) target, ref target.Comp, false) || healing.Comp.DamageContainers != null && target.Comp.DamageContainerID.HasValue && !healing.Comp.DamageContainers.Contains(target.Comp.DamageContainerID.Value) || user != target.Owner && !this._interactionSystem.InRangeUnobstructed((Entity<TransformComponent>) user, (Entity<TransformComponent>) target.Owner, popup: true) || this.TryComp<StackComponent>((EntityUid) healing, out comp) && comp.Count < 1)
      return false;
    if (!this.HasDamage(healing, target))
    {
      this._popupSystem.PopupClient(this.Loc.GetString("medical-item-cant-use", ("item", (object) healing.Owner)), (EntityUid) healing, new EntityUid?(user));
      return false;
    }
    this._audio.PlayPredicted(healing.Comp.HealingBeginSound, (EntityUid) healing, new EntityUid?(user));
    int num = user != target.Owner ? 1 : 0;
    if (num != 0)
      this._popupSystem.PopupEntity(this.Loc.GetString("medical-item-popup-target", (nameof (user), (object) Identity.Entity(user, (IEntityManager) this.EntityManager)), ("item", (object) healing.Owner)), (EntityUid) target, (EntityUid) target, PopupType.Medium);
    float seconds = num != 0 ? healing.Comp.Delay : healing.Comp.Delay * this.GetScaledHealingPenalty(healing);
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, seconds, (DoAfterEvent) new HealingDoAfterEvent(), new EntityUid?((EntityUid) target), new EntityUid?((EntityUid) target), new EntityUid?((EntityUid) healing))
    {
      NeedHand = true,
      BreakOnMove = true,
      BreakOnWeightlessMove = false
    });
    return true;
  }

  public float GetScaledHealingPenalty(Entity<HealingComponent> healing)
  {
    float delay = healing.Comp.Delay;
    MobThresholdsComponent comp1;
    DamageableComponent comp2;
    if (!this.TryComp<MobThresholdsComponent>((EntityUid) healing, out comp1) || !this.TryComp<DamageableComponent>((EntityUid) healing, out comp2))
      return delay;
    FixedPoint2? threshold;
    if (!this._mobThresholdSystem.TryGetThresholdForState((EntityUid) healing, MobState.Critical, out threshold, comp1))
      return 1f;
    FixedPoint2 totalDamage = comp2.TotalDamage;
    FixedPoint2? nullable = threshold;
    return Math.Max((float) ((double) (float) (nullable.HasValue ? new FixedPoint2?(totalDamage / nullable.GetValueOrDefault()) : new FixedPoint2?()).Value * ((double) healing.Comp.SelfHealPenaltyMultiplier - 1.0) + 1.0), 1f);
  }
}
