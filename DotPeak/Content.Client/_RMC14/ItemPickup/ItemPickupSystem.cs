// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.ItemPickup.ItemPickupSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Hands;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Client._RMC14.ItemPickup;

public sealed class ItemPickupSystem : EntitySystem
{
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IGameTiming _timing;
  private TimeSpan _lastPickUp;

  public bool RecentItemPickUp { get; private set; }

  public virtual void Initialize()
  {
    this.SubscribeLocalEvent<RequestStopShootEvent>(new EntityEventHandler<RequestStopShootEvent>(this.OnRequestStopShoot), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ItemPickedUpEvent>(new EntityEventRefHandler<ItemPickedUpEvent>((object) this, __methodptr(OnItemPickedUp)), (Type[]) null, (Type[]) null);
  }

  private void OnRequestStopShoot(RequestStopShootEvent ev) => this.RecentItemPickUp = false;

  private void OnItemPickedUp(ref ItemPickedUpEvent ev)
  {
    EntityUid user = ev.User;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if ((localEntity.HasValue ? (EntityUid.op_Inequality(user, localEntity.GetValueOrDefault()) ? 1 : 0) : 1) != 0)
      return;
    this.RecentItemPickUp = true;
    this._lastPickUp = this._timing.CurTime;
  }

  public virtual void Update(float frameTime)
  {
    if (!this.RecentItemPickUp || !(this._timing.CurTime > this._lastPickUp + TimeSpan.FromSeconds(0.15000000596046448)))
      return;
    this.RecentItemPickUp = false;
  }
}
