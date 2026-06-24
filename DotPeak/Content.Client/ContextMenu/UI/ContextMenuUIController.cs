// Decompiled with JetBrains decompiler
// Type: Content.Client.ContextMenu.UI.ContextMenuUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.CombatMode;
using Content.Client.Gameplay;
using Content.Client.Mapping;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;

#nullable enable
namespace Content.Client.ContextMenu.UI;

public sealed class ContextMenuUIController : 
  UIController,
  IOnStateEntered<GameplayState>,
  IOnStateExited<GameplayState>,
  IOnSystemChanged<CombatModeSystem>,
  IOnSystemLoaded<CombatModeSystem>,
  IOnSystemUnloaded<CombatModeSystem>,
  IOnStateEntered<MappingState>,
  IOnStateExited<MappingState>
{
  public static readonly TimeSpan HoverDelay = TimeSpan.FromSeconds(0.2);
  public ContextMenuPopup RootMenu;
  public CancellationTokenSource? CancelOpen;
  public CancellationTokenSource? CancelClose;
  public Action? OnContextClosed;
  public Action<ContextMenuElement>? OnContextMouseEntered;
  public Action<ContextMenuElement>? OnContextMouseExited;
  public Action<ContextMenuElement>? OnSubMenuOpened;
  public Action<ContextMenuElement, GUIBoundKeyEventArgs>? OnContextKeyEvent;
  private bool _setup;

  public Stack<ContextMenuPopup> Menus { get; } = new Stack<ContextMenuPopup>();

  public void OnStateEntered(GameplayState state) => this.Setup();

  public void OnStateExited(GameplayState state) => this.Shutdown();

  public void OnStateEntered(MappingState state) => this.Setup();

  public void OnStateExited(MappingState state) => this.Shutdown();

  public void Setup()
  {
    if (this._setup)
      return;
    this._setup = true;
    this.RootMenu = new ContextMenuPopup(this, (ContextMenuElement) null);
    this.RootMenu.OnPopupHide += new Action(this.Close);
    this.Menus.Push(this.RootMenu);
  }

  public void Shutdown()
  {
    if (!this._setup)
      return;
    this._setup = false;
    this.Close();
    this.RootMenu.OnPopupHide -= new Action(this.Close);
    ((Control) this.RootMenu).Orphan();
    this.RootMenu = (ContextMenuPopup) null;
  }

  public void Close()
  {
    ((Control) this.RootMenu.MenuBody).DisposeAllChildren();
    this.CancelOpen?.Cancel();
    this.CancelClose?.Cancel();
    Action onContextClosed = this.OnContextClosed;
    if (onContextClosed != null)
      onContextClosed();
    this.RootMenu.Close();
  }

  public void CloseSubMenus(ContextMenuPopup? menu)
  {
    if (menu == null || !((Control) menu).Visible)
      return;
    ContextMenuPopup result;
    while (this.Menus.TryPeek(out result) && result != menu)
      this.Menus.Pop().Close();
    this.CancelClose?.Cancel();
    this.CancelClose = (CancellationTokenSource) null;
  }

  private void OnMouseEntered(ContextMenuElement element)
  {
    ContextMenuPopup result;
    if (!this.Menus.TryPeek(out result))
    {
      this.Log.Error("Context Menu: Mouse entered menu without any open menus?");
    }
    else
    {
      if (element.ParentMenu == result || element.SubMenu == result)
        this.CancelClose?.Cancel();
      if (element.SubMenu == result)
        return;
      this.CancelOpen?.Cancel();
      this.CancelOpen = new CancellationTokenSource();
      Timer.Spawn(ContextMenuUIController.HoverDelay, (Action) (() => this.OpenSubMenu(element)), this.CancelOpen.Token);
    }
  }

  private void OnMouseExited(ContextMenuElement element)
  {
    this.CancelOpen?.Cancel();
    if (element.SubMenu == null)
      return;
    this.CancelClose?.Cancel();
    this.CancelClose = new CancellationTokenSource();
    Timer.Spawn(ContextMenuUIController.HoverDelay, (Action) (() => this.CloseSubMenus(element.ParentMenu)), this.CancelClose.Token);
    Action<ContextMenuElement> contextMouseExited = this.OnContextMouseExited;
    if (contextMouseExited == null)
      return;
    contextMouseExited(element);
  }

  private void OnKeyBindDown(ContextMenuElement element, GUIBoundKeyEventArgs args)
  {
    Action<ContextMenuElement, GUIBoundKeyEventArgs> onContextKeyEvent = this.OnContextKeyEvent;
    if (onContextKeyEvent == null)
      return;
    onContextKeyEvent(element, args);
  }

  public void OpenSubMenu(ContextMenuElement element)
  {
    ContextMenuPopup result;
    if (!this.Menus.TryPeek(out result))
    {
      this.Log.Error("Context Menu: Attempting to open sub menu without any open menus?");
    }
    else
    {
      if (element.SubMenu == result || ((Control) element).Disposed || element.ParentMenu == null || !((Control) element.ParentMenu).Visible)
        return;
      this.CloseSubMenus(element.ParentMenu);
      this.CancelOpen?.Cancel();
      this.CancelOpen = (CancellationTokenSource) null;
      if (element.SubMenu == null)
        return;
      Vector2 globalPosition = ((Control) element).GlobalPosition;
      Vector2 vector2 = globalPosition + new Vector2(((Control) element).Width + 4f, -4f);
      element.SubMenu.Open(new UIBox2?(UIBox2.FromDimensions(vector2, new Vector2(1f, 1f))), new Vector2?(globalPosition), new Vector2?());
      ((Control) element.SubMenu).SetPositionLast();
      this.Menus.Push(element.SubMenu);
      Action<ContextMenuElement> onSubMenuOpened = this.OnSubMenuOpened;
      if (onSubMenuOpened == null)
        return;
      onSubMenuOpened(element);
    }
  }

  public void AddElement(ContextMenuPopup menu, ContextMenuElement element)
  {
    ((Control) element).OnMouseEntered += (Action<GUIMouseHoverEventArgs>) (_ => this.OnMouseEntered(element));
    ((Control) element).OnMouseExited += (Action<GUIMouseHoverEventArgs>) (_ => this.OnMouseExited(element));
    ((Control) element).OnKeyBindDown += (Action<GUIBoundKeyEventArgs>) (args => this.OnKeyBindDown(element, args));
    element.ParentMenu = menu;
    ((Control) menu.MenuBody).AddChild((Control) element);
    ((Control) menu).InvalidateMeasure();
  }

  public void OnRemoveElement(ContextMenuPopup menu, Control control)
  {
    ContextMenuElement element = control as ContextMenuElement;
    if (element == null)
      return;
    ((Control) element).OnMouseEntered -= (Action<GUIMouseHoverEventArgs>) (_ => this.OnMouseEntered(element));
    ((Control) element).OnMouseExited -= (Action<GUIMouseHoverEventArgs>) (_ => this.OnMouseExited(element));
    ((Control) element).OnKeyBindDown -= (Action<GUIBoundKeyEventArgs>) (args => this.OnKeyBindDown(element, args));
    ((Control) menu).InvalidateMeasure();
  }

  private void OnCombatModeUpdated(bool inCombatMode)
  {
    if (!inCombatMode)
      return;
    this.Close();
  }

  public void OnSystemLoaded(CombatModeSystem system)
  {
    system.LocalPlayerCombatModeUpdated += new Action<bool>(this.OnCombatModeUpdated);
  }

  public void OnSystemUnloaded(CombatModeSystem system)
  {
    system.LocalPlayerCombatModeUpdated -= new Action<bool>(this.OnCombatModeUpdated);
  }
}
