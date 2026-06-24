// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Tumble.XenoTumbleSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Damage.ObstacleSlamming;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared.Damage;
using Content.Shared.Mobs.Systems;
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
using Robust.Shared.Timing;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Tumble;

public sealed class XenoTumbleSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private SharedXenoHiveSystem _hive;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private RMCObstacleSlammingSystem _rmcObstacleSlamming;
  [Dependency]
  private RMCPullingSystem _rmcPulling;
  [Dependency]
  private RMCSizeStunSystem _sizeStun;
  [Dependency]
  private SharedStunSystem _stun;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private ThrowingSystem _throwing;
  [Dependency]
  private ThrownItemSystem _thrownItem;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private XenoSystem _xeno;
  private Robust.Shared.GameObjects.EntityQuery<PhysicsComponent> _physicsQuery;
  private Robust.Shared.GameObjects.EntityQuery<ThrownItemComponent> _thrownItemQuery;

  public override void Initialize()
  {
    this._physicsQuery = this.GetEntityQuery<PhysicsComponent>();
    this._thrownItemQuery = this.GetEntityQuery<ThrownItemComponent>();
    this.SubscribeLocalEvent<XenoTumbleComponent, XenoTumbleActionEvent>(new EntityEventRefHandler<XenoTumbleComponent, XenoTumbleActionEvent>(this.OnXenoTumbleAction));
    this.SubscribeLocalEvent<XenoTumbleComponent, ThrowDoHitEvent>(new EntityEventRefHandler<XenoTumbleComponent, ThrowDoHitEvent>(this.OnXenoTumbleHit));
    this.SubscribeLocalEvent<XenoTumbleComponent, LandEvent>(new EntityEventRefHandler<XenoTumbleComponent, LandEvent>(this.OnXenoTumbleLand));
  }

  private void OnXenoTumbleAction(Entity<XenoTumbleComponent> xeno, ref XenoTumbleActionEvent args)
  {
    if (args.Handled)
      return;
    MapCoordinates mapCoordinates = this._transform.GetMapCoordinates((EntityUid) xeno);
    Direction dir = DirectionExtensions.GetDir(Vector2Helpers.Normalized(this._transform.ToMapCoordinates(args.Target).Position - mapCoordinates.Position) * xeno.Comp.Range);
    Angle worldRotation = this._transform.GetWorldRotation((EntityUid) xeno);
    (Direction First, Direction Second) perpendiculars = ((Angle) ref worldRotation).GetCardinalDir().GetPerpendiculars();
    Direction direction1;
    if (dir == perpendiculars.First)
      direction1 = perpendiculars.First;
    else if (dir == perpendiculars.Second)
    {
      direction1 = perpendiculars.Second;
    }
    else
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-tumble-not-perpendicular"), args.Target, new EntityUid?((EntityUid) xeno), PopupType.LargeCaution);
      return;
    }
    Vector2 direction2 = DirectionExtensions.ToVec(direction1) * xeno.Comp.Range;
    args.Handled = true;
    this._rmcPulling.TryStopAllPullsFromAndOn((EntityUid) xeno);
    xeno.Comp.Target = new Vector2?(direction2);
    this.Dirty<XenoTumbleComponent>(xeno);
    this._rmcObstacleSlamming.MakeImmune((EntityUid) xeno);
    this._throwing.TryThrow((EntityUid) xeno, direction2, 30f, animated: false);
    this._audio.PlayPredicted(xeno.Comp.Sound, (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
  }

  private void OnXenoTumbleHit(Entity<XenoTumbleComponent> xeno, ref ThrowDoHitEvent args)
  {
    if (!this._mobState.IsAlive((EntityUid) xeno) || this.HasComp<StunnedComponent>((EntityUid) xeno))
    {
      xeno.Comp.Target = new Vector2?();
      this.Dirty<XenoTumbleComponent>(xeno);
    }
    else
    {
      if (this._mobState.IsDead(args.Target))
        return;
      PhysicsComponent component1;
      ThrownItemComponent component2;
      if (this._physicsQuery.TryGetComponent((EntityUid) xeno, out component1) && this._thrownItemQuery.TryGetComponent((EntityUid) xeno, out component2))
      {
        this._thrownItem.LandComponent((EntityUid) xeno, component2, component1, true);
        this._thrownItem.StopThrow((EntityUid) xeno, component2);
      }
      if (this._timing.IsFirstTimePredicted && xeno.Comp.Target.HasValue)
        xeno.Comp.Target = new Vector2?();
      if (this._hive.FromSameHive((Entity<HiveMemberComponent>) xeno.Owner, (Entity<HiveMemberComponent>) args.Target))
        return;
      if (this._net.IsServer)
        this._stun.TryParalyze(args.Target, xeno.Comp.StunTime, true);
      this.StopTumble((EntityUid) xeno);
      MapCoordinates mapCoordinates = this._transform.GetMapCoordinates((EntityUid) xeno);
      this._sizeStun.KnockBack(args.Target, new MapCoordinates?(mapCoordinates), xeno.Comp.ImpactRange, xeno.Comp.ImpactRange, 10f);
      this._damageable.TryChangeDamage(new EntityUid?(args.Target), this._xeno.TryApplyXenoSlashDamageMultiplier(args.Target, xeno.Comp.Damage), origin: new EntityUid?((EntityUid) xeno), tool: new EntityUid?((EntityUid) xeno), armorPiercing: xeno.Comp.ArmorPiercing);
    }
  }

  private void OnXenoTumbleLand(Entity<XenoTumbleComponent> xeno, ref LandEvent args)
  {
    if (!xeno.Comp.Target.HasValue)
      return;
    xeno.Comp.Target = new Vector2?();
    this.Dirty<XenoTumbleComponent>(xeno);
  }

  private void StopTumble(EntityUid xeno)
  {
    PhysicsComponent component;
    if (!this._physicsQuery.TryGetComponent(xeno, out component))
      return;
    this._physics.SetLinearVelocity(xeno, Vector2.Zero, body: component);
    this._physics.SetBodyStatus(xeno, component, BodyStatus.OnGround);
  }
}
