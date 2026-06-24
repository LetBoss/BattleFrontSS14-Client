// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vendors.RMCVendorUserRechargeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.Vendors;

public sealed class RMCVendorUserRechargeSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _gameTiming;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RMCVendorUserRechargeComponent, ComponentStartup>(new EntityEventRefHandler<RMCVendorUserRechargeComponent, ComponentStartup>(this.OnStartup));
  }

  private void OnStartup(Entity<RMCVendorUserRechargeComponent> ent, ref ComponentStartup args)
  {
    if (!(ent.Comp.LastUpdate == TimeSpan.Zero))
      return;
    ent.Comp.LastUpdate = this._gameTiming.CurTime;
  }
}
