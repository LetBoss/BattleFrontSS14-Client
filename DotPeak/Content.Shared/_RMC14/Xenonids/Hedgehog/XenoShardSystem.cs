// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Hedgehog.XenoShardSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Aura;
using Content.Shared._RMC14.Explosion;
using Content.Shared._RMC14.Shields;
using Content.Shared._RMC14.Xenonids.Energy;
using Content.Shared._RMC14.Xenonids.Projectile;
using Content.Shared._RMC14.Xenonids.Sweep;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Hedgehog;

public sealed class XenoShardSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeed;
  [Dependency]
  private CMArmorSystem _armor;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedAuraSystem _aura;
  [Dependency]
  private XenoProjectileSystem _xenoProjectile;
  [Dependency]
  private XenoShieldSystem _shield;
  [Dependency]
  private XenoEnergySystem _energy;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedRMCExplosionSystem _explosion;
  [Dependency]
  private INetManager _net;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoShardComponent, DamageChangedEvent>(new EntityEventRefHandler<XenoShardComponent, DamageChangedEvent>(this.OnShardHitBy));
    this.SubscribeLocalEvent<XenoShardComponent, CMGetArmorEvent>(new EntityEventRefHandler<XenoShardComponent, CMGetArmorEvent>(this.OnShardGetArmor));
    this.SubscribeLocalEvent<XenoShardComponent, MapInitEvent>(new EntityEventRefHandler<XenoShardComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<XenoShardComponent, XenoEnergyChangedEvent>(new EntityEventRefHandler<XenoShardComponent, XenoEnergyChangedEvent>(this.OnShardLevelChanged));
    this.SubscribeLocalEvent<XenoSpikeShedComponent, ActionXenoSpikeShedEvent>(new EntityEventRefHandler<XenoSpikeShedComponent, ActionXenoSpikeShedEvent>(this.OnSpikeShed));
    this.SubscribeLocalEvent<XenoSpikeShieldComponent, ActionXenoSpikeShieldEvent>(new EntityEventRefHandler<XenoSpikeShieldComponent, ActionXenoSpikeShieldEvent>(this.OnSpikeShield));
    this.SubscribeLocalEvent<XenoShardComponent, RefreshMovementSpeedModifiersEvent>(new EntityEventRefHandler<XenoShardComponent, RefreshMovementSpeedModifiersEvent>(this.OnRefreshMovementSpeed));
    this.SubscribeLocalEvent<XenoShardComponent, XenoEnergyGainAttemptEvent>(new EntityEventRefHandler<XenoShardComponent, XenoEnergyGainAttemptEvent>(this.OnSpikeEnergyGain));
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoShardComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoShardComponent>();
    EntityUid uid;
    XenoShardComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (comp1.SpikeShedCooldownEnd.HasValue)
      {
        TimeSpan timeSpan = curTime;
        TimeSpan? spikeShedCooldownEnd = comp1.SpikeShedCooldownEnd;
        if ((spikeShedCooldownEnd.HasValue ? (timeSpan >= spikeShedCooldownEnd.GetValueOrDefault() ? 1 : 0) : 0) != 0)
        {
          comp1.SpikeShedCooldownEnd = new TimeSpan?();
          this._popup.PopupEntity(this.Loc.GetString("rmc-shed-spikes-back"), uid, uid, PopupType.Medium);
          this.Dirty(uid, (IComponent) comp1);
        }
      }
    }
  }

  private void OnShardHitBy(Entity<XenoShardComponent> ent, ref DamageChangedEvent args)
  {
    XenoEnergyComponent comp;
    if (args.Damageable.Damage == null || args.Damageable.Damage.GetTotal() <= FixedPoint2.Zero || !this.HasComp<ProjectileComponent>(args.Tool) || !this.TryComp<XenoEnergyComponent>((EntityUid) ent, out comp))
      return;
    this._energy.AddEnergy((Entity<XenoEnergyComponent>) (ent.Owner, comp), ent.Comp.ShardsOnDamage);
  }

  private void OnShardLevelChanged(Entity<XenoShardComponent> ent, ref XenoEnergyChangedEvent args)
  {
    this.UpdateSpikes(ent, args.NewEnergy);
  }

  private void UpdateSpikes(Entity<XenoShardComponent> ent, FixedPoint2 shards)
  {
    StunOnExplosionReceivedComponent comp;
    if (this.TryComp<StunOnExplosionReceivedComponent>((EntityUid) ent, out comp))
    {
      if (shards >= 50)
        this._explosion.ChangeExplosionStunResistance((EntityUid) ent, comp, false);
      else
        this._explosion.ChangeExplosionStunResistance((EntityUid) ent, comp, true);
      this.Dirty<XenoShardComponent>(ent);
    }
    this._armor.UpdateArmorValue((Entity<CMArmorComponent>) ent.Owner);
    this.UpdateHedgehogSprite(ent);
  }

  private void OnShardGetArmor(Entity<XenoShardComponent> ent, ref CMGetArmorEvent args)
  {
    XenoEnergyComponent comp;
    if (ent.Comp.ShardsPerArmorBonus <= 0 || !this.TryComp<XenoEnergyComponent>((EntityUid) ent, out comp))
      return;
    float num = (float) (comp.Current / ent.Comp.ShardsPerArmorBonus) * ent.Comp.ArmorPerShard;
    args.XenoArmor += (int) num;
  }

  private void OnSpikeShed(Entity<XenoSpikeShedComponent> ent, ref ActionXenoSpikeShedEvent args)
  {
    XenoShardComponent comp1;
    XenoEnergyComponent comp2;
    if (!this.TryComp<XenoShardComponent>((EntityUid) ent, out comp1) || !this.TryComp<XenoEnergyComponent>((EntityUid) ent, out comp2) || !this._energy.HasEnergyPopup((Entity<XenoEnergyComponent>) ((EntityUid) ent, comp2), ent.Comp.MinShards))
      return;
    this._energy.RemoveEnergy((Entity<XenoEnergyComponent>) ((EntityUid) ent, comp2), comp2.Current);
    comp1.SpikeShedCooldownEnd = new TimeSpan?(this._timing.CurTime + TimeSpan.FromSeconds(30L));
    this.Dirty((EntityUid) ent, (IComponent) comp1);
    this._armor.UpdateArmorValue((Entity<CMArmorComponent>) ent.Owner);
    this.UpdateHedgehogSprite((Entity<XenoShardComponent>) (ent.Owner, comp1));
    if (this._net.IsServer)
      this._popup.PopupEntity(this.Loc.GetString("rmc-shed-spikes"), (EntityUid) ent, (EntityUid) ent);
    this._audio.PlayPredicted(ent.Comp.Sound, (EntityUid) ent, new EntityUid?((EntityUid) ent));
    this.EnsureComp<XenoSweepingComponent>((EntityUid) ent);
    XenoProjectileSystem xenoProjectile = this._xenoProjectile;
    EntityUid owner = ent.Owner;
    EntityCoordinates targetCoords = new EntityCoordinates((EntityUid) ent, Vector2.UnitX * ent.Comp.ShedRadius);
    FixedPoint2 zero = FixedPoint2.Zero;
    EntProtoId projectile = ent.Comp.Projectile;
    int projectileCount = ent.Comp.ProjectileCount;
    Angle deviation = new Angle(2.0 * Math.PI);
    int? projectileHitLimit1 = ent.Comp.ProjectileHitLimit;
    float? stopAtDistance = new float?();
    EntityUid? target = new EntityUid?();
    int? projectileHitLimit2 = projectileHitLimit1;
    xenoProjectile.TryShoot(owner, targetCoords, zero, projectile, (SoundSpecifier) null, projectileCount, deviation, 20f, stopAtDistance, target, projectileHitLimit: projectileHitLimit2);
    this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) ent);
    args.Handled = true;
  }

  private void OnSpikeShield(
    Entity<XenoSpikeShieldComponent> ent,
    ref ActionXenoSpikeShieldEvent args)
  {
    if (!this.TryComp<XenoShardComponent>((EntityUid) ent, out XenoShardComponent _) || !this._energy.TryRemoveEnergyPopup((Entity<XenoEnergyComponent>) ent.Owner, ent.Comp.ShardCost))
      return;
    ent.Comp.ShieldExpireAt = new TimeSpan?(this._timing.CurTime + ent.Comp.ShieldDuration);
    this.Dirty<XenoSpikeShieldComponent>(ent);
    this._shield.ApplyShield((EntityUid) ent, XenoShieldSystem.ShieldType.Hedgehog, ent.Comp.ShieldAmount);
    this._popup.PopupPredicted(this.Loc.GetString("rmc-spike-shield-self"), this.Loc.GetString("rmc-spike-shield-others", ("user", (object) ent)), (EntityUid) ent, new EntityUid?((EntityUid) ent));
    this._aura.GiveAura((EntityUid) ent, Color.Blue, new TimeSpan?(ent.Comp.ShieldDuration));
    this.Spawn((string) ent.Comp.EffectId, ent.Owner.ToCoordinates());
    args.Handled = true;
  }

  private void UpdateHedgehogSprite(Entity<XenoShardComponent> ent)
  {
    XenoEnergyComponent comp;
    if (!this.TryComp<XenoEnergyComponent>((EntityUid) ent, out comp))
      return;
    int current = comp.Current;
    XenoShardLevel xenoShardLevel = current >= 150 ? (current < 300 ? XenoShardLevel.Level3 : XenoShardLevel.Level4) : (current < 50 ? XenoShardLevel.Level1 : XenoShardLevel.Level2);
    this._appearance.SetData((EntityUid) ent, (Enum) XenoShardVisuals.Level, (object) xenoShardLevel);
  }

  private void OnMapInit(Entity<XenoShardComponent> ent, ref MapInitEvent args)
  {
    this.UpdateHedgehogSprite(ent);
  }

  private void OnRefreshMovementSpeed(
    Entity<XenoShardComponent> ent,
    ref RefreshMovementSpeedModifiersEvent args)
  {
    if (!ent.Comp.SpikeShedCooldownEnd.HasValue)
      return;
    args.ModifySpeed(1f + ent.Comp.SpeedModifier, 1f + ent.Comp.SpeedModifier);
  }

  private void OnSpikeEnergyGain(
    Entity<XenoShardComponent> ent,
    ref XenoEnergyGainAttemptEvent args)
  {
    if (!ent.Comp.SpikeShedCooldownEnd.HasValue)
      return;
    args.Cancel();
  }
}
