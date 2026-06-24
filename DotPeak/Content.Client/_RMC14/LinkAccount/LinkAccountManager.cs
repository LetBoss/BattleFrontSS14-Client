// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.LinkAccount.LinkAccountManager
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.LinkAccount;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._RMC14.LinkAccount;

public sealed class LinkAccountManager : IPostInjectInit
{
  [Dependency]
  private INetManager _net;
  private readonly List<SharedRMCPatron> _allPatrons = new List<SharedRMCPatron>();

  public SharedRMCPatronTier? Tier { get; private set; }

  public bool Linked { get; private set; }

  public Color? GhostColor { get; private set; }

  public SharedRMCLobbyMessage? LobbyMessage { get; private set; }

  public SharedRMCRoundEndShoutouts? RoundEndShoutout { get; private set; }

  public event Action<Guid>? CodeReceived;

  public event Action? Updated;

  private void OnCode(LinkAccountCodeMsg message)
  {
    Action<Guid> codeReceived = this.CodeReceived;
    if (codeReceived == null)
      return;
    codeReceived(message.Code);
  }

  private void OnStatus(LinkAccountStatusMsg ev)
  {
    this.Tier = ev.Patron?.Tier;
    SharedRMCPatronFull patron = ev.Patron;
    this.Linked = (object) patron != null && patron.Linked;
    this.GhostColor = (Color?) ev.Patron?.GhostColor;
    this.LobbyMessage = ev.Patron?.LobbyMessage;
    this.RoundEndShoutout = ev.Patron?.RoundEndShoutout;
    Action updated = this.Updated;
    if (updated == null)
      return;
    updated();
  }

  private void OnPatronList(RMCPatronListMsg ev)
  {
    this._allPatrons.Clear();
    this._allPatrons.AddRange((IEnumerable<SharedRMCPatron>) ev.Patrons);
  }

  public IReadOnlyList<SharedRMCPatron> GetPatrons()
  {
    return (IReadOnlyList<SharedRMCPatron>) this._allPatrons;
  }

  public bool CanViewPatronPerks()
  {
    SharedRMCPatronTier tier = this.Tier;
    if ((object) tier == null)
      return false;
    return tier.GhostColor || tier.NamedItems || tier.Figurines || tier.LobbyMessage || tier.RoundEndShoutout;
  }

  void IPostInjectInit.PostInject()
  {
    // ISSUE: method pointer
    this._net.RegisterNetMessage<LinkAccountCodeMsg>(new ProcessMessage<LinkAccountCodeMsg>((object) this, __methodptr(OnCode)), (NetMessageAccept) 3);
    this._net.RegisterNetMessage<LinkAccountRequestMsg>((ProcessMessage<LinkAccountRequestMsg>) null, (NetMessageAccept) 3);
    // ISSUE: method pointer
    this._net.RegisterNetMessage<LinkAccountStatusMsg>(new ProcessMessage<LinkAccountStatusMsg>((object) this, __methodptr(OnStatus)), (NetMessageAccept) 3);
    // ISSUE: method pointer
    this._net.RegisterNetMessage<RMCPatronListMsg>(new ProcessMessage<RMCPatronListMsg>((object) this, __methodptr(OnPatronList)), (NetMessageAccept) 3);
    this._net.RegisterNetMessage<RMCClearGhostColorMsg>((ProcessMessage<RMCClearGhostColorMsg>) null, (NetMessageAccept) 3);
    this._net.RegisterNetMessage<RMCChangeGhostColorMsg>((ProcessMessage<RMCChangeGhostColorMsg>) null, (NetMessageAccept) 3);
    this._net.RegisterNetMessage<RMCChangeLobbyMessageMsg>((ProcessMessage<RMCChangeLobbyMessageMsg>) null, (NetMessageAccept) 3);
    this._net.RegisterNetMessage<RMCChangeMarineShoutoutMsg>((ProcessMessage<RMCChangeMarineShoutoutMsg>) null, (NetMessageAccept) 3);
    this._net.RegisterNetMessage<RMCChangeXenoShoutoutMsg>((ProcessMessage<RMCChangeXenoShoutoutMsg>) null, (NetMessageAccept) 3);
  }
}
