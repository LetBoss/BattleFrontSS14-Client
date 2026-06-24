// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Bwoink.UserAHelpUIHandler
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Administration.UI.Bwoink;
using Content.Shared.Administration;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.UserInterface.Systems.Bwoink;

public sealed class UserAHelpUIHandler : IAHelpUIHandler, IDisposable
{
  private readonly NetUserId _ownerId;
  private DefaultWindow? _window;
  private BwoinkPanel? _chatPanel;
  private bool _discordRelayActive;

  public UserAHelpUIHandler(NetUserId owner) => this._ownerId = owner;

  public bool IsAdmin => false;

  public bool IsOpen
  {
    get
    {
      DefaultWindow window = this._window;
      return window != null && !((Control) window).Disposed && ((BaseWindow) window).IsOpen;
    }
  }

  public void Receive(SharedBwoinkSystem.BwoinkTextMessage message)
  {
    this.EnsureInit(this._discordRelayActive);
    this._chatPanel.ReceiveLine(message);
    ((BaseWindow) this._window).OpenCentered();
  }

  public void Close() => ((BaseWindow) this._window)?.Close();

  public void ToggleWindow()
  {
    this.EnsureInit(this._discordRelayActive);
    if (((BaseWindow) this._window).IsOpen)
      ((BaseWindow) this._window).Close();
    else
      ((BaseWindow) this._window).OpenCentered();
  }

  public void PopOut()
  {
  }

  public void DiscordRelayChanged(bool active)
  {
    this._discordRelayActive = active;
    if (this._chatPanel == null)
      return;
    ((Control) this._chatPanel.RelayedToDiscordLabel).Visible = active;
  }

  public void PeopleTypingUpdated(BwoinkPlayerTypingUpdated args)
  {
  }

  public event Action? OnClose;

  public event Action? OnOpen;

  public Action<NetUserId, string, bool, bool>? SendMessageAction { get; set; }

  public event Action<NetUserId, string>? InputTextChanged;

  public void Open(NetUserId channelId, bool relayActive)
  {
    this.EnsureInit(relayActive);
    ((BaseWindow) this._window).OpenCentered();
  }

  private void EnsureInit(bool relayActive)
  {
    DefaultWindow window = this._window;
    if (window != null && !((Control) window).Disposed)
      return;
    this._chatPanel = new BwoinkPanel((Action<string>) (text =>
    {
      Action<NetUserId, string, bool, bool> sendMessageAction = this.SendMessageAction;
      if (sendMessageAction == null)
        return;
      sendMessageAction(this._ownerId, text, true, false);
    }));
    this._chatPanel.InputTextChanged += (Action<string>) (text =>
    {
      Action<NetUserId, string> inputTextChanged = this.InputTextChanged;
      if (inputTextChanged == null)
        return;
      inputTextChanged(this._ownerId, text);
    });
    ((Control) this._chatPanel.RelayedToDiscordLabel).Visible = relayActive;
    DefaultWindow defaultWindow = new DefaultWindow();
    defaultWindow.TitleClass = "windowTitleAlert";
    defaultWindow.HeaderClass = "windowHeaderAlert";
    defaultWindow.Title = Loc.GetString("bwoink-user-title");
    ((Control) defaultWindow).MinSize = new Vector2(500f, 300f);
    this._window = defaultWindow;
    ((BaseWindow) this._window).OnClose += (Action) (() =>
    {
      Action onClose = this.OnClose;
      if (onClose == null)
        return;
      onClose();
    });
    ((BaseWindow) this._window).OnOpen += (Action) (() =>
    {
      Action onOpen = this.OnOpen;
      if (onOpen == null)
        return;
      onOpen();
    });
    this._window.Contents.AddChild((Control) this._chatPanel);
    this.Receive(new SharedBwoinkSystem.BwoinkTextMessage(this._ownerId, SharedBwoinkSystem.SystemUserId, Loc.GetString("bwoink-system-introductory-message")));
  }

  public void Dispose()
  {
    ((Control) this._window)?.Orphan();
    this._window = (DefaultWindow) null;
    this._chatPanel = (BwoinkPanel) null;
  }
}
