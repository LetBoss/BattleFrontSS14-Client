// Decompiled with JetBrains decompiler
// Type: Content.Client.Robotics.UI.RoboticsConsoleBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Robotics;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Robotics.UI;

public sealed class RoboticsConsoleBoundUserInterface(EntityUid owner, Enum uiKey) : 
  BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  public RoboticsConsoleWindow _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<RoboticsConsoleWindow>((BoundUserInterface) this);
    this._window.SetEntity(this.Owner);
    this._window.OnDisablePressed += (Action<string>) (address => this.SendMessage((BoundUserInterfaceMessage) new RoboticsConsoleDisableMessage(address)));
    this._window.OnDestroyPressed += (Action<string>) (address => this.SendMessage((BoundUserInterfaceMessage) new RoboticsConsoleDestroyMessage(address)));
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is RoboticsConsoleState state1))
      return;
    this._window.UpdateState(state1);
  }
}
