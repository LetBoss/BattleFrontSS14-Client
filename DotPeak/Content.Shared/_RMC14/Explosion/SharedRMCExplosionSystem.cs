// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Explosion.SharedRMCExplosionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.BlurredVision;
using Content.Shared._RMC14.Deafness;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Construction.Nest;
using Content.Shared.Body.Systems;
using Content.Shared.Coordinates;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Explosion;
using Content.Shared.FixedPoint;
using Content.Shared.Flash.Components;
using Content.Shared.Inventory;
using Content.Shared.Standing;
using Content.Shared.StatusEffect;
using Content.Shared.Sticky.Components;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Explosion;

public abstract class SharedRMCExplosionSystem : EntitySystem
{
  [Dependency]
  private SharedBodySystem _body;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private EntityWhitelistSystem _entityWhitelist;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private RMCSizeStunSystem _sizeStun;
  [Dependency]
  private StandingStateSystem _standing;
  [Dependency]
  private SharedStunSystem _stun;
  [Dependency]
  private ThrowingSystem _throwing;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private RMCSlowSystem _slow;
  [Dependency]
  private RMCDazedSystem _dazed;
  [Dependency]
  private StatusEffectsSystem _statusEffects;
  [Dependency]
  private SharedDeafnessSystem _deafness;
  [Dependency]
  private INetManager _net;
  private static readonly ProtoId<DamageTypePrototype> StructuralDamage = (ProtoId<DamageTypePrototype>) "Structural";
  private static readonly ProtoId<StatusEffectPrototype> FlashedKey = (ProtoId<StatusEffectPrototype>) "Flashed";
  private static readonly ProtoId<StatusEffectPrototype> BlindKey = (ProtoId<StatusEffectPrototype>) "Blinded";
  private readonly HashSet<Entity<RMCWallExplosionDeletableComponent>> _walls = new HashSet<Entity<RMCWallExplosionDeletableComponent>>();

  public override void Initialize()
  {
    this.SubscribeLocalEvent<CMExplosionEffectComponent, CMExplosiveTriggeredEvent>(new EntityEventRefHandler<CMExplosionEffectComponent, CMExplosiveTriggeredEvent>(this.OnExplosionEffectTriggered));
    this.SubscribeLocalEvent<RMCExplosiveDeleteComponent, CMExplosiveTriggeredEvent>(new EntityEventRefHandler<RMCExplosiveDeleteComponent, CMExplosiveTriggeredEvent>(this.OnDeleteWallsTriggered));
    this.SubscribeLocalEvent<ExplosionRandomResistanceComponent, GetExplosionResistanceEvent>(new EntityEventRefHandler<ExplosionRandomResistanceComponent, GetExplosionResistanceEvent>(this.OnExplosionRandomResistanceGet));
    this.SubscribeLocalEvent<StunOnExplosionReceivedComponent, ExplosionReceivedEvent>(new EntityEventRefHandler<StunOnExplosionReceivedComponent, ExplosionReceivedEvent>(this.OnStunOnExplosionReceivedBeforeExplode));
    this.SubscribeLocalEvent<DestroyedByExplosionTypeComponent, ExplosionReceivedEvent>(new EntityEventRefHandler<DestroyedByExplosionTypeComponent, ExplosionReceivedEvent>(this.OnDestroyedByExplosionReceived));
    this.SubscribeLocalEvent<MobGibbedByExplosionTypeComponent, ExplosionReceivedEvent>(new EntityEventRefHandler<MobGibbedByExplosionTypeComponent, ExplosionReceivedEvent>(this.OnMobGibbedByExplosionReceived));
  }

  private void OnExplosionEffectTriggered(
    Entity<CMExplosionEffectComponent> ent,
    ref CMExplosiveTriggeredEvent args)
  {
    this.DoEffect(ent);
  }

  private void OnDeleteWallsTriggered(
    Entity<RMCExplosiveDeleteComponent> ent,
    ref CMExplosiveTriggeredEvent args)
  {
    this._walls.Clear();
    this._entityLookup.GetEntitiesInRange<RMCWallExplosionDeletableComponent>(ent.Owner.ToCoordinates(), (float) ent.Comp.Range, this._walls);
    foreach (Entity<RMCWallExplosionDeletableComponent> wall in this._walls)
      this.QueueDel(new EntityUid?((EntityUid) wall));
    if (ent.Comp.Whitelist == null || !this.HasComp<StickyComponent>((EntityUid) ent))
      return;
    EntityUid parentUid = this.Transform((EntityUid) ent).ParentUid;
    if (!parentUid.Valid || !this._entityWhitelist.IsWhitelistPass(ent.Comp.Whitelist, parentUid))
      return;
    this.QueueDel(new EntityUid?(parentUid));
  }

