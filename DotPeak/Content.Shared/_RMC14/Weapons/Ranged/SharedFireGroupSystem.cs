// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.SharedFireGroupSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Timing;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged;

public sealed class SharedFireGroupSystem : EntitySystem
{
  [Dependency]
  private UseDelaySystem _delay;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private IGameTiming _timing;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCFireGroupComponent, GunShotEvent>(new EntityEventRefHandler<RMCFireGroupComponent, GunShotEvent>(this.OnGunShot));
    this.SubscribeLocalEvent<RMCFireGroupComponent, ShotAttemptedEvent>(new EntityEventRefHandler<RMCFireGroupComponent, ShotAttemptedEvent>(this.OnShotAttempt));
  }

  private void OnGunShot(Entity<RMCFireGroupComponent> ent, ref GunShotEvent args)
  {
    EntityUid owner = ent.Owner;
    RMCFireGroupComponent comp1 = ent.Comp;
    foreach (EntityUid uid in this._hands.EnumerateHeld((Entity<HandsComponent>) args.User))
    {
      RMCFireGroupComponent comp2;
      UseDelayComponent comp3;
      if (this.TryComp<RMCFireGroupComponent>(uid, out comp2) && !(uid == owner) && !(comp2.Group != comp1.Group) && this.TryComp<UseDelayComponent>(uid, out comp3))
      {
        (EntityUid, UseDelayComponent) ent1 = (uid, comp3);
        this._delay.SetLength((Entity<UseDelayComponent>) ent1, comp1.Delay, comp1.UseDelayID);
        this._delay.TryResetDelay((Entity<UseDelayComponent>) ent1, true, comp1.UseDelayID);
      }
    }
    RMCUserFireGroupComponent fireGroupComponent = this.EnsureComp<RMCUserFireGroupComponent>(args.User);
    fireGroupComponent.LastFired[ent.Comp.Group] = this._timing.CurTime;
    fireGroupComponent.LastGun[ent.Comp.Group] = (EntityUid) ent;
    this.Dirty(args.User, (IComponent) fireGroupComponent);
  }

  private void OnShotAttempt(Entity<RMCFireGroupComponent> ent, ref ShotAttemptedEvent args)
  {
    if (args.Cancelled)
      return;
    UseDelayComponent comp1;
    if (this.TryComp<UseDelayComponent>(ent.Owner, out comp1) && this._delay.IsDelayed((Entity<UseDelayComponent>) (ent.Owner, comp1), ent.Comp.UseDelayID))
    {
      args.Cancel();
    }
    else
    {
      RMCUserFireGroupComponent comp2;
      TimeSpan timeSpan;
      if (!this.TryComp<RMCUserFireGroupComponent>(args.User, out comp2) || !comp2.LastFired.TryGetValue(ent.Comp.Group, out timeSpan) || !(this._timing.CurTime < timeSpan + ent.Comp.Delay) || !(comp2.LastGun.GetValueOrDefault<string, EntityUid>(ent.Comp.Group) != ent.Owner))
        return;
      args.Cancel();
    }
  }
}
