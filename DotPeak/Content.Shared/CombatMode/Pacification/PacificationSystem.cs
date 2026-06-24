// Decompiled with JetBrains decompiler
// Type: Content.Shared.CombatMode.Pacification.PacificationSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Alert;
using Content.Shared.FixedPoint;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Content.Shared.Throwing;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared.CombatMode.Pacification;

public sealed class PacificationSystem : EntitySystem
{
  [Dependency]
  private AlertsSystem _alertsSystem;
  [Dependency]
  private SharedActionsSystem _actionsSystem;
  [Dependency]
  private SharedCombatModeSystem _combatSystem;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IGameTiming _timing;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PacifiedComponent, ComponentStartup>(new ComponentEventHandler<PacifiedComponent, ComponentStartup>((object) this, __methodptr(OnStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PacifiedComponent, ComponentShutdown>(new ComponentEventHandler<PacifiedComponent, ComponentShutdown>((object) this, __methodptr(OnShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PacifiedComponent, BeforeThrowEvent>(new EntityEventRefHandler<PacifiedComponent, BeforeThrowEvent>((object) this, __methodptr(OnBeforeThrow)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PacifiedComponent, AttackAttemptEvent>(new ComponentEventHandler<PacifiedComponent, AttackAttemptEvent>((object) this, __methodptr(OnAttackAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PacifiedComponent, ShotAttemptedEvent>(new EntityEventRefHandler<PacifiedComponent, ShotAttemptedEvent>((object) this, __methodptr(OnShootAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PacifismDangerousAttackComponent, AttemptPacifiedAttackEvent>(new EntityEventRefHandler<PacifismDangerousAttackComponent, AttemptPacifiedAttackEvent>((object) this, __methodptr(OnPacifiedDangerousAttack)), (Type[]) null, (Type[]) null);
  }

  private bool PacifiedCanAttack(EntityUid user, EntityUid target, [NotNullWhen(false)] out string? reason)
  {
    AttemptPacifiedAttackEvent pacifiedAttackEvent = new AttemptPacifiedAttackEvent(user);
    this.RaiseLocalEvent<AttemptPacifiedAttackEvent>(target, ref pacifiedAttackEvent, false);
    if (pacifiedAttackEvent.Cancelled)
    {
      reason = pacifiedAttackEvent.Reason;
      return false;
    }
    reason = (string) null;
    return true;
  }

  private void ShowPopup(Entity<PacifiedComponent> user, EntityUid target, string reason)
  {
    EntityUid entityUid1 = target;
    EntityUid? nullable = user.Comp.LastAttackedEntity;
    if ((nullable.HasValue ? (EntityUid.op_Equality(entityUid1, nullable.GetValueOrDefault()) ? 1 : 0) : 0) != 0)
    {
      TimeSpan curTime = this._timing.CurTime;
      TimeSpan? nextPopupTime = user.Comp.NextPopupTime;
      if ((nextPopupTime.HasValue ? (curTime > nextPopupTime.GetValueOrDefault() ? 1 : 0) : 0) == 0)
        return;
    }
    EntityUid uid = target;
    EntityManager entityManager = this.EntityManager;
    nullable = new EntityUid?();
    EntityUid? viewer = nullable;
    EntityUid entityUid2 = Identity.Entity(uid, (IEntityManager) entityManager, viewer);
    this._popup.PopupClient(this.Loc.GetString(reason, ("entity", (object) entityUid2)), Entity<PacifiedComponent>.op_Implicit(user), new EntityUid?(Entity<PacifiedComponent>.op_Implicit(user)));
    user.Comp.NextPopupTime = new TimeSpan?(this._timing.CurTime + user.Comp.PopupCooldown);
    user.Comp.LastAttackedEntity = new EntityUid?(target);
  }

  private void OnShootAttempt(Entity<PacifiedComponent> ent, ref ShotAttemptedEvent args)
  {
    if (this.HasComp<PacifismAllowedGunComponent>(Entity<GunComponent>.op_Implicit(args.Used)))
      return;
    this.ShowPopup(ent, Entity<GunComponent>.op_Implicit(args.Used), "pacified-cannot-fire-gun");
    args.Cancel();
  }

  private void OnAttackAttempt(EntityUid uid, PacifiedComponent component, AttackAttemptEvent args)
  {
    if (component.DisallowAllCombat || args.Disarm && component.DisallowDisarm)
    {
      args.Cancel();
    }
    else
    {
      if (args.Disarm || !args.Target.HasValue || args.Weapon.HasValue && args.Weapon.Value.Comp.Damage.GetTotal() == FixedPoint2.Zero)
        return;
      EntityUid user1 = uid;
      EntityUid? target1 = args.Target;
      EntityUid target2 = target1.Value;
      string str;
      ref string local = ref str;
      if (this.PacifiedCanAttack(user1, target2, out local))
        return;
      Entity<PacifiedComponent> user2 = Entity<PacifiedComponent>.op_Implicit((uid, component));
      target1 = args.Target;
      EntityUid target3 = target1.Value;
      string reason = str;
      this.ShowPopup(user2, target3, reason);
      args.Cancel();
    }
  }

  private void OnStartup(EntityUid uid, PacifiedComponent component, ComponentStartup args)
  {
    CombatModeComponent component1;
    if (!this.TryComp<CombatModeComponent>(uid, ref component1))
      return;
    if (component.DisallowDisarm && component1.CanDisarm.HasValue)
      this._combatSystem.SetCanDisarm(uid, false, component1);
    if (component.DisallowAllCombat)
    {
      this._combatSystem.SetInCombatMode(uid, false, component1);
      SharedActionsSystem actionsSystem = this._actionsSystem;
      EntityUid? toggleActionEntity = component1.CombatToggleActionEntity;
      Entity<ActionComponent>? action = toggleActionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(toggleActionEntity.GetValueOrDefault())) : new Entity<ActionComponent>?();
      actionsSystem.SetEnabled(action, false);
    }
    this._alertsSystem.ShowAlert(uid, component.PacifiedAlert);
  }

  private void OnShutdown(EntityUid uid, PacifiedComponent component, ComponentShutdown args)
  {
    CombatModeComponent component1;
    if (!this.TryComp<CombatModeComponent>(uid, ref component1))
      return;
    if (component1.CanDisarm.HasValue)
      this._combatSystem.SetCanDisarm(uid, true, component1);
    SharedActionsSystem actionsSystem = this._actionsSystem;
    EntityUid? toggleActionEntity = component1.CombatToggleActionEntity;
    Entity<ActionComponent>? action = toggleActionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(toggleActionEntity.GetValueOrDefault())) : new Entity<ActionComponent>?();
    actionsSystem.SetEnabled(action, true);
    this._alertsSystem.ClearAlert(uid, component.PacifiedAlert);
  }

  private void OnBeforeThrow(Entity<PacifiedComponent> ent, ref BeforeThrowEvent args)
  {
    EntityUid itemUid = args.ItemUid;
    EntityUid entityUid = Identity.Entity(itemUid, (IEntityManager) this.EntityManager);
    AttemptPacifiedThrowEvent pacifiedThrowEvent = new AttemptPacifiedThrowEvent(itemUid, Entity<PacifiedComponent>.op_Implicit(ent));
    this.RaiseLocalEvent<AttemptPacifiedThrowEvent>(itemUid, ref pacifiedThrowEvent, false);
    if (!pacifiedThrowEvent.Cancelled)
      return;
    args.Cancelled = true;
    this._popup.PopupEntity(this.Loc.GetString(pacifiedThrowEvent.CancelReasonMessageId ?? "pacified-cannot-throw", ("projectile", (object) entityUid)), Entity<PacifiedComponent>.op_Implicit(ent), Entity<PacifiedComponent>.op_Implicit(ent));
  }

  private void OnPacifiedDangerousAttack(
    Entity<PacifismDangerousAttackComponent> ent,
    ref AttemptPacifiedAttackEvent args)
  {
    args.Cancelled = true;
    args.Reason = "pacified-cannot-harm-indirect";
  }
}