  private void OnExplosionRandomResistanceGet(
    Entity<ExplosionRandomResistanceComponent> ent,
    ref GetExplosionResistanceEvent args)
  {
    float num = this._random.NextFloat(ent.Comp.Min.Float(), ent.Comp.Max.Float());
    args.DamageCoefficient *= num;
  }

  public void ChangeExplosionStunResistance(
    EntityUid ent,
    StunOnExplosionReceivedComponent? comp,
    bool isStunnable)
  {
    if (!this.Resolve<StunOnExplosionReceivedComponent>(ent, ref comp, false))
      return;
    comp.Weak = isStunnable;
  }

  private void OnStunOnExplosionReceivedBeforeExplode(
    Entity<StunOnExplosionReceivedComponent> ent,
    ref ExplosionReceivedEvent args)
  {
    FixedPoint2 total = args.Damage.GetTotal();
    double num1 = Math.Min(20.0, Math.Round(total.Double() * 0.05) / 2.0);
    if (this._standing.IsDown((EntityUid) ent))
      num1 *= 0.5;
    RMCSizes size;
    this._sizeStun.TryGetSize((EntityUid) ent, out size);
    Vector2 vector2 = this._transform.GetWorldPosition((EntityUid) ent) - args.Epicenter.Position;
    if (size == RMCSizes.Humanoid)
    {
      CMGetArmorEvent args1 = new CMGetArmorEvent(SlotFlags.OUTERCLOTHING | SlotFlags.INNERCLOTHING);
      this.RaiseLocalEvent<CMGetArmorEvent>((EntityUid) ent, ref args1);
      double num2 = (double) (100 - args1.ExplosionArmor) * 0.01;
      double knockBackSpeed = num1 * 5.0;
      this._statusEffects.TryAddStatusEffect<FlashedComponent>((EntityUid) ent, (string) SharedRMCExplosionSystem.FlashedKey, ent.Comp.BlindTime * num2, true);
      this._deafness.TryDeafen((EntityUid) ent, TimeSpan.FromSeconds(knockBackSpeed * 0.5), true);
      float num3 = (float) Math.Clamp(knockBackSpeed / 5.0 / (double) vector2.Length(), 0.5, Math.Max(knockBackSpeed / 10.0, 0.5));
      if (!this.HasComp<XenoNestedComponent>((EntityUid) ent))
        this._sizeStun.KnockBack((EntityUid) ent, new MapCoordinates?(args.Epicenter), num3, num3, (float) knockBackSpeed);
      double num4 = knockBackSpeed * 0.1;
      double num5 = total.Double() * 0.1;
      double num6 = num2;
      TimeSpan time = TimeSpan.FromSeconds(Math.Max(num4 * num6, 1.0));
      this._stun.TryParalyze((EntityUid) ent, time, false);
      double val1 = Math.Max(num5 * num2 * 0.5, 0.5);
      if (args.Explosion == (ProtoId<ExplosionPrototype>) "GrenadeLauncher")
        val1 *= 0.2;
      TimeSpan duration = TimeSpan.FromSeconds(Math.Min(val1, 5.0));
      this._sizeStun.TryKnockOut((EntityUid) ent, duration, false);
      this._dazed.TryDaze((EntityUid) ent, duration * 2.0, stutter: true);
      this._statusEffects.TryAddStatusEffect<RMCBlindedComponent>((EntityUid) ent, (string) SharedRMCExplosionSystem.BlindKey, ent.Comp.BlurTime, false);
    }
    else if (num1 > 0.0 && ent.Comp.Weak)
    {
      TimeSpan time = TimeSpan.FromSeconds(num1 / 2.5);
      this._stun.TryStun((EntityUid) ent, time, true);
      this._stun.TryKnockdown((EntityUid) ent, time, true);
      if (size < RMCSizes.Big)
      {
        this._slow.TrySlowdown((EntityUid) ent, TimeSpan.FromSeconds(num1));
        this._slow.TrySuperSlowdown((EntityUid) ent, TimeSpan.FromSeconds(num1 / 2.0));
      }
      else
        this._slow.TrySlowdown((EntityUid) ent, TimeSpan.FromSeconds(num1 / 3.0));
      this._sizeStun.KnockBack((EntityUid) ent, new MapCoordinates?(args.Epicenter), ignoreSize: true);
    }
    else
    {
      if (num1 <= 10.0)
        return;
      double num7 = num1 / 5.0;
      TimeSpan time = TimeSpan.FromSeconds(num7 / 5.0);
      this._stun.TryStun((EntityUid) ent, time, true);
      this._stun.TryKnockdown((EntityUid) ent, time, true);
      if (size < RMCSizes.Big)
      {
        this._slow.TrySlowdown((EntityUid) ent, TimeSpan.FromSeconds(num7));
        this._slow.TrySuperSlowdown((EntityUid) ent, TimeSpan.FromSeconds(num7 / 2.0));
      }
      else
        this._slow.TrySlowdown((EntityUid) ent, TimeSpan.FromSeconds(num7 / 3.0));
    }
  }

