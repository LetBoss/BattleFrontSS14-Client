// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Crippling.XenoCripplingStrikeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Aura;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared.Actions;
using Content.Shared.Damage;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Crippling;

public sealed class XenoCripplingStrikeSystem : EntitySystem
{
  [Dependency]
  private RMCSlowSystem _slow;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private SharedRMCMeleeWeaponSystem _rmcMelee;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private SharedAuraSystem _aura;
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeed;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<XenoCripplingStrikeComponent, XenoCripplingStrikeActionEvent>(new EntityEventRefHandler<XenoCripplingStrikeComponent, XenoCripplingStrikeActionEvent>(this.OnXenoCripplingStrikeAction));
    this.SubscribeLocalEvent<XenoActiveCripplingStrikeComponent, MeleeHitEvent>(new EntityEventRefHandler<XenoActiveCripplingStrikeComponent, MeleeHitEvent>(this.OnXenoCripplingStrikeHit));
    this.SubscribeLocalEvent<XenoActiveCripplingStrikeComponent, MeleeAttackAttemptEvent>(new EntityEventRefHandler<XenoActiveCripplingStrikeComponent, MeleeAttackAttemptEvent>(this.OnActiveCripplingStrikeMeleeAttempt));
    this.SubscribeLocalEvent<XenoActiveCripplingStrikeComponent, RefreshMovementSpeedModifiersEvent>(new EntityEventRefHandler<XenoActiveCripplingStrikeComponent, RefreshMovementSpeedModifiersEvent>(this.OnActiveCripplingRefreshSpeed));
    this.SubscribeLocalEvent<XenoActiveCripplingStrikeComponent, ComponentRemove>(new EntityEventRefHandler<XenoActiveCripplingStrikeComponent, ComponentRemove>(this.OnActiveCripplingRemove));
    this.SubscribeLocalEvent<VictimCripplingStrikeDamageComponent, DamageModifyEvent>(new EntityEventRefHandler<VictimCripplingStrikeDamageComponent, DamageModifyEvent>(this.OnVictimCripplingModify), new Type[1]
    {
      typeof (CMArmorSystem)
    });
  }

  private void OnXenoCripplingStrikeAction(
    Entity<XenoCripplingStrikeComponent> xeno,
    ref XenoCripplingStrikeActionEvent args)
  {
    if (args.Handled || !this._rmcActions.TryUseAction((InstantActionEvent) args))
      return;
    args.Handled = true;
    XenoActiveCripplingStrikeComponent cripplingStrikeComponent = this.EnsureComp<XenoActiveCripplingStrikeComponent>((EntityUid) xeno);
    if (xeno.Comp.ResetMeleeCooldown)
    {
      MeleeResetComponent meleeResetComponent = this.EnsureComp<MeleeResetComponent>((EntityUid) xeno);
      this._rmcMelee.MeleeResetInit((Entity<MeleeResetComponent>) (xeno.Owner, meleeResetComponent));
    }
    cripplingStrikeComponent.ExpireAt = this._timing.CurTime + xeno.Comp.ActiveDuration;
    cripplingStrikeComponent.NextSlashBuffed = true;
    cripplingStrikeComponent.SlowDuration = xeno.Comp.SlowDuration;
    cripplingStrikeComponent.DamageMult = xeno.Comp.DamageMult;
    cripplingStrikeComponent.HitText = xeno.Comp.HitText;
    cripplingStrikeComponent.DeactivateText = xeno.Comp.DeactivateText;
    cripplingStrikeComponent.ExpireText = xeno.Comp.ExpireText;
    cripplingStrikeComponent.Speed = xeno.Comp.Speed;
    cripplingStrikeComponent.RemoveOnHit = xeno.Comp.RemoveOnHit;
    cripplingStrikeComponent.PreventTackle = xeno.Comp.PreventTackle;
    this.Dirty((EntityUid) xeno, (IComponent) cripplingStrikeComponent);
    this._popup.PopupClient(this.Loc.GetString((string) xeno.Comp.ActivateText), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) xeno);
    Color? auraColor = xeno.Comp.AuraColor;
    if (!auraColor.HasValue)
      return;
    Color valueOrDefault = auraColor.GetValueOrDefault();
    this._aura.GiveAura((EntityUid) xeno, valueOrDefault, new TimeSpan?(xeno.Comp.ActiveDuration), 1f);
  }

  private void OnXenoCripplingStrikeHit(
    Entity<XenoActiveCripplingStrikeComponent> xeno,
    ref MeleeHitEvent args)
  {
    if (!xeno.Comp.NextSlashBuffed || !args.IsHit || args.HitEntities.Count == 0)
      return;
    foreach (EntityUid hitEntity in (IEnumerable<EntityUid>) args.HitEntities)
    {
      if (this._xeno.CanAbilityAttackTarget((EntityUid) xeno, hitEntity) && !this.HasComp<VictimCripplingStrikeDamageComponent>(hitEntity))
      {
        VictimCripplingStrikeDamageComponent strikeDamageComponent = this.EnsureComp<VictimCripplingStrikeDamageComponent>(hitEntity);
        strikeDamageComponent.DamageMult = xeno.Comp.DamageMult;
        this.Dirty(hitEntity, (IComponent) strikeDamageComponent);
        this._slow.TrySlowdown(hitEntity, this._xeno.TryApplyXenoDebuffMultiplier(hitEntity, xeno.Comp.SlowDuration), ignoreDurationModifier: true);
        string message = this.Loc.GetString((string) xeno.Comp.HitText, ("target", (object) hitEntity));
        if (this._net.IsServer)
          this._popup.PopupEntity(message, hitEntity, (EntityUid) xeno);
        xeno.Comp.NextSlashBuffed = false;
        break;
      }
    }
    if (!xeno.Comp.RemoveOnHit)
      return;
    this.RemCompDeferred<XenoActiveCripplingStrikeComponent>((EntityUid) xeno);
  }

  private void OnActiveCripplingRefreshSpeed(
    Entity<XenoActiveCripplingStrikeComponent> xeno,
    ref RefreshMovementSpeedModifiersEvent args)
  {
    float? speed = xeno.Comp.Speed;
    if (!speed.HasValue)
      return;
    float valueOrDefault = speed.GetValueOrDefault();
    args.ModifySpeed(valueOrDefault, valueOrDefault);
  }

  private void OnActiveCripplingRemove(
    Entity<XenoActiveCripplingStrikeComponent> xeno,
    ref ComponentRemove args)
  {
    if (this.TerminatingOrDeleted((EntityUid) xeno))
      return;
    this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) xeno);
  }

  private void OnVictimCripplingModify(
    Entity<VictimCripplingStrikeDamageComponent> victim,
    ref DamageModifyEvent args)
  {
    args.Damage *= victim.Comp.DamageMult;
    this.RemCompDeferred<VictimCripplingStrikeDamageComponent>((EntityUid) victim);
  }

  private void OnActiveCripplingStrikeMeleeAttempt(
    Entity<XenoActiveCripplingStrikeComponent> ent,
    ref MeleeAttackAttemptEvent args)
  {
    if (!ent.Comp.PreventTackle)
      return;
    NetEntity netEntity = this.GetNetEntity((EntityUid) ent);
    if (!(args.Attack is DisarmAttackEvent attack))
      return;
    args.Attack = (AttackEvent) new LightAttackEvent(attack.Target, netEntity, attack.Coordinates);
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoActiveCripplingStrikeComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoActiveCripplingStrikeComponent>();
    EntityUid uid;
    XenoActiveCripplingStrikeComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!(curTime < comp1.ExpireAt))
      {
        if (comp1.NextSlashBuffed)
        {
          this._popup.PopupEntity(this.Loc.GetString((string) comp1.ExpireText), uid, uid, PopupType.SmallCaution);
        }
        else
        {
          LocId? deactivateText = comp1.DeactivateText;
          if (deactivateText.HasValue)
            this._popup.PopupEntity(this.Loc.GetString((string) deactivateText.GetValueOrDefault()), uid, uid, PopupType.MediumCaution);
        }
        this.RemCompDeferred<XenoActiveCripplingStrikeComponent>(uid);
      }
    }
  }
}
