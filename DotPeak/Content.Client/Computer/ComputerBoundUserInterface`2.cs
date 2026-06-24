// Decompiled with JetBrains decompiler
// Type: Content.Client.Computer.ComputerBoundUserInterface`2
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Computer;

[Virtual]
public class ComputerBoundUserInterface<TWindow, TState>(EntityUid owner, Enum uiKey) : 
  ComputerBoundUserInterfaceBase(owner, uiKey)
  where TWindow : BaseWindow, IComputerWindow<TState>, new()
  where TState : BoundUserInterfaceState
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private TWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<TWindow>((BoundUserInterface) this);
    this._window.SetupComputerWindow((ComputerBoundUserInterfaceBase) this);
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if ((object) this._window == null)
      return;
    this._window.UpdateState((TState) state);
  }

  protected virtual void ReceiveMessage(BoundUserInterfaceMessage message)
  {
    this._window?.ReceiveMessage(message);
  }
}
