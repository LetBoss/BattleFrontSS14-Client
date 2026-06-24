// Decompiled with JetBrains decompiler
// Type: Content.Client.Administration.UI.PlayerPanel.PlayerPanelEui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Administration.Managers;
using Content.Client.Eui;
using Content.Shared.Administration;
using Content.Shared.Eui;
using Robust.Client.Console;
using Robust.Client.UserInterface;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using System;

#nullable enable
namespace Content.Client.Administration.UI.PlayerPanel;

public sealed class PlayerPanelEui : BaseEui
{
  [Dependency]
  private IClientConsoleHost _console;
  [Dependency]
  private IClientAdminManager _admin;
  [Dependency]
  private IClipboardManager _clipboard;

  private Content.Client.Administration.UI.PlayerPanel.PlayerPanel PlayerPanel { get; }

  public PlayerPanelEui()
  {
    this.PlayerPanel = new Content.Client.Administration.UI.PlayerPanel.PlayerPanel(this._admin);
    this.PlayerPanel.OnUsernameCopy += (Action<string>) (username => this._clipboard.SetText(username));
    this.PlayerPanel.OnOpenNotes += (Action<NetUserId?>) (id => ((IConsoleHost) this._console).ExecuteCommand($"adminnotes \"{id}\""));
    this.PlayerPanel.OnKick += (Action<string>) (username => ((IConsoleHost) this._console).ExecuteCommand($"kick \"{username}\""));
    this.PlayerPanel.OnOpenBanPanel += (Action<NetUserId?>) (id => ((IConsoleHost) this._console).ExecuteCommand($"banpanel \"{id}\""));
    this.PlayerPanel.OnOpenBans += (Action<NetUserId?>) (id => ((IConsoleHost) this._console).ExecuteCommand($"banlist \"{id}\""));
    this.PlayerPanel.OnAhelp += (Action<NetUserId?>) (id => ((IConsoleHost) this._console).ExecuteCommand($"openahelp \"{id}\""));
    this.PlayerPanel.OnWhitelistToggle += (Action<NetUserId?, bool>) ((id, whitelisted) =>
    {
      IClientConsoleHost console = this._console;
      string str;
      if (!whitelisted)
        str = $"whitelistadd \"{id}\"";
      else
        str = $"whitelistremove \"{id}\"";
      ((IConsoleHost) console).ExecuteCommand(str);
    });
    this.PlayerPanel.OnFreezeAndMuteToggle += (Action) (() => this.SendMessage((EuiMessageBase) new PlayerPanelFreezeMessage(true)));
    this.PlayerPanel.OnFreeze += (Action) (() => this.SendMessage((EuiMessageBase) new PlayerPanelFreezeMessage()));
    this.PlayerPanel.OnLogs += (Action) (() => this.SendMessage((EuiMessageBase) new PlayerPanelLogsMessage()));
    this.PlayerPanel.OnRejuvenate += (Action) (() => this.SendMessage((EuiMessageBase) new PlayerPanelRejuvenationMessage()));
    this.PlayerPanel.OnDelete += (Action) (() => this.SendMessage((EuiMessageBase) new PlayerPanelDeleteMessage()));
    this.PlayerPanel.OnFollow += (Action) (() => this.SendMessage((EuiMessageBase) new PlayerPanelFollowMessage()));
    this.PlayerPanel.OnClose += (Action) (() => this.SendMessage((EuiMessageBase) new CloseEuiMessage()));
  }

  public override void Opened() => this.PlayerPanel.OpenCentered();

  public override void Closed() => this.PlayerPanel.Close();

  public override void HandleState(EuiStateBase state)
  {
    if (!(state is PlayerPanelEuiState playerPanelEuiState))
      return;
    this.PlayerPanel.TargetPlayer = new NetUserId?(playerPanelEuiState.Guid);
    this.PlayerPanel.TargetUsername = playerPanelEuiState.Username;
    this.PlayerPanel.SetUsername(playerPanelEuiState.Username);
    this.PlayerPanel.SetPlaytime(playerPanelEuiState.Playtime);
    this.PlayerPanel.SetBans(playerPanelEuiState.TotalBans, playerPanelEuiState.TotalRoleBans);
    this.PlayerPanel.SetNotes(playerPanelEuiState.TotalNotes);
    this.PlayerPanel.SetWhitelisted(playerPanelEuiState.Whitelisted);
    this.PlayerPanel.SetSharedConnections(playerPanelEuiState.SharedConnections);
    this.PlayerPanel.SetFrozen(playerPanelEuiState.CanFreeze, playerPanelEuiState.Frozen);
    this.PlayerPanel.SetAhelp(playerPanelEuiState.CanAhelp);
    this.PlayerPanel.SetButtons();
  }
}
