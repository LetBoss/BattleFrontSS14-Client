// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Damage.RMCTemporaryInvincibilitySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.Evasion;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.Damage;

public sealed class RMCTemporaryInvincibilitySystem : EntitySystem
{
  [Dependency]
  private INetManager _net;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private EvasionSystem _evasion;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RMCTemporaryInvincibilityComponent, RMCIgniteAttemptEvent>(new EntityEventRefHandler<RMCTemporaryInvincibilityComponent, RMCIgniteAttemptEvent>(this.OnIgnite));
    this.SubscribeLocalEvent<RMCTemporaryInvincibilityComponent, BeforeDamageChangedEvent>(new EntityEventRefHandler<RMCTemporaryInvincibilityComponent, BeforeDamageChangedEvent>(this.OnBeforeDamage));
    this.SubscribeLocalEvent<RMCTemporaryInvincibilityComponent, EvasionRefreshModifiersEvent>(new EntityEventRefHandler<RMCTemporaryInvincibilityComponent, EvasionRefreshModifiersEvent>(this.OnGetEvasion));
    this.SubscribeLocalEvent<RMCTemporaryInvincibilityComponent, ComponentStartup>(new EntityEventRefHandler<RMCTemporaryInvincibilityComponent, ComponentStartup>(this.OnAdded));
  }

  private void OnAdded(Entity<RMCTemporaryInvincibilityComponent> ent, ref ComponentStartup args)
  {
    this._evasion.RefreshEvasionModifiers((EntityUid) ent);
  }

  private void OnIgnite(
    Entity<RMCTemporaryInvincibilityComponent> ent,
    ref RMCIgniteAttemptEvent args)
  {
    args.Cancel();
  }

  private void OnBeforeDamage(
    Entity<RMCTemporaryInvincibilityComponent> ent,
    ref BeforeDamageChangedEvent args)
  {
    args.Cancelled = true;
  }

  private void OnGetEvasion(
    Entity<RMCTemporaryInvincibilityComponent> ent,
    ref EvasionRefreshModifiersEvent args)
  {
    args.Evasion += (FixedPoint2) 1000;
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCTemporaryInvincibilityComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCTemporaryInvincibilityComponent>();
    EntityUid uid;
    RMCTemporaryInvincibilityComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!(curTime < comp1.ExpiresAt))
      {
        this.RemCompDeferred<RMCTemporaryInvincibilityComponent>(uid);
        this._evasion.RefreshEvasionModifiers(uid);
      }
    }
  }
}
