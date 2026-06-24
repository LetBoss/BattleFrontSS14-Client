// Decompiled with JetBrains decompiler
// Type: Content.Client.Rendering.Profiled08b.RMCViewDriftSeeda432c5
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Weapons;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Rendering.Profiled08b;

public sealed class RMCViewDriftSeeda432c5 : EntitySystem
{
  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<RMCWeaponProfileHelloEvent>(new EntityEventHandler<RMCWeaponProfileHelloEvent>(this._m3138436880cc), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<RMCWeaponDrawSkewEvent>(new EntityEventHandler<RMCWeaponDrawSkewEvent>(this._m8e12e33972bb), (Type[]) null, (Type[]) null);
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    this.EntityManager.System<RMCProfileCacheNodea4fdbc>()._m9814afeff83e(frameTime);
  }

  private void _m3138436880cc(RMCWeaponProfileHelloEvent ev)
  {
    this.EntityManager.System<RMCProfileCacheNodea4fdbc>()._mf1a84035361f(ev);
  }

  private void _m8e12e33972bb(RMCWeaponDrawSkewEvent ev)
  {
    this.EntityManager.System<RMCProfileCacheNodea4fdbc>()._m5e41c632b8aa(ev);
  }
}
