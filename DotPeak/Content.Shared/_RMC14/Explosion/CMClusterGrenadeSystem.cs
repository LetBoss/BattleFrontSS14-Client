// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Explosion.CMClusterGrenadeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Projectiles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Events;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Runtime.InteropServices;

#nullable enable
namespace Content.Shared._RMC14.Explosion;

public sealed class CMClusterGrenadeSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  private Robust.Shared.GameObjects.EntityQuery<UserLimitHitsComponent> _userLimits;

  public override void Initialize()
  {
    this._userLimits = this.GetEntityQuery<UserLimitHitsComponent>();
    this.SubscribeLocalEvent<ClusterLimitHitsComponent, CMClusterSpawnedEvent>(new EntityEventRefHandler<ClusterLimitHitsComponent, CMClusterSpawnedEvent>(this.OnClusterLimitHitsSpawned));
    this.SubscribeLocalEvent<ProjectileLimitHitsComponent, ProjectileHitEvent>(new EntityEventRefHandler<ProjectileLimitHitsComponent, ProjectileHitEvent>(this.OnProjectileLimitHitsHit));
    this.SubscribeLocalEvent<ProjectileLimitHitsComponent, PreventCollideEvent>(new EntityEventRefHandler<ProjectileLimitHitsComponent, PreventCollideEvent>(this.OnProjectileLimitHitsPreventCollide));
  }

  private void OnClusterLimitHitsSpawned(
    Entity<ClusterLimitHitsComponent> ent,
    ref CMClusterSpawnedEvent args)
  {
    foreach (EntityUid uid in args.Spawned)
    {
      ProjectileLimitHitsComponent limitHitsComponent = this.EnsureComp<ProjectileLimitHitsComponent>(uid);
      if (ent.Comp.IgnoreFirstHit)
        limitHitsComponent.IgnoredEntities = args.HitEntities;
      limitHitsComponent.Limit = ent.Comp.Limit;
      limitHitsComponent.OriginEntity = args.OriginEntity;
      limitHitsComponent.ExtraId = args.ExtraId;
      this.Dirty(uid, (IComponent) limitHitsComponent);
    }
  }

  private void OnProjectileLimitHitsHit(
    Entity<ProjectileLimitHitsComponent> ent,
    ref ProjectileHitEvent args)
  {
    UserLimitHitsComponent component;
    if (!this._userLimits.TryComp(args.Target, out component))
      return;
    if (!this.CanHit((Entity<UserLimitHitsComponent>) (args.Target, component), ent))
    {
      args.Handled = true;
    }
    else
    {
      component.HitBy.Add(new Hit(this.GetNetEntity(ent.Comp.OriginEntity), this._timing.CurTime + component.Expire, ent.Comp.ExtraId));
      this.Dirty(args.Target, (IComponent) component);
    }
  }

  private void OnProjectileLimitHitsPreventCollide(
    Entity<ProjectileLimitHitsComponent> ent,
    ref PreventCollideEvent args)
  {
    UserLimitHitsComponent component;
    if (args.Cancelled || !this._userLimits.TryComp(args.OtherEntity, out component) || this.CanHit((Entity<UserLimitHitsComponent>) (args.OtherEntity, component), ent))
      return;
    args.Cancelled = true;
  }

  public bool CanHit(
    Entity<UserLimitHitsComponent> user,
    Entity<ProjectileLimitHitsComponent> projectile)
  {
    TimeSpan curTime = this._timing.CurTime;
    Span<Hit> span1 = CollectionsMarshal.AsSpan<Hit>(user.Comp.HitBy);
    int num = 0;
    Span<Hit> span2 = span1;
    for (int index = 0; index < span2.Length; ++index)
    {
      ref Hit local = ref span2[index];
      if (projectile.Comp.Limit != 0 || projectile.Comp.IgnoredEntities.Contains((EntityUid) user))
      {
        int? extraId1;
        int? extraId2;
        if (local.Id.Id == projectile.Comp.OriginEntity.Id)
        {
          extraId1 = local.ExtraId;
          if (extraId1.HasValue)
          {
            extraId1 = local.ExtraId;
            extraId2 = projectile.Comp.ExtraId;
            if (extraId1.GetValueOrDefault() == extraId2.GetValueOrDefault() & extraId1.HasValue == extraId2.HasValue)
              goto label_9;
          }
          else
            goto label_9;
        }
        if (local.Id == this.GetNetEntity(projectile.Owner))
        {
          extraId2 = local.ExtraId;
          if (extraId2.HasValue)
          {
            extraId2 = local.ExtraId;
            extraId1 = projectile.Comp.ExtraId;
            if (!(extraId2.GetValueOrDefault() == extraId1.GetValueOrDefault() & extraId2.HasValue == extraId1.HasValue))
              goto label_10;
          }
          if (!(local.ExpireAt > curTime))
            goto label_10;
        }
        else
          goto label_10;
label_9:
        ++num;
label_10:
        if (num >= projectile.Comp.Limit && num != 0)
          return false;
      }
    }
    return true;
  }

  public override void Update(float frameTime)
  {
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<UserLimitHitsComponent> entityQueryEnumerator = this.EntityQueryEnumerator<UserLimitHitsComponent>();
    EntityUid uid;
    UserLimitHitsComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      bool flag = false;
      for (int index = comp1.HitBy.Count - 1; index >= 0; --index)
      {
        Hit hit = comp1.HitBy[index];
        if (!(curTime <= hit.ExpireAt))
        {
          comp1.HitBy.RemoveSwap<Hit>(index);
          flag = true;
        }
      }
      if (flag)
        this.Dirty(uid, (IComponent) comp1);
    }
  }
}
