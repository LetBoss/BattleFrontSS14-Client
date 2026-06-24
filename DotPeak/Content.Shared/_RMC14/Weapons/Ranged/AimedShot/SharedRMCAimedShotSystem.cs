// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.AimedShot.SharedRMCAimedShotSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Projectiles.Aimed;
using Content.Shared._RMC14.Rangefinder.Spotting;
using Content.Shared._RMC14.Targeting;
using Content.Shared._RMC14.Weapons.Ranged.Homing;
using Content.Shared._RMC14.Weapons.Ranged.Laser;
using Content.Shared._RMC14.Weapons.Ranged.Whitelist;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.CombatMode;
using Content.Shared.Examine;
using Content.Shared.Popups;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Content.Shared.Whitelist;
using Content.Shared.Wieldable.Components;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.AimedShot;

public abstract class SharedRMCAimedShotSystem : EntitySystem
{
  [Dependency]
  private SharedRMCTargetingSystem _targeting;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedGunSystem _gunSystem;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private EntityWhitelistSystem _whitelist;
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private ExamineSystemShared _examine;
  [Dependency]
  private SharedCombatModeSystem _combatMode;
  [Dependency]
  protected IGameTiming Timing;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<AimedShotComponent, GetItemActionsEvent>(new EntityEventRefHandler<AimedShotComponent, GetItemActionsEvent>(this.OnAimedShotGetItemActions));
    this.SubscribeLocalEvent<AimedShotComponent, AimedShotActionEvent>(new EntityEventRefHandler<AimedShotComponent, AimedShotActionEvent>(this.OnAimedShot));
    this.SubscribeLocalEvent<AimedShotComponent, TargetingFinishedEvent>(new EntityEventRefHandler<AimedShotComponent, TargetingFinishedEvent>(this.OnTargetingFinished));
    this.SubscribeLocalEvent<AimedShotComponent, TargetingCancelledEvent>(new EntityEventRefHandler<AimedShotComponent, TargetingCancelledEvent>(this.OnTargetingCancelled));
    this.SubscribeLocalEvent<AimedShotComponent, AmmoShotEvent>(new EntityEventRefHandler<AimedShotComponent, AmmoShotEvent>(this.OnAmmoShot));
  }

  private void OnAimedShotGetItemActions(
    Entity<AimedShotComponent> ent,
    ref GetItemActionsEvent args)
  {
    args.AddAction(ref ent.Comp.Action, (string) ent.Comp.ActionId);
    GunUserWhitelistComponent comp;
    ent.Comp.Whitelist = !this.TryComp<GunUserWhitelistComponent>(args.Provider, out comp) ? new EntityWhitelist() : comp.Whitelist;
    this.Dirty<AimedShotComponent>(ent);
  }

  protected void AimedShotRequested(NetEntity netGun, NetEntity netUser, NetEntity netTarget)
  {
    EntityUid entity1 = this.GetEntity(netGun);
    EntityUid entity2 = this.GetEntity(netUser);
    EntityUid entity3 = this.GetEntity(netTarget);
    AimedShotComponent comp1;
    if (!this.TryComp<AimedShotComponent>(entity1, out comp1) || !comp1.Activated || !this.CanAimShot((Entity<AimedShotComponent>) (entity1, comp1), entity3, entity2))
      return;
    float num1 = comp1.AimDuration + (this._transform.GetMoverCoordinates(entity3).Position - this._transform.GetMoverCoordinates(entity2).Position).Length() * (float) comp1.AimDistanceDifficulty;
    bool flag = false;
    float num2 = 1f;
    TargetedEffects targetEffect = comp1.TargetEffect;
    DirectionTargetedEffects directionEffect = comp1.DirectionTargetEffect;
    SpottedComponent comp2;
    if (this.TryComp<SpottedComponent>(entity3, out comp2))
    {
      num2 = comp2.AimDurationMultiplier;
      flag = true;
    }
    GunToggleableLaserComponent comp3;
    if (this.TryComp<GunToggleableLaserComponent>(entity1, out comp3))
    {
      if (comp3.Active)
      {
        if (flag)
          num2 -= comp3.SpottedAimDurationMultiplierSubtraction;
        else
          num2 = comp3.AimDurationMultiplier;
        directionEffect = DirectionTargetedEffects.None;
      }
      TargetingLaserComponent comp4;
      if (this.TryComp<TargetingLaserComponent>(entity1, out comp4))
      {
        comp4.ShowLaser = comp3.Active;
        this.Dirty(entity1, (IComponent) comp4);
      }
    }
    float targetingDuration = num1 * num2;
    comp1.Targets.Add(entity3);
    comp1.NextAimedShot = this.Timing.CurTime + comp1.AimedShotCooldown;
    this.Dirty(entity1, (IComponent) comp1);
    this._audio.PlayPredicted(comp1.AimingSound, entity1, new EntityUid?(entity2));
    this._targeting.Target(entity1, entity2, entity3, targetingDuration, targetEffect, directionEffect);
  }

  private void OnAimedShot(Entity<AimedShotComponent> ent, ref AimedShotActionEvent args)
  {
    ent.Comp.Activated = !ent.Comp.Activated;
    this.Dirty<AimedShotComponent>(ent);
    SharedActionsSystem actions = this._actions;
    EntityUid? action1 = ent.Comp.Action;
    Entity<ActionComponent>? action2 = action1.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) action1.GetValueOrDefault()) : new Entity<ActionComponent>?();
    int num = ent.Comp.Activated ? 1 : 0;
    actions.SetToggled(action2, num != 0);
    args.Handled = true;
  }

  private void OnAmmoShot(Entity<AimedShotComponent> ent, ref AmmoShotEvent args)
  {
    if (!ent.Comp.CurrentTarget.HasValue || this.TerminatingOrDeleted(ent.Comp.CurrentTarget))
      return;
    EntityUid entityUid = ent.Comp.CurrentTarget.Value;
    foreach (EntityUid firedProjectile in args.FiredProjectiles)
    {
      AimedShotEvent args1 = new AimedShotEvent(entityUid);
      this.RaiseLocalEvent<AimedShotEvent>((EntityUid) ent, ref args1);
      AimedProjectileComponent projectileComponent1 = this.EnsureComp<AimedProjectileComponent>(firedProjectile);
      projectileComponent1.Target = entityUid;
      projectileComponent1.Source = (EntityUid) ent;
      this.Dirty(firedProjectile, (IComponent) projectileComponent1);
      HomingProjectileComponent projectileComponent2 = this.EnsureComp<HomingProjectileComponent>(firedProjectile);
      projectileComponent2.Target = entityUid;
      projectileComponent2.ProjectileSpeed = ent.Comp.ProjectileSpeed;
      this.Dirty(firedProjectile, (IComponent) projectileComponent2);
      TargetedProjectileComponent projectileComponent3 = this.EnsureComp<TargetedProjectileComponent>(firedProjectile);
      projectileComponent3.Target = entityUid;
      this.Dirty(firedProjectile, (IComponent) projectileComponent3);
      ShotByAimedShotEvent args2 = new ShotByAimedShotEvent((EntityUid) ent, entityUid);
      this.RaiseLocalEvent<ShotByAimedShotEvent>(firedProjectile, ref args2);
    }
    this.RemoveTarget(ent, entityUid);
  }

  private void OnTargetingFinished(Entity<AimedShotComponent> ent, ref TargetingFinishedEvent args)
  {
    GunComponent comp;
    if (!this.TryComp<GunComponent>((EntityUid) ent, out comp))
      return;
    if (!this._examine.InRangeUnOccluded(args.User, args.Target, (float) ent.Comp.Range))
    {
      this.RemoveTarget(ent, args.Target);
      this._popup.PopupClient(this.Loc.GetString("rmc-action-popup-aiming-target-blocked", ("gun", (object) ent)), args.User, new EntityUid?(args.User));
    }
    else
    {
      ent.Comp.CurrentTarget = new EntityUid?(args.Target);
      this.Dirty<AimedShotComponent>(ent);
      if (this._net.IsServer)
      {
        List<EntityUid> entityUidList = this._gunSystem.AttemptShoot((Entity<GunComponent>) ((EntityUid) ent, comp), args.User, args.Coordinates);
        GetAmmoCountEvent args1 = new GetAmmoCountEvent();
        this.RaiseLocalEvent<GetAmmoCountEvent>((EntityUid) ent, ref args1);
        if (entityUidList != null)
        {
          this._audio.PlayEntity(comp.SoundGunshotModified, args.User, (EntityUid) ent);
          if (args1.Count == 0)
            this._audio.PlayPvs((SoundSpecifier) new SoundPathSpecifier("/Audio/Weapons/Guns/EmptyAlarm/smg_empty_alarm.ogg"), (EntityUid) ent);
        }
        else
        {
          this.RemoveTarget(ent, args.Target);
          if (args1.Count < 0)
            this._audio.PlayEntity(comp.SoundEmpty, args.User, (EntityUid) ent);
        }
      }
      UpdateClientAmmoEvent args2 = new UpdateClientAmmoEvent();
      this.RaiseLocalEvent<UpdateClientAmmoEvent>((EntityUid) ent, ref args2);
    }
  }

  private void OnTargetingCancelled(
    Entity<AimedShotComponent> ent,
    ref TargetingCancelledEvent args)
  {
    if (args.Handled)
      return;
    ent.Comp.Targets.Clear();
    this.Dirty<AimedShotComponent>(ent);
    args.Handled = true;
  }

  private bool CanAimShot(Entity<AimedShotComponent> ent, EntityUid target, EntityUid user)
  {
    if (!this.HasComp<SpottableComponent>(target) || !this._examine.InRangeUnOccluded(user, target, (float) ent.Comp.Range) || ent.Comp.NextAimedShot > this.Timing.CurTime || !this._combatMode.IsInCombatMode(new EntityUid?(user)))
      return false;
    if (!this._whitelist.IsValid(ent.Comp.Whitelist, user) && ent.Comp.Whitelist.Components != null)
    {
      this._popup.PopupClient(this.Loc.GetString("cm-gun-unskilled", ("gun", (object) ent)), user, new EntityUid?(user), PopupType.SmallCaution);
      return false;
    }
    WieldableComponent comp;
    if (this.TryComp<WieldableComponent>((EntityUid) ent, out comp) && !comp.Wielded)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-action-popup-aiming-user-must-wield", ("gun", (object) ent)), user, new EntityUid?(user));
      return false;
    }
    GetAmmoCountEvent args = new GetAmmoCountEvent();
    this.RaiseLocalEvent<GetAmmoCountEvent>((EntityUid) ent, ref args);
    if (args.Count <= 0)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-action-popup-aiming-gun-no-ammo", ("gun", (object) ent)), user, new EntityUid?(user));
      return false;
    }
    if (!this._transform.InRange(this.Transform(target).Coordinates, this.Transform(user).Coordinates, (float) ent.Comp.MinRange))
      return true;
    this._popup.PopupClient(this.Loc.GetString("rmc-action-popup-aiming-target-too-close", (nameof (target), (object) target)), user, new EntityUid?(user));
    return false;
  }

  private void RemoveTarget(Entity<AimedShotComponent> ent, EntityUid target)
  {
    this._targeting.StopTargeting((Entity<TargetingComponent>) ent.Owner, target);
    ent.Comp.Targets.Remove(target);
    ent.Comp.CurrentTarget = new EntityUid?();
    this.Dirty<AimedShotComponent>(ent);
  }
}
