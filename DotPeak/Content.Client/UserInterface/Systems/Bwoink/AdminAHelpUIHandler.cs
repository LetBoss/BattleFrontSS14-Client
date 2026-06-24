// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Bwoink.AdminAHelpUIHandler
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Administration.UI.Bwoink;
using Content.Shared.Administration;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Client.UserInterface.Systems.Bwoink;

public sealed class AdminAHelpUIHandler : IAHelpUIHandler, IDisposable
{
  private readonly NetUserId _ownerId;
  private readonly Dictionary<NetUserId, BwoinkPanel> _activePanelMap = new Dictionary<NetUserId, BwoinkPanel>();
  public bool EverOpened;
  public BwoinkWindow? Window;
  public WindowRoot? WindowRoot;
  public IClydeWindow? ClydeWindow;
  public BwoinkControl? Control;

  public AdminAHelpUIHandler(NetUserId owner) => this._ownerId = owner;

  public bool IsAdmin => true;

  public bool IsOpen
  {
    get
    {
      BwoinkWindow window = this.Window;
      if (window != null && !((Robust.Client.UserInterface.Control) window).Disposed && ((BaseWindow) window).IsOpen)
        return true;
      IClydeWindow clydeWindow = this.ClydeWindow;
      return clydeWindow != null && !clydeWindow.IsDisposed;
    }
  }

  public void Receive(SharedBwoinkSystem.BwoinkTextMessage message)
  {
    this.EnsurePanel(message.UserId).ReceiveLine(message);
    this.Control?.OnBwoink(message.UserId);
  }

  private void OpenWindow()
  {
    if (this.Window == null)
      return;
    if (this.EverOpened)
      ((BaseWindow) this.Window).Open();
    else
      ((BaseWindow) this.Window).OpenCentered();
  }

  public void Close()
  {
    ((BaseWindow) this.Window)?.Close();
    if (this.ClydeWindow == null)
      return;
    this.ClydeWindow.RequestClosed -= new Action<WindowRequestClosedEventArgs>(this.OnRequestClosed);
    ((IDisposable) this.ClydeWindow).Dispose();
    if (this.Control != null)
    {
      foreach ((NetUserId _, BwoinkPanel bwoinkPanel) in this._activePanelMap)
        ((Robust.Client.UserInterface.Control) bwoinkPanel).Orphan();
      this.Control?.Orphan();
    }
    Action onClose = this.OnClose;
    if (onClose == null)
      return;
    onClose();
  }

  public void ToggleWindow()
  {
    this.EnsurePanel(this._ownerId);
    if (this.IsOpen)
      this.Close();
    else
      this.OpenWindow();
  }

  public void DiscordRelayChanged(bool active)
  {
  }

  public void PeopleTypingUpdated(BwoinkPlayerTypingUpdated args)
  {
    BwoinkPanel bwoinkPanel;
    if (!this._activePanelMap.TryGetValue(args.Channel, out bwoinkPanel))
      return;
    bwoinkPanel.UpdatePlayerTyping(args.PlayerName, args.Typing);
  }

  public event Action? OnClose;

  public event Action? OnOpen;

  public Action<NetUserId, string, bool, bool>? SendMessageAction { get; set; }

  public event Action<NetUserId, string>? InputTextChanged;

  public void Open(NetUserId channelId, bool relayActive)
  {
    this.SelectChannel(channelId);
    this.OpenWindow();
  }

  public void OnRequestClosed(WindowRequestClosedEventArgs args) => this.Close();

  private void EnsureControl()
  {
    BwoinkControl control = this.Control;
    if (control != null && !control.Disposed)
      return;
    this.Window = new BwoinkWindow();
    this.Control = this.Window.Bwoink;
    ((BaseWindow) this.Window).OnClose += (Action) (() =>
    {
      Action onClose = this.OnClose;
      if (onClose == null)
        return;
      onClose();
    });
    ((BaseWindow) this.Window).OnOpen += (Action) (() =>
    {
      Action onOpen = this.OnOpen;
      if (onOpen != null)
        onOpen();
      this.EverOpened = true;
    });
    foreach ((NetUserId _, BwoinkPanel bwoinkPanel) in this._activePanelMap)
    {
      if (!((Robust.Client.UserInterface.Control) this.Control.BwoinkArea).Children.Contains((Robust.Client.UserInterface.Control) bwoinkPanel))
        ((Robust.Client.UserInterface.Control) this.Control.BwoinkArea).AddChild((Robust.Client.UserInterface.Control) bwoinkPanel);
      ((Robust.Client.UserInterface.Control) bwoinkPanel).Visible = false;
    }
  }

  public void HideAllPanels()
  {
    foreach (Robust.Client.UserInterface.Control control in this._activePanelMap.Values)
      control.Visible = false;
  }

  public BwoinkPanel EnsurePanel(NetUserId channelId)
  {
    this.EnsureControl();
    BwoinkPanel bwoinkPanel1;
    if (this._activePanelMap.TryGetValue(channelId, out bwoinkPanel1))
      return bwoinkPanel1;
    BwoinkPanel bwoinkPanel2;
    this._activePanelMap[channelId] = bwoinkPanel2 = new BwoinkPanel((Action<string>) (text =>
    {
      Action<NetUserId, string, bool, bool> sendMessageAction = this.SendMessageAction;
      if (sendMessageAction == null)
        return;
      NetUserId netUserId = channelId;
      string str = text;
      BwoinkWindow window1 = this.Window;
      int num1 = window1 != null ? (((BaseButton) window1.Bwoink.PlaySound).Pressed ? 1 : 0) : 1;
      BwoinkWindow window2 = this.Window;
      int num2 = window2 != null ? (((BaseButton) window2.Bwoink.AdminOnly).Pressed ? 1 : 0) : 0;
      sendMessageAction(netUserId, str, num1 != 0, num2 != 0);
    }));
    bwoinkPanel2.InputTextChanged += (Action<string>) (text =>
    {
      Action<NetUserId, string> inputTextChanged = this.InputTextChanged;
      if (inputTextChanged == null)
        return;
      inputTextChanged(channelId, text);
    });
    ((Robust.Client.UserInterface.Control) bwoinkPanel2).Visible = false;
    if (!((Robust.Client.UserInterface.Control) this.Control.BwoinkArea).Children.Contains((Robust.Client.UserInterface.Control) bwoinkPanel2))
      ((Robust.Client.UserInterface.Control) this.Control.BwoinkArea).AddChild((Robust.Client.UserInterface.Control) bwoinkPanel2);
    return bwoinkPanel2;
  }

  public bool TryGetChannel(NetUserId ch, [NotNullWhen(true)] out BwoinkPanel? bp)
  {
    return this._activePanelMap.TryGetValue(ch, out bp);
  }

  private void SelectChannel(NetUserId uid)
  {
    this.EnsurePanel(uid);
    this.Control.SelectChannel(uid);
  }

  public void Dispose()
  {
    ((Robust.Client.UserInterface.Control) this.Window)?.Orphan();
    this.Window = (BwoinkWindow) null;
    this.Control = (BwoinkControl) null;
    this._activePanelMap.Clear();
    this.EverOpened = false;
  }
}
