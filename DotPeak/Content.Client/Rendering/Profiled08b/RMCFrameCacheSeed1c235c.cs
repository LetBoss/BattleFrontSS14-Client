// Decompiled with JetBrains decompiler
// Type: Content.Client.Rendering.Profiled08b.RMCFrameCacheSeed1c235c
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Weapons;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Rendering.Profiled08b;

public sealed class RMCFrameCacheSeed1c235c : EntitySystem
{
  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<RMCWeaponProfileCatalogRequestEvent>(new EntityEventHandler<RMCWeaponProfileCatalogRequestEvent>(this._m493e22efb540), (Type[]) null, (Type[]) null);
  }

  private void _m493e22efb540(RMCWeaponProfileCatalogRequestEvent ev)
  {
    this.EntityManager.System<RMCProfileCacheNodea4fdbc>()._me13d53b6c217(ev);
  }
}
