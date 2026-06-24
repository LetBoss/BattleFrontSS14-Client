// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Damage.ObstacleSlamming.RMCObstacleSlammingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stun;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.IdentityManagement;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Damage.ObstacleSlamming;

public sealed class RMCObstacleSlammingSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedStunSystem _stun;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedColorFlashEffectSystem _color;
  [Dependency]
  private RMCSizeStunSystem _size;
  [Dependency]
  private RMCSlowSystem _slow;
  private static readonly ProtoId<DamageTypePrototype> SlamDamageType = (ProtoId<DamageTypePrototype>) "Blunt";
  private readonly HashSet<EntityUid> _queuedImmuneEntities = new HashSet<EntityUid>();
  private readonly HashSet<EntityUid> _queuedBonusEntities = new HashSet<EntityUid>();

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RMCObstacleSlammingComponent, ThrowDoHitEvent>(new EntityEventRefHandler<RMCObstacleSlammingComponent, ThrowDoHitEvent>(this.HandleCollide));
  }

  private void HandleCollide(Entity<RMCObstacleSlammingComponent> ent, ref ThrowDoHitEvent args)
  {
    EntityUid thrown = args.Thrown;
    EntityUid target = args.Target;
    PhysicsComponent comp1;
    PhysicsComponent comp2;
    RMCSizes size;
    if (args.Handled || thrown != ent.Owner || this.HasComp<RMCObstacleSlamImmuneComponent>(thrown) || this.HasComp<RMCObstacleSlamCauserImmuneComponent>(target) || !this.TryComp<PhysicsComponent>(thrown, out comp1) || !this.TryComp<PhysicsComponent>(target, out comp2) || !comp1.Hard || !comp2.Hard || !this._size.TryGetSize(thrown, out size) || !this.HasComp<DamageableComponent>(thrown))
      return;
    float num1 = comp1.LinearVelocity.Length();
    if (ent.Comp.LastHit.HasValue && this._timing.CurTime - ent.Comp.LastHit.Value < ent.Comp.DamageCooldown)
      return;
    ent.Comp.LastHit = new TimeSpan?(this._timing.CurTime);
    float num2 = (float) (1.0 + (double) ent.Comp.MobSizeCoefficient / (double) (size + (byte) 1)) * ent.Comp.ThrowSpeedCoefficient * num1;
    DamageSpecifier damage = new DamageSpecifier()
    {
      DamageDict = {
        [(string) RMCObstacleSlammingSystem.SlamDamageType] = (FixedPoint2) num2
      }
    };
    this._damageable.TryChangeDamage(new EntityUid?(thrown), damage);
    SharedColorFlashEffectSystem color = this._color;
    Color red = Color.Red;
    List<EntityUid> entities = new List<EntityUid>();
    entities.Add(thrown);
    Filter filter = Filter.Pvs(thrown);
    color.RaiseEffect(red, entities, filter);
    RMCObstacleSlamBonusEffectsComponent comp3;
    if (this.TryComp<RMCObstacleSlamBonusEffectsComponent>(thrown, out comp3))
    {
      this._slow.TrySlowdown(thrown, comp3.Slow, false);
      this._stun.TryParalyze(thrown, comp3.Stun, false);
    }
    this._physics.SetLinearVelocity((EntityUid) ent, Vector2.Zero);
    this._physics.SetAngularVelocity((EntityUid) ent, 0.0f);
    if ((double) (this._transform.GetMoverCoordinates(thrown).Position - this._transform.GetMoverCoordinates(target).Position).Length() != 0.0)
      this._size.KnockBack(thrown, new MapCoordinates?(this._transform.GetMapCoordinates(target)), ent.Comp.KnockbackPower, ent.Comp.KnockbackPower, ent.Comp.KnockBackSpeed);
    if (this._timing.IsFirstTimePredicted)
      this._audio.PlayPvs(ent.Comp.SoundHit, target);
    if (this._net.IsServer)
    {
      EntProtoId? hitEffect = ent.Comp.HitEffect;
      this.SpawnAttachedTo(hitEffect.HasValue ? (string) hitEffect.GetValueOrDefault() : (string) null, thrown.ToCoordinates(), rotation: new Angle());
    }
    this._popup.PopupClient(this.Loc.GetString("rmc-obstacle-slam-self", (nameof (ent), (object) thrown), ("object", (object) Identity.Name(target, (IEntityManager) this.EntityManager, new EntityUid?(thrown)))), thrown, new EntityUid?(thrown), PopupType.MediumCaution);
    foreach (ICommonSession recipient in Filter.PvsExcept(thrown).Recipients)
    {
      EntityUid? attachedEntity = recipient.AttachedEntity;
      if (attachedEntity.HasValue)
      {
        EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
        this._popup.PopupEntity(this.Loc.GetString("rmc-obstacle-slam-others", (nameof (ent), (object) thrown), ("object", (object) Identity.Name(target, (IEntityManager) this.EntityManager, new EntityUid?(valueOrDefault)))), thrown, valueOrDefault, PopupType.MediumCaution);
      }
    }
    args.Handled = true;
  }

  public void MakeImmune(EntityUid uid, float immuneDuration = 2f)
  {
    RMCObstacleSlamImmuneComponent slamImmuneComponent = this.EnsureComp<RMCObstacleSlamImmuneComponent>(uid);
    slamImmuneComponent.ExpireIn = TimeSpan.FromSeconds((double) immuneDuration);
    slamImmuneComponent.ExpireAt = new TimeSpan?(this._timing.CurTime + slamImmuneComponent.ExpireIn);
    this.Dirty(uid, (IComponent) slamImmuneComponent);
  }

  public void ApplyBonuses(EntityUid uid, TimeSpan stun, TimeSpan slow)
  {
    RMCObstacleSlamBonusEffectsComponent effectsComponent = this.EnsureComp<RMCObstacleSlamBonusEffectsComponent>(uid);
    effectsComponent.ExpireAt = new TimeSpan?(this._timing.CurTime + effectsComponent.ExpireIn);
    effectsComponent.Stun = stun;
    effectsComponent.Slow = slow;
    this.Dirty(uid, (IComponent) effectsComponent);
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    this._queuedImmuneEntities.Clear();
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCObstacleSlamImmuneComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<RMCObstacleSlamImmuneComponent>();
    EntityUid uid1;
    RMCObstacleSlamImmuneComponent comp1_1;
    while (entityQueryEnumerator1.MoveNext(out uid1, out comp1_1))
    {
      if (!comp1_1.ExpireAt.HasValue || !(comp1_1.ExpireAt.Value > this._timing.CurTime))
        this._queuedImmuneEntities.Add(uid1);
    }
    foreach (EntityUid queuedImmuneEntity in this._queuedImmuneEntities)
      this.RemComp<RMCObstacleSlamImmuneComponent>(queuedImmuneEntity);
    this._queuedBonusEntities.Clear();
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCObstacleSlamBonusEffectsComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<RMCObstacleSlamBonusEffectsComponent>();
    EntityUid uid2;
    RMCObstacleSlamBonusEffectsComponent comp1_2;
    while (entityQueryEnumerator2.MoveNext(out uid2, out comp1_2))
    {
      if (!comp1_2.ExpireAt.HasValue || !(comp1_2.ExpireAt.Value > this._timing.CurTime))
        this._queuedBonusEntities.Add(uid2);
    }
    foreach (EntityUid queuedBonusEntity in this._queuedBonusEntities)
      this.RemComp<RMCObstacleSlamBonusEffectsComponent>(queuedBonusEntity);
  }
}
