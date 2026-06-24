// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Ghost.Controls.Roles.MakeGhostRoleEui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Eui;
using Content.Shared.Eui;
using Content.Shared.Ghost.Roles;
using Content.Shared.Ghost.Roles.Raffles;
using Robust.Client.Console;
using Robust.Client.Player;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Client.UserInterface.Systems.Ghost.Controls.Roles;

public sealed class MakeGhostRoleEui : BaseEui
{
  [Dependency]
  private IEntityManager _entManager;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IClientConsoleHost _consoleHost;
  private readonly MakeGhostRoleWindow _window;

  public MakeGhostRoleEui()
  {
    this._window = new MakeGhostRoleWindow();
    ((BaseWindow) this._window).OnClose += new Action(this.OnClose);
    this._window.OnMake += new MakeGhostRoleWindow.MakeRole(this.OnMake);
  }

  public override void HandleState(EuiStateBase state)
  {
    if (!(state is MakeGhostRoleEuiState ghostRoleEuiState))
      return;
    this._window.SetEntity(this._entManager, ghostRoleEuiState.Entity);
  }

  public override void Opened()
  {
    base.Opened();
    ((BaseWindow) this._window).OpenCentered();
  }

  private void OnMake(
    NetEntity entity,
    string name,
    string description,
    string rules,
    bool makeSentient,
    GhostRoleRaffleSettings? raffleSettings)
  {
    ICommonSession localSession = ((ISharedPlayerManager) this._playerManager).LocalSession;
    if (localSession == null)
      return;
    string str1 = $"{(raffleSettings != null ? "makeghostroleraffled" : "makeghostrole")} \"{CommandParsing.Escape(entity.ToString())}\" \"{CommandParsing.Escape(name)}\" \"{CommandParsing.Escape(description)}\" ";
    if (raffleSettings != null)
      str1 += $"{raffleSettings.InitialDuration} {raffleSettings.JoinExtendsDurationBy} {raffleSettings.MaxDuration} ";
    string str2 = $"{str1}\"{CommandParsing.Escape(rules)}\"";
    ((IConsoleHost) this._consoleHost).ExecuteCommand(localSession, str2);
    if (makeSentient)
    {
      string str3 = $"makesentient \"{CommandParsing.Escape(entity.ToString())}\"";
      ((IConsoleHost) this._consoleHost).ExecuteCommand(localSession, str3);
    }
    ((BaseWindow) this._window).Close();
  }

  private void OnClose()
  {
    this.Closed();
    this.SendMessage((EuiMessageBase) new CloseEuiMessage());
  }
}
