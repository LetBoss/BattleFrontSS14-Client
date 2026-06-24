// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Marines.Lifeboat.LifeboatComputerBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Evacuation;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._RMC14.Marines.Lifeboat;

public sealed class LifeboatComputerBui(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  private LifeboatComputerWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<LifeboatComputerWindow>((BoundUserInterface) this);
    ((BaseButton) this._window.EmergencyLaunchButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new LifeboatComputerLaunchBuiMsg()));
    ((BaseButton) this._window.NoButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => ((BaseWindow) this._window).Close());
    ((BaseButton) this._window.YesButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new LifeboatComputerLaunchBuiMsg()));
  }
}
