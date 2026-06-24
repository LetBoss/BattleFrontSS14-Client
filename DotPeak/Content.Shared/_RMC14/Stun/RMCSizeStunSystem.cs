// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Stun.RMCSizeStunSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Deafness;
using Content.Shared._RMC14.Explosion;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stamina;
using Content.Shared.Coordinates;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Flash;
using Content.Shared.Interaction;
using Content.Shared.Pointing;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Speech.Muting;
using Content.Shared.Standing;
using Content.Shared.StatusEffect;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Stun;

public sealed class RMCSizeStunSystem : EntitySystem
{
  private const double DazedMultiplierSmallXeno = 0.7;
  private const double DazedMultiplierBigXeno = 1.2;
  private static readonly ProtoId<StatusEffectPrototype> KnockedOut = (ProtoId<StatusEffectPrototype>) "Unconscious";
  [Dependency]
  private RMCDazedSystem _dazed;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private EntityWhitelistSystem _entityWhitelist;
  [Dependency]
  private SharedFlashSystem _flash;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private RMCPullingSystem _rmcPulling;
  [Dependency]
  private RMCSlowSystem _slow;
  [Dependency]
  private RMCStaminaSystem _stamina;
  [Dependency]
  private StandingStateSystem _stand;
  [Dependency]
  private SharedStunSystem _stun;
  [Dependency]
  private ThrowingSystem _throwing;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private StatusEffectsSystem _status;
  [Dependency]
  private IGameTiming _timing;
  private readonly HashSet<Entity<MarineComponent>> _marines = new HashSet<Entity<MarineComponent>>();

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RMCStunOnHitComponent, MapInitEvent>(new EntityEventRefHandler<RMCStunOnHitComponent, MapInitEvent>(this.OnSizeStunMapInit));
    this.SubscribeLocalEvent<RMCStunOnHitComponent, ProjectileHitEvent>(new EntityEventRefHandler<RMCStunOnHitComponent, ProjectileHitEvent>(this.OnHit));
    this.SubscribeLocalEvent<RMCStunOnHitComponent, RMCTriggerEvent>(new EntityEventRefHandler<RMCStunOnHitComponent, RMCTriggerEvent>(this.OnTrigger));
    this.SubscribeLocalEvent<RMCStunOnTriggerComponent, RMCTriggerEvent>(new EntityEventRefHandler<RMCStunOnTriggerComponent, RMCTriggerEvent>(this.OnStunOnTrigger));
    this.SubscribeLocalEvent<RMCUnconsciousComponent, ComponentStartup>(new EntityEventRefHandler<RMCUnconsciousComponent, ComponentStartup>(this.OnUnconsciousStart));
    this.SubscribeLocalEvent<RMCUnconsciousComponent, ComponentShutdown>(new EntityEventRefHandler<RMCUnconsciousComponent, ComponentShutdown>(this.OnUnconsciousEnd));
    this.SubscribeLocalEvent<RMCUnconsciousComponent, StatusEffectEndedEvent>(new EntityEventRefHandler<RMCUnconsciousComponent, StatusEffectEndedEvent>(this.OnUnconsciousUpdate));
    this.SubscribeLocalEvent<RMCUnconsciousComponent, PointAttemptEvent>(new EntityEventRefHandler<RMCUnconsciousComponent, PointAttemptEvent>(this.OnUnconsciousPointAttempt));
    this.SubscribeLocalEvent<RMCKnockOutOnCollideComponent, ProjectileHitEvent>(new EntityEventRefHandler<RMCKnockOutOnCollideComponent, ProjectileHitEvent>(this.OnKnockOutCollideProjectileHit));
    this.SubscribeLocalEvent<RMCKnockOutOnCollideComponent, ThrowDoHitEvent>(new EntityEventRefHandler<RMCKnockOutOnCollideComponent, ThrowDoHitEvent>(this.OnKnockOutCollideThrowHit));
  }

  public bool IsHumanoidSized(Entity<RMCSizeComponent> ent) => ent.Comp.Size <= RMCSizes.Humanoid;

  public bool IsHumanoidSized(RMCSizes size) => size <= RMCSizes.Humanoid;

  public bool IsXenoSized(Entity<RMCSizeComponent?> ent)
  {
    return this.Resolve<RMCSizeComponent>((EntityUid) ent, ref ent.Comp, false) && ent.Comp.Size >= RMCSizes.VerySmallXeno;
  }

  public bool IsXenoSized(RMCSizes size) => size >= RMCSizes.VerySmallXeno;

  public bool TryGetSize(EntityUid ent, out RMCSizes size)
  {
    size = RMCSizes.Small;
    RMCSizeComponent comp;
    if (!this.TryComp<RMCSizeComponent>(ent, out comp))
      return false;
    size = comp.Size;
    return true;
  }

  private void OnSizeStunMapInit(Entity<RMCStunOnHitComponent> projectile, ref MapInitEvent args)
  {
    projectile.Comp.ShotFrom = new MapCoordinates?(this._transform.GetMapCoordinates(projectile.Owner));
    this.Dirty<RMCStunOnHitComponent>(projectile);
  }

  private void OnHit(Entity<RMCStunOnHitComponent> bullet, ref ProjectileHitEvent args)
  {
    if (!bullet.Comp.ShotFrom.HasValue)
      return;
    foreach (RMCStunOnHit stun in bullet.Comp.Stuns)
    {
      if (!this._entityWhitelist.IsWhitelistFail(stun.Whitelist, args.Target))
      {
        float num1 = (this._transform.GetMoverCoordinates(args.Target).Position - bullet.Comp.ShotFrom.Value.Position).Length();
        RMCSizeComponent comp;
        if ((double) num1 > (double) stun.MaxRange || this._stand.IsDown(args.Target) || !this.TryComp<RMCSizeComponent>(args.Target, out comp))
          break;
        this.KnockBack(args.Target, bullet.Comp.ShotFrom, stun.KnockBackPowerMin, stun.KnockBackPowerMax, stun.KnockBackSpeed);
        if (this._net.IsClient)
          break;
        double num2 = 1.0;
        if (comp.Size >= RMCSizes.Big)
          num2 = 1.2;
        else if (comp.Size <= RMCSizes.SmallXeno && this.IsXenoSized((Entity<RMCSizeComponent>) (args.Target, comp)))
          num2 = 0.7;
        this._dazed.TryDaze(args.Target, stun.DazeTime * num2);
        if (this.IsXenoSized((Entity<RMCSizeComponent>) (args.Target, comp)))
        {
          TimeSpan stunTime = stun.StunTime;
          TimeSpan superSlowTime = stun.SuperSlowTime;
          TimeSpan slowTime = stun.SlowTime;
          if (stun.LosesEffectWithRange)
          {
            stunTime -= TimeSpan.FromSeconds((double) num1 / 50.0);
            superSlowTime -= TimeSpan.FromSeconds((double) num1 / 10.0);
            slowTime -= TimeSpan.FromSeconds((double) num1 / 5.0);
          }
          if (stun.SlowsEffectBigXenos || comp.Size < RMCSizes.Big)
            this.ApplyEffects(args.Target, stunTime, slowTime, superSlowTime);
          this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-stun-shaken"), args.Target, args.Target, PopupType.MediumCaution);
        }
        else
          this._stamina.DoStaminaDamage((Entity<RMCStaminaComponent>) args.Target, (double) args.Damage.GetTotal().Float());
      }
    }
  }

  private void ApplyEffects(EntityUid uid, TimeSpan stun, TimeSpan slow, TimeSpan superSlow)
  {
    this._slow.TrySlowdown(uid, slow);
    this._slow.TrySuperSlowdown(uid, superSlow);
    RMCSizeComponent comp;
    if (!this.TryComp<RMCSizeComponent>(uid, out comp) || comp.Size >= RMCSizes.Big)
      return;
    this._stun.TryParalyze(uid, stun, true);
  }

  public void KnockBack(
    EntityUid target,
    MapCoordinates? knockedBackFrom,
    float knockBackPowerMin = 1f,
    float knockBackPowerMax = 1f,
    float knockBackSpeed = 5f,
    bool ignoreSize = false)
  {
    RMCSizeComponent comp1;
    if ((!this.TryComp<RMCSizeComponent>(target, out comp1) || comp1.Size >= RMCSizes.Big) && !ignoreSize || !knockedBackFrom.HasValue)
      return;
    PhysicsComponent comp2;
    if (this.TryComp<PhysicsComponent>(target, out comp2))
    {
      this._physics.SetLinearVelocity(target, Vector2.Zero, body: comp2);
      this._physics.SetAngularVelocity(target, 0.0f, body: comp2);
    }
    Vector2 vector2 = this._transform.GetMoverCoordinates(target).Position - knockedBackFrom.Value.Position;
    if ((double) vector2.Length() == 0.0)
      return;
    this._rmcPulling.TryStopPullsOn(target);
    float num = this._random.NextFloat(knockBackPowerMin, knockBackPowerMax);
    Vector2 direction = Vector2Helpers.Normalized(vector2) * num;
    this._throwing.TryThrow(target, direction, knockBackSpeed, compensateFriction: true, animated: false, playSound: false);
  }

  private void OnTrigger(Entity<RMCStunOnHitComponent> ent, ref RMCTriggerEvent args)
  {
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates((EntityUid) ent, this.Transform((EntityUid) ent));
    foreach (RMCStunOnHit stun in ent.Comp.Stuns)
    {
      foreach (Entity<StatusEffectsComponent> entity in this._entityLookup.GetEntitiesInRange<StatusEffectsComponent>(moverCoordinates, stun.StunArea))
      {
        if (!this._entityWhitelist.IsWhitelistFail(stun.Whitelist, (EntityUid) entity))
        {
          this.ApplyEffects((EntityUid) entity, stun.StunTime, stun.SlowTime, stun.SuperSlowTime);
          this.KnockBack((EntityUid) entity, ent.Comp.ShotFrom, stun.KnockBackPowerMin, stun.KnockBackPowerMax, stun.KnockBackSpeed);
          break;
        }
      }
    }
  }

  private void OnStunOnTrigger(Entity<RMCStunOnTriggerComponent> ent, ref RMCTriggerEvent args)
  {
    if (this._net.IsClient)
      return;
    this._marines.Clear();
    this._entityLookup.GetEntitiesInRange<MarineComponent>(ent.Owner.ToCoordinates(), ent.Comp.Range, this._marines);
    foreach (Entity<MarineComponent> marine in this._marines)
    {
      if (ent.Comp.Filters != null)
      {
        bool flag = false;
        foreach (RMCStunOnTriggerFilter filter in ent.Comp.Filters)
        {
          if (!this._entityWhitelist.IsWhitelistFail(filter.Whitelist, (EntityUid) marine))
          {
            float? nullable1 = filter.Probability;
            float probability = (float) ((double) nullable1 ?? (double) ent.Comp.Probability);
            nullable1 = filter.Range;
            float range = (float) ((double) nullable1 ?? (double) ent.Comp.Range);
            TimeSpan? nullable2 = filter.Stun;
            TimeSpan stun = nullable2 ?? ent.Comp.Stun;
            nullable2 = filter.Flash;
            TimeSpan flash = nullable2 ?? ent.Comp.Flash;
            nullable2 = filter.FlashAdditionalStunTime;
            TimeSpan flashAdditionalStunTime = nullable2 ?? ent.Comp.FlashAdditionalStunTime;
            nullable2 = filter.Paralyze;
            TimeSpan paralyze = nullable2 ?? ent.Comp.Paralyze;
            this.Stun(ent, (EntityUid) marine, args.User, probability, range, stun, flash, flashAdditionalStunTime, paralyze);
            flag = true;
            break;
          }
        }
        if (flag)
          continue;
      }
      this.Stun(ent, (EntityUid) marine, args.User, ent.Comp.Probability, ent.Comp.Range, ent.Comp.Stun, ent.Comp.Flash, ent.Comp.FlashAdditionalStunTime, ent.Comp.Paralyze);
    }
    args.Handled = true;
  }

  private void Stun(
    Entity<RMCStunOnTriggerComponent> ent,
    EntityUid target,
    EntityUid? user,
    float probability,
    float range,
    TimeSpan stun,
    TimeSpan flash,
    TimeSpan flashAdditionalStunTime,
    TimeSpan paralyze)
  {
    EntityCoordinates coordinates = this.Transform(target).Coordinates;
    if (!this._random.Prob(probability) || !this._interaction.InRangeUnobstructed((EntityUid) ent, coordinates, range))
      return;
    if (this._flash.Flash(target, user, new EntityUid?((EntityUid) ent), (float) flash.TotalMilliseconds, displayPopup: false))
    {
      stun += flashAdditionalStunTime;
      paralyze += flashAdditionalStunTime;
    }
    if (stun > TimeSpan.Zero)
    {
      this._stun.TryStun(target, stun, true);
      this._stun.TryKnockdown(target, stun, true);
    }
    if (!(paralyze > TimeSpan.Zero))
      return;
    this.TryKnockOut(target, paralyze);
  }

  public bool TryKnockOut(
    EntityUid uid,
    TimeSpan duration,
    bool refresh = true,
    StatusEffectsComponent? status = null)
  {
    return !(duration <= TimeSpan.Zero) && this.Resolve<StatusEffectsComponent>(uid, ref status, false) && this._status.TryAddStatusEffect<RMCUnconsciousComponent>(uid, (string) RMCSizeStunSystem.KnockedOut, duration, refresh);
  }

  private void OnUnconsciousStart(Entity<RMCUnconsciousComponent> ent, ref ComponentStartup args)
  {
    this.EnsureComp<StunnedComponent>((EntityUid) ent);
    this.EnsureComp<KnockedDownComponent>((EntityUid) ent);
    this.EnsureComp<TemporaryBlindnessComponent>((EntityUid) ent);
    this.EnsureComp<MutedComponent>((EntityUid) ent);
    this.EnsureComp<DeafComponent>((EntityUid) ent);
  }

  private void OnUnconsciousEnd(Entity<RMCUnconsciousComponent> ent, ref ComponentShutdown args)
  {
    TimeSpan curTime = this._timing.CurTime;
    (TimeSpan, TimeSpan)? time;
    if (!this._status.TryGetTime((EntityUid) ent, "Stun", out time) || time.Value.Item2 < curTime)
      this.RemCompDeferred<StunnedComponent>((EntityUid) ent);
    if (!this._status.TryGetTime((EntityUid) ent, "KnockedDown", out time) || time.Value.Item2 < curTime)
      this.RemCompDeferred<KnockedDownComponent>((EntityUid) ent);
    if (!this._status.TryGetTime((EntityUid) ent, "TemporaryBlindness", out time) || time.Value.Item2 < curTime)
      this.RemCompDeferred<TemporaryBlindnessComponent>((EntityUid) ent);
    if (!this._status.TryGetTime((EntityUid) ent, "Muted", out time) || time.Value.Item2 < curTime)
      this.RemCompDeferred<MutedComponent>((EntityUid) ent);
    if (this._status.TryGetTime((EntityUid) ent, "Deaf", out time) && !(time.Value.Item2 < curTime))
      return;
    this.RemCompDeferred<DeafComponent>((EntityUid) ent);
  }

  private void OnUnconsciousUpdate(
    Entity<RMCUnconsciousComponent> ent,
    ref StatusEffectEndedEvent args)
  {
    if (!this.IsKnockedOut((EntityUid) ent))
      return;
    this.EnsureComp<StunnedComponent>((EntityUid) ent);
    this.EnsureComp<KnockedDownComponent>((EntityUid) ent);
    this.EnsureComp<TemporaryBlindnessComponent>((EntityUid) ent);
    this.EnsureComp<MutedComponent>((EntityUid) ent);
    this.EnsureComp<DeafComponent>((EntityUid) ent);
  }

  private void OnUnconsciousPointAttempt(
    Entity<RMCUnconsciousComponent> ent,
    ref PointAttemptEvent args)
  {
    if (!this.IsKnockedOut((EntityUid) ent))
      return;
    args.Cancel();
  }

  private void OnKnockOutCollideProjectileHit(
    Entity<RMCKnockOutOnCollideComponent> ent,
    ref ProjectileHitEvent args)
  {
    this.TryKnockOut(args.Target, ent.Comp.ParalyzeTime);
  }

  private void OnKnockOutCollideThrowHit(
    Entity<RMCKnockOutOnCollideComponent> ent,
    ref ThrowDoHitEvent args)
  {
    this.TryKnockOut(args.Target, ent.Comp.ParalyzeTime);
  }

  public bool IsKnockedOut(EntityUid uid)
  {
    return this._status.HasStatusEffect(uid, (string) RMCSizeStunSystem.KnockedOut);
  }
}
