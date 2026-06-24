// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Stamina.RMCStaminaSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.BlurredVision;
using Content.Shared._RMC14.Movement;
using Content.Shared._RMC14.Stun;
using Content.Shared.Administration.Logs;
using Content.Shared.Alert;
using Content.Shared.Damage.Events;
using Content.Shared.Database;
using Content.Shared.Effects;
using Content.Shared.Movement.Systems;
using Content.Shared.Projectiles;
using Content.Shared.Rejuvenate;
using Content.Shared.Speech.EntitySystems;
using Content.Shared.StatusEffect;
using Content.Shared.Throwing;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Wieldable.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Stamina;

public sealed class RMCStaminaSystem : EntitySystem
{
  [Dependency]
  private INetManager _net;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private StatusEffectsSystem _status;
  [Dependency]
  private SharedStutteringSystem _stutter;
  [Dependency]
  private RMCDazedSystem _daze;
  [Dependency]
  private MovementSpeedModifierSystem _speed;
  [Dependency]
  private TemporarySpeedModifiersSystem _temporarySpeed;
  [Dependency]
  private SharedColorFlashEffectSystem _color;
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private AlertsSystem _alerts;
  [Dependency]
  private RMCSizeStunSystem _sizeStun;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RMCStaminaComponent, ComponentStartup>(new EntityEventRefHandler<RMCStaminaComponent, ComponentStartup>(this.OnStaminaStartup));
    this.SubscribeLocalEvent<RMCStaminaComponent, RefreshMovementSpeedModifiersEvent>(new EntityEventRefHandler<RMCStaminaComponent, RefreshMovementSpeedModifiersEvent>(this.OnStaminaMovementSpeedModify));
    this.SubscribeLocalEvent<RMCStaminaComponent, RejuvenateEvent>(new EntityEventRefHandler<RMCStaminaComponent, RejuvenateEvent>(this.OnStaminaRejuvenate));
    this.SubscribeLocalEvent<RMCStaminaDamageOnHitComponent, MeleeHitEvent>(new EntityEventRefHandler<RMCStaminaDamageOnHitComponent, MeleeHitEvent>(this.OnStaminaOnHit));
    this.SubscribeLocalEvent<RMCStaminaDamageOnCollideComponent, ProjectileHitEvent>(new EntityEventRefHandler<RMCStaminaDamageOnCollideComponent, ProjectileHitEvent>(this.OnStaminaOnProjectileHit));
    this.SubscribeLocalEvent<RMCStaminaDamageOnCollideComponent, ThrowDoHitEvent>(new EntityEventRefHandler<RMCStaminaDamageOnCollideComponent, ThrowDoHitEvent>(this.OnStaminaOnThrowHit));
  }

  private void OnStaminaStartup(Entity<RMCStaminaComponent> ent, ref ComponentStartup args)
  {
    this.SetStaminaAlert(ent);
  }

  private void OnStaminaRejuvenate(Entity<RMCStaminaComponent> ent, ref RejuvenateEvent args)
  {
    this.DoStaminaDamage((Entity<RMCStaminaComponent>) ((EntityUid) ent, ent.Comp), -ent.Comp.Max, false);
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCStaminaComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCStaminaComponent>();
    EntityUid uid;
    RMCStaminaComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (comp1.Current != comp1.Max)
      {
        if (curTime >= comp1.NextRegen)
          this.DoStaminaDamage((Entity<RMCStaminaComponent>) (uid, comp1), (double) -comp1.RegenPerTick);
        else if (curTime >= comp1.NextCheck)
          this.ProcessEffects((Entity<RMCStaminaComponent>) (uid, comp1));
      }
    }
  }

  public void DoStaminaDamage(Entity<RMCStaminaComponent?> ent, double amount, bool visual = true)
  {
    if (!this.Resolve<RMCStaminaComponent>((EntityUid) ent, ref ent.Comp, false))
      return;
    ent.Comp.Current = Math.Clamp(ent.Comp.Current - amount, 0.0, ent.Comp.Max);
    if (visual && amount > 0.0 && this._timing.IsFirstTimePredicted)
    {
      SharedColorFlashEffectSystem color = this._color;
      Color aqua = Color.Aqua;
      List<EntityUid> entities = new List<EntityUid>();
      entities.Add((EntityUid) ent);
      Filter filter = Filter.Pvs((EntityUid) ent, entityManager: (IEntityManager) this.EntityManager);
      color.RaiseEffect(aqua, entities, filter);
    }
    this.ProcessEffects((Entity<RMCStaminaComponent>) ((EntityUid) ent, ent.Comp));
    ent.Comp.NextRegen = this._timing.CurTime + (amount > 0.0 ? ent.Comp.RestPeriod : ent.Comp.TimeBetweenChecks);
    this.SetStaminaAlert((Entity<RMCStaminaComponent>) ((EntityUid) ent, ent.Comp));
  }

  private void ProcessEffects(Entity<RMCStaminaComponent> ent)
  {
    ent.Comp.NextCheck = this._timing.CurTime + ent.Comp.TimeBetweenChecks;
    int index = 0;
    while (index < ent.Comp.TierThresholds.Length && ent.Comp.Current < (double) ent.Comp.TierThresholds[index])
      ++index;
    if (index >= 2)
    {
      this._status.TryAddStatusEffect<RMCBlindedComponent>((EntityUid) ent, "Blinded", ent.Comp.EffectTime, true);
      this._stutter.DoStutter((EntityUid) ent, ent.Comp.EffectTime, true);
    }
    if (index >= 3 && index != ent.Comp.Level)
      this._daze.TryDaze((EntityUid) ent, ent.Comp.EffectTime, true, stutter: true);
    if (index >= 4)
      this._sizeStun.TryKnockOut((EntityUid) ent, ent.Comp.EffectTime);
    int level = ent.Comp.Level;
    ent.Comp.Level = index;
    this.Dirty<RMCStaminaComponent>(ent);
    if (index == level)
      return;
    this._speed.RefreshMovementSpeedModifiers((EntityUid) ent);
  }

  private void OnStaminaMovementSpeedModify(
    Entity<RMCStaminaComponent> ent,
    ref RefreshMovementSpeedModifiersEvent args)
  {
    if (ent.Comp.Level > ent.Comp.SpeedModifiers.Length || ent.Comp.Level < 0)
      return;
    float? speedModifier = this._temporarySpeed.CalculateSpeedModifier((EntityUid) ent, ent.Comp.SpeedModifiers[ent.Comp.Level]);
    if (!speedModifier.HasValue)
      return;
    args.ModifySpeed(speedModifier.Value, speedModifier.Value);
  }

  private void OnStaminaOnHit(Entity<RMCStaminaDamageOnHitComponent> ent, ref MeleeHitEvent args)
  {
    WieldableComponent comp;
    if (ent.Comp.RequiresWield && this.TryComp<WieldableComponent>(ent.Owner, out comp) && !comp.Wielded || !args.IsHit || !args.HitEntities.Any<EntityUid>() || ent.Comp.Damage <= 0.0)
      return;
    StaminaDamageOnHitAttemptEvent args1 = new StaminaDamageOnHitAttemptEvent();
    this.RaiseLocalEvent<StaminaDamageOnHitAttemptEvent>((EntityUid) ent, ref args1);
    if (args1.Cancelled)
      return;
    Robust.Shared.GameObjects.EntityQuery<RMCStaminaComponent> entityQuery = this.GetEntityQuery<RMCStaminaComponent>();
    List<(EntityUid, RMCStaminaComponent)> valueTupleList = new List<(EntityUid, RMCStaminaComponent)>();
    foreach (EntityUid hitEntity in (IEnumerable<EntityUid>) args.HitEntities)
    {
      RMCStaminaComponent component;
      if (entityQuery.TryGetComponent(hitEntity, out component))
        valueTupleList.Add((hitEntity, component));
    }
    double damage = ent.Comp.Damage;
    foreach ((EntityUid ent1, RMCStaminaComponent _) in valueTupleList)
    {
      this.DoStaminaDamage((Entity<RMCStaminaComponent>) ent1, damage / (double) valueTupleList.Count);
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(39, 4);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) ent1), "target", "ToPrettyString(hit)");
      logStringHandler.AppendLiteral(" was dealt ");
      logStringHandler.AppendFormatted<double>(damage, "damage");
      logStringHandler.AppendLiteral(" stamina damage from ");
      logStringHandler.AppendFormatted<EntityUid>(args.User, "args.User");
      logStringHandler.AppendLiteral(" with ");
      logStringHandler.AppendFormatted<EntityUid>(args.Weapon, "args.Weapon");
      logStringHandler.AppendLiteral(".");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.Stamina, ref local);
    }
  }

  private void OnStaminaOnProjectileHit(
    Entity<RMCStaminaDamageOnCollideComponent> ent,
    ref ProjectileHitEvent args)
  {
    this.OnCollide(ent, args.Target);
  }

  private void OnStaminaOnThrowHit(
    Entity<RMCStaminaDamageOnCollideComponent> ent,
    ref ThrowDoHitEvent args)
  {
    this.OnCollide(ent, args.Target);
  }

  private void OnCollide(Entity<RMCStaminaDamageOnCollideComponent> ent, EntityUid target)
  {
    RMCStaminaComponent comp;
    if (!this.TryComp<RMCStaminaComponent>(target, out comp))
      return;
    this.DoStaminaDamage((Entity<RMCStaminaComponent>) (target, comp), ent.Comp.Damage);
  }

  private void SetStaminaAlert(Entity<RMCStaminaComponent> ent)
  {
    if (!ent.Comp.ShowAlert)
      this._alerts.ClearAlert((EntityUid) ent, ent.Comp.StaminaAlert);
    else
      this._alerts.ShowAlert((EntityUid) ent, ent.Comp.StaminaAlert, new short?((short) (ent.Comp.TierThresholds.Length - 1 - ent.Comp.Level)));
  }
}
