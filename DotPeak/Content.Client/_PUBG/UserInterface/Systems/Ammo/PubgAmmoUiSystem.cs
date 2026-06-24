// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.Systems.Ammo.PubgAmmoUiSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._PUBG.UserInterface.Systems.Ammo;

public sealed class PubgAmmoUiSystem : EntitySystem
{
  public event Action<PubgAmmoUpdateEvent>? AmmoUpdated;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<PubgAmmoUpdateEvent>(new EntityEventHandler<PubgAmmoUpdateEvent>(this.OnAmmoUpdate), (Type[]) null, (Type[]) null);
  }

  private void OnAmmoUpdate(PubgAmmoUpdateEvent msg)
  {
    Action<PubgAmmoUpdateEvent> ammoUpdated = this.AmmoUpdated;
    if (ammoUpdated == null)
      return;
    ammoUpdated(msg);
  }

  public void RequestRefresh()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new PubgAmmoRefreshRequestEvent());
  }
}
