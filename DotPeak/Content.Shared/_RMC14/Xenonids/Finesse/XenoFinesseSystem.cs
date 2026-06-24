// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Finesse.XenoFinesseSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Finesse;

public sealed class XenoFinesseSystem : EntitySystem
{
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private INetManager _net;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<XenoFinesseComponent, MeleeHitEvent>(new EntityEventRefHandler<XenoFinesseComponent, MeleeHitEvent>(this.OnFinesseMeleeHit));
  }

  private void OnFinesseMeleeHit(Entity<XenoFinesseComponent> xeno, ref MeleeHitEvent args)
  {
    foreach (EntityUid hitEntity in (IEnumerable<EntityUid>) args.HitEntities)
    {
      if (this._xeno.CanAbilityAttackTarget((EntityUid) xeno, hitEntity))
      {
        XenoMarkedComponent xenoMarkedComponent = this.EnsureComp<XenoMarkedComponent>(hitEntity);
        xenoMarkedComponent.WearOffAt = this._timing.CurTime + xeno.Comp.MarkedTime;
        xenoMarkedComponent.TimeAdded = this._timing.CurTime;
        break;
      }
    }
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoMarkedComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoMarkedComponent>();
    EntityUid uid;
    XenoMarkedComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!(curTime < comp1.WearOffAt))
        this.RemCompDeferred<XenoMarkedComponent>(uid);
    }
  }
}
