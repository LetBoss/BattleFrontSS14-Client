// Decompiled with JetBrains decompiler
// Type: Content.Client.Disposal.Tube.DisposalRouterBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Disposal.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Disposal.Tube;

public sealed class DisposalRouterBoundUserInterface(EntityUid owner, Enum uiKey) : 
  BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private DisposalRouterWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<DisposalRouterWindow>((BoundUserInterface) this);
    ((BaseButton) this._window.Confirm).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.ButtonPressed(SharedDisposalRouterComponent.UiAction.Ok, this._window.TagInput.Text));
    this._window.TagInput.OnTextEntered += (Action<LineEdit.LineEditEventArgs>) (args => this.ButtonPressed(SharedDisposalRouterComponent.UiAction.Ok, args.Text));
  }

  private void ButtonPressed(SharedDisposalRouterComponent.UiAction action, string tag)
  {
    this.SendMessage((BoundUserInterfaceMessage) new SharedDisposalRouterComponent.UiActionMessage(action, tag));
    this.Close();
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is SharedDisposalRouterComponent.DisposalRouterUserInterfaceState state1))
      return;
    this._window?.UpdateState(state1);
  }
}
