// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Party.PubgAdminPartySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.Party;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._PUBG.Party;

public sealed class PubgAdminPartySystem : EntitySystem
{
  public event Action<IReadOnlyList<PubgAdminPartyPlayerInfo>>? PartyListUpdated;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<PubgAdminPartyListResponseEvent>(new EntityEventHandler<PubgAdminPartyListResponseEvent>(this.OnPartyListResponse), (Type[]) null, (Type[]) null);
  }

  public void RequestPartyList()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new PubgAdminPartyListRequestEvent());
  }

  public void RequestBreakParty(NetUserId targetUserId)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new PubgAdminPartyBreakRequestEvent(targetUserId));
  }

  public void RequestForceJoin(NetUserId targetUserId)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new PubgAdminPartyForceJoinRequestEvent(targetUserId));
  }

  private void OnPartyListResponse(PubgAdminPartyListResponseEvent ev)
  {
    Action<IReadOnlyList<PubgAdminPartyPlayerInfo>> partyListUpdated = this.PartyListUpdated;
    if (partyListUpdated == null)
      return;
    partyListUpdated((IReadOnlyList<PubgAdminPartyPlayerInfo>) ev.Players);
  }
}
