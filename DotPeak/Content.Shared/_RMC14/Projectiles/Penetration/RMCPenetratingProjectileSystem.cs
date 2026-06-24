// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Projectiles.Penetration.RMCPenetratingProjectileSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Construction;
using Content.Shared.Projectiles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Physics.Events;
using System;

#nullable enable
namespace Content.Shared._RMC14.Projectiles.Penetration;

public sealed class RMCPenetratingProjectileSystem : EntitySystem
{
  private const int HardCollisionGroup = 10;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private RMCSizeStunSystem _rmcSize;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCPenetratingProjectileComponent, MapInitEvent>(new EntityEventRefHandler<RMCPenetratingProjectileComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<RMCPenetratingProjectileComponent, PreventCollideEvent>(new EntityEventRefHandler<RMCPenetratingProjectileComponent, PreventCollideEvent>(this.OnPreventCollide));
    this.SubscribeLocalEvent<RMCPenetratingProjectileComponent, StartCollideEvent>(new EntityEventRefHandler<RMCPenetratingProjectileComponent, StartCollideEvent>(this.OnStartCollide), after: new Type[1]
    {
      typeof (SharedProjectileSystem)
    });
    this.SubscribeLocalEvent<RMCPenetratingProjectileComponent, ProjectileHitEvent>(new EntityEventRefHandler<RMCPenetratingProjectileComponent, ProjectileHitEvent>(this.OnProjectileHit));
    this.SubscribeLocalEvent<RMCPenetratingProjectileComponent, AfterProjectileHitEvent>(new EntityEventRefHandler<RMCPenetratingProjectileComponent, AfterProjectileHitEvent>(this.OnAllowAdditionalHits));
  }

  private void OnMapInit(Entity<RMCPenetratingProjectileComponent> ent, ref MapInitEvent args)
  {
    ent.Comp.ShotFrom = new EntityCoordinates?(this._transform.GetMoverCoordinates((EntityUid) ent));
    this.Dirty<RMCPenetratingProjectileComponent>(ent);
  }

  private void OnPreventCollide(
    Entity<RMCPenetratingProjectileComponent> ent,
    ref PreventCollideEvent args)
  {
    if (!ent.Comp.HitTargets.Contains(args.OtherEntity))
      return;
    args.Cancelled = true;
  }

  private void OnProjectileHit(
    Entity<RMCPenetratingProjectileComponent> ent,
    ref ProjectileHitEvent args)
  {
    if (ent.Comp.HitTargets.Contains(args.Target))
    {
      args.Handled = true;
    }
    else
    {
      ent.Comp.HitTargets.Add(args.Target);
      this.Dirty<RMCPenetratingProjectileComponent>(ent);
    }
  }

  private void OnStartCollide(
    Entity<RMCPenetratingProjectileComponent> ent,
    ref StartCollideEvent args)
  {
    ProjectileComponent comp1;
    if (!this.TryComp<ProjectileComponent>((EntityUid) ent, out comp1) || !ent.Comp.ShotFrom.HasValue)
      return;
    float rangeLossPerHit = ent.Comp.RangeLossPerHit;
    float multiplierLossPerHit = ent.Comp.DamageMultiplierLossPerHit;
    RMCSizes size;
    this._rmcSize.TryGetSize(args.OtherEntity, out size);
    if ((args.OtherFixture.CollisionLayer & 10) != 0)
    {
      OccluderComponent comp2;
      if (this.TryComp<OccluderComponent>(args.OtherEntity, out comp2) && !comp2.Enabled)
      {
        rangeLossPerHit *= ent.Comp.ThickMembraneMultiplier;
        multiplierLossPerHit *= ent.Comp.ThickMembraneMultiplier;
        if (this.HasComp<XenoStructureUpgradeableComponent>(args.OtherEntity))
        {
          rangeLossPerHit *= ent.Comp.MembraneMultiplier;
          multiplierLossPerHit *= ent.Comp.MembraneMultiplier;
        }
      }
      else
      {
        rangeLossPerHit *= ent.Comp.WallMultiplier;
        multiplierLossPerHit *= ent.Comp.WallMultiplier;
      }
    }
    else if (size >= RMCSizes.Big)
    {
      rangeLossPerHit *= ent.Comp.BigXenoMultiplier;
      multiplierLossPerHit *= ent.Comp.BigXenoMultiplier;
    }
    ent.Comp.Range -= rangeLossPerHit;
    this.Dirty<RMCPenetratingProjectileComponent>(ent);
    comp1.Damage *= 1f - multiplierLossPerHit;
    this.Dirty((EntityUid) ent, (IComponent) comp1);
  }

  private void OnAllowAdditionalHits(
    Entity<RMCPenetratingProjectileComponent> ent,
    ref AfterProjectileHitEvent args)
  {
    if (!ent.Comp.ShotFrom.HasValue)
      return;
    float num1 = (this._transform.GetMoverCoordinates((EntityUid) ent).Position - ent.Comp.ShotFrom.Value.Position).Length();
    double num2 = (double) ent.Comp.Range - (double) num1;
    ent.Comp.HitTargets.Add(args.Target);
    this.Dirty<RMCPenetratingProjectileComponent>(ent);
    if (num2 < 0.0)
      return;
    args.Projectile.Comp.ProjectileSpent = false;
    this.Dirty<ProjectileComponent>(args.Projectile);
  }
}
