// Decompiled with JetBrains decompiler
// Type: Content.Shared.Execution.SharedExecutionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.ActionBlocker;
using Content.Shared.Chat;
using Content.Shared.CombatMode;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Shared.Execution;

public sealed class SharedExecutionSystem : EntitySystem
{
  private static readonly bool DisableExecution = true;
  [Dependency]
  private ActionBlockerSystem _actionBlocker;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedSuicideSystem _suicide;
  [Dependency]
  private SharedCombatModeSystem _combat;
  [Dependency]
  private SharedExecutionSystem _execution;
  [Dependency]
  private SharedMeleeWeaponSystem _melee;
  [Dependency]
  private IConfigurationManager _config;
  private bool _canSuicide;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ExecutionComponent, GetVerbsEvent<UtilityVerb>>(new ComponentEventHandler<ExecutionComponent, GetVerbsEvent<UtilityVerb>>(this.OnGetInteractionsVerbs));
    this.SubscribeLocalEvent<ExecutionComponent, GetMeleeDamageEvent>(new EntityEventRefHandler<ExecutionComponent, GetMeleeDamageEvent>(this.OnGetMeleeDamage));
    this.SubscribeLocalEvent<ExecutionComponent, SuicideByEnvironmentEvent>(new EntityEventRefHandler<ExecutionComponent, SuicideByEnvironmentEvent>(this.OnSuicideByEnvironment));
    this.SubscribeLocalEvent<ExecutionComponent, ExecutionDoAfterEvent>(new EntityEventRefHandler<ExecutionComponent, ExecutionDoAfterEvent>(this.OnExecutionDoAfter));
    this.Subs.CVar<bool>(this._config, RMCCVars.RMCEnableSuicide, (Action<bool>) (v => this._canSuicide = v));
  }

  private void OnGetInteractionsVerbs(
    EntityUid uid,
    ExecutionComponent comp,
    GetVerbsEvent<UtilityVerb> args)
  {
    if (args.Hands == null || !args.Using.HasValue || !args.CanAccess || !args.CanInteract)
      return;
    EntityUid attacker = args.User;
    EntityUid weapon = args.Using.Value;
    EntityUid victim = args.Target;
    if (!this.CanBeExecuted(victim, attacker))
      return;
    UtilityVerb utilityVerb1 = new UtilityVerb();
    utilityVerb1.Act = (Action) (() => this.TryStartExecutionDoAfter(weapon, victim, attacker, comp));
    utilityVerb1.Impact = LogImpact.High;
    utilityVerb1.Text = this.Loc.GetString("execution-verb-name");
    utilityVerb1.Message = this.Loc.GetString("execution-verb-message");
    UtilityVerb utilityVerb2 = utilityVerb1;
    args.Verbs.Add(utilityVerb2);
  }

  private void TryStartExecutionDoAfter(
    EntityUid weapon,
    EntityUid victim,
    EntityUid attacker,
    ExecutionComponent comp)
  {
    if (!this.CanBeExecuted(victim, attacker))
      return;
    if (attacker == victim)
    {
      this.ShowExecutionInternalPopup((string) comp.InternalSelfExecutionMessage, attacker, victim, weapon);
      this.ShowExecutionExternalPopup((string) comp.ExternalSelfExecutionMessage, attacker, victim, weapon);
    }
    else
    {
      this.ShowExecutionInternalPopup((string) comp.InternalMeleeExecutionMessage, attacker, victim, weapon);
      this.ShowExecutionExternalPopup((string) comp.ExternalMeleeExecutionMessage, attacker, victim, weapon);
    }
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, attacker, comp.DoAfterDuration, (DoAfterEvent) new ExecutionDoAfterEvent(), new EntityUid?(weapon), new EntityUid?(victim), new EntityUid?(weapon))
    {
      BreakOnMove = true,
      BreakOnDamage = true,
      NeedHand = true
    });
  }

  public bool CanBeExecuted(EntityUid victim, EntityUid attacker)
  {
    MobStateComponent comp;
    return !SharedExecutionSystem.DisableExecution && (!(victim == attacker) || this._canSuicide) && !this.HasComp<XenoComponent>(victim) && this.HasComp<DamageableComponent>(victim) && this.TryComp<MobStateComponent>(victim, out comp) && !this._mobState.IsDead(victim, comp) && this._actionBlocker.CanAttack(attacker, new EntityUid?(victim)) && (!(victim != attacker) || !this._actionBlocker.CanInteract(victim, new EntityUid?()));
  }

  private void OnGetMeleeDamage(Entity<ExecutionComponent> entity, ref GetMeleeDamageEvent args)
  {
    MeleeWeaponComponent comp;
    if (!this.TryComp<MeleeWeaponComponent>((EntityUid) entity, out comp) || !entity.Comp.Executing)
      return;
    DamageSpecifier damageSpecifier = comp.Damage * entity.Comp.DamageMultiplier - comp.Damage;
    args.Damage += damageSpecifier;
    args.ResistanceBypass = true;
  }

  private void OnSuicideByEnvironment(
    Entity<ExecutionComponent> entity,
    ref SuicideByEnvironmentEvent args)
  {
    MeleeWeaponComponent comp1;
    if (!this.TryComp<MeleeWeaponComponent>((EntityUid) entity, out comp1))
      return;
    string executionMessage1 = (string) entity.Comp.CompleteInternalSelfExecutionMessage;
    string executionMessage2 = (string) entity.Comp.CompleteExternalSelfExecutionMessage;
    DamageableComponent comp2;
    if (!this.TryComp<DamageableComponent>(args.Victim, out comp2))
      return;
    this.ShowExecutionInternalPopup(executionMessage1, args.Victim, args.Victim, (EntityUid) entity, false);
    this.ShowExecutionExternalPopup(executionMessage2, args.Victim, args.Victim, (EntityUid) entity);
    this._audio.PlayPredicted(comp1.HitSound, args.Victim, new EntityUid?(args.Victim));
    this._suicide.ApplyLethalDamage((Entity<DamageableComponent>) (args.Victim, comp2), comp1.Damage);
    args.Handled = true;
  }

  private void ShowExecutionInternalPopup(
    string locString,
    EntityUid attacker,
    EntityUid victim,
    EntityUid weapon,
    bool predict = true)
  {
    if (predict)
      this._popup.PopupClient(this.Loc.GetString(locString, (nameof (attacker), (object) Identity.Entity(attacker, (IEntityManager) this.EntityManager)), (nameof (victim), (object) Identity.Entity(victim, (IEntityManager) this.EntityManager)), (nameof (weapon), (object) weapon)), attacker, new EntityUid?(attacker), PopupType.MediumCaution);
    else
      this._popup.PopupEntity(this.Loc.GetString(locString, (nameof (attacker), (object) Identity.Entity(attacker, (IEntityManager) this.EntityManager)), (nameof (victim), (object) Identity.Entity(victim, (IEntityManager) this.EntityManager)), (nameof (weapon), (object) weapon)), attacker, attacker, PopupType.MediumCaution);
  }

  private void ShowExecutionExternalPopup(
    string locString,
    EntityUid attacker,
    EntityUid victim,
    EntityUid weapon)
  {
    this._popup.PopupEntity(this.Loc.GetString(locString, (nameof (attacker), (object) Identity.Entity(attacker, (IEntityManager) this.EntityManager)), (nameof (victim), (object) Identity.Entity(victim, (IEntityManager) this.EntityManager)), (nameof (weapon), (object) weapon)), attacker, Filter.PvsExcept(attacker), true, PopupType.MediumCaution);
  }

  private void OnExecutionDoAfter(Entity<ExecutionComponent> entity, ref ExecutionDoAfterEvent args)
  {
    if (args.Handled || args.Cancelled)
      return;
    EntityUid? nullable = args.Used;
    if (!nullable.HasValue)
      return;
    nullable = args.Target;
    MeleeWeaponComponent comp;
    if (!nullable.HasValue || !this.TryComp<MeleeWeaponComponent>((EntityUid) entity, out comp))
      return;
    EntityUid user = args.User;
    nullable = args.Target;
    EntityUid entityUid = nullable.Value;
    nullable = args.Used;
    EntityUid weaponUid = nullable.Value;
    if (!this._execution.CanBeExecuted(entityUid, user))
      return;
    bool flag = this._combat.IsInCombatMode(new EntityUid?(user));
    this._combat.SetInCombatMode(user, true);
    entity.Comp.Executing = true;
    LocId executionMessage1 = entity.Comp.CompleteInternalMeleeExecutionMessage;
    LocId executionMessage2 = entity.Comp.CompleteExternalMeleeExecutionMessage;
    if (user == entityUid)
    {
      SuicideEvent args1 = new SuicideEvent(entityUid);
      this.RaiseLocalEvent<SuicideEvent>(entityUid, args1);
      SuicideGhostEvent args2 = new SuicideGhostEvent(entityUid);
      this.RaiseLocalEvent<SuicideGhostEvent>(entityUid, args2);
    }
    else
      this._melee.AttemptLightAttack(user, weaponUid, comp, entityUid);
    this._combat.SetInCombatMode(user, flag);
    entity.Comp.Executing = false;
    args.Handled = true;
    if (!(user != entityUid))
      return;
    this._execution.ShowExecutionInternalPopup((string) executionMessage1, user, entityUid, (EntityUid) entity);
    this._execution.ShowExecutionExternalPopup((string) executionMessage2, user, entityUid, (EntityUid) entity);
  }
}
