// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Melee.MeleeThrowOnHitSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Content.Shared.Timing;
using Content.Shared.Weapons.Melee.Components;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared.Weapons.Melee;

public sealed class MeleeThrowOnHitSystem : EntitySystem
{
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private UseDelaySystem _delay;
  [Dependency]
  private SharedStunSystem _stun;
  [Dependency]
  private ThrowingSystem _throwing;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<MeleeThrowOnHitComponent, MeleeHitEvent>(new EntityEventRefHandler<MeleeThrowOnHitComponent, MeleeHitEvent>(this.OnMeleeHit));
    this.SubscribeLocalEvent<MeleeThrowOnHitComponent, ThrowDoHitEvent>(new EntityEventRefHandler<MeleeThrowOnHitComponent, ThrowDoHitEvent>(this.OnThrowHit));
  }

  private void OnMeleeHit(Entity<MeleeThrowOnHitComponent> weapon, ref MeleeHitEvent args)
  {
    if (!args.IsHit || this._delay.IsDelayed((Entity<UseDelayComponent>) weapon.Owner) || args.HitEntities.Count == 0)
      return;
    Vector2 worldPosition = this._transform.GetWorldPosition(args.User);
    foreach (EntityUid hitEntity in (IEnumerable<EntityUid>) args.HitEntities)
    {
      Vector2 position = this._transform.GetMapCoordinates(hitEntity).Position;
      Vector2 direction = args.Direction ?? position - worldPosition;
      this.ThrowOnHitHelper(weapon, new EntityUid?(args.User), hitEntity, direction);
    }
  }

  private void OnThrowHit(Entity<MeleeThrowOnHitComponent> weapon, ref ThrowDoHitEvent args)
  {
    PhysicsComponent comp;
    if (!weapon.Comp.ActivateOnThrown || !this.TryComp<PhysicsComponent>(args.Thrown, out comp))
      return;
    this.ThrowOnHitHelper(weapon, args.Component.Thrower, args.Target, comp.LinearVelocity);
  }

  private void ThrowOnHitHelper(
    Entity<MeleeThrowOnHitComponent> ent,
    EntityUid? user,
    EntityUid target,
    Vector2 direction)
  {
    AttemptMeleeThrowOnHitEvent args1 = new AttemptMeleeThrowOnHitEvent(target, user);
    this.RaiseLocalEvent<AttemptMeleeThrowOnHitEvent>(ent.Owner, ref args1);
    if (args1.Cancelled)
      return;
    MeleeThrowOnHitStartEvent args2 = new MeleeThrowOnHitStartEvent(ent.Owner, user);
    this.RaiseLocalEvent<MeleeThrowOnHitStartEvent>(target, ref args2);
    if (ent.Comp.StunTime.HasValue)
      this._stun.TryParalyze(target, ent.Comp.StunTime.Value, false);
    if (direction == Vector2.Zero)
      return;
    ThrowingSystem throwing = this._throwing;
    EntityUid uid = target;
    Vector2 direction1 = Vector2Helpers.Normalized(direction) * ent.Comp.Distance;
    double speed = (double) ent.Comp.Speed;
    EntityUid? user1 = user;
    bool unanchorOnHit = ent.Comp.UnanchorOnHit;
    float? friction = new float?();
    int num = unanchorOnHit ? 1 : 0;
    throwing.TryThrow(uid, direction1, (float) speed, user1, friction: friction, unanchor: num != 0);
  }
}
