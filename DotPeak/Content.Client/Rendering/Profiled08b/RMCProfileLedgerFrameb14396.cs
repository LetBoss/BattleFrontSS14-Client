// Decompiled with JetBrains decompiler
// Type: Content.Client.Rendering.Profiled08b.RMCProfileLedgerFrameb14396
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Weapons;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Rendering.Profiled08b;

public sealed class RMCProfileLedgerFrameb14396 : EntitySystem
{
  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<RMCWeaponProfilePingEvent>(new EntityEventHandler<RMCWeaponProfilePingEvent>(this._m0298b3d9346b), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<RMCWeaponProfilePulseRequestEvent>(new EntityEventHandler<RMCWeaponProfilePulseRequestEvent>(this._ma706658768c1), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<RMCWeaponProfileFrameRequestEvent>(new EntityEventHandler<RMCWeaponProfileFrameRequestEvent>(this._ma54add8e8957), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<RMCWeaponProfileIntegrityRequestEvent>(new EntityEventHandler<RMCWeaponProfileIntegrityRequestEvent>(this._m5a00f42b3368), (Type[]) null, (Type[]) null);
  }

  private void _m0298b3d9346b(RMCWeaponProfilePingEvent ev)
  {
    this.EntityManager.System<RMCProfileCacheNodea4fdbc>()._m09870773dc85(ev);
  }

  private void _ma706658768c1(RMCWeaponProfilePulseRequestEvent ev)
  {
    this.EntityManager.System<RMCProfileCacheNodea4fdbc>()._m2670d1b35bd4(ev);
  }

  private void _ma54add8e8957(RMCWeaponProfileFrameRequestEvent ev)
  {
    this.EntityManager.System<RMCProfileCacheNodea4fdbc>()._mfb1ca1c81369(ev);
  }

  private void _m5a00f42b3368(RMCWeaponProfileIntegrityRequestEvent ev)
  {
    this.EntityManager.System<RMCProfileCacheNodea4fdbc>()._m4ec44a315a26(ev);
  }
}