  private void OnDestroyedByExplosionReceived(
    Entity<DestroyedByExplosionTypeComponent> ent,
    ref ExplosionReceivedEvent args)
  {
    if (args.Explosion != ent.Comp.Explosion || args.Damage.GetTotal() < ent.Comp.Threshold || this.TerminatingOrDeleted((EntityUid) ent))
      return;
    this.QueueDel(new EntityUid?((EntityUid) ent));
  }

  private void OnMobGibbedByExplosionReceived(
    Entity<MobGibbedByExplosionTypeComponent> ent,
    ref ExplosionReceivedEvent args)
  {
    if (Array.IndexOf<ProtoId<ExplosionPrototype>>(ent.Comp.Explosions, args.Explosion) == -1)
      return;
    FixedPoint2 zero = FixedPoint2.Zero;
    foreach ((string key, FixedPoint2 fixedPoint2) in args.Damage.DamageDict)
    {
      if (!((ProtoId<DamageTypePrototype>) key == SharedRMCExplosionSystem.StructuralDamage))
        zero += fixedPoint2;
    }
    if (zero < ent.Comp.Threshold || this._net.IsClient || this.TerminatingOrDeleted((EntityUid) ent))
      return;
    this._body.GibBody((EntityUid) ent, splatCone: new Angle());
  }

  public void DoEffect(Entity<CMExplosionEffectComponent> ent)
  {
    EntProtoId? shockWave = ent.Comp.ShockWave;
    if (shockWave.HasValue)
      this.SpawnNextToOrDrop((string) shockWave.GetValueOrDefault(), (EntityUid) ent);
    EntProtoId? explosion = ent.Comp.Explosion;
    if (explosion.HasValue)
      this.SpawnNextToOrDrop((string) explosion.GetValueOrDefault(), (EntityUid) ent);
    if (ent.Comp.MaxShrapnel <= 0)
      return;
    foreach (EntProtoId shrapnelEffect in ent.Comp.ShrapnelEffects)
    {
      int num = this._random.Next(ent.Comp.MinShrapnel, ent.Comp.MaxShrapnel);
      for (int index = 0; index < num; ++index)
      {
        Angle angle = this._random.NextAngle();
        Vector2 direction = Vector2Helpers.Normalized(((Angle) ref angle).ToVec()) * 10f;
        this._throwing.TryThrow(this.SpawnNextToOrDrop((string) shrapnelEffect, (EntityUid) ent), direction, ent.Comp.ShrapnelSpeed / 10f);
      }
    }
  }

  public void TryDoEffect(Entity<CMExplosionEffectComponent?> ent)
  {
    if (!this.Resolve<CMExplosionEffectComponent>((EntityUid) ent, ref ent.Comp, false))
      return;
    this.DoEffect((Entity<CMExplosionEffectComponent>) ((EntityUid) ent, ent.Comp));
  }

  public virtual void QueueExplosion(
    MapCoordinates epicenter,
    string typeId,
    float totalIntensity,
    float slope,
    float maxTileIntensity,
    EntityUid? cause,
    float tileBreakScale = 1f,
    int maxTileBreak = 2147483647 /*0x7FFFFFFF*/,
    bool canCreateVacuum = true,
    bool addLog = true,
    float? vehicleLightDamage = null,
    float? vehicleHeavyDamage = null)
  {
  }

  public virtual void TriggerExplosive(
    EntityUid uid,
    bool delete = true,
    float? totalIntensity = null,
    float? radius = null,
    EntityUid? user = null)
  {
  }
}
