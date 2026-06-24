// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Admin.AdminUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Administration.Managers;
using Content.Client.Administration.Systems;
using Content.Client.Administration.UI;
using Content.Client.Administration.UI.Tabs.ObjectsTab;
using Content.Client.Administration.UI.Tabs.PanicBunkerTab;
using Content.Client.Administration.UI.Tabs.PlayerTab;
using Content.Client.Gameplay;
using Content.Client.Lobby;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Content.Client.Verbs.UI;
using Content.Shared.Administration;
using Content.Shared.Administration.Events;
using Content.Shared.Input;
using Robust.Client.Console;
using Robust.Client.Input;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.UserInterface.Systems.Admin;

public sealed class AdminUIController : 
  UIController,
  IOnStateEntered<GameplayState>,
  IOnStateEntered<LobbyState>,
  IOnSystemChanged<AdminSystem>,
  IOnSystemLoaded<AdminSystem>,
  IOnSystemUnloaded<AdminSystem>
{
  [Dependency]
  private IClientAdminManager _admin;
  [Dependency]
  private IClientConGroupController _conGroups;
  [Dependency]
  private IClientConsoleHost _conHost;
  [Dependency]
  private IInputManager _input;
  [Dependency]
  private VerbMenuUIController _verb;
  private AdminMenuWindow? _window;
  private PanicBunkerStatus? _panicBunker;

  private MenuButton? AdminButton
  {
    get => this.UIManager.GetActiveUIWidgetOrNull<GameTopMenuBar>()?.AdminButton;
  }

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<PanicBunkerChangedEvent>(new EntitySessionEventHandler<PanicBunkerChangedEvent>(this.OnPanicBunkerUpdated), (Type[]) null, (Type[]) null);
  }

  private void OnPanicBunkerUpdated(PanicBunkerChangedEvent msg, EntitySessionEventArgs args)
  {
    int num = this._panicBunker != null ? 0 : (msg.Status.Enabled ? 1 : 0);
    this._panicBunker = msg.Status;
    this._window?.PanicBunkerControl.UpdateStatus(msg.Status);
    if (num == 0)
      return;
    ((BaseWindow) this.UIManager.CreateWindow<PanicBunkerStatusWindow>()).OpenCentered();
  }

  public void OnStateEntered(GameplayState state)
  {
    this.EnsureWindow();
    this.AdminStatusUpdated();
  }

  public void OnStateEntered(LobbyState state)
  {
    this.EnsureWindow();
    this.AdminStatusUpdated();
  }

  public void OnSystemLoaded(AdminSystem system)
  {
    this.EnsureWindow();
    this._admin.AdminStatusUpdated += new Action(this.AdminStatusUpdated);
    // ISSUE: method pointer
    this._input.SetInputCommand(ContentKeyFunctions.OpenAdminMenu, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003COnSystemLoaded\u003Eb__13_0)), (StateInputCmdDelegate) null, true, true));
  }

  public void OnSystemUnloaded(AdminSystem system)
  {
    if (this._window != null)
      ((Control) this._window).Orphan();
    this._admin.AdminStatusUpdated -= new Action(this.AdminStatusUpdated);
    CommandBinds.Unregister<AdminUIController>();
  }

  private void EnsureWindow()
  {
    AdminMenuWindow window1 = this._window;
    if (window1 != null && !((Control) window1).Disposed)
      return;
    AdminMenuWindow window2 = this._window;
    if ((window2 != null ? (((Control) window2).Disposed ? 1 : 0) : 0) != 0)
      this.OnWindowDisposed();
    this._window = this.UIManager.CreateWindow<AdminMenuWindow>();
    LayoutContainer.SetAnchorPreset((Control) this._window, (LayoutContainer.LayoutPreset) 8, false);
    if (this._panicBunker != null)
      this._window.PanicBunkerControl.UpdateStatus(this._panicBunker);
    this._window.PlayerTabControl.OnEntryKeyBindDown += new Action<GUIBoundKeyEventArgs, ListData>(this.PlayerTabEntryKeyBindDown);
    this._window.ObjectsTabControl.OnEntryKeyBindDown += new Action<GUIBoundKeyEventArgs, ListData>(this.ObjectsTabEntryKeyBindDown);
    ((BaseWindow) this._window).OnOpen += new Action(this.OnWindowOpen);
    ((BaseWindow) this._window).OnClose += new Action(this.OnWindowClosed);
    this._window.OnDisposed += new Action(this.OnWindowDisposed);
  }

  public void UnloadButton()
  {
    if (this.AdminButton == null)
      return;
    ((BaseButton) this.AdminButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.AdminButtonPressed);
  }

  public void LoadButton()
  {
    if (this.AdminButton == null)
      return;
    ((BaseButton) this.AdminButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.AdminButtonPressed);
  }

  private void OnWindowOpen() => ((BaseButton) this.AdminButton)?.SetClickPressed(true);

  private void OnWindowClosed() => ((BaseButton) this.AdminButton)?.SetClickPressed(false);

  private void OnWindowDisposed()
  {
    if (this.AdminButton != null)
      ((BaseButton) this.AdminButton).Pressed = false;
    if (this._window == null)
      return;
    this._window.PlayerTabControl.OnEntryKeyBindDown -= new Action<GUIBoundKeyEventArgs, ListData>(this.PlayerTabEntryKeyBindDown);
    this._window.ObjectsTabControl.OnEntryKeyBindDown -= new Action<GUIBoundKeyEventArgs, ListData>(this.ObjectsTabEntryKeyBindDown);
    ((BaseWindow) this._window).OnOpen -= new Action(this.OnWindowOpen);
    ((BaseWindow) this._window).OnClose -= new Action(this.OnWindowClosed);
    this._window.OnDisposed -= new Action(this.OnWindowDisposed);
    this._window = (AdminMenuWindow) null;
  }

  private void AdminStatusUpdated()
  {
    if (this.AdminButton == null)
      return;
    ((Control) this.AdminButton).Visible = ((IClientConGroupImplementation) this._conGroups).CanAdminMenu();
  }

  private void AdminButtonPressed(BaseButton.ButtonEventArgs args) => this.Toggle();

  private void Toggle()
  {
    AdminMenuWindow window = this._window;
    if (window != null && ((BaseWindow) window).IsOpen)
    {
      ((BaseWindow) this._window).Close();
    }
    else
    {
      if (!((IClientConGroupImplementation) this._conGroups).CanAdminMenu())
        return;
      ((BaseWindow) this._window)?.Open();
    }
  }

  private void PlayerTabEntryKeyBindDown(GUIBoundKeyEventArgs args, ListData? data)
  {
    PlayerListData playerListData = data as PlayerListData;
    if ((object) playerListData == null)
      return;
    PlayerInfo info = playerListData.Info;
    if (!info.NetEntity.HasValue)
      return;
    NetEntity target = info.NetEntity.Value;
    BoundKeyFunction function = ((BoundKeyEventArgs) args).Function;
    if (BoundKeyFunction.op_Equality(function, EngineKeyFunctions.UIClick))
    {
      ((IConsoleHost) this._conHost).ExecuteCommand($"vv {target}");
    }
    else
    {
      if (!BoundKeyFunction.op_Equality(function, EngineKeyFunctions.UIRightClick))
        return;
      this._verb.OpenVerbMenu(target, true);
    }
    ((BoundKeyEventArgs) args).Handle();
  }

  private void ObjectsTabEntryKeyBindDown(GUIBoundKeyEventArgs args, ListData? data)
  {
    ObjectsListData objectsListData = data as ObjectsListData;
    if ((object) objectsListData == null)
      return;
    NetEntity entity = objectsListData.Info.Entity;
    BoundKeyFunction function = ((BoundKeyEventArgs) args).Function;
    if (BoundKeyFunction.op_Equality(function, EngineKeyFunctions.UIClick))
    {
      ((IConsoleHost) this._conHost).ExecuteCommand($"vv {entity}");
    }
    else
    {
      if (!BoundKeyFunction.op_Equality(function, EngineKeyFunctions.UIRightClick))
        return;
      this._verb.OpenVerbMenu(entity, true);
    }
    ((BoundKeyEventArgs) args).Handle();
  }
}
