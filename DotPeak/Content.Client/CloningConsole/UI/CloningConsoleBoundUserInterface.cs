// Decompiled with JetBrains decompiler
// Type: Content.Client.CloningConsole.UI.CloningConsoleBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Cloning.CloningConsole;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using System;

#nullable enable
namespace Content.Client.CloningConsole.UI;

public sealed class CloningConsoleBoundUserInterface(EntityUid owner, Enum uiKey) : 
  BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private CloningConsoleWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<CloningConsoleWindow>((BoundUserInterface) this);
    this._window.Title = Loc.GetString("cloning-console-window-title");
    ((BaseButton) this._window.CloneButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((BoundUserInterfaceMessage) new UiButtonPressedMessage(UiButton.Clone)));
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    this._window?.Populate((CloningConsoleBoundUserInterfaceState) state);
  }
}
