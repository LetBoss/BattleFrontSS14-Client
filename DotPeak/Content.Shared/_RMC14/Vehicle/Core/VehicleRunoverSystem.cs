// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleRunoverSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Standing;
using Content.Shared.Stunnable;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Events;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleRunoverSystem : EntitySystem
{
  [Dependency]
  private readonly EntityLookupSystem _lookup;
  [Dependency]
  private readonly SharedStunSystem _stun;
  [Dependency]
  private readonly IGameTiming _timing;
  [Dependency]
  private readonly INetManager _net;
  public static readonly TimeSpan StandUpGrace = TimeSpan.FromSeconds(0.6);

  public override void Initialize()
  {
    this.SubscribeLocalEvent<VehicleRunoverComponent, PreventCollideEvent>(new EntityEventRefHandler<VehicleRunoverComponent, PreventCollideEvent>(this.OnPreventCollide));
    this.SubscribeLocalEvent<VehicleRunoverComponent, StoodEvent>(new EntityEventRefHandler<VehicleRunoverComponent, StoodEvent>(this.OnRunoverStood));
  }

  private void OnPreventCollide(Entity<VehicleRunoverComponent> ent, ref PreventCollideEvent args)
  {
    if (!(args.OtherEntity == ent.Comp.Vehicle))
      return;
    args.Cancelled = true;
  }

  private void OnRunoverStood(Entity<VehicleRunoverComponent> ent, ref StoodEvent args)
  {
    if (this._net.IsClient)
      return;
    if (this.TerminatingOrDeleted(ent.Comp.Vehicle))
    {
      this.RemCompDeferred<VehicleRunoverComponent>((EntityUid) ent);
    }
    else
    {
      TimeSpan timeSpan = this._timing.CurTime + VehicleRunoverSystem.StandUpGrace;
      if (!(ent.Comp.ExpiresAt < timeSpan))
        return;
      ent.Comp.ExpiresAt = timeSpan;
      this.Dirty<VehicleRunoverComponent>(ent);
    }
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<VehicleRunoverComponent> entityQueryEnumerator = this.EntityQueryEnumerator<VehicleRunoverComponent>();
    EntityUid uid;
    VehicleRunoverComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (this.TerminatingOrDeleted(comp1.Vehicle))
        this.RemCompDeferred<VehicleRunoverComponent>(uid);
      else if (this.IsOverlapping(uid, comp1.Vehicle))
      {
        if (comp1.Duration == TimeSpan.Zero)
          comp1.Duration = TimeSpan.FromSeconds(1.5);
        comp1.ExpiresAt = curTime + comp1.Duration + VehicleRunoverSystem.StandUpGrace;
        this._stun.TryKnockdown(uid, comp1.Duration, true);
      }
      else if (comp1.ExpiresAt <= curTime)
        this.RemCompDeferred<VehicleRunoverComponent>(uid);
    }
  }

  private bool IsOverlapping(EntityUid mob, EntityUid vehicle)
  {
    Box2 worldAabb1 = this._lookup.GetWorldAABB(mob);
    Box2 worldAabb2 = this._lookup.GetWorldAABB(vehicle);
    return ((Box2) ref worldAabb1).Intersects(ref worldAabb2);
  }
}
