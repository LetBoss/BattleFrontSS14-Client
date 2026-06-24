// Decompiled with JetBrains decompiler
// Type: Content.Client.Ame.UI.AmeControllerBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Ame.Components;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Ame.UI;

public sealed class AmeControllerBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  private AmeWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<AmeWindow>((BoundUserInterface) this);
    this._window.OnAmeButton += new Action<UiButton>(this.ButtonPressed);
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    this._window?.UpdateState(state);
  }

  public void ButtonPressed(UiButton button)
  {
    this.SendMessage((BoundUserInterfaceMessage) new UiButtonPressedMessage(button));
  }
}
