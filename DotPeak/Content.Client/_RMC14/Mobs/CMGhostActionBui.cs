// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Mobs.CMGhostActionBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Message;
using Content.Shared._RMC14.Mobs;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using System;

#nullable enable
namespace Content.Client._RMC14.Mobs;

public sealed class CMGhostActionBui(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private CMGhostActionWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    if (this._window != null)
      return;
    this._window = BoundUserInterfaceExt.CreateWindow<CMGhostActionWindow>((BoundUserInterface) this);
    this._window.Text.SetMarkupPermissive(Loc.GetString("cm-ghost-window-text"));
    ((BaseButton) this._window.Stay).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.Close());
    ((BaseButton) this._window.Ghost).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new CMGhostActionBuiMsg()));
  }
}
