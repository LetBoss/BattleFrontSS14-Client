// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.EntitySystems.HypospraySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Chemistry;
using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.Hypospray.Events;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Content.Shared.Forensics;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Timing;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Chemistry.EntitySystems;

public sealed class HypospraySystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private ReactiveSystem _reactiveSystem;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedSolutionContainerSystem _solutionContainers;
  [Dependency]
  private UseDelaySystem _useDelay;
  [Dependency]
  private RMCSharedHypospraySystem _rmcHypospray;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<HyposprayComponent, AfterInteractEvent>(new EntityEventRefHandler<HyposprayComponent, AfterInteractEvent>((object) this, __methodptr(OnAfterInteract)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<HyposprayComponent, MeleeHitEvent>(new EntityEventRefHandler<HyposprayComponent, MeleeHitEvent>((object) this, __methodptr(OnAttack)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<HyposprayComponent, UseInHandEvent>(new EntityEventRefHandler<HyposprayComponent, UseInHandEvent>((object) this, __methodptr(OnUseInHand)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<HyposprayComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<HyposprayComponent, GetVerbsEvent<AlternativeVerb>>((object) this, __methodptr(AddToggleModeVerb)), (Type[]) null, (Type[]) null);
  }

  private void OnUseInHand(Entity<HyposprayComponent> entity, ref UseInHandEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = this.TryDoInject(entity, args.User, args.User);
  }

  private void OnAfterInteract(Entity<HyposprayComponent> entity, ref AfterInteractEvent args)
  {
    if (args.Handled || !args.CanReach)
      return;
    EntityUid? target1 = args.Target;
    if (!target1.HasValue)
      return;
    AfterInteractEvent afterInteractEvent = args;
    Entity<HyposprayComponent> entity1 = entity;
    target1 = args.Target;
    EntityUid target2 = target1.Value;
    EntityUid user = args.User;
    int num = this.TryUseHypospray(entity1, target2, user) ? 1 : 0;
    afterInteractEvent.Handled = num != 0;
  }

  private void OnAttack(Entity<HyposprayComponent> entity, ref MeleeHitEvent args)
  {
    IReadOnlyList<EntityUid> hitEntities = args.HitEntities;
    if (hitEntities != null && hitEntities.Count == 0)
      return;
    this.TryDoInject(entity, args.HitEntities[0], args.User);
  }

  private bool TryUseHypospray(Entity<HyposprayComponent> entity, EntityUid target, EntityUid user)
  {
    Entity<SolutionComponent>? soln;
    return entity.Comp.CanContainerDraw && !this.EligibleEntity(target, Entity<HyposprayComponent>.op_Implicit(entity)) && this._solutionContainers.TryGetDrawableSolution(Entity<DrawableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(target), out soln, out Solution _) ? this.TryDraw(entity, target, soln.Value, user) : this.TryDoInject(entity, target, user);
  }

  public bool TryDoInject(
    Entity<HyposprayComponent> entity,
    EntityUid target,
    EntityUid user,
    bool doAfter = true)
  {
    if (doAfter)
      return this._rmcHypospray.DoAfter(entity, target, user);
    EntityUid entityUid1;
    HyposprayComponent hyposprayComponent;
    entity.Deconstruct(ref entityUid1, ref hyposprayComponent);
    EntityUid entityUid2 = entityUid1;
    HyposprayComponent component = hyposprayComponent;
    UseDelayComponent useDelayComponent;
    if (!this.EligibleEntity(target, component) || this.TryComp<UseDelayComponent>(entityUid2, ref useDelayComponent) && this._useDelay.IsDelayed(Entity<UseDelayComponent>.op_Implicit((entityUid2, useDelayComponent))))
      return false;
    string str = (string) null;
    SelfBeforeHyposprayInjectsEvent hyposprayInjectsEvent1 = new SelfBeforeHyposprayInjectsEvent(user, entity.Owner, target);
    this.RaiseLocalEvent<SelfBeforeHyposprayInjectsEvent>(user, hyposprayInjectsEvent1, false);
    if (hyposprayInjectsEvent1.Cancelled)
    {
      this._popup.PopupClient(this.Loc.GetString(hyposprayInjectsEvent1.InjectMessageOverride ?? "hypospray-cant-inject", ("owner", (object) Identity.Entity(target, (IEntityManager) this.EntityManager))), target, new EntityUid?(user));
      return false;
    }
    target = hyposprayInjectsEvent1.TargetGettingInjected;
    if (!this.EligibleEntity(target, component))
      return false;
    TargetBeforeHyposprayInjectsEvent hyposprayInjectsEvent2 = new TargetBeforeHyposprayInjectsEvent(user, entity.Owner, target);
    this.RaiseLocalEvent<TargetBeforeHyposprayInjectsEvent>(target, hyposprayInjectsEvent2, false);
    if (hyposprayInjectsEvent2.Cancelled)
    {
      this._popup.PopupClient(this.Loc.GetString(hyposprayInjectsEvent2.InjectMessageOverride ?? "hypospray-cant-inject", ("owner", (object) Identity.Entity(target, (IEntityManager) this.EntityManager))), target, new EntityUid?(user));
      return false;
    }
    target = hyposprayInjectsEvent2.TargetGettingInjected;
    if (!this.EligibleEntity(target, component))
      return false;
    if (hyposprayInjectsEvent2.InjectMessageOverride != null)
      str = hyposprayInjectsEvent2.InjectMessageOverride;
    else if (hyposprayInjectsEvent1.InjectMessageOverride != null)
      str = hyposprayInjectsEvent1.InjectMessageOverride;
    else if (EntityUid.op_Equality(target, user))
      str = "hypospray-component-inject-self-message";
    Entity<SolutionComponent>? entity1;
    Solution solution1;
    if (!this._solutionContainers.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(entityUid2), component.SolutionName, out entity1, out solution1) || solution1.Volume == 0)
    {
      this._popup.PopupClient(this.Loc.GetString("hypospray-component-empty-message"), target, new EntityUid?(user));
      return true;
    }
    Entity<SolutionComponent>? soln;
    Solution solution2;
    if (!this._solutionContainers.TryGetInjectableSolution(Entity<InjectableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(target), out soln, out solution2))
    {
      this._popup.PopupClient(this.Loc.GetString("hypospray-cant-inject", (nameof (target), (object) Identity.Entity(target, (IEntityManager) this.EntityManager))), target, new EntityUid?(user));
      return false;
    }
    this._popup.PopupClient(this.Loc.GetString(str ?? "hypospray-component-inject-other-message", ("other", (object) target)), target, new EntityUid?(user));
    if (EntityUid.op_Inequality(target, user))
      this._popup.PopupEntity(this.Loc.GetString("hypospray-component-feel-prick-message"), target, target);
    this._audio.PlayPredicted(component.InjectSound, target, new EntityUid?(user), new AudioParams?());
    if (useDelayComponent != null)
      this._useDelay.TryResetDelay(Entity<UseDelayComponent>.op_Implicit((entityUid2, useDelayComponent)));
    FixedPoint2 quantity = FixedPoint2.Min(component.TransferAmount, solution2.AvailableVolume);
    if (quantity <= 0)
    {
      this._popup.PopupClient(this.Loc.GetString("hypospray-component-transfer-already-full-message", ("owner", (object) target)), target, new EntityUid?(user));
      return true;
    }
    Solution solution3 = this._solutionContainers.SplitSolution(entity1.Value, quantity);
    if (!solution2.CanAddSolution(solution3))
      return true;
    this._reactiveSystem.DoEntityReaction(target, solution3, ReactionMethod.Injection);
    this._solutionContainers.TryAddSolution(soln.Value, solution3);
    TransferDnaEvent transferDnaEvent = new TransferDnaEvent()
    {
      Donor = target,
      Recipient = entityUid2
    };
    this.RaiseLocalEvent<TransferDnaEvent>(target, ref transferDnaEvent, false);
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(36, 4);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), nameof (user), "ToPrettyString(user)");
    logStringHandler.AppendLiteral(" injected ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), nameof (target), "ToPrettyString(target)");
    logStringHandler.AppendLiteral(" with a solution ");
    logStringHandler.AppendFormatted(SharedSolutionContainerSystem.ToPrettyString(solution3), format: "removedSolution");
    logStringHandler.AppendLiteral(" using a ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(entityUid2)), "using", "ToPrettyString(uid)");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.ForceFeed, ref local);
    return true;
  }

  private bool TryDraw(
    Entity<HyposprayComponent> entity,
    EntityUid target,
    Entity<SolutionComponent> targetSolution,
    EntityUid user)
  {
    Entity<SolutionComponent>? entity1;
    Solution solution;
    if (!this._solutionContainers.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(entity.Owner), entity.Comp.SolutionName, out entity1, out solution) || solution.AvailableVolume == 0)
      return false;
    FixedPoint2 quantity = FixedPoint2.Min(entity.Comp.TransferAmount, targetSolution.Comp.Solution.Volume, solution.AvailableVolume);
    if (quantity <= 0)
    {
      this._popup.PopupClient(this.Loc.GetString("injector-component-target-is-empty-message", (nameof (target), (object) Identity.Entity(target, (IEntityManager) this.EntityManager))), entity.Owner, new EntityUid?(user));
      return false;
    }
    Solution toAdd = this._solutionContainers.Draw(Entity<DrawableSolutionComponent>.op_Implicit(target), targetSolution, quantity);
    if (!this._solutionContainers.TryAddSolution(entity1.Value, toAdd))
      return false;
    this._popup.PopupClient(this.Loc.GetString("injector-component-draw-success-message", ("amount", (object) toAdd.Volume), (nameof (target), (object) Identity.Entity(target, (IEntityManager) this.EntityManager))), entity.Owner, new EntityUid?(user));
    return true;
  }

  public bool EligibleEntity(EntityUid entity, HyposprayComponent component)
  {
    if (!component.OnlyAffectsMobs)
      return this.HasComp<SolutionContainerManagerComponent>(entity);
    return this.HasComp<SolutionContainerManagerComponent>(entity) && this.HasComp<MobStateComponent>(entity);
  }

  private void AddToggleModeVerb(
    Entity<HyposprayComponent> entity,
    ref GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || args.Hands == null || entity.Comp.InjectOnly)
      return;
    EntityUid user = args.User;
    AlternativeVerb alternativeVerb1 = new AlternativeVerb();
    alternativeVerb1.Text = this.Loc.GetString("hypospray-verb-mode-label");
    alternativeVerb1.Act = (Action) (() => this.ToggleMode(entity, user));
    AlternativeVerb alternativeVerb2 = alternativeVerb1;
    args.Verbs.Add(alternativeVerb2);
  }

  private void ToggleMode(Entity<HyposprayComponent> entity, EntityUid user)
  {
    this.SetMode(entity, !entity.Comp.OnlyAffectsMobs);
    this._popup.PopupClient(this.Loc.GetString(!entity.Comp.OnlyAffectsMobs || !entity.Comp.CanContainerDraw ? "hypospray-verb-mode-inject-all" : "hypospray-verb-mode-inject-mobs-only"), Entity<HyposprayComponent>.op_Implicit(entity), new EntityUid?(user));
  }

  public void SetMode(Entity<HyposprayComponent> entity, bool onlyAffectsMobs)
  {
    if (entity.Comp.OnlyAffectsMobs == onlyAffectsMobs)
      return;
    entity.Comp.OnlyAffectsMobs = onlyAffectsMobs;
    this.Dirty<HyposprayComponent>(entity, (MetaDataComponent) null);
  }
}
