// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Weapons.JoinReplySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Weapons;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._RMC14.Weapons;

public sealed class JoinReplySystem : EntitySystem
{
  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<RMCWeaponProfileInitEvent>(new EntityEventHandler<RMCWeaponProfileInitEvent>(this.OnJoinRequest), (Type[]) null, (Type[]) null);
  }

  private void OnJoinRequest(RMCWeaponProfileInitEvent ev)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new RMCWeaponProfileReadyEvent(ev.Nonce, ev.Token));
  }
}
