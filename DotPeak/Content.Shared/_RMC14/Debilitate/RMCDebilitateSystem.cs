// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Debilitate.RMCDebilitateSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Projectiles;
using Content.Shared.Stunnable;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared._RMC14.Debilitate;

public sealed class RMCDebilitateSystem : EntitySystem
{
  [Dependency]
  private EntityWhitelistSystem _entityWhitelist;
  [Dependency]
  private SharedStunSystem _stun;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCDebilitateComponent, ProjectileHitEvent>(new EntityEventRefHandler<RMCDebilitateComponent, ProjectileHitEvent>(this.OnDebilitateProjectileHit));
  }

  private void OnDebilitateProjectileHit(
    Entity<RMCDebilitateComponent> ent,
    ref ProjectileHitEvent args)
  {
    if (!this._entityWhitelist.CheckBoth(new EntityUid?(args.Target), ent.Comp.Blacklist, ent.Comp.Whitelist))
      return;
    this._stun.TryParalyze(args.Target, ent.Comp.Knockdown, true);
  }
}
