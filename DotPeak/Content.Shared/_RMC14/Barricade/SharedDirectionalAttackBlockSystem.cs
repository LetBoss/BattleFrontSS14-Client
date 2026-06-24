// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Barricade.SharedDirectionalAttackBlockSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Random;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared.Damage;
using Content.Shared.Mobs.Components;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.Barricade;

public abstract class SharedDirectionalAttackBlockSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<MobStateComponent, MeleeAttackAttemptEvent>(new EntityEventRefHandler<MobStateComponent, MeleeAttackAttemptEvent>(this.OnMeleeAttackAttempt));
  }

  private void OnMeleeAttackAttempt(Entity<MobStateComponent> ent, ref MeleeAttackAttemptEvent args)
  {
    foreach (NetEntity potentialTarget in args.PotentialTargets)
    {
      if (potentialTarget == args.Target)
        break;
      EntityUid entity = this.GetEntity(potentialTarget);
      if (this.IsAttackBlocked((EntityUid) ent, entity))
      {
        NetCoordinates netCoordinates = this.GetNetCoordinates(this._transform.GetMoverCoordinates(this.GetEntity(potentialTarget)));
        switch (args.Attack)
        {
          case LightAttackEvent _:
            args.Attack = (AttackEvent) new LightAttackEvent(new NetEntity?(potentialTarget), args.Weapon, netCoordinates);
            return;
          case DisarmAttackEvent _:
            args.Attack = (AttackEvent) new LightAttackEvent(new NetEntity?(potentialTarget), args.Weapon, netCoordinates);
            return;
          default:
            return;
        }
      }
    }
  }

  public bool IsAttackBlocked(EntityUid attacker, EntityUid target)
  {
    DirectionalAttackBlockerComponent comp1;
    if (!this.TryComp<DirectionalAttackBlockerComponent>(target, out comp1) || !this.Transform(target).Anchored || !comp1.BlockMarineAttacks && this.HasComp<MarineComponent>(attacker) || !this.IsFacingTarget(target, attacker))
      return false;
    long seed = (long) this._timing.CurTick.Value << 32 /*0x20*/ | (long) (uint) this.GetNetEntity(attacker).Id;
    DamageableComponent comp2;
    return !this.TryComp<DamageableComponent>(target, out comp2) || (double) Math.Max(comp1.MinimumBlockChance, ((float) comp1.MaxHealth - (float) comp2.TotalDamage) / (float) comp1.MaxHealth) >= (double) new Xoroshiro64S(seed).NextFloat(0.0f, 1f);
  }

  private sbyte GetRelativeDiff(
    EntityUid blocker,
    EntityUid target,
    EntityCoordinates? originCoordinates = null)
  {
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates(target);
    if (originCoordinates.HasValue)
      moverCoordinates = originCoordinates.Value;
    (EntityCoordinates Coords, Angle worldRot) coordinateRotation = this._transform.GetMoverCoordinateRotation(blocker, this.Transform(blocker));
    // ISSUE: explicit reference operation
    return Math.Abs((sbyte) (DirectionExtensions.GetDir(Vector2Helpers.Normalized(moverCoordinates.Position - coordinateRotation.Coords.Position)) - ((Angle) @coordinateRotation.worldRot).GetDir()));
  }

  public bool IsFacingTarget(
    EntityUid blocker,
    EntityUid target,
    EntityCoordinates? originCoordinates = null)
  {
    bool flag;
    switch (this.GetRelativeDiff(blocker, target, originCoordinates))
    {
      case 0:
      case 1:
      case 7:
        flag = true;
        break;
      default:
        flag = false;
        break;
    }
    return flag;
  }

  public bool IsBehindTarget(
    EntityUid blocker,
    EntityUid target,
    EntityCoordinates? originCoordinates = null)
  {
    bool flag;
    switch (this.GetRelativeDiff(blocker, target, originCoordinates))
    {
      case 3:
      case 4:
      case 5:
        flag = true;
        break;
      default:
        flag = false;
        break;
    }
    return flag;
  }
}
