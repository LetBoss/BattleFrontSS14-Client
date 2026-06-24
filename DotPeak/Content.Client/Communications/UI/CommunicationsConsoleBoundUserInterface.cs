// Decompiled with JetBrains decompiler
// Type: Content.Client.Communications.UI.CommunicationsConsoleBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.CCVar;
using Content.Shared.Chat;
using Content.Shared.Communications;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Communications.UI;

public sealed class CommunicationsConsoleBoundUserInterface(EntityUid owner, Enum uiKey) : 
  BoundUserInterface(owner, uiKey)
{
  [Dependency]
  private IConfigurationManager _cfg;
  [Robust.Shared.ViewVariables.ViewVariables]
  private CommunicationsConsoleMenu? _menu;

  protected virtual void Open()
  {
    base.Open();
    this._menu = BoundUserInterfaceExt.CreateWindow<CommunicationsConsoleMenu>((BoundUserInterface) this);
    this._menu.OnAnnounce += new Action<string>(this.AnnounceButtonPressed);
    this._menu.OnBroadcast += new Action<string>(this.BroadcastButtonPressed);
    this._menu.OnAlertLevel += new Action<string>(this.AlertLevelSelected);
    this._menu.OnEmergencyLevel += new Action(this.EmergencyShuttleButtonPressed);
  }

  public void AlertLevelSelected(string level)
  {
    if (!this._menu.AlertLevelSelectable)
      return;
    this._menu.CurrentLevel = level;
    this.SendMessage((BoundUserInterfaceMessage) new CommunicationsConsoleSelectAlertLevelMessage(level));
  }

  public void EmergencyShuttleButtonPressed()
  {
    if (this._menu.CountdownStarted)
      this.RecallShuttle();
    else
      this.CallShuttle();
  }

  public void AnnounceButtonPressed(string message)
  {
    int cvar = this._cfg.GetCVar<int>(CCVars.ChatMaxAnnouncementLength);
    this.SendMessage((BoundUserInterfaceMessage) new CommunicationsConsoleAnnounceMessage(SharedChatSystem.SanitizeAnnouncement(message, cvar)));
  }

  public void BroadcastButtonPressed(string message)
  {
    this.SendMessage((BoundUserInterfaceMessage) new CommunicationsConsoleBroadcastMessage(message));
  }

  public void CallShuttle()
  {
    this.SendMessage((BoundUserInterfaceMessage) new CommunicationsConsoleCallEmergencyShuttleMessage());
  }

  public void RecallShuttle()
  {
    this.SendMessage((BoundUserInterfaceMessage) new CommunicationsConsoleRecallEmergencyShuttleMessage());
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is CommunicationsConsoleInterfaceState consoleInterfaceState) || this._menu == null)
      return;
    this._menu.CanAnnounce = consoleInterfaceState.CanAnnounce;
    this._menu.CanBroadcast = consoleInterfaceState.CanBroadcast;
    this._menu.CanCall = consoleInterfaceState.CanCall;
    this._menu.CountdownStarted = consoleInterfaceState.CountdownStarted;
    this._menu.AlertLevelSelectable = consoleInterfaceState.AlertLevels != null && !float.IsNaN(consoleInterfaceState.CurrentAlertDelay) && (double) consoleInterfaceState.CurrentAlertDelay <= 0.0;
    this._menu.CurrentLevel = consoleInterfaceState.CurrentAlert;
    this._menu.CountdownEnd = consoleInterfaceState.ExpectedCountdownEnd;
    this._menu.UpdateCountdown();
    this._menu.UpdateAlertLevels(consoleInterfaceState.AlertLevels, this._menu.CurrentLevel);
    ((BaseButton) this._menu.AlertLevelButton).Disabled = !this._menu.AlertLevelSelectable;
    ((BaseButton) this._menu.EmergencyShuttleButton).Disabled = !this._menu.CanCall;
    ((BaseButton) this._menu.AnnounceButton).Disabled = !this._menu.CanAnnounce;
    ((BaseButton) this._menu.BroadcastButton).Disabled = !this._menu.CanBroadcast;
  }
}
