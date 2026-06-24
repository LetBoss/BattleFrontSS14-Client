// Decompiled with JetBrains decompiler
// Type: Content.Client.Administration.UI.BanPanel.BanPanelEui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Eui;
using Content.Shared.Administration;
using Content.Shared.Database;
using Content.Shared.Eui;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using System;
using System.Net;

#nullable enable
namespace Content.Client.Administration.UI.BanPanel;

public sealed class BanPanelEui : BaseEui
{
  private Content.Client.Administration.UI.BanPanel.BanPanel BanPanel { get; }

  public BanPanelEui()
  {
    this.BanPanel = new Content.Client.Administration.UI.BanPanel.BanPanel();
    ((BaseWindow) this.BanPanel).OnClose += (Action) (() => this.SendMessage((EuiMessageBase) new CloseEuiMessage()));
    this.BanPanel.BanSubmitted += (Action<string, (IPAddress, int)?, bool, ImmutableTypedHwid, bool, uint, string, NoteSeverity, string[], bool>) ((player, ip, useLastIp, hwid, useLastHwid, minutes, reason, severity, roles, erase) => this.SendMessage((EuiMessageBase) new BanPanelEuiStateMsg.CreateBanRequest(player, ip, useLastIp, hwid, useLastHwid, minutes, reason, severity, roles, erase)));
    this.BanPanel.PlayerChanged += (Action<string>) (player => this.SendMessage((EuiMessageBase) new BanPanelEuiStateMsg.GetPlayerInfoRequest(player)));
  }

  public override void HandleState(EuiStateBase state)
  {
    if (!(state is BanPanelEuiState banPanelEuiState))
      return;
    this.BanPanel.UpdateBanFlag(banPanelEuiState.HasBan);
    this.BanPanel.UpdatePlayerData(banPanelEuiState.PlayerName);
  }

  public override void Opened() => ((BaseWindow) this.BanPanel).OpenCentered();

  public override void Closed()
  {
    ((BaseWindow) this.BanPanel).Close();
    ((Control) this.BanPanel).Orphan();
  }
}
